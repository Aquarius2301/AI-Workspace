import { Row, Col, Flex, theme, Empty } from "antd";
import { useTranslation } from "react-i18next";
import { AICard } from "@/components";
import {
  CheckSquareOutlined,
  CommentOutlined,
} from "@ant-design/icons";
import type { TaskItemSummary, CommentSummary } from "@/types";
import { RecentTaskItem } from "./RecentTaskItem";
import { RecentCommentItem } from "./RecentCommentItem";

interface RecentActivitySectionProps {
  isLoading: boolean;
  recentTasks: TaskItemSummary[];
  recentComments: CommentSummary[];
}

export function RecentActivitySection({
  isLoading,
  recentTasks,
  recentComments,
}: RecentActivitySectionProps) {
  const { t } = useTranslation();
  const { token } = theme.useToken();

  return (
    <Row gutter={[16, 16]}>
      {/* ── Recent Tasks ── */}
      <Col xs={24} md={12}>
        <AICard
          isLoading={isLoading}
          title={
            <Flex align="center" gap={8}>
              <CheckSquareOutlined style={{ color: token.colorPrimary }} />
              <span>{t("overview.recentTasks.default")}</span>
            </Flex>
          }
          style={{ height: "100%" }}
        >
          {recentTasks.length === 0 ? (
            <div style={{ padding: "32px 0" }}>
              <Empty
                description={t("overview.recentTasks.empty")}
                image={Empty.PRESENTED_IMAGE_SIMPLE}
              />
            </div>
          ) : (
            <Flex vertical>
              {recentTasks.slice(0, 5).map((task, idx) => (
                <RecentTaskItem
                  key={task.id}
                  task={task}
                  isLast={idx === Math.min(recentTasks.length, 5) - 1}
                />
              ))}
            </Flex>
          )}
        </AICard>
      </Col>

      {/* ── Recent Comments ── */}
      <Col xs={24} md={12}>
        <AICard
          isLoading={isLoading}
          title={
            <Flex align="center" gap={8}>
              <CommentOutlined style={{ color: token.colorPrimary }} />
              <span>{t("overview.recentComments.default")}</span>
            </Flex>
          }
          style={{ height: "100%" }}
        >
          {recentComments.length === 0 ? (
            <div style={{ padding: "32px 0" }}>
              <Empty
                description={t("overview.recentComments.empty")}
                image={Empty.PRESENTED_IMAGE_SIMPLE}
              />
            </div>
          ) : (
            <Flex vertical>
              {recentComments.slice(0, 5).map((comment, idx) => (
                <RecentCommentItem
                  key={comment.id}
                  comment={comment}
                  isLast={idx === Math.min(recentComments.length, 5) - 1}
                />
              ))}
            </Flex>
          )}
        </AICard>
      </Col>
    </Row>
  );
}
