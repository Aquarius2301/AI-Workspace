import { Form, App, Input } from "antd";
import { useTranslation } from "react-i18next";
import { AIModal, AIVisibilitySelect } from "@/components";
import { useUpdateProject } from "@/hooks";
import type { UpdateProjectRequest } from "@/types";
import { getErrorMessage, getFormFieldErrors } from "@/utils";

interface EditProjectModalProps {
  isOpen: boolean;
  onClose: () => void;
  projectId: string;
  initialName: string;
  initialDescription?: string | null;
  initialVisibility: string;
}

export function EditProjectModal({
  isOpen,
  onClose,
  projectId,
  initialName,
  initialDescription,
  initialVisibility,
}: EditProjectModalProps) {
  const { t } = useTranslation();
  const [form] = Form.useForm<UpdateProjectRequest>();
  const { message } = App.useApp();
  const update = useUpdateProject();

  const handleSave = async (values: UpdateProjectRequest) => {
    try {
      await update.mutateAsync({ id: projectId, data: values });
      message.success(t("projectDetailPage.editProject.success"));
      onClose();
    } catch (error) {
      const fieldErrors = getFormFieldErrors<UpdateProjectRequest>(error);
      if (fieldErrors.length > 0) {
        form.setFields(fieldErrors);
      } else {
        message.error(getErrorMessage(error));
      }
    }
  };

  const name = Form.useWatch("name", form);
  const description = Form.useWatch("description", form);
  const visibility = Form.useWatch("visibility", form);

  const isChanged =
    (name?.trim() ?? "") !== (initialName ?? "") ||
    (description?.trim() ?? "") !== (initialDescription ?? "") ||
    (visibility ?? "") !== (initialVisibility ?? "");

  return (
    <AIModal
      title={t("projectDetailPage.editProject.title")}
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
          description: initialDescription ?? undefined,
          visibility: initialVisibility as UpdateProjectRequest["visibility"],
        }}
      >
        <Form.Item
          name="name"
          label={t("projectDetailPage.editProject.name.title")}
          rules={[
            {
              required: true,
              message: t("projectDetailPage.editProject.name.required"),
            },
            {
              max: 100,
              message: t("projectDetailPage.editProject.name.max"),
            },
          ]}
        >
          <Input placeholder={initialName} />
        </Form.Item>
        <Form.Item
          name="description"
          label={t("projectDetailPage.editProject.description.title")}
          rules={[
            {
              max: 500,
              message: t("projectDetailPage.editProject.description.max"),
            },
          ]}
        >
          <Input.TextArea
            placeholder={t(
              "projectDetailPage.editProject.description.placeholder",
            )}
            rows={3}
          />
        </Form.Item>
        <Form.Item
          name="visibility"
          label={t("projectDetailPage.editProject.visibility.title")}
        >
          <AIVisibilitySelect style={{ width: "100%" }} />
        </Form.Item>
      </Form>
    </AIModal>
  );
}
