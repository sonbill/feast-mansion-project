// Hide the notification after 5 seconds
setTimeout(function() {
    var notification = document.getElementById("notification");
    if (notification) {
    notification.style.display = "none";
    }
}, 5000);
