import { useTranslation } from "react-i18next";

export const useLanguage = () => {
  const { i18n } = useTranslation();

  const changeLanguage = (lang: "vi" | "en") => {
    i18n.changeLanguage(lang);
  };

  return {
    language: i18n.language as "vi" | "en",
    changeLanguage,
  };
};
