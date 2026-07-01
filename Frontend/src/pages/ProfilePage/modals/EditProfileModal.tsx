import { useEffect, useRef, useState } from "react";
import { App, Form, Input, Select, Typography, theme } from "antd";
import { CameraOutlined } from "@ant-design/icons";
import { useTranslation } from "react-i18next";
import { UserAvatar, AIModal } from "@/components";
import { AUTH_ME_QUERY_KEY, useGetCacheData, useUser, useUpload } from "@/hooks";
import type { AuthResponse } from "@/types";

const { Text } = Typography;

interface EditProfileModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export function EditProfileModal({ isOpen, onClose }: EditProfileModalProps) {
  const { t } = useTranslation();
  const { token } = theme.useToken();
  const { message } = App.useApp();
  const [form] = Form.useForm();

  const me = useGetCacheData<AuthResponse>(AUTH_ME_QUERY_KEY);

  const { mutateAsync: updateProfile, isPending: isUpdatePending } =
    useUser().updateProfile;
  const { mutateAsync: uploadPicture, isPending: isUploadPending } =
    useUpload().uploadPicture;
  const isPending = isUpdatePending || isUploadPending;

  const fileInputRef = useRef<HTMLInputElement>(null);
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [previewUrl, setPreviewUrl] = useState("");

  // Populate form fields when modal opens
  useEffect(() => {
    if (isOpen) {
      form.setFieldsValue({
        name: me?.name ?? "",
        language: me?.language ?? "vi",
      });
      // Reset file state
      if (previewUrl) URL.revokeObjectURL(previewUrl);
      setSelectedFile(null);
      setPreviewUrl("");
      if (fileInputRef.current) fileInputRef.current.value = "";
    }
  }, [isOpen]);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    const allowedTypes = ["image/jpeg", "image/png", "image/webp", "image/gif"];
    if (!allowedTypes.includes(file.type)) {
      message.error(t("profile.avatarTypeError"));
      if (fileInputRef.current) fileInputRef.current.value = "";
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      message.error(t("profile.avatarSizeError"));
      if (fileInputRef.current) fileInputRef.current.value = "";
      return;
    }

    if (previewUrl) URL.revokeObjectURL(previewUrl);
    setSelectedFile(file);
    setPreviewUrl(URL.createObjectURL(file));
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      let avatarUrl = me?.avatar;

      // Phase 1: upload file if user selected a new one
      if (selectedFile) {
        try {
          avatarUrl = await uploadPicture(selectedFile);
        } catch {
          message.error(t("profile.avatarUploadError"));
          return;
        }
      }

      // Phase 2: update profile
      await updateProfile({ ...values, avatarUrl });
      message.success(t("profile.updateSuccess"));
      handleClose();
    } catch (error) {
      if (error && typeof error === "object" && "errorFields" in error) {
        return;
      }
      message.error(t("profile.updateError"));
    }
  };

  const handleClose = () => {
    if (previewUrl) URL.revokeObjectURL(previewUrl);
    setSelectedFile(null);
    setPreviewUrl("");
    if (fileInputRef.current) fileInputRef.current.value = "";
    form.resetFields();
    onClose();
  };

  return (
    <AIModal
      open={isOpen}
      onCancel={handleClose}
      title={t("profile.edit")}
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
      {/* Clickable avatar with camera overlay */}
      <div style={{ textAlign: "center", marginBottom: 24 }}>
        <div
          onClick={() => fileInputRef.current?.click()}
          style={{
            cursor: "pointer",
            display: "inline-block",
            position: "relative",
          }}
        >
          <UserAvatar
            src={previewUrl || me?.avatar}
            userName={me?.name}
            size={80}
          />
          <div
            style={{
              position: "absolute",
              bottom: 0,
              right: 0,
              background: token.colorPrimary,
              borderRadius: "50%",
              width: 28,
              height: 28,
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              border: `2px solid ${token.colorBgContainer}`,
            }}
          >
            <CameraOutlined style={{ color: "#fff", fontSize: 14 }} />
          </div>
        </div>
        <input
          ref={fileInputRef}
          type="file"
          accept="image/jpeg,image/png,image/webp,image/gif"
          hidden
          onChange={handleFileChange}
        />
        <div style={{ marginTop: 8 }}>
          <Text type="secondary" style={{ fontSize: 13 }}>
            {t("profile.avatar")}
          </Text>
        </div>
      </div>

      <Form
        form={form}
        layout="vertical"
        size="large"
        requiredMark={false}
        disabled={isPending}
      >
        <Form.Item
          name="name"
          label={t("profile.name")}
          rules={[{ required: true, message: t("profile.nameRequired") }]}
        >
          <Input placeholder={t("profile.name")} />
        </Form.Item>

        <Form.Item name="language" label={t("profile.language")}>
          <Select
            options={[
              { value: "vi", label: "Tiếng Việt" },
              { value: "en", label: "English" },
            ]}
          />
        </Form.Item>
      </Form>
    </AIModal>
  );
}
