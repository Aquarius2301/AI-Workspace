import { Flex, Typography } from "antd";
import { UserOutlined, CalendarOutlined } from "@ant-design/icons";
import { AICard, AITaskPriorityTag } from "@/components";
import type { TaskItemResult } from "@/types";
import { formatIsoLocaleDate } from "@/utils";

const { Text } = Typography;

interface TaskCardProps {
  task?: TaskItemResult;
  isLoading?: boolean;
}

export function TaskCard({ task, isLoading }: TaskCardProps) {
  // Loading state: render skeleton only
  if (isLoading) {
    return <AICard isLoading />;
  }

  // Should never happen when not loading
  if (!task) return null;

  return (
    <AICard style={{ marginTop: 8 }}>
      <Flex vertical gap={6}>
        {/* Title + Priority */}
        <Flex align="center" justify="space-between" gap={8}>
          <Text strong style={{ fontSize: 14, flex: 1, lineHeight: 1.4 }}>
            {task.title}
          </Text>
          <AITaskPriorityTag priority={task.priority} />
        </Flex>

        {/* Description preview */}
        {task.description && (
          <Text
            type="secondary"
            style={{
              fontSize: 13,
              display: "-webkit-box",
              WebkitLineClamp: 2,
              WebkitBoxOrient: "vertical",
              overflow: "hidden",
              lineHeight: 1.4,
            }}
          >
            {task.description}
          </Text>
        )}

        {/* Assigned user + Due date */}
        <Flex justify="space-between" align="center" style={{ marginTop: 2 }}>
          {task.assignedToName ? (
            <Text type="secondary" style={{ fontSize: 12 }}>
              <UserOutlined style={{ marginRight: 4 }} />
              {task.assignedToName}
            </Text>
          ) : (
            <span />
          )}
          {task.dueDate && (
            <Text type="secondary" style={{ fontSize: 12 }}>
              <CalendarOutlined style={{ marginRight: 4 }} />
              {formatIsoLocaleDate(task.dueDate)}
            </Text>
          )}
        </Flex>
      </Flex>
    </AICard>
  );
}
