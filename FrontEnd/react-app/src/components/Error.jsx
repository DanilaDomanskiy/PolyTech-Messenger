import React from "react";
import { useEffect } from "react";
import { useTranslation } from "react-i18next";

const Error = () => {
  const { t, i18n } = useTranslation();

  useEffect(() => {
    // При загрузке компонента проверяем, сохранен ли язык в localStorage
    const savedLanguage = localStorage.getItem("appLanguage") || "ru"; // По умолчанию русский
    i18n.changeLanguage(savedLanguage); // Устанавливаем язык в i18n
  }, []);

  return (
    <div className="modal-overlay">
      <div className="modal">
        <h2>{t("error")}</h2>
      </div>
    </div>
  );
};

export default Error;
