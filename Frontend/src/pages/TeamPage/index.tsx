import React, { useEffect } from "react";
import { useDebounce, useTeam } from "@/hooks";
import MainLayout from "@/layouts";
import { Button, Card, Flex, Input, Select, Space } from "antd";
import { TeamList } from "./components/TeamList";

export default function TeamPage() {
  const [search, setSearch] = React.useState("");
  const [currentPage, setCurrentPage] = React.useState(1);
  const [pageSize, setPageSize] = React.useState(10);
  const [hasFetched, setHasFetched] = React.useState(false);
  const [hasData, setHasData] = React.useState(false);

  const debouncedSearch = useDebounce(search, 500);

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

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearch(e.target.value);
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
          <Flex gap={24} justify="space-between" wrap>
            {hasFetched && hasData ? (
              <Space>
                <Input
                  placeholder="Search teams..."
                  value={search}
                  onChange={handleSearchChange}
                  style={{ width: 300 }}
                />
                <Select
                  value={pageSize}
                  onChange={handlePageSizeChange}
                  options={[
                    { value: 5, label: "5 nhóm / trang" },
                    { value: 10, label: "10 nhóm / trang" },
                    { value: 20, label: "20 nhóm / trang" },
                  ]}
                />
              </Space>
            ) : null}
            <Button onClick={() => setSearch("")}>Tạo nhóm</Button>
          </Flex>
        </Card>
        <Card>
          <TeamList
            teamsData={data ?? { items: [], total: 0, page: 1, pageSize: 10 }}
            handlePageChange={handlePageChange}
            isLoading={isLoading}
          />
        </Card>
      </Space>
    </MainLayout>
  );
}
