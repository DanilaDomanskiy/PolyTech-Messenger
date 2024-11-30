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
  const location = useLocation();
  const chatId = location.state?.chatId;
  const userName = location.state?.userName;
  const connection = useSignalR();
  const messagesEndRef = useRef(null);
  const scrollRef = useRef(null);
  const [isInitialLoadDone, setIsInitialLoadDone] = useState(false); // Новое состояние

  // Функция для загрузки сообщений
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
        setHasMore(false); // Нет больше сообщений для загрузки
      } else {
        setMessages((prevMessages) => [...newMessages, ...prevMessages]);
        setPage((prevPageNumber) => prevPageNumber + 1);

        // Отмечаем завершение первой загрузки
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

  useEffect(() => {
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

            // Прокрутка вниз после получения нового сообщения
            setTimeout(() => {
              if (messagesEndRef.current) {
                messagesEndRef.current.scrollIntoView({ behavior: "smooth" });
              }
            }, 0);
          });

          await loadMessages();

          // Прокручиваем вниз после завершения загрузки начальных сообщений
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

    initialConnection();

    return () => {
      if (connection && chatId) {
        connection.off("ReceivePrivateChatMessage");
        connection.invoke("LeavePrivateChatAsync", chatId);
        setIsConnected(false);
      }
    };
  }, [connection, chatId]);

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

      // Прокручиваем чат до самого низа после обновления DOM
      setTimeout(() => {
        if (messagesEndRef.current) {
          messagesEndRef.current.scrollIntoView({ behavior: "smooth" });
        }
      }, 0); // Добавляем небольшую задержку
    } catch (err) {
      console.error("Ошибка при отправке сообщения:", err);
    }
  };

  const handleKeyDown = (e) => {
    if (e.key === "Enter") {
      handleSendMessage();
    }
  };

  const handleScroll = async () => {
    const scrollContainer = scrollRef.current;

    // Проверяем, что мы находимся вверху чата (чтобы подгрузить старые сообщения)
    if (scrollContainer.scrollTop === 0 && hasMore && !loading) {
      const chatMessagesElement = document.querySelector(".chat-messages");
      const previousScrollHeight = chatMessagesElement.scrollHeight;

      setLoading(true); // Включаем состояние загрузки

      // Загружаем более старые сообщения
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
    // Прокручиваем вниз до самого низа после загрузки новых сообщений
    if (!isInitialLoadDone && messagesEndRef.current) {
      messagesEndRef.current.scrollIntoView({ behavior: "auto" });
    }
  }, [messages]); // Срабатывает при изменении сообщений

  return (
    <div className="chat-page">
      <div className="chat-header">
        <h3>{userName}</h3>
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
