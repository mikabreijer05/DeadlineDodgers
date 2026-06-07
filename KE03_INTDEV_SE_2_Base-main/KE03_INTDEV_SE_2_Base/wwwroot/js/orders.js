document.addEventListener('DOMContentLoaded', function() {
    const statusButtons = document.querySelectorAll('.status-btn');
    const orderRows = document.querySelectorAll('.order-row');
    const resultCountSpan = document.getElementById('resultCount');

    let selectedStatuses = new Set();
    let filterInputs = [];

    // Initialize filter inputs array
    document.querySelectorAll('.filter-input').forEach(input => {
        filterInputs.push({
            element: input,
            value: ''
        });
    });

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
            applyFilters();
        });
    });

    // Add click event to the "Apply" button
    document.getElementById('applyFiltersBtn').addEventListener('click', function() {
        // Update filter input values
        filterInputs.forEach(inputInfo => {
            inputInfo.value = inputInfo.element.value.trim().toLowerCase();
        });

        // Filter orders
        applyFilters();
    });

    // Add click event to the "Remove All Filters" button
    document.getElementById('removeAllFiltersBtn').addEventListener('click', function() {
        // Clear all filters
        selectedStatuses.clear();

        filterInputs.forEach(inputInfo => {
            inputInfo.element.value = '';
            inputInfo.value = '';
        });

        // Reset sort options if needed
        const sortButtons = document.querySelectorAll('.sort-btn');
        sortButtons.forEach(button => button.classList.remove('active'));
        document.querySelector('.sort-btn.active').textContent = 'Datum ↓';

        // Update result count and filter orders
        applyFilters();
    });

    function applyFilters() {
        let visibleCount = 0;

        orderRows.forEach(row => {
            let isVisible = true;

            // Check customer name
            if (isVisible && document.getElementById('customerName').value) {
                const value = document.getElementById('customerName').value.toLowerCase().trim();
                const rowValue = row.getAttribute(`data-customername`).toLowerCase().trim();
                if (!rowValue.includes(value)) {
                    isVisible = false;
                }
            }

            // Check date range
            if (isVisible && document.getElementById('fromDate').value || document.getElementById('toDate').value) {
                const valueFrom = document.getElementById('fromDate').value ? new Date(document.getElementById('fromDate').value) : null;
                const valueTo = document.getElementById('toDate').value ? new Date(document.getElementById('toDate').value) : null;
                const rowValue = new Date(row.getAttribute(`data-orderdate`).trim());
                if (valueFrom && valueTo) {
                    if (!(rowValue >= valueFrom && rowValue <= valueTo)) {
                        isVisible = false;
                    }
                } else if (valueFrom) {
                    if (!(rowValue >= valueFrom)) {
                        isVisible = false;
                    }
                } else if (valueTo) {
                    if (!(rowValue <= valueTo)) {
                        isVisible = false;
                    }
                }
            }

            // Check total price range
            if (isVisible && document.getElementById('totalPriceFrom').value || document.getElementById('totalPriceTo').value) {
                const valueFrom = document.getElementById('totalPriceFrom').value ? parseFloat(document.getElementById('totalPriceFrom').value.replace(',', '.')) : null;
                const valueTo = document.getElementById('totalPriceTo').value ? parseFloat(document.getElementById('totalPriceTo').value.replace(',', '.')) : null;
                const rowValue = parseFloat(row.getAttribute(`data-total`).replace('€', '').replace(',', '.'));
                if (valueFrom && valueTo) {
                    if (!(rowValue >= valueFrom && rowValue <= valueTo)) {
                        isVisible = false;
                    }
                } else if (valueFrom) {
                    if (!(rowValue >= valueFrom)) {
                        isVisible = false;
                    }
                } else if (valueTo) {
                    if (!(rowValue <= valueTo)) {
                        isVisible = false;
                    }
                }
            }

            // Check minimum number of products
            if (isVisible && document.getElementById('minProducts').value) {
                const value = parseInt(document.getElementById('minProducts').value);
                const rowValue = row.getAttribute(`data-minproducts`);
                if (!rowValue || parseInt(rowValue) < value) {
                    isVisible = false;
                }
            }

            // Check address
            if (isVisible && document.getElementById('address').value) {
                const value = document.getElementById('address').value.toLowerCase().trim();
                const rowValue = row.getAttribute(`data-address`).toLowerCase().trim();
                if (!rowValue.includes(value)) {
                    isVisible = false;
                }
            }

            // Check status
            if (isVisible && selectedStatuses.size > 0) {
                let statusMatch = false;
                selectedStatuses.forEach(status => {
                    const rowStatus = row.getAttribute(`data-status`);
                    if (rowStatus && rowStatus.toLowerCase().includes(status.toLowerCase())) {
                        statusMatch = true;
                    }
                });
                if (!statusMatch) {
                    isVisible = false;
                }
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
        } else {
            console.log(`${visibleCount} matching orders found`);
        }
    }
});