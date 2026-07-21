import { Form, App, Input, Checkbox } from "antd";
import { useTranslation } from "react-i18next";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { AIModal, AIVisibilitySelect } from "@/components";
import type { CreateProjectRequest } from "@/types";
import { PROJECT_VISIBILIES } from "@/types";
import { ROUTE } from "@/constants";
import { useCreateProject } from "@/hooks";
import { getErrorMessage, getFormFieldErrors } from "@/utils";
import Text from "antd/es/typography/Text";

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
  const [isRedirect, setIsRedirect] = useState(false);
  const { message } = App.useApp();
  const create = useCreateProject();
  const navigate = useNavigate();

  // ── Create project ──
  const handleCreate = async (values: CreateProjectRequest) => {
    try {
      const res = await create.mutateAsync({
        ...values,
        teamId,
      });
      if (isRedirect) {
        navigate(`${ROUTE.PROJECT}/${res.slug}`);
      }
      message.success(t("teamDetailPage.projects.createProject.success"));
      onClose();
      form.resetFields();
    } catch (error) {
      const fieldErrors = getFormFieldErrors<CreateProjectRequest>(error);
      if (fieldErrors.length > 0) {
        form.setFields(fieldErrors);
      } else {
        message.error(getErrorMessage(error));
      }
    }
  };

  return (
    <AIModal
      title={t("teamDetailPage.projects.createProject.title")}
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
          label={t("teamDetailPage.projects.createProject.name.title")}
          rules={[
            {
              required: true,
              message: t("teamDetailPage.projects.createProject.name.required"),
            },
            {
              max: 100,
              message: t("teamDetailPage.projects.createProject.name.max"),
            },
          ]}
        >
          <Input
            placeholder={t(
              "teamDetailPage.projects.createProject.name.placeholder",
            )}
          />
        </Form.Item>

        <Form.Item
          name="description"
          label={t("teamDetailPage.projects.createProject.description.title")}
          rules={[
            {
              max: 500,
              message: t(
                "teamDetailPage.projects.createProject.description.max",
              ),
            },
          ]}
        >
          <Input.TextArea
            placeholder={t(
              "teamDetailPage.projects.createProject.description.placeholder",
            )}
            rows={3}
          />
        </Form.Item>

        <Form.Item
          name="visibility"
          label={t("teamDetailPage.projects.createProject.visibility.title")}
          rules={[
            {
              required: true,
              message: t("teamDetailPage.projects.createProject.name.required"),
            },
          ]}
          initialValue={PROJECT_VISIBILIES[0]}
        >
          <AIVisibilitySelect style={{ width: "100%" }} />
        </Form.Item>

        <Checkbox
          value={isRedirect}
          onChange={(e) => setIsRedirect(e.target.checked)}
        >
          <Text>{t("teamDetailPage.projects.createProject.goToProject")}</Text>
        </Checkbox>
      </Form>
    </AIModal>
  );
}
