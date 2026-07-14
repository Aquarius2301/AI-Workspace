import { useTheme } from "@/hooks";
import { Switch } from "antd";
import { MoonOutlined, SunOutlined } from "@ant-design/icons";

export function AIThemeSwitch() {
  const { theme: currentTheme, toggleTheme } = useTheme();

  return (
    <Switch
      checked={currentTheme === "dark"}
      onChange={toggleTheme}
      checkedChildren={<MoonOutlined />}
      unCheckedChildren={<SunOutlined />}
    />
  );
}
