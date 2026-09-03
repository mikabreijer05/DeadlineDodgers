document.addEventListener('DOMContentLoaded', function () {
    const addCategoryButton = document.getElementById('addCategoryButton');
    const newCategoryNameInput = document.getElementById('newCategoryName');
    const categoryDropdown = document.getElementById('categoryDropdown');
    const addCategoryMessage = document.getElementById('addCategoryMessage');

    if (!addCategoryButton || !newCategoryNameInput || !categoryDropdown) {
        return;
    }

    addCategoryButton.addEventListener('click', async function () {
        const categoryName = newCategoryNameInput.value.trim();

        if (!categoryName) {
            addCategoryMessage.textContent = 'Vul eerst een categorienaam in.';
            return;
        }

        const formData = new FormData();
        formData.append('categoryName', categoryName);

        try {
            const response = await fetch('/Product/AddCategory', {
                method: 'POST',
                body: formData
            });

            if (!response.ok) {
                addCategoryMessage.textContent = 'Categorie kon niet worden toegevoegd.';
                return;
            }

            const category = await response.json();

            const option = document.createElement('option');
            option.value = category.id;
            option.textContent = category.name;
            option.selected = true;

            categoryDropdown.appendChild(option);
            categoryDropdown.value = category.id;

            newCategoryNameInput.value = '';
            addCategoryMessage.textContent = 'Categorie toegevoegd en geselecteerd.';
        } catch {
            addCategoryMessage.textContent = 'Er is iets misgegaan.';
        }
    });
});