import { useState } from "react";
import {
  Button,
  Card,
  Descriptions,
  Flex,
  Grid,
  Skeleton,
  Typography,
  theme,
} from "antd";
import { KeyOutlined } from "@ant-design/icons";
import { useTranslation } from "react-i18next";
import { UserAvatar } from "@/components";
import { AUTH_ME_QUERY_KEY, useGetCacheData } from "@/hooks";
import { AppLayout } from "@/layouts";
import type { AuthResponse } from "@/types";
import { EditProfileModal } from "./modals/EditProfileModal";
import { ChangePasswordModal } from "./modals/ChangePasswordModal";

const { useBreakpoint } = Grid;
const { Title, Text } = Typography;

export default function ProfilePage() {
  const { t } = useTranslation();
  const { token } = theme.useToken();
  const screens = useBreakpoint();
  const isMobile = !screens.md;

  const [editModalOpen, setEditModalOpen] = useState(false);
  const [passwordModalOpen, setPasswordModalOpen] = useState(false);

  const me = useGetCacheData<AuthResponse>(AUTH_ME_QUERY_KEY);

  const breadcrumbItems = [{ title: t("profile.title") }];

  const handleOpenEditModal = () => setEditModalOpen(true);
  const handleCloseEditModal = () => setEditModalOpen(false);
  const handleOpenPasswordModal = () => setPasswordModalOpen(true);
  const handleClosePasswordModal = () => setPasswordModalOpen(false);

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

                <Button
                  type="primary"
                  onClick={handleOpenEditModal}
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
          <>
            <Card
              title={
                <Flex
                  align="center"
                  justify="space-between"
                  style={{ width: "100%" }}
                >
                  <Text strong style={{ fontSize: 15 }}>
                    {t("profile.details")}
                  </Text>
                  <Button
                    type="text"
                    icon={<KeyOutlined />}
                    onClick={handleOpenPasswordModal}
                    size="small"
                  >
                    {t("profile.changePassword")}
                  </Button>
                </Flex>
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
          </>
        )}
      </Flex>

      <EditProfileModal isOpen={editModalOpen} onClose={handleCloseEditModal} />
      <ChangePasswordModal
        isOpen={passwordModalOpen}
        onClose={handleClosePasswordModal}
      />
    </AppLayout>
  );
}
