import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5003/hubs/chathub") // Update with your actual server URL
    .configureLogging(signalR.LogLevel.Information)
    .build();

export const startConnection = async () => {
    if (connection.state === signalR.HubConnectionState.Disconnected) {
        try {
            await connection.start();
            console.log("Connected to SignalR hub");
        } catch (err) {
            console.error("SignalR Connection Error: ", err);
            setTimeout(startConnection, 5000);
        }
    }
};

export const sendMessage = (sender, receiver, message) => {
    if (connection.state === signalR.HubConnectionState.Connected) {
        connection.invoke("SendMessage", sender, receiver, message).catch(err => console.error(err));
    } else {
        console.error("Cannot send message when not connected.");
    }
};

export const isConnected = () => {
    return connection.state === signalR.HubConnectionState.Connected;
};

export { connection }; // Export the connection object
export default connection; // Optional: This export can be used to maintain the current import style
