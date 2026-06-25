import { useTranslation } from "react-i18next";
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
  const { t } = useTranslation();
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
      message.success(t("team.editSuccess"));
    } catch (error) {
      console.error("Failed to update team:", error);
      message.error(t("team.editError"));
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
      title={t("team.editTeamTitle")}
      open={isOpen}
      onOk={handleUpdate}
      onCancel={handleClose}
      okText={t("team.editButton")}
      cancelText={t("team.cancel")}
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
          label={t("team.editTeamNameLabel")}
          name="name"
          rules={[{ required: true, message: t("team.editTeamNameRequired") }]}
        >
          <Input />
        </Form.Item>
        <Form.Item label={t("team.descriptionLabel")} name="description">
          <Input.TextArea
            rows={4}
            placeholder={t("team.descriptionPlaceholder")}
          />
        </Form.Item>
      </Form>
    </Modal>
  );
}
