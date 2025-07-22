var chatName = $('#chat-name');
var chatAvatar = $('#chat-avatar');
var chatId = $('#chat-id');
var chatType = $('#chat-type');

$('#chatList').on('click', '.chat-item', function (e) {
    e.preventDefault();
    $('.chat-item').removeClass('active');
    $(this).addClass('active');
});


function checkSpam(reciverId) {
    var messageSpam = `${reciverId}_${Date.now()}`;
    var spamArr = sessionStorage.getItem("spamArr");

    if (spamArr == null) {
       sessionStorage.setItem("spamArr", JSON.stringify([messageSpam]));
       return true;
    }
    
    const sessionSpamArr = JSON.parse(spamArr);

    var lastMessage = sessionSpamArr[sessionSpamArr.length - 1];
    var lastMessageSplit = lastMessage.split('_');
    if (lastMessageSplit[0] !== reciverId) {
        sessionStorage.setItem("spamArr", JSON.stringify([messageSpam]));
        return false;
    }

    if (sessionSpamArr.length < 2) {
        sessionSpamArr.push(messageSpam);
        sessionStorage.setItem("spamArr", JSON.stringify(sessionSpamArr));
        return true;
    }    
   

    var diffDate = Date.now() - parseInt(lastMessageSplit[1]);
    if (diffDate < 30000) {
        showNotification("Please don't spam message");
        return false;
    }
    
    sessionStorage.setItem("spamArr", JSON.stringify([messageSpam]));
    
    return true;
}

$('#btn-send').on('click', async function (e) {
    e.preventDefault();
    const reciverId = chatId.val();
    const typeChat = chatType.val();
    var chatList = $('#chat-messages');
    var message = $('#chat-input').val();
    if (!reciverId) return;
    if (message == '' && selectedFiles.length == 0) return;

    if (!checkSpam(reciverId)) return;

    try {
        if (message != '') {
            var chatItem = $(` <div class="d-flex flex-column text-black align-items-end">
                                <div class="my-mes-bubble">${message}</div>
                            </div>
                        `);
        }

        chatList.append(chatItem);

        var formData = new FormData();
        formData.append('receiverId', reciverId);
        formData.append('content', message);
        selectedFiles.forEach((data) => {
            formData.append('files', data.file);
            var chatFileItem = $(`
                    <div class="d-flex flex-column text-black align-items-end  position-relative">
                            ${renderPreviewFileBubble(data.file, data.preview, reciverId)}
                        <div class="spinner-border text-primary spinner-border-sm position-absolute" style="right: 5px;bottom: 10px;" role="status">
                            <span class="visually-hidden">Loading...</span>
                        </div>
                    </div>

                `);
            chatList.append(chatFileItem);
        });
        scrollToBottom();
        sendMessage(reciverId, formData, typeChat);
        console.log("Gửi message thanh cong");

        $('#chat-input').val('');
        selectedFiles = [];
        renderPreviews()
    } catch (err) {
        console.error('Send MessageTo User error:', err);
    }
});

window.openChat = function (id, name, avatar, type) {
    var isHidden = $('#chat-detail').hasClass('d-none');
    if (!isHidden) {
        $('#chat-detail').addClass('d-none');
    }
    chatAvatar.attr('src', avatar);
    chatName.text(name);
    chatId.val(id);
    chatType.val(type);
    if (type == 0) {
        fetchMessageUser(id);
    } else if (type == 1) {
        fetchMessageInGroup(id);
    }
};
function renderPreviewFileBubble(item, itemPreview, reciverId) {
    var type = item.type.split("/")[0];
    switch (type) {
        case 0:
            return `<div class=" text-break my-mes-bubble">
                            ${item.content}
                        </div>`;
        case "image":
            return `<div class=" text-break my-file-bubble">
                                <img src="${itemPreview}" class="m-h-12" alt="Image">
                        </div>`;
        case "video":
            return `<div class=" text-break my-file-bubble">
                    <video  class="m-h-12" >
                                <source src="${itemPreview}"   type="video/mp4" />
                    </video>
                </div>`;
        case 3:
            return `<div class=" text-break my-file-bubble">
                    <audio >
                                <source src="${itemPreview}"  type="audio/mpeg" />
                    </audio>
                </div>`;
        default:
            var filename = item.name;
            const dotIndex = item.name.lastIndexOf('.');
            let name = item.name.substring(0, dotIndex);
            let ext = item.name.substring(dotIndex);
            if (name.length > 10) {
                name = name.substring(0, 10) + '...';
                filename = name + ext;
            }
            return `<div class="text-break my-file-bubble" style="background-color: #2b2b2b">
                            <a href="#" target="_blank">
                                <i class="bi bi-file-earmark-text-fill ms-2 fs-2 text-white"></i>
                                <span class="text-white me-2 fw-bold "> ${filename} </span>
                            </a>
                    </div>`;
    }
}

function sendMessage(reciverId, message, chatType) {
        var senderId = localStorage.getItem('id');
        $.ajax({
            url: API_URL + `/api/Message/send-message/${chatType}/${reciverId}`,
            type: 'POST',
            processData: false,
            contentType: false,
            data: message,
            success: async function (result) {
                if (result.success) {
                    if (chatType == 0) {
                        await connection.invoke('SendMessage', senderId, reciverId);
                        fetchMessageUser(reciverId);
                    } else if (chatType == 1) {
                        await connection.invoke('SendMessageToGroup', chatName.text(), chatId.val());
                        fetchMessageInGroup(reciverId);
                    }
                } else {
                    console.error(result.message);
                }
            },
            error: function (error) {
                console.error("Error fetching chat items:", error);
            }
        });
    }

function scrollToBottom() {
    var chatMessages = document.getElementById('chat-messages');
    if (chatMessages) {
        chatMessages.scrollTop = chatMessages.scrollHeight;
    }
}

function timeToNow(time) {
    if (time == null) return "";
    var inputDate = new Date(time);
    var now = new Date();
    var diffMs = now - inputDate;
    var diffSec = Math.floor(diffMs / 1000);
    var diffMin = Math.floor(diffSec / 60);
    var diffHour = Math.floor(diffMin / 60);
    var diffDay = Math.floor(diffHour / 24);

    if (diffHour >= 24) {
        var day = inputDate.getDate().toString().padStart(2, '0');
        var month = (inputDate.getMonth() + 1).toString().padStart(2, '0');
        return `${day}/${month}`;
    } else if (diffHour >= 1) {
        return `${diffHour} giờ`;
    } else if (diffMin >= 1) {
        return `${diffMin} phút`;
    } else {
        return `Vừa xong`;
    }
}

function fetchMessageUser(reciverId) {
    $.ajax({
        url: API_URL + `/api/Message/get-messages/user/${reciverId}`,
        type: 'GET',
        contentType: 'application/json',
        success: function (result) {
            if (result.success) {
                var data = result.data;
                var chatList = $('#chat-messages');
                chatType.val(0);
                chatList.empty();
                data.forEach(function (item, index) {
                    var chatItem = $(`
                            <div class="d-flex flex-column text-black align-items-end">
                                ${item.receiver.userId != reciverId ? `<div class="align-self-start text-white"><small>${item.sender.fullName}</small></div>` : ''}
                                ${renderMessageBubble(item, reciverId)}
                                ${index == data.length - 2 ? `<small class="message-time">${timeToNow(item.sentAt)}</small>` : ''}
                            </div>
                        `);
                    chatList.append(chatItem);

                    if (item.receiver.userId == reciverId) {
                        chatAvatar.attr('src', item.receiver.avatar);
                        chatName.text(item.receiver.fullName);
                        chatId.val(item.receiver.userId);
                    }
                });

                $('#loading-spinner').hide();
                $('#chat-messages').children().not('#loading-spinner').show();

                setTimeout(() => {
                    scrollToBottom();
                }, 100);
            } else {
            }
        },
        error: function (error) {
            console.error("Error fetching chat items:", error);
        }
    });
}
function renderMessageBubble(item, reciverId) {
    switch (item.type) {
        case 0:
            return `<div class=" text-break ${item.receiver.userId == reciverId ? 'my-mes-bubble' : 'other-mes-bubble'}">
                            ${item.content}
                        </div>`;
        case 1:
            return `<div class=" text-break cursor-pointer ${item.receiver.userId == reciverId ? 'my-file-bubble' : 'other-file-bubble'}" onclick="OpenFile('${item.content}','${item.type}')">
                            <img src="${item.content}"  class="m-h-12" alt="Image">
                        </div>`;
        case 2:
            return `<div class="text-break cursor-pointer ${item.receiver.userId == reciverId ? 'my-file-bubble' : 'other-file-bubble'}" onclick="OpenFile('${item.content}','${item.type}')">
                    <video  class="m-h-12" >
                        <source src="${item.content}"   type="video/mp4" />
                    </video>
                </div>`;
        case 3:
            return `<div class=" text-break ${item.receiver.userId == reciverId ? 'my-file-bubble' : 'other-file-bubble'}">
                    <audio controls="controls">
                        <source src="${item.content}"  type="audio/mpeg" />
                    </audio>
                </div>`;
        case 4:
            const lastSegment = item.content.split('/').pop();
            var filename = lastSegment.replace(/_(.*?)\./, '.')
            const dotIndex = filename.lastIndexOf('.');
            let name = filename.substring(0, dotIndex);
            let ext = filename.substring(dotIndex);
            if (name.length > 10) {
                name = name.substring(0, 10) + '...';
                filename = name + ext;
            }
            return `<div class="text-break ${item.receiver.userId == reciverId ? 'my-file-bubble' : 'other-file-bubble'}" style="background-color: #2b2b2b">
                    <a href="${lastSegment}" download target="_blank">
                        <i class="bi bi-file-earmark-text-fill ms-2 fs-2 text-white"></i>
                        <span class="text-white me-2 fw-bold "> ${filename} </span>
                    </a>
                </div>`;
    }
}
function fetchMessageInGroup(groupId) {
    var myId = localStorage.getItem('id')
    $.ajax({
        url: API_URL + `/api/Message/get-messages/group/${groupId}`,
        type: 'GET',
        contentType: 'application/json',
        success: function (result) {
            var chatList = $('#chat-messages');
            groupId = 10;
            if (result.success) {
                var data = result.data;
                chatType.val(1);
                chatList.empty();
                data.forEach(function (item, index) {
                    var chatItem = $(`
                                <div class="d-flex flex-column text-black align-items-end">
                                        ${item.sender.userId != myId ? `<div class="align-self-start text-white"><small>${item.sender.fullName}</small></div>` : ''}
                                        ${renderMessageGroupBubble(item, myId)}
                                    ${index == data.length - 2 ? `<small class="message-time">${timeToNow(item.sentAt)}</small>` : ''}
                                </div>
                            `);
                    chatList.append(chatItem);
                });

                chatAvatar.attr('src', data[0].group.avatar);
                chatName.text(data[0].group.name);
                chatId.val(data[0].group.groupId);

                $('#loading-spinner').hide();
                $('#chat-messages').children().not('#loading-spinner').show();

                setTimeout(() => {
                    scrollToBottom();
                }, 100);
            } else {
                chatList.empty();
            }
        },
        error: function (error) {
            console.error("Error fetching chat items in group:", error);
        }
    });
}
function renderMessageGroupBubble(item, myId) {
    switch (item.type) {
        case 0:
            return `<div class=" text-break ${item.sender.userId == myId ? 'my-mes-bubble' : 'other-mes-bubble'}">
                            ${item.content}
                        </div>`;
        case 1:
            return `<div class=" text-break cursor-pointer ${item.sender.userId == myId ? 'my-file-bubble' : 'other-file-bubble'}" onclick="OpenFile('${item.content}','${item.type}')">
                            <img src="${item.content}"  class="m-h-12" alt="Image">
                        </div>`;
        case 2:
            return `<div class=" text-break cursor-pointer ${item.sender.userId == myId ? 'my-file-bubble' : 'other-file-bubble'}" onclick="OpenFile('${item.content}','${item.type}')">
                    <video class="m-h-12" >
                        <source src="${item.content}"  type="video/mp4" />
                    </video>
                </div>`;
        case 3:
            return `<div class=" text-break ${item.sender.userId == myId ? 'my-file-bubble' : 'other-file-bubble'}">
                    <audio controls="controls">
                        <source src="${item.content}"  type="audio/mpeg" />
                    </audio>
                </div>`;
        case 4:
            const lastSegment = item.content.split('/').pop();
            var filename = lastSegment.replace(/_(.*?)\./, '.')
            const dotIndex = filename.lastIndexOf('.');
            let name = filename.substring(0, dotIndex);
            let ext = filename.substring(dotIndex);
            if (name.length > 10) {
                name = name.substring(0, 10) + '...';
                filename = name + ext;
            }
            return `<div class="text-break ${item.sender.userId == myId ? 'my-file-bubble' : 'other-file-bubble'}" style="background-color: #2b2b2b">
                    <a href="${lastSegment}" download target="_blank">
                        <i class="bi bi-file-earmark-text-fill ms-2 fs-2 text-white"></i>
                        <span class="text-white me-2 fw-bold "> ${filename} </span>
                    </a>
                </div>`;
    }
}

// window.onSignalRMessage = function (userId) {
//     const currentChatId = $('#chat-id').val();
//     const currentChatType = $('#chat-type').val();

//     if (currentChatType == "0" && currentChatId == userId){
//         fetchMessageUser(userId);
//     }else if ( currentChatType == "1" ){

//     }
// };

let selectedFiles = [];

function getFilePreviewHtml(file, previewUrl, index) {
    let inner;
    if (file.type.startsWith("image/")) {
        inner = `<img src="${previewUrl}">`;
    } else if (file.type.startsWith("video/")) {
        inner = `<video src="${previewUrl}" muted autoplay loop></video>`;
    } else {
        const filename = file.name.length > 8 ? file.name.slice(0, 5) + "..." : file.name;
        return `<div class="preview-file-box">
                            <div class="preview-file-text d-flex align-items-center justify-content-center h-100">
                                <i class="bi bi-file-earmark fs-3 text-white"></i>
                                <span class="text-white fw-bold">${filename}</span>
                            </div>
                            <button type="button" class="btn-close btn-close-white btn-sm btn-remove-file " data-index="${index}"></button>
                        </div>`;
    }
    return `
            <div class="preview-box">
                ${inner}
                 <button type="button" class="btn-close btn-close-white btn-sm btn-remove-file text-white" data-index="${index}"></button>
            </div>`;
}

function renderPreviews() {
    const wrapper = $('#preview-wrapper');
    wrapper.empty();

    selectedFiles.forEach((fileData, index) => {
        const html = getFilePreviewHtml(fileData.file, fileData.preview, index);
        wrapper.append(html);
    });
}

$('#btn-select-file').on('click', () => $('#file-input').click());

$('#file-input').on('change', function () {
    const files = Array.from(this.files);
    files.forEach(file => {
        if (file.size > 1024 * 1024 * 10) {
            alert("File không được quá 10MB");
            return;
        }
        const reader = new FileReader();
        reader.onload = function (e) {
            selectedFiles.push({
                file: file,
                preview: e.target.result
            });
            renderPreviews();
        };
        reader.readAsDataURL(file);
    });
    $(this).val('');
});

$('#preview-wrapper').on('click', '.btn-remove-file', function () {
    const index = $(this).data('index');
    selectedFiles.splice(index, 1);
    renderPreviews();
});

function OpenFile(url, type) {
    console.log("File type: " + type);

    let contentHtml = '';
    if (type === '1') {
        contentHtml = `
                <img src="${url}"
                        style="max-width: 100vw; max-height: 100vh; object-fit: contain;"
                        class="img-fluid  bg-dark"
                        alt="Image" />
            `;
    } else if (type === '2') {
        contentHtml = `
                <video src="${url}"
                        style="max-width: 100vw; max-height: 100vh; object-fit: contain;"
                        class="img-fluid  bg-dark"
                        controls autoplay></video>
            `;
    } else {
        return;
    }

    const modalContent = $('#view-full-file');
    modalContent.empty().append(contentHtml);

    $("#file-viewer-modal").modal("show");
}