import { Row, Col, Flex, Typography, theme, Empty, Space } from "antd";
import { useTranslation } from "react-i18next";
import { AICard, AICardItem, AITeamRoleTag } from "@/components";
import { TeamOutlined } from "@ant-design/icons";
import type { TeamSummary } from "@/types";
import { useNavigate } from "react-router-dom";
import { ROUTE } from "@/constants";

const { Text } = Typography;

interface TeamSummariesCardProps {
  isLoading: boolean;
  teamSummaries: TeamSummary[];
}

export function TeamSummariesCard({
  isLoading,
  teamSummaries,
}: TeamSummariesCardProps) {
  const { t } = useTranslation();
  const { token } = theme.useToken();

  const navigate = useNavigate();

  return (
    <AICard
      isLoading={isLoading}
      title={
        <Flex align="center" gap={8}>
          <TeamOutlined style={{ color: token.colorPrimary }} />
          <span>{t("overviewPage.team.title")}</span>
        </Flex>
      }
    >
      {teamSummaries.length === 0 ? (
        <div style={{ padding: "32px 0" }}>
          <Empty
            description={t("overviewPage.team.empty")}
            image={Empty.PRESENTED_IMAGE_SIMPLE}
          />
        </div>
      ) : (
        <Row gutter={[16, 16]}>
          {teamSummaries.map((team) => (
            <Col xs={24} sm={12} lg={8} key={team.teamId}>
              <AICardItem
                isHoverable
                onClick={() => navigate(`${ROUTE.TEAM}/${team.slug}`)}
                isLoading={isLoading}
                style={{ height: "100%" }}
                header={
                  <Space>
                    <Text strong style={{ fontSize: 15 }}>
                      {team.teamName}
                    </Text>
                    <AITeamRoleTag role={team.myRole} />
                  </Space>
                }
                content={
                  <Flex gap={8} align="center" style={{ flexWrap: "wrap" }}>
                    <Text type="secondary" style={{ fontSize: 12 }}>
                      {team.memberCount} {t("overviewPage.team.members")}
                    </Text>
                    <Text type="secondary" style={{ fontSize: 12 }}>
                      ·
                    </Text>
                    <Text type="secondary" style={{ fontSize: 12 }}>
                      {team.projectCount} {t("overviewPage.team.projects")}
                    </Text>
                  </Flex>
                }
              />
            </Col>
          ))}
        </Row>
      )}
    </AICard>
  );
}
