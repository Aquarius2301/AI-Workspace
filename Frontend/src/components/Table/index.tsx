import { Grid, Table as AntdTable } from "antd";
import type { ColumnsType, TableProps } from "antd/es/table";
import { useMemo } from "react";

const { useBreakpoint } = Grid;

export type CustomColumnsType<T> = ColumnsType<T> & {
  noShowMobileTitle?: boolean;
};

export function Table<T>(props: TableProps<T>) {
  const screens = useBreakpoint();

  const isMobile = !screens.md;
  const mobileColumns = useMemo<ColumnsType<T>>(() => {
    return [
      {
        title: "Thông tin chi tiết",
        key: "mobile-summary",
        render: (_, record: T, index: number) => (
          <div
            style={{
              display: "flex",
              flexDirection: "column",
              gap: "8px",
              padding: "8px 0",
            }}
          >
            {props.columns?.map((col: any) => {
              const rawValue = col.dataIndex
                ? (record as any)[col.dataIndex]
                : undefined;
              let renderedValue = rawValue;
              if (col.render) {
                renderedValue = col.render(rawValue, record, index);
              }
              return (
                <div key={col.key || col.dataIndex}>
                  {col.title && !col.noShowMobileTitle && (
                    <strong style={{ marginRight: "6px" }}>{col.title}:</strong>
                  )}
                  <span>{renderedValue ?? "-"}</span>
                </div>
              );
            })}
          </div>
        ),
      },
    ];
  }, [props.columns]);

  return (
    <AntdTable {...props} columns={isMobile ? mobileColumns : props.columns} />
  );
}
