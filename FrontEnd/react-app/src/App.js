import React from "react";
import Login from "./pages/Login/Login";
import Registration from "./pages/Registration/Registration";
import SelectChat from "./pages/SelectChat/SelectChat";
import ChatPage from "./pages/ChatPage/ChatPage";
import { Routes, Route } from "react-router-dom";
import "./styles/Global.css";
import { SignalRProvider } from "./SignalRProvider";

const App = () => {
  return (
    <SignalRProvider>
      <Routes>
        <Route path="/" element={<SelectChat />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Registration />} />
        <Route path="/chat" element={<ChatPage />} />
      </Routes>
    </SignalRProvider>
  );
};

export default App;
