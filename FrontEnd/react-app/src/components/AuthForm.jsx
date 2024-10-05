import React from "react";
import Button from "./Button";
import { Link } from "react-router-dom";

const AuthForm = () => {
  return (
    <form className="auth-form" id="loginform">
      <div className="form-group">
        <input
          type="text"
          className="input login"
          name="login"
          placeholder="Введите логин..."
          required
          minLength="2"
          maxLength="100"
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
        />
      </div>
      <div className="link-registration">
        <Link to="/register">Зарегистрироваться</Link>
      </div>
      <div className="button-signUp">
        <Button text="Войти" />
      </div>
    </form>
  );
};

export default AuthForm;
