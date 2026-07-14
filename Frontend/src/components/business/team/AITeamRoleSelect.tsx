import { Select } from "antd";
import { TEAM_ROLES } from "@/types";
import type { TeamRole } from "@/types";
import { useTranslation } from "react-i18next";

export interface AITeamRoleSelectProps {
  value?: TeamRole | undefined;
  onChange?: (value: TeamRole | undefined) => void;
  placeholder?: string;
  style?: React.CSSProperties;
  exceptRoles?: TeamRole[];
  allowClear?: boolean;
}

export function AITeamRoleSelect({
  value,
  onChange,
  placeholder,
  style,
  exceptRoles = [],
  allowClear = false,
}: AITeamRoleSelectProps) {
  const { t } = useTranslation();

  const options = TEAM_ROLES.filter((role) => !exceptRoles.includes(role)).map(
    (role) => ({
      value: role,
      label: t(`roleSelect.${role}`),
    }),
  );

  return (
    <Select<TeamRole>
      style={{ ...style, width: style?.width ?? 180 }}
      placeholder={placeholder || t("roleSelect.all")}
      options={options}
      value={value}
      onChange={onChange}
      allowClear={allowClear}
    />
  );
}
