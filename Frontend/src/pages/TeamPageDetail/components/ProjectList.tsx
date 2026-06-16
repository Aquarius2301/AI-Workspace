import type { TeamProjectItem } from "@/types";
import { Empty, Table, Tag } from "antd";
import type { ColumnsType } from "antd/es/table";
import Text from "antd/es/typography/Text";

interface ProjectListProps {
  projects?: TeamProjectItem[];
  isProjectsLoading: boolean;
}

export function ProjectList({ projects, isProjectsLoading }: ProjectListProps) {
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
  ];

  return (
    <Table<TeamProjectItem>
      dataSource={projects ?? []}
      columns={projectColumns}
      rowKey="id"
      loading={isProjectsLoading}
      locale={{ emptyText: <Empty description="Chưa có dự án nào" /> }}
      pagination={false}
    />
  );
}
