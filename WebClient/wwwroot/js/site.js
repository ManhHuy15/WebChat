//$(function () {
//    IsLogin();
//});

function showNotification(message) {
    const time = new Date().toLocaleTimeString('vi-VN', { hour12: false });
    const alertId = 'alert-' + Date.now();
    const alertHtml = `
                 <div id="${alertId}" class="alert bg-white border rounded-3 p-2 mt-3 w-100" role="alert">
                    <div class="d-flex align-items-center justify-content-between">
                        <div>
                            <i class="bi bi-bell-fill text-warning"></i>
                        </div>
                        <div>
                            <span class="text-sm text-muted align-top" style="font-size: 0.9rem">${time}</span>
                            <button type="button" class="btn-close btn-sm" data-bs-dismiss="alert" aria-label="Close"></button>
                        </div>
                    </div>
                    <hr class="m-0" />
                    <span class="fs-6">${message}</span>
                </div>

            `;

    $('#snackbar').append(alertHtml);

    setTimeout(function () {
        $('#' + alertId).fadeOut(400, function () {
            $(this).remove();
        });
    }, 3000);
}

