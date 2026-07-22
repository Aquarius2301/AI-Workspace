import { useMemo, type DragEvent } from "react";
import { Flex, Typography } from "antd";
import { useTranslation } from "react-i18next";
import { AITaskStatusTag } from "@/components";
import { TASK_STATUS } from "@/types";
import type { TaskItemResult, TaskStatus } from "@/types";
import { useUpdateTaskStatus, useAdminUpdateTaskStatus } from "@/hooks";
import { TaskCard } from "./TaskCard";

const { Text } = Typography;

interface TaskListProps {
  tasks: TaskItemResult[];
  isLoading: boolean;
  canEdit?: boolean;
  projectId?: string;
}

const COLORS: Record<TaskStatus, string> = {
  toDo: "#1677ff",
  doing: "#fa8c16",
  done: "#52c41a",
};

export function TaskList({
  tasks,
  isLoading,
  canEdit,
  projectId,
}: TaskListProps) {
  const { t } = useTranslation();
  const updateTaskStatus = useUpdateTaskStatus(projectId ?? "");
  const adminUpdateTaskStatus = useAdminUpdateTaskStatus(projectId ?? "");

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

  const handleDragOver = (e: DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    e.dataTransfer.dropEffect = "move";
  };

  const handleDrop =
    (columnStatus: TaskStatus) => (e: DragEvent<HTMLDivElement>) => {
      e.preventDefault();
      const taskId = e.dataTransfer.getData("text/plain");
      if (!taskId || !projectId) return;

      // Don't update if the task is already in this column
      const task = tasks.find((t) => t.id === taskId);
      if (!task || task.status === columnStatus) return;

      if (canEdit) {
        adminUpdateTaskStatus.mutate({ taskId, status: columnStatus });
      } else {
        updateTaskStatus.mutate({ taskId, status: columnStatus });
      }
    };

  return (
    <Flex gap={16} style={{ overflowX: "auto", paddingBottom: 8 }}>
      {TASK_STATUS.map((status) => (
        <div
          key={status}
          style={{ flex: 1, minWidth: 280, maxWidth: 420 }}
          onDragOver={handleDragOver}
          onDrop={handleDrop(status)}
        >
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
        </div>
      ))}
    </Flex>
  );
}
