// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
function toggleHidden(ids) {
    for (const id of ids)
        document.getElementById(id).classList.toggle('hidden');
}