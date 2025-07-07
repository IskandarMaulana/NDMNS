const DOWNTIME_UPDATE_INTERVAL = 60 * 60 * 1000;

const downtimeUpdateSentCount = {};

var networksData = null;
var settingsData = null;
var currentGroupBy = "";
var lastNetworkStatus = {};
var networkDownStartTime = {};

const connection = new signalR.HubConnectionBuilder()
  .withUrl("https://localhost:44318/hubs/monitoring")
  .configureLogging(signalR.LogLevel.Information)
  .build();

connection.onclose(async () => {
  console.log("SignalR Disconnected. Trying to reconnect...");
  await startConnection();
});

connection.on("Ping", function (networks) {
  console.log("Receive Ping data.....", new Date(), networks);
  renderNetworks(networks);
  checkNetworkStatusChanges(networks);
  networksData = networks;
});

function fetchSettings() {
  $.ajax({
    url: "https://localhost:44318/api/Setting",
    type: "GET",
    dataType: "json",
    success: function (response) {
      settingsData = response.data;
      populateSettingsForm(settingsData);
    },
    error: function (xhr, status, error) {
      console.error("Error fetching settings data:", error);
    },
  });
}

function updateSetting(setting) {
  return new Promise((resolve, reject) => {
    $.ajax({
      url: `https://localhost:44318/api/Setting/${setting.id}`,
      type: "PUT",
      contentType: "application/json",
      headers: {
        "X-User-Id": "@ViewBag.UserId;",
      },
      data: JSON.stringify(setting),
      success: function (response) {
        showAlert("success", response.message);
        resolve(true);
      },
      error: function (xhr, status, error) {
        console.error(`Error saving setting ${setting.code}:`, error);
        showAlert("danger", error.message);
        reject(false);
      },
    });
  });
}

function showAlert(type, message) {
  $(".alert").remove();

  const alertHtml = `
    <div class="alert alert-${type} alert-dismissible fade show" role="alert">
      ${message}
      <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    </div>
  `;

  $("#settingsPanel").after(alertHtml);

  setTimeout(function () {
    $(".alert").fadeOut("slow");
  }, 5000);
}

function fetchNetworkData() {
  $.ajax({
    url: "https://localhost:44318/api/Network",
    type: "GET",
    dataType: "json",
    success: function (data) {
      networksData = data.data;
      initializeNetworkStatus(networksData);
      renderNetworks(networksData);
    },
    error: function (xhr, status, error) {
      console.error("Error fetching network data:", error);
    },
  });
}

async function startConnection() {
  try {
    await connection.start();
  } catch (err) {
    setTimeout(startConnection, 5000);
  }
}

function populateSettingsForm(settings) {
  if (!settings) return;

  const pingAttemptsData = settings.find((s) => s.code === "PING_ATTEMPTS");
  if (pingAttemptsData) {
    $("#pingAttempts").val(pingAttemptsData.value);
  }

  const pingTimeoutData = settings.find((s) => s.code === "PING_TIMEOUT");
  if (pingTimeoutData) {
    $("#pingTimeout").val(pingTimeoutData.value);
  }
}

function initializeNetworkStatus(networks) {
  if (!networks) return;

  networks.forEach((network) => {
    const networkId = network.id;
    lastNetworkStatus[networkId] = network.status;

    if (network.status === 1) {
      const lastUpdateTimestamp = new Date(network.lastUpdate);
      networkDownStartTime[networkId] = lastUpdateTimestamp;
    }
  });
}

function checkNetworkStatusChanges(networks) {
  if (!networks) return;

  networks.forEach((network) => {
    const networkId = network.id;
    const currentStatus = network.status;
    const previousStatus = lastNetworkStatus[networkId];

    if (previousStatus === undefined) {
      lastNetworkStatus[networkId] = currentStatus;
      if (currentStatus === 1) {
        const lastUpdateTimestamp = new Date(network.lastUpdate);
        networkDownStartTime[networkId] = lastUpdateTimestamp;
      }
      return;
    }

    if (previousStatus !== currentStatus) {
      console.log(`Network ${network.name} (${networkId}) status changed from ${getStatusText(previousStatus)} to ${getStatusText(currentStatus)}`);

      // Down -> Up (1 -> 2)
      if ((previousStatus === 1 && currentStatus === 2) || (previousStatus === 3 && currentStatus === 2)) {
        sendUpNotification(network);
        delete networkDownStartTime[networkId];
      }
      // Up -> Down (2 -> 1)
      else if ((previousStatus === 2 && currentStatus === 1) || (previousStatus === 2 && currentStatus === 3)) {
        sendDownNotification(network);
        networkDownStartTime[networkId] = new Date().getTime();
      }

      lastNetworkStatus[networkId] = currentStatus;
    }
  });
}

