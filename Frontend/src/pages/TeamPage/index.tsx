import { useState } from "react";
import { Button, Input, Flex, Empty, Typography } from "antd";
import { PlusOutlined, TeamOutlined } from "@ant-design/icons";
import { AICardItem, AIList, AITeamRoleTag } from "@/components";
import { useSearch, useTeamList } from "@/hooks";
import { useTranslation } from "react-i18next";
import { AppLayout } from "@/layouts";
import type { TeamItem } from "@/types";
import { useNavigate } from "react-router-dom";
import { CreateTeamModal } from "./modals";

const { Text } = Typography;

export default function TeamPage() {
  const { t } = useTranslation();
  const breadcrumbItems = [{ title: t("teamPage.title") }];
  const { searchProps, paginationProps, queryParams } = useSearch({});
  const navigate = useNavigate();

  const [modalOpen, setModalOpen] = useState(false);

  const { data, isLoading } = useTeamList(
    queryParams.search,
    queryParams.page,
    queryParams.pageSize,
  );

  const hasSearchQuery = !!queryParams.search;
  const isDataEmpty = !isLoading && data && data.total === 0;
  const showSearchBar = !(isDataEmpty && !hasSearchQuery);

  return (
    <AppLayout breadcrumbItems={breadcrumbItems} title={t("teamPage.title")}>
      <Flex vertical gap={16}>
        {/* ── Header: search + create button ── */}
        <Flex align="center" justify="space-between" gap={12} wrap>
          {showSearchBar && (
            <Input
              placeholder={t("teamPage.searchTeams")}
              allowClear
              value={searchProps.search}
              onChange={(e) => searchProps.onSearchChange(e.target.value)}
              style={{ maxWidth: 360 }}
            />
          )}

          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={() => setModalOpen(true)}
          >
            {t("teamPage.createTeam.title")}
          </Button>
        </Flex>

        {/* ── Team list ── */}
        <AIList<TeamItem>
          data={data?.items ?? []}
          itemKey={(item) => item.id}
          renderItem={(team) => (
            <AICardItem
              header={
                <Flex align="center" gap={8}>
                  <Text strong style={{ fontSize: 15 }}>
                    {team.name}
                  </Text>

                  <AITeamRoleTag role={team.currentUserRole} />
                </Flex>
              }
              content={
                team.description && (
                  <Text type="secondary">{team.description}</Text>
                )
              }
              footer={
                <Flex justify="space-between">
                  <Text type="secondary" style={{ fontSize: 13 }}>
                    <TeamOutlined /> {team.memberCount} {t("teamPage.members")}
                  </Text>
                  <Text type="secondary" style={{ fontSize: 13 }}>
                    {t("teamPage.view")}
                  </Text>
                </Flex>
              }
              isHoverable
              onClick={() => navigate(`/teams/${team.slug}`)}
            />
          )}
          isLoading={isLoading}
          paginationProps={{
            ...paginationProps,
            total: data?.total ?? 0,
          }}
          notFound={{
            icon: Empty.PRESENTED_IMAGE_SIMPLE,
            title: t("teamPage.notFound.title"),
            subTitle: t("teamPage.notFound.description"),
          }}
          empty={{
            icon: Empty.PRESENTED_IMAGE_SIMPLE,
            title: t("teamPage.empty.title"),
            subTitle: t("teamPage.empty.description"),
          }}
          hasSearchQuery={hasSearchQuery}
        />
      </Flex>

      {/* ── Create Team Modal ── */}
      <CreateTeamModal isOpen={modalOpen} onClose={() => setModalOpen(false)} />
    </AppLayout>
  );
}
