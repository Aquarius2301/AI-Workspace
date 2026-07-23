import { App, Flex, Typography } from "antd";
import { useTranslation } from "react-i18next";
import {
  AIModal,
  AIProjectRoleSelect,
  AIProjectRoleTag,
  UserAvatar,
} from "@/components";
import type { ProjectMemberItem, ProjectRole } from "@/types";
import { useState } from "react";
import { useUpdateProjectMemberRole } from "@/hooks";
import { getErrorMessage } from "@/utils";

const { Text } = Typography;

interface EditProjectMemberRoleModalProps {
  isOpen: boolean;
  onClose: () => void;
  projectId: string;
  member: ProjectMemberItem;
  creatorId: string;
}

export function EditProjectMemberRoleModal({
  isOpen,
  onClose,
  projectId,
  member,
  creatorId,
}: EditProjectMemberRoleModalProps) {
  const { t } = useTranslation();

  const { message } = App.useApp();
  const updateMemberRole = useUpdateProjectMemberRole();

  const [selectedRole, setSelectedRole] = useState<ProjectRole>(
    member.role as ProjectRole,
  );

  const isCreator = member.userId === creatorId;

  const handleSave = async () => {
    const newRole = selectedRole;
    if (!newRole || newRole === (member.role as ProjectRole)) {
      onClose();
      return;
    }
    try {
      await updateMemberRole.mutateAsync({
        projectId,
        memberId: member.userId,
        data: { role: newRole },
      });
      message.success(t("projectDetailPage.members.editRole.success"));
      onClose();
    } catch (error) {
      message.error(getErrorMessage(error));
    }
  };

  return (
    <AIModal
      title={t("projectDetailPage.members.editRole.title")}
      open={isOpen}
      onCancel={onClose}
      onOk={handleSave}
      footer={[
        { type: "cancel" },
        { type: "update", disabled: member.role === selectedRole },
      ]}
    >
      <Flex vertical gap={16} style={{ marginTop: 16 }}>
        {/* ── Member info card ── */}
        <Flex align="center" gap={12}>
          <UserAvatar
            userId={member.userId}
            userName={member.userName}
            size={48}
          />
          <Flex vertical gap={2}>
            <Text strong style={{ fontSize: 15 }}>
              {member.userName}
            </Text>
            <Text type="secondary" style={{ fontSize: 13 }}>
              {member.userEmail}
            </Text>
            <AIProjectRoleTag role={member.role} />
          </Flex>
        </Flex>

        {/* ── Role selector ── */}
        {isCreator ? (
          <Text type="warning" style={{ fontStyle: "italic" }}>
            {t("projectDetailPage.members.editRole.creatorNote")}
          </Text>
        ) : (
          <Flex vertical gap={8}>
            <Text strong>
              {t("projectDetailPage.members.editRole.searchRole")}
            </Text>
            <AIProjectRoleSelect
              value={selectedRole}
              onChange={(role) => setSelectedRole(role as ProjectRole)}
              style={{ width: "100%" }}
              allowClear={false}
            />
          </Flex>
        )}
      </Flex>
    </AIModal>
  );
}