function checkProlongedDowntime() {
  const currentTime = new Date();

  for (const networkId in networkDownStartTime) {
    if (!networkExists(networkId)) continue;

    const downStartTime = networkDownStartTime[networkId];
    const downDuration = currentTime - downStartTime;

    const intervalPassed = Math.floor(downDuration / DOWNTIME_UPDATE_INTERVAL);
    const sentCount = downtimeUpdateSentCount[networkId] || 0;

    if (intervalPassed > sentCount) {
      const network = findNetworkById(networkId);
      if (network) {
        sendUpdateDowntimeNotification(network, Math.floor(downDuration / DOWNTIME_UPDATE_INTERVAL));
        downtimeUpdateSentCount[networkId] = intervalPassed;
      }
    }
  }
}

function captureNetworkCardAsImage(networkId) {
  return new Promise((resolve, reject) => {
    try {
      const networkCard = $(`#network${networkId}`)[0];

      if (!networkCard) {
        console.error(`Network card element for ID ${networkId} not found`);
        reject("Network card element not found");
        return;
      }

      html2canvas(networkCard, {
        backgroundColor: null,
        scale: 2,
        logging: false,
      })
        .then((canvas) => {
          const base64Image = canvas.toDataURL("image/png");
          resolve(base64Image);
        })
        .catch((error) => {
          console.error("Error capturing network card as image:", error);
          reject(error);
        });
    } catch (error) {
      console.error("Error in captureNetworkCardAsImage:", error);
      reject(error);
    }
  });
}

function sendUpNotification(network) {
  captureNetworkCardAsImage(network.id)
    .then((imageBase64) => {
      $.ajax({
        url: "https://localhost:44318/api/WhatsApp/alertUptime",
        type: "POST",
        contentType: "application/json",
        headers: {
          "X-User-Id": "@ViewBag.UserId;",
        },
        data: JSON.stringify({
          networkId: network.id,
          siteId: network.siteId,
          ispId: network.ispId,
          status: network.status,
          image: imageBase64,
        }),
        success: function (response) {
          console.log(`Up notification sent for ${network.name}`);
          showAlert("success", `${response.message} -  ${network.name}`);
        },
        error: function (xhr, status, error) {
          console.error(`Error sending Up notification for ${network.name}:`, error);
          showAlert("danger", `${error.message} -  ${network.name}`);
        },
      });
    })
    .catch((error) => {
      console.error(`Failed to capture image for ${network.name}:`, error);
    });
}

function sendDownNotification(network) {
  captureNetworkCardAsImage(network.id)
    .then((imageBase64) => {
      $.ajax({
        url: "https://localhost:44318/api/WhatsApp/alertDowntime",
        type: "POST",
        contentType: "application/json",
        headers: {
          "X-User-Id": "@ViewBag.UserId;",
        },
        data: JSON.stringify({
          networkId: network.id,
          siteId: network.siteId,
          ispId: network.ispId,
          status: network.status,
          image: imageBase64,
        }),
        success: function (response) {
          console.log(`Down notification sent for ${network.name}`);
          showAlert("success", `${response.message} -  ${network.name}`);
        },
        error: function (xhr, status, error) {
          console.error(`Error sending Down notification for ${network.name}:`, error);
          showAlert("danger", `${error.message} -  ${network.name}`);
        },
      });
    })
    .catch((error) => {
      console.error(`Failed to capture image for ${network.name}:`, error);
    });
}

function sendUpdateDowntimeNotification(network, hoursDown) {
  $.ajax({
    url: "https://localhost:44318/api/WhatsApp/alertUpdateDowntime",
    type: "POST",
    contentType: "application/json",
    headers: {
      "X-User-Id": "@ViewBag.UserId;",
    },
    data: JSON.stringify({
      networkId: network.id,
      siteId: network.siteId,
      ispId: network.ispId,
      status: network.status,
      image: "",
    }),
    success: function (response) {
      console.log(`Update downtime notification sent for ${network.name} (${hoursDown} hours)`);
      showAlert("success", `${response.message} -  ${network.name}`);
    },
    error: function (xhr, status, error) {
      console.error(`Error sending update downtime notification for ${network.name}:`, error);
      showAlert("danger", `${error.message} -  ${network.name}`);
    },
  });
}

function networkExists(networkId) {
  if (!networksData) return false;
  return networksData.some((network) => network.id === networkId);
}

function findNetworkById(networkId) {
  if (!networksData) return null;
  return networksData.find((network) => network.id === networkId);
}

function groupNetworksByField(networks, groupByField) {
  if (!groupByField || !networks) return { "All Networks": networks };

  const grouped = {};

  networks.forEach((network) => {
    let groupKey;
    if (groupByField === "location") {
      groupKey = network.siteLocation || "Unknown Location";
    } else if (groupByField === "site") {
      groupKey = network.siteName || "Unknown Site";
    } else if (groupByField === "isp") {
      groupKey = network.ispName || "Unknown ISP";
    } else if (groupByField === "location-isp") {
      const location = network.siteLocation || "Unknown Location";
      const isp = network.ispName || "Unknown ISP";
      groupKey = `${location} - ${isp}`;
    }

    if (!grouped[groupKey]) {
      grouped[groupKey] = [];
    }
    grouped[groupKey].push(network);
  });

  return grouped;
}

