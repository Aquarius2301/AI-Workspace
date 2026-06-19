import { Input, Pagination, Row, Select, Space } from "antd";

interface SearchProps {
  searchText: string;
  onSearchChange: (value: string) => void;
}

interface PaginationProps {
  page: number;
  pageSize: number;
  total: number;
  onPageChange: (page: number) => void;
  onPageSizeChange: (pageSize: number) => void;
}

interface SearchPaginationProps {
  search?: SearchProps;

  pagination?: PaginationProps;

  children?: React.ReactNode;
}

export function SearchPagination({
  search,
  pagination,
  children,
}: SearchPaginationProps) {
  // const [searchValue, setSearchValue] = useState(search?.searchText || "");

  return (
    <Space
      vertical
      size={24}
      style={{
        width: "100%",
      }}
    >
      {search && (
        <Input
          placeholder="Tìm kiếm..."
          value={search.searchText}
          allowClear
          onChange={(e) => search.onSearchChange(e.target.value)}
        />
      )}

      {children}

      {pagination && pagination.total > 0 && (
        <Row>
          <Pagination
            current={pagination.page}
            pageSize={pagination.pageSize}
            total={pagination.total}
            onChange={(page) => {
              pagination.onPageChange?.(page);
            }}
            hideOnSinglePage
          />
          <Select
            value={pagination.pageSize}
            onChange={(value: number) => {
              pagination.onPageSizeChange?.(value);
            }}
            options={[
              { value: 5, label: "5 / trang" },
              { value: 10, label: "10 / trang" },
              { value: 20, label: "20 / trang" },
            ]}
          />
        </Row>
      )}
    </Space>
  );
}
