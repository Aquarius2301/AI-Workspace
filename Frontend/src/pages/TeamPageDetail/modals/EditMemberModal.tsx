import React from "react";
import { Modal, Button, message, Space } from "antd";
import { ExclamationCircleFilled } from "@ant-design/icons";
import type { TeamRole } from "@/types";
import { useTeam, TEAM_MEMBERS_QUERY_KEY } from "@/hooks";
import { useQueryClient } from "@tanstack/react-query";
import Text from "antd/es/typography/Text";
import { RoleSelect } from "@/components";

interface EditMemberModalProps {
  isOpen: boolean;
  onClose: () => void;
  memberId: string;
  teamId: string;
  userName: string;
  email: string;
  currentRole: TeamRole;
}

export function EditMemberModal({
  isOpen,
  onClose,
  memberId,
  teamId,
  userName,
  email,
  currentRole,
}: EditMemberModalProps) {
  const queryClient = useQueryClient();
  const { updateMemberRole, removeMember } = useTeam();

  const [role, setRole] = React.useState<TeamRole>(currentRole);
  const [isRemoving, setIsRemoving] = React.useState(false);
  const [isUpdatingRole, setIsUpdatingRole] = React.useState(false);
  const [modal, contextHolder] = Modal.useModal();

  React.useEffect(() => {
    if (isOpen) {
      setRole(currentRole);
    }
  }, [currentRole, isOpen]);

  const invalidateMembers = () => {
    queryClient.invalidateQueries({
      queryKey: [...TEAM_MEMBERS_QUERY_KEY, teamId],
    });
  };

  const handleRoleChange = (value: TeamRole) => {
    setRole(value);
  };

  const executeRemoveMember = async () => {
    setIsRemoving(true);
    try {
      await removeMember.mutateAsync({
        id: teamId,
        memberId: memberId,
      });
      message.success("Thành viên đã được xóa khỏi nhóm thành công!");
      onClose();
      invalidateMembers();
    } catch (error) {
      message.error("Đã xảy ra lỗi khi xóa thành viên. Vui lòng thử lại.");
      console.error("Failed to remove member:", error);
    } finally {
      setIsRemoving(false);
    }
  };

  const showConfirmDelete = () => {
    modal.confirm({
      title: "Xác nhận xóa thành viên?",
      icon: <ExclamationCircleFilled style={{ color: "#ff4d4f" }} />,
      content: (
        <Space vertical size={8}>
          <Text>
            Bạn có chắc chắn muốn xóa thành viên {userName} ({email}) ra khỏi
            nhóm. này không?
          </Text>
          <Text type="danger">Hành động này không thể hoàn tác.</Text>
        </Space>
      ),
      okText: "Xóa ngay",
      okType: "danger",
      cancelText: "Hủy",
      centered: true,
      onOk() {
        executeRemoveMember();
      },
    });
  };

  const handleUpdateRole = async () => {
    if (!role) return;

    setIsUpdatingRole(true);
    try {
      await updateMemberRole.mutateAsync({
        id: teamId,
        memberId: memberId,
        data: { role },
      });
      message.success("Vai trò thành viên đã được cập nhật thành công!");
      onClose();
      invalidateMembers();
    } catch (error) {
      message.error("Đã xảy ra lỗi khi cập nhật vai trò. Vui lòng thử lại.");
      console.error("Failed to update member role:", error);
    } finally {
      setIsUpdatingRole(false);
    }
  };

  return (
    <>
      {contextHolder}

      <Modal
        title="Chỉnh sửa thành viên"
        open={isOpen}
        onCancel={onClose}
        width={400}
        centered
        destroyOnHidden
        mask={{ closable: false }}
        footer={[
          <Button
            key="remove"
            danger
            onClick={showConfirmDelete}
            disabled={isRemoving || isUpdatingRole}
          >
            {isRemoving ? "Đang xóa..." : "Xóa thành viên"}
          </Button>,
          <Button
            key="update"
            type="primary"
            onClick={handleUpdateRole}
            disabled={isUpdatingRole || isRemoving || role === currentRole}
          >
            {isUpdatingRole ? "Đang cập nhật..." : "Cập nhật vai trò"}
          </Button>,
        ]}
      >
        <Space
          vertical
          size={12}
          style={{ width: "100%", paddingTop: 8, paddingBottom: 8 }}
        >
          <div>
            <Text strong>Thành viên:</Text> <Text>{userName}</Text>
          </div>
          <div>
            <Text strong>Email:</Text> <Text>{email}</Text>
          </div>
          <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
            <Text strong style={{ minWidth: 60 }}>
              Vai trò:
            </Text>
            <div style={{ flex: 1 }}>
              <RoleSelect
                value={role}
                onChange={(role) => handleRoleChange(role as TeamRole)}
                placeholder="Chọn vai trò"
                showAll={false}
              />
            </div>
          </div>
        </Space>
      </Modal>
    </>
  );
}
