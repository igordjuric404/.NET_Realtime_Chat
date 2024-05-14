import React, { useState, useEffect } from 'react';
import { sendMessage, connection } from '../signalRService';

const ChatWindow = ({ user, currentUser }) => {
    const [message, setMessage] = useState('');
    const [messages, setMessages] = useState([]);

    useEffect(() => {
        const handleReceiveMessage = (sender, receiver, receivedMessage) => {
            if ((sender === user && receiver === currentUser) || (sender === currentUser && receiver === user)) {
                setMessages(messages => [...messages, { sender, message: receivedMessage }]);
            }
        };

        connection.on("ReceiveMessage", handleReceiveMessage);

        return () => {
            connection.off("ReceiveMessage", handleReceiveMessage);
        };
    }, [user, currentUser]);

    const handleSend = () => {
        sendMessage(currentUser, user, message);
        setMessages([...messages, { sender: currentUser, message }]);
        setMessage('');
    };

    return (
        <div>
            <h2>Chat with {user}</h2>
            <div>
                {messages.map((msg, index) => (
                    <p key={index}><strong>{msg.sender}:</strong> {msg.message}</p>
                ))}
            </div>
            <input value={message} onChange={e => setMessage(e.target.value)} placeholder="Type a message..." />
            <button onClick={handleSend}>Send</button>
        </div>
    );
};

export default ChatWindow;
