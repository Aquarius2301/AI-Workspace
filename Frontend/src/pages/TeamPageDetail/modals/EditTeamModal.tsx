import { useState } from "react";
import { useTeam } from "@/hooks";
import { Modal, App, Form, Input } from "antd";

interface EditTeamModalProps {
  teamId: string;
  teamName: string;
  teamDescription?: string;
  isOpen: boolean;
  onClose: () => void;
}

export function EditTeamModal({
  teamId,
  teamName,
  teamDescription,
  isOpen,
  onClose,
}: EditTeamModalProps) {
  const { update } = useTeam();
  const [form] = Form.useForm();
  const [hasChanges, setHasChanges] = useState(false);

  const { message } = App.useApp();

  const resetForm = () => {
    form.resetFields();
    setHasChanges(false);
  };

  const handleClose = () => {
    resetForm();
    onClose();
  };

  const handleUpdate = async () => {
    try {
      const formValues = form.getFieldsValue();
      const updateValues = {
        name: formValues.name === teamName ? null : formValues.name,
        description:
          formValues.description === (teamDescription || "")
            ? null
            : formValues.description || null,
      };

      await update.mutateAsync({
        id: teamId,
        data: updateValues,
      });
      handleClose();
      message.success("Nhóm đã được sửa thành công!");
    } catch (error) {
      console.error("Failed to update team:", error);
      message.error("Đã xảy ra lỗi khi sửa nhóm. Vui lòng thử lại.");
    }
  };

  const handleValuesChange = () => {
    const values = form.getFieldsValue();
    const nameChanged = values.name !== teamName;
    const descChanged = values.description !== (teamDescription || "");
    setHasChanges(nameChanged || descChanged);
  };

  return (
    <Modal
      title="Chỉnh sửa nhóm"
      open={isOpen}
      onOk={handleUpdate}
      onCancel={handleClose}
      okText="Chỉnh sửa"
      cancelText="Hủy"
      confirmLoading={update.isPending}
      okButtonProps={{ disabled: !hasChanges }}
    >
      <Form
        disabled={update.isPending}
        form={form}
        layout="vertical"
        initialValues={{
          name: teamName,
          description: teamDescription || "",
        }}
        onValuesChange={handleValuesChange}
      >
        <Form.Item
          label="Tên nhóm"
          name="name"
          rules={[{ required: true, message: "Vui lòng nhập tên nhóm!" }]}
        >
          <Input />
        </Form.Item>
        <Form.Item label="Mô tả" name="description">
          <Input.TextArea
            rows={4}
            placeholder="Nhập mô tả cho nhóm (tùy chọn)"
          />
        </Form.Item>
      </Form>
    </Modal>
  );
}
