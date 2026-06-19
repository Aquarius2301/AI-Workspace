import type { TeamProjectItem } from "@/types";
import { Button, Empty, Table, Tag } from "antd";
import type { ColumnsType } from "antd/es/table";
import Text from "antd/es/typography/Text";
import { EyeOutlined } from "@ant-design/icons";
import { SearchPagination } from "@/components";
import { useDebounce, useProject } from "@/hooks";
import { useState } from "react";

interface ProjectListProps {
  teamId: string;
}

export function ProjectList({ teamId }: ProjectListProps) {
  const projectColumns: ColumnsType<TeamProjectItem> = [
    {
      title: "Tên dự án",
      dataIndex: "name",
      key: "name",
      render: (name: string) => <Text strong>{name}</Text>,
    },
    {
      title: "Mô tả",
      dataIndex: "description",
      key: "description",
      render: (description?: string) =>
        description || <Text type="secondary">—</Text>,
    },
    {
      title: "Hiển thị",
      dataIndex: "visibility",
      key: "visibility",
      render: (visibility: string) => (
        <Tag color={visibility === "Public" ? "blue" : "default"}>
          {visibility === "Public" ? "Công khai" : "Riêng tư"}
        </Tag>
      ),
    },
    {
      title: "Xem",
      render: (_, record) =>
        record.canView && (
          <Button>
            <EyeOutlined />
          </Button>
        ),
    },
  ];

  const [searchText, setSearchText] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  const { getByTeam } = useProject();

  const debounceText = useDebounce(searchText, 500);

  const { data, isLoading } = getByTeam(teamId, debounceText, page, pageSize);

  return (
    <SearchPagination
      search={{
        searchText: searchText,
        onSearchChange: (value) => {
          setSearchText(value);
          setPage(1);
        },
      }}
      pagination={{
        page: page,
        pageSize: pageSize,
        total: data?.total ?? 0,
        onPageChange: (page) => setPage(page),
        onPageSizeChange: (pageSize) => {
          setPageSize(pageSize);
          setPage(1);
        },
      }}
    >
      <Table<TeamProjectItem>
        dataSource={data?.items ?? []}
        columns={projectColumns}
        rowKey="id"
        loading={isLoading}
        locale={{ emptyText: <Empty description="Chưa có dự án nào" /> }}
        pagination={false}
      />
    </SearchPagination>
  );
}
