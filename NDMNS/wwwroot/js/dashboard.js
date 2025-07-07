const connection = new signalR.HubConnectionBuilder()
  .withUrl("https://localhost:44318/hubs/monitoring")
  .configureLogging(signalR.LogLevel.Information)
  .build();

async function startConnection() {
  try {
    await connection.start();
  } catch (err) {
    setTimeout(startConnection, 5000);
  }
}

connection.onclose(async () => {
  await startConnection();
});


connection.on("SystemHealth", function (systemhealth) {
    updateSystemHealthCards(systemhealth);
});

connection.on("Ping", function (networks) {
    console.log(networks);
});

function updateSystemHealthCards(systemHealth) {
    if (!systemHealth) return;
    
    if (systemHealth.database) {
        updateDatabaseCard(systemHealth.database);
    }
    
    if (systemHealth.internet) {
        updateInternetCard(systemHealth.internet);
    }
    
    if (systemHealth.nodeJsService) {
        updateNodeJsCard(systemHealth.nodeJsService);
    }

    if (systemHealth.whatsAppService) {
        updateWhatsAppCard(systemHealth.whatsAppService);
    }
    
    updateTimestamp();
}

function updateDatabaseCard(database) {
    const dbCard = $('.stat-card').has('.stat-title:contains("Database Status")');
    const dbValueElement = dbCard.find('.stat-value');
    
    let status = "Unknown";
    let statusClass = "status-error";
    let icon = "bi-question-circle-fill";
    
    if (database.status === "Healthy") {
        status = "Connected";
        statusClass = "status-connected";
        icon = "bi-check-circle-fill";
    } else {
        status = "Disconnected";
        statusClass = "status-disconnected";
        icon = "bi-x-circle-fill";
    }
    
    dbValueElement.fadeOut(200, function() {
        dbValueElement.html(`
            <span class="${statusClass}">
                <i class="bi ${icon}"></i>
                ${status}
            </span>
        `).fadeIn(200);
    });
    
    addStatusChangeEffect(dbCard, database.status === "Healthy");
}

function updateInternetCard(internet) {
    const internetCard = $('.stat-card').has('.stat-title:contains("Internet Status")');
    const internetValueElement = internetCard.find('.stat-value');
    
    let status = "Unknown";
    let statusClass = "status-error";
    let icon = "bi-exclamation-triangle";
    
    if (internet.status === "Healthy") {
        status = "Online";
        statusClass = "status-connected";
        icon = "bi-wifi";
    } else {
        status = "Offline";
        statusClass = "status-disconnected";
        icon = "bi-wifi-off";
    }
    
    internetValueElement.fadeOut(200, function() {
        internetValueElement.html(`
            <span class="${statusClass}">
                <i class="bi ${icon}"></i>
                ${status}
            </span>
        `).fadeIn(200);
    });
    
    addStatusChangeEffect(internetCard, internet.status === "Healthy");
}

function updateNodeJsCard(nodeJsService) {
    const nodeCard = $('.stat-card').has('.stat-title:contains("Node.js Status")');
    const nodeValueElement = nodeCard.find('.stat-value');
    
    let status = "Unknown";
    let statusClass = "status-error";
    let icon = "bi-question-circle-fill";
    
    if (nodeJsService.status === "Healthy") {
        status = "Running";
        statusClass = "status-connected";
        icon = "bi-play-circle-fill";
    } else {
        status = "Stopped";
        statusClass = "status-disconnected";
        icon = "bi-stop-circle-fill";
    }
    
    nodeValueElement.fadeOut(200, function() {
        nodeValueElement.html(`
            <span class="${statusClass}">
                <i class="bi ${icon}"></i>
                ${status}
            </span>
        `).fadeIn(200);
    });
    
    addStatusChangeEffect(nodeCard, nodeJsService.status === "Healthy");
}

function updateWhatsAppCard(whatsAppService) {
    const whatsAppCard = $('.stat-card').has('.stat-title:contains("WhatsApp Status")');
    const shatsAppValueElement = whatsAppCard.find('.stat-value');
    
    let status = "Unknown";
    let statusClass = "status-error";
    let icon = "bi-question-circle-fill";
    
    switch (whatsAppService.status) {
        case "Healthy":
            status = "Connected";
            statusClass = "status-connected";
            icon = "bi-check-circle-fill";
            break;
        case "Authenticated":
            status = "Authenticated";
            statusClass = "status-authenticated";
            icon = "bi-shield-check-fill";
            break;
        case "Initializing":
            status = "Initializing";
            statusClass = "status-initializing";
            icon = "bi-arrow-clockwise";
            break;
        case "Disconnected":
            status = "Disconnected";
            statusClass = "status-disconnected";
            icon = "bi-wifi-off";
            break;
        case "Auth Failed":
            status = "Auth Failed";
            statusClass = "status-error";
            icon = "bi-shield-x-fill";
            break;
        case "Qr Ready":
            status = "QR Ready";
            statusClass = "status-qr-ready";
            icon = "bi-qr-code";
            break;
        case "Init Failed":
            status = "Init Failed";
            statusClass = "status-error";
            icon = "bi-exclamation-triangle-fill";
            break;
        case "Setup Failed":
            status = "Setup Failed";
            statusClass = "status-error";
            icon = "bi-gear-fill";
            break;
        case "Max Attempts Reached":
            status = "Max Attempts";
            statusClass = "status-error";
            icon = "bi-stop-circle-fill";
            break;
        case "Error":
        default:
            status = "Error";
            statusClass = "status-error";
            icon = "bi-exclamation-circle-fill";
            break;
    }
    
    shatsAppValueElement.fadeOut(200, function() {
        shatsAppValueElement.html(`
            <span class="${statusClass}">
                <i class="bi ${icon}"></i>
                ${status}
            </span>
        `).fadeIn(200);
    });
    
    addStatusChangeEffect(whatsAppCard, whatsAppService.status === "Healthy");
}

function addStatusChangeEffect(cardElement, isHealthy) {
    cardElement.removeClass('status-change-success status-change-error');
    
    const effectClass = isHealthy ? 'status-change-success' : 'status-change-error';
    cardElement.addClass(effectClass);
    
    setTimeout(() => {
        cardElement.removeClass(effectClass);
    }, 2000);
}

startConnection();
