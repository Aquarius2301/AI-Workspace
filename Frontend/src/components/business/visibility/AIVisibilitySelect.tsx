import { Select } from "antd";
import type { ProjectVisibility } from "@/types";
import { useTranslation } from "react-i18next";

export interface AIVisibilitySelectProps {
  value?: ProjectVisibility | undefined;
  onChange?: (value: ProjectVisibility | undefined) => void;
  placeholder?: string;
  style?: React.CSSProperties;
  exceptVisibilities?: ProjectVisibility[];
  allowClear?: boolean;
}

const ALL_VISIBILITIES: ProjectVisibility[] = ["Public", "Private"];

export function AIVisibilitySelect({
  value,
  onChange,
  placeholder,
  style,
  exceptVisibilities = [],
  allowClear = true,
}: AIVisibilitySelectProps) {
  const { t } = useTranslation();

  const options = ALL_VISIBILITIES.filter(
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
