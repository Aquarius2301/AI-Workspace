import { Select } from "antd";
import type { TeamRole } from "@/types";
import { useTranslation } from "react-i18next";

export interface RoleSelectProps {
  value?: TeamRole | "All";
  onChange?: (value: TeamRole | undefined) => void;
  placeholder?: string;
  showAll?: boolean;
  style?: React.CSSProperties;
}

export function RoleSelect({
  value,
  onChange,
  placeholder,
  showAll = true,
  style,
}: RoleSelectProps) {
  const { t } = useTranslation();
  const options: { value: TeamRole | "All"; label: string }[] = [
    { value: "Admin", label: t("roleSelect.admin") },
    { value: "Leader", label: t("roleSelect.leader") },
    { value: "Member", label: t("roleSelect.member") },
  ];

  if (showAll) {
    options.unshift({ value: "All", label: t("roleSelect.all") });
  }

  return (
    <Select<TeamRole | "All">
      style={style}
      placeholder={placeholder || t("roleSelect.placeholder")}
      options={options}
      value={value}
      onChange={(e) => onChange?.(e === "All" ? undefined : e)}
    />
  );
}
