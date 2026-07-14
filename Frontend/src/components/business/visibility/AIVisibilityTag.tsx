import { Tag } from "antd";
import { useTranslation } from "react-i18next";
import type { ProjectVisibility } from "@/types";

interface AIVisibilityTagProps {
  visibility: ProjectVisibility | string;
  size?: number;
}

const visibilityConfig: Record<string, { color: string }> = {
  Public: { color: "blue" },
  Private: { color: "default" },
};

export function AIVisibilityTag({ visibility, size }: AIVisibilityTagProps) {
  const { t } = useTranslation();
  const config = visibilityConfig[visibility] || visibilityConfig.Private;
  return (
    <Tag
      color={config.color}
      style={{ borderRadius: 6, margin: 0, fontSize: size || 12 }}
    >
      {visibility === "Public"
        ? t("visibilitySelect.public")
        : t("visibilitySelect.private")}
    </Tag>
  );
}
