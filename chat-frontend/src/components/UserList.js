import React from 'react';

const UserList = ({ users, onSelectUser }) => {
    return (
        <div>
            <h2>User List</h2>
            <ul>
                {users.map((user, index) => (
                    <li key={index} onClick={() => onSelectUser(user)}>
                        {user}
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default UserList;
