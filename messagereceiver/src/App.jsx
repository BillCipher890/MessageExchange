import { useEffect, useState, useRef } from 'react';
import './App.css'

function App() {
    const [messages, setMessages] = useState([]);
    const ws = useRef(null);

    useEffect(() => {
        ws.current = new WebSocket('wss://localhost:7034/api/Message/ws');
        ws.current.onopen = () => console.log('Connected to WebSocket server');
        ws.current.onmessage = (event) => {
            const newMessage = event.data;
            setMessages((prevMessages) => [...prevMessages, newMessage]);
        };
        ws.current.onclose = () => console.log('Disconnected from WebSocket server');

        return () => {
            if (ws.current) {
                ws.current.close();
            }
        };
    }, []);

    return (
        <div>
            <h2>Received messages</h2>
            <ul>
                {messages.map((msg, index) => (
                    <li key={index}>{msg}</li>
                ))}
            </ul>
        </div>
    );
}

export default App
