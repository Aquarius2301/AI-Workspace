import type { TeamProjectItem } from "@/types";
import { Button, Tag } from "antd";
import Text from "antd/es/typography/Text";
import {
  SearchPagination,
  type CustomColumnsType,
  type PaginationProps,
  type SearchProps,
} from "@/components";
import Paragraph from "antd/es/typography/Paragraph";
import { useMemo } from "react";

interface ProjectListProps {
  hasHadData: boolean;
  isLoading: boolean;
  data: TeamProjectItem[];
  searchProps: SearchProps;
  paginationProps: PaginationProps;
}

export function ProjectList({
  hasHadData,
  isLoading,
  data,
  searchProps,
  paginationProps,
}: ProjectListProps) {
  const projectColumns = useMemo<CustomColumnsType<TeamProjectItem>>(
    () => [
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
        title: "Hành động",
        noShowMobileTitle: true,
        render: (_, record) =>
          record.canView && (
            <Button style={{ width: "100%" }} onClick={() => {}}>
              View
            </Button>
          ),
      },
    ],
    [],
  );

  return (
    <SearchPagination
      search={
        hasHadData ? { ...searchProps, placeholder: "Tên dự án..." } : undefined
      }
      pagination={{ ...paginationProps }}
      tableProps={{
        dataSource: data,
        columns: projectColumns,
        rowKey: "id",
        loading: isLoading,
        locale: {
          emptyText: "Chưa có dự án nào",
        },
      }}
    />
  );
}
