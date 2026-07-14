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
} from "antd";
import { useTranslation } from "react-i18next";
import { useTeamMembers, useSearch } from "@/hooks";
import {
  AIList,
  UserAvatar,
  AICardItem,
  AITeamRoleSelect,
  AITeamRoleTag,
} from "@/components";
import type { TeamMemberItem, TeamRole } from "@/types";
import {
  DeleteOutlined,
  EditOutlined,
  EllipsisOutlined,
} from "@ant-design/icons";

import { formatIsoLocaleDate } from "@/utils";
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

  const menu = (member: TeamMemberItem): MenuProps["items"] => {
    var modals = [
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
      },
    ];

    if (isCoAdmin) modals.shift();

    return modals;
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
        // empty={{   // No empty since a project has at least 1 member
        //   icon: Empty.PRESENTED_IMAGE_SIMPLE,
        //   title: t("teamDetailPage.members.empty"),
        // }}
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
    </Flex>
  );
}
