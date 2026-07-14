import { Select } from "antd";
import { TASK_STATUS } from "@/types";
import type { TaskStatus } from "@/types";
import { useTranslation } from "react-i18next";

export interface AITaskStatusSelectProps {
  value?: TaskStatus | undefined;
  onChange?: (value: TaskStatus | undefined) => void;
  placeholder?: string;
  style?: React.CSSProperties;
  exceptRoles?: TaskStatus[];
  allowClear?: boolean;
}

export function AITaskStatusSelect({
  value,
  onChange,
  placeholder,
  style,
  exceptRoles = [],
  allowClear = true,
}: AITaskStatusSelectProps) {
  const { t } = useTranslation();

  const options = TASK_STATUS.filter((role) => !exceptRoles.includes(role)).map(
    (role) => ({
      value: role,
      label: t(`taskStatusSelect.${role}`),
    }),
  );

  return (
    <Select<TaskStatus>
      style={{ ...style, width: 180 }}
      placeholder={placeholder || t("taskStatusSelect.all")}
      options={options}
      value={value}
      onChange={onChange}
      allowClear={allowClear}
    />
  );
}
