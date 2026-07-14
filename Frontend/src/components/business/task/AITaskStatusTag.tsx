import { Tag } from "antd";
import { useTranslation } from "react-i18next";
import type { TaskStatus } from "@/types";

interface AITaskStatusTagProps {
  status: TaskStatus | "overdue";
}

const statusConfig: Record<TaskStatus | "overdue", { color: string }> = {
  toDo: { color: "blue" },
  doing: { color: "orange" },
  done: { color: "green" },
  overdue: { color: "volcano" },
};

export function AITaskStatusTag({ status }: AITaskStatusTagProps) {
  const { t } = useTranslation();
  const config = statusConfig[status];
  return <Tag color={config.color}>{t(`taskStatusSelect.${status}`)}</Tag>;
}
