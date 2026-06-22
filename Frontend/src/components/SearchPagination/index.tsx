import type { PageSize, TeamRole } from "@/types";
import { Col, Input, Row, Select, Space, type TableProps } from "antd";
import { Table } from "../Table";

export interface RoleProps {
  role: TeamRole | null;
  onRoleChange: (value: TeamRole | null) => void;
}

export interface SearchProps {
  search: string;
  placeholder?: string;
  onSearchChange: (value: string) => void;
}

export interface PaginationProps {
  page: number;
  pageSize: number;
  total: number;
  onPageChange: (page: number) => void;
  onPageSizeChange: (pageSize: PageSize) => void;
}

export interface SearchPaginationProps<T> {
  search?: SearchProps;
  role?: RoleProps;
  pagination?: PaginationProps;
  tableProps: TableProps<T>;
}

export function SearchPagination<T>({
  search,
  role,
  pagination,
  tableProps,
}: SearchPaginationProps<T>) {
  return (
    <Space
      vertical
      size={24}
      style={{
        width: "100%",
      }}
    >
      <Row gutter={[16, 16]}>
        {search && (
          <Col xs={24} sm={24} md={16} lg={12} xl={8}>
            <Input
              placeholder={search.placeholder}
              value={search.search}
              allowClear
              onChange={(e) => search.onSearchChange(e.target.value)}
            />
          </Col>
        )}

        {role && (
          <Col xs={24} sm={12} md={8} lg={6} xl={4}>
            <Select<TeamRole | null>
              style={{ width: "100%" }}
              placeholder="Chọn vai trò..."
              options={[
                { value: null, label: "Tất cả vai trò" },
                { value: "Admin", label: "Admin" },
                { value: "Leader", label: "Leader" },
                { value: "Member", label: "Member" },
              ]}
              value={role.role}
              onChange={(e) => role.onRoleChange(e)}
            />
          </Col>
        )}
      </Row>

      <Table
        {...tableProps}
        pagination={
          pagination
            ? {
                showSizeChanger: true,
                current: pagination.page,
                pageSize: pagination.pageSize,
                total: pagination.total,
                hideOnSinglePage: true,
                sizeChangerRender: ({ size }) => <span>{size} / trang</span>,
                onChange: (page, pageSize) => {
                  if (page !== pagination.page) {
                    pagination.onPageChange(page);
                  }
                  if (pageSize !== pagination.pageSize) {
                    pagination.onPageSizeChange(pageSize as PageSize);
                  }
                },
              }
            : false
        }
      />
    </Space>
  );
}
