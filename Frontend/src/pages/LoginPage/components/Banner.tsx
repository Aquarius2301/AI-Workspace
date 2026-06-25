import { Typography } from "antd";
import { useTranslation } from "react-i18next";

const { Title, Paragraph } = Typography;

export default function Banner() {
  const { t } = useTranslation();
  return (
    <div className="banner-container">
      <div className="banner-icons">
        <span className="banner-icon">🤖</span>
        <span className="banner-icon">⚡</span>
        <span className="banner-icon">☁️</span>
      </div>
      <Title level={1} className="banner-title">
        AI Workspace
      </Title>
      <Paragraph className="banner-slogan">
        {t("login.bannerSlogan1")}
        <br />
        {t("login.bannerSlogan2")}
      </Paragraph>
    </div>
  );
}
