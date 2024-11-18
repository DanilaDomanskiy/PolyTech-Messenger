import React, { useState } from "react";
import Button from "./Button";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import Modal from "./Modal";

const RegistrForm = () => {
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [message, setMessage] = useState("");
  const [isSuccess, setIsSuccess] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const response = await axios.post(
        "https://localhost:7205/api/user",
        {
          name,
          email,
          password,
        },
        {
          withCredentials: true,
        }
      );

      if (response.status >= 200 && response.status <= 299) {
        setMessage("Регистрация прошла успешно!");
        setIsSuccess(true);
      } else {
        console.log(response.status);
      }
    } catch (error) {
      if (error.response) {
        // Ошибка от сервера
        if (error.response.status === 409) {
          setMessage("Пользователь с таким email уже существует!");
        }
      } else if (error.request) {
        // Запрос был сделан, но сервер не ответил
        setMessage("Нет ответа от сервера. Проверьте соединение с интернетом.");
      } else {
        console.log("Ошибка:", error.message);
        setMessage("Что-то пошло не так. Попробуйте позже.");
      }
    }
  };

  const handleCloseModal = () => {
    setMessage("");
    if (isSuccess) {
      navigate("/login");
    }
  };

  return (
    <form className="auth-form" id="registrform" onSubmit={handleSubmit}>
      <div className="form-group">
        <input
          type="text"
          className="input name"
          name="name"
          placeholder="Введите имя..."
          required
          minLength="2"
          maxLength="100"
          value={name}
          onChange={(e) => setName(e.target.value)}
        />
      </div>
      <div className="form-group">
        <input
          type="email"
          className="input email"
          name="email"
          placeholder="Введите почту..."
          required
          minLength="2"
          maxLength="100"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />
      </div>
      <div className="form-group">
        <input
          type="password"
          className="input password"
          name="password"
          placeholder="Введите пароль..."
          required
          minLength="10"
          maxLength="100"
          pattern="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{10,100}$"
          title="Пароль должен содержать хотя бы одну заглавную букву, одну строчную букву, одну цифру и быть длиной от 10 до 100 символов."
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
      </div>
      <div className="button-signUp">
        <Button text="Зарегистрироваться" />
      </div>
      {message && <Modal message={message} onClose={handleCloseModal} />}
    </form>
  );
};

export default RegistrForm;
