import React from "react";
import ChatMessages from "./ChatMessages";
import GroupMessages from "./GroupMessages";

const Content = ({ activeSection }) => {
  return (
    <div id="content">
      {activeSection === "private" && <ChatMessages />}
      {activeSection === "group" && <GroupMessages />}
    </div>
  );
};

export default Content;
