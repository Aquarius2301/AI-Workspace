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
  DeleteOutlined,
  DownOutlined,
} from "@ant-design/icons";
import { EditTeamModal } from "../modals/EditTeamModal";
import { DeleteTeamModal } from "../modals/DeleteTeamModal";

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
  const [editModalOpen, setEditModalOpen] = useState(false);
  const [deleteModalOpen, setDeleteModalOpen] = useState(false);

  const menu: MenuProps["items"] = [
    {
      key: "edit",
      icon: <EditOutlined />,
      label: t("teamDetailPage.editTeam.title"),
      onClick: () => setEditModalOpen(true),
    },
    {
      key: "delete",
      icon: <DeleteOutlined />,
      danger: true,
      label: t("teamDetailPage.deleteTeam.menu"),
      onClick: () => setDeleteModalOpen(true),
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
        isOpen={editModalOpen}
        onClose={() => setEditModalOpen(false)}
        teamId={teamId}
        initialName={name}
        initialDescription={description}
      />
      <DeleteTeamModal
        isOpen={deleteModalOpen}
        onClose={() => setDeleteModalOpen(false)}
        teamId={teamId}
        teamName={name}
      />
    </>
  );
}
