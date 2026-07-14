import { AITaskStatusTag } from "@/components";
import type { TaskItemSummary } from "@/types";
import { formatIsoLocaleDate } from "@/utils";
import { Flex, theme } from "antd";
import Text from "antd/es/typography/Text";

interface RecentTaskItemProps {
  task: TaskItemSummary;
  isLast: boolean;
}

export function RecentTaskItem({ task, isLast }: RecentTaskItemProps) {
  const { token } = theme.useToken();

  return (
    <Flex
      vertical
      style={{
        padding: "14px 0",
        borderBottom: isLast
          ? undefined
          : `1px solid ${token.colorBorderSecondary}`,
      }}
    >
      <Flex
        align="center"
        justify="space-between"
        gap={8}
        style={{ marginBottom: 6 }}
      >
        <Text
          strong
          style={{ fontSize: 14, flex: 1 }}
          ellipsis={{ tooltip: task.title }}
        >
          {task.title}
        </Text>
        <AITaskStatusTag status={task.status} />
      </Flex>
      <Flex align="center" gap={8} style={{ flexWrap: "wrap" }}>
        <Text type="secondary" style={{ fontSize: 12 }}>
          {task.projectName}
        </Text>
        <Text type="secondary" style={{ fontSize: 12 }}>
          ·
        </Text>
        <Text type="secondary" style={{ fontSize: 12 }}>
          {formatIsoLocaleDate(task.createdAt)}
        </Text>
        {task.dueDate && (
          <>
            <Text type="secondary" style={{ fontSize: 12 }}>
              ·
            </Text>
            <Text
              type="secondary"
              style={{
                fontSize: 12,
                color:
                  new Date(task.dueDate) < new Date()
                    ? token.colorError
                    : undefined,
              }}
            >
              {formatIsoLocaleDate(task.dueDate)}
            </Text>
          </>
        )}
      </Flex>
    </Flex>
  );
}
