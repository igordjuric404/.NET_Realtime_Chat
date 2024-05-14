import React, { useState } from 'react';
import UserList from "./components/UserList";
import ChatWindow from "./components/ChatWindow";

const App = () => {
    const [users] = useState(['User1', 'User2', 'User3']); // Sample users
    const [currentUser] = useState('User1'); // Static current user for example
    const [selectedUser, setSelectedUser] = useState(null);

    const handleSelectUser = (user) => {
        setSelectedUser(user);
    };

    return (
        <div>
            <h1>Real-Time Chat</h1>
            <UserList users={users} onSelectUser={handleSelectUser} />
            {selectedUser && <ChatWindow user={selectedUser} currentUser={currentUser} />}
        </div>
    );
};

export default App;
