function toggleMenu() {
    var menu = document.getElementById("mobile-menu-2");
    var button = document.querySelector("[data-collapse-toggle='mega-menu-full']");

    if (menu.classList.contains("hidden")) {
        menu.classList.remove("hidden");
        button.setAttribute("aria-expanded", "true");
    } else {
        menu.classList.add("hidden");
        button.setAttribute("aria-expanded", "false");
    }
}


document.querySelector("[data-collapse-toggle='mega-menu-full']").addEventListener("click", toggleMenu);