import { Select } from "antd";
import type { TeamRole } from "@/types";

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
  placeholder = "Chọn vai trò...",
  showAll = true,
  style,
}: RoleSelectProps) {
  const options: { value: TeamRole | "All"; label: string }[] = [
    { value: "Admin", label: "Admin" },
    { value: "Leader", label: "Leader" },
    { value: "Member", label: "Member" },
  ];

  if (showAll) {
    options.unshift({ value: "All", label: "Tất cả" });
  }

  return (
    <Select<TeamRole | "All">
      style={style}
      placeholder={placeholder}
      options={options}
      value={value}
      onChange={(e) => onChange?.(e === "All" ? undefined : e)}
    />
  );
}
