import { useTranslation } from "react-i18next";
import React from "react";
import { Modal, Button, message, Space } from "antd";
import { ExclamationCircleFilled } from "@ant-design/icons";
import type { TeamRole } from "@/types";
import { useTeam, TEAM_MEMBERS_QUERY_KEY } from "@/hooks";
import { useQueryClient } from "@tanstack/react-query";
import Text from "antd/es/typography/Text";
import { RoleSelect } from "@/components";

interface EditMemberModalProps {
  isOpen: boolean;
  onClose: () => void;
  memberId: string;
  teamId: string;
  userName: string;
  email: string;
  currentRole: TeamRole;
}

export function EditMemberModal({
  isOpen,
  onClose,
  memberId,
  teamId,
  userName,
  email,
  currentRole,
}: EditMemberModalProps) {
  const { t } = useTranslation();
  const queryClient = useQueryClient();
  const { updateMemberRole, removeMember } = useTeam();

  const [role, setRole] = React.useState<TeamRole>(currentRole);
  const [isRemoving, setIsRemoving] = React.useState(false);
  const [isUpdatingRole, setIsUpdatingRole] = React.useState(false);
  const [modal, contextHolder] = Modal.useModal();

  React.useEffect(() => {
    if (isOpen) {
      setRole(currentRole);
    }
  }, [currentRole, isOpen]);

  const invalidateMembers = () => {
    queryClient.invalidateQueries({
      queryKey: [...TEAM_MEMBERS_QUERY_KEY, teamId],
    });
  };

  const handleRoleChange = (value: TeamRole) => {
    setRole(value);
  };

  const executeRemoveMember = async () => {
    setIsRemoving(true);
    try {
      await removeMember.mutateAsync({
        id: teamId,
        memberId: memberId,
      });
      message.success(t("team.removeMemberSuccess"));
      onClose();
      invalidateMembers();
    } catch (error) {
      message.error(t("team.removeMemberError"));
      console.error("Failed to remove member:", error);
    } finally {
      setIsRemoving(false);
    }
  };

  const showConfirmDelete = () => {
    modal.confirm({
      title: t("team.confirmDeleteMember"),
      icon: <ExclamationCircleFilled style={{ color: "#ff4d4f" }} />,
      content: (
        <Space vertical size={8}>
          <Text>
            {t("team.confirmDeleteMemberContent", { userName, email })}
          </Text>
          <Text type="danger">{t("team.confirmDeleteMemberWarning")}</Text>
        </Space>
      ),
      okText: t("team.confirmDeleteOk"),
      okType: "danger",
      cancelText: t("team.cancel"),
      centered: true,
      onOk() {
        executeRemoveMember();
      },
    });
  };

  const handleUpdateRole = async () => {
    if (!role) return;

    setIsUpdatingRole(true);
    try {
      await updateMemberRole.mutateAsync({
        id: teamId,
        memberId: memberId,
        data: { role },
      });
      message.success(t("team.updateRoleSuccess"));
      onClose();
      invalidateMembers();
    } catch (error) {
      message.error(t("team.updateRoleError"));
      console.error("Failed to update member role:", error);
    } finally {
      setIsUpdatingRole(false);
    }
  };

  return (
    <>
      {contextHolder}

      <Modal
        title={t("team.editMemberTitle")}
        open={isOpen}
        onCancel={onClose}
        width={400}
        centered
        destroyOnHidden
        mask={{ closable: false }}
        footer={[
          <Button
            key="remove"
            danger
            onClick={showConfirmDelete}
            disabled={isRemoving || isUpdatingRole}
          >
            {isRemoving
              ? t("team.removeMemberDeleting")
              : t("team.removeMemberButton")}
          </Button>,
          <Button
            key="update"
            type="primary"
            onClick={handleUpdateRole}
            disabled={isUpdatingRole || isRemoving || role === currentRole}
          >
            {isUpdatingRole
              ? t("team.updateRoleUpdating")
              : t("team.updateRoleButton")}
          </Button>,
        ]}
      >
        <Space
          vertical
          size={12}
          style={{ width: "100%", paddingTop: 8, paddingBottom: 8 }}
        >
          <div>
            <Text strong>{t("team.memberNameLabel")}</Text>{" "}
            <Text>{userName}</Text>
          </div>
          <div>
            <Text strong>{t("team.emailLabelColon")}</Text> <Text>{email}</Text>
          </div>
          <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
            <Text strong style={{ minWidth: 60 }}>
              {t("team.roleLabel")}
            </Text>
            <div style={{ flex: 1 }}>
              <RoleSelect
                value={role}
                onChange={(role) => handleRoleChange(role as TeamRole)}
                placeholder={t("team.assignRolePlaceholder")}
                showAll={false}
              />
            </div>
          </div>
        </Space>
      </Modal>
    </>
  );
}
