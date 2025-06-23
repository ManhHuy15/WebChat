// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
'use strict';

$(function () {
    IsLogin();
    $(document).on("ajaxSend", function (event, jqxhr, settings) {
        if (settings.url === 'http://localhost:5050/api/auth/refresh-token') {
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
        url: 'http://localhost:5050/api/auth/refresh-token',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(localStorage.getItem('refresh-token')),
        success: function (response) {
            if (response.status == 200) {
                localStorage.setItem('access-token', response.data.accessToken);
            } else {
                localStorage.removeItem('refresh-token');
                localStorage.removeItem('access-token');
                localStorage.removeItem('email');
                localStorage.removeItem('expires');
                localStorage.removeItem('id');
                window.location.href = '/login';
            }
        }
    });
}