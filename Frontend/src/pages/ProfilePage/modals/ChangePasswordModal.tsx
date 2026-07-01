import { Form, Input, App } from "antd";
import { useTranslation } from "react-i18next";
import { AIModal } from "@/components";
import { useUser } from "@/hooks";
import { getTranslatedErrorMessage } from "@/utils";

interface ChangePasswordModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export function ChangePasswordModal({
  isOpen,
  onClose,
}: ChangePasswordModalProps) {
  const { t } = useTranslation();
  const { message } = App.useApp();
  const [form] = Form.useForm();

  const { mutateAsync: changePassword, isPending } = useUser().changePassword;

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      await changePassword({
        oldPassword: values.oldPassword,
        newPassword: values.newPassword,
      });
      message.success(t("profile.changePasswordSuccess"));
      handleClose();
    } catch (error) {
      if (error && typeof error === "object" && "errorFields" in error) {
        return;
      }
      message.error(getTranslatedErrorMessage(error));
    }
  };

  const handleClose = () => {
    form.resetFields();
    onClose();
  };

  return (
    <AIModal
      open={isOpen}
      onCancel={handleClose}
      title={t("profile.changePassword")}
      onOk={handleSubmit}
      isLoading={isPending}
      footer={[
        { type: "cancel" },
        {
          type: "update",
          disabled: isPending,
          onClick: handleSubmit,
        },
      ]}
    >
      <Form
        form={form}
        layout="vertical"
        size="large"
        requiredMark={false}
        disabled={isPending}
      >
        <Form.Item
          name="oldPassword"
          label={t("profile.oldPassword")}
          rules={[
            { required: true, message: t("profile.oldPasswordRequired") },
          ]}
        >
          <Input.Password placeholder={t("profile.oldPassword")} />
        </Form.Item>

        <Form.Item
          name="newPassword"
          label={t("profile.newPassword")}
          rules={[
            { required: true, message: t("profile.newPasswordRequired") },
            {
              min: 8,
              message: t("profile.newPasswordLength"),
            },
            {
              pattern: /[A-Z]/,
              message: t("profile.newPasswordUppercase"),
            },
            {
              pattern: /[!@#$%^&*(),.?":{}|<>_\-\\[\]`~;'+=]/,
              message: t("profile.newPasswordSpecialChar"),
            },
          ]}
        >
          <Input.Password placeholder={t("profile.newPassword")} />
        </Form.Item>

        <Form.Item
          name="confirmPassword"
          label={t("profile.confirmPassword")}
          dependencies={["newPassword"]}
          rules={[
            { required: true, message: t("profile.confirmPasswordRequired") },
            ({ getFieldValue }) => ({
              validator(_, value) {
                if (!value || getFieldValue("newPassword") === value) {
                  return Promise.resolve();
                }
                return Promise.reject(
                  new Error(t("profile.confirmPasswordMismatch")),
                );
              },
            }),
          ]}
        >
          <Input.Password placeholder={t("profile.confirmPassword")} />
        </Form.Item>
      </Form>
    </AIModal>
  );
}
