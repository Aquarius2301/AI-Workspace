import { Flex, Space, Typography } from "antd";
import { EditOutlined, DeleteOutlined } from "@ant-design/icons";
import { useTranslation } from "react-i18next";
import { AICardItem, AIVisibilityTag } from "@/components";
import { Button } from "antd";

const { Text, Paragraph } = Typography;

interface ProjectInfoCardProps {
  name: string;
  description?: string | null;
  visibility: string;
  creatorName: string;
  teamName: string;
  isLoading?: boolean;
  canEdit?: boolean;
  onEdit?: () => void;
  onDelete?: () => void;
}

export function ProjectInfoCard({
  name,
  description,
  visibility,
  creatorName,
  teamName,
  isLoading = false,
  canEdit,
  onEdit,
  onDelete,
}: ProjectInfoCardProps) {
  const { t } = useTranslation();

  return (
    <AICardItem
      isLoading={isLoading}
      header={
        <Flex justify="space-between" align="center">
          <Space>
            <Text strong style={{ fontSize: 18 }}>
              {name}
            </Text>
            <AIVisibilityTag visibility={visibility} />
          </Space>
          {canEdit && (
            <Flex gap={4}>
              {onEdit && (
                <Button type="text" icon={<EditOutlined />} onClick={onEdit} />
              )}
              {onDelete && (
                <Button
                  type="text"
                  danger
                  icon={<DeleteOutlined />}
                  onClick={onDelete}
                />
              )}
            </Flex>
          )}
        </Flex>
      }
      content={
        <Flex vertical gap={4}>
          {description && (
            <Paragraph type="secondary" style={{ margin: 0, fontSize: 14 }}>
              {description}
            </Paragraph>
          )}
          <Space size={16}>
            <Text type="secondary" style={{ fontSize: 13 }}>
              {t("projectDetailPage.creator")}: {creatorName}
            </Text>
            <Text type="secondary" style={{ fontSize: 13 }}>
              {t("projectDetailPage.team")}: {teamName}
            </Text>
          </Space>
        </Flex>
      }
    />
  );
}
