import { useEffect, useMemo, useState } from "react";
import { useTranslation } from "react-i18next";
import { useSearch, useTeam } from "@/hooks";
import MainLayout from "@/layouts";
import { Button, Card, Space } from "antd";
import { TeamModal } from "./modals";
import Paragraph from "antd/es/typography/Paragraph";
import { useNavigate } from "react-router-dom";
import type { TeamItem } from "@/types";
import { SearchPagination, type CustomColumnsType } from "@/components";

export default function TeamPage() {
  const { t } = useTranslation();
  const { searchProps, paginationProps, queryParams } = useSearch({});

  const [isModalOpen, setIsModalOpen] = useState(false);

  const [hasHadData, setHasHadData] = useState(false);

  const teamsQuery = useTeam().getList({
    myTeams: true,
    ...queryParams,
  });

  const { data, isLoading } = teamsQuery;

  const navigate = useNavigate();

  useEffect(() => {
    if (!isLoading && data && data.total > 0 && !hasHadData) {
      setHasHadData(true);
    }
  }, [data, isLoading, hasHadData]);

  const columns = useMemo<CustomColumnsType<TeamItem>>(
    () => [
      {
        title: t("team.name"),
        dataIndex: "name",
        key: "name",
      },
      {
        title: t("team.description"),
        dataIndex: "description",
        key: "description",
        render: (description: string) => (
          <Paragraph
            ellipsis={{ rows: 2, expandable: true }}
            style={{ margin: 0 }}
          >
            {description}
          </Paragraph>
        ),
      },
      {
        title: t("team.actions"),
        noShowMobileTitle: true,
        render: (_, record) => (
          <Button
            style={{ width: "100%" }}
            onClick={() => navigate(`/teams/${record.id}`)}
          >
            {t("team.view")}
          </Button>
        ),
      },
    ],
    [navigate, t],
  );

  return (
    <MainLayout breadcrumbItems={[{ title: t("team.myTeams") }]}>
      <Space vertical size={24} style={{ width: "100%" }}>
        <Card>
          <Button
            type="primary"
            onClick={() => setIsModalOpen(true)}
            style={{ marginBottom: "16px" }}
          >
            {t("team.createNew")}
          </Button>
          <SearchPagination
            search={
              hasHadData
                ? { ...searchProps, placeholder: t("team.searchPlaceholder") }
                : undefined
            }
            pagination={{
              ...paginationProps,
              total: data?.total ?? 0,
            }}
            tableProps={{
              dataSource: data?.items ?? [],
              loading: isLoading,
              rowKey: "id",
              columns: columns,
              locale: {
                emptyText: t("team.noTeams"),
              },
            }}
          />
        </Card>
      </Space>

      <TeamModal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} />
    </MainLayout>
  );
}
