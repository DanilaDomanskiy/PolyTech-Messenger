import React, { useState, useEffect } from "react";
import axios from "axios";
import "../../pages/SelectChat/SelectChat.css";

const ChatMessages = () => {
  const [chats, setChats] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchChats = async () => {
      try {
        const response = await axios.get(
          "https://localhost:7205/api/privateChat/all",
          {
            withCredentials: true,
          }
        );

        setChats(response.data);
      } catch (error) {
        // Обработка ошибок
        setError(error.response ? error.response.data.message : error.message);
      } finally {
        // Завершаем загрузку
        setLoading(false);
      }
    };

    fetchChats(); // Вызов функции получения чатов
  }, []);

  // Отображение во время загрузки
  if (loading) {
    return <p>Загрузка чатов...</p>;
  }

  // Обработка ошибок
  if (error) {
    return <p>Ошибка: {error}</p>;
  }

  return (
    <div id="privateMessages">
      <h2 className="title">Чаты</h2>
      {chats.length > 0 ? (
        <ul>
          {chats.map((chat) => (
            <li key={chat.id}>{chat.name}</li>
          ))}
        </ul>
      ) : (
        <p>Чаты не найдены.</p>
      )}
    </div>
  );
};

export default ChatMessages;
