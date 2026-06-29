import { useState } from "react";
import { Form, Input, App, Flex, Typography } from "antd";
import { useTranslation } from "react-i18next";
import { AIModal, AIVisibilitySelect } from "@/components";
import { useProject } from "@/hooks";
import type { CreateProjectRequest } from "@/types";
import { getTranslatedErrorMessage } from "@/utils";

const { Text } = Typography;

interface CreateProjectModalProps {
  isOpen: boolean;
  onClose: () => void;
  teamId: string;
}

export function CreateProjectModal({
  isOpen,
  onClose,
  teamId,
}: CreateProjectModalProps) {
  const { t } = useTranslation();
  const [form] = Form.useForm<CreateProjectRequest>();
  const { message } = App.useApp();
  const { create } = useProject();

  const [visibility, setVisibility] = useState<"Public" | "Private">("Public");

  const handleCreate = async () => {
    try {
      const values = await form.validateFields();
      await create.mutateAsync({ ...values, teamId, visibility });
      message.success(t("teamDetail.projects.create.success"));
      form.resetFields();
      setVisibility("Public");
      onClose();
    } catch (error) {
      message.error(getTranslatedErrorMessage(error));
    }
  };

  const handleClose = () => {
    form.resetFields();
    setVisibility("Public");
    onClose();
  };

  return (
    <AIModal
      title={t("teamDetail.projects.create.title")}
      open={isOpen}
      onOk={handleCreate}
      onCancel={handleClose}
      isLoading={create.isPending}
      footer={[{ type: "cancel" }, { type: "create" }]}
    >
      <Form
        form={form}
        layout="vertical"
        autoComplete="off"
        style={{ marginTop: 16 }}
      >
        <Form.Item
          name="name"
          label={t("teamDetail.projects.create.name.default")}
          rules={[
            {
              required: true,
              message: t("teamDetail.projects.create.name.required"),
            },
          ]}
        >
          <Input placeholder={t("teamDetail.projects.create.name.default")} />
        </Form.Item>

        <Form.Item
          name="description"
          label={t("teamDetail.projects.create.description.default")}
        >
          <Input.TextArea
            placeholder={t("teamDetail.projects.create.description.placeholder")}
            rows={3}
          />
        </Form.Item>

        <Form.Item label={t("teamDetail.projects.create.visibility")}>
          <Flex vertical gap={4}>
            <AIVisibilitySelect
              value={visibility}
              onChange={(v) => setVisibility(v ?? "Public")}
              allowClear={false}
              style={{ width: "100%" }}
            />
            <Text type="secondary" style={{ fontSize: 12 }}>
              {visibility === "Public"
                ? t("teamDetail.projects.create.publicDescription")
                : t("teamDetail.projects.create.privateDescription")}
            </Text>
          </Flex>
        </Form.Item>
      </Form>
    </AIModal>
  );
}
