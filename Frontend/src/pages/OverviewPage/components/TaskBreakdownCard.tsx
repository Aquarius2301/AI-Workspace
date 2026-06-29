import { Flex, Row, Col, Typography, theme, Progress } from "antd";
import { useTranslation } from "react-i18next";
import { AICard } from "@/components";
import type { TaskSummary } from "@/types";

const { Text } = Typography;

interface TaskBreakdownCardProps {
  isLoading: boolean;
  taskSummary: TaskSummary;
}

const TASK_BADGE_CONFIG = [
  { key: "open" as const, color: "#1677ff" },
  { key: "inProgress" as const, color: "#fa8c16" },
  { key: "done" as const, color: "#52c41a" },
  { key: "blocked" as const, color: "#ff4d4f" },
  { key: "overdue" as const, color: "#fa541c" },
];

export function TaskBreakdownCard({
  isLoading,
  taskSummary,
}: TaskBreakdownCardProps) {
  const { t } = useTranslation();
  const { token } = theme.useToken();

  const completionPct =
    taskSummary.total > 0
      ? Math.round((taskSummary.done / taskSummary.total) * 100)
      : 0;

  return (
    <AICard isLoading={isLoading} title={t("overview.taskStats.default")}>
      <Flex vertical gap={20}>
        <Progress
          percent={completionPct}
          strokeColor={token.colorSuccess}
          format={() =>
            taskSummary.total > 0
              ? `${taskSummary.done}/${taskSummary.total} ${t("overview.done").toLowerCase()}`
              : "—"
          }
        />
        <Row gutter={[8, 12]}>
          {TASK_BADGE_CONFIG.map((item) => (
            <Col xs={12} sm={8} md={4} key={item.key}>
              <Flex align="center" gap={8}>
                <div
                  style={{
                    width: 8,
                    height: 8,
                    borderRadius: "50%",
                    background: item.color,
                    flexShrink: 0,
                  }}
                />
                <Text
                  style={{
                    fontSize: 13,
                    color: token.colorTextSecondary,
                  }}
                >
                  {t(`overview.${item.key}`)}
                </Text>
                <Text strong style={{ fontSize: 14 }}>
                  {taskSummary[item.key]}
                </Text>
              </Flex>
            </Col>
          ))}
        </Row>
      </Flex>
    </AICard>
  );
}
