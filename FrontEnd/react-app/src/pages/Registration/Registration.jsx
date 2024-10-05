import React from "react";
import Header from "../../components/Header";
import RegistrForm from "../../components/RegistrForm";
import Footer from "../../components/Footer";
import "./Registration.css";

const Registration = () => {
  return (
    <div className="wrapper">
      <div className="content">
        <Header />
        <main className="main">
          <h1 className="title">Регистрация</h1>
          <RegistrForm />
        </main>
        <Footer />
      </div>
    </div>
  );
};

export default Registration;
