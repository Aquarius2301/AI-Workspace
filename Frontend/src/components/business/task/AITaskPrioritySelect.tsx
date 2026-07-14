import { Select } from "antd";
import { TASK_PRIORITY } from "@/types";
import type { TaskPriority } from "@/types";
import { useTranslation } from "react-i18next";

export interface AITaskPrioritySelectProps {
  value?: TaskPriority | undefined;
  onChange?: (value: TaskPriority | undefined) => void;
  placeholder?: string;
  style?: React.CSSProperties;
  allowClear?: boolean;
}

export function AITaskPrioritySelect({
  value,
  onChange,
  placeholder,
  style,
  allowClear = true,
}: AITaskPrioritySelectProps) {
  const { t } = useTranslation();

  const options = TASK_PRIORITY.map((priority) => ({
    value: priority,
    label: t(`taskPrioritySelect.${priority}`),
  }));

  return (
    <Select<TaskPriority>
      style={{ ...style, minWidth: 140 }}
      placeholder={placeholder || t("taskPrioritySelect.all")}
      options={options}
      value={value}
      onChange={onChange}
      allowClear={allowClear}
    />
  );
}
