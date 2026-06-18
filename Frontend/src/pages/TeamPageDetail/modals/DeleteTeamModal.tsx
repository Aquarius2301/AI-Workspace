import { useTeam } from "@/hooks";
import { Modal, Space, App } from "antd";
import Input from "antd/es/input/Input";
import Text from "antd/es/typography/Text";
import { useState } from "react";
import { useNavigate } from "react-router-dom";

interface DeleteTeamModalProps {
  teamId: string;
  teamName: string;
  isOpen: boolean;
  onClose: () => void;
}

export function DeleteTeamModal({
  teamId,
  teamName,
  isOpen,
  onClose,
}: DeleteTeamModalProps) {
  const { deleteTeam } = useTeam();
  const [confirmName, setConfirmName] = useState("");
  const navigate = useNavigate();

  const [isConfirmModalOpen, setIsConfirmModalOpen] = useState(false);

  const { message } = App.useApp();

  const isConfirmMatch = confirmName === teamName;

  const resetForm = () => {
    setConfirmName("");
  };

  const handleClose = () => {
    resetForm();
    onClose();
  };

  const handleDelete = async () => {
    if (!isConfirmMatch) return;
    try {
      await deleteTeam.mutateAsync(teamId);
      handleClose();
      message.success("Nhóm đã được xóa thành công!");
      navigate("/teams");
    } catch (error) {
      console.error("Failed to delete team:", error);
      message.error("Đã xảy ra lỗi khi xóa nhóm. Vui lòng thử lại.");
    }
  };

  return (
    <>
      <Modal
        title="Xóa nhóm"
        open={isOpen}
        onOk={() => {
          setIsConfirmModalOpen(true);
          handleClose();
        }}
        onCancel={handleClose}
        okText="Xóa nhóm"
        cancelText="Hủy"
        okButtonProps={{ danger: true }}
      >
        <Space direction="vertical" size={12} style={{ width: "100%" }}>
          <Text>Bạn có chắc chắn muốn xóa nhóm này không?</Text>
          <Text type="danger">
            Tất cả mọi dự án, nhiệm vụ, và thành viên trong nhóm sẽ bị xóa vĩnh
            viễn và không thể khôi phục. Hãy chắc chắn rằng bạn muốn thực hiện
            hành động này.
          </Text>
        </Space>
      </Modal>

      <Modal
        title="Xóa nhóm"
        open={isConfirmModalOpen}
        onCancel={() => setIsConfirmModalOpen(false)}
        onOk={handleDelete}
        okText="Xóa nhóm"
        cancelText="Hủy"
        okButtonProps={{ danger: true, disabled: !isConfirmMatch }}
        confirmLoading={deleteTeam.isPending}
      >
        <div style={{ marginBottom: 16 }}>
          <Text strong>
            Nhập <Text code>{teamName}</Text> để xác nhận xóa nhóm:
          </Text>
        </div>
        <Input
          style={{ marginBottom: 16 }}
          value={confirmName}
          onChange={(e) => setConfirmName(e.target.value)}
        />
        <Text type="danger">
          Sau khi ấn "Xóa nhóm", tất cả mọi dự án, nhiệm vụ, và thành viên trong
          nhóm sẽ bị xóa vĩnh viễn và không thể khôi phục.
        </Text>
      </Modal>
    </>
  );
}
