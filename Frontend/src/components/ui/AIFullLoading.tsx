import { Spin, theme } from "antd";

export interface AIFullscreenLoadingProps {
  description?: string;
}

export function AIFullscreenLoading({ description }: AIFullscreenLoadingProps) {
  const { token } = theme.useToken();

  return (
    <Spin
      size="large"
      description={description}
      fullscreen
      style={{
        color: token.colorText,
      }}
    />
  );
}
