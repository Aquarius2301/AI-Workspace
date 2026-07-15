import { Flex, Space, Typography } from "antd";
import { useTranslation } from "react-i18next";
import { AICardItem, AIVisibilityTag } from "@/components";

const { Text, Paragraph } = Typography;

interface ProjectInfoCardProps {
  name: string;
  description?: string | null;
  visibility: string;
  creatorName: string;
  teamName: string;
  isLoading?: boolean;
}

export function ProjectInfoCard({
  name,
  description,
  visibility,
  creatorName,
  teamName,
  isLoading = false,
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
