
        var currentMonthRevenue = @currentMonthRevenue;
        var previousMonthRevenue = @previousMonthRevenue;
        var completedOrders = @completedOrders;
        var canceledOrders = @canceledOrders;

        var revenueChartCanvas = document.getElementById('revenueChart').getContext('2d');
        var revenueChart = new Chart(revenueChartCanvas, {
            type: 'bar',
            data: {
            labels: ['Current Month', 'Previous Month'],
                datasets: [{
            label: 'Revenue',
                    data: [currentMonthRevenue, previousMonthRevenue],
                    backgroundColor: [
                        'rgba(54, 162, 235, 0.5)',
                        'rgba(255, 99, 132, 0.5)'
                    ],
                    borderColor: [
                        'rgba(54, 162, 235, 1)',
                        'rgba(255, 99, 132, 1)'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
            responsive: true,
                scales: {
            y: {
            beginAtZero: true
                    }
                }
            }
        });

        var orderStatsChartCanvas = document.getElementById('orderStatsChart').getContext('2d');
        var orderStatsChart = new Chart(orderStatsChartCanvas, {
            type: 'bar',
            data: {
            labels: ['Completed Orders', 'Canceled Orders'],
                datasets: [{
            label: 'Order Statistics',
                    data: [completedOrders, canceledOrders],
                    backgroundColor: [
                        'rgba(75, 192, 192, 0.5)',
                        'rgba(255, 205, 86, 0.5)'
                    ],
                    borderColor: [
                        'rgba(75, 192, 192, 1)',
                        'rgba(255, 205, 86, 1)'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
            responsive: true,
                scales: {
            y: {
            beginAtZero: true
            }
}
}
});

