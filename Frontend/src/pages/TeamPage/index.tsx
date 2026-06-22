import { useEffect, useMemo, useState } from "react";
import { useSearch, useTeam } from "@/hooks";
import MainLayout from "@/layouts";
import { Button, Card, Space } from "antd";
import { TeamModal } from "./modals";
import Paragraph from "antd/es/typography/Paragraph";
import { useNavigate } from "react-router-dom";
import type { TeamItem } from "@/types";
import { SearchPagination, type CustomColumnsType } from "@/components";

export default function TeamPage() {
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
        title: "Tên nhóm",
        dataIndex: "name",
        key: "name",
      },
      {
        title: "Mô tả",
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
        title: "Hành động",
        noShowMobileTitle: true,
        render: (_, record) => (
          <Button
            style={{ width: "100%" }}
            onClick={() => navigate(`/teams/${record.id}`)}
          >
            View
          </Button>
        ),
      },
    ],
    [navigate],
  );

  return (
    <MainLayout breadcrumbItems={[{ title: "Nhóm của tôi" }]}>
      <Space vertical size={24} style={{ width: "100%" }}>
        <Card>
          <Button
            type="primary"
            onClick={() => setIsModalOpen(true)}
            style={{ marginBottom: "16px" }}
          >
            Tạo nhóm mới
          </Button>
          <SearchPagination
            search={
              hasHadData
                ? { ...searchProps, placeholder: "Tên nhóm..." }
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
                emptyText: "Chưa có nhóm nào",
              },
            }}
          />
        </Card>
      </Space>

      <TeamModal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} />
    </MainLayout>
  );
}
