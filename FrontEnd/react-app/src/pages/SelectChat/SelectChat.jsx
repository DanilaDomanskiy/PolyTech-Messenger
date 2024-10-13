import React, { useState } from "react";
import "./SelectChat.css";
import Search from "../../components/SelectChat/Search";
import Content from "../../components/SelectChat/Content";
import ButtonContainer from "../../components/SelectChat/ButtonContainer";

const SelectChat = () => {
  const [activeSection, setActiveSection] = useState("private");

  return (
    <div className="container">
      <Search />
      <Content activeSection={activeSection} />
      <ButtonContainer
        setActiveSection={setActiveSection}
        activeSection={activeSection}
      />
    </div>
  );
};

export default SelectChat;
