// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function getTimeInState(host, interval) {
  let time;
  const statusRecDate = new Date(host.statusRecDate);

  if (host.availabilityMethod === 0) {
    time = 0;
  } else if (host.statusEventCount > 0 && [1, 2, 5].includes(host.status)) {
    time = host.statusEventCount * interval;
  } else if (statusRecDate < new Date(1999, 10, 29) && [0, 3].includes(host.status)) {
    time = (host.totalPolls || 0) * interval;
  } else if (statusRecDate > new Date(1999, 10, 29)) {
    time = Math.floor((Date.now() - statusRecDate.getTime()) / 1000);
  } else if (host.snmpSysUpTimeInstance > 0) {
    time = Math.floor(host.snmpSysUpTimeInstance / 100);
  } else {
    time = 0;
  }

  if (time > 2e13) {
    time = 0;
  }

  return time > 0 ? getDaysFromTime(time) : "N/A";
}

function getDaysFromTime(time, showSeconds = false) {
  const days = Math.floor(time / 86400);
  const hours = Math.floor((time % 86400) / 3600);
  const minutes = Math.floor((time % 3600) / 60);
  const seconds = time % 60;

  let result = `${days}d ${hours}h ${minutes}m`;
  if (showSeconds) {
    result += ` ${seconds}s`;
  }

  return result;
}

function parseCustomDate(dateStr) {
  // Misal dateStr = "26/05/2025 09:20:53"
  const [datePart, timePart] = dateStr.split(" ");
  const [day, month, year] = datePart.split("/").map(Number);
  const [hour, minute, second] = timePart.split(":").map(Number);

  return new Date(year, month - 1, day, hour, minute, second); // month -1 karena 0-indexed
}

function filterValidNameCharacters(value) {
  return value.replace(/[^a-zA-Z\s\-']/g, "");
}

function filterValidNumberCharacters(value) {
  return value.replace(/[^\d.-]+/g, "");
}

function filterValidCodenameCharacters(value) {
  return value.replace(/[^a-zA-Z\s\-'_]/g, "");
}

function filterValidEmailCharacters(value) {
  return value.replace(/[^a-zA-Z0-9@.\-'_+]/g, "");
}

function validateEmailFormat(value) {
  const atCount = (value.match(/@/g) || []).length;
  if (atCount > 1) {
    const parts = value.split("@");
    value = parts[0] + "@" + parts.slice(1).join("");
  }

  value = value.replace(/\.{2,}/g, ".");

  value = value.replace(/^[.\-_]+|[.\-_]+$/g, "");

  return value;
}

function filterValidIpCharacters(value) {
  value = value.replace(/[^0-9.]/g, "");

  const segments = value.split(".");

  if (segments.length > 4) {
    segments.splice(4);
  }

  const validatedSegments = segments.map((segment) => {
    segment = segment.replace(/^0+/, "") || "0";

    if (segment.length > 3) {
      segment = segment.substring(0, 3);
    }

    const numValue = parseInt(segment, 10);
    if (numValue > 255) {
      segment = "255";
    }

    return segment;
  });

  let result = validatedSegments.join(".");

  if (result.endsWith(".")) {
    result = result.slice(0, -1);
  }

  result = result.replace(/\.{2,}/g, ".");

  return result;
}

function filterValidUsernameCharacters(value) {
  return value.replace(/[^a-zA-Z0-9@\-'_]/g, "");
}

function filterValidCodeCharacters(value) {
  return value.replace(/[^a-zA-Z]/g, "");
}

function filterValidPhoneCharacters(value) {
  return value.replace(/[^0-9]/g, "");
}

function filterValidNameNumberCharacters(value) {
  return value.replace(/[^a-zA-Z0-9\s\-_+]/g, "");
}
// $(".name-input").on("input keypress paste", function (e) {
//   var input = $(this);
//   var value = input.val();

//   if (e.type === "keypress") {
//     var char = String.fromCharCode(e.which);

//     if (
//       [8, 9, 27, 13, 46].indexOf(e.keyCode) !== -1 ||
//       (e.keyCode === 65 && e.ctrlKey === true) ||
//       (e.keyCode === 67 && e.ctrlKey === true) ||
//       (e.keyCode === 86 && e.ctrlKey === true) ||
//       (e.keyCode === 88 && e.ctrlKey === true)
//     ) {
//       return;
//     }

//     if (!/^[a-zA-Z\s\-']$/.test(char)) {
//       e.preventDefault();
//       showValidationError(input, "Invalid character! Only letters, spaces, hyphens (-), and apostrophes (') are allowed.");
//       return false;
//     }
//   }

//   setTimeout(function () {
//     validateNameField(input);
//   }, 10);
// });
