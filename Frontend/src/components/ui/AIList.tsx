import { Flex, Pagination, Empty, Grid, Result } from "antd";
import { AICardItem, type AICardItemProps } from "./AICardItem";
import type { PageSize } from "@/types";
import { useTranslation } from "react-i18next";

const { useBreakpoint } = Grid;
export interface AIListProps<T = any> {
  /** Array of data items to render */
  data: T[];
  /** Maps each item → unique key (used for React list rendering) */
  itemKey: (item: T) => React.Key;
  /** Maps each item → AICardItemProps (header, content, footer, leftSide, rightSide, …) */
  renderItem: (item: T) => AICardItemProps;
  /** Optional antd Pagination props — when provided, data is auto-sliced by pageSize/current */
  paginationProps: {
    page: number;
    pageSize: number;
    total: number;
    onPageChange: (page: number) => void;
    onPageSizeChange: (pageSize: PageSize) => void;
  };
  /** Forwarded to each AICardItem for skeleton loading */
  isLoading?: boolean;
  /** Gap between items (px). Default: 12 */
  gap?: number;
  /** Whether there's an active search query — when true, falls back to notFound instead of empty */
  hasSearchQuery?: boolean;
  /** Custom empty state when data is empty AND there is no search query */
  empty?: {
    title?: React.ReactNode;
    subTitle?: React.ReactNode;
    icon?: React.ReactNode;
  };
  /** Custom empty state when data is empty AND there is an active search query. Falls back to `empty` if not provided. */
  notFound?: {
    title?: React.ReactNode;
    subTitle?: React.ReactNode;
    icon?: React.ReactNode;
  };
}

export function AIList<T>({
  data,
  itemKey,
  renderItem,
  paginationProps,
  isLoading = false,
  gap = 12,
  hasSearchQuery = false,
  empty = { title: undefined, subTitle: undefined, icon: undefined },
  notFound,
}: AIListProps<T>) {
  // Slice data based on pagination state
  const pageSize = paginationProps.pageSize;
  const current = paginationProps.page;
  const start = (current - 1) * pageSize;
  const end = start + pageSize;
  const displayData = paginationProps ? data.slice(start, end) : data;
  const { t } = useTranslation();

  const screens = useBreakpoint();
  const isMobile = !screens.md;

  if (!isLoading && data.length === 0) {
    const emptyConfig = hasSearchQuery ? (notFound ?? empty) : empty;
    return (
      <div style={{ padding: "48px 0" }}>
        <Result
          title={emptyConfig.title || t("list.empty")}
          subTitle={emptyConfig.subTitle || t("list.emptyDescription")}
          icon={emptyConfig.icon || Empty.PRESENTED_IMAGE_SIMPLE}
        />
      </div>
    );
  }

  return (
    <Flex vertical gap={gap}>
      {isLoading && <AICardItem isLoading={isLoading} />}
      {displayData.map((item) => {
        return (
          <AICardItem
            key={itemKey(item)}
            {...renderItem(item)}
            isLoading={isLoading}
            isHoverable
          />
        );
      })}

      {paginationProps && data.length > 0 && (
        <Flex justify="end" style={{ marginTop: gap ? 12 : 0 }}>
          <Pagination
            current={paginationProps.page}
            pageSize={paginationProps.pageSize}
            total={paginationProps.total}
            showSizeChanger={!isMobile}
            simple={isMobile}
            showTotal={
              isMobile
                ? undefined
                : (total, range) =>
                    `${range[0]}-${range[1]} ${t("list.of")} ${total}`
            }
            onChange={(page, pageSize) => {
              if (page !== paginationProps.page) {
                paginationProps.onPageChange(page);
              }
              if (pageSize !== paginationProps.pageSize) {
                paginationProps.onPageSizeChange(pageSize as PageSize);
              }
            }}
            style={{ borderRadius: 8 }}
          />
        </Flex>
      )}
    </Flex>
  );
}
