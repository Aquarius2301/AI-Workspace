import { Row, Col, theme } from "antd";
import { useTranslation } from "react-i18next";
import {
  TeamOutlined,
  ProjectOutlined,
  CheckSquareOutlined,
  ClockCircleOutlined,
} from "@ant-design/icons";
import { StatCard } from "./StatCard";

interface StatCardsRowProps {
  isLoading: boolean;
  totalTeams: number;
  totalProjects: number;
  taskTotal: number;
  overdue: number;
}

export function StatCardsRow({
  isLoading,
  totalTeams,
  totalProjects,
  taskTotal,
  overdue,
}: StatCardsRowProps) {
  const { t } = useTranslation();
  const { token } = theme.useToken();

  return (
    <Row gutter={[16, 16]}>
      <Col xs={12} md={6}>
        <StatCard
          isLoading={isLoading}
          icon={<TeamOutlined />}
          value={totalTeams}
          label={t("overviewPage.welcome.teams")}
          color={token.colorPrimary}
        />
      </Col>
      <Col xs={12} md={6}>
        <StatCard
          isLoading={isLoading}
          icon={<ProjectOutlined />}
          value={totalProjects}
          label={t("overviewPage.welcome.projects")}
          color={token.colorSuccess}
        />
      </Col>
      <Col xs={12} md={6}>
        <StatCard
          isLoading={isLoading}
          icon={<CheckSquareOutlined />}
          value={taskTotal}
          label={t("overviewPage.welcome.taskStats")}
          color={token.colorWarning}
        />
      </Col>
      <Col xs={12} md={6}>
        <StatCard
          isLoading={isLoading}
          icon={<ClockCircleOutlined />}
          value={overdue}
          label={t("overviewPage.welcome.overdue")}
          color={token.colorError}
        />
      </Col>
    </Row>
  );
}
