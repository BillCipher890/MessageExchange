import { useState } from 'react';
import './App.css';

function App() {
    const [startDate, setStartDate] = useState('');
    const [endDate, setEndDate] = useState('');
    const [messages, setMessages] = useState([]);

    const getMessages = async () => {
        const url = new URL('https://localhost:7034/api/Message/GetMessages');
        url.searchParams.append("startDate", startDate);
        url.searchParams.append("endDate", endDate);

        try {
            const response = await fetch(url);

            if (response.ok) {
                const responseData = await response.json();
                setMessages(responseData);
            } else {
                console.error('Ошибка при проверке пароля');
            }
        } catch (error) {
            console.error('Exception:', error);
        }
    };

    return (
        <div>
            <h2>Received messages for time</h2>
            <table>
                <thead>
                    <tr>
                        <th>Number</th>
                        <th>Text</th>
                        <th>Date</th>
                    </tr>
                </thead>
                <tbody>
                    {messages.map((msg, index) => (
                        <tr key={index}>
                            <td>{msg.id}</td>
                            <td>{msg.body}</td>
                            <td>{msg.createdAt}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
            <input
                type="datetime-local"
                value={startDate}
                onChange={(e) => setStartDate(e.target.value)}
                placeholder="Enter start datetime"
            />
            <input
                type="datetime-local"
                value={endDate}
                onChange={(e) => setEndDate(e.target.value)}
                placeholder="Enter end datetime"
            />
            <button onClick={getMessages}>Get</button>
        </div>
    );
}


export default App;