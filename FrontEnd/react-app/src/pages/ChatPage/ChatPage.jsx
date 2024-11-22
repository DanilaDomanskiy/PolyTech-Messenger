import { useState, useEffect } from "react";
import { useParams, useLocation } from "react-router-dom";
import io from "socket.io-client";
import "../ChatPage/ChatPage.css";

const socket = io("https://localhost:7205"); // Подключение к серверу WebSocket

const ChatPage = () => {
  const location = useLocation();
  const { chatId, userName } = location.state || {
    chatId: null,
    userName: "Unknow",
  };
  const [messages, setMessages] = useState([]); // Список сообщений
  const [newMessage, setNewMessage] = useState(""); // Новое сообщение

  // Форматирование времени
  const formatDate = (date) => {
    const options = {
      hour: "2-digit",
      minute: "2-digit",
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
    };
    return new Intl.DateTimeFormat("ru-RU", options).format(date);
  };

  useEffect(() => {
    // Присоединение к чату
    socket.emit("joinChat", chatId);

    // Получение сообщений из WebSocket
    socket.on("newMessage", (message) => {
      setMessages((prevMessages) => [...prevMessages, message]);
    });

    // Отключение сокета при выходе из компонента
    return () => {
      socket.emit("leaveChat", chatId);
      socket.off();
    };
  }, [chatId]);

  // Отправка сообщения
  const handleSendMessage = () => {
    if (newMessage.trim() === "") return;

    const messageData = {
      chatId,
      sender: userName,
      content: newMessage,
      timestamp: new Date(),
    };

    // Отправка сообщения на сервер
    socket.emit("sendMessage", messageData);

    // Отображение сообщения в чате
    setMessages((prevMessages) => [...prevMessages, messageData]);
    setNewMessage("");
  };

  // Отправка сообщения по нажатию клавиши Enter
  const handleKeyDown = (event) => {
    if (event.key === "Enter") {
      handleSendMessage();
    }
  };

  return (
    <div className="chat-page">
      <div className="chat-header">
        <h3>{userName}</h3>
      </div>

      <div className="chat-messages">
        {messages.map((message, index) => (
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
