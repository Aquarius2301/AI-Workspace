import React from "react";
import { Modal, Form, Input, App } from "antd";
import { useTeam } from "@/hooks/useTeam.hook";
import Text from "antd/es/typography/Text";

interface TeamModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export function TeamModal({ isOpen, onClose }: TeamModalProps) {
  const [form] = Form.useForm();
  const { create } = useTeam();
  const [submitting, setSubmitting] = React.useState(false);

  const { message } = App.useApp();

  const handleFinish = async (values: any) => {
    setSubmitting(true);
    try {
      await create.mutateAsync(values);
      onClose(); // Close modal on success
      message.success("Nhóm đã được tạo thành công!");
    } catch (error) {
      console.error("Failed to create team:", error);
      message.error("Đã xảy ra lỗi khi tạo nhóm. Vui lòng thử lại.");
      // Handle error (e.g., show notification)
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Modal
      title="Tạo nhóm"
      open={isOpen}
      onOk={() => form.submit()}
      onCancel={onClose}
      confirmLoading={submitting}
      okText="Tạo"
      cancelText="Hủy"
    >
      <Form
        form={form}
        layout="vertical"
        onFinish={handleFinish}
        disabled={submitting}
        name="create-team-form"
        style={{ width: "100%" }}
      >
        <Form.Item
          label="Tên nhóm"
          name="name"
          rules={[{ required: true, message: "Vui lòng nhập tên nhóm" }]}
        >
          <Input placeholder="Nhập tên nhóm" />
        </Form.Item>
        <Form.Item label="Mô tả" name="description">
          <Input.TextArea placeholder="Nhập mô tả nhóm (tùy chọn)" rows={4} />
        </Form.Item>
        <Text type="secondary">
          Bạn sẽ là Admin của nhóm này. Bạn có thể mời thêm thành viên sau khi
          tạo nhóm.
        </Text>
      </Form>
    </Modal>
  );
}
