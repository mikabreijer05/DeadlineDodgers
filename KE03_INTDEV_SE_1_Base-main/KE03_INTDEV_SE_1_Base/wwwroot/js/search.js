// =============================
// Matrix Market Cart Logic with Toast
// =============================
console.log("search.js loaded!");

document.addEventListener('DOMContentLoaded', function () {
    setupSiteSearch();
});

function setupSiteSearch() {
    const searchInput = document.getElementById('siteSearchInput');
    const resultsContainer = document.getElementById('siteSearchResults');

    if (!searchInput || !resultsContainer) return;

    let debounceTimer;

    searchInput.addEventListener('input', function () {
        clearTimeout(debounceTimer);

        const searchTerm = searchInput.value.trim();

        if (searchTerm.length < 2) {
            clearSearchResults(resultsContainer);
            return;
        }

        debounceTimer = setTimeout(function () {
            fetchSearchResults(searchTerm, resultsContainer);
        }, 250);
    });

    document.addEventListener('click', function (event) {
        if (!event.target.closest('.search-container')) {
            resultsContainer.style.display = 'none';
        }
    });

    searchInput.addEventListener('focus', function () {
        if (resultsContainer.innerHTML.trim() !== '') {
            resultsContainer.style.display = 'block';
        }
    });
}

async function fetchSearchResults(searchTerm, resultsContainer) {
    try {
        const response = await fetch(`/Producten?handler=Search&term=${encodeURIComponent(searchTerm)}`);

        if (!response.ok) {
            clearSearchResults(resultsContainer);
            return;
        }

        const results = await response.json();
        renderSearchResults(results, resultsContainer);
    } catch (error) {
        console.error('Search error:', error);
        clearSearchResults(resultsContainer);
    }
}

function renderSearchResults(results, resultsContainer) {
    resultsContainer.innerHTML = '';

    if (results.length === 0) {
        resultsContainer.innerHTML = '<div class="search-result-empty">Geen resultaten gevonden</div>';
        resultsContainer.style.display = 'block';
        return;
    }

    results.forEach(product => {
        const resultItem = document.createElement('a');
        resultItem.className = 'search-result-item';
        resultItem.href = `/ProductPagina/${product.id}`;

        const image = document.createElement('img');
        image.src = product.imageUrl || '/images/placeholder.png';
        image.alt = product.name;

        const details = document.createElement('div');

        const name = document.createElement('strong');
        name.textContent = product.name;

        const price = document.createElement('span');
        price.textContent = `€${product.price}`;

        details.appendChild(name);
        details.appendChild(price);

        resultItem.appendChild(image);
        resultItem.appendChild(details);

        resultsContainer.appendChild(resultItem);
    });

    resultsContainer.style.display = 'block';
}

function clearSearchResults(resultsContainer) {
    resultsContainer.innerHTML = '';
    resultsContainer.style.display = 'none';
}