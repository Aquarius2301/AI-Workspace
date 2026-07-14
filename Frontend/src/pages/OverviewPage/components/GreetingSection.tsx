import { Flex, Typography, Grid } from "antd";
import { useTranslation } from "react-i18next";

const { useBreakpoint } = Grid;
const { Title, Text } = Typography;

interface GreetingSectionProps {
  userName?: string;
  taskTotal: number;
  totalTeams: number;
  isMobile?: boolean;
}

// ── Helpers ──────────────────────────────────────────────

function getGreeting(): string {
  const hour = new Date().getHours();
  if (hour < 12) return "overviewPage.welcome.morning";
  if (hour < 18) return "overviewPage.welcome.afternoon";
  return "overviewPage.welcome.evening";
}

function getGreetingEmoji(): string {
  const hour = new Date().getHours();
  if (hour < 12) return "🌤️";
  if (hour < 18) return "☀️";
  return "🌙";
}

export function GreetingSection({
  userName,
  taskTotal,
  totalTeams,
}: GreetingSectionProps) {
  const { t } = useTranslation();
  const screens = useBreakpoint();
  const isMobile = !screens.md;

  return (
    <Flex
      vertical={isMobile}
      align={isMobile ? "stretch" : "center"}
      justify="space-between"
      gap={8}
    >
      <Flex vertical gap={4}>
        <Title
          level={4}
          style={{
            margin: 0,
            fontWeight: 600,
            fontSize: isMobile ? 20 : 24,
          }}
        >
          {t(getGreeting())}
          {userName ? `, ${userName}` : ""} {getGreetingEmoji()}
        </Title>
        <Text type="secondary" style={{ fontSize: 15 }}>
          {taskTotal > 0
            ? t("overviewPage.welcome.taskStats") + ": " + taskTotal
            : ""}
          {taskTotal > 0 && totalTeams > 0 ? " · " : ""}
          {totalTeams > 0
            ? t("overviewPage.welcome.teams") + ": " + totalTeams
            : ""}
        </Text>
      </Flex>
    </Flex>
  );
}
