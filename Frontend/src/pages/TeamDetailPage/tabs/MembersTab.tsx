import { useState } from "react";
import { Flex, Input, Typography, Empty, Button, Grid } from "antd";
import { useTranslation } from "react-i18next";
import { useTeamMembers, useSearch } from "@/hooks";
import { AIList, UserAvatar, AIRoleTag, AIRoleSelect } from "@/components";
import type { TeamMemberItem, TeamRole } from "@/types";
import { EditMemberRoleModal } from "../modals/EditMemberRoleModal";
const { Text } = Typography;
const { useBreakpoint } = Grid;

interface MembersTabProps {
  teamId: string;
  userId: string;
  role: TeamRole;
}

export function MembersTab({ teamId, userId, role }: MembersTabProps) {
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
  );

  const hasSearchQuery = !!queryParams.search || !!queryParams.role;
  const isDataEmpty = !isLoading && membersData && membersData.total === 0;
  const showFilters = !(isDataEmpty && !hasSearchQuery);

  const isAdmin = role === "Admin";
  const isCoAdmin = role === "CoAdmin";

  // ── Edit member role modal state ──
  const [editModal, setEditModal] = useState<{
    isOpened: boolean;
    member: TeamMemberItem | null;
  }>({
    isOpened: false,
    member: null,
  });

  return (
    <Flex vertical gap={16}>
      {/* ── Filter bar: search + role filter ── */}
      {showFilters && (
        <Flex wrap="wrap" gap={12} align="center">
          <Input.Search
            placeholder={t("teamDetail.members.searchMembers")}
            allowClear
            value={searchProps.search}
            onChange={(e) => searchProps.onSearchChange(e.target.value)}
            onSearch={(val) => searchProps.onSearchChange(val)}
            style={{ maxWidth: isMobile ? "100%" : 280, flex: isMobile ? 1 : undefined }}
          />
          <AIRoleSelect
            value={roleProps.role}
            onChange={(role) => roleProps.onRoleChange(role)}
          />
        </Flex>
      )}

      {/* ── Member list ── */}
      <AIList<TeamMemberItem>
        data={membersData?.items ?? []}
        itemKey={(item) => item.userId}
        renderItem={(member) => ({
          header: (
            <Flex align="center" gap={8}>
              <UserAvatar
                userId={member.userId}
                userName={member.userName}
                src={member.avatar}
                size={44}
                showActive={true}
                lastActiveAt={member.lastActiveAt}
              />
              <Text strong style={{ fontSize: 15 }}>
                {member.userName}{" "}
              </Text>
              <Text type="secondary" style={{ fontSize: 13 }}>
                {member.userId === userId && t("teamDetail.members.you")}
              </Text>
              <AIRoleTag role={member.role} />
            </Flex>
          ),
          content: (
            <Flex vertical gap={2}>
              <Text type="secondary" style={{ fontSize: 13 }}>
                {t("teamDetail.members.email")} {member.email}
              </Text>
              <Text type="secondary" style={{ fontSize: 13 }}>
                {t("teamDetail.members.joinedAt")}:{" "}
                {new Date(member.joinedAt).toLocaleDateString()}
              </Text>
            </Flex>
          ),
          rightSide: member.userId !== userId &&
            (isAdmin ||
              (isCoAdmin &&
                member.role != "Admin" &&
                member.role != "CoAdmin")) && (
              <Button onClick={() => setEditModal({ isOpened: true, member })}>
                {t("teamDetail.members.edit")}
              </Button>
            ),
        })}
        isLoading={isLoading}
        paginationProps={{
          ...paginationProps,
          total: membersData?.total ?? 0,
        }}
        hasSearchQuery={hasSearchQuery}
        empty={{
          icon: Empty.PRESENTED_IMAGE_SIMPLE,
          title: t("teamDetail.members.empty"),
        }}
        notFound={{
          icon: Empty.PRESENTED_IMAGE_SIMPLE,
          title: t("teamDetail.members.notFound"),
        }}
      />

      {/* ── Edit member role modal ── */}
      {editModal.isOpened && editModal.member && (
        <EditMemberRoleModal
          isOpen
          onClose={() => setEditModal({ isOpened: false, member: null })}
          teamId={teamId}
          member={editModal.member}
          currentUserRole={role}
        />
      )}
    </Flex>
  );
}
