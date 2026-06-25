import { useTranslation } from "react-i18next";
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
import { RightOutlined, FolderOpenOutlined } from "@ant-design/icons";
import type { TeamProjectItem } from "@/types";

const { Text } = Typography;

interface ProjectListProps {
  projects: TeamProjectItem[];
  isLoading: boolean;
}

export function ProjectList({ projects, isLoading }: ProjectListProps) {
  const { t } = useTranslation();
  const { token } = theme.useToken();

  return (
    <Card
      title={
        <Text strong style={{ fontSize: 16, color: token.colorTextBase }}>
          {t("home.projectList")}
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
        body: { padding: 12 },
      }}
    >
      {isLoading ? (
        <Skeleton active={true} />
      ) : projects.length === 0 ? (
        <Empty
          image={Empty.PRESENTED_IMAGE_SIMPLE}
          description={t("home.noProjects")}
          style={{ margin: "32px 0" }}
        />
      ) : (
        <List
          dataSource={projects}
          renderItem={(project) => (
            <List.Item
              style={{
                padding: "10px 12px",
                borderRadius: token.borderRadius,
                cursor: "pointer",
                transition: "all 0.2s",
                border: "none",
              }}
              onClick={() => {
                // Handle project click
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
                <FolderOpenOutlined
                  style={{ fontSize: 20, color: token.colorPrimary }}
                />
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
                    {project.name}
                  </Text>
                  {project.description && (
                    <Text
                      type="secondary"
                      style={{
                        fontSize: 12,
                        display: "block",
                        whiteSpace: "nowrap",
                        overflow: "hidden",
                        textOverflow: "ellipsis",
                      }}
                    >
                      {project.description}
                    </Text>
                  )}
                </div>
                <Tag
                  color={project.visibility === "Public" ? "blue" : "default"}
                  style={{ borderRadius: 12, flexShrink: 0, fontSize: 11 }}
                >
                  {project.visibility === "Public"
                    ? t("home.public")
                    : t("home.private")}
                </Tag>
              </div>
            </List.Item>
          )}
        />
      )}
    </Card>
  );
}
