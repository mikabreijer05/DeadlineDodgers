// ==========================
// Matrix Market - Cart Page
// README: I have made any comments in english, to not only encourage this when i start working but because i find it easier to do so in english
// Noa Scipio - 2507177
// This script handles rendering the cart page, showing products in the cart, calculating totals,
// and allowing users to remove items or proceed to checkout.
// ==========================

document.addEventListener('DOMContentLoaded', function () {
    renderCart();
});

// Renders the cart page
function renderCart() {
    // Get the HTML element where cart contents will be placed
    const cartNode = document.getElementById('cart-contents');


    // Get the current cart from localStorage, or set as empty array if none
    // HOW: localStorage persists data across page reloads and browser sessions
    const cart = JSON.parse(localStorage.getItem('cart')) || [];


    // If cart is empty, show a message and return (end the function here)

    if (cart.length === 0) {
        cartNode.innerHTML = `
            <div style="color: #20ff62; text-align: center; margin-top: 64px; font-size: 1.3rem;">
                Je winkelwagen is leeg.
            </div>
        `;
        return;
    }


    // Will hold the sum of all product subtotals
    let total = 0;
    let html = `
        <div class="product-grid" style="margin-top: 30px;">
    `;


    // For each product in the cart
    cart.forEach(item => {
        const price = item.price || 0; // Get product price; WHY: fallback to 0 if somehow missing so it doesn't break
        total += price * item.quantity; // HOW: Add to the total the price times quantity of this item

        // Add product details to HTML: name, quantity, price per, and remove button
        html += `
            <div class="product-card">
                <h3>${item.name}</h3>
                <p>Aantal: ${item.quantity}</p>
                <p>Prijs per stuk: €${price.toFixed(2)}</p>
                <button class="remove-btn" data-product-id="${item.id}">Verwijder</button>
            </div>
        `;
    });

    html += '</div>'; // End the product grid

    // Add the total price (sum) and the checkout button to HTML
    html += `
        <div style="margin-top: 38px; font-size:1.3rem; color:#20ff62;">
            <strong>Totaal bedrag: €<span id="cart-total">${total.toFixed(2)}</span></strong>
        </div>
        <button class="checkout-btn" style="margin-top:18px; padding:12px 28px; background:#20ff62; color:black; border-radius:12px; border:none; font-family:monospace; font-size:1.2rem; cursor:pointer; font-weight:bold;">Afrekenen</button>
    `;


    // Place the constructed HTML inside the cart container
    cartNode.innerHTML = html;

    
    // For every "Verwijder" (Remove) button, set up its click handler
    // HOW: Each button has its product ID and will remove one quantity at a time
    document.querySelectorAll('.remove-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            const id = btn.getAttribute('data-product-id');
            removeOneFromCart(id); // Remove one of this product
        });
    });

    // For the checkout button: when clicked, alert, save order, clear cart, refresh UI and cart badge.
    document.querySelector('.checkout-btn').addEventListener('click', function () {
        alert('Bedankt voor je bestelling! (hier zou een echt betaalsysteem komen)');
        saveOrderToHistory(); // Store all order details for later access (localStorage)
        localStorage.removeItem('cart');
        renderCart(); // Rerender page, now showing "cart empty"
        if (typeof updateCartCount === 'function') updateCartCount();
    });
}

// --------------------------
// Remove ONE quantity of a product from cart, or remove product if last one
// --------------------------
// WHY: User may want to decrement quantity rather than delete everything
function removeOneFromCart(id) {
    let cart = JSON.parse(localStorage.getItem('cart')) || [];
    let idx = cart.findIndex(item => item.id === id);
    if (idx !== -1) {
        if (cart[idx].quantity > 1) {
            cart[idx].quantity -= 1; // Subtract one from quantity if more than 1 left
        } else {
            // Remove the product if only one left
            cart.splice(idx, 1);
        }
    }
    // Save the changed cart and update UI/badge
    localStorage.setItem('cart', JSON.stringify(cart));
    renderCart();
    if (typeof updateCartCount === 'function') updateCartCount();
}

// ==========================
// Save complete order to localStorage after checkout
// ==========================
// WHAT: Collects full order data (all products, quantity, price/pc, totals, VAT) and saves for review/export/history.
// WHY: This is important for order confirmation, admin, receipt, statistics, or a later server sync.
function saveOrderToHistory() {
    // Load current cart (will only be called if cart has items)
    const cart = JSON.parse(localStorage.getItem('cart')) || [];
    if (cart.length === 0) return;

    let items = [];    // Will be all products as {name, amount, price_per_product}
    let subtotal = 0;  // Sum of products

    cart.forEach(item => {
        // Save just what you want shown in history: name, quantity, price per item
        const one = {
            name: item.name,
            amount: item.quantity,
            price_per_product: item.price
        };
        items.push(one);
        subtotal += (item.price || 0) * item.quantity;
    });

    // Now save entire order including BTW (VAT 21%)
    const order = {
        items: items,                         // Whole product list with their names/amounts/prices
        total_price: subtotal,                // Cart subtotal BEFORE VAT
        total_price_vat21: +(subtotal * 1.21).toFixed(2) // Subtotal + 21% VAT, rounded to 2 decimals
    };

    // Save ONLY last order in localStorage (will overwrite previous)
    // If you want ORDER HISTORY, switch to storing an array
    localStorage.setItem('lastOrder', JSON.stringify(order));
    // Why: Persist order summary for use by other pages, confirmations, admin, export, etc.
}