import { useTranslation } from "react-i18next";
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
  const { t } = useTranslation();
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
      message.success(t("project.createSuccess"));
    } catch (error) {
      console.error("Failed to create project:", error);
      message.error(t("project.createError"));
    }
  };

  return (
    <Modal
      title={t("project.createTitle")}
      open={isOpen}
      onOk={handleCreate}
      onCancel={handleClose}
      okText={t("project.createButton")}
      cancelText={t("team.cancel")}
      confirmLoading={create.isPending}
    >
      <Form
        form={form}
        layout="vertical"
        style={{ width: "100%" }}
        disabled={create.isPending}
      >
        <Form.Item
          label={t("project.createNameLabel")}
          name="name"
          rules={[{ required: true, message: t("project.createNameRequired") }]}
        >
          <Input />
        </Form.Item>
        <Form.Item
          label={t("project.createDescriptionLabel")}
          name="description"
        >
          <Input.TextArea
            rows={4}
            placeholder={t("project.createDescriptionPlaceholder")}
          />
        </Form.Item>
        <Form.Item
          label={t("project.createVisibilityLabel")}
          name="visibility"
          rules={[
            { required: true, message: t("project.createVisibilityRequired") },
          ]}
          initialValue={{
            visibility: "Public",
          }}
        >
          <Select
            options={[
              { value: "Public", label: t("project.publicOption") },
              { value: "Private", label: t("project.privateOption") },
            ]}
            defaultValue="Public"
          />
        </Form.Item>
        <Text type="secondary">{t("project.publicDesc")}</Text>
        <br />
        <Text type="secondary">{t("project.privateDesc")}</Text>
      </Form>
    </Modal>
  );
}
