const tabs = document.querySelectorAll('[role="tab"]');
const tabContents = document.querySelectorAll('[role="tabpanel"]');

tabs.forEach(tab => {
    tab.addEventListener('click', e => {
        // Hide all tab contents
        tabContents.forEach(content => {
            content.classList.add('hidden');
        });
        // Deactivate all tabs
        tabs.forEach(tab => {
            tab.setAttribute('aria-selected', false);
            tab.classList.remove('text-gray-800', 'border-red-700');
            tab.classList.add('text-gray-500', 'border-transparent');
        });
        // Show the selected tab content
        const target = e.currentTarget.getAttribute('data-target');
        document.querySelector(target).classList.remove('hidden');
        // Activate the selected tab
        e.currentTarget.setAttribute('aria-selected', true);
        e.currentTarget.classList.remove('text-gray-500', 'border-transparent');
        e.currentTarget.classList.add('text-gray-800', 'border-red-700');
    });
});