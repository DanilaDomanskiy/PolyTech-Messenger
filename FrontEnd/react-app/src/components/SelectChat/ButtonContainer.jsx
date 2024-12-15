import React from "react";
import { useEffect } from "react";
import { useTranslation } from "react-i18next";

const ButtonContainer = ({ setActiveSection, activeSection }) => {
  const { t, i18n } = useTranslation();

  useEffect(() => {
    // При загрузке компонента проверяем, сохранен ли язык в localStorage
    const savedLanguage = localStorage.getItem("appLanguage") || "ru"; // По умолчанию русский
    i18n.changeLanguage(savedLanguage); // Устанавливаем язык в i18n
  }, []);

  return (
    <div className="button-container">
      <button
        id="privateBtn"
        className={`button ${activeSection === "private" ? "active" : ""}`}
        onClick={() => setActiveSection("private")}
      >
        {t("chats")}
      </button>
      <button
        id="groupBtn"
        className={`button ${activeSection === "group" ? "active" : ""}`}
        onClick={() => setActiveSection("group")}
      >
        {t("groups")}
      </button>
    </div>
  );
};

export default ButtonContainer;
