import React, { useState } from "react";
import Button from "./Button";
import { Link, useNavigate } from "react-router-dom";
import axios from "axios";
import Modal from "./Modal";

const AuthForm = () => {
  const [login, setLogin] = useState("");
  const [password, setPassword] = useState("");
  const [errorMessage, setErrorMessage] = useState("");
  const navigate = useNavigate(); // Хук для навигации после успешной авторизации

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const response = await axios.post(
        "https://localhost:7205/api/user/login",
        {
          login,
          password,
        }
      );

      if (response.status >= 200 && response.status <= 299) {
        navigate("/selectChat");
      } else {
        console.log(response.status);
      }
    } catch (error) {
      if (error.response) {
        setErrorMessage(
          error.response.data.message || "Неверный логин или пароль!"
        );
      } else {
        setErrorMessage("Ошибка сети. Попробуйте позже."); // Общая ошибка
      }
    }
  };

  return (
    <form className="auth-form" id="loginform" onSubmit={handleSubmit}>
      <div className="form-group">
        <input
          type="text"
          className="input login"
          name="login"
          placeholder="Введите логин..."
          required
          minLength="2"
          maxLength="100"
          value={login}
          onChange={(e) => setLogin(e.target.value)}
        />
      </div>
      <div className="form-group">
        <input
          type="password"
          className="input password"
          name="password"
          placeholder="Введите пароль..."
          required
          minLength="6"
          maxLength="100"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
      </div>
      <div className="link-registration">
        <Link to="/register">Зарегистрироваться</Link>
      </div>
      <div className="button-signUp">
        <Button text="Войти" />
      </div>
      {errorMessage && (
        <Modal message={errorMessage} onClose={() => setErrorMessage("")} />
      )}
    </form>
  );
};

export default AuthForm;
