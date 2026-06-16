import { Spin, theme } from "antd";
import { LoadingOutlined } from "@ant-design/icons";

export function FullscreenLoading() {
  // Lấy token hiện tại để đồng bộ màu sắc tự động
  const { token } = theme.useToken();

  // Khởi tạo icon loading lớn, sử dụng màu Primary (Indigo) bạn đã cấu hình
  const loadingIcon = (
    <LoadingOutlined
      style={{
        fontSize: 48,
        color: token.colorPrimary,
      }}
      spin
    />
  );

  return (
    <div
      style={{
        position: "fixed",
        top: 0,
        left: 0,
        width: "100vw",
        height: "100vh",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        // Đồng bộ màu nền theo Layout (Sáng: #F8FAFC / Tối: #09090B)
        backgroundColor: `${token.colorBgLayout}CC`,

        // Đảm bảo che phủ lên trên tất cả các thành phần khác (kể cả Header/Navbar nếu có)
        zIndex: 9999,
        transition: "all 0.3s ease",
      }}
    >
      <Spin indicator={loadingIcon} size="large" />
    </div>
  );
}
