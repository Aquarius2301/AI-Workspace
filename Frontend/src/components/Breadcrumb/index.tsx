import {
  type BreadcrumbProps,
  Breadcrumb as AntdBreadCrumb,
  theme,
} from "antd";

export function Breadcrumb({ items }: BreadcrumbProps) {
  const { token } = theme.useToken();
  return (
    <div
      style={{
        padding: 10,
        fontWeight: "bold",
        border: `1px solid ${token.colorBorder}`,
        borderRadius: token.borderRadius,
        backgroundColor: token.colorBgContainer,
      }}
    >
      <AntdBreadCrumb items={items} />
    </div>
  );
}
