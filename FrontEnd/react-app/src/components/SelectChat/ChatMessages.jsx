import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import { useSignalR } from "../../SignalRProvider";
import "../../pages/SelectChat/SelectChat.css";
import avatar from "../../assets/images/download.png";

const ChatMessages = () => {
  const [chats, setChats] = useState([]);
  const [userId, setUserId] = useState(null); // Хранение текущего userId
  const { connection, handleError } = useSignalR();
  const navigate = useNavigate();

  // Получение текущего userId
  useEffect(() => {
    const fetchUserId = async () => {
      try {
        const response = await axios.get("https://localhost:7205/api/user/id", {
          withCredentials: true,
        });
        setUserId(response.data);
      } catch (err) {
        handleError(err); // Обработка ошибки через провайдер
      }
    };

    fetchUserId();
  }, [handleError]);

  // Подключение SignalR
  useEffect(() => {
    const connectSignalR = async () => {
      if (connection) {
        try {
          connection.on("ReceivePrivateChatMessages", (chatId, message) => {
            setChats((prevChats) =>
              prevChats.map((chat) =>
                chat.id === chatId
                  ? {
                      ...chat,
                      lastMessage: message,
                      unreadMessagesCount: chat.unreadMessagesCount + 1,
                    }
                  : chat
              )
            );
          });

          connection.on("IsActiveUser", (userId, isActive) => {
            setChats((prevChats) =>
              prevChats.map((chat) =>
                chat.secondUser.id === userId
                  ? { ...chat, secondUser: { ...chat.secondUser, isActive } }
                  : chat
              )
            );
          });
        } catch (err) {
          handleError(err);
        }
      }
    };

    connectSignalR();

    return () => {
      if (connection) {
        connection.off("ReceivePrivateChatMessages");
        connection.off("IsActiveUser");
      }
    };
  }, [connection, handleError]);

  // Загрузка списка чатов
  useEffect(() => {
    const fetchChats = async () => {
      try {
        const response = await axios.get(
          "https://localhost:7205/api/chat/all",
          {
            withCredentials: true,
          }
        );

        setChats(response.data);
      } catch (err) {
        handleError(err);
      }
    };

    fetchChats();
  }, [handleError]);

  // Обработка клика по чату
  const handleChatClick = (chatId, userName) => {
    setChats((prevChats) =>
      prevChats.map((chat) =>
        chat.id === chatId ? { ...chat, unreadMessagesCount: 0 } : chat
      )
    );
    navigate("/chat", { state: { chatId, userName } });
  };

  // Отображение последнего сообщения
  const renderLastMessage = (lastMessage, secondUser) => {
    const isSentByCurrentUser = lastMessage.senderId === userId;

    return (
      <span>
        {isSentByCurrentUser ? "Вы: " : `${secondUser.name || ""}: `}
        {lastMessage.content}
      </span>
    );
  };

  return (
    <div id="privateMessages">
      <ul>
        {chats.map((chat) => (
          <li
            key={chat.id}
            onClick={() => handleChatClick(chat.id, chat.secondUser.name)}
          >
            <div className="profile-container">
              <img src={avatar} alt={`${chat.secondUser.name}'s profile`} />
              {chat.secondUser.isActive && <div className="status-indicator" />}
            </div>

            <div className="chat-details">
              <span className="name">{chat.secondUser.name}</span>
              {chat.unreadMessagesCount > 0 && (
                <span className="unread-count">{chat.unreadMessagesCount}</span>
              )}
              <div className="last-message">
                {renderLastMessage(chat.lastMessage, chat.secondUser)}
              </div>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default ChatMessages;
