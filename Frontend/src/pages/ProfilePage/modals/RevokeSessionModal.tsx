import { useTranslation } from "react-i18next";
import { Typography, theme, App } from "antd";
import { WarningOutlined } from "@ant-design/icons";
import { AIModal } from "@/components";
import { useRevokeSession } from "@/hooks";

const { Text } = Typography;

interface RevokeSessionModalProps {
  isOpen: boolean;
  deviceId: string;
  onClose: () => void;
}

export function RevokeSessionModal({
  isOpen,
  deviceId,
  onClose,
}: RevokeSessionModalProps) {
  const { t } = useTranslation();
  const { token } = theme.useToken();
  const { message } = App.useApp();
  const revokeSession = useRevokeSession();

  const handleRevoke = () => {
    revokeSession.mutate(deviceId, {
      onSuccess: () => {
        onClose();
        message.success(t("profilePage.sessions.revoke.success"));
      },
    });
  };

  return (
    <AIModal
      title={t("profilePage.sessions.revoke.title")}
      open={isOpen}
      onCancel={onClose}
      onOk={handleRevoke}
      isLoading={revokeSession.isPending}
      footer={[
        { type: "cancel", onClick: onClose },
        {
          type: "delete",
          text: t("profilePage.sessions.revoke.title"),
          onClick: handleRevoke,
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
        <Text>{t("profilePage.sessions.revoke.confirmation")}</Text>
      </div>
    </AIModal>
  );
}
