import { Tag } from "antd";
import { useTranslation } from "react-i18next";
import type { TeamRole } from "@/types";

export interface AITeamRoleTagProps {
  role: TeamRole;
}

const roleConfig: Record<TeamRole, { color: string }> = {
  admin: { color: "red" },
  coAdmin: { color: "warning" },
  member: { color: "default" },
};

export function AITeamRoleTag({ role }: AITeamRoleTagProps) {
  const { t } = useTranslation();
  const config = roleConfig[role];
  return <Tag color={config.color}>{t(`roleSelect.${role}`)}</Tag>;
}
