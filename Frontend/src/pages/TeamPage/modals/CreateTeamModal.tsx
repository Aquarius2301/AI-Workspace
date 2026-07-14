import { AIModal } from "@/components";
import type { CreateTeamRequest } from "@/types";
import { Form, App, Input } from "antd";
import { useTranslation } from "react-i18next";
import { useCreateTeam } from "@/hooks";
import Text from "antd/es/typography/Text";
import { getErrorMessage, getFormFieldErrors } from "@/utils";

interface CreateTeamModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export function CreateTeamModal({ isOpen, onClose }: CreateTeamModalProps) {
  const { t } = useTranslation();
  const [form] = Form.useForm<CreateTeamRequest>();
  const { message } = App.useApp();
  const create = useCreateTeam();

  // ── Create team ──
  const handleCreate = async (values: CreateTeamRequest) => {
    try {
      await create.mutateAsync(values);
      message.success(t("teamPage.createTeam.success"));
      onClose();
      form.resetFields();
    } catch (error) {
      const fieldErrors = getFormFieldErrors<CreateTeamRequest>(error);
      if (fieldErrors.length > 0) {
        form.setFields(fieldErrors);
      } else {
        message.error(getErrorMessage(error));
      }
    }
  };

  return (
    <AIModal
      title={t("teamPage.createTeam.title")}
      open={isOpen}
      onOk={() => form.submit()}
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
        onFinish={handleCreate}
        style={{ marginTop: 16 }}
      >
        <Form.Item
          name="name"
          label={t("teamPage.createTeam.name.title")}
          rules={[
            {
              required: true,
              message: t("teamPage.createTeam.name.required"),
            },
            {
              max: 100,
              message: t("teamPage.createTeam.name.max"),
            },
          ]}
        >
          <Input placeholder={t("teamPage.createTeam.name.placeholder")} />
        </Form.Item>

        <Form.Item
          name="description"
          label={t("teamPage.createTeam.description.title")}
          rules={[
            {
              max: 500,
              message: t("teamPage.createTeam.description.max"),
            },
          ]}
        >
          <Input.TextArea
            placeholder={t("teamPage.createTeam.description.placeholder")}
            rows={3}
          />
        </Form.Item>

        <Text type="secondary"> {t("teamPage.createTeam.helper")}</Text>
      </Form>
    </AIModal>
  );
}
