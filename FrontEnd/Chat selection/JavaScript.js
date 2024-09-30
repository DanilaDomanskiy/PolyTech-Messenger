document.addEventListener('DOMContentLoaded', function () {
    const privateBtn = document.getElementById('privateBtn');
    const groupBtn = document.getElementById('groupBtn');
    const privateMessages = document.getElementById('privateMessages');
    const groupMessages = document.getElementById('groupMessages');

    // Обработка нажатий на кнопки
    privateBtn.addEventListener('click', function () {
        privateBtn.classList.add('active');
        groupBtn.classList.remove('active');
        privateMessages.style.display = 'block';
        groupMessages.style.display = 'none';
    });

    groupBtn.addEventListener('click', function () {
        groupBtn.classList.add('active');
        privateBtn.classList.remove('active');
        groupMessages.style.display = 'block';
        privateMessages.style.display = 'none';
    });
});
