import type { CommentSummary } from "@/types";
import { formatIsoLocaleDate } from "@/utils";
import { theme, Flex } from "antd";
import Text from "antd/es/typography/Text";

interface RecentCommentItemProps {
  comment: CommentSummary;
  isLast: boolean;
}

export function RecentCommentItem({ comment, isLast }: RecentCommentItemProps) {
  const { token } = theme.useToken();

  return (
    <Flex
      vertical
      style={{
        padding: "14px 0",
        borderBottom: isLast
          ? undefined
          : `1px solid ${token.colorBorderSecondary}`,
      }}
    >
      <Text
        style={{
          fontSize: 13,
          marginBottom: 6,
          color: token.colorTextSecondary,
          lineHeight: 1.5,
        }}
        ellipsis={{ tooltip: comment.content }}
      >
        {comment.content}
      </Text>
      <Flex align="center" gap={8}>
        {comment.projectName && (
          <Text type="secondary" style={{ fontSize: 12 }}>
            {comment.projectName}
          </Text>
        )}
        {comment.projectName && (
          <Text type="secondary" style={{ fontSize: 12 }}>
            ·
          </Text>
        )}
        <Text type="secondary" style={{ fontSize: 12 }}>
          {formatIsoLocaleDate(comment.createdAt)}
        </Text>
      </Flex>
    </Flex>
  );
}
