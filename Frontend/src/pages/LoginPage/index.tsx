import { Card, Layout } from "antd";
import Banner from "./components/Banner";
import LoginForm from "./components/LoginForm";
import "./login.css";

export default function LoginPage() {
  return (
    <Layout className="login-page">
      <Card className="login-card" bordered={false}>
        <div className="login-card-content">
          <div className="login-banner-section">
            <Banner />
          </div>
          <div className="login-form-section">
            <LoginForm />
          </div>
        </div>
      </Card>
    </Layout>
  );
}
