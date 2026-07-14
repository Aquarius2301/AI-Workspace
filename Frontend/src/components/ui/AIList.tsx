import { Flex, Result, Empty } from "antd";
import { AICardItem } from "./AICardItem";
import { AIPagination } from "./AIPagination";
import type { PageSize } from "@/types";
import { useTranslation } from "react-i18next";
import React from "react";

export interface PaginationProps {
  page: number;
  pageSize: number;
  total: number;
  onPageChange: (page: number) => void;
  onPageSizeChange: (pageSize: PageSize) => void;
}

export interface AIListProps<T = any> {
  /** Array of data items to render */
  data: T[];
  /** Maps each item → unique key (used for React list rendering) */
  itemKey: (item: T) => React.Key;
  /** Maps each item → AICardItemProps (header, content, footer, leftSide, rightSide, …) */
  renderItem: (item: T) => React.ReactNode;
  /** Optional antd Pagination props — when provided, data is auto-sliced by pageSize/current and paginator is shown. When omitted, renders full data without pagination. */
  paginationProps?: PaginationProps;
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
  // Slice data based on pagination state (if pagination is enabled)
  const displayData = paginationProps
    ? data.slice(
        (paginationProps.page - 1) * paginationProps.pageSize,
        paginationProps.page * paginationProps.pageSize,
      )
    : data;
  const { t } = useTranslation();

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
          <React.Fragment key={itemKey(item)}>
            {renderItem(item)}
          </React.Fragment>
        );
      })}

      {paginationProps && data.length > 0 && (
        <div style={{ marginTop: gap ? 12 : 0 }}>
          <AIPagination
            page={paginationProps.page}
            pageSize={paginationProps.pageSize}
            total={paginationProps.total}
            onPageChange={paginationProps.onPageChange}
            onPageSizeChange={paginationProps.onPageSizeChange}
          />
        </div>
      )}
    </Flex>
  );
}
