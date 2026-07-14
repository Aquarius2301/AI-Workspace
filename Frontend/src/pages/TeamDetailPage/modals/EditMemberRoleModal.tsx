import { App, Flex, Typography } from "antd";
import { useTranslation } from "react-i18next";
import {
  AIModal,
  AITeamRoleSelect,
  AITeamRoleTag,
  UserAvatar,
} from "@/components";
import type { TeamMemberItem, TeamRole } from "@/types";
import { useState } from "react";
import { useUpdateMemberRole } from "@/hooks";
import { getErrorMessage } from "@/utils";

const { Text } = Typography;

interface EditMemberRoleModalProps {
  isOpen: boolean;
  onClose: () => void;
  teamId: string;
  member: TeamMemberItem;
  role: TeamRole;
}

export function EditMemberRoleModal({
  isOpen,
  onClose,
  teamId,
  member,
}: EditMemberRoleModalProps) {
  const { t } = useTranslation();

  const { message } = App.useApp();
  const updateMemberRole = useUpdateMemberRole();

  const [selectedRole, setSelectedRole] = useState<TeamRole>(member.role);

  const exceptRoles: TeamRole[] = ["admin"];

  const handleSave = async () => {
    const newRole = selectedRole;
    if (!newRole || newRole === member.role) {
      onClose();
      return;
    }
    try {
      await updateMemberRole.mutateAsync({
        id: teamId,
        memberId: member.userId,
        data: { role: newRole },
      });
      message.success(
        t("teamDetailPage.members.editRole.success", {
          userName: member.userName,
        }),
      );
      onClose();
    } catch (error) {
      message.error(getErrorMessage(error));
    }
  };

  return (
    <AIModal
      title={t("teamDetailPage.members.editRole.title")}
      open={isOpen}
      onCancel={onClose}
      onOk={handleSave}
      footer={[
        { type: "cancel" },
        { type: "update", disabled: member.role == selectedRole },
      ]}
    >
      <Flex vertical gap={16} style={{ marginTop: 16 }}>
        {/* ── Member info card ── */}
        <Flex align="center" gap={12}>
          <UserAvatar
            userId={member.userId}
            userName={member.userName}
            src={member.avatarUrl}
            size={48}
          />
          <Flex vertical gap={2}>
            <Text strong style={{ fontSize: 15 }}>
              {member.userName}
            </Text>
            <Text type="secondary" style={{ fontSize: 13 }}>
              {member.email}
            </Text>
            <AITeamRoleTag role={member.role} />
          </Flex>
        </Flex>

        {/* ── Role selector ── */}
        <Flex vertical gap={8}>
          <Text strong>{t("teamDetailPage.members.editRole.searchRole")}</Text>
          <AITeamRoleSelect
            value={selectedRole}
            exceptRoles={exceptRoles}
            onChange={(role) => setSelectedRole(role as TeamRole)}
            style={{ width: "100%" }}
          />
        </Flex>
      </Flex>
    </AIModal>
  );
}
