// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Debug helper: ensure Support forms actually submit and surface console logs
document.addEventListener('submit', function (e) {
    try {
        console.log('Form submit event captured for', e.target);
    } catch { }
});

// Defensive capture-phase handler: if a submit button is clicked, prevent other handlers from blocking the submit
document.addEventListener('click', function (e) {
    var target = e.target;
    if (!target) return;

    // normalize in case of nested elements inside the button
    var el = target.closest && target.closest('button, input[type="submit"]');
    if (!el) return;

    var form = el.form;
    if (!form) return;

    // stop other listeners from preventing the submission
    try {
        e.stopImmediatePropagation();
    } catch { }
    // allow the native submit to proceed
    // (do not call preventDefault here)

    console.log('Submit click detected on', el, 'for form', form);
}, true);
