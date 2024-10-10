import React from "react";

const ButtonContainer = ({ setActiveSection, activeSection }) => {
  return (
    <div className="button-container">
      <button
        id="privateBtn"
        className={`button ${activeSection === "private" ? "active" : ""}`}
        onClick={() => setActiveSection("private")}
      >
        Чаты
      </button>
      <button
        id="groupBtn"
        className={`button ${activeSection === "group" ? "active" : ""}`}
        onClick={() => setActiveSection("group")}
      >
        Группы
      </button>
    </div>
  );
};

export default ButtonContainer;
