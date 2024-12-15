import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import en from "../locales/en.json";
import ru from "../locales/ru.json";
import be from "../locales/be.json";

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
      be: {
        translation: be,
      },
    },
    lng: "ru", // Язык по умолчанию
    fallbackLng: "ru", // Если перевод не найден
    interpolation: {
      escapeValue: false, // React уже безопасно обрабатывает вывод
    },
  });

export default i18n;
