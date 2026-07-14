import { Tag } from "antd";
import { useTranslation } from "react-i18next";
import type { TaskPriority } from "@/types";

interface AITaskPriorityTagProps {
  priority: TaskPriority;
}

const priorityConfig: Record<TaskPriority, { color: string }> = {
  low: { color: "blue" },
  medium: { color: "gold" },
  high: { color: "volcano" },
};

export function AITaskPriorityTag({ priority }: AITaskPriorityTagProps) {
  const { t } = useTranslation();
  const config = priorityConfig[priority];
  return <Tag color={config.color}>{t(`taskPrioritySelect.${priority}`)}</Tag>;
}
