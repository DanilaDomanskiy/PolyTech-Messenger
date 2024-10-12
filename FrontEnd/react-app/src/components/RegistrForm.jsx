import React, { useState } from "react";
import Button from "./Button";

const RegistrForm = () => {
  return (
    <form className="auth-form" id="registrform">
      <div className="form-group">
        <input
          type="text"
          className="input name"
          name="name"
          placeholder="Введите имя..."
          required
          minLength="2"
          maxLength="100"
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
        />
      </div>
      <div className="button-signUp">
        <Button text="Зарегистрироваться" />
      </div>
    </form>
  );
};

export default RegistrForm;
