import { Select } from "antd";
import { PROJECT_ROLES } from "@/types";
import type { ProjectRole } from "@/types";
import { useTranslation } from "react-i18next";

export interface AIProjectRoleSelectProps {
  value?: ProjectRole | undefined;
  onChange?: (value: ProjectRole | undefined) => void;
  placeholder?: string;
  style?: React.CSSProperties;
  exceptRoles?: ProjectRole[];
  allowClear?: boolean;
}

export function AIProjectRoleSelect({
  value,
  onChange,
  placeholder,
  style,
  exceptRoles = [],
  allowClear = true,
}: AIProjectRoleSelectProps) {
  const { t } = useTranslation();

  const options = PROJECT_ROLES.filter(
    (role) => !exceptRoles.includes(role),
  ).map((role) => ({
    value: role,
    label: t(`roleSelect.${role}`),
  }));

  return (
    <Select<ProjectRole>
      style={{ ...style, width: 180 }}
      placeholder={placeholder || t("roleSelect.all")}
      options={options}
      value={value}
      onChange={onChange}
      allowClear={allowClear}
    />
  );
}
