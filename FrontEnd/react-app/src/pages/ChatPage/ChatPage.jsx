import { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import io from "socket.io-client";
import "../ChatPage/ChatPage.css";

const socket = io("https://localhost:7205");

const ChatPage = () => {
  const { chatId } = useParams();
  const [message, setMessage] = useState([]);
  const [newMessage, setNewMessage] = useState("");
  const [userName, setUserName] = useState("user");

  const formatDate = (date) => {
    const options = {
      hour: "2-digit",
      minute: "2-digit",
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
    };
    return new Intl.DateTimeFormat("ru-Ru", options).format(date);
  };

  useEffect(() => {
    socket.emit("joinChat", chatId);
    socket.on("newMessage", (message) => {
      setMessage((prevMessage) => [...prevMessage, message]);
    });

    return () => {
      socket.emit("leaveChat", chatId);
      socket.off();
    };
  }, [chatId]);

  const handleSendMessage = () => {
    if (newMessage.trim === "") return;

    const messageData = {
      chatId,
      sender: userName,
      content: newMessage,
      timestamp: new Date(),
    };

    socket.emit("sendMessage", messageData);
    setMessage((prevMessage) => [...prevMessage, messageData]);
    setNewMessage("");
  };

  const handleKeyDown = (event) => {
    if (event.key === "Enter") {
      handleSendMessage();
    }
  };

  return (
    <div className="chat-page">
      <div className="chat-header">
        <h2>Чат с пользователем (ID: {chatId})</h2>
      </div>

      <div className="chat-messages">
        {message.map((message, index) => (
          <div
            key={index}
            className={`message ${
              message.sender === userName ? "my-message" : "other-message"
            }`}
          >
            <p>{message.content}</p>
            <span className="timestamp">
              {formatDate(new Date(message.timestamp))}
            </span>
          </div>
        ))}
      </div>

      <div className="chat-input">
        <input
          type="text"
          value={newMessage}
          onChange={(e) => setNewMessage(e.target.value)}
          onKeyDown={handleKeyDown}
          placeholder="Введите сообщение..."
        />
        <button onClick={handleSendMessage}>Отправить</button>
      </div>
    </div>
  );
};

export default ChatPage;
