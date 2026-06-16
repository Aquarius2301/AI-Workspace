import type { TeamMemberItem } from "@/types";
import { getDistanceToNow } from "@/utils/date.util";
import { Badge, Empty, Space } from "antd";
import type { ColumnsType } from "antd/es/table";
import Table from "antd/es/table";
import Text from "antd/es/typography/Text";

interface MemberListProps {
  members?: TeamMemberItem[];
  isMembersLoading: boolean;
  userId: string;
}

export function MemberList({
  members,
  isMembersLoading,
  userId,
}: MemberListProps) {
  const memberColumns: ColumnsType<TeamMemberItem> = [
    {
      title: "Tên người dùng",
      dataIndex: "userName",
      key: "userName",
      render: (userName: string, record: TeamMemberItem) => {
        const timeDistance = getDistanceToNow(
          record.lastActiveAt,
          "minute",
          false,
        );
        const isActive =
          timeDistance !== null &&
          timeDistance <= 10 &&
          userId != record.userId;

        return (
          <Space>
            <Text strong>{userName}</Text>
            {isActive && <Badge status="processing" />}
            {userId === record.userId && <Text strong>(Bạn)</Text>}
          </Space>
        );
      },
    },
    {
      title: "Email",
      dataIndex: "email",
      key: "email",
      render: (email: string) => email || <Text type="secondary">—</Text>,
    },

    {
      title: "Vai trò",
      dataIndex: "role",
      key: "role",
      render: (role: string) => role || <Text type="secondary">—</Text>,
    },
    {
      title: "Ngày tham gia",
      dataIndex: "joinedAt",
      key: "joinedAt",
      render: (joinedAt: string) =>
        new Date(joinedAt).toLocaleDateString("vi-VN"),
    },
  ];

  return (
    <Space vertical size="large" style={{ width: "100%" }}>
      {/* {(role === "Admin" || role === "Leader") && (
        <Button>Thêm thành viên</Button>
      )} */}
      <Table<TeamMemberItem>
        dataSource={members ?? []}
        columns={memberColumns}
        rowKey="userId"
        loading={isMembersLoading}
        locale={{ emptyText: <Empty description="Chưa có thành viên nào" /> }}
        pagination={false}
      />
    </Space>
  );
}
