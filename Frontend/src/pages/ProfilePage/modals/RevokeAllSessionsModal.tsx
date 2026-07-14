import { useTranslation } from "react-i18next";
import { App, Typography, theme } from "antd";
import { WarningOutlined } from "@ant-design/icons";
import { AIModal } from "@/components";
import { useRevokeAllSessions } from "@/hooks";

const { Text } = Typography;

interface RevokeAllSessionsModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export function RevokeAllSessionsModal({
  isOpen,
  onClose,
}: RevokeAllSessionsModalProps) {
  const { t } = useTranslation();
  const { token } = theme.useToken();
  const { message } = App.useApp();
  const revokeAllSessions = useRevokeAllSessions();

  const handleRevokeAll = () => {
    revokeAllSessions.mutate(undefined, {
      onSuccess: () => {
        onClose();
        message.success(t("profilePage.sessions.revokeAll.success"));
      },
    });
  };

  return (
    <AIModal
      title={t("profilePage.sessions.revokeAll.title")}
      open={isOpen}
      onCancel={onClose}
      onOk={handleRevokeAll}
      isLoading={revokeAllSessions.isPending}
      footer={[
        { type: "cancel", onClick: onClose },
        {
          type: "delete",
          text: t("profilePage.sessions.revokeAll.title"),
          onClick: handleRevokeAll,
        },
      ]}
    >
      <div style={{ padding: "16px 0", textAlign: "center" }}>
        <WarningOutlined
          style={{
            fontSize: 48,
            color: token.colorWarning,
            marginBottom: 16,
          }}
        />
        <br />
        <Text>{t("profilePage.sessions.revokeAll.confirmation")}</Text>
      </div>
    </AIModal>
  );
}