function renderNetworks(networks) {
  if (networks == null) {
    console.log("No networks data");
    return;
  }

  const container = document.getElementById("network-grid");
  container.innerHTML = "";

  const groupedNetworks = groupNetworksByField(networks, currentGroupBy);

  let cardSizeClass = "col-lg-2 col-md-3";
  let extraClass = "";
  const count = networks.length;

  if (count > 80) {
    cardSizeClass = "col-lg-1 col-md-2 col-sm-3 col-4";
    extraClass = "compact-xl";
  } else if (count > 60) {
    cardSizeClass = "col-lg-1 col-md-2 col-sm-3 col-4";
  } else if (count > 40) {
    cardSizeClass = "col-lg-1 col-md-2 col-sm-3 col-6";
  } else if (count > 20) {
    cardSizeClass = "col-lg-2 col-md-3 col-sm-4 col-6";
  } else if (count <= 10) {
    cardSizeClass = "col-lg-3 col-md-4 col-sm-6";
  }

  cleanupTooltips();

  Object.keys(groupedNetworks).forEach((groupName) => {
    const groupNetworks = groupedNetworks[groupName];

    if (currentGroupBy) {
      let headerIcon = "collection";
      if (currentGroupBy === "location") {
        headerIcon = "geo-alt";
      } else if (currentGroupBy === "site") {
        headerIcon = "building";
      } else if (currentGroupBy === "isp") {
        headerIcon = "router";
      } else if (currentGroupBy === "location-isp") {
        headerIcon = "diagram-3";
      }

      const groupHeader = document.createElement("div");
      groupHeader.className = "col-12 mb-2";
      groupHeader.innerHTML = `
                        <h5 class="group-header bg-secondary text-white p-2 rounded">
                            <i class="bi bi-${headerIcon}-fill me-2"></i>
                            ${groupName} (${groupNetworks.length} networks)
                        </h5>
                    `;
      container.appendChild(groupHeader);
    }

    const gridContainer = document.createElement("div");
    gridContainer.className = `row g-2 ${extraClass} mb-3`;

    groupNetworks.forEach((network) => {
      const statusClass = getStatusClass(network.status);
      const statusText = getStatusText(network.status);
      const lastUpdateTime = formatLastUpdate(network.lastUpdate);
      const iconColor = statusClass;

      const networkCard = document.createElement("div");
      networkCard.className = cardSizeClass;
      networkCard.innerHTML = `
                        <div id="network${network.id}" class="network-card ${statusClass}" 
                                data-network-id="${network.id}"
                                data-bs-toggle="tooltip"
                                data-bs-html="true"
                                data-bs-title="<strong>Name: </strong> ${network.name}<br>
                                            <strong>IP: </strong> ${network.ip}<br>
                                            <strong>Latency: </strong> ${network.latency.toFixed(1)} ms<br>
                                            <strong>Last Update: </strong> ${formatDateTime(network.lastUpdate)}">
                            <div class="server-icon">
                                <i class="bi bi-pc-display ${iconColor}" style="font-size: 1.5rem;" alt="Server"></i>
                            </div>
                            <div class="network-info">
                                <h5 class="site-heading">${network.name}</h5>
                                <div class="last-update">${lastUpdateTime}</div>
                            </div>
                        </div>
                    `;
      gridContainer.appendChild(networkCard);
    });

    container.appendChild(gridContainer);
  });

  $('[data-bs-toggle="tooltip"]').tooltip({ html: true });
}

function cleanupTooltips() {
  if ($("[data-bs-toggle='tooltip']").length > 0) {
    $("[data-bs-toggle='tooltip']").tooltip("dispose");
  }
}

function getStatusClass(status) {
  switch (status) {
    case 1:
      return "offline";
    case 2:
      return "online";
    case 3:
      return "intermittent";
    default:
      return "unknown";
  }
}

function getStatusText(status) {
  switch (status) {
    case 1:
      return "Offline";
    case 2:
      return "Online";
    case 3:
      return "Intermittent";
    default:
      return "Unknown";
  }
}

function formatDateTime(dateString) {
  const d = new Date(dateString);
  const day = String(d.getDate()).padStart(2, "0");
  const month = String(d.getMonth() + 1).padStart(2, "0");
  const year = d.getFullYear();
  const hours = String(d.getHours()).padStart(2, "0");
  const minutes = String(d.getMinutes()).padStart(2, "0");
  const seconds = String(d.getSeconds()).padStart(2, "0");
  return `${day}-${month}-${year} ${hours}:${minutes}:${seconds}`;
}

function formatLastUpdate(dateString) {
  const date = new Date(dateString);
  const now = new Date();
  const diffMs = now - date;
  const diffMins = Math.floor(diffMs / 60000);

  if (diffMins < 60) {
    return `${diffMins}m ago`;
  } else if (diffMins < 1440) {
    // < 1 day
    const diffHours = Math.floor(diffMins / 60);
    const remainingMins = diffMins % 60;
    return `${diffHours}h:${remainingMins}m ago`;
  } else {
    const diffDays = Math.floor(diffMins / 1440);
    const remainingMinutes = diffMins % 1440;
    const diffHours = Math.floor(remainingMinutes / 60);
    const remainingMins = remainingMinutes % 60;
    return `${diffDays}d:${diffHours}h:${remainingMins}m ago`;
  }
}
