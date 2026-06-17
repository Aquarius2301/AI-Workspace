import {
  Card,
  Typography,
  List,
  Tag,
  Button,
  Empty,
  theme,
  Skeleton,
} from "antd";
import {
  RightOutlined,
  ClockCircleOutlined,
  ExclamationCircleOutlined,
} from "@ant-design/icons";
import type { MyTaskItemResponse, TaskStatus } from "@/types";

const { Text } = Typography;

interface UpcomingTasksProps {
  tasks: MyTaskItemResponse[];
  isLoading: boolean;
}

const getStatusColor = (status: TaskStatus) => {
  switch (status) {
    case "Open":
      return "default";
    case "InProgress":
      return "processing";
    case "Done":
      return "success";
    case "Blocked":
      return "error";
    default:
      return "error";
  }
};

const getStatusLabel = (status: string) => {
  switch (status) {
    case "Open":
      return "Chưa làm";
    case "InProgress":
      return "Đang làm";
    case "Done":
      return "Đã làm";
    case "Blocked":
      return "Bị chặn";
    default:
      return status;
  }
};

const formatDate = (dateStr?: string) => {
  if (!dateStr) return "Không có hạn";
  try {
    const date = new Date(dateStr);
    return date.toLocaleDateString("vi-VN", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
    });
  } catch {
    return dateStr;
  }
};

const getDaysUntilDue = (dueDate?: string): number | null => {
  if (!dueDate) return null;
  const due = new Date(dueDate);
  const now = new Date();
  const diffTime = due.getTime() - now.getTime();
  return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
};

export function UpcomingTasks({ tasks, isLoading }: UpcomingTasksProps) {
  const { token } = theme.useToken();

  // Filter out Done tasks, sort by dueDate (earliest first), take top 10
  const sortedTasks = [...tasks]
    .filter((t) => t.status !== "Done")
    .sort((a, b) => {
      if (!a.dueDate && !b.dueDate) return 0;
      if (!a.dueDate) return 1;
      if (!b.dueDate) return -1;
      return new Date(a.dueDate).getTime() - new Date(b.dueDate).getTime();
    })
    .slice(0, 10);

  return (
    <Card
      title={
        <Text strong style={{ fontSize: 16, color: token.colorTextBase }}>
          Công việc sắp đến hạn
        </Text>
      }
      extra={
        <Button type="link" style={{ padding: 0 }}>
          Xem thêm <RightOutlined style={{ fontSize: 12 }} />
        </Button>
      }
      style={{
        borderRadius: token.borderRadius * 2,
        border: `1px solid ${token.colorBorder}`,
        width: "100%",
      }}
      styles={{
        body: { padding: 12 },
      }}
    >
      {isLoading ? (
        <Skeleton active={true} />
      ) : sortedTasks.length === 0 ? (
        <Empty
          image={Empty.PRESENTED_IMAGE_SIMPLE}
          description="Không có công việc nào cần làm"
          style={{ margin: "32px 0" }}
        />
      ) : (
        <List
          dataSource={sortedTasks}
          renderItem={(task) => {
            const daysLeft = getDaysUntilDue(task.dueDate);
            const isOverdue = daysLeft !== null && daysLeft < 0;
            const isUrgent =
              daysLeft !== null && daysLeft >= 0 && daysLeft <= 3;

            return (
              <List.Item
                style={{
                  padding: "10px 12px",
                  borderRadius: token.borderRadius,
                  cursor: "pointer",
                  transition: "all 0.2s",
                  border: "none",
                }}
                onMouseEnter={(e) => {
                  e.currentTarget.style.backgroundColor = token.colorBgLayout;
                }}
                onMouseLeave={(e) => {
                  e.currentTarget.style.backgroundColor = "transparent";
                }}
              >
                <div
                  style={{
                    display: "flex",
                    alignItems: "center",
                    gap: 12,
                    width: "100%",
                  }}
                >
                  <div style={{ flex: 1, minWidth: 0 }}>
                    <Text
                      strong
                      style={{
                        color: token.colorTextBase,
                        display: "block",
                        whiteSpace: "nowrap",
                        overflow: "hidden",
                        textOverflow: "ellipsis",
                      }}
                    >
                      {task.title}
                    </Text>
                    <div
                      style={{
                        display: "flex",
                        alignItems: "center",
                        gap: 8,
                        marginTop: 4,
                        flexWrap: "wrap",
                      }}
                    >
                      {task.projectName && (
                        <Text type="secondary" style={{ fontSize: 12 }}>
                          {task.projectName}
                        </Text>
                      )}
                      {task.dueDate && (
                        <span
                          style={{
                            display: "inline-flex",
                            alignItems: "center",
                            gap: 4,
                            fontSize: 12,
                            color: isOverdue
                              ? "#FF4D4F"
                              : isUrgent
                                ? "#FAAD14"
                                : token.colorTextDescription,
                          }}
                        >
                          {isOverdue ? (
                            <ExclamationCircleOutlined />
                          ) : (
                            <ClockCircleOutlined />
                          )}
                          {formatDate(task.dueDate)}
                          {isOverdue && (
                            <Text
                              style={{
                                color: "#FF4D4F",
                                fontSize: 11,
                                fontWeight: 600,
                              }}
                            >
                              (Quá hạn {Math.abs(daysLeft)} ngày)
                            </Text>
                          )}
                          {isUrgent && !isOverdue && (
                            <Text
                              style={{
                                color: "#FAAD14",
                                fontSize: 11,
                                fontWeight: 600,
                              }}
                            >
                              (Còn {daysLeft} ngày)
                            </Text>
                          )}
                        </span>
                      )}
                      {!task.dueDate && (
                        <Text type="secondary" style={{ fontSize: 12 }}>
                          <ClockCircleOutlined style={{ marginRight: 4 }} />
                          Không có hạn
                        </Text>
                      )}
                    </div>
                  </div>
                  <Tag
                    color={getStatusColor(task.status)}
                    style={{ borderRadius: 12, flexShrink: 0, fontSize: 11 }}
                  >
                    {getStatusLabel(task.status)}
                  </Tag>
                </div>
              </List.Item>
            );
          }}
        />
      )}
    </Card>
  );
}
