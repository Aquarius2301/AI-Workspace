import { Pagination, Grid, Flex } from "antd";
import { PAGE_SIZES, type PageSize } from "@/types";
import { useTranslation } from "react-i18next";

const { useBreakpoint } = Grid;

export interface AIPaginationProps {
  page: number;
  pageSize: number;
  total: number;
  onPageChange: (page: number) => void;
  onPageSizeChange: (pageSize: PageSize) => void;
  /** Show total range text (e.g. "1-10 of 20"). Default: true on desktop, false on mobile */
  showTotal?: boolean;
  /** Use small size. Default: false */
  small?: boolean;
}

export function AIPagination({
  page,
  pageSize,
  total,
  onPageChange,
  onPageSizeChange,
  showTotal,
  small = false,
}: AIPaginationProps) {
  const { t } = useTranslation();
  const screens = useBreakpoint();
  const isMobile = !screens.md;

  const resolvedShowTotal = showTotal ?? !isMobile;

  return (
    <Flex justify="end">
      <Pagination
        current={page}
        pageSize={pageSize}
        total={total}
        showSizeChanger={!isMobile}
        showTotal={
          resolvedShowTotal
            ? (total, range) =>
                `${range[0]}-${range[1]} ${t("list.of")} ${total}`
            : undefined
        }
        pageSizeOptions={PAGE_SIZES.map((size) => size.toString())}
        locale={{ items_per_page: `/ ${t("list.page")}` }}
        simple={isMobile && !small}
        size={small ? "small" : undefined}
        onChange={(newPage, newPageSize) => {
          if (newPage !== page) onPageChange(newPage);
          if (newPageSize !== pageSize) {
            onPageSizeChange(newPageSize as PageSize);
          }
        }}
        style={{ borderRadius: 8 }}
      />
    </Flex>
  );
}
