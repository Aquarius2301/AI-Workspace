import { Typography } from "antd";

const { Title, Paragraph } = Typography;

export default function Banner() {
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
        Nơi trí tuệ nhân tạo và sáng tạo hội tụ
        <br />
        Khám phá sức mạnh của AI trong công việc của bạn
      </Paragraph>
    </div>
  );
}