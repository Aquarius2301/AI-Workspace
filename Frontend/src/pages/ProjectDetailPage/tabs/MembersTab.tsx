import { useState } from "react";
import {
  Flex,
  Input,
  Typography,
  Empty,
  Button,
  Dropdown,
  App,
  theme,
  type MenuProps,
} from "antd";
import {
  WarningOutlined,
  EllipsisOutlined,
  EditOutlined,
  DeleteOutlined,
} from "@ant-design/icons";
import { useTranslation } from "react-i18next";
import { useProjectMembers, useSearch, useRemoveProjectMember } from "@/hooks";
import {
  AIList,
  UserAvatar,
  AICardItem,
  AIProjectRoleSelect,
  AIProjectRoleTag,
  AIModal,
} from "@/components";
import type { ProjectMemberItem } from "@/types";
import { formatIsoLocaleDate, getErrorMessage } from "@/utils";
import { AddProjectMemberModal } from "../modals/AddProjectMemberModal";
import { EditProjectMemberRoleModal } from "../modals/EditProjectMemberRoleModal";

const { Text } = Typography;
const { useToken } = theme;

interface MembersTabProps {
  projectId: string;
  canAddMember?: boolean;
  creatorId?: string;
}

export function MembersTab({
  projectId,
  canAddMember,
  creatorId,
}: MembersTabProps) {
  const { t } = useTranslation();
  const { token } = useToken();
  const { message } = App.useApp();
  const { searchProps, projectRoleProps, paginationProps, queryParams } =
    useSearch({
      hasProjectRoleFilter: true,
    });

  const { data: membersData, isLoading } = useProjectMembers(
    projectId,
    queryParams.search,
    queryParams.projectRole,
    queryParams.page,
    queryParams.pageSize,
  );

  const removeMember = useRemoveProjectMember();

  const [isAddModalOpen, setIsAddModalOpen] = useState(false);

  // ── Edit member role modal state ──
  const [editModal, setEditModal] = useState<{
    isOpened: boolean;
    member: ProjectMemberItem | null;
  }>({
    isOpened: false,
    member: null,
  });

  // ── Delete member confirm state ──
  const [deleteMemberTarget, setDeleteMemberTarget] =
    useState<ProjectMemberItem | null>(null);

  const hasSearchQuery = !!queryParams.search || !!queryParams.projectRole;
  const isDataEmpty = !isLoading && membersData && membersData.total === 0;
  const showFilters = !(isDataEmpty && !hasSearchQuery);

  const handleDeleteMember = async () => {
    if (!deleteMemberTarget) return;

    try {
      await removeMember.mutateAsync({
        projectId,
        memberId: deleteMemberTarget.userId,
      });
      message.success(
        t("projectDetailPage.members.deleteMember.success", {
          name: deleteMemberTarget.userName,
        }),
      );
      setDeleteMemberTarget(null);
    } catch (error) {
      message.error(getErrorMessage(error));
    }
  };

  const menu = (member: ProjectMemberItem): MenuProps["items"] => {
    const isCreator = member.userId === creatorId;
    if (isCreator) return [];

    return [
      {
        key: "editRole",
        icon: <EditOutlined />,
        label: t("projectDetailPage.members.editRole.title"),
        onClick: () => setEditModal({ isOpened: true, member }),
      },
      {
        key: "deleteMember",
        icon: <DeleteOutlined />,
        danger: true,
        label: t("projectDetailPage.members.deleteMember.title"),
        onClick: () => setDeleteMemberTarget(member),
      },
    ];
  };

  return (
    <Flex vertical gap={16}>
      {/* ── Filter bar ── */}
      <Flex wrap justify="space-between" gap={8}>
        {showFilters && (
          <Flex gap={12} align="center" wrap>
            <Input
              placeholder={t("projectDetailPage.members.searchMembers")}
              allowClear
              value={searchProps.search}
              onChange={(e) => searchProps.onSearchChange(e.target.value)}
              style={{ maxWidth: 360 }}
            />
            <AIProjectRoleSelect
              value={projectRoleProps.projectRole}
              onChange={(value) => projectRoleProps.onProjectRoleChange(value)}
              allowClear
            />
          </Flex>
        )}
        {canAddMember && (
          <Button type="primary" onClick={() => setIsAddModalOpen(true)}>
            {t("projectDetailPage.members.addMember.title")}
          </Button>
        )}
      </Flex>

      {/* ── Member list ── */}
      <AIList<ProjectMemberItem>
        data={membersData?.items ?? []}
        itemKey={(item) => item.userId}
        renderItem={(member) => {
          const isCreator = member.userId === creatorId;
          return (
            <AICardItem
              header={
                <Flex justify="space-between" align="center">
                  <Flex align="center" wrap gap={8}>
                    <UserAvatar
                      userId={member.userId}
                      userName={member.userName}
                      size={44}
                    />
                    <Text strong style={{ fontSize: 15 }}>
                      {member.userName}
                    </Text>
                    <AIProjectRoleTag role={member.role} />
                  </Flex>
                  {!isCreator && canAddMember && (
                    <Dropdown menu={{ items: menu(member) }}>
                      <Button type="text">
                        <EllipsisOutlined />
                      </Button>
                    </Dropdown>
                  )}
                </Flex>
              }
              content={
                <Flex vertical gap={2}>
                  <Text type="secondary" style={{ fontSize: 13 }}>
                    {t("projectDetailPage.members.email")}: {member.userEmail}
                  </Text>
                  <Text type="secondary" style={{ fontSize: 13 }}>
                    {t("projectDetailPage.members.joinedAt")}:{" "}
                    {formatIsoLocaleDate(member.joinedAt)}
                  </Text>
                </Flex>
              }
            />
          );
        }}
        isLoading={isLoading}
        paginationProps={{
          ...paginationProps,
          total: membersData?.total ?? 0,
        }}
        hasSearchQuery={hasSearchQuery}
        empty={{
          icon: Empty.PRESENTED_IMAGE_SIMPLE,
          title: t("projectDetailPage.members.empty.title"),
          subTitle: t("projectDetailPage.members.empty.description"),
        }}
        notFound={{
          icon: Empty.PRESENTED_IMAGE_SIMPLE,
          title: t("projectDetailPage.members.notFound.title"),
          subTitle: t("projectDetailPage.members.notFound.description"),
        }}
      />

      {/* ── Add member modal ── */}
      {isAddModalOpen && (
        <AddProjectMemberModal
          isOpen
          onClose={() => setIsAddModalOpen(false)}
          projectId={projectId}
          creatorId={creatorId!}
        />
      )}

      {/* ── Edit member role modal ── */}
      {editModal.isOpened && editModal.member && (
        <EditProjectMemberRoleModal
          isOpen
          onClose={() => setEditModal({ isOpened: false, member: null })}
          projectId={projectId}
          member={editModal.member}
          creatorId={creatorId!}
        />
      )}

      {/* ── Delete member confirm modal ── */}
      <AIModal
        title={t("projectDetailPage.members.deleteMember.title")}
        open={!!deleteMemberTarget}
        onCancel={() => setDeleteMemberTarget(null)}
        onOk={handleDeleteMember}
        isLoading={removeMember.isPending}
        footer={[
          { type: "cancel" as const },
          {
            type: "delete" as const,
            text: t("projectDetailPage.members.deleteMember.title"),
          },
        ]}
      >
        <Flex vertical align="center" gap={12} style={{ padding: "16px 0" }}>
          <WarningOutlined
            style={{ fontSize: 48, color: token.colorWarning }}
          />
          <Text style={{ textAlign: "center", fontSize: 14 }}>
            {t("projectDetailPage.members.deleteMember.confirmation", {
              name: deleteMemberTarget?.userName,
            })}
          </Text>
        </Flex>
      </AIModal>
    </Flex>
  );
}
