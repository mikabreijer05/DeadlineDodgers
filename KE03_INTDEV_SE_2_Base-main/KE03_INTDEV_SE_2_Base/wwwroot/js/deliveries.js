document.addEventListener('DOMContentLoaded', function () {
    const deliveryCards = Array.from(document.querySelectorAll('.delivery-card'));
    const deliveriesList = document.getElementById('deliveriesList');
    const resultCount = document.getElementById('deliveryResultCount');
    const noDeliveriesMessage = document.getElementById('noDeliveriesMessage');
    const printButtons = document.querySelectorAll('.delivery-action.print');

    const vehicleFilter = document.getElementById('vehicleFilter');
    const fromDateFilter = document.getElementById('fromDateFilter');
    const toDateFilter = document.getElementById('toDateFilter');
    const clearFiltersButton = document.getElementById('clearDeliveryFilters');
    const sortButtons = document.querySelectorAll('.delivery-sort-button');

    let currentSort = {
        key: 'id',
        direction: 'asc'
    };

    function getSortValue(card, key) {
        switch (key) {
            case 'id':
                return parseInt(card.dataset.id || '0');

            case 'vehicle':
                return card.dataset.vehicle || '';

            case 'date':
                return new Date(card.dataset.date || '1970-01-01').getTime();

            default:
                return '';
        }
    }

    function updateSortButtons() {
        sortButtons.forEach(button => {
            const sortKey = button.dataset.sort;
            const baseLabel = button.dataset.label || button.textContent.replace(/[↑↓]/g, '').trim();

            button.dataset.label = baseLabel;

            if (sortKey === currentSort.key) {
                button.classList.add('active');
                button.textContent = `${baseLabel} ${currentSort.direction === 'asc' ? '↑' : '↓'}`;
            } else {
                button.classList.remove('active');
                button.textContent = baseLabel;
            }
        });
    }

    function sortDeliveries() {
        if (!deliveriesList) {
            return;
        }

        const sortedCards = [...deliveryCards].sort((first, second) => {
            const firstValue = getSortValue(first, currentSort.key);
            const secondValue = getSortValue(second, currentSort.key);

            if (firstValue < secondValue) {
                return currentSort.direction === 'asc' ? -1 : 1;
            }

            if (firstValue > secondValue) {
                return currentSort.direction === 'asc' ? 1 : -1;
            }

            return 0;
        });

        sortedCards.forEach(card => deliveriesList.appendChild(card));
        updateSortButtons();
    }

    function applyDeliveryFilters() {
        const vehicleValue = vehicleFilter?.value.toLowerCase().trim() || '';
        const fromDateValue = fromDateFilter?.value ? new Date(fromDateFilter.value) : null;
        const toDateValue = toDateFilter?.value ? new Date(toDateFilter.value) : null;

        let visibleCount = 0;

        deliveryCards.forEach(card => {
            let isVisible = true;

            const vehicle = card.dataset.vehicle || '';
            const deliveryDate = new Date(card.dataset.date || '1970-01-01');

            if (vehicleValue && !vehicle.includes(vehicleValue)) {
                isVisible = false;
            }

            if (isVisible && fromDateValue && deliveryDate < fromDateValue) {
                isVisible = false;
            }

            if (isVisible && toDateValue && deliveryDate > toDateValue) {
                isVisible = false;
            }

            card.hidden = !isVisible;

            if (isVisible) {
                visibleCount++;
            }
        });

        if (resultCount) {
            resultCount.textContent = visibleCount;
        }

        if (noDeliveriesMessage) {
            noDeliveriesMessage.hidden = visibleCount !== 0;
        }

        sortDeliveries();
    }

    [vehicleFilter, fromDateFilter, toDateFilter].forEach(input => {
        if (!input) {
            return;
        }

        input.addEventListener('input', applyDeliveryFilters);
        input.addEventListener('change', applyDeliveryFilters);
    });

    if (clearFiltersButton) {
        clearFiltersButton.addEventListener('click', function () {
            if (vehicleFilter) {
                vehicleFilter.value = '';
            }

            if (fromDateFilter) {
                fromDateFilter.value = '';
            }

            if (toDateFilter) {
                toDateFilter.value = '';
            }

            applyDeliveryFilters();
        });
    }

    sortButtons.forEach(button => {
        button.addEventListener('click', function () {
            const sortKey = this.dataset.sort;

            if (currentSort.key === sortKey) {
                currentSort.direction = currentSort.direction === 'asc' ? 'desc' : 'asc';
            } else {
                currentSort.key = sortKey;
                currentSort.direction = this.dataset.direction || 'asc';
            }

            sortDeliveries();
        });
    });

    printButtons.forEach(button => {
        button.addEventListener('click', async function () {
            const deliveryId = this.dataset.deliveryId;
            const antiForgeryToken = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

            if (!deliveryId) {
                return;
            }

            try {
                const response = await fetch(`/Deliveries/Print/${deliveryId}`, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': antiForgeryToken || ''
                    }
                });

                if (!response.ok) {
                    alert('De levering kon niet worden geprint.');
                    return;
                }

                const result = await response.json();

                alert(result.message || 'The delivery details are being printed');
            } catch {
                alert('Er is iets misgegaan tijdens het printen.');
            }
        });
    });

    applyDeliveryFilters();
});

function confirmDeliveryDelete(deliveryId) {
    const confirmed = confirm('Weet je zeker dat je deze levering wilt verwijderen?');

    if (!confirmed) {
        return;
    }

    const form = document.getElementById(`delete-delivery-form-${deliveryId}`);

    if (form) {
        form.submit();
    }
}