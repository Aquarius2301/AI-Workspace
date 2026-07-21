import { useMemo } from "react";
import { Flex, Typography } from "antd";
import { useTranslation } from "react-i18next";
import { AITaskStatusTag } from "@/components";
import { TASK_STATUS } from "@/types";
import type { TaskItemResult, TaskStatus } from "@/types";
import { TaskCard } from "./TaskCard";

const { Text } = Typography;

interface TaskListProps {
  tasks: TaskItemResult[];
  isLoading: boolean;
}

const COLORS: Record<TaskStatus, string> = {
  toDo: "#1677ff",
  doing: "#fa8c16",
  done: "#52c41a",
};

export function TaskList({ tasks, isLoading }: TaskListProps) {
  const { t } = useTranslation();

  const grouped = useMemo(() => {
    const groups: Record<TaskStatus, TaskItemResult[]> = {
      toDo: [],
      doing: [],
      done: [],
    };
    tasks.forEach((task) => {
      if (groups[task.status]) {
        groups[task.status].push(task);
      }
    });
    return groups;
  }, [tasks]);

  return (
    <Flex gap={16} style={{ overflowX: "auto", paddingBottom: 8 }}>
      {TASK_STATUS.map((status) => (
        <div key={status} style={{ flex: 1, minWidth: 280, maxWidth: 420 }}>
          {/* Column header */}
          <Flex align="center" gap={8} style={{ marginBottom: 12 }}>
            <span
              style={{
                width: 10,
                height: 10,
                borderRadius: "50%",
                background: COLORS[status],
                display: "inline-block",
                flexShrink: 0,
              }}
            />
            <Text strong style={{ fontSize: 15 }}>
              {t(`taskStatusSelect.${status}`)}
            </Text>
            <AITaskStatusTag status={status} />
            <Text type="secondary" style={{ fontSize: 13 }}>
              {grouped[status].length}
            </Text>
          </Flex>

          {/* Task list */}
          <Flex vertical gap={8}>
            {isLoading ? (
              <>
                <TaskCard isLoading />
                <TaskCard isLoading />
              </>
            ) : grouped[status].length > 0 ? (
              grouped[status].map((task) => (
                <TaskCard key={task.id} task={task} />
              ))
            ) : (
              <Flex justify="center" style={{ padding: "24px 0" }}>
                <Text type="secondary" style={{ fontSize: 13 }}>
                  {t("projectDetailPage.taskList.empty.title")}
                </Text>
              </Flex>
            )}
          </Flex>
        </div>
      ))}
    </Flex>
  );
}
