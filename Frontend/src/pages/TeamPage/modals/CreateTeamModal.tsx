import { AIModal } from "@/components";
import { useTeam } from "@/hooks";
import type { CreateTeamRequest } from "@/types";
import { Form, App, Input } from "antd";
import { getTranslatedErrorMessage } from "@/utils";
import { useTranslation } from "react-i18next";

interface CreateTeamModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export function CreateTeamModal({ isOpen, onClose }: CreateTeamModalProps) {
  const { t } = useTranslation();
  const [form] = Form.useForm<CreateTeamRequest>();
  const { message } = App.useApp();
  const { create } = useTeam();

  // ── Create team ──
  const handleCreate = async () => {
    try {
      const values = await form.validateFields();
      await create.mutateAsync(values);
      message.success(t("team.create.success"));
      onClose();
      form.resetFields();
    } catch (error) {
      message.error(getTranslatedErrorMessage(error));
    }
  };

  return (
    <AIModal
      title={t("team.create.title")}
      open={isOpen}
      onOk={handleCreate}
      onCancel={() => {
        onClose();
        form.resetFields();
      }}
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
          label={t("team.create.name.default")}
          rules={[
            {
              required: true,
              message: t("team.create.name.required"),
            },
          ]}
        >
          <Input placeholder={t("team.create.name.placeholder")} />
        </Form.Item>
        <Form.Item
          name="description"
          label={t("team.create.description.default")}
        >
          <Input.TextArea
            placeholder={t("team.create.description.placeholder")}
            rows={3}
          />
        </Form.Item>
      </Form>
    </AIModal>
  );
}
