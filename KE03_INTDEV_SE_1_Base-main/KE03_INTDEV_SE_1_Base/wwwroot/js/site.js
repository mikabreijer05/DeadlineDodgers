// =============================
// Matrix Market Cart Logic with Toast
// =============================



console.log("site.js loaded!");

// Setup cart button functionality after page load  - Noa
document.addEventListener('DOMContentLoaded', function () {
    updateCartCount();

    // Setup quantity selector buttons
    document.querySelectorAll('.qty-minus').forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            const input = this.parentElement.querySelector('.qty-input');
            const currentValue = parseInt(input.value);
            if (currentValue > 1) {
                input.value = currentValue - 1;
            }
        });
    });

    document.querySelectorAll('.qty-plus').forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            const input = this.parentElement.querySelector('.qty-input');
            const currentValue = parseInt(input.value);
            if (currentValue < 999) {
                input.value = currentValue + 1;
            }
        });
    });

    // Attach add to cart logic to every .add-btn button
    document.querySelectorAll('.add-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            // Read product ID, Name, and PRICE from data attributes
            const productId = btn.getAttribute('data-product-id');
            const productName = btn.getAttribute('data-product-name');
            const productPrice = parseFloat(btn.getAttribute('data-product-price'));

            // Get quantity from the quantity selector if it exists
            const quantitySelector = btn.parentElement.querySelector('.qty-input');
            const quantity = quantitySelector ? parseInt(quantitySelector.value) : 1;

            addToCart(productId, productName, productPrice, quantity);

            // Reset quantity selector back to 1
            if (quantitySelector) {
                quantitySelector.value = 1;
            }
        });
    });
});

// Add a product to cart in localStorage - Noa
function addToCart(productId, productName, productPrice, quantity = 1) {
    let cart = JSON.parse(localStorage.getItem('cart')) || [];
    let found = cart.find(item => item.id === productId);
    if (found) {
        found.quantity += quantity;
    } else {
        cart.push({ id: productId, name: productName, price: productPrice, quantity: quantity });
    }
    localStorage.setItem('cart', JSON.stringify(cart));
    updateCartCount();
    showToast(quantity + " × " + productName + " added to cart!");
}

// Update the cart badge in the nav - Noa
function updateCartCount() {
    let cart = JSON.parse(localStorage.getItem('cart')) || [];
    let totalItems = cart.reduce((sum, item) => sum + item.quantity, 0);
    let cartCount = document.getElementById('cartCount');
    if (cartCount) cartCount.textContent = totalItems > 0 ? totalItems : '';
}

// Show a toast/snackbar notification at the bottom of the page Noa
function showToast(message) {
    const toast = document.getElementById('toast');
    if (!toast) return;
    toast.textContent = message;
    toast.style.display = 'block';
    toast.style.opacity = 1;
    setTimeout(() => {
        toast.style.opacity = 0;
        setTimeout(() => { toast.style.display = 'none'; }, 400);
    }, 2000);
}