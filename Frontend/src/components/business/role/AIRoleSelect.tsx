import { Select } from "antd";
import type { TeamRole } from "@/types";
import { useTranslation } from "react-i18next";

const ALL_ROLES: TeamRole[] = ["Admin", "CoAdmin", "Leader", "Member"];

export interface AIRoleSelectProps {
  value?: TeamRole | undefined;
  onChange?: (value: TeamRole | undefined) => void;
  placeholder?: string;
  style?: React.CSSProperties;
  exceptRoles?: TeamRole[];
  allowClear?: boolean;
}

export function AIRoleSelect({
  value,
  onChange,
  placeholder,
  style,
  exceptRoles = [],
  allowClear = true,
}: AIRoleSelectProps) {
  const { t } = useTranslation();

  const options = ALL_ROLES.filter((role) => !exceptRoles.includes(role)).map(
    (role) => ({
      value: role,
      label: t(`roleSelect.${role}`),
    }),
  );

  return (
    <Select<TeamRole>
      style={{ ...style, width: 180 }}
      placeholder={placeholder || t("roleSelect.all")}
      options={options}
      value={value}
      onChange={onChange}
      allowClear={allowClear}
    />
  );
}
