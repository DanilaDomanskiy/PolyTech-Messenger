import React, { useEffect, useState, useRef } from "react";
import { useLocation } from "react-router-dom";
import "../ChatPage/ChatPage.css";
import axios from "axios";
import { useSignalR } from "../../SignalRProvider";

const formatDate = (date) => {
  const options = {
    year: "numeric",
    month: "numeric",
    day: "numeric",
    hour: "numeric",
    minute: "numeric",
  };
  return new Date(date).toLocaleString(undefined, options);
};

const ChatPage = () => {
  const [messages, setMessages] = useState([]);
  const [newMessage, setNewMessage] = useState("");
  const [isConnected, setIsConnected] = useState(false);
  const [loading, setLoading] = useState(false);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [userId, setUserId] = useState(null);
  const [userStatus, setUserStatus] = useState("Не в сети"); // Состояние для статуса
  const location = useLocation();
  const chatId = location.state?.chatId;
  const userName = location.state?.userName;
  const connection = useSignalR();
  const messagesEndRef = useRef(null);
  const scrollRef = useRef(null);
  const [isInitialLoadDone, setIsInitialLoadDone] = useState(false);

  // Функция для получения userId
  const fetchUserId = async () => {
    try {
      const response = await axios.get("https://localhost:7205/api/user/id", {
        withCredentials: true,
      });
      setUserId(response.data); // Предполагаем, что API возвращает userId в response.data
    } catch (error) {
      console.error("Ошибка при получении userId:", error);
    }
  };

  // Загрузка сообщений
  const loadMessages = async () => {
    if (loading || !hasMore || !userId) return; // Проверяем userId

    setLoading(true);
    try {
      const response = await axios.get(
        `https://localhost:7205/api/message/chat/${chatId}`,
        {
          params: {
            page,
            pageSize: 20,
          },
          withCredentials: true,
        }
      );

      const newMessages = Array.isArray(response.data)
        ? response.data
            .slice()
            .reverse()
            .map((message) => ({
              Id: message.Id,
              Content: message.content,
              Timestamp: message.timestamp,
              Sender: message.senderId,
              IsSender: message.senderId === userId,
            }))
        : [];

      if (newMessages.length === 0) {
        setHasMore(false);
      } else {
        setMessages((prevMessages) => [...newMessages, ...prevMessages]);
        setPage((prevPageNumber) => prevPageNumber + 1);

        if (!isInitialLoadDone) {
          setIsInitialLoadDone(true);
        }
      }
    } catch (err) {
      console.error("Ошибка при загрузке сообщений:", err);
    } finally {
      setLoading(false);
    }
  };

  // Инициализация соединения SignalR
  useEffect(() => {
    const initialConnection = async () => {
      if (connection && chatId && userId) {
        try {
          await connection.invoke("JoinPrivateChatAsync", chatId);
          setIsConnected(true);

          connection.on("ReceivePrivateChatMessage", (message) => {
            setMessages((prev) => [
              ...prev,
              {
                Id: message.Id,
                Content: message.content,
                Timestamp: message.timestamp,
                Sender: message.senderId,
                IsSender: message.senderId === userId,
              },
            ]);

            setTimeout(() => {
              if (messagesEndRef.current) {
                messagesEndRef.current.scrollIntoView({ behavior: "smooth" });
              }
            }, 0);
          });

          await loadMessages();

          setTimeout(() => {
            if (messagesEndRef.current) {
              messagesEndRef.current.scrollIntoView({ behavior: "auto" });
            }
          }, 0);
        } catch (err) {
          console.error("Ошибка подключения:", err);
        }
      }
    };

    if (userId) {
      initialConnection();
    }

    return () => {
      if (connection && chatId) {
        connection.off("ReceivePrivateChatMessage");
        connection.invoke("LeavePrivateChatAsync", chatId);
        setIsConnected(false);
      }
    };
  }, [connection, chatId, userId]); // Теперь добавляется зависимость от userId

  // Отправка нового сообщения
  const handleSendMessage = async () => {
    if (!newMessage || !isConnected) return;

    try {
      const sentMessage = {
        Id: Date.now(),
        Content: newMessage,
        Timestamp: new Date().toISOString(),
        Sender: userName,
        IsSender: true,
      };

      await connection.invoke("SendPrivateChatMessageAsync", {
        PrivateChatId: chatId,
        Content: newMessage,
        Timestamp: sentMessage.Timestamp,
      });

      setMessages((prevMessages) => [...prevMessages, sentMessage]);
      setNewMessage("");

      setTimeout(() => {
        if (messagesEndRef.current) {
          messagesEndRef.current.scrollIntoView({ behavior: "smooth" });
        }
      }, 0);
    } catch (err) {
      console.error("Ошибка при отправке сообщения:", err);
    }
  };

  // Обработчик нажатия Enter
  const handleKeyDown = (e) => {
    if (e.key === "Enter") {
      handleSendMessage();
    }
  };

  // Прокрутка в верх при загрузке старых сообщений
  const handleScroll = async () => {
    const scrollContainer = scrollRef.current;

    if (scrollContainer.scrollTop === 0 && hasMore && !loading) {
      const chatMessagesElement = document.querySelector(".chat-messages");
      const previousScrollHeight = chatMessagesElement.scrollHeight;

      setLoading(true);
      await loadMessages();

      setTimeout(() => {
        const newScrollHeight = chatMessagesElement.scrollHeight;
        chatMessagesElement.scrollTop =
          newScrollHeight -
          previousScrollHeight +
          chatMessagesElement.scrollTop;
      }, 0);

      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUserId(); // Получаем userId при монтировании
  }, []); // Вызывается один раз при монтировании

  return (
    <div className="chat-page">
      <div className="chat-header">
        <h3>{userName}</h3>
        <span className="status">{userStatus}</span>
      </div>

      <div className="chat-messages" onScroll={handleScroll} ref={scrollRef}>
        {messages.map((message, index) => (
          <div
            key={`${message.Id}-${index}`}
            className={`message ${
              message.IsSender ? "my-message" : "other-message"
            }`}
          >
            <p>{message.Content}</p>
            <span className="timestamp">{formatDate(message.Timestamp)}</span>
          </div>
        ))}
        <div ref={messagesEndRef} />
      </div>

      <div className="chat-input">
        <input
          type="text"
          value={newMessage}
          onChange={(e) => setNewMessage(e.target.value)}
          onKeyDown={handleKeyDown}
          placeholder="Введите сообщение..."
        />
        <button onClick={handleSendMessage} disabled={!isConnected}>
          Отправить
        </button>
      </div>
    </div>
  );
};

export default ChatPage;
