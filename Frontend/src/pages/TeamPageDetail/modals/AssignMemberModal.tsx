import { TEAM_MEMBERS_QUERY_KEY, useTeam } from "@/hooks";
import type { TeamMemberItem, TeamRole } from "@/types";
import { Modal, App, Select, Button } from "antd";
import type { ColumnsType } from "antd/es/table";
import { useEffect, useState } from "react";
import { useQueryClient } from "@tanstack/react-query";

interface AssignMemberModalProps {
  teamId: string;
  isOpen: boolean;
  onClose: () => void;
}

const ROLE_OPTIONS: { value: TeamRole; label: string }[] = [
  { value: "Admin", label: "Admin" },
  { value: "Leader", label: "Leader" },
  { value: "Member", label: "Member" },
];

export function AssignMemberModal({
  teamId,
  isOpen,
  onClose,
}: AssignMemberModalProps) {
  const queryClient = useQueryClient();
  const { getMembers, updateMemberRole } = useTeam();
  const { message } = App.useApp();

  const {
    data: members,
    isLoading: isMembersLoading,
    isError,
  } = getMembers(teamId);

  const [roleChanges, setRoleChanges] = useState<
    Record<string, TeamRole | undefined>
  >({});
  const [savingUserId, setSavingUserId] = useState<string | null>(null);

  useEffect(() => {
    if (isOpen) {
      setRoleChanges({});
      setSavingUserId(null);
    }
  }, [isOpen]);

  const handleRoleChange = (userId: string, role: TeamRole) => {
    setRoleChanges((prev) => ({
      ...prev,
      [userId]: role,
    }));
  };

  const handleSaveRole = async (userId: string) => {
    const newRole = roleChanges[userId];
    if (!newRole) return;

    setSavingUserId(userId);
    try {
      await updateMemberRole.mutateAsync({
        id: teamId,
        memberId: userId,
        data: { role: newRole },
      });
      message.success("Vai trò đã được cập nhật thành công!");
      setRoleChanges((prev) => {
        const next = { ...prev };
        delete next[userId];
        return next;
      });
    } catch (error) {
      console.error("Failed to update member role:", error);
      message.error("Đã xảy ra lỗi khi cập nhật vai trò. Vui lòng thử lại.");
    } finally {
      setSavingUserId(null);
    }
  };

  const handleClose = () => {
    setRoleChanges({});
    queryClient.removeQueries({
      queryKey: [...TEAM_MEMBERS_QUERY_KEY, teamId],
    });
    onClose();
  };

  const columns: ColumnsType<TeamMemberItem> = [
    {
      title: "Thành viên",
      key: "member",
      render: (_, record) => (
        <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
          <img
            src={
              record.email ? `/avatar/${record.email}` : "/default-avatar.png"
            }
            alt={record.userName}
            style={{ width: 32, height: 32, borderRadius: "50%" }}
          />
          <div>
            <div style={{ fontWeight: 500 }}>{record.userName}</div>
            <div style={{ fontSize: 12, color: "#888" }}>
              {record.email || ""}
            </div>
          </div>
        </div>
      ),
    },
    {
      title: "Vai trò hiện tại",
      dataIndex: "role",
      key: "currentRole",
      width: 120,
      render: (role?: TeamRole) => {
        if (!role) return "-";
        const option = ROLE_OPTIONS.find((o) => o.value === role);
        return option?.label ?? role;
      },
    },
    {
      title: "Vai trò mới",
      key: "newRole",
      width: 180,
      render: (_, record) => {
        const changedRole = roleChanges[record.userId];
        return (
          <Select
            allowClear
            placeholder="Chọn vai trò"
            value={changedRole}
            onChange={(value) =>
              handleRoleChange(record.userId, value as TeamRole)
            }
            options={ROLE_OPTIONS}
            style={{ width: 130 }}
          />
        );
      },
    },
    {
      title: "Hành động",
      key: "action",
      width: 120,
      render: (_, record) => {
        const hasChange = roleChanges[record.userId] !== undefined;
        return (
          <Button
            type="primary"
            size="small"
            disabled={!hasChange}
            loading={savingUserId === record.userId}
            onClick={() => handleSaveRole(record.userId)}
          >
            Lưu
          </Button>
        );
      },
    },
    {
      title: "Ngày tham gia",
      dataIndex: "joinedAt",
      key: "joinedAt",
      width: 180,
      render: (joinedAt: string) =>
        joinedAt
          ? new Date(joinedAt).toLocaleDateString("vi-VN", {
              year: "numeric",
              month: "2-digit",
              day: "2-digit",
            })
          : "-",
    },
  ];

  return (
    <Modal
      title="Gán vai trò thành viên"
      open={isOpen}
      onCancel={handleClose}
      footer={[
        <Button key="back" onClick={handleClose}>
          Đóng
        </Button>,
      ]}
      width={800}
    >
      {isError && !isMembersLoading && (
        <div style={{ color: "red", marginBottom: 16 }}>
          Không thể tải danh sách thành viên. Vui lòng thử lại sau.
        </div>
      )}
      {/* <Table
        dataSource={members}
        columns={columns}
        rowKey="userId"
        loading={isMembersLoading}
        pagination={false}
        scroll={{ y: 400 }}
      /> */}
    </Modal>
  );
}
