import { Tag } from "antd";
import { useTranslation } from "react-i18next";
import type { TeamRole } from "@/types";

export interface AIRoleTagProps {
  role: TeamRole | string;
}

const roleConfig: Record<string, { color: string }> = {
  Admin: { color: "red" },
  CoAdmin: { color: "warning" },
  Leader: { color: "processing" },
  Member: { color: "default" },
};

export function AIRoleTag({ role }: AIRoleTagProps) {
  const { t } = useTranslation();
  const config = roleConfig[role] || roleConfig.Member;
  return <Tag color={config.color}>{t(`roleSelect.${role}`)}</Tag>;
}
