document.addEventListener("DOMContentLoaded", function () {
    const canvas = document.getElementById("videoCanvas");
    const ctx = canvas.getContext("2d");
    const startButton = document.getElementById("startButton");
    const stopButton = document.getElementById("stopButton");
    let socket;

    startButton.addEventListener("click", async function () {
        try {
            const response = await fetch('http://localhost:5039/api/websocket/start', { method: 'POST', credentials: 'include' });
            if (!response.ok) throw new Error('Failed to start streaming');
            console.log(await response.text());

            socket = new WebSocket("ws://localhost:5039/ws/client");
            socket.binaryType = "arraybuffer";

            socket.onopen = function () {
                console.log("Connected to WebSocket server.");
            };

            socket.onmessage = function (event) {
                console.log("Received data of size: " + event.data.byteLength);
                if (event.data instanceof ArrayBuffer) {
                    const imageData = new Uint8Array(event.data);
                    console.log("Processed image data of size:", imageData.length);

                    const blob = new Blob([imageData], { type: "image/png" });
                    const url = URL.createObjectURL(blob);

                    const img = new Image();
                    img.onload = function () {
                        ctx.clearRect(0, 0, canvas.width, canvas.height); // Clear before drawing
                        ctx.drawImage(img, 0, 0, canvas.width, canvas.height);
                        URL.revokeObjectURL(url); // Clean up
                    };
                    img.src = url;
                }
            };

            socket.onclose = function (event) {
                img.src = new Image();
                console.log("Disconnected from WebSocket server.");
            };

            socket.onerror = function (error) {
                console.log("WebSocket error:", error);
            };
        } catch (err) {
            console.log('Error:', err);
        }
    });

    stopButton.addEventListener("click", async function () {
        if (socket) {
            socket.close();
        }

        try {
            const response = await fetch('http://localhost:5039/api/websocket/stop', { method: 'POST', credentials: 'include' });
            if (!response.ok) throw new Error('Failed to stop streaming');
            console.log(await response.text());
        } catch (err) {
            console.log('Error:', err);
        }
    });
});