import { useTranslation } from "react-i18next";
import {
  Card,
  Typography,
  Statistic,
  Row,
  Col,
  Button,
  theme,
  Skeleton,
} from "antd";
import {
  RightOutlined,
  UnorderedListOutlined,
  LoadingOutlined,
  CheckCircleOutlined,
} from "@ant-design/icons";
import type { MyTaskItemResponse } from "@/types";

const { Text } = Typography;

interface TaskStatusOverviewProps {
  tasks: MyTaskItemResponse[];
  isLoading: boolean;
}

export function TaskStatusOverview({
  tasks,
  isLoading,
}: TaskStatusOverviewProps) {
  const { t } = useTranslation();
  // export default function TaskStatusOverview({) => {
  const { token } = theme.useToken();

  const openTasks = tasks.filter((t) => t.status === "Open");
  const inProgressTasks = tasks.filter((t) => t.status === "InProgress");
  const doneTasks = tasks.filter((t) => t.status === "Done");

  const statusCards = [
    {
      label: t("home.openTasks"),
      count: openTasks.length,
      color: "#FF4D4F",
      bgColor: "#FFF1F0",
      icon: (
        <UnorderedListOutlined style={{ fontSize: 28, color: "#FF4D4F" }} />
      ),
    },
    {
      label: t("home.inProgressTasks"),
      count: inProgressTasks.length,
      color: "#FAAD14",
      bgColor: "#FFFBE6",
      icon: <LoadingOutlined style={{ fontSize: 28, color: "#FAAD14" }} />,
    },
    {
      label: t("home.doneTasks"),
      count: doneTasks.length,
      color: "#52C41A",
      bgColor: "#F6FFED",
      icon: <CheckCircleOutlined style={{ fontSize: 28, color: "#52C41A" }} />,
    },
  ];

  return (
    <Card
      title={
        <Text strong style={{ fontSize: 16, color: token.colorTextBase }}>
          {t("home.taskStatus")}
        </Text>
      }
      extra={
        <Button type="link" style={{ padding: 0 }}>
          {t("home.viewMore")} <RightOutlined style={{ fontSize: 12 }} />
        </Button>
      }
      style={{
        borderRadius: token.borderRadius * 2,
        border: `1px solid ${token.colorBorder}`,
        height: "100%",
      }}
      styles={{
        body: { padding: 20 },
      }}
    >
      <Row gutter={[16, 16]}>
        {statusCards.map((status) => (
          <Col xs={24} sm={8} key={status.label}>
            <Card
              style={{
                borderRadius: token.borderRadius,
                backgroundColor: token.colorBgLayout,
                border: `1px solid ${token.colorBorder}`,
              }}
              styles={{
                body: {
                  padding: 16,
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "space-between",
                },
              }}
            >
              {isLoading ? (
                <Skeleton active={true} />
              ) : (
                <>
                  <Statistic
                    title={
                      <Text
                        style={{
                          color: token.colorTextDescription,
                          fontSize: 13,
                        }}
                      >
                        {status.label}
                      </Text>
                    }
                    value={status.count}
                    styles={{
                      content: {
                        color: status.color,
                        fontSize: 28,
                        fontWeight: 700,
                      },
                    }}
                  />
                  {status.icon}
                </>
              )}
            </Card>
          </Col>
        ))}
      </Row>
    </Card>
  );
}
