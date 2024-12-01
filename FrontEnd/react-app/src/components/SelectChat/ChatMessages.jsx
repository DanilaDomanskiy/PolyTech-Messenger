import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import { useSignalR } from "../../SignalRProvider";
import "../../pages/SelectChat/SelectChat.css";

const ChatMessages = () => {
  const [chats, setChats] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const connection = useSignalR();
  const navigate = useNavigate();

  useEffect(() => {
    const connectSignalR = async () => {
      if (connection) {
        try {
          await connection.on(
            "ReceivePrivateChatMessages",
            (chatId, message) => {
              setChats((prevChats) =>
                prevChats.map((chat) =>
                  chat.id === chatId
                    ? {
                        ...chat,
                        lastMessage: message, // Обновляем последнее сообщение
                        unreadMessagesCount: chat.unreadMessagesCount + 1, // Увеличиваем количество непрочитанных
                      }
                    : chat
                )
              );
            }
          );
        } catch (err) {
          console.error("Ошибка подключения:", err);
        }
      }
    };

    connectSignalR();

    // Отключение соединения при размонтировании компонента
    return () => {
      if (connection) {
        connection.off("ReceivePrivateChatMessages");
      }
    };
  }, [connection]);

  useEffect(() => {
    const fetchChats = async () => {
      try {
        const response = await axios.get(
          "https://localhost:7205/api/chat/all",
          {
            withCredentials: true,
          }
        );

        console.log("Полученные чаты с сервера:", response.data); // Логирование
        setChats(response.data);
      } catch (error) {
        setError(error.response ? error.response.data.message : error.message);
      } finally {
        setLoading(false);
      }
    };

    fetchChats(); // Вызов функции получения чатов
  }, []);

  const handleChatClick = (chatId, userName) => {
    // Если чат открыт, сбрасываем количество непрочитанных сообщений
    setChats((prevChats) =>
      prevChats.map((chat) =>
        chat.id === chatId ? { ...chat, unreadMessagesCount: 0 } : chat
      )
    );
    navigate("/chat", { state: { chatId, userName } });
  };

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
            <li
              key={chat.id}
              onClick={() => handleChatClick(chat.id, chat.secondUser.name)}
              style={{
                cursor: "pointer",
                display: "flex",
                alignItems: "flex-start",
                marginBottom: "10px", // Отступ между чатов
              }}
            >
              {/* Картинка пользователя */}
              <img
                src={
                  chat.secondUser.profileImagePath || "defaultProfileImage.jpg"
                } // Если изображения нет, используем дефолтное
                alt={`${chat.secondUser.name}'s profile`}
                style={{
                  width: "50px",
                  height: "50px",
                  borderRadius: "50%",
                  marginRight: "15px",
                }}
              />
              <div
                style={{
                  display: "flex",
                  flexDirection: "column",
                  justifyContent: "flex-start",
                  alignItems: "flex-start",
                  position: "relative",
                }}
              >
                <span>{chat.secondUser.name}</span>

                {/* Отображаем количество непрочитанных сообщений */}
                {chat.unreadMessagesCount > 0 && (
                  <span
                    style={{
                      position: "absolute",
                      top: "0",
                      right: "-30px",
                      backgroundColor: "gray",
                      color: "#fff",
                      borderRadius: "50%",
                      padding: "5px 10px",
                      fontSize: "12px",
                    }}
                  >
                    {chat.unreadMessagesCount}
                  </span>
                )}

                <div
                  style={{
                    display: "flex",
                    flexDirection: "column",
                    marginTop: "5px",
                  }}
                >
                  {chat.lastMessage ? (
                    <span style={{ color: "#888", fontSize: "14px" }}>
                      {chat.lastMessage.content}
                    </span>
                  ) : (
                    <span style={{ color: "#888", fontSize: "14px" }}>
                      Нет сообщений
                    </span>
                  )}
                </div>
              </div>
            </li>
          ))}
        </ul>
      ) : (
        <p>Чаты не найдены.</p>
      )}
    </div>
  );
};

export default ChatMessages;
