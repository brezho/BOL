var ws;
function initializeWebSocket() {
    ws = new WebSocket("ws://localhost:81/mainSocket");

    ws.onopen = function () {
        // Web Socket is connected, send data using send()
    };

    ws.onmessage = function (evt) {
        var received_msg = evt.data;
        var msg = received_msg.split(":", 2);
        console.log("topic:" + msg[0]);
        console.log("payload:" + msg[1]);
      //  amplify.publish(msg[0], msg[1]);
    };

    ws.onclose = function () {
        // websocket is closed.
        console.log("Connection is closed...");
    };

    window.onbeforeunload = function (event) {
        socket.close();
    };
}
