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
        connection.on("ReceiveMessage", function () {
            console.log("ReceiveMessage");
        });
        connection.start()
            .then(() => console.log("SignalR Successfully Connected"))
            .catch((err) => {
                console.error("SignalR Connection Error: ", err);
                setTimeout(() => {
                    connection.start();
                }, 2000);
            });
    }
      
});