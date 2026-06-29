import { Form, App, Input } from "antd";
import { useTranslation } from "react-i18next";
import { AIModal } from "@/components";
import { useTeam } from "@/hooks";
import type { UpdateTeamRequest } from "@/types";
import { getTranslatedErrorMessage } from "@/utils";

interface EditTeamModalProps {
  isOpen: boolean;
  onClose: () => void;
  teamId: string;
  initialName: string;
  initialDescription?: string;
}

export function EditTeamModal({
  isOpen,
  onClose,
  teamId,
  initialName,
  initialDescription,
}: EditTeamModalProps) {
  const { t } = useTranslation();
  const [form] = Form.useForm<UpdateTeamRequest>();
  const { message } = App.useApp();
  const { update } = useTeam();

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      await update.mutateAsync({ id: teamId, data: values });
      message.success(t("teamDetail.editTeam.success"));
      onClose();
    } catch (error) {
      message.error(getTranslatedErrorMessage(error));
    }
  };

  return (
    <AIModal
      title={t("teamDetail.editTeam.title")}
      open={isOpen}
      onOk={handleSave}
      onCancel={onClose}
      isLoading={update.isPending}
      footer={[{ type: "cancel" }, { type: "update" }]}
    >
      <Form
        form={form}
        layout="vertical"
        autoComplete="off"
        style={{ marginTop: 16 }}
        initialValues={{
          name: initialName,
          description: initialDescription,
        }}
      >
        <Form.Item
          name="name"
          label={t("teamDetail.editTeam.name.default")}
          rules={[
            {
              required: true,
              message: t("teamDetail.editTeam.name.required"),
            },
          ]}
        >
          <Input placeholder={initialName} />
        </Form.Item>
        <Form.Item
          name="description"
          label={t("teamDetail.editTeam.description.default")}
        >
          <Input.TextArea
            placeholder={t("teamDetail.editTeam.description.placeholder")}
            rows={3}
          />
        </Form.Item>
      </Form>
    </AIModal>
  );
}
