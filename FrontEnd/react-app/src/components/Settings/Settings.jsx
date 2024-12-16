import React, { useState, useEffect } from "react";
import "../../styles/Settings.css";
import Sergey from "../../assets/images/Sergey.jpg";
import { useTranslation } from "react-i18next";
import axios from "axios";
import { useSignalR } from "../../SignalRProvider";

const Settings = ({ onClose }) => {
  const { t, i18n } = useTranslation();
  const [currentLanguage, setCurrentLanguage] = useState(i18n.language);
  const { handleError } = useSignalR();
  const [currentUser, setCurrentUser] = useState({
    userName: "",
    profileImage: "",
  });

  // Данные профиля, которые отображаются
  const [profileData, setProfileData] = useState({
    name: "",
    profileImage: Sergey,
  });

  // Редактируемые данные
  const [editedData, setEditedData] = useState({
    name: "",
    oldPassword: "",
    newPassword: "",
    profileImage: Sergey,
  });

  const [isEditing, setIsEditing] = useState(false);

  useEffect(() => {
    const handleGetUser = async () => {
      try {
        const response = await axios.get(`https://localhost:7205/api/user`, {
          withCredentials: true,
        });
        const user = {
          userName: response.data.name,
          profileImage: response.data.profileImagePath || Sergey,
        };

        // Обновляем текущего пользователя и профильные данные
        setCurrentUser(user);
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
  }, []);

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
      const reader = new FileReader();
      reader.onload = () => {
        setEditedData((prev) => ({
          ...prev,
          profileImage: reader.result, // Сохраняем base64-изображение
        }));
      };
      reader.readAsDataURL(file);
    }
  };

  const handleSave = async () => {
    const { name, oldPassword, newPassword, profileImage } = editedData;

    try {
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
        console.log("Имя успешно обновлено.");
      }

      // Изменение пароля
      if (oldPassword && newPassword) {
        await fetch("https://localhost:7205/api/user/password", {
          method: "PATCH",
          headers: {
            "Content-Type": "application/json",
          },
          credentials: "include",
          body: JSON.stringify({ oldPassword, newPassword }),
        });
        console.log("Пароль успешно обновлен.");
      }

      // Изменение изображения
      if (profileImage !== profileData.profileImage) {
        const formData = new FormData();
        formData.append("profileImage", profileImage);

        await fetch("https://localhost:7205/api/user/profile-image", {
          method: "PATCH",
          credentials: "include",
          body: formData,
        });
        console.log("Фотография успешно обновлена.");
      }

      // Обновляем профильные данные после успешного сохранения
      setProfileData({
        name,
        profileImage,
      });

      alert(t("settings_save_success"));
    } catch (error) {
      console.error("Ошибка при сохранении данных:", error);
      alert(t("settings_save_error"));
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
            alt="Аватар"
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
              />
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
