import { useState } from "react";
import { Button, Input, Flex, Empty, Typography } from "antd";
import { PlusOutlined, TeamOutlined } from "@ant-design/icons";
import { AIList, AIRoleTag } from "@/components";
import { useSearch, useTeamList } from "@/hooks";
import { useTranslation } from "react-i18next";
import { AppLayout } from "@/layouts";
import type { TeamItem } from "@/types";
import { CreateTeamModal } from "./modals";
import { useNavigate } from "react-router-dom";

const { Text } = Typography;

export default function TeamPage() {
  const { t } = useTranslation();
  const breadcrumbItems = [{ title: t("team.title") }];
  const { searchProps, paginationProps, queryParams } = useSearch({});
  const navigate = useNavigate();

  const [modalOpen, setModalOpen] = useState(false);

  const { data, isLoading } = useTeamList(
    true,
    queryParams.search,
    queryParams.page,
    queryParams.pageSize,
  );

  const hasSearchQuery = !!queryParams.search;
  const isDataEmpty = !isLoading && data && data.total === 0;
  const showSearchBar = !(isDataEmpty && !hasSearchQuery);

  return (
    <AppLayout breadcrumbItems={breadcrumbItems}>
      <Flex vertical gap={16}>
        {/* ── Header: search + create button ── */}
        <Flex align="center" justify="space-between" gap={12} wrap>
          {showSearchBar && (
            <Input.Search
              placeholder={t("team.searchTeams")}
              allowClear
              value={searchProps.search}
              onChange={(e) => searchProps.onSearchChange(e.target.value)}
              onSearch={(val) => searchProps.onSearchChange(val)}
              style={{ maxWidth: 360 }}
            />
          )}

          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={() => setModalOpen(true)}
          >
            {t("team.createTeam")}
          </Button>
        </Flex>

        {/* ── Team list ── */}
        <AIList<TeamItem>
          data={data?.items ?? []}
          itemKey={(item) => item.id}
          renderItem={(team) => ({
            header: (
              <Flex align="center" gap={8}>
                <Text strong style={{ fontSize: 15 }}>
                  {team.name}
                </Text>

                <AIRoleTag role={team.currentUserRole} />
              </Flex>
            ),
            content: (
              <Flex vertical gap={4}>
                {team.description && (
                  <Text type="secondary">{team.description}</Text>
                )}
                <Text type="secondary" style={{ fontSize: 13 }}>
                  <TeamOutlined /> {team.memberCount} {t("team.members")}
                </Text>
              </Flex>
            ),
            rightSide: (
              <Button onClick={() => navigate(`/teams/${team.id}`)}>
                {t("team.view")}
              </Button>
            ),
            isHoverable: true,
          })}
          isLoading={isLoading}
          paginationProps={{
            ...paginationProps,
            total: data?.total ?? 0,
          }}
          notFound={{
            icon: Empty.PRESENTED_IMAGE_SIMPLE,
            title: t("team.notFound.title"),
            subTitle: t("team.notFound.description"),
          }}
          empty={{
            icon: Empty.PRESENTED_IMAGE_SIMPLE,
            title: t("team.empty"),
            subTitle: t("team.emptyDescription"),
          }}
          hasSearchQuery={hasSearchQuery}
        />
      </Flex>

      {/* ── Create Team Modal ── */}
      <CreateTeamModal isOpen={modalOpen} onClose={() => setModalOpen(false)} />
    </AppLayout>
  );
}
