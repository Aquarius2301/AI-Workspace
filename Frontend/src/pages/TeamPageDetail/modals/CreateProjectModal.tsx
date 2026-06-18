import { useProject } from "@/hooks";
import { Modal, App, Form, Input, Select } from "antd";
import Text from "antd/es/typography/Text";

interface CreateProjectModalProps {
  teamId: string;
  isOpen: boolean;
  onClose: () => void;
}

export function CreateProjectModal({
  teamId,
  isOpen,
  onClose,
}: CreateProjectModalProps) {
  const { create } = useProject();
  const [form] = Form.useForm();

  const { message } = App.useApp();

  const resetForm = () => {
    form.resetFields();
  };

  const handleClose = () => {
    resetForm();
    onClose();
  };

  const handleCreate = async () => {
    try {
      const formValues = await form.getFieldsValue();
      await create.mutateAsync({
        teamId,
        data: {
          name: formValues.name,
          description: formValues.description || undefined,
          isPublic: formValues.visibility == "Public",
        },
      });
      handleClose();
      message.success("Dự án đã được tạo thành công!");
    } catch (error) {
      console.error("Failed to create project:", error);
      message.error("Đã xảy ra lỗi khi tạo dự án. Vui lòng thử lại.");
    }
  };

  return (
    <Modal
      title="Tạo dự án mới"
      open={isOpen}
      onOk={handleCreate}
      onCancel={handleClose}
      okText="Tạo dự án"
      cancelText="Hủy"
      confirmLoading={create.isPending}
    >
      <Form
        form={form}
        layout="vertical"
        style={{ width: "100%" }}
        disabled={create.isPending}
      >
        <Form.Item
          label="Tên dự án"
          name="name"
          rules={[{ required: true, message: "Vui lòng nhập tên dự án!" }]}
        >
          <Input />
        </Form.Item>
        <Form.Item label="Mô tả dự án" name="description">
          <Input.TextArea
            rows={4}
            placeholder="Nhập mô tả cho dự án (tùy chọn)"
          />
        </Form.Item>
        <Form.Item
          label="Dự án công khai / riêng tư"
          name="visibility"
          rules={[{ required: true, message: "Vui lòng chọn tính riêng tư!" }]}
          initialValue={{
            visibility: "Public",
          }}
        >
          <Select
            options={[
              { value: "Public", label: "Công khai" },
              { value: "Private", label: "Riêng tư" },
            ]}
            defaultValue="Public"
          />
        </Form.Item>
        <Text type="secondary">
          "Công khai": mọi người trong nhóm có thể nhìn thấy tên dự án.
        </Text>
        <br />
        <Text type="secondary">
          "Riêng tư": chỉ có Admin, Leader tạo dự án và những thành viên được
          mời vào project có thể thấy dự án.
        </Text>
      </Form>
    </Modal>
  );
}
