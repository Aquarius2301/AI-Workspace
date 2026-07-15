import { useEffect, useRef, useState } from "react";
import { Input, Flex, Empty, Typography, Space, theme } from "antd";
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
import type { PageSize } from "@/types";

const { Text } = Typography;

interface MyTaskTabProps {
  projectId: string;
  selectedTaskId?: string;
}

export function MyTaskTab({ projectId, selectedTaskId }: MyTaskTabProps) {
  const { t } = useTranslation();
  const { token } = theme.useToken();
  const itemRefs = useRef<Map<string, HTMLDivElement>>(new Map());

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

  // ── If selectedTaskId is provided, auto-search across pages ──
  const [targetPage, setTargetPage] = useState(queryParams.page);

  // Sync targetPage when user manually changes page
  useEffect(() => {
    setTargetPage(queryParams.page);
  }, [queryParams.page]);

  const { data, isLoading } = useMyTasksByProject(
    projectId,
    queryParams.search,
    queryParams.taskStatus,
    queryParams.priority,
    targetPage,
    queryParams.pageSize as PageSize,
  );

  const hasSearchQuery =
    !!queryParams.search || !!queryParams.taskStatus || !!queryParams.priority;
  const isDataEmpty = !isLoading && data && data.total === 0;
  const showSearchBar = !(isDataEmpty && !hasSearchQuery);

  // ── Auto-paginate to find selected task ──
  useEffect(() => {
    if (!selectedTaskId || !data || isLoading) return;

    const foundInCurrentPage = data.items.some(
      (item) => item.id === selectedTaskId,
    );
    if (foundInCurrentPage) return;

    // Check if there are more pages to search
    const totalPages = Math.ceil(data.total / (queryParams.pageSize || 10));
    if (targetPage < totalPages) {
      setTargetPage((prev) => prev + 1);
    }
  }, [selectedTaskId, data, isLoading, targetPage, queryParams.pageSize]);

  // ── Scroll selected task into view ──
  useEffect(() => {
    if (selectedTaskId) {
      const el = itemRefs.current.get(selectedTaskId);
      if (el) {
        el.scrollIntoView({ behavior: "smooth", block: "center" });
      }
    }
  }, [selectedTaskId, data]);

  // Reset targetPage when filters change
  useEffect(() => {
    setTargetPage(1);
  }, [queryParams.search, queryParams.taskStatus, queryParams.priority]);

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
        renderItem={(task) => {
          const isSelected = task.id === selectedTaskId;
          return (
            <div
              ref={(el) => {
                if (el) itemRefs.current.set(task.id, el);
                else itemRefs.current.delete(task.id);
              }}
            >
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
                style={
                  isSelected
                    ? {
                        border: `2px solid ${token.colorPrimary}`,
                        background: token.colorPrimaryBg,
                      }
                    : undefined
                }
              />
            </div>
          );
        }}
        isLoading={isLoading}
        paginationProps={{
          ...paginationProps,
          page: targetPage,
          total: data?.total ?? 0,
          onPageChange: (newPage: number) => {
            setTargetPage(newPage);
            paginationProps.onPageChange(newPage);
          },
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
