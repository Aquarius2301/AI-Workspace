import { Form, App, Input } from "antd";
import { useTranslation } from "react-i18next";
import { AIModal } from "@/components";
import { useUpdateTeam } from "@/hooks";
import type { UpdateTeamRequest } from "@/types";
import { getErrorMessage, getFormFieldErrors } from "@/utils";

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
  const update = useUpdateTeam();

  const handleSave = async (values: UpdateTeamRequest) => {
    try {
      await update.mutateAsync({ id: teamId, data: values });
      message.success(t("teamDetailPage.editTeam.success"));
      onClose();
    } catch (error) {
      const fieldErrors = getFormFieldErrors<UpdateTeamRequest>(error);
      if (fieldErrors.length > 0) {
        form.setFields(fieldErrors);
      } else {
        message.error(getErrorMessage(error));
      }
    }
  };

  const name = Form.useWatch("name", form);
  const description = Form.useWatch("description", form);

  const isChanged =
    (name?.trim() ?? "") !== (initialName ?? "") ||
    (description?.trim() ?? "") !== (initialDescription ?? "");

  return (
    <AIModal
      title={t("teamDetailPage.editTeam.title")}
      open={isOpen}
      onOk={form.submit}
      onCancel={onClose}
      isLoading={update.isPending}
      footer={[{ type: "cancel" }, { type: "update", disabled: !isChanged }]}
    >
      <Form
        form={form}
        layout="vertical"
        autoComplete="off"
        style={{ marginTop: 16 }}
        onFinish={handleSave}
        initialValues={{
          name: initialName,
          description: initialDescription,
        }}
      >
        <Form.Item
          name="name"
          label={t("teamDetailPage.editTeam.name.title")}
          rules={[
            {
              required: true,
              message: t("teamDetailPage.editTeam.name.required"),
            },
            {
              max: 100,
              message: t("teamDetailPage.editTeam.name.max"),
            },
          ]}
        >
          <Input placeholder={initialName} />
        </Form.Item>
        <Form.Item
          name="description"
          label={t("teamDetailPage.editTeam.description.title")}
          rules={[
            {
              max: 500,
              message: t("teamDetailPage.editTeam.desctiption.max"),
            },
          ]}
        >
          <Input.TextArea
            placeholder={t("teamDetailPage.editTeam.description.placeholder")}
            rows={3}
          />
        </Form.Item>
      </Form>
    </AIModal>
  );
}
