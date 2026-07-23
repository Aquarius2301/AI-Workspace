import { useState } from "react";
import { Flex, Input, Typography, Empty, Button } from "antd";
import { useTranslation } from "react-i18next";
import { useProjectMembers, useSearch } from "@/hooks";
import {
  AIList,
  UserAvatar,
  AICardItem,
  AIProjectRoleSelect,
  AIProjectRoleTag,
} from "@/components";
import type { ProjectMemberItem } from "@/types";
import { formatIsoLocaleDate } from "@/utils";
import { AddProjectMemberModal } from "../modals/AddProjectMemberModal";

const { Text } = Typography;

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

  const [isAddModalOpen, setIsAddModalOpen] = useState(false);

  const hasSearchQuery = !!queryParams.search || !!queryParams.projectRole;
  const isDataEmpty = !isLoading && membersData && membersData.total === 0;
  const showFilters = !(isDataEmpty && !hasSearchQuery);

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
        renderItem={(member) => (
          <AICardItem
            header={
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
        )}
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
    </Flex>
  );
}
