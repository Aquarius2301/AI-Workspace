import { useState } from "react";
import {
  Button,
  Flex,
  List,
  Tag,
  Typography,
  Empty,
  message,
  Space,
} from "antd";
import {
  LaptopOutlined,
  AppleOutlined,
  AndroidOutlined,
  WindowsOutlined,
  DesktopOutlined,
  LogoutOutlined,
  QuestionCircleOutlined,
} from "@ant-design/icons";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { AICard } from "@/components/ui/AICard";
import { AIModal } from "@/components/ui/AIModal";
import { useSessionsQuery, useAuth } from "@/hooks/api/useAuth.hook";
import type { SessionResult } from "@/types";

const { Text } = Typography;

function getOsIcon(os: string | null) {
  if (!os) return <QuestionCircleOutlined />;
  const lower = os.toLowerCase();
  if (lower.includes("windows")) return <WindowsOutlined />;
  if (lower.includes("macos") || lower.includes("mac"))
    return <AppleOutlined />;
  if (lower.includes("android")) return <AndroidOutlined />;
  if (
    lower.includes("ios") ||
    lower.includes("iphone") ||
    lower.includes("ipad")
  )
    return <AppleOutlined />;
  if (lower.includes("linux")) return <LaptopOutlined />;
  return <DesktopOutlined />;
}

export default function ActiveSessionsCard() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const { data: sessions, isLoading, isError, refetch } = useSessionsQuery();
  const { logout, revokeSession, revokeAllRefresh } = useAuth();

  const [revokingDeviceId, setRevokingDeviceId] = useState<string | null>(null);

  // Revoke single session modal state
  const [revokeModalOpen, setRevokeModalOpen] = useState(false);
  const [targetSession, setTargetSession] = useState<SessionResult | null>(
    null,
  );

  // Revoke all modal state
  const [revokeAllModalOpen, setRevokeAllModalOpen] = useState(false);

  const handleOpenRevokeModal = (session: SessionResult) => {
    if (!session.deviceId) return;
    setTargetSession(session);
    setRevokeModalOpen(true);
  };

  const handleConfirmRevoke = async () => {
    if (!targetSession?.deviceId) return;
    setRevokingDeviceId(targetSession.deviceId);
    try {
      await revokeSession.mutateAsync(targetSession.deviceId);
      setRevokeModalOpen(false);
      setTargetSession(null);
      if (targetSession.isCurrent) {
        await logout.mutateAsync();
        message.success(t("sessions.sessionRevoked"));
        navigate("/login");
      } else {
        message.success(t("sessions.sessionRevoked"));
      }
    } catch {
      message.error(t("sessions.error"));
    } finally {
      setRevokingDeviceId(null);
    }
  };

  const handleOpenRevokeAllModal = () => {
    setRevokeAllModalOpen(true);
  };

  const handleConfirmRevokeAll = async () => {
    try {
      await revokeAllRefresh.mutateAsync();
      setRevokeAllModalOpen(false);
      message.success(t("sessions.sessionRevoked"));
      navigate("/login");
    } catch {
      message.error(t("sessions.error"));
    }
  };

  const cardExtra = sessions && sessions.length > 0 && (
    <Button
      type="text"
      danger
      icon={<LogoutOutlined />}
      size="small"
      onClick={(e) => {
        e.stopPropagation();
        handleOpenRevokeAllModal();
      }}
    >
      {t("sessions.logOutAllDevices")}
    </Button>
  );

  return (
    <AICard
      title={
        <Text strong style={{ fontSize: 15 }}>
          {t("sessions.title")}
        </Text>
      }
      extra={cardExtra}
      isLoading={isLoading}
    >
      {isError ? (
        <Flex vertical align="center" gap={12} style={{ padding: "24px 0" }}>
          <Text type="danger">{t("sessions.error")}</Text>
          <Button onClick={() => refetch()}>{t("sessions.retry")}</Button>
        </Flex>
      ) : !sessions || sessions.length === 0 ? (
        <Empty
          description={t("sessions.noSessions")}
          style={{ padding: "24px 0" }}
        />
      ) : (
        <List
          dataSource={sessions}
          renderItem={(session: SessionResult) => (
            <List.Item
              actions={[
                <Button
                  key="revoke"
                  type="text"
                  danger
                  icon={<LogoutOutlined />}
                  loading={revokingDeviceId === session.deviceId}
                  onClick={() => handleOpenRevokeModal(session)}
                >
                  {t("sessions.logOutDevice")}
                </Button>,
              ]}
            >
              <List.Item.Meta
                avatar={
                  <Flex
                    align="center"
                    justify="center"
                    style={{
                      width: 40,
                      height: 40,
                      borderRadius: 8,
                      background: "rgba(0,0,0,0.04)",
                      fontSize: 22,
                    }}
                  >
                    {getOsIcon(session.os)}
                  </Flex>
                }
                title={
                  <Space size={8}>
                    <Text strong>
                      {session.browser || t("sessions.unknown")}
                    </Text>
                    {session.isCurrent && (
                      <Tag color="blue" style={{ marginInlineEnd: 0 }}>
                        {t("sessions.currentDevice")}
                      </Tag>
                    )}
                  </Space>
                }
                description={
                  <Flex vertical gap={2}>
                    <Text type="secondary" style={{ fontSize: 13 }}>
                      {session.os || t("sessions.unknown")}
                    </Text>
                    <Text type="secondary" style={{ fontSize: 12 }}>
                      {t("sessions.loggedInAt", {
                        date: new Date(session.createdAt).toLocaleString(),
                      })}
                    </Text>
                  </Flex>
                }
              />
            </List.Item>
          )}
        />
      )}

      {/* Revoke single session modal */}
      <AIModal
        open={revokeModalOpen}
        onCancel={() => {
          setRevokeModalOpen(false);
          setTargetSession(null);
        }}
        onOk={handleConfirmRevoke}
        title={t("sessions.confirmRevokeTitle")}
        footer={[
          { type: "cancel" },
          {
            type: "delete",
            text: t("sessions.logOutDevice"),
            onClick: handleConfirmRevoke,
          },
        ]}
      >
        <Text>{t("sessions.confirmRevokeContent")}</Text>
      </AIModal>

      {/* Revoke all sessions modal */}
      <AIModal
        open={revokeAllModalOpen}
        onCancel={() => setRevokeAllModalOpen(false)}
        onOk={handleConfirmRevokeAll}
        title={t("sessions.confirmRevokeTitle")}
        footer={[
          { type: "cancel" },
          {
            type: "delete",
            text: t("sessions.logOutAllDevices"),
            onClick: handleConfirmRevokeAll,
          },
        ]}
      >
        <Text>{t("sessions.confirmRevokeAllContent")}</Text>
      </AIModal>
    </AICard>
  );
}
