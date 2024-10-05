import React from "react";
import Login from "./pages/Login/Login";
import Registration from "./pages/Registration/Registration";
import { Routes, Route } from "react-router-dom";
import "./styles/Global.css";

const App = () => {
  return (
    <div className="App">
      <Routes>
        <Route path="/" element={<Login />} />
        <Route path="/register" element={<Registration />} />
      </Routes>
    </div>
  );
};

export default App;
