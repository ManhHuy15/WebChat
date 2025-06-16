$(function () {
    IsLogin();
});

function IsLogin() {
    const token = localStorage.getItem('access-token');
    if (!token) {
        window.location.href = '/login';
        return false;
    } 

    if (isTokenExpired(token)) {
        refreshToken();
    }

    return true;
}