import { Grid, Flex, Typography, theme } from "antd";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { AICard } from "@/components";
import {
  TeamOutlined,
  ProjectOutlined,
  BarChartOutlined,
  RightOutlined,
} from "@ant-design/icons";

const { useBreakpoint } = Grid;
const { Title, Text } = Typography;

const QUICK_ACTIONS = [
  {
    key: "overview",
    icon: <BarChartOutlined />,
    titleKey: "homePage.overview.title",
    descKey: "homePage.overview.desc",
    path: "/overview",
    color: "#1677ff",
  },
  {
    key: "teams",
    icon: <TeamOutlined />,
    titleKey: "homePage.teams.title",
    descKey: "homePage.teams.desc",
    path: "/teams",
    color: "#722ed1",
  },
  {
    key: "projects",
    icon: <ProjectOutlined />,
    titleKey: "homePage.projects.title",
    descKey: "homePage.projects.desc",
    path: "/projects",
    color: "#13c2c2",
  },
];

export default function HomePage() {
  const { t } = useTranslation();
  const { token } = theme.useToken();
  const screens = useBreakpoint();
  const isMobile = !screens.md;
  const navigate = useNavigate();

  return (
    <Flex
      vertical
      align="center"
      justify="center"
      style={{
        minHeight: "100dvh",
        background: `linear-gradient(135deg, ${token.colorPrimaryBg} 0%, ${token.colorBgLayout} 100%)`,
        padding: isMobile ? 24 : 40,
      }}
    >
      {/* ── Hero ── */}
      <Flex
        vertical
        align="center"
        style={{ maxWidth: 600, textAlign: "center" }}
      >
        <Title level={2} style={{ marginBottom: 8, fontWeight: 700 }}>
          {t("homePage.hero.title")}
        </Title>
        <Text
          type="secondary"
          style={{
            fontSize: isMobile ? 15 : 17,
            marginBottom: 40,
            display: "block",
          }}
        >
          {t("homePage.hero.subtitle")}
        </Text>
      </Flex>

      {/* ── Quick Actions ── */}
      <Flex
        vertical={isMobile}
        gap={16}
        style={{ width: "100%", maxWidth: 800 }}
      >
        {QUICK_ACTIONS.map((action) => (
          <AICard
            key={action.key}
            onClick={() => navigate(action.path)}
            style={{ width: "100%" }}
          >
            <Flex align="center" gap={16}>
              <Flex
                align="center"
                justify="center"
                style={{
                  width: 48,
                  height: 48,
                  borderRadius: token.borderRadius,
                  background: `${action.color}14`,
                  fontSize: 22,
                  color: action.color,
                  flexShrink: 0,
                }}
              >
                {action.icon}
              </Flex>
              <Flex vertical flex={1}>
                <Text strong style={{ fontSize: 15 }}>
                  {t(action.titleKey)}
                </Text>
                <Text type="secondary" style={{ fontSize: 13 }}>
                  {t(action.descKey)}
                </Text>
              </Flex>
              <RightOutlined style={{ color: token.colorTextQuaternary }} />
            </Flex>
          </AICard>
        ))}
      </Flex>
    </Flex>
  );
}
