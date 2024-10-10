import React from "react";
import search from "../../assets/images/search.png";

const Search = () => {
  return (
    <div className="search-container">
      <input
        type="text"
        id="search"
        placeholder="Поиск..."
        className="search-input"
      />
      <img src={search} alt="Поиск" className="search-icon" />
    </div>
  );
};

export default Search;
