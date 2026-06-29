import { Tag } from "antd";
import { useTranslation } from "react-i18next";
import type { TaskStatus } from "@/types";

interface AITaskStatusTagProps {
  status: TaskStatus | string;
}

const statusConfig: Record<string, { color: string }> = {
  Open: { color: "blue" },
  InProgress: { color: "orange" },
  Done: { color: "green" },
  Blocked: { color: "red" },
  Overdue: { color: "volcano" },
};

const statusKeyMap: Record<string, string> = {
  Open: "taskStatus.open",
  InProgress: "taskStatus.inProgress",
  Done: "taskStatus.done",
  Blocked: "taskStatus.blocked",
  Overdue: "taskStatus.overdue",
};

export function AITaskStatusTag({ status }: AITaskStatusTagProps) {
  const { t } = useTranslation();
  const config = statusConfig[status] || statusConfig.Open;
  return (
    <Tag color={config.color} style={{ margin: 0, fontSize: 11 }}>
      {t(statusKeyMap[status] || status)}
    </Tag>
  );
}
