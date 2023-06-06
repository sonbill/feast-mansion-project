const mobileAside = document.getElementById('mobile-aside');
const toggleBtn = document.getElementById('toggleBtn');
const overlay = document.getElementById('overlay');

toggleBtn.addEventListener('click', () => {
    if (mobileAside.classList.contains('-translate-x-full')) {
        mobileAside.classList.remove('-translate-x-full');
        overlay.classList.remove('hidden')
    }
    else {
        mobileAside.classList.add('-translate-x-full');
    }
    //mobileAside.classList.toggle('-translate-x-full');
});

overlay.addEventListener('click', () => {
    console.log("click");
    mobileAside.classList.add('-translate-x-full');
    overlay.classList.add('hidden');

})

// Listen for window resize events to hide the mobile aside when the screen size changes
window.addEventListener('resize', function () {
    const mobileAside = document.getElementById('mobile-aside');
    const desktopAside = document.getElementById('desktop-sidebar');
    const headerMobile = document.getElementById('header-mobile');
    const toggleBtn = document.getElementById('toggleBtn');

    if (mobileAside && toggleBtn) {
        // Hide the mobile aside if the screen size is no longer mobile
        if (window.innerWidth > 640 && !mobileAside.classList.contains('-translate-x-full')) {
            mobileAside.classList.add('-translate-x-full');
            overlay.classList.add('hidden');
            //headerMobile.classList.add('hidden');
        }

        // Show the mobile aside if the screen size is mobile and the toggle button is visible
        if (window.innerWidth <= 640 && toggleBtn.offsetParent !== null && !mobileAside.classList.contains('-translate-x-full')) {
            desktopAside.classList.add('-translate-x-full');
        }
    } else {
        console.error("Element not found.");
    }
});

