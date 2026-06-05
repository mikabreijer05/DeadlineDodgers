document.addEventListener('DOMContentLoaded', function() {
    const statusButtons = document.querySelectorAll('.status-btn');
    const orderRows = document.querySelectorAll('.order-row');
    const resultCountSpan = document.getElementById('resultCount');
    let selectedStatuses = new Set();

    // Add click event to each status button
    statusButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            const status = this.getAttribute('data-status');
            
            // Toggle status selection
            if (selectedStatuses.has(status)) {
                selectedStatuses.delete(status);
                this.classList.remove('active');
            } else {
                selectedStatuses.add(status);
                this.classList.add('active');
            }
            
            // Filter orders
            filterOrders();
        });
    });

    function filterOrders() {
        let visibleCount = 0;
        
        orderRows.forEach(row => {
            const rowStatus = row.getAttribute('data-status');
            let isVisible = false;

            // If no filters selected, show all rows
            if (selectedStatuses.size === 0) {
                isVisible = true;
            } else {
                // Check if row's status matches any selected status
                selectedStatuses.forEach(status => {
                    if (rowStatus && rowStatus.toLowerCase().includes(status.toLowerCase())) {
                        isVisible = true;
                    }
                });
            }

            if (isVisible) {
                row.style.display = '';
                visibleCount++;
            } else {
                row.style.display = 'none';
            }
        });

        // Update result count
        if (resultCountSpan) {
            resultCountSpan.textContent = visibleCount;
        }

        // Show empty state if no results
        if (visibleCount === 0) {
            console.log('No matching orders found');
        }
    }
});
document.addEventListener('DOMContentLoaded', function () {
    const sortButtons = document.querySelectorAll('.sort-btn');
    const ordersTableBody = document.getElementById('ordersTableBody');
    let currentSortColumn = 'date';
    let sortDirection = 'desc';

    // Add click event to each sort button
    sortButtons.forEach(button => {
        button.addEventListener('click', function (e) {
            e.preventDefault();

            // Remove active class from all buttons
            sortButtons.forEach(btn => btn.classList.remove('active'));

            // Add active class to clicked button
            this.classList.add('active');

            // Determine sort column
            const buttonText = this.textContent.trim();
            if (buttonText.includes('Datum')) {
                currentSortColumn = 'date';
                sortDirection = 'desc';
                this.textContent = 'Datum ↓';
            } else if (buttonText.includes('ID')) {
                currentSortColumn = 'id';
                sortDirection = 'asc';
                this.textContent = 'ID ↑';
            } else if (buttonText.includes('Prijs')) {
                currentSortColumn = 'price';
                sortDirection = 'desc';
                this.textContent = 'Prijs ↓';
            } else if (buttonText.includes('Producten')) {
                currentSortColumn = 'products';
                sortDirection = 'asc';
                this.textContent = 'Producten ↑';
            }

            // Sort orders
            sortOrders();
        });
    });

    function sortOrders() {
        const rows = Array.from(ordersTableBody.querySelectorAll('tr.order-row'));

        rows.sort((rowA, rowB) => {
            let valueA, valueB;

            switch (currentSortColumn) {
                case 'date':
                    // Extract date from 3rd td (yyyy-MM-dd format)
                    valueA = new Date(rowA.cells[2].textContent.trim());
                    valueB = new Date(rowB.cells[2].textContent.trim());
                    return sortDirection === 'desc' ? valueB - valueA : valueA - valueB;

                case 'id':
                    // Extract ID from 1st td
                    valueA = parseInt(rowA.cells[0].textContent.trim());
                    valueB = parseInt(rowB.cells[0].textContent.trim());
                    return sortDirection === 'asc' ? valueA - valueB : valueB - valueA;

                case 'price':
                    // Extract price from 4th td (€X,XXX.XX format)
                    valueA = parseFloat(rowA.cells[3].textContent.trim().replace('€', '').replace(',', ''));
                    valueB = parseFloat(rowB.cells[3].textContent.trim().replace('€', '').replace(',', ''));
                    return sortDirection === 'desc' ? valueB - valueA : valueA - valueB;

                case 'products':
                    // For now, we'll sort by number of action buttons (as a placeholder)
                    // In a real scenario, you'd want this data in a data attribute
                    valueA = rowA.querySelectorAll('.action-btn').length;
                    valueB = rowB.querySelectorAll('.action-btn').length;
                    return sortDirection === 'asc' ? valueA - valueB : valueB - valueA;

                default:
                    return 0;
            }
        });

        // Re-append sorted rows to table body
        rows.forEach(row => ordersTableBody.appendChild(row));
    }
});