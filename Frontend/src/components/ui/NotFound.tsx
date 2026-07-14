import { Button, Empty, Flex, Typography } from "antd";
import { AICard } from "./AICard";

const { Text } = Typography;

interface NotFoundProps {
  title?: string;
  description?: string;
  buttonText?: string;
  onBack?: () => void;
}

export function NotFound({
  title,
  description,
  buttonText = "Back",
  onBack,
}: NotFoundProps) {
  return (
    <AICard
      style={{
        textAlign: "center",
        padding: "48px 24px",
      }}
    >
      <Flex vertical gap={16} align="center">
        <Empty
          image={Empty.PRESENTED_IMAGE_SIMPLE}
          description={
            <Flex vertical gap={4}>
              {title && <Text strong>{title}</Text>}
              {description && (
                <Text type="secondary">{description}</Text>
              )}
            </Flex>
          }
        />
        {onBack && (
          <Button type="primary" onClick={onBack}>
            {buttonText}
          </Button>
        )}
      </Flex>
    </AICard>
  );
}
