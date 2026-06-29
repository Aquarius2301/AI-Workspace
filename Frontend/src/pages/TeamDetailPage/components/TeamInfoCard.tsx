import { useState } from "react";
import { Button, Space, Typography } from "antd";
import { useTranslation } from "react-i18next";
import { AICardItem, AIRoleTag } from "@/components";
import type { TeamRole } from "@/types";
import { EditTeamModal } from "../modals/EditTeamModal";
import { EditOutlined } from "@ant-design/icons";

const { Text, Paragraph } = Typography;

interface TeamInfoCardProps {
  teamId: string;
  name: string;
  role: TeamRole;
  description?: string;
  currentUserRole?: TeamRole;
}

export function TeamInfoCard({
  teamId,
  name,
  role,
  description,
  currentUserRole,
}: TeamInfoCardProps) {
  const { t } = useTranslation();
  const [modalOpen, setModalOpen] = useState(false);

  const isAdmin = currentUserRole === "Admin";

  return (
    <div>
      <AICardItem
        style={{ marginBottom: 0 }}
        header={
          <Space>
            <Text strong style={{ fontSize: 18 }}>
              {name}
            </Text>
            <AIRoleTag role={role} />
          </Space>
        }
        content={
          description && (
            <Paragraph
              type="secondary"
              style={{ margin: 0, fontSize: 14 }}
              ellipsis={{
                rows: 2,
                expandable: true,
                symbol: t("common.showMore"),
              }}
            >
              {description}
            </Paragraph>
          )
        }
        rightSideSpan={5}
        rightSide={
          isAdmin && (
            <Button onClick={() => setModalOpen(true)}>
              <Text type="secondary">
                <EditOutlined /> {t("teamDetail.editTeam.title")}
              </Text>
            </Button>
          )
        }
      />

      <EditTeamModal
        isOpen={modalOpen}
        onClose={() => setModalOpen(false)}
        teamId={teamId}
        initialName={name}
        initialDescription={description}
      />
    </div>
  );
}
