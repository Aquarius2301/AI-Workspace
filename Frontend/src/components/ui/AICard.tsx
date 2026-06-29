import React, { useState } from "react";
import { Card, theme } from "antd";

export interface AICardProps {
  isLoading?: boolean;
  isHovering?: boolean;
  children?: React.ReactNode;
  style?: React.CSSProperties;
  title?: React.ReactNode;
  extra?: React.ReactNode;
  onClick?: () => void;
}

export function AICard({
  isLoading = false,
  isHovering = false,
  children,
  style,
  title,
  extra,
  onClick,
}: AICardProps) {
  const { token } = theme.useToken();
  const [isHovered, setIsHovered] = useState(false);

  const shouldLift = isHovering && isHovered;

  return (
    <Card
      title={title}
      extra={extra}
      loading={isLoading}
      onClick={onClick}
      onMouseEnter={() => setIsHovered(true)}
      onMouseLeave={() => setIsHovered(false)}
      style={{
        borderRadius: token.borderRadiusLG,
        border: `1px solid ${shouldLift ? token.colorPrimary : token.colorBorder}`,
        background: token.colorBgContainer,
        transition: "all 0.3s cubic-bezier(0.4, 0, 0.2, 1)",
        transform: shouldLift ? "translateY(-6px)" : "translateY(0)",
        boxShadow: shouldLift
          ? `0 12px 32px -8px ${token.colorPrimary}30`
          : "0 1px 3px 0 rgba(0, 0, 0, 0.06)",
        cursor: onClick || isHovering ? "pointer" : undefined,
        ...style,
      }}
    >
      {children}
    </Card>
  );
}
