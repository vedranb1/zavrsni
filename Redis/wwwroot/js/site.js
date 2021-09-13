// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function addToCart(gameId) {
    $.ajax({
        type: "POST",
        url: "/Shop/AddToCart",
        data: JSON.stringify({ id: gameId }),
        dataType: 'json',
        contentType: 'application/json',
        success: function (response) {
            location.reload();
        },
        failure: function (response) {
            console.log(response);
        },
        error: function (response) {
            console.log(response);
        }
    });
}

function removeFromCart(gameId) {
    $.ajax({
        type: "POST",
        url: "/Shop/RemoveFromCart",
        data: JSON.stringify({ id: gameId }),
        dataType: 'json',
        contentType: 'application/json',
        success: function (response) {
            location.reload();
        },
        failure: function (response) {
            console.log(response);
        },
        error: function (response) {
            console.log(response);
        }
    });
}

function editGame(id, name, genre, price, imageUrl) {
    $('#addOrUpdateGameModal').modal('show');

    $('#Id').val(id);
    $('#Name').val(name);
    $('#Genre').val(genre);
    $('#Price').val(price);
    $('#ImageUrl').val(imageUrl);
}