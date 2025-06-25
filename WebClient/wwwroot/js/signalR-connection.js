"use strict";
$(function () {
    if (IsLogin()) {
        window.connection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5050/hubs/chat", {
                withCredentials: false,
                accessTokenFactory: () => localStorage.getItem("access-token")
            })
            .withAutomaticReconnect()
            .build();

        connection.on("ReceiveMessage", function (UserId) {
            console.log("ReceiveGroupMessage" + $('#chat-type').val() + " " + $('#chat-id').val() + " " + UserId);
            if ($('#chat-type').val() == "0" && $('#chat-id').val() == UserId) {
                fetchMessageUser(UserId);
            }
            //if (typeof window.onSignalRMessage === 'function') {
            //    console.log("onSignalRMessage");
            //    window.onSignalRMessage(UserId);
            //}
        });

        connection.on("ReceiveGroupMessage", function (groupId) {
            console.log("ReceiveGroupMessage" + $('#chat-type').val() + " " + $('#chat-id').val() + " " + groupId);
            if ($('#chat-type').val() == "1" && $('#chat-id').val() == groupId) {
                fetchMessageInGroup(groupId);
            }
        });
        connection.start()
        .then(() => {
            console.log("SignalR Successfully Connected")
                getMyGroups();
        })
        .catch((err) => {
            console.error("SignalR Connection Error: ", err);
            setTimeout(() => {
                connection.start();
            }, 2000);
        });
    }
    function getMyGroups() {
        $.ajax({
            url: "http://localhost:5050/api/Group/my-group",
            method: "GET",
            success: async function (response) {
                if (response.status == 200) {
                    for (var i = 0; i < response.data.length; i++) {
                        await connection.invoke("JoinGroup", response.data[i].name);
                    }
                }
            },
            error: function (response) {
                console.log(response);
            }
        })
    }
      
});