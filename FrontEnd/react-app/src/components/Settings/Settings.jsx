import React from "react";
import "../../styles/Settings.css";
import Sergey from "../../assets/images/Sergey.jpg";
import { useTranslation } from "react-i18next";
import { useState, useEffect } from "react";

const Settings = ({ onClose, userName, profileImage }) => {
  const { t, i18n } = useTranslation();
  const [currentLanguage, setCurrentLanguage] = useState(i18n.language);

  // Сохранение выбранного языка в локальном хранилище
  const handleLanguageChange = (e) => {
    const selectedLanguage = e.target.value;
    i18n.changeLanguage(selectedLanguage); // Меняем язык
    setCurrentLanguage(selectedLanguage); // Обновляем состояние
    localStorage.setItem("appLanguage", selectedLanguage); // Сохраняем язык в localStorage
  };

  useEffect(() => {
    // При загрузке компонента проверяем, сохранен ли язык в localStorage
    const savedLanguage = localStorage.getItem("appLanguage") || "ru"; // По умолчанию русский
    i18n.changeLanguage(savedLanguage); // Устанавливаем язык в i18n
    setCurrentLanguage(savedLanguage); // Обновляем состояние
  }, []);

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <button className="close-btn" onClick={onClose}>
          ✕
        </button>
        <h2>Профиль</h2>
        <div className="profile-section">
          <img
            src={profileImage || Sergey}
            alt="Аватар"
            className="profile-avatar"
          />
          <div className="profile-info">
            <span className="profile-name">{userName}</span>
          </div>
          <button className="edit-profile-btn">Редактировать профиль</button>
        </div>
        <div className="settings-content">
          <div className="setting-item">
            <label htmlFor="language-select">Выбор языка:</label>
            <select
              id="language-select"
              value={currentLanguage}
              onChange={handleLanguageChange}
            >
              <option value="ru">Русский</option>
              <option value="en">English</option>
              <option value="be">Беларуская</option>
            </select>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Settings;
