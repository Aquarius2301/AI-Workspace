import { Tag } from "antd";
import { useTranslation } from "react-i18next";

export interface AIProjectRoleTagProps {
  role: string;
}

const roleConfig: Record<string, { color: string }> = {
  leader: { color: "geekblue" },
  member: { color: "default" },
};

export function AIProjectRoleTag({ role }: AIProjectRoleTagProps) {
  const { t } = useTranslation();
  const config = roleConfig[role] ?? { color: "default" };
  return <Tag color={config.color}>{t(`roleSelect.${role}`)}</Tag>;
}
