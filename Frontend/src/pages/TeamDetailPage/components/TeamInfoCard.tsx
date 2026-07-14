import { useState } from "react";
import {
  Button,
  Dropdown,
  Flex,
  Space,
  Typography,
  type MenuProps,
} from "antd";
import { useTranslation } from "react-i18next";
import { AICardItem, AITeamRoleTag } from "@/components";
import type { TeamRole } from "@/types";
import {
  EditOutlined,
  EllipsisOutlined,
  DownOutlined,
} from "@ant-design/icons";
import { EditTeamModal } from "../modals/EditTeamModal";

const { Text, Paragraph } = Typography;

interface TeamInfoCardProps {
  teamId: string;
  name: string;
  role: TeamRole;
  description?: string;
  isLoading?: boolean;
}

export function TeamInfoCard({
  teamId,
  name,
  role,
  description,
  isLoading = false,
}: TeamInfoCardProps) {
  const { t } = useTranslation();
  const [modalOpen, setModalOpen] = useState(false);

  const menu: MenuProps["items"] = [
    {
      key: "edit",
      icon: <EditOutlined />,
      label: t("teamDetailPage.editTeam.title"),
      onClick: () => setModalOpen(true),
    },
  ];

  return (
    <>
      <AICardItem
        isLoading={isLoading}
        header={
          <Flex justify="space-between">
            <Space>
              <Text strong style={{ fontSize: 18 }}>
                {name}
              </Text>
              <AITeamRoleTag role={role} />
            </Space>
            {role == "admin" && (
              <Dropdown menu={{ items: menu }}>
                <Button type="text">
                  <EllipsisOutlined />
                </Button>
              </Dropdown>
            )}
          </Flex>
        }
        content={
          description && (
            <Paragraph
              type="secondary"
              style={{ margin: 0, fontSize: 14 }}
              ellipsis={{
                rows: 1,
                expandable: true,
                symbol: <DownOutlined />,
              }}
            >
              {description}
            </Paragraph>
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
    </>
  );
}
