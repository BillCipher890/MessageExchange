import { useState } from 'react';
import './App.css';

function App() {
    const [message, setMessage] = useState('');

    const sendMessage = async () => {
        const url = new URL('https://localhost:7034/api/Message/SaveMessage');
        url.searchParams.append("message", message);

        try {
            await fetch(url);
            setMessage('');
        } catch (error) {
            console.error('Exception:', error);
        }
    };

    return (
        <div>
            <input
                type="text"
                value={message}
                onChange={(e) => setMessage(e.target.value)}
                placeholder="Enter message"
            />
            <button onClick={sendMessage}>Send</button>
        </div>
    );
}


export default App;