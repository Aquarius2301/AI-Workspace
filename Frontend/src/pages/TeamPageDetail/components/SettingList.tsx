import { useTranslation } from "react-i18next";
import { Button, Card, Space, Row, Col, Typography, Divider } from "antd";
import { useState } from "react";
import { DeleteOutlined, EditOutlined } from "@ant-design/icons";
import { SettingOutlined } from "@ant-design/icons";
import type { TeamRole } from "@/types";
import { DeleteTeamModal, EditTeamModal } from "../modals";

const { Title, Text } = Typography;

interface SettingListProps {
  teamId: string;
  teamName: string;
  teamDescription: string;
  role: TeamRole;
}

type ModalItems = "none" | "delete" | "edit";

export function SettingList({
  teamId,
  teamName,
  teamDescription,
  role,
}: SettingListProps) {
  const { t } = useTranslation();
  const [isModalOpen, setIsModalOpen] = useState<ModalItems>("none");

  return (
    <div style={{ padding: "8px 0" }}>
      <div style={{ marginBottom: 32 }}>
        <Title level={3} style={{ margin: 0 }}>
          <SettingOutlined style={{ marginRight: 12, color: "#1677ff" }} />
          {t("team.teamSettings")}
        </Title>
        <Text
          type="secondary"
          style={{ fontSize: 15, marginTop: 8, display: "block" }}
        >
          {t("team.teamSettingsDesc", { teamName })}
        </Text>
      </div>
      {role == "Admin" && (
        <Row gutter={[24, 24]}>
          <Col sm={24} lg={12}>
            <Card>
              <Space vertical size={20} style={{ width: "100%" }}>
                <Space align="start" size={16} style={{ width: "100%" }}>
                  <EditOutlined />
                  <div style={{ flex: 1 }}>
                    <Title level={5} style={{ margin: 0, marginBottom: 4 }}>
                      {t("team.editTeam")}
                    </Title>
                    <Text type="secondary">{t("team.editTeamDesc")}</Text>
                  </div>
                </Space>

                <Divider style={{ margin: 0 }} />

                <Button
                  type="primary"
                  icon={<EditOutlined />}
                  block
                  size="large"
                  onClick={() => setIsModalOpen("edit")}
                >
                  {t("team.editTeamButton")}
                </Button>
              </Space>
            </Card>
          </Col>

          <Col sm={24} lg={12}>
            <Card style={{ width: "100%" }}>
              <Space vertical size={20} style={{ width: "100%" }}>
                <Space align="start" size={16} style={{ width: "100%" }}>
                  <DeleteOutlined />

                  <div style={{ flex: 1 }}>
                    <Title level={5} style={{ margin: 0, marginBottom: 4 }}>
                      {t("team.deleteTeam")}
                    </Title>
                    <Text type="secondary">{t("team.deleteTeamDesc")}</Text>
                  </div>
                </Space>

                <Divider style={{ margin: 0 }} />

                <Button
                  type="primary"
                  icon={<DeleteOutlined />}
                  block
                  size="large"
                  danger
                  onClick={() => setIsModalOpen("delete")}
                >
                  {t("team.deleteTeamButton")}
                </Button>
              </Space>
            </Card>
          </Col>
        </Row>
      )}

      <DeleteTeamModal
        teamId={teamId}
        teamName={teamName}
        isOpen={isModalOpen == "delete"}
        onClose={() => setIsModalOpen("none")}
      />

      <EditTeamModal
        teamId={teamId}
        teamName={teamName}
        teamDescription={teamDescription}
        isOpen={isModalOpen == "edit"}
        onClose={() => setIsModalOpen("none")}
      />
    </div>
  );
}
