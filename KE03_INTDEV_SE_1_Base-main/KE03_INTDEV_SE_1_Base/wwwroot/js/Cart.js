// ==========================
// Matrix Market - Cart Page
// Noa Scipio - 2507177
// This script handles rendering the cart page, showing products in the cart, calculating totals,
// and allowing users to remove items or proceed to checkout.
// ==========================

document.addEventListener('DOMContentLoaded', function () {
    renderCart();
});

// Renders the cart page
function renderCart() {
    const cartNode = document.getElementById('cart-contents');
    const cart = JSON.parse(localStorage.getItem('cart')) || [];

    if (cart.length === 0) {
        cartNode.innerHTML = `
            <div style="color: #20ff62; text-align: center; margin-top: 64px; font-size: 1.3rem;">
                Je winkelmandje is leeg.
            </div>
        `;
        return;
    }

    let total = 0;
    let html = `
        <div class="product-grid" style="margin-top: 30px;">
    `;

    cart.forEach(item => {
        const price = item.price || 0;
        total += price * item.quantity;
        html += `
            <div class="product-card">
                <h3>${item.name}</h3>
                <p>Aantal: ${item.quantity}</p>
                <p>Prijs per stuk: €${price.toFixed(2)}</p>
                <button class="remove-btn" data-product-id="${item.id}">Verwijder</button>
            </div>
        `;
    });

    html += '</div>';

    html += `
        <div style="margin-top: 38px; font-size:1.3rem; color:#20ff62;">
            <strong>Totaal bedrag: €<span id="cart-total">${total.toFixed(2)}</span></strong>
        </div>
        <button class="checkout-btn" style="margin-top:18px; padding:12px 28px; background:#20ff62; color:black; border-radius:12px; border:none; font-family:monospace; font-size:1.2rem; cursor:pointer; font-weight:bold;">Afrekenen</button>
    `;

    cartNode.innerHTML = html;

    // Add remove (decrement quantity) listeners
    document.querySelectorAll('.remove-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            const id = btn.getAttribute('data-product-id');
            removeOneFromCart(id);
        });
    });

    // Checkout (demo)
    document.querySelector('.checkout-btn').addEventListener('click', function () {
        alert('Bedankt voor je bestelling! (hier zou een echt betaalsysteem komen)');
        localStorage.removeItem('cart');
        renderCart();
        if (typeof updateCartCount === 'function') updateCartCount();
    });
}

// Removes ONE quantity of a product (if 0 left, removes entirely)
function removeOneFromCart(id) {
    let cart = JSON.parse(localStorage.getItem('cart')) || [];
    let idx = cart.findIndex(item => item.id === id);
    if (idx !== -1) {
        if (cart[idx].quantity > 1) {
            cart[idx].quantity -= 1;
        } else {
            // Remove the product if only one left
            cart.splice(idx, 1);
        }
    }
    localStorage.setItem('cart', JSON.stringify(cart));
    renderCart();
    if (typeof updateCartCount === 'function') updateCartCount();
}