import { useState } from "react";
import { Flex, Typography, App } from "antd";
import { useTranslation } from "react-i18next";
import { AIModal, UserAvatar, AIRoleTag, AIRoleSelect } from "@/components";
import { useTeam } from "@/hooks";
import type { TeamMemberItem, TeamRole } from "@/types";
import { getTranslatedErrorMessage } from "@/utils";

const { Text } = Typography;

interface EditMemberRoleModalProps {
  isOpen: boolean;
  onClose: () => void;
  teamId: string;
  member: TeamMemberItem;
  currentUserRole: TeamRole;
}

/**
 * Modal for changing a team member's role.
 *
 * Role permission logic:
 * - Admin can promote members to CoAdmin / Leader / Member
 * - CoAdmin can promote members to Leader / Member
 */
export function EditMemberRoleModal({
  isOpen,
  onClose,
  teamId,
  member,
  currentUserRole,
}: EditMemberRoleModalProps) {
  const { t } = useTranslation();
  const { message } = App.useApp();
  const { updateMemberRole } = useTeam();

  const [selectedRole, setSelectedRole] = useState<TeamRole>(member.role);

  const exceptRoles: TeamRole[] =
    currentUserRole === "Admin" ? ["Admin"] : ["Admin", "CoAdmin"];

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
        t("teamDetail.members.updateRole.success", {
          userName: member.userName,
        }),
      );
      onClose();
    } catch (error) {
      message.error(getTranslatedErrorMessage(error));
    }
  };

  return (
    <AIModal
      title={t("teamDetail.members.editMember.title")}
      open={isOpen}
      onOk={handleSave}
      onCancel={onClose}
      isLoading={updateMemberRole.isPending}
      footer={[{ type: "cancel" }, { type: "update" }]}
    >
      <Flex vertical gap={16} style={{ marginTop: 16 }}>
        {/* ── Member info card ── */}
        <Flex align="center" gap={12}>
          <UserAvatar
            userId={member.userId}
            userName={member.userName}
            src={member.avatar}
            size={48}
          />
          <Flex vertical gap={2}>
            <Text strong style={{ fontSize: 15 }}>
              {member.userName}
            </Text>
            <Text type="secondary" style={{ fontSize: 13 }}>
              {member.email}
            </Text>
            <AIRoleTag role={member.role} />
          </Flex>
        </Flex>

        {/* ── Role selector ── */}
        <Flex vertical gap={8}>
          <Text strong>{t("teamDetail.members.updateRole.label")}</Text>
          <AIRoleSelect
            value={selectedRole}
            exceptRoles={exceptRoles}
            onChange={(role) => setSelectedRole(role as TeamRole)}
            style={{ width: "100%" }}
            allowClear={false}
          />
        </Flex>
      </Flex>
    </AIModal>
  );
}
