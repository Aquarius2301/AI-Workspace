import { useRef, useState } from "react";
import { Flex, Typography, Button, Tag, Empty, theme, App } from "antd";
import {
  LogoutOutlined,
  LaptopOutlined,
  GlobalOutlined,
  ClockCircleOutlined,
  EditOutlined,
  CameraOutlined,
  LoadingOutlined,
  WindowsOutlined,
  AppleOutlined,
  AndroidOutlined,
  DesktopOutlined,
  QuestionCircleOutlined,
} from "@ant-design/icons";
import { useTranslation } from "react-i18next";
import { AICard, UserAvatar, AICardItem, AIList } from "@/components";
import { AppLayout } from "@/layouts";
import { useSessionsQuery, useUploadPicture, useUpdateProfile } from "@/hooks";
import { useAuthMe } from "@/hooks/api/useAuth.hook";
import type { SessionResult } from "@/types";
import { formatIsoLocaleDate, getErrorMessage, parseUserAgent } from "@/utils";
import { compressImage, isValidImageType } from "@/utils";
import {
  RevokeSessionModal,
  RevokeAllSessionsModal,
  UpdateProfileModal,
} from "./modals";

const { Text, Title } = Typography;

type ModalType = "revokeTarget" | "revokeAll" | "updateProfile";

/** Map OS name → Ant Design icon component */
function getOsIcon(os: string) {
  switch (os) {
    case "Windows":
      return <WindowsOutlined />;
    case "macOS":
    case "iOS":
      return <AppleOutlined />;
    case "Android":
      return <AndroidOutlined />;
    case "Linux":
      return <DesktopOutlined />;
    default:
      return <QuestionCircleOutlined />;
  }
}

