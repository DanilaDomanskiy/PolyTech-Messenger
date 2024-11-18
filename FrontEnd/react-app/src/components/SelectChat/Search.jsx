import React, { useState } from "react";
import axios from "axios";
import search from "../../assets/images/search.png";
import "../../styles/Search.css";

const Search = () => {
  const [searchQuery, setSearchQuery] = useState("");
  const [users, setUsers] = useState([]);
  const [userSelected, setUserSelected] = useState(false); // Состояние для отслеживания выбора пользователя

  const handleSearch = async (query) => {
    if (!query) {
      setUsers([]);
      return;
    }

    try {
      const response = await axios.get(
        `https://localhost:7205/api/user/searchByEmail`,
        {
          params: { email: query },
          withCredentials: true,
        }
      );
      setUsers(response.data);
    } catch (error) {
      console.error("Ошибка при поиске пользователей:", error);
    }
  };

  const handleChange = (e) => {
    const value = e.target.value;
    setSearchQuery(value);
    setUserSelected(false); // Сбрасываем выбор пользователя при вводе текста
    handleSearch(value);
  };

  const handleSelectUser = (email) => {
    setSearchQuery(email); // Устанавливаем email в поле поиска
    setUsers([]); // Очищаем список пользователей
    setUserSelected(true); // Устанавливаем состояние выбора пользователя
  };

  return (
    <div className="search-container">
      <input
        type="text"
        id="search"
        placeholder="Поиск..."
        className="search-input"
        value={searchQuery}
        onChange={handleChange}
      />
      <img src={search} alt="Поиск" className="search-icon" />

      {searchQuery &&
        users.length > 0 &&
        !userSelected && ( // Показываем список только если есть результаты и пользователь не выбран
          <div className="dropdown">
            <ul className="user-list">
              {users.map((user) => (
                <li
                  key={user.id}
                  className="user-item"
                  onClick={() => handleSelectUser(user.email)} // Добавляем обработчик выбора
                >
                  {user.email}
                </li>
              ))}
            </ul>
          </div>
        )}

      {/* Отображаем сообщение о том, что пользователи не найдены, только если есть текст в поиске */}
      {searchQuery && users.length === 0 && !userSelected && (
        <div className="dropdown">
          <ul className="user-list">
            <li className="no-users">Пользователи не найдены</li>
          </ul>
        </div>
      )}
    </div>
  );
};

export default Search;
