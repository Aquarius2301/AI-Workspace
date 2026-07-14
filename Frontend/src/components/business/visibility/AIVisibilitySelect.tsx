import { Select } from "antd";
import { PROJECT_VISIBILIES, type ProjectVisibility } from "@/types";
import { useTranslation } from "react-i18next";

export interface AIVisibilitySelectProps {
  value?: ProjectVisibility | undefined;
  onChange?: (value: ProjectVisibility | undefined) => void;
  placeholder?: string;
  style?: React.CSSProperties;
  exceptVisibilities?: ProjectVisibility[];
  allowClear?: boolean;
}

export function AIVisibilitySelect({
  value,
  onChange,
  placeholder,
  style,
  exceptVisibilities = [],
  allowClear = false,
}: AIVisibilitySelectProps) {
  const { t } = useTranslation();

  const options = PROJECT_VISIBILIES.filter(
    (v) => !exceptVisibilities.includes(v),
  ).map((v) => ({
    value: v,
    label: t(`visibilitySelect.${v}`),
  }));

  return (
    <Select<ProjectVisibility | "All">
      style={{ ...style, width: 180 }}
      placeholder={placeholder || t("visibilitySelect.all")}
      options={options}
      value={value}
      onChange={(e) => onChange?.(e === "All" ? undefined : e)}
      allowClear={allowClear}
    />
  );
}