export default function ProfilePage() {
  const { t } = useTranslation();
  const { token } = theme.useToken();
  const { message } = App.useApp();

  const { data: userInfo } = useAuthMe();

  // Fetch sessions
  const { data: sessions, isLoading: sessionsLoading } = useSessionsQuery();

  // Mutations
  const uploadPicture = useUploadPicture();
  const updateProfile = useUpdateProfile();

  // Modal states
  const [modal, setModal] = useState<ModalType | null>(null);

  const [revokeTarget, setRevokeTarget] = useState<string | null>(null);

  // Avatar upload
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [avatarUploading, setAvatarUploading] = useState(false);

  const hasMultipleSessions = (sessions?.length ?? 0) > 1;

  const handleAvatarClick = () => {
    fileInputRef.current?.click();
  };

  const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    e.target.value = ""; // reset so same file can be re-selected

    // Validate type (with fallback for empty MIME)
    if (!isValidImageType(file)) {
      message.error(t("profilePage.uploadAvatar.invalidType"));
      return;
    }

    setAvatarUploading(true);

    try {
      // Compress if needed (>3MB → compress to ensure <5MB)
      const { file: compressedFile } =
        file.size > 3 * 1024 * 1024 ? await compressImage(file) : { file };

      // Upload picture → receive { fileId, url }
      const uploadResult = await uploadPicture.mutateAsync(compressedFile);

      // Ensure fileId is present
      if (!uploadResult?.fileId) {
        throw new Error("Upload failed: no fileId returned");
      }

      // Update profile with new avatar
      await updateProfile.mutateAsync({
        fileId: uploadResult.fileId,
      });

      message.success(t("profilePage.uploadAvatar.success"));
    } catch (ex) {
      message.error(getErrorMessage(ex));
    } finally {
      setAvatarUploading(false);
    }
  };

  const breadcrumbItems = [{ title: t("profilePage.title") }];

  const languageDisplay =
    userInfo?.language === "vi"
      ? t("loginPage.language.vietnamese")
      : t("loginPage.language.english");

  const isUploading =
    avatarUploading || uploadPicture.isPending || updateProfile.isPending;

  return (
    <AppLayout breadcrumbItems={breadcrumbItems} title={t("profilePage.title")}>
      <Flex vertical gap={24} style={{ maxWidth: 900, margin: "0 auto" }}>
        {/* ── User Info Card ── */}
        <AICard
          title={t("profilePage.userInfo.title")}
          extra={
            <Button
              type="text"
              icon={<EditOutlined />}
              onClick={() => setModal("updateProfile")}
              style={{ fontWeight: 500 }}
            >
              {t("profilePage.editProfile")}
            </Button>
          }
        >
          <Flex align="center" gap={20} wrap>
            {/* Avatar với overlay camera */}
            <div
              style={{
                position: "relative",
                cursor: "pointer",
                flexShrink: 0,
              }}
              onClick={handleAvatarClick}
            >
              <UserAvatar
                userId={userInfo?.id}
                userName={userInfo?.name}
                src={userInfo?.avatarUrl}
                size={80}
              />
              {/* Camera icon overlay */}
              <div
                style={{
                  position: "absolute",
                  bottom: 0,
                  right: 0,
                  width: 28,
                  height: 28,
                  borderRadius: "50%",
                  background: token.colorPrimary,
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  border: `2px solid ${token.colorBgContainer}`,
                }}
              >
                {isUploading ? (
                  <LoadingOutlined style={{ color: "#fff", fontSize: 14 }} />
                ) : (
                  <CameraOutlined style={{ color: "#fff", fontSize: 14 }} />
                )}
              </div>
            </div>

            <input
              ref={fileInputRef}
              type="file"
              accept="image/jpeg,image/png,image/gif,image/webp"
              style={{ display: "none" }}
              onChange={handleFileChange}
            />

            <Flex vertical gap={4}>
              <Title level={4} style={{ margin: 0 }}>
                {userInfo?.name ?? "—"}
              </Title>
              <Text type="secondary" style={{ fontSize: 15 }}>
                {userInfo?.email ?? "—"}
              </Text>
              <Flex align="center" gap={6}>
                <GlobalOutlined
                  style={{ color: token.colorTextTertiary, fontSize: 13 }}
                />
                <Text type="secondary" style={{ fontSize: 13 }}>
                  {t("profilePage.userInfo.language")}: {languageDisplay}
                </Text>
              </Flex>
              <Text type="secondary" style={{ fontSize: 12 }}>
                {t("profilePage.helper")}
              </Text>
            </Flex>
          </Flex>
        </AICard>

        {/* ── Sessions Card ── */}
        <AICard
          title={
            <Flex align="center" gap={8}>
              <LaptopOutlined />
              <span>{t("profilePage.sessions.title")}</span>
            </Flex>
          }
          extra={
            hasMultipleSessions && (
              <Button
                type="text"
                danger
                icon={<LogoutOutlined />}
                onClick={() => setModal("revokeAll")}
                style={{ fontWeight: 500 }}
              >
                {t("profilePage.sessions.revokeAll.title")}
              </Button>
            )
          }
        >
          <AIList<SessionResult>
            data={sessions ?? []}
            itemKey={(session) => session.deviceId}
            isLoading={sessionsLoading}
            empty={{
              icon: Empty.PRESENTED_IMAGE_SIMPLE,
              title: t("list.empty"),
            }}
            renderItem={(session) => {
              const parsed = session.deviceInfo
                ? parseUserAgent(session.deviceInfo)
                : null;
              const deviceName = parsed
                ? t("profilePage.sessions.deviceInfo", {
                    browser: parsed.browser,
                    os: parsed.os,
                  })
                : t("profilePage.sessions.unknownDevice");

              return (
                <AICardItem
                  header={
                    <Flex align="center" gap={8} wrap>
                      {parsed && getOsIcon(parsed.os)}
                      <Text strong>{deviceName}</Text>
                      {session.isCurrent && (
                        <Tag color="green" style={{ margin: 0 }}>
                          {t("profilePage.sessions.currentSession")}
                        </Tag>
                      )}
                    </Flex>
                  }
                  content={
                    <Flex vertical gap={4}>
                      <Flex align="center" gap={6}>
                        <ClockCircleOutlined
                          style={{
                            color: token.colorTextTertiary,
                            fontSize: 12,
                          }}
                        />
                        <Text type="secondary" style={{ fontSize: 13 }}>
                          {t("profilePage.sessions.createdAt")}:{" "}
                          {formatIsoLocaleDate(session.createdAt, true)}
                        </Text>
                      </Flex>
                      <Flex align="center" gap={6}>
                        <ClockCircleOutlined
                          style={{
                            color: token.colorTextTertiary,
                            fontSize: 12,
                          }}
                        />
                        <Text type="secondary" style={{ fontSize: 13 }}>
                          {t("profilePage.sessions.expiresAt")}:{" "}
                          {formatIsoLocaleDate(session.expiresAt, true)}
                        </Text>
                      </Flex>
                    </Flex>
                  }
                  rightSide={
                    !session.isCurrent && (
                      <Button
                        type="text"
                        danger
                        size="small"
                        onClick={() => {
                          setModal("revokeTarget");
                          setRevokeTarget(session.deviceId);
                        }}
                      >
                        {t("profilePage.sessions.revoke.title")}
                      </Button>
                    )
                  }
                />
              );
            }}
          />
        </AICard>
      </Flex>

      {/* ── Modals ── */}
      <RevokeSessionModal
        onClose={() => {
          setModal(null);
          setRevokeTarget(null);
        }}
        deviceId={revokeTarget ?? ""}
        isOpen={modal == "revokeTarget"}
      />

      <RevokeAllSessionsModal
        onClose={() => setModal(null)}
        isOpen={modal == "revokeAll"}
      />

      <UpdateProfileModal
        onClose={() => setModal(null)}
        isOpen={modal == "updateProfile"}
      />
    </AppLayout>
  );
}
