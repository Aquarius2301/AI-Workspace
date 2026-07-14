import { Flex, theme, Empty } from "antd";
import { useTranslation } from "react-i18next";
import { AICard } from "@/components";
import { CheckSquareOutlined } from "@ant-design/icons";
import type { TaskItemSummary } from "@/types";
import { RecentTaskItem } from "./RecentTaskItem";

interface RecentActivitySectionProps {
  isLoading: boolean;
  recentTasks: TaskItemSummary[];
}

export function RecentActivitySection({
  isLoading,
  recentTasks,
}: RecentActivitySectionProps) {
  const { t } = useTranslation();
  const { token } = theme.useToken();

  return (
    <AICard
      isLoading={isLoading}
      title={
        <Flex align="center" gap={8}>
          <CheckSquareOutlined style={{ color: token.colorPrimary }} />
          <span>{t("overviewPage.recentTasks.title")}</span>
        </Flex>
      }
      style={{ height: "100%" }}
    >
      {recentTasks.length === 0 ? (
        <div style={{ padding: "32px 0" }}>
          <Empty
            description={t("overviewPage.recentTasks.empty")}
            image={Empty.PRESENTED_IMAGE_SIMPLE}
          />
        </div>
      ) : (
        <Flex vertical>
          {recentTasks.slice(0, 5).map((task, idx) => (
            <RecentTaskItem
              key={task.id}
              task={task}
              isLast={idx === Math.min(recentTasks.length, 5) - 1}
            />
          ))}
        </Flex>
      )}
    </AICard>
  );
}
