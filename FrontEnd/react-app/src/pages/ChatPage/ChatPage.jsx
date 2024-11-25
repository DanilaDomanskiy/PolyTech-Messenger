import React, { useEffect, useState, useCallback, useRef } from "react";
import { useNavigate, useLocation } from "react-router-dom";
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
  const navigate = useNavigate();
  const location = useLocation();
  const chatId = location.state?.chatId;
  const userName = location.state?.userName;
  const connection = useSignalR();
  const messagesEndRef = useRef(null);

  // Обработка ошибок
  const handleError = useCallback(
    (error) => {
      if (error?.response?.status === 401 || error?.response?.status === 403) {
        navigate("/login");
      } else {
        console.error("SignalR Error:", error);
      }
    },
    [navigate]
  );

  const loadMessages = async (pageNumber) => {
    if (loading || !hasMore) return;
    setLoading(true);

    try {
      const response = await axios.get(
        "https://localhost:7205/api/message/byChatId",
        {
          params: {
            chatId,
            pageNumber,
            pageSize: 20,
          },
          withCredentials: true, // Указание на отправку cookies
        }
      );

      // Логируем полный ответ, чтобы проверить структуру данных
      console.log(response.data);

      // Проверка на существование messages и если их нет, используем пустой массив
      const newMessages = Array.isArray(response.data)
        ? response.data
            .slice()
            .reverse()
            .map((message) => ({
              Id: message.Id,
              Content: message.content,
              Timestamp: message.timestamp,
              Sender: message.sender,
            }))
        : [];

      if (newMessages.length === 0) {
        console.log("Сообщения не найдены или пришел пустой массив");
      }

      // Если есть новые сообщения, добавляем их в список
      setMessages((prevMessages) => [...prevMessages, ...newMessages]);

      // Если новых сообщений меньше 20, значит, больше нет
      if (newMessages.length < 20) {
        setHasMore(false);
      }

      setPage(pageNumber + 1);
    } catch (err) {
      console.error("Ошибка при загрузке сообщений:", err);
    } finally {
      setLoading(false);
    }
  };

  // Инициализация соединения при изменении маршрута
  useEffect(() => {
    if (location.pathname === "/login" || location.pathname === "/register") {
      return; // Пропускаем подключение, если находимся на странице логина или регистрации
    }

    const initialConnection = async () => {
      if (connection && chatId) {
        try {
          await connection.invoke("JoinPrivateChatAsync", chatId);
          setIsConnected(true);

          // Подписка на событие получения сообщения
          connection.on("ReceivePrivateChatMessage", (message) => {
            console.log("Получено сообщение:", message);

            setMessages((prev) => [
              ...prev,
              {
                Id: message.Id,
                Content: message.content,
                Timestamp: message.timestamp,
                Sender: message.sender,
              },
            ]);
          });

          loadMessages(1);
        } catch (error) {
          console.error(error);
          handleError(error);
        }
      }
    };

    initialConnection();

    return () => {
      if (connection && chatId) {
        connection.off("ReceivePrivateChatMessage");
        connection.invoke("LeavePrivateChatAsync", chatId);
        setIsConnected(false);
      }
    };
  }, [connection, chatId, handleError, location.pathname]);

  useEffect(() => {
    if (messagesEndRef.current) {
      messagesEndRef.current.scrollIntoView({ behavior: "smooth" });
    }
  }, [messages]);

  // Отправка нового сообщения
  const handleSendMessage = async () => {
    console.log("Проверяем перед отправкой: ", {
      newMessage,
      connection,
      chatId,
    });

    if (newMessage && connection && chatId) {
      try {
        await connection.invoke("SendPrivateChatMessageAsync", {
          PrivateChatId: chatId,
          Content: newMessage,
          Timestamp: new Date().toISOString(),
        });
        console.log("Сообщение отправлено успешно:", newMessage);
        // Локально добавляем сообщение для немедленного отображения
        setMessages((prevMessages) => [
          ...prevMessages,
          {
            Id: newMessage.Id,
            Content: newMessage,
            Timestamp: new Date().toISOString(),
            Sender: userName,
          },
        ]);
        setNewMessage(""); // Очищаем поле ввода после отправки
      } catch (error) {
        console.error("Ошибка при отправке сообщения:", error);
      }
    } else {
      console.error("Ошибка: сообщение или подключение отсутствуют.");
    }
  };

  // Обработка нажатия клавиши "Enter"
  const handleKeyDown = (e) => {
    if (e.key === "Enter") {
      handleSendMessage();
    }
  };

  // Обработчик прокрутки для загрузки дополнительных сообщений
  const handleScroll = (e) => {
    const top = e.target.scrollTop === 0; // Если прокрутка достигла верхней части
    if (top && hasMore && !loading) {
      loadMessages(page);
    }
  };

  return (
    <div className="chat-page">
      <div className="chat-header">
        <h3>{userName}</h3>
      </div>

      <div className="chat-messages" onScroll={handleScroll}>
        {messages.map((message, index) => (
          <div
            key={index}
            className={`message ${
              message.Sender === userName ? "my-message" : "other-message"
            }`}
          >
            <p>{message.Content}</p>
            <span className="timestamp">{formatDate(message.Timestamp)}</span>
          </div>
        ))}
        {/* Скрытый элемент, к которому прокручиваем */}
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

      {loading && <p>Загрузка сообщений...</p>}
    </div>
  );
};

export default ChatPage;
