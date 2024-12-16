import React, { useState, useEffect } from "react";
import "../../styles/Settings.css";
import avatar from "../../assets/images/download.jpg";
import { useTranslation } from "react-i18next";
import axios from "axios";
import { useSignalR } from "../../SignalRProvider";

const Settings = ({ onClose }) => {
  const { t, i18n } = useTranslation();
  const [currentLanguage, setCurrentLanguage] = useState(i18n.language);
  const { handleError } = useSignalR();

  const [profileData, setProfileData] = useState({
    name: "",
    profileImage: avatar,
  });

  const [editedData, setEditedData] = useState({
    name: "",
    oldPassword: "",
    newPassword: "",
    profileImage: avatar,
  });

  const [isEditing, setIsEditing] = useState(false);

  // Состояние для ошибки текущего пароля
  const [passwordError, setPasswordError] = useState(false);

  const handleGetUser = async () => {
    try {
      const response = await axios.get(`https://localhost:7205/api/user`, {
        withCredentials: true,
      });

      console.log(response.data);
      const user = {
        userName: response.data.name,
        profileImage: response.data.profileImagePath,
      };
      setProfileData({
        name: user.userName,
        profileImage: user.profileImage,
      });
      setEditedData({
        ...editedData,
        name: user.userName,
        profileImage: user.profileImage,
      });
    } catch (error) {
      handleError(error);
    }
  };

  useEffect(() => {
    handleGetUser();
  }, []);

  const handleLanguageChange = (e) => {
    const selectedLanguage = e.target.value;
    i18n.changeLanguage(selectedLanguage);
    setCurrentLanguage(selectedLanguage);
    localStorage.setItem("appLanguage", selectedLanguage);
  };

  useEffect(() => {
    const savedLanguage = localStorage.getItem("appLanguage") || "ru";
    i18n.changeLanguage(savedLanguage);
    setCurrentLanguage(savedLanguage);
  }, [i18n]);

  const handleEditChange = (e) => {
    const { name, value } = e.target;
    setEditedData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleImageChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setEditedData((prev) => ({
        ...prev,
        profileImage: file, // Сохраняем исходный объект File
      }));
    }
  };

  const handleSave = async () => {
    const { name, oldPassword, newPassword, profileImage } = editedData;

    try {
      // Сбрасываем ошибку текущего пароля перед началом обработки
      setPasswordError(false);

      // Изменение имени
      if (name !== profileData.name) {
        await fetch("https://localhost:7205/api/user/name", {
          method: "PATCH",
          headers: {
            "Content-Type": "application/json",
          },
          credentials: "include",
          body: JSON.stringify({ name }),
        });
      }

      // Изменение пароля
      if (oldPassword && newPassword) {
        const passwordResponse = await fetch(
          "https://localhost:7205/api/user/password",
          {
            method: "PATCH",
            headers: {
              "Content-Type": "application/json",
            },
            credentials: "include",
            body: JSON.stringify({ oldPassword, newPassword }),
          }
        );

        if (!passwordResponse.ok) {
          if (
            passwordResponse.status >= 400 &&
            passwordResponse.status <= 499
          ) {
            // Устанавливаем ошибку для текущего пароля
            setPasswordError(true);
            return; // Завершаем обработку, так как пароль не сохранен
          } else {
            throw new Error("Password update failed");
          }
        }
      }

      // Изменение изображения
      if (profileImage !== profileData.profileImage) {
        const formData = new FormData();
        formData.append("file", profileImage);

        const imageResponse = await fetch(
          "https://localhost:7205/api/user/profile-image",
          {
            method: "PATCH",
            credentials: "include",
            body: formData,
          }
        );

        if (!imageResponse.ok) {
          throw new Error("Image update failed");
        }
      }

      // После успешного сохранения загружаем актуальные данные с сервера
      await handleGetUser();
    } catch (error) {
      // Если ошибка не связана с паролем, вызываем обработчик
      if (!passwordError) {
        handleError(error);
      }
    }

    setIsEditing(false); // Закрываем режим редактирования после сохранения
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <button className="close-btn" onClick={onClose}>
          ✕
        </button>
        <h2>{t("settings_profile")}</h2>
        <div className="profile-section">
          <img
            src={profileData.profileImage}
            alt="Аватарка"
            className="profile-avatar"
          />
          <div className="profile-info">
            <span className="profile-name">{profileData.name}</span>
          </div>
          <button
            className="edit-profile-btn"
            onClick={() => setIsEditing((prev) => !prev)}
          >
            {isEditing
              ? t("settings_cancel")
              : t("settings_edit_profile_button")}
          </button>
        </div>
        <div className="settings-content">
          <div className="setting-item">
            <label className="language-select">
              {t("settings_select_language")}
            </label>
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

        {isEditing && (
          <div className="edit-profile-section">
            <h3>{t("settings_edit_profile")}</h3>
            <div className="edit-item">
              <label htmlFor="edit-name">
                {t("settings_edit_profile_name")}
              </label>
              <input
                id="edit-name"
                name="name"
                type="text"
                value={editedData.name}
                onChange={handleEditChange}
              />
            </div>
            <div className="edit-item">
              <label htmlFor="edit-old-password">
                {t("settings_edit_profile_old_password")}
              </label>
              <input
                id="edit-old-password"
                name="oldPassword"
                type="password"
                placeholder={t("settings_placeholder_old_password")}
                value={editedData.oldPassword}
                onChange={handleEditChange}
                style={{
                  border: passwordError ? "2px solid red" : "1px solid #ccc", // Красная граница при ошибке, иначе стандартная
                }}
              />
              {passwordError && (
                <span className="error-text">
                  {t("settings_error_incorrect_current_password")}
                </span>
              )}
            </div>
            <div className="edit-item">
              <label htmlFor="edit-new-password">
                {t("settings_edit_profile_new_password")}
              </label>
              <input
                id="edit-new-password"
                name="newPassword"
                type="password"
                placeholder={t("settings_placeholder_new_password")}
                value={editedData.newPassword}
                onChange={handleEditChange}
              />
            </div>
            <div className="edit-item">
              <label htmlFor="edit-image">
                {t("settings_edit_profile_photo")}
              </label>
              <input
                id="edit-image"
                type="file"
                accept="image/*"
                onChange={handleImageChange}
              />
            </div>
            <button className="save-profile-btn" onClick={handleSave}>
              {t("settings_save_button")}
            </button>
          </div>
        )}
      </div>
    </div>
  );
};

export default Settings;
