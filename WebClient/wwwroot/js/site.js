$(function () {
    IsLogin();
});

function IsLogin() {
    const token = localStorage.getItem('access-token');
    if (!token) {
        window.location.href = '/login';
    } 

    if (isTokenExpired(token)) {
        refreshToken();
    }
}