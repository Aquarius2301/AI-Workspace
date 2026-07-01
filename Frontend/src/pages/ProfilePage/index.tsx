import { useRef, useState } from "react";
import {
  App,
  Button,
  Card,
  Descriptions,
  Flex,
  Form,
  Grid,
  Input,
  Select,
  Skeleton,
  Typography,
  theme,
} from "antd";
import { CameraOutlined } from "@ant-design/icons";
import { useTranslation } from "react-i18next";
import { UserAvatar } from "@/components";
import { AUTH_ME_QUERY_KEY, useGetCacheData, useUser } from "@/hooks";
import { AIModal } from "@/components";
import { AppLayout } from "@/layouts";
import type { AuthResponse } from "@/types";

const { useBreakpoint } = Grid;
const { Title, Text } = Typography;

export default function ProfilePage() {
  const { t } = useTranslation();
  const { token } = theme.useToken();
  const { message } = App.useApp();
  const screens = useBreakpoint();
  const isMobile = !screens.md;

  const [modalOpen, setModalOpen] = useState(false);
  const [form] = Form.useForm();

  const me = useGetCacheData<AuthResponse>(AUTH_ME_QUERY_KEY);

  const { mutateAsync: updateProfile, isPending: isUpdatePending } =
    useUser().updateProfile;
  const { mutateAsync: uploadAvatar, isPending: isUploadPending } =
    useUser().uploadAvatar;
  const isPending = isUpdatePending || isUploadPending;

  const fileInputRef = useRef<HTMLInputElement>(null);
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [previewUrl, setPreviewUrl] = useState("");

  const breadcrumbItems = [{ title: t("profile.title") }];

  const handleOpenModal = () => {
    form.setFieldsValue({
      name: me?.name ?? "",
      language: me?.language ?? "vi",
    });
    // Reset file state
    if (previewUrl) URL.revokeObjectURL(previewUrl);
    setSelectedFile(null);
    setPreviewUrl("");
    if (fileInputRef.current) fileInputRef.current.value = "";
    setModalOpen(true);
  };

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

  const handleCloseModal = () => {
    setModalOpen(false);
    if (previewUrl) URL.revokeObjectURL(previewUrl);
    setSelectedFile(null);
    setPreviewUrl("");
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      let avatarUrl = me?.avatar;

      // Phase 1: upload file if user selected a new one
      if (selectedFile) {
        try {
          const result = await uploadAvatar(selectedFile);
          avatarUrl = result.avatarUrl;
        } catch {
          message.error(t("profile.avatarUploadError"));
          return;
        }
      }

      // Phase 2: update profile
      await updateProfile({ ...values, avatarUrl });
      message.success(t("profile.updateSuccess"));
      setModalOpen(false);
      if (previewUrl) URL.revokeObjectURL(previewUrl);
      setSelectedFile(null);
      setPreviewUrl("");
    } catch (error) {
      if (error && typeof error === "object" && "errorFields" in error) {
        return;
      }
      message.error(t("profile.updateError"));
    }
  };

  return (
    <AppLayout breadcrumbItems={breadcrumbItems}>
      <Flex vertical gap={24} style={{ maxWidth: 720, margin: "0 auto" }}>
        {/* Profile Card */}
        <Card
          styles={{
            body: {
              padding: isMobile ? 24 : 32,
            },
          }}
        >
          {!me ? (
            <Skeleton active avatar paragraph={{ rows: 3 }} />
          ) : (
            <Flex
              vertical={isMobile}
              align={isMobile ? "center" : "flex-start"}
              gap={isMobile ? 20 : 0}
            >
              {/* Avatar + basic info */}
              <Flex
                vertical={isMobile}
                align={isMobile ? "center" : "flex-start"}
                gap={24}
                style={{ width: "100%" }}
              >
                <Flex
                  vertical={isMobile}
                  align={isMobile ? "center" : "flex-start"}
                  gap={isMobile ? 12 : 24}
                  style={{ width: "100%" }}
                >
                  <UserAvatar
                    src={me.avatar}
                    userName={me.name}
                    size={isMobile ? 80 : 100}
                  />

                  <Flex
                    vertical
                    align={isMobile ? "center" : "flex-start"}
                    gap={4}
                  >
                    <Title
                      level={4}
                      style={{
                        margin: 0,
                        fontWeight: 600,
                        textAlign: isMobile ? "center" : "left",
                      }}
                    >
                      {me.name}
                    </Title>
                    <Text type="secondary" style={{ fontSize: 14 }}>
                      {me.email}
                    </Text>
                    <Text
                      style={{
                        fontSize: 13,
                        color: token.colorPrimary,
                        marginTop: 2,
                      }}
                    >
                      {me.language === "vi" ? "Tiếng Việt" : "English"}
                    </Text>
                  </Flex>
                </Flex>

                {/* Edit Button */}
                <Button
                  type="primary"
                  onClick={handleOpenModal}
                  style={{ marginTop: isMobile ? 8 : 0 }}
                >
                  {t("profile.edit")}
                </Button>
              </Flex>
            </Flex>
          )}
        </Card>

        {/* Detail Info Card */}
        {me && (
          <Card
            title={
              <Text strong style={{ fontSize: 15 }}>
                {t("profile.details")}
              </Text>
            }
            styles={{
              body: {
                padding: isMobile ? 20 : 24,
              },
            }}
          >
            <Descriptions
              column={1}
              size="large"
              labelStyle={{
                fontWeight: 500,
                color: token.colorTextSecondary,
              }}
            >
              <Descriptions.Item label={t("profile.name")}>
                {me.name}
              </Descriptions.Item>
              <Descriptions.Item label={t("profile.email")}>
                {me.email}
              </Descriptions.Item>
              <Descriptions.Item label={t("profile.language")}>
                {me.language === "vi" ? "Tiếng Việt" : "English"}
              </Descriptions.Item>
            </Descriptions>
          </Card>
        )}
      </Flex>

      {/* Edit Modal */}
      <AIModal
        open={modalOpen}
        onCancel={handleCloseModal}
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
            style={{ cursor: "pointer", display: "inline-block", position: "relative" }}
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

        <Form form={form} layout="vertical" size="large" requiredMark={false}>
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
    </AppLayout>
  );
}
