document.getElementById('loginForm').addEventListener('submit', function(event) {
    event.preventDefault();

    const login = document.querySelector('.login').value;
    const password = document.querySelector('.password').value;

    const data = {
        login: login,
        password: password
    };

    fetch('https://localhost:7205/api/User/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        credentials: "include",
        body: JSON.stringify(data)
    })
    .then(response => {

        if (response.status === 401) {
            alert('Неверный логин или пароль. Попробуйте снова.');
        }

        if (!response.ok) {
            throw new Error('Ошибка авторизации');
        }
        
        //перейти дальше
    })
    .catch(error => {
        console.error(error);
    });
});
