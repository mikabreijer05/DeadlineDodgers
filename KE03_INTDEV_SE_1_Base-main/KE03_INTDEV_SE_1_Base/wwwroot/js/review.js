document.addEventListener('DOMContentLoaded', function () {
    const ratingLabels = document.querySelectorAll('.star-rating-input label');
    const ratingInputs = document.querySelectorAll('.star-rating-input input');
    const feedback = document.getElementById('starRatingFeedback');

    if (!feedback) {
        return;
    }

    function getSelectedRatingText() {
        const selectedInput = document.querySelector('.star-rating-input input:checked');

        if (!selectedInput) {
            return 'Geen beoordeling gekozen';
        }

        return selectedInput.value === '1'
            ? '1 ster gekozen'
            : `${selectedInput.value} sterren gekozen`;
    }

    ratingLabels.forEach(function (label) {
        label.addEventListener('mouseenter', function () {
            const rating = label.dataset.rating;

            feedback.textContent = rating === '1'
                ? 'klik om 1 ster te kiezen'
                : `klik om ${rating} sterren te kiezen`;
        });

        label.addEventListener('mouseleave', function () {
            feedback.textContent = getSelectedRatingText();
        });

        label.addEventListener('click', function () {
            const rating = label.dataset.rating;

            feedback.textContent = rating === '1'
                ? '1 ster gekozen'
                : `${rating} sterren gekozen`;
        });
    });

    ratingInputs.forEach(function (input) {
        input.addEventListener('change', function () {
            feedback.textContent = getSelectedRatingText();
        });
    });

    feedback.textContent = getSelectedRatingText();
});