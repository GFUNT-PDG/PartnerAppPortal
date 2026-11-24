window.pacDownloadFile = function (fileName, contentType, base64) {
  var binary = atob(base64);
  var bytes = new Uint8Array(binary.length);
  for (var i = 0; i < binary.length; i++) {
    bytes[i] = binary.charCodeAt(i);
  }
  var blob = new Blob([bytes], { type: contentType || "application/octet-stream" });
  var url = URL.createObjectURL(blob);
  var link = document.createElement("a");
  link.href = url;
  link.download = fileName || "download.csv";
  document.body.appendChild(link);
  link.click();
  link.remove();
  URL.revokeObjectURL(url);
};

window.pacShowPdfInFrame = function (elementId, base64) {
  var el = document.getElementById(elementId);
  if (!el) return;

  if (el.dataset.pacBlobUrl) {
    URL.revokeObjectURL(el.dataset.pacBlobUrl);
    delete el.dataset.pacBlobUrl;
  }

  var binary = atob(base64);
  var bytes = new Uint8Array(binary.length);
  for (var i = 0; i < binary.length; i++) {
    bytes[i] = binary.charCodeAt(i);
  }
  var blob = new Blob([bytes], { type: "application/pdf" });
  var url = URL.createObjectURL(blob);
  el.dataset.pacBlobUrl = url;
  el.src = url;
};

window.pacOpenPdfTab = function (base64) {
  var binary = atob(base64);
  var bytes = new Uint8Array(binary.length);
  for (var i = 0; i < binary.length; i++) {
    bytes[i] = binary.charCodeAt(i);
  }
  var blob = new Blob([bytes], { type: "application/pdf" });
  var url = URL.createObjectURL(blob);
  window.open(url, "_blank", "noopener");
};

window.pacScrollTeamsLog = function (el) {
  if (!el) return;
  el.scrollTop = el.scrollHeight;
};

document.addEventListener("click", function (e) {
  var target = e.target;
  if (!target || !target.closest) return;

  var btn = target.closest(".waffle-btn");
  if (btn && e.defaultPrevented) {
    var panel = document.getElementById("pdg-hub-apps");
    if (panel && panel.togglePopover) panel.togglePopover();
  }

  if (target.closest("#pdg-hub-apps a")) {
    var openPanel = document.getElementById("pdg-hub-apps");
    if (openPanel && openPanel.hidePopover) openPanel.hidePopover();
  }
});

