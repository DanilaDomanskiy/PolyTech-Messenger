import React, { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import search from "../../assets/images/search.png";
import avatar from "../../assets/images/download.jpg";
import settingsIcons from "../../assets/images/Settings.png";
import { useSignalR } from "../../SignalRProvider";
import Settings from "../Settings/Settings";
import "../../styles/Search.css";

const Search = () => {
  const [searchQuery, setSearchQuery] = useState("");
  const [users, setUsers] = useState([]);
  const [userSelected, setUserSelected] = useState(false); // Состояние для отслеживания выбора пользователя
  const { t, i18n } = useTranslation();
  const { handleError } = useSignalR();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    // При загрузке компонента проверяем, сохранен ли язык в localStorage
    const savedLanguage = localStorage.getItem("appLanguage") || "ru"; // По умолчанию русский
    i18n.changeLanguage(savedLanguage); // Устанавливаем язык в i18n
  }, []);

  const handleSearch = async (query) => {
    if (!query) {
      setUsers([]);
      return;
    }

    try {
      const response = await axios.get(
        `https://localhost:7205/api/user/search`,
        {
          params: { email: query },
          withCredentials: true,
        }
      );
      setUsers(response.data);
    } catch (error) {
      handleError(error);
    }
  };

  const handleChange = (e) => {
    const value = e.target.value;
    setSearchQuery(value);
    setUserSelected(false); // Сбрасываем выбор пользователя при вводе текста
    handleSearch(value);
  };

  const handleSelectUser = async (
    otherUserId,
    userName,
    userProfilePicture
  ) => {
    try {
      const response = await axios.post(
        `https://localhost:7205/api/chat`,
        { otherUserId }, // Отправляем ID пользователя
        { withCredentials: true }
      );

      // Если запрос успешен, открываем созданный чат
      const chatId = response.data; // Предполагаем, что сервер возвращает ID чата
      navigate("/chat", { state: { chatId, userName, userProfilePicture } });
    } catch (error) {
      handleError(error);
    }

    setSearchQuery("");
    setUsers([]);
    setUserSelected(true);
  };

  const openSettings = () => {
    setIsModalOpen(true);
  };

  const closeSettings = () => {
    setIsModalOpen(false);
  };

  return (
    <div className="search-container">
      <input
        type="text"
        id="search"
        placeholder={t("search_placeholder")}
        className="search-input"
        value={searchQuery}
        onChange={handleChange}
      />
      <img src={search} alt={t("search_icon")} className="search-icon" />

      <img
        src={settingsIcons}
        alt="icons"
        className="settings-icons"
        onClick={openSettings}
      />

      {/* Модальное окно для настроек */}
      {isModalOpen && <Settings onClose={closeSettings} />}

      {searchQuery &&
        users.length > 0 &&
        !userSelected && ( // Показываем список только если есть результаты и пользователь не выбран
          <div className="dropdown">
            <ul className="user-list">
              {users.map((user) => (
                <li
                  key={user.id}
                  className="user-item"
                  onClick={() =>
                    handleSelectUser(
                      user.id,
                      user.name,
                      user.profilePicturePath || avatar
                    )
                  }
                >
                  <img
                    src={user.profilePicturePath || avatar} // Указываем путь к аватарке или дефолтное изображение
                    alt={user.name}
                    className="user-avatar"
                  />
                  <div className="user-details">
                    <p className="user-name">{user.name}</p>
                    <p className="user-email">{user.email}</p>
                  </div>
                </li>
              ))}
            </ul>
          </div>
        )}

      {/* Отображаем сообщение о том, что пользователи не найдены, только если есть текст в поиске */}
      {searchQuery && users.length === 0 && !userSelected && (
        <div className="dropdown">
          <ul className="user-list">
            <li className="no-users">{t("users_not_found")}</li>
          </ul>
        </div>
      )}
    </div>
  );
};

export default Search;
