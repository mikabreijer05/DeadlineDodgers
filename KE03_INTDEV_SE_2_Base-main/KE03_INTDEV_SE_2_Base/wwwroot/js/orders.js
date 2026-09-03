document.addEventListener('DOMContentLoaded', function() {
    const statusButtons = document.querySelectorAll('.status-btn[data-status]');
    const sortButtons = document.querySelectorAll('.sort-btn[data-sort]');
    const orderRows = document.querySelectorAll('.order-row');
    const ordersTableBody = document.getElementById('ordersTableBody');
    const resultCountSpan = document.getElementById('resultCount');
    const applyFiltersButton = document.getElementById('applyFiltersBtn');
    const removeAllFiltersButton = document.getElementById('removeAllFiltersBtn');

    let selectedStatuses = new Set();
    let currentSort = {
        key: 'date',
        direction: 'desc'
    };

    const filterInputs = [
        document.getElementById('customerName'),
        document.getElementById('fromDate'),
        document.getElementById('toDate'),
        document.getElementById('totalPriceFrom'),
        document.getElementById('totalPriceTo'),
        document.getElementById('minProducts'),
        document.getElementById('address')
    ].filter(Boolean);

    function showButtonFeedback(button) {
        button.classList.add('clicked-feedback');

        setTimeout(() => {
            button.classList.remove('clicked-feedback');
        }, 200);
    }

    function getSortValue(row, sortKey) {
        switch (sortKey) {
            case 'id':
                return parseInt(row.getAttribute('data-id') || '0');

            case 'date':
                return new Date(row.getAttribute('data-orderdate') || '1970-01-01').getTime();

            case 'total':
                return parseFloat((row.getAttribute('data-total') || '0').replace(',', '.'));

            case 'products':
                return parseInt(row.getAttribute('data-minproducts') || '0');

            default:
                return 0;
        }
    }

    function updateSortButtonLabels() {
        sortButtons.forEach(button => {
            const sortKey = button.getAttribute('data-sort');
            const baseLabel = button.getAttribute('data-label') || button.textContent.replace(/[↑↓]/g, '').trim();

            button.setAttribute('data-label', baseLabel);

            if (sortKey === currentSort.key) {
                button.classList.add('active');
                button.textContent = `${baseLabel} ${currentSort.direction === 'asc' ? '↑' : '↓'}`;
            } else {
                button.classList.remove('active');
                button.textContent = baseLabel;
            }
        });
    }

    function sortOrders() {
        if (!ordersTableBody) {
            return;
        }

        const rows = Array.from(orderRows);

        rows.sort((a, b) => {
            const firstValue = getSortValue(a, currentSort.key);
            const secondValue = getSortValue(b, currentSort.key);

            if (firstValue < secondValue) {
                return currentSort.direction === 'asc' ? -1 : 1;
            }

            if (firstValue > secondValue) {
                return currentSort.direction === 'asc' ? 1 : -1;
            }

            return 0;
        });

        rows.forEach(row => {
            ordersTableBody.appendChild(row);
        });

        updateSortButtonLabels();
    }

    function applyFilters() {
        let visibleCount = 0;

        orderRows.forEach(row => {
            let isVisible = true;

            const customerNameInput = document.getElementById('customerName');
            const fromDateInput = document.getElementById('fromDate');
            const toDateInput = document.getElementById('toDate');
            const totalPriceFromInput = document.getElementById('totalPriceFrom');
            const totalPriceToInput = document.getElementById('totalPriceTo');
            const minProductsInput = document.getElementById('minProducts');
            const addressInput = document.getElementById('address');

            // Check customer name
            if (isVisible && customerNameInput.value) {
                const value = customerNameInput.value.toLowerCase().trim();
                const rowValue = (row.getAttribute('data-customername') || '').toLowerCase().trim();

                if (!rowValue.includes(value)) {
                    isVisible = false;
                }
            }

            // Check date range
            if (isVisible && (fromDateInput.value || toDateInput.value)) {
                const valueFrom = fromDateInput.value ? new Date(fromDateInput.value) : null;
                const valueTo = toDateInput.value ? new Date(toDateInput.value) : null;
                const rowValue = new Date((row.getAttribute('data-orderdate') || '').trim());

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
            if (isVisible && (totalPriceFromInput.value || totalPriceToInput.value)) {
                const valueFrom = totalPriceFromInput.value ? parseFloat(totalPriceFromInput.value.replace(',', '.')) : null;
                const valueTo = totalPriceToInput.value ? parseFloat(totalPriceToInput.value.replace(',', '.')) : null;
                const rowValue = parseFloat((row.getAttribute('data-total') || '0').replace('€', '').replace(',', '.'));

                if (valueFrom !== null && valueTo !== null) {
                    if (!(rowValue >= valueFrom && rowValue <= valueTo)) {
                        isVisible = false;
                    }
                } else if (valueFrom !== null) {
                    if (!(rowValue >= valueFrom)) {
                        isVisible = false;
                    }
                } else if (valueTo !== null) {
                    if (!(rowValue <= valueTo)) {
                        isVisible = false;
                    }
                }
            }

            // Check minimum number of products
            if (isVisible && minProductsInput.value) {
                const value = parseInt(minProductsInput.value);
                const rowValue = row.getAttribute('data-minproducts');

                if (!rowValue || parseInt(rowValue) < value) {
                    isVisible = false;
                }
            }

            // Check address
            if (isVisible && addressInput.value) {
                const value = addressInput.value.toLowerCase().trim();
                const rowValue = (row.getAttribute('data-address') || '').toLowerCase().trim();

                if (!rowValue.includes(value)) {
                    isVisible = false;
                }
            }

            // Check status
            if (isVisible && selectedStatuses.size > 0) {
                const rowStatus = (row.getAttribute('data-status') || '').toLowerCase().trim();
                let statusMatch = false;

                selectedStatuses.forEach(status => {
                    const selectedStatus = status.toLowerCase().trim();

                    if (rowStatus.includes(selectedStatus)) {
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

        if (resultCountSpan) {
            resultCountSpan.textContent = visibleCount;
        }

        sortOrders();
    }

    statusButtons.forEach(button => {
        button.addEventListener('click', function() {
            const status = this.getAttribute('data-status');

            if (selectedStatuses.has(status)) {
                selectedStatuses.delete(status);
                this.classList.remove('active');
            } else {
                selectedStatuses.add(status);
                this.classList.add('active');
            }

            showButtonFeedback(this);
            applyFilters();
        });
    });

    sortButtons.forEach(button => {
        button.addEventListener('click', function() {
            const sortKey = this.getAttribute('data-sort');

            if (currentSort.key === sortKey) {
                currentSort.direction = currentSort.direction === 'asc' ? 'desc' : 'asc';
            } else {
                currentSort.key = sortKey;
                currentSort.direction = this.getAttribute('data-direction') || 'asc';
            }

            showButtonFeedback(this);
            sortOrders();
        });
    });

    filterInputs.forEach(input => {
        input.addEventListener('input', applyFilters);
        input.addEventListener('change', applyFilters);
    });

    if (applyFiltersButton) {
        applyFiltersButton.addEventListener('click', function() {
            showButtonFeedback(this);
            applyFilters();
        });
    }

    if (removeAllFiltersButton) {
        removeAllFiltersButton.addEventListener('click', function() {
            filterInputs.forEach(input => {
                input.value = '';
            });

            selectedStatuses.clear();

            statusButtons.forEach(button => {
                button.classList.remove('active');
            });

            showButtonFeedback(this);
            applyFilters();
        });
    }

    applyFilters();
});