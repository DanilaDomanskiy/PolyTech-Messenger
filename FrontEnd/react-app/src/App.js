import React from "react";
import Login from "./pages/Login/Login";
import Registration from "./pages/Registration/Registration";
import SelectChat from "./pages/SelectChat/SelectChat";
import { Routes, Route, Navigate } from "react-router-dom";
import "./styles/Global.css";

const App = () => {
  return (
    <div className="App">
      <Routes>
        <Route path="/" element={<Navigate to="/login" />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Registration />} />
        <Route path="/selectChat" element={<SelectChat />} />
      </Routes>
    </div>
  );
};

export default App;
