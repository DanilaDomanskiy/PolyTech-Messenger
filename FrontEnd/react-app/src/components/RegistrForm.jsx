import React from "react";
import Button from "./Button";

const RegistrForm = () => {
  return (
    <form className="auth-form" id="registrform">
      <div class="form-group">
        <input
          type="text"
          class="input name"
          name="name"
          placeholder="Введите имя..."
          required
          minlength="2"
          maxlength="100"
        />
      </div>
      <div class="form-group">
        <input
          type="email"
          class="input email"
          name="email"
          placeholder="Введите почту..."
          required
          minlength="2"
          maxlength="100"
        />
      </div>
      <div class="form-group">
        <input
          type="password"
          class="input password"
          name="password"
          placeholder="Введите пароль..."
          required
          minlength="6"
          maxlength="100"
        />
      </div>
      <div className="button-signUp">
        <Button text="Зарегистрироваться" />
      </div>
    </form>
  );
};

export default RegistrForm;
