import { useState } from "react";
import { Flex, Input, Typography, App, theme } from "antd";
import { WarningOutlined } from "@ant-design/icons";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { AIModal } from "@/components";
import { useDeleteTeam } from "@/hooks";
import { ROUTE } from "@/constants";
import { getErrorMessage } from "@/utils";

const { Text } = Typography;

type DeleteStep = "confirm" | "verify";

interface DeleteTeamModalProps {
  isOpen: boolean;
  onClose: () => void;
  teamId: string;
  teamName: string;
}

export function DeleteTeamModal({
  isOpen,
  onClose,
  teamId,
  teamName,
}: DeleteTeamModalProps) {
  const { t } = useTranslation();
  const { token } = theme.useToken();
  const { message } = App.useApp();
  const navigate = useNavigate();
  const deleteTeam = useDeleteTeam();

  const [step, setStep] = useState<DeleteStep>("confirm");
  const [inputValue, setInputValue] = useState("");

  const isNameMatched = inputValue.trim() === teamName;

  const handleClose = () => {
    setStep("confirm");
    setInputValue("");
    onClose();
  };

  // ── Step 1: Confirm warning ──
  const handleConfirmContinue = () => {
    setStep("verify");
  };

  // ── Step 2: Verify and delete ──
  const handleDelete = async () => {
    if (!isNameMatched) return;

    try {
      await deleteTeam.mutateAsync(teamId);
      message.success(t("teamDetailPage.deleteTeam.success"));
      handleClose();
      navigate(ROUTE.TEAM);
    } catch (error) {
      message.error(getErrorMessage(error));
    }
  };

  return (
    <AIModal
      title={
        step === "confirm"
          ? t("teamDetailPage.deleteTeam.confirmTitle")
          : t("teamDetailPage.deleteTeam.verifyTitle")
      }
      open={isOpen}
      onCancel={handleClose}
      onOk={step === "confirm" ? handleConfirmContinue : handleDelete}
      isLoading={deleteTeam.isPending}
      footer={
        step === "confirm"
          ? [
              { type: "cancel" as const },
              {
                type: "delete" as const,
                text: t("teamDetailPage.deleteTeam.confirmButton"),
              },
            ]
          : [
              { type: "cancel" as const },
              {
                type: "delete" as const,
                text: t("modal.delete"),
                disabled: !isNameMatched,
              },
            ]
      }
    >
      {step === "confirm" ? (
        <Flex vertical align="center" gap={12} style={{ padding: "16px 0" }}>
          <WarningOutlined
            style={{ fontSize: 48, color: token.colorWarning }}
          />
          <Text style={{ textAlign: "center", fontSize: 14 }}>
            {t("teamDetailPage.deleteTeam.confirmWarning")}
          </Text>
        </Flex>
      ) : (
        <Flex vertical gap={8} style={{ marginTop: 16 }}>
          <Text>
            {t("teamDetailPage.deleteTeam.verifyLabel", { teamName })}
          </Text>
          <Input
            placeholder={t("teamDetailPage.deleteTeam.verifyPlaceholder")}
            value={inputValue}
            onChange={(e) => setInputValue(e.target.value)}
            status={inputValue && !isNameMatched ? "error" : undefined}
            autoFocus
          />
          {inputValue && !isNameMatched && (
            <Text type="danger" style={{ fontSize: 13 }}>
              {t("teamDetailPage.deleteTeam.verifyError")}
            </Text>
          )}
        </Flex>
      )}
    </AIModal>
  );
}
