// ==========================
// Matrix Market - Winkelwagen Pagina
// README: Ik heb nu alle opmerkingen in het Nederlands gezet, voor duidelijkheid en toekomstige documentatie
// Noa Scipio - 2507177
// Dit script regelt het tonen van de winkelwagenpagina, het weergeven van producten in de winkelwagen, het berekenen van totalen,
// en geeft gebruikers de mogelijkheid om items te verwijderen of af te rekenen.
// ==========================

document.addEventListener('DOMContentLoaded', function () {
    renderCart();
});

// Rendeert de winkelwagenpagina
function renderCart() {
    // Haal het HTML-element op waar de inhoud van de winkelwagen geplaatst wordt
    const cartNode = document.getElementById('cart-contents');

    // Haal de huidige winkelwagen op uit localStorage, of gebruik een lege array als er geen winkelwagen is
    // HOE: localStorage bewaart data over pagina-herladingen en browsersessies heen
    const cart = JSON.parse(localStorage.getItem('cart')) || [];

    // Als de winkelwagen leeg is, toon een bericht en stop (einde functie)
    if (cart.length === 0) {
        cartNode.innerHTML = `
            <div style="color: #20ff62; text-align: center; margin-top: 64px; font-size: 1.3rem;">
                Je winkelwagen is leeg.
            </div>
        `;
        return;
    }

    // Houdt het totaalbedrag bij van alle producten
    let total = 0;
    let html = `
        <div class="product-grid" style="margin-top: 30px;">
    `;

    // Doorloop elk product in de winkelwagen
    cart.forEach(item => {
        const price = item.price || 0; // Haal de prijs van het product op; WAAROM: terugvallen op 0 als prijs ontbreekt zodat de code niet stukgaat
        total += price * item.quantity; // HOE: Tel prijs keer aantal op bij het totaal

        // Voeg productdetails toe aan de HTML: naam, aantal, prijs per stuk en verwijderknop
        html += `
            <div class="product-card">
                <h3>${item.name}</h3>
                <p>Aantal: ${item.quantity}</p>
                <p>Prijs per stuk: €${price.toFixed(2)}</p>
                <button class="remove-btn" data-product-id="${item.id}">Verwijder</button>
            </div>
        `;
    });

    html += '</div>'; // Einde van de producten-grid

    // Voeg het totaalbedrag en de afrekenknop toe aan de HTML
    html += `
        <div style="margin-top: 38px; font-size:1.3rem; color:#20ff62;">
            <strong>Totaal bedrag: €<span id="cart-total">${total.toFixed(2)}</span></strong>
        </div>
        <button class="checkout-btn" style="margin-top:18px; padding:12px 28px; background:#20ff62; color:black; border-radius:12px; border:none; font-family:monospace; font-size:1.2rem; cursor:pointer; font-weight:bold;">Afrekenen</button>
    `;

    // Zet de samengestelde HTML in de winkelwagen-container
    cartNode.innerHTML = html;

    // Voeg voor iedere 'Verwijder' knop een click-handler toe
    // HOE: Elke knop heeft het product-ID en verwijdert telkens 1 van dit product
    document.querySelectorAll('.remove-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            const id = btn.getAttribute('data-product-id');
            removeOneFromCart(id); // Verwijder één exemplaar van dit product
        });
    });

    // Voor de afrekenknop: wanneer geklikt, toon een alert, sla bestelling op, maak winkelwagen leeg, update UI en badge
    document.querySelector('.checkout-btn').addEventListener('click', function () {
        alert('Bedankt voor je bestelling! (hier zou een echt betaalsysteem komen)');
        saveOrderToHistory(); // Sla alle bestelgegevens op voor later gebruik (in localStorage)
        localStorage.removeItem('cart');
        renderCart(); // Pagina herladen, nu staat er "winkelwagen is leeg"
        if (typeof updateCartCount === 'function') updateCartCount();
    });
}

// --------------------------
// Verwijdert ÉÉN stuk van een product uit de winkelwagen, of het product volledig als er nog maar één was
// --------------------------
// WAAROM: Gebruiker wil soms slechts aantal verlagen i.p.v. alles verwijderen
function removeOneFromCart(id) {
    let cart = JSON.parse(localStorage.getItem('cart')) || [];
    let idx = cart.findIndex(item => item.id === id);
    if (idx !== -1) {
        if (cart[idx].quantity > 1) {
            cart[idx].quantity -= 1; // Trek er eentje van af als er meer dan 1 zijn
        } else {
            // Verwijder het product als dit het laatste was
            cart.splice(idx, 1);
        }
    }
    // Sla de aangepaste winkelwagen op en update de UI/badge
    localStorage.setItem('cart', JSON.stringify(cart));
    renderCart();
    if (typeof updateCartCount === 'function') updateCartCount();
}

// ==========================
// Sla complete bestelling op in localStorage na het afrekenen
// ==========================
// WAT: Verzamelt alle besteldata (producten, hoeveelheid, prijs/stuk, totalen, BTW)
// WAAROM: Nodig voor bevestiging, klantenservice, factuur, statistiek of later synchroniseren met server
function saveOrderToHistory() {
    // Laad de huidige winkelwagen (wordt alleen aangeroepen als er items zijn)
    const cart = JSON.parse(localStorage.getItem('cart')) || [];
    if (cart.length === 0) return;

    let items = [];    // Hierin zet ik alle producten als {naam, hoeveelheid, prijs_per_stuk}
    let subtotal = 0;  // Hier tel ik alles bij elkaar op

    cart.forEach(item => {
        // Sla alleen de gewenste info op voor later: naam, aantal stuks, prijs per product
        const one = {
            name: item.name,
            amount: item.quantity,
            price_per_product: item.price
        };
        items.push(one);
        subtotal += (item.price || 0) * item.quantity;
    });

    // Sla de hele bestelling op incl. 21% BTW (Nederland)
    const order = {
        items: items,                         // Complete lijst met producten en hun aantallen/prijzen
        total_price: subtotal,                // Totaal vóór BTW
        total_price_vat21: +(subtotal * 1.21).toFixed(2) // Totaal incl. 21% BTW (afgerond op 2 decimalen)
    };

    // Sla ALLEEN de laatste bestelling op in localStorage (overschrijft eerdere)
    // Wil je bestelgeschiedenis, sla dan alle bestellingen in een array op
    localStorage.setItem('lastOrder', JSON.stringify(order));
    // Waarom: Kan later getoond worden als bon/factuur, voor administratie, export, enz.
}