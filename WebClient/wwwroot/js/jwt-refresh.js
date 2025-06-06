// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
'use strict';

$(function () {
    $(document).on("ajaxSend", function (event, jqxhr, settings) {
        if (settings.url === '/api/auth/refresh-token') {
            return;
        }
        var token = localStorage.getItem('access-token');
        if (token) {
            if (isTokenExpired(token)) {
                event.preventDefault();

                refreshToken().then(function () {
                    var newToken = localStorage.getItem('access-token');
                    settings.beforeSend = function (xhr) {
                        xhr.setRequestHeader('Authorization', 'Bearer ' + newToken);
                    };
                    $.ajax(settings);
                });
            } else {
                jqxhr.setRequestHeader('Authorization', 'Bearer ' + token);
            }
        }
    });
});

function isTokenExpired(token) {
    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        const currentTime = Math.floor(Date.now() / 1000);
        return payload.exp < currentTime;
    } catch (e) {
        return true;
    }
}

function refreshToken() {
    return $.ajax({
        url: '/api/auth/refresh-token',
        method: 'POST',
        data: {
            refreshToken: localStorage.getItem('refresh-token')
        }
    }).then(function (response) {
        localStorage.setItem('refresh-token', response.data.refreshToken);
        localStorage.setItem('access-token', response.data.accessToken);
        localStorage.setItem('email', JSON.stringify(response.data.email));
    });
}