import { useState } from "react";
import {
  Flex,
  Input,
  Typography,
  Empty,
  Button,
  Grid,
  type MenuProps,
  Dropdown,
  App,
  theme,
} from "antd";
import { WarningOutlined } from "@ant-design/icons";
import { useTranslation } from "react-i18next";
import { useTeamMembers, useSearch, useDeleteMember } from "@/hooks";
import {
  AIList,
  UserAvatar,
  AICardItem,
  AITeamRoleSelect,
  AITeamRoleTag,
  AIModal,
} from "@/components";
import type { TeamMemberItem, TeamRole } from "@/types";
import {
  DeleteOutlined,
  EditOutlined,
  EllipsisOutlined,
} from "@ant-design/icons";

import { formatIsoLocaleDate, getErrorMessage } from "@/utils";
import { EditMemberRoleModal } from "../modals/EditMemberRoleModal";
import { AddMemberModal } from "../modals/AddMemberModal";
const { Text } = Typography;
const { useBreakpoint } = Grid;

interface MembersTabProps {
  teamId: string;
  userId: string;
  role: TeamRole;
  enabled?: boolean;
}

export function MembersTab({ teamId, userId, role, enabled }: MembersTabProps) {
  const { t } = useTranslation();
  const { token } = theme.useToken();
  const { message } = App.useApp();
  const screens = useBreakpoint();
  const isMobile = !screens.md;
  const { searchProps, roleProps, paginationProps, queryParams } = useSearch({
    hasRoleFilter: true,
  });

  const { data: membersData, isLoading } = useTeamMembers(
    teamId,
    queryParams.search,
    queryParams.role,
    queryParams.page,
    queryParams.pageSize,
    enabled,
  );

  const deleteMember = useDeleteMember();

  const hasSearchQuery = !!queryParams.search || !!queryParams.role;
  const isDataEmpty = !isLoading && membersData && membersData.total === 0;
  const showFilters = !(isDataEmpty && !hasSearchQuery);

  const isAdmin = role === "admin";
  const isCoAdmin = role === "coAdmin";

  // ── Edit member role modal state ──
  const [editModal, setEditModal] = useState<{
    isOpened: boolean;
    member: TeamMemberItem | null;
  }>({
    isOpened: false,
    member: null,
  });

  // ── Add member modal state ──
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);

  // ── Delete member confirm state ──
  const [deleteMemberTarget, setDeleteMemberTarget] =
    useState<TeamMemberItem | null>(null);

  const handleDeleteMember = async () => {
    if (!deleteMemberTarget) return;

    try {
      await deleteMember.mutateAsync({
        id: teamId,
        memberId: deleteMemberTarget.userId,
      });
      message.success(
        t("teamDetailPage.members.deleteMember.success", {
          name: deleteMemberTarget.userName,
        }),
      );
      setDeleteMemberTarget(null);
    } catch (error) {
      message.error(getErrorMessage(error));
    }
  };

  const menu = (member: TeamMemberItem): MenuProps["items"] => {
    const items = [
      {
        key: "editRole",
        icon: <EditOutlined />,
        label: t("teamDetailPage.members.editRole.title"),
        onClick: () => setEditModal({ isOpened: true, member }),
      },
      {
        key: "deleteMember",
        icon: <DeleteOutlined />,
        danger: true,
        label: t("teamDetailPage.members.deleteMember.title"),
        onClick: () => setDeleteMemberTarget(member),
      },
    ];

    if (isCoAdmin) items.shift();

    return items;
  };

  return (
    <Flex vertical gap={16}>
      <Flex wrap justify="space-between" gap={8}>
        {/* ── Filter bar: search + role filter + create ── */}
        {showFilters && (
          <Flex gap={12} align="center">
            <Input
              placeholder={t("teamDetailPage.members.searchMembers")}
              allowClear
              value={searchProps.search}
              onChange={(e) => searchProps.onSearchChange(e.target.value)}
              style={{
                maxWidth: isMobile ? "100%" : 360,
                flex: isMobile ? 1 : undefined,
              }}
            />
            <AITeamRoleSelect
              value={roleProps.role}
              onChange={(role) => roleProps.onRoleChange(role)}
              allowClear
            />
          </Flex>
        )}
        {(isAdmin || isCoAdmin) && (
          <Button
            type="primary"
            style={{ maxWidth: isMobile ? "100%" : 150 }}
            onClick={() => setIsAddModalOpen(true)}
          >
            {t("teamDetailPage.members.addMember.title")}
          </Button>
        )}
      </Flex>
      {/* ── Member list ── */}
      <AIList<TeamMemberItem>
        data={membersData?.items ?? []}
        itemKey={(item) => item.userId}
        renderItem={(member) => (
          <AICardItem
            header={
              <Flex justify="space-between">
                <Flex align="center" wrap gap={8}>
                  <UserAvatar
                    userId={member.userId}
                    userName={member.userName}
                    src={member.avatarUrl}
                    size={44}
                    showActive={true}
                    lastActiveAt={member.lastActiveAt}
                  />

                  <Text strong style={{ fontSize: 15 }}>
                    {member.userName}
                  </Text>
                  <Text type="secondary" style={{ fontSize: 13 }}>
                    {member.userId === userId &&
                      t("teamDetailPage.members.you")}
                  </Text>

                  <AITeamRoleTag role={member.role} />
                </Flex>
                {member.userId !== userId && (isAdmin || isCoAdmin) && (
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
                  {t("teamDetailPage.members.email")}: {member.email}
                </Text>
                <Text type="secondary" style={{ fontSize: 13 }}>
                  {t("teamDetailPage.members.joinedAt")}:{" "}
                  {formatIsoLocaleDate(member.joinedAt)}
                </Text>
              </Flex>
            }
          />
        )}
        isLoading={isLoading}
        paginationProps={{
          ...paginationProps,
          total: membersData?.total ?? 0,
        }}
        hasSearchQuery={hasSearchQuery}
        notFound={{
          icon: Empty.PRESENTED_IMAGE_SIMPLE,
          title: t("teamDetailPage.members.notFound.title"),
          subTitle: t("teamDetailPage.members.notFound.description"),
        }}
      />
      {/* ── Edit member role modal ── */}
      {editModal.isOpened && editModal.member && (
        <EditMemberRoleModal
          isOpen
          onClose={() => setEditModal({ isOpened: false, member: null })}
          teamId={teamId}
          member={editModal.member}
          role={role}
        />
      )}
      {/* ── Add member modal ── */}
      {isAddModalOpen && (
        <AddMemberModal
          isOpen
          onClose={() => setIsAddModalOpen(false)}
          teamId={teamId}
          currentUserRole={role}
        />
      )}
      {/* ── Delete member confirm modal ── */}
      <AIModal
        title={t("teamDetailPage.members.deleteMember.title")}
        open={!!deleteMemberTarget}
        onCancel={() => setDeleteMemberTarget(null)}
        onOk={handleDeleteMember}
        isLoading={deleteMember.isPending}
        footer={[
          { type: "cancel" as const },
          {
            type: "delete" as const,
            text: t("teamDetailPage.members.deleteMember.title"),
          },
        ]}
      >
        <Flex vertical align="center" gap={12} style={{ padding: "16px 0" }}>
          <WarningOutlined
            style={{ fontSize: 48, color: token.colorWarning }}
          />
          <Text style={{ textAlign: "center", fontSize: 14 }}>
            {t("teamDetailPage.members.deleteMember.confirmation", {
              name: deleteMemberTarget?.userName,
            })}
          </Text>
        </Flex>
      </AIModal>
    </Flex>
  );
}
