import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { Form, Input, Select, App } from "antd";
import { AIModal } from "@/components";
import { useUpdateProfile } from "@/hooks";
import { useAuthMe } from "@/hooks/api/useAuth.hook";
import type { LanguageDisplay } from "@/types";
import { getErrorMessage, getFormFieldErrors } from "@/utils";
import { LANGUAGE_DISPLAY } from "@/types";

interface UpdateProfileFormValues {
  name: string;
  language: LanguageDisplay;
}

interface UpdateProfileModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export function UpdateProfileModal({
  isOpen,
  onClose,
}: UpdateProfileModalProps) {
  const { t } = useTranslation();
  const { message } = App.useApp();
  const [form] = Form.useForm<UpdateProfileFormValues>();
  const updateProfile = useUpdateProfile();

  const { data: userInfo } = useAuthMe();

  // Pre-fill form when modal opens
  useEffect(() => {
    if (isOpen) {
      form.setFieldsValue({
        name: userInfo?.name ?? "",
        language: userInfo?.language ?? "en",
      });
    }
  }, [isOpen, userInfo, form]);

  const handleUpdate = async (values: UpdateProfileFormValues) => {
    try {
      await updateProfile.mutateAsync({
        name: values.name,
        language: values.language,
      });
      message.success(t("profilePage.updateProfile.success"));
      onClose();
    } catch (error) {
      const fieldErrors = getFormFieldErrors<UpdateProfileFormValues>(error);
      if (fieldErrors.length > 0) {
        form.setFields(fieldErrors);
      } else {
        message.error(getErrorMessage(error));
      }
    }
  };

  return (
    <AIModal
      title={t("profilePage.updateProfile.title")}
      open={isOpen}
      onOk={() => form.submit()}
      onCancel={() => {
        onClose();
        form.resetFields();
      }}
      isLoading={updateProfile.isPending}
      footer={[{ type: "cancel" }, { type: "update" }]}
    >
      <Form
        form={form}
        layout="vertical"
        autoComplete="off"
        onFinish={handleUpdate}
        style={{ marginTop: 16 }}
      >
        <Form.Item
          name="name"
          label={t("profilePage.updateProfile.name.title")}
          rules={[
            {
              required: true,
              message: t("profilePage.updateProfile.name.required"),
            },
            {
              max: 100,
              message: t("profilePage.updateProfile.name.max"),
            },
          ]}
        >
          <Input
            placeholder={t("profilePage.updateProfile.name.placeholder")}
          />
        </Form.Item>

        <Form.Item
          name="language"
          label={t("profilePage.updateProfile.language")}
        >
          <Select>
            {LANGUAGE_DISPLAY.map((lang) => (
              <Select.Option key={lang} value={lang}>
                {lang === "vi"
                  ? t("loginPage.language.vietnamese")
                  : t("loginPage.language.english")}
              </Select.Option>
            ))}
          </Select>
        </Form.Item>
      </Form>
    </AIModal>
  );
}
