document
  .getElementById("loginForm")
  .addEventListener("submit", async function (event) {
    event.preventDefault();

    const login = document.querySelector(".login").value;
    const password = document.querySelector(".password").value;
    try {
      const response = await fetch("https://localhost:7205/api/User/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ login: login, password: password }),
      });

      if (response.ok) {
        alert("Авторизация прошла успешно!");
        // перенаправление на другую страницу
        // window.location.href = "/dashboard";
      } else {
        const errorMessage = await response.text();
        alert(errorMessage);
      }
    } catch (error) {
      alert("Ошибка подключения к серверу.");
    }
  });
