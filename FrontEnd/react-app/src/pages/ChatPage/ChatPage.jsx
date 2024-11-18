import { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import io from "socket.io-client";
import "../ChatPage/ChatPage.css";

const ChatPage = () => {
  return (
    <div>
      <h2>Чат с пользователем (ID: {chatId})</h2>
      <div>
        {messages.map((message, index) => (
          <div key={index}>
            <p>{message.content}</p>
          </div>
        ))}
      </div>
      <input
        type="text"
        value={newMessage}
        onChange={(e) => setNewMessage(e.target.value)}
        placeholder="Введите сообщение..."
      />
      <button onClick={handleSendMessage}>Отправить</button>
    </div>
  );
};

export default ChatPage;
