
//function toggleSidebarAdminLayout() {
//    console.log("click");
//    var toggleBtn = document.getElementById("toggleBtn");
//    var sidebar = document.getElementById("mobile-aside");

//    if (sidebar.classList.contains("hidden")) {
//        sidebar.classList.remove("hidden");
//        sidebar.classList.add("block");
//    } else {
//        sidebar.classList.add("block");
//    }

//}

//document.querySelector(".toggleBtn").addEventListener("click", toggleSidebarAdminLayout);


//const sidebar = document.getElementById('mobile-aside');
//const toggleButton = document.getElementById('toggleBtn');

//toggleButton.addEventListener('click', () => {
//    sidebar.classList.toggle('-translate-x-full');
//});




const mobileAside = document.getElementById('mobile-aside');
const toggleBtn = document.getElementById('toggleBtn');

toggleBtn.addEventListener('click', () => {
    if (mobileAside.classList.contains('-translate-x-full')) {
        mobileAside.classList.remove('-translate-x-full');
    }
    else {
        mobileAside.classList.add('-translate-x-full');
    }
    //mobileAside.classList.toggle('-translate-x-full');
});


// Listen for window resize events to hide the mobile aside when the screen size changes
window.addEventListener('resize', function () {
    const mobileAside = document.getElementById('mobile-aside');
    const desktopAside = document.getElementById('desktop-sidebar');

    const toggleBtn = document.getElementById('toggleBtn');

    if (mobileAside && toggleBtn) {
        // Hide the mobile aside if the screen size is no longer mobile
        if (window.innerWidth > 640 && !mobileAside.classList.contains('-translate-x-full')) {
            mobileAside.classList.add('-translate-x-full');
        }

        // Show the mobile aside if the screen size is mobile and the toggle button is visible
        if (window.innerWidth <= 640 && toggleBtn.offsetParent !== null && !mobileAside.classList.contains('-translate-x-full')) {
            desktopAside.classList.add('-translate-x-full');
        }
    } else {
        console.error("Element not found.");
    }
});

// Add event listener to the toggle button
//const toggleButton = document.getElementById('toggleBtn');
//if (toggleButton) {
//    toggleButton.addEventListener('click', toggleSidebarAdminLayout);
//} else {
//    console.error("Toggle button not found.");
//}

