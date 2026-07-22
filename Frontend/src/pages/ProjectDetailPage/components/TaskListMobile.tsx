import { useMemo } from "react";
import { Flex, Typography } from "antd";
import { Tabs } from "antd";
import { useTranslation } from "react-i18next";
import { AITaskStatusTag } from "@/components";
import { TASK_STATUS } from "@/types";
import type { TaskItemResult, TaskStatus } from "@/types";
import { TaskCard } from "./TaskCard";

const { Text } = Typography;

interface TaskListMobileProps {
  tasks: TaskItemResult[];
  isLoading: boolean;
  canEdit?: boolean;
  projectId?: string;
}

export function TaskListMobile({
  tasks,
  isLoading,
  canEdit,
  projectId,
}: TaskListMobileProps) {
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
    <Tabs
      items={TASK_STATUS.map((status) => ({
        key: status,
        label: (
          <Flex align="center" gap={4}>
            <AITaskStatusTag status={status} />
            <Text>{grouped[status].length}</Text>
          </Flex>
        ),
        children: (
          <Flex vertical gap={8}>
            {isLoading ? (
              <>
                <TaskCard isLoading />
                <TaskCard isLoading />
              </>
            ) : grouped[status].length > 0 ? (
              grouped[status].map((task) => (
                <TaskCard
                  key={task.id}
                  task={task}
                  canEdit={canEdit}
                  projectId={projectId}
                />
              ))
            ) : (
              <Flex justify="center" style={{ padding: "24px 0" }}>
                <Text type="secondary" style={{ fontSize: 13 }}>
                  {t("projectDetailPage.taskList.empty.title")}
                </Text>
              </Flex>
            )}
          </Flex>
        ),
      }))}
    />
  );
}
