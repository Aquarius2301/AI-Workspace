import { useTranslation } from "react-i18next";
import { useTeam } from "@/hooks";
import { Modal, Space, App } from "antd";
import Input from "antd/es/input/Input";
import Text from "antd/es/typography/Text";
import { useState } from "react";
import { useNavigate } from "react-router-dom";

interface DeleteTeamModalProps {
  teamId: string;
  teamName: string;
  isOpen: boolean;
  onClose: () => void;
}

export function DeleteTeamModal({
  teamId,
  teamName,
  isOpen,
  onClose,
}: DeleteTeamModalProps) {
  const { t } = useTranslation();
  const { deleteTeam } = useTeam();
  const [confirmName, setConfirmName] = useState("");
  const navigate = useNavigate();

  const [isConfirmModalOpen, setIsConfirmModalOpen] = useState(false);

  const { message } = App.useApp();

  const isConfirmMatch = confirmName === teamName;

  const resetForm = () => {
    setConfirmName("");
  };

  const handleClose = () => {
    resetForm();
    onClose();
  };

  const handleDelete = async () => {
    if (!isConfirmMatch) return;
    try {
      await deleteTeam.mutateAsync(teamId);
      handleClose();
      message.success(t("team.deleteSuccess"));
      navigate("/teams");
    } catch (error) {
      console.error("Failed to delete team:", error);
      message.error(t("team.deleteError"));
    }
  };

  return (
    <>
      <Modal
        title={t("team.deleteConfirmTitle")}
        open={isOpen}
        onOk={() => {
          setIsConfirmModalOpen(true);
          handleClose();
        }}
        onCancel={handleClose}
        okText={t("team.deleteTeamButton")}
        cancelText={t("team.cancel")}
        okButtonProps={{ danger: true }}
      >
        <Space direction="vertical" size={12} style={{ width: "100%" }}>
          <Text>{t("team.deleteConfirmMessage")}</Text>
          <Text type="danger">{t("team.deleteConfirmWarning")}</Text>
        </Space>
      </Modal>

      <Modal
        title={t("team.deleteConfirmTitle")}
        open={isConfirmModalOpen}
        onCancel={() => setIsConfirmModalOpen(false)}
        onOk={handleDelete}
        okText={t("team.deleteTeamButton")}
        cancelText={t("team.cancel")}
        okButtonProps={{ danger: true, disabled: !isConfirmMatch }}
        confirmLoading={deleteTeam.isPending}
      >
        <div style={{ marginBottom: 16 }}>
          <Text strong>{t("team.deleteConfirmInput", { teamName })}</Text>
        </div>
        <Input
          style={{ marginBottom: 16 }}
          value={confirmName}
          onChange={(e) => setConfirmName(e.target.value)}
        />
        <Text type="danger">{t("team.deleteConfirmDanger")}</Text>
      </Modal>
    </>
  );
}
