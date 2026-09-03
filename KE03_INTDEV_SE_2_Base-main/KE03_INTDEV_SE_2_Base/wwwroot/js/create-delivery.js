function initCreateDeliveryPage() {
    console.log('create-delivery.js initialized');
    const createDeliveryPage = document.querySelector('.create-delivery-page');

    if (!createDeliveryPage) {
        return;
    }

    const orderCheckboxes = Array.from(document.querySelectorAll('.order-checkbox'));
    const orderCards = Array.from(document.querySelectorAll('.order-select-card'));
    const productRows = Array.from(document.querySelectorAll('.product-select-row'));
    const quantityInputs = Array.from(document.querySelectorAll('.product-quantity-input'));
    const quantityButtons = Array.from(document.querySelectorAll('.quantity-button'));
    const selectedOrderCount = document.getElementById('selectedOrderCount');
    const emptyProductsState = document.getElementById('emptyProductsState');


    console.log('Quantity buttons found:', quantityButtons.length);
    console.log('Quantity inputs found:', quantityInputs.length);
    console.log('Quantity buttons:', quantityButtons);

    document.addEventListener('click', function (event) {
        console.log('Clicked element:', event.target);
        console.log('Closest quantity button:', event.target.closest('.quantity-button'));
    });
    
    const orderSearch = document.getElementById('orderSearch');
    const vehicleSearch = document.getElementById('vehicleSearch');
    const vehicleCards = Array.from(document.querySelectorAll('.vehicle-select-card'));
    const dimensionKeys = ['XS', 'S', 'M', 'L', 'XL'];

    function getSelectedOrderIds() {
        return orderCheckboxes
            .filter(checkbox => checkbox.checked)
            .map(checkbox => checkbox.value);
    }

    function updateVisibleProducts() {
        const selectedOrderIds = getSelectedOrderIds();
        let visibleProductCount = 0;

        productRows.forEach(row => {
            const shouldShow = selectedOrderIds.includes(row.dataset.orderId);

            row.hidden = !shouldShow;

            if (!shouldShow) {
                const input = row.querySelector('.product-quantity-input');

                if (input) {
                    input.value = 0;
                }
            } else {
                visibleProductCount++;
            }
        });

        if (selectedOrderCount) {
            selectedOrderCount.textContent = selectedOrderIds.length;
        }

        if (emptyProductsState) {
            emptyProductsState.hidden = visibleProductCount > 0;
        }

        updateDimensionSummary();
    }

    function updateDimensionSummary() {
        const totals = {
            XS: 0,
            S: 0,
            M: 0,
            L: 0,
            XL: 0
        };

        productRows.forEach(row => {
            if (row.hidden) {
                return;
            }

            const dimension = (row.dataset.dimension || '').toUpperCase();
            const input = row.querySelector('.product-quantity-input');
            const quantity = parseInt(input?.value || '0');

            if (Object.prototype.hasOwnProperty.call(totals, dimension)) {
                totals[dimension] += isNaN(quantity) ? 0 : quantity;
            }
        });

        dimensionKeys.forEach(key => {
            const element = document.getElementById(`dimensionCount-${key}`);

            if (element) {
                element.textContent = totals[key];
            }
        });
    }

    function clampQuantity(input) {
        const min = parseInt(input.min || '0');
        const max = parseInt(input.max || '0');
        let value = parseInt(input.value || '0');

        if (isNaN(value)) {
            value = 0;
        }

        if (value < min) {
            value = min;
        }

        if (value > max) {
            value = max;
        }

        input.value = value;
    }

    orderCheckboxes.forEach(checkbox => {
        checkbox.addEventListener('change', updateVisibleProducts);
    });

    quantityInputs.forEach(input => {
        input.addEventListener('input', function () {
            clampQuantity(this);
            updateDimensionSummary();
        });

        input.addEventListener('change', function () {
            clampQuantity(this);
            updateDimensionSummary();
        });
    });

    quantityButtons.forEach(button => {
        button.addEventListener('click', function (event) {
            event.preventDefault();
            event.stopPropagation();

            const row = this.closest('.product-select-row');
            const input = row?.querySelector('.product-quantity-input');

            if (!input) {
                console.log('No quantity input found for button', this);
                return;
            }

            const min = Number(input.min || 0);
            const max = Number(input.max || 0);
            const currentValue = Number(input.value || 0);
            const adjustment = this.classList.contains('quantity-plus') ? 1 : -1;
            const newValue = currentValue + adjustment;

            console.log('Before update:', {
                currentValue,
                adjustment,
                newValue,
                min,
                max
            });

            input.value = newValue;

            clampQuantity(input);
            updateDimensionSummary();

            console.log('After update:', {
                value: input.value
            });

            input.dispatchEvent(new Event('input', { bubbles: true }));
            input.dispatchEvent(new Event('change', { bubbles: true }));
        });
    });

    if (orderSearch) {
        orderSearch.addEventListener('input', function () {
            const value = this.value.toLowerCase().trim();

            orderCards.forEach(card => {
                card.hidden = value && !(card.dataset.search || '').includes(value);
            });
        });
    }

    if (orderSearch) {
        orderSearch.addEventListener('input', function () {
            const value = this.value.toLowerCase().trim();

            orderCards.forEach(card => {
                card.hidden = value && !(card.dataset.search || '').includes(value);
            });
        });
    }

    if (vehicleSearch) {
        vehicleSearch.addEventListener('input', function () {
            const value = this.value.toLowerCase().trim();

            vehicleCards.forEach(card => {
                card.hidden = value && !(card.dataset.search || '').includes(value);
            });
        });
    }

    updateVisibleProducts();
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initCreateDeliveryPage);
} else {
    initCreateDeliveryPage();
}