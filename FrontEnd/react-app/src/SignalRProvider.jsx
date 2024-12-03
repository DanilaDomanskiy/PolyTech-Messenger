import React, { createContext, useContext, useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import * as signalR from "@microsoft/signalr";
import { useHandleError } from "./Scripts"; // Убедитесь, что этот хук подключен корректно
import Error from "./components/Error"; // Компонент для отображения ошибок

const CHAT_HUB_URL = "https://localhost:7205/chatHub";
const SignalRContext = createContext(null);
let connection = null;

const getSignalRConnection = async (handleError, setErrorMessage) => {
  if (!connection) {
    try {
      connection = new signalR.HubConnectionBuilder()
        .withUrl(CHAT_HUB_URL)
        .withAutomaticReconnect()
        .build();

      await connection.start();
      await connection.invoke("ConnectAsync");

      // Убедитесь, что обработчик добавлен только один раз
      if (!window.onbeforeunload) {
        window.addEventListener("beforeunload", handleBeforeUnload);
      }
    } catch (error) {
      handleError(error, setErrorMessage);
      connection = null;
    }
  }
  return connection;
};

const handleBeforeUnload = async () => {
  if (connection) {
    try {
      await connection.invoke("DisconnectAsync");
      await connection.stop();
    } catch (error) {
      console.error("Ошибка при остановке соединения:", error);
    }
  }
};

export const useSignalR = () => useContext(SignalRContext);

export const SignalRProvider = ({ children }) => {
  const [currentConnection, setCurrentConnection] = useState(null);
  const location = useLocation();
  const handleError = useHandleError();
  const [errorMessage, setErrorMessage] = useState(false);

  useEffect(() => {
    const initConnection = async () => {
      if (location.pathname === "/login" || location.pathname === "/register") {
        console.log("SignalR отключен на страницах входа и регистрации.");
        if (currentConnection) {
          try {
            await currentConnection.invoke("DisconnectAsync");
            await currentConnection.stop();
            setCurrentConnection(null);
          } catch (error) {
            console.error("Ошибка при отключении SignalR:", error);
          }
        }
        return;
      }

      const conn = await getSignalRConnection(handleError, setErrorMessage);
      setCurrentConnection(conn);
    };

    initConnection();

    return () => {
      window.removeEventListener("beforeunload", handleBeforeUnload);
      if (currentConnection) {
        currentConnection
          .stop()
          .catch((error) =>
            console.error("Ошибка при остановке соединения:", error)
          );
      }
    };
  }, [location.pathname, handleError, currentConnection]);

  return (
    <SignalRContext.Provider value={currentConnection}>
      {errorMessage ? <Error /> : children}
    </SignalRContext.Provider>
  );
};
