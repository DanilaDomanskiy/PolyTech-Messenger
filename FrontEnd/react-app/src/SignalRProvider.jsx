import React, {
  createContext,
  useContext,
  useEffect,
  useState,
  useCallback,
} from "react";
import { useLocation, useNavigate } from "react-router-dom";
import * as signalR from "@microsoft/signalr";
import Error from "./components/Error";

const CHAT_HUB_URL = "https://localhost:7205/chatHub";
const SignalRContext = createContext(null);

let connection = null;

const getSignalRConnection = async (handleError) => {
  if (!connection) {
    try {
      connection = new signalR.HubConnectionBuilder()
        .withUrl(CHAT_HUB_URL)
        .withAutomaticReconnect()
        .build();
      await connection.start();
      await connection.invoke("ConnectAsync");
    } catch (error) {
      connection = null;
      handleError(error);
    }
    window.addEventListener("beforeunload", handleBeforeUnload);
  }
  return connection;
};

const handleBeforeUnload = async () => {
  if (connection) {
    await connection.invoke("DisconnectAsync");
    connection.stop();
    connection = null;
  }
};

export const useSignalR = () => useContext(SignalRContext);

export const SignalRProvider = ({ children }) => {
  const [connection, setConnection] = useState(null);
  const [error, setError] = useState(false);
  const location = useLocation();
  const navigate = useNavigate();

  // Мемоизация handleError с помощью useCallback
  const handleError = useCallback(
    (error) => {
      if (error?.response?.status === 401 || error?.response?.status === 403) {
        navigate("/login");
      } else {
        setError(true);
        console.error(error);
      }
    },
    [navigate]
  );

  useEffect(() => {
    if (location.pathname === "/login" || location.pathname === "/register") {
      return;
    }

    const initConnection = async () => {
      if (!error) {
        // Проверяем, что ошибки нет
        const conn = await getSignalRConnection(handleError);
        setConnection(conn);
      }
    };
    initConnection();
  }, [handleError, location.pathname]);

  // Если ошибка произошла, показываем компонент <Error />
  return (
    <SignalRContext.Provider value={{ connection, handleError }}>
      {error ? <Error /> : children}
    </SignalRContext.Provider>
  );
};
