import React from "react";
import "../styles/Modal.css";

const Modal = ({ message, onClose }) => {
  return (
    <div className="modal-overlay">
      <div className="modal">
        <div className="modal-content">
          <p>{message}</p>
          <button onClick={onClose} className="modal-button">
            ОК
          </button>
        </div>
      </div>
    </div>
  );
};

export default Modal;
