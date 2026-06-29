import { theme, type ThemeConfig } from "antd";

export const aiWorkspaceTheme = {
  // Light mode
  light: {
    algorithm: theme.defaultAlgorithm,
    token: {
      colorPrimary: "#4F46E5", // Màu Indigo chủ đạo cho Button, Active state
      colorBgBase: "#FFFFFF", // Nền chính
      colorBgLayout: "#F8FAFC", // Nền Sidebar (Slate 50)
      colorTextBase: "#0F172A", // Chữ chính (Slate 900)
      colorTextDescription: "#475569", // Chữ phụ (Slate 600)
      colorBorder: "#E2E8F0", // Đường viền (Slate 200)
      borderRadius: 8, // Bo góc hiện đại cho các component
    },
    components: {
      Layout: {
        siderBg: "#F8FAFC", // Custom riêng cho màu nền Sidebar
      },
    },
  } as ThemeConfig,

  // Dark mode
  dark: {
    algorithm: theme.darkAlgorithm,
    token: {
      colorPrimary: "#6366F1", // Indigo sáng hơn một chút để nổi bật trên nền tối
      colorBgBase: "#09090B", // Nền tối sâu (Zinc 950)
      colorBgContainer: "#18181B", // Nền của Card, Input, Menu (Zinc 900)
      colorBgLayout: "#09090B", // Nền Layout tổng
      colorTextBase: "#FAFAFA", // Chữ chính (Zinc 50)
      colorTextDescription: "#A1A1AA", // Chữ phụ (Zinc 400)
      colorBorder: "#27272A", // Đường viền tối (Zinc 800)
      borderRadius: 8,
    },
    components: {
      Layout: {
        siderBg: "#18181B",
      },
    },
  } as ThemeConfig,
};
