import { useEffect, useState } from "react";
import { useDebounce, useTeam } from "@/hooks";
import MainLayout from "@/layouts";
import { Button, Card, Empty, Space, Table } from "antd";
import { SearchPagination } from "@/components";
import { EyeOutlined } from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import { TeamModal } from "./modals";
import Paragraph from "antd/es/typography/Paragraph";

export default function TeamPage() {
  const [search, setSearch] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [hasFetched, setHasFetched] = useState(false);
  const [hasData, setHasData] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);

  const debouncedSearch = useDebounce(search, 500);
  const navigate = useNavigate();

  const teamsQuery = useTeam().getList({
    myTeams: true,
    search: debouncedSearch,
    page: currentPage,
    pageSize,
  });

  const { data, isLoading } = teamsQuery;

  // Set flags after first fetch completes
  useEffect(() => {
    if (!isLoading && !hasFetched) {
      setHasFetched(true);
      if (data && data.items.length > 0) {
        setHasData(true);
      }
    }
  }, [isLoading, hasFetched, data]);

  const handleSearchChange = (value: string) => {
    setSearch(value);
    setCurrentPage(1); // Reset to first page when search changes
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  const handlePageSizeChange = (size: number) => {
    setPageSize(size);
    setCurrentPage(1); // Reset to first page when page size changes
  };

  return (
    <MainLayout breadcrumbItems={[{ title: "Nhóm của tôi" }]}>
      <Space vertical style={{ width: "100%" }} size={24}>
        <Card style={{ width: "100%" }}>
          <Button
            style={{ marginBottom: 16 }}
            type="primary"
            onClick={() => setIsModalOpen(true)}
          >
            Tạo nhóm
          </Button>
          <SearchPagination
            search={
              hasFetched && hasData
                ? {
                    searchText: search,
                    onSearchChange: handleSearchChange,
                  }
                : undefined
            }
            pagination={{
              page: currentPage,
              pageSize: pageSize,
              total: data?.total ?? 0,
              onPageChange: handlePageChange,
              onPageSizeChange: handlePageSizeChange,
            }}
          >
            <Table
              dataSource={data?.items ?? []}
              rowKey={"id"}
              loading={isLoading}
              locale={{
                emptyText: <Empty description="Không tìm thấy nhóm nào" />,
              }}
              columns={[
                {
                  title: "Name",
                  dataIndex: "name",
                  key: "name",
                },
                {
                  title: "Description",
                  dataIndex: "description",
                  key: "description",
                  render: (description: string) => (
                    <Paragraph ellipsis={{ rows: 2, expandable: true }}>
                      {description}
                    </Paragraph>
                  ),
                },
                {
                  title: "View",

                  render: (_, record) => (
                    <Button onClick={() => navigate(`/teams/${record.id}`)}>
                      <EyeOutlined />
                    </Button>
                  ),
                },
              ]}
              pagination={false}
            />
          </SearchPagination>
        </Card>
      </Space>

      <TeamModal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} />
    </MainLayout>
  );
}
