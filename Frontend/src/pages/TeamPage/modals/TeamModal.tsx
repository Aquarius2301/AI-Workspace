import React from "react";
import { useTranslation } from "react-i18next";
import { Modal, Form, Input, App } from "antd";
import { useTeam } from "@/hooks/useTeam.hook";
import Text from "antd/es/typography/Text";

interface TeamModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export function TeamModal({ isOpen, onClose }: TeamModalProps) {
  const { t } = useTranslation();
  const [form] = Form.useForm();
  const { create } = useTeam();
  const [submitting, setSubmitting] = React.useState(false);

  const { message } = App.useApp();

  const handleFinish = async (values: any) => {
    setSubmitting(true);
    try {
      await create.mutateAsync(values);
      onClose(); // Close modal on success
      message.success(t("team.createSuccess"));
    } catch (error) {
      console.error("Failed to create team:", error);
      message.error(t("team.createError"));
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Modal
      title={t("team.createTitle")}
      open={isOpen}
      onOk={() => form.submit()}
      onCancel={onClose}
      confirmLoading={submitting}
      okText={t("team.createOk")}
      cancelText={t("team.cancel")}
      closeIcon={null}
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
          label={t("team.nameLabel")}
          name="name"
          rules={[{ required: true, message: t("team.nameRequired") }]}
        >
          <Input placeholder={t("team.namePlaceholder")} />
        </Form.Item>
        <Form.Item label={t("team.descriptionLabel")} name="description">
          <Input.TextArea
            placeholder={t("team.descriptionPlaceholder")}
            rows={4}
          />
        </Form.Item>
        <Text type="secondary">{t("team.adminNote")}</Text>
      </Form>
    </Modal>
  );
}
