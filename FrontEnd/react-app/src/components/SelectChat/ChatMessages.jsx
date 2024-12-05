import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import { useSignalR } from "../../SignalRProvider";
import "../../pages/SelectChat/SelectChat.css";
import avatar from "../../assets/images/download.png";

const ChatMessages = () => {
  const [chats, setChats] = useState([]);
  const { connection, handleError } = useSignalR();
  const navigate = useNavigate();

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
                chat.secondUser.id === userId ? { ...chat, isActive } : chat
              )
            );
          });
        } catch (err) {
          handleError(err); // Обработка ошибки через провайдер
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
        handleError(err); // Обработка ошибки через провайдер
      }
    };

    fetchChats();
  }, [handleError]);

  const handleChatClick = (chatId, userName) => {
    setChats((prevChats) =>
      prevChats.map((chat) =>
        chat.id === chatId ? { ...chat, unreadMessagesCount: 0 } : chat
      )
    );
    navigate("/chat", { state: { chatId, userName } });
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
                {chat.lastMessage ? (
                  <span>{chat.lastMessage.content}</span>
                ) : (
                  <span>Нет сообщений</span>
                )}
              </div>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default ChatMessages;
