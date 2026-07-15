import { Input, Flex, Empty, Typography, Space } from "antd";
import { UserOutlined, CalendarOutlined } from "@ant-design/icons";
import {
  AICardItem,
  AIList,
  AITaskStatusSelect,
  AITaskPrioritySelect,
  AITaskStatusTag,
  AITaskPriorityTag,
} from "@/components";
import { useSearch, useMyTasksByProject } from "@/hooks";
import { useTranslation } from "react-i18next";
import type { TaskItemResult } from "@/types";

const { Text } = Typography;

interface MyTaskTabProps {
  projectId: string;
}

export function MyTaskTab({ projectId }: MyTaskTabProps) {
  const { t } = useTranslation();
  const {
    searchProps,
    taskStatusProps,
    priorityProps,
    paginationProps,
    queryParams,
  } = useSearch({
    hasTaskStatusFilter: true,
    hasPriorityFilter: true,
  });

  const { data, isLoading } = useMyTasksByProject(
    projectId,
    queryParams.search,
    queryParams.taskStatus,
    queryParams.priority,
    queryParams.page,
    queryParams.pageSize,
  );

  const hasSearchQuery =
    !!queryParams.search || !!queryParams.taskStatus || !!queryParams.priority;
  const isDataEmpty = !isLoading && data && data.total === 0;
  const showSearchBar = !(isDataEmpty && !hasSearchQuery);

  return (
    <Flex vertical gap={16}>
      {/* ── Filter bar ── */}
      {showSearchBar && (
        <Flex gap={12} align="center" wrap>
          <Input
            placeholder={t("projectDetailPage.myTasks.search")}
            allowClear
            value={searchProps.search}
            onChange={(e) => searchProps.onSearchChange(e.target.value)}
            style={{ maxWidth: 360 }}
          />
          <AITaskStatusSelect
            value={queryParams.taskStatus}
            onChange={taskStatusProps.onTaskStatusChange}
            allowClear
          />
          <AITaskPrioritySelect
            value={queryParams.priority}
            onChange={priorityProps.onPriorityChange}
            allowClear
          />
        </Flex>
      )}

      {/* ── Task list ── */}
      <AIList<TaskItemResult>
        data={data?.items ?? []}
        itemKey={(item) => item.id}
        renderItem={(task) => (
          <AICardItem
            header={
              <Flex align="center" gap={8}>
                <Text strong style={{ fontSize: 15 }}>
                  {task.title}
                </Text>
                <AITaskStatusTag status={task.status} />
                <AITaskPriorityTag priority={task.priority} />
              </Flex>
            }
            content={
              task.description && (
                <Text type="secondary">{task.description}</Text>
              )
            }
            footer={
              <Flex justify="space-between">
                <Space size={16}>
                  {task.assignedToName && (
                    <Text type="secondary" style={{ fontSize: 13 }}>
                      <UserOutlined /> {task.assignedToName}
                    </Text>
                  )}
                  {task.dueDate && (
                    <Text type="secondary" style={{ fontSize: 13 }}>
                      <CalendarOutlined />{" "}
                      {new Date(task.dueDate).toLocaleDateString()}
                    </Text>
                  )}
                </Space>
                <Text type="secondary" style={{ fontSize: 13 }}>
                  {t("projectDetailPage.myTasks.view")}
                </Text>
              </Flex>
            }
            isHoverable
          />
        )}
        isLoading={isLoading}
        paginationProps={{
          ...paginationProps,
          total: data?.total ?? 0,
        }}
        notFound={{
          icon: Empty.PRESENTED_IMAGE_SIMPLE,
          title: t("projectDetailPage.myTasks.notFound.title"),
          subTitle: t("projectDetailPage.myTasks.notFound.description"),
        }}
        empty={{
          icon: Empty.PRESENTED_IMAGE_SIMPLE,
          title: t("projectDetailPage.myTasks.empty.title"),
          subTitle: t("projectDetailPage.myTasks.empty.description"),
        }}
        hasSearchQuery={hasSearchQuery}
      />
    </Flex>
  );
}
