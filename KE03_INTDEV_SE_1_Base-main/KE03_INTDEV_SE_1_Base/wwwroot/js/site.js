// =============================
// Matrix Market Cart Logic with Dropdown & Toast
// =============================

console.log("site.js loaded!");

// Setup cart button functionality after page load
document.addEventListener('DOMContentLoaded', function () {
    updateCartCount();

    // Attach add to cart logic to every .add-btn button Noa
    document.querySelectorAll('.add-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            // Read product ID and Name from data attributes
            const productId = btn.getAttribute('data-product-id');
            const productName = btn.getAttribute('data-product-name');
            addToCart(productId, productName);
        });
    });

    // Close cart dropdown on scroll (optional for better UX)
    window.addEventListener('scroll', function () {
        hideCartDropdown();
    });
});

// Add a product to cart in localStorage Noa
function addToCart(productId, productName) {
    let cart = JSON.parse(localStorage.getItem('cart')) || [];
    let found = cart.find(item => item.id === productId);
    if (found) {
        found.quantity += 1;
    } else {
        cart.push({ id: productId, name: productName, quantity: 1 });
    }
    localStorage.setItem('cart', JSON.stringify(cart));
    updateCartCount();
    showToast(productName + " added to cart!");
}

// Update the cart badge in the nav Noa
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