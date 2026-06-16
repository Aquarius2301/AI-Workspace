import React from "react";
import { Space, Input, Select } from "antd";
import { useTeam } from "@/hooks";
import { TeamList } from "./components/TeamList";
import MainLayout from "@/layouts";

export default function TeamPage() {
  const [search, setSearch] = React.useState("");
  const [currentPage, setCurrentPage] = React.useState(1);
  const [pageSize, setPageSize] = React.useState(10);

  const teamsQuery = useTeam().getList({
    myTeams: true,
    search,
    page: currentPage,
    pageSize,
  });

  const { data, isLoading } = teamsQuery;

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
    <MainLayout
      isLoading={isLoading}
      breadcrumbItems={[{ title: "Nhóm của tôi" }]}
    >
      <Space vertical size={24} style={{ width: "100%" }}>
        <Space align="start" style={{ width: "100%" }}>
          <Input
            placeholder="Search teams..."
            value={search}
            onChange={handleSearchChange}
            style={{ width: 300 }}
          />

          <Select
            placeholder="Items per page"
            value={pageSize}
            onChange={handlePageSizeChange}
            options={[5, 10, 20, 50].map((size) => ({
              value: size,
              label: size,
            }))}
          />
        </Space>
        <TeamList
          teamsData={data ?? { items: [], total: 0, page: 1, pageSize: 10 }}
          handlePageChange={handlePageChange}
        />
      </Space>
    </MainLayout>
  );
}
