import React, { useEffect, useState, useRef } from "react";
import { useLocation } from "react-router-dom";
import "../ChatPage/ChatPage.css";
import axios from "axios";
import { useSignalR } from "../../SignalRProvider";
import { DateTime } from "luxon";

const formatDate = (dateString) => {
  const date = DateTime.fromISO(dateString, { zone: "utc" }).setZone(
    "Europe/Minsk"
  );
  const now = DateTime.now().setZone("Europe/Minsk");

  const isToday = date.hasSame(now, "day");

  if (isToday) {
    // Если сообщение отправлено сегодня, показываем только время в Минске
    return date.setZone("Europe/Minsk").toFormat("HH:mm");
  } else {
    // Если сообщение не сегодня, показываем полную дату и время в Минске
    return date.setZone("Europe/Minsk").toFormat("yyyy.MM.dd HH:mm");
  }
};

const ChatPage = () => {
  const [messages, setMessages] = useState([]);
  const [newMessage, setNewMessage] = useState("");
  const [isConnected, setIsConnected] = useState(false);
  const [loading, setLoading] = useState(false);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [userId, setUserId] = useState(null);
  const [userStatus, setUserStatus] = useState({
    isActive: false,
    lastActive: null,
  });
  const location = useLocation();
  const chatId = location.state?.chatId;
  const userName = location.state?.userName;
  const { connection, handleError } = useSignalR();
  const messagesEndRef = useRef(null);
  const scrollRef = useRef(null);
  const [isInitialLoadDone, setIsInitialLoadDone] = useState(false);

  // Получение userId
  const fetchUserId = async () => {
    try {
      const response = await axios.get("https://localhost:7205/api/user/id", {
        withCredentials: true,
      });
      setUserId(response.data);
    } catch (error) {
      handleError(error);
    }
  };

  // Загрузка сообщений
  const loadMessages = async () => {
    if (loading || !hasMore || !userId) return;

    setLoading(true);
    try {
      const responseForLastActive = await axios.get(
        `https://localhost:7205/api/chat/${chatId}`,
        {
          withCredentials: true,
        }
      );

      setUserStatus({
        isActive: responseForLastActive.data.secondUser.isActive,
        lastActive: responseForLastActive.data.secondUser.lastActive,
      });

      const response = await axios.get(
        `https://localhost:7205/api/message/chat/${chatId}`,
        {
          params: { page, pageSize: 20 },
          withCredentials: true,
        }
      );
      console.log(response.data);

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
      handleError(err);
    } finally {
      setLoading(false);
    }
  };

  // Инициализация соединения SignalR
  useEffect(() => {
    const initialConnection = async () => {
      if (connection && chatId && userId) {
        try {
          if (connection.state !== "Connected") {
            await connection.start();
          }
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

          connection.on("IsActiveUser", (updatedUserId, isActive) => {
            if (updatedUserId === userId) return; // Игнорируем изменения для текущего пользователя

            setUserStatus((prevStatus) => ({
              ...prevStatus,
              isActive,
              lastActive: isActive ? null : DateTime.now().toISO(), // Обновляем время, если пользователь стал неактивным
            }));
          });

          await loadMessages();

          setTimeout(() => {
            if (messagesEndRef.current) {
              messagesEndRef.current.scrollIntoView({ behavior: "auto" });
            }
          }, 0);
        } catch (err) {
          handleError(err);
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
        connection.off("IsActiveUser");
        setIsConnected(false);
      }
    };
  }, [connection, handleError, chatId, userId]);

  // Отправка нового сообщения
  const handleSendMessage = async () => {
    if (!newMessage || !isConnected) return;

    try {
      const timestamp = new Date().toISOString(); // Генерация времени в ISO-формате

      const sentMessage = {
        Id: Date.now(),
        Content: newMessage,
        Timestamp: timestamp,
        Sender: userName,
        IsSender: true,
      };

      // Отправка сообщения на сервер
      await connection.invoke("SendPrivateChatMessageAsync", {
        PrivateChatId: chatId,
        Content: newMessage,
        Timestamp: timestamp,
      });

      // Добавление отправленного сообщения в локальное состояние
      setMessages((prevMessages) => [
        ...prevMessages,
        {
          ...sentMessage,
          Timestamp: timestamp, // Мы используем оригинальный ISO-строку для отображения
        },
      ]);

      setNewMessage(""); // Очистка поля ввода

      // Скроллинг в конец чата
      setTimeout(() => {
        if (messagesEndRef.current) {
          messagesEndRef.current.scrollIntoView({ behavior: "smooth" });
        }
      }, 0);
    } catch (err) {
      handleError(err);
    }
  };

  // Обработчик нажатия Enter
  const handleKeyDown = (e) => {
    if (e.key === "Enter") {
      handleSendMessage();
    }
  };

  // Прокрутка вверх при загрузке старых сообщений
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
  }, []);

  const getUserStatus = (isActive, lastActive) => {
    if (isActive) {
      return "В сети";
    }

    return "Последний раз в сети: " + formatDate(lastActive);
  };

  return (
    <div className="chat-page">
      <div className="chat-header">
        <h3>{userName}</h3>
        <span className="status">
          {" "}
          {getUserStatus(userStatus.isActive, userStatus.lastActive)}
        </span>
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
