import React from "react";
import Header from "../../components/Header";
import AuthForm from "../../components/AuthForm";
import Footer from "../../components/Footer";
import "./Login.css";

const Login = () => {
  return (
    <div className="wrapper">
      <div className="content">
        <Header />
        <main className="main">
          <h1 className="title">PolyTech Messenger</h1>
          <AuthForm />
        </main>
        <Footer />
      </div>
    </div>
  );
};

export default Login;
