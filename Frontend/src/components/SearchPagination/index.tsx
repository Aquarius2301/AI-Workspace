import type { PageSize, TeamRole } from "@/types";
import { Col, Input, Row, Space, type TableProps } from "antd";
import { Table } from "../Table";
import { RoleSelect } from "../RoleSelect";
import { useTranslation } from "react-i18next";

export interface RoleProps {
  role: TeamRole | undefined;
  onRoleChange: (value: TeamRole | undefined) => void;
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
  const { t } = useTranslation();
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
            <RoleSelect
              value={role.role}
              onChange={role.onRoleChange}
              placeholder={t("searchPagination.rolePlaceholder")}
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
                sizeChangerRender: ({ size }) => (
                  <span>
                    {size} / {t("searchPagination.page")}
                  </span>
                ),
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
