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
  const [isLoadingOlderMessages, setIsLoadingOlderMessages] = useState(false); // Новое состояние
  const navigate = useNavigate();
  const location = useLocation();
  const chatId = location.state?.chatId;
  const userName = location.state?.userName;
  const connection = useSignalR();
  const messagesEndRef = useRef(null);
  const scrollRef = useRef(null);

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

  const loadMessages = async () => {
    if (loading || !hasMore) return;
    setLoading(true);

    try {
      const response = await axios.get(
        "https://localhost:7205/api/message/byChatId",
        {
          params: {
            chatId,
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
              Sender: message.sender,
              IsSender: message.isSender,
            }))
        : [];

      if (newMessages.length === 0) {
        setHasMore(false);
      } else {
        setMessages((prevMessages) => [
          ...newMessages,
          ...prevMessages, // Новые сообщения добавляем в начало
        ]);
        setPage((prevPageNumber) => prevPageNumber + 1);
      }
    } catch (err) {
      console.error("Ошибка при загрузке сообщений:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (location.pathname === "/login" || location.pathname === "/register") {
      return;
    }

    const initialConnection = async () => {
      if (connection && chatId) {
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
                Sender: message.sender,
                IsSender: message.isSender,
              },
            ]);
          });

          await loadMessages();

          // После первоначальной загрузки прокручиваем вниз
          if (messagesEndRef.current) {
            messagesEndRef.current.scrollIntoView({ behavior: "auto" });
          }
        } catch (error) {
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
    if (messagesEndRef.current && !loading && !isLoadingOlderMessages) {
      messagesEndRef.current.scrollIntoView({ behavior: "smooth" });
    }
  }, [messages, loading, isLoadingOlderMessages]);

  const handleSendMessage = async () => {
    if (newMessage && connection && chatId) {
      try {
        const sentMessage = {
          Id: Date.now(), // Пример генерации уникального ID
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
      } catch (error) {
        console.error("Ошибка при отправке сообщения:", error);
      }
    } else {
      console.error("Ошибка: сообщение или подключение отсутствуют.");
    }
  };

  const handleKeyDown = (e) => {
    if (e.key === "Enter") {
      handleSendMessage();
    }
  };

  const handleScroll = async () => {
    const scrollContainer = scrollRef.current;

    if (scrollContainer.scrollTop === 0 && hasMore && !loading) {
      const previousScrollHeight = scrollContainer.scrollHeight;
      setIsLoadingOlderMessages(true); // Устанавливаем флаг загрузки старых сообщений
      await loadMessages();
      const newScrollHeight = scrollContainer.scrollHeight;
      // Сохраняем позицию прокрутки
      scrollContainer.scrollTop = newScrollHeight - previousScrollHeight;
      setIsLoadingOlderMessages(false); // Сбрасываем флаг после загрузки
    }
  };

  return (
    <div className="chat-page">
      <div className="chat-header">
        <h3>{userName}</h3>
      </div>

      <div className="chat-messages" onScroll={handleScroll} ref={scrollRef}>
        {messages.map((message, index) => (
          <div
            key={`${message.Id}-${index}`} // Комбинация уникального ID и индекса
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

      {loading && <p>Загрузка сообщений...</p>}
    </div>
  );
};

export default ChatPage;
