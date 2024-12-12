import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import en from "../locales/en.json";
import ru from "../locales/ru.json";

i18n
  .use(initReactI18next) // подключаем react-i18next
  .init({
    resources: {
      en: {
        translation: en,
      },
      ru: {
        translation: ru,
      },
    },
    lng: "ru", // Язык по умолчанию
    fallbackLng: "ru", // Если перевод не найден
    interpolation: {
      escapeValue: false, // React уже безопасно обрабатывает вывод
    },
  });

export default i18n;
