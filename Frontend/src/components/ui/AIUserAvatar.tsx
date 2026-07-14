import { getDistanceToNow } from "@/utils";
import { Avatar, Tooltip, theme } from "antd";
import { useTranslation } from "react-i18next";

interface UserAvatarProps {
  userId?: string;
  userName?: string;
  src?: string;
  size?: number;
  showActive?: boolean;
  lastActiveAt?: string;
}

const avatarColors = [
  "#1677ff",
  "#722ed1",
  "#13c2c2",
  "#52c41a",
  "#eb2f96",
  "#fa8c16",
  "#f5222d",
  "#2f54eb",
  "#a0d911",
  "#fa541c",
];

function getColor(id: string): string {
  let hash = 0;
  for (let i = 0; i < id.length; i++) {
    hash = id.charCodeAt(i) + ((hash << 5) - hash);
  }

  return avatarColors[Math.abs(hash) % avatarColors.length];
}

function getInitials(name: string): string {
  return name ? name.charAt(0).toUpperCase() : "?";
}

function getTimeTooltip(
  timeDistance: number | null,
  t: (key: string, options?: any) => string,
): string {
  if (timeDistance === null) return t("userAvatar.offline");
  if (timeDistance < 1) return t("userAvatar.online");
  if (timeDistance < 60)
    return t("userAvatar.minutesAgo", { minutes: timeDistance });
  const hours = Math.floor(timeDistance / 60);
  if (hours < 24) return t("userAvatar.hoursAgo", { hours });
  const days = Math.floor(hours / 24);
  return t("userAvatar.daysAgo", { days });
}

export function UserAvatar({
  userId,
  userName,
  src,
  size = 44,
  showActive = false,
  lastActiveAt,
}: UserAvatarProps) {
  const { t } = useTranslation();
  const { token } = theme.useToken();
  const backgroundColor = userId ? getColor(userId) : "#7b7b7bff";
  const initials = getInitials(userName || "");

  const timeDistance = getDistanceToNow(lastActiveAt, "minute");

  const avatar = (
    <div
      style={{ position: "relative", flexShrink: 0, display: "inline-block" }}
    >
      <Avatar
        size={size}
        src={src}
        style={{
          backgroundColor: src ? "transparent" : backgroundColor,
          verticalAlign: "middle",
          fontWeight: 600,
          fontSize: size >= 40 ? 16 : size >= 28 ? 13 : 11,
        }}
      >
        {!src && initials}
      </Avatar>
      {showActive &&
        (timeDistance !== null && timeDistance <= 10 ? (
          <Tooltip title={t("userAvatar.online")}>
            <span
              style={{
                position: "absolute",
                bottom: 0,
                right: 0,
                width: size >= 40 ? 12 : 10,
                height: size >= 40 ? 12 : 10,
                borderRadius: "50%",
                background: "#52c41a",
                border: `2px solid ${token.colorBgContainer}`,
              }}
            />
          </Tooltip>
        ) : (
          <Tooltip title={getTimeTooltip(timeDistance, t)}>
            <span
              style={{
                position: "absolute",
                bottom: 0,
                right: 0,
                width: size >= 40 ? 12 : 10,
                height: size >= 40 ? 12 : 10,
                borderRadius: "50%",
                background: "#777777ff",
                border: `2px solid ${token.colorBgContainer}`,
              }}
            />
          </Tooltip>
        ))}
    </div>
  );

  return avatar;
}
