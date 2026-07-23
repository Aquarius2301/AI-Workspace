import { useState, useMemo } from "react";
import { Flex, Typography, Button, Radio, Space, theme, Divider } from "antd";
import {
  UserOutlined,
  CalendarOutlined,
  EditOutlined,
  DeleteOutlined,
} from "@ant-design/icons";
import { AICard, AITaskPriorityTag, AIModal } from "@/components";
import type { TaskItemResult, TaskStatus } from "@/types";
import { TASK_STATUS } from "@/types";
import { formatIsoLocaleDate } from "@/utils";
import {
  useDeleteTask,
  useUpdateTaskStatus,
  useAdminUpdateTaskStatus,
  useAuthMe,
} from "@/hooks";
import { EditTaskModal } from "../modals/EditTaskModal";
import { App } from "antd";
import { useTranslation } from "react-i18next";

const { Text, Paragraph } = Typography;

interface TaskCardProps {
  task?: TaskItemResult;
  isLoading?: boolean;
  canEdit?: boolean;
  projectId?: string;
  isMobile?: boolean;
}

export function TaskCard({
  task,
  isLoading,
  canEdit = false,
  projectId,
  isMobile = false,
}: TaskCardProps) {
  const { token } = theme.useToken();
  const { message } = App.useApp();
  const { t } = useTranslation();
  const { data: currentUser } = useAuthMe();
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
  const [isStatusOpen, setIsStatusOpen] = useState(false);
  const [selectedStatus, setSelectedStatus] = useState<TaskStatus>("toDo");
  const deleteTask = useDeleteTask(projectId ?? "");
  const updateTaskStatus = useUpdateTaskStatus(projectId ?? "");
  const adminUpdateTaskStatus = useAdminUpdateTaskStatus(projectId ?? "");

  const isMyTask = useMemo(
    () => currentUser?.id === task?.assignedToId,
    [currentUser?.id, task?.assignedToId],
  );
  const canChangeStatus = canEdit || isMyTask;

  const isOverdue = useMemo(
    () =>
      task?.dueDate &&
      new Date(task.dueDate) < new Date() &&
      task.status !== "done",
    [task?.dueDate, task?.status],
  );

  // Loading state: render skeleton only
  if (isLoading) {
    return <AICard isLoading />;
  }

  // Should never happen when not loading
  if (!task) return null;

  const handleDelete = async () => {
    if (!projectId) return;

    try {
      await deleteTask.mutateAsync(task.id);
      setIsDeleteModalOpen(false);
      message.success(t("projectDetailPage.deleteTask.success"));
    } catch {
      message.error(t("error.Forbidden"));
    }
  };

  const [isDragging, setIsDragging] = useState(false);

  const handleDragStart = (e: React.DragEvent<HTMLDivElement>) => {
    e.dataTransfer.setData("text/plain", task.id);
    e.dataTransfer.effectAllowed = "move";
    setIsDragging(true);
  };

  const handleDragEnd = () => {
    setIsDragging(false);
  };

  // Mobile: tap → mở status modal
  // Desktop: double-click → mở status modal (vì click dùng cho drag)
  const openStatusModal = () => {
    setSelectedStatus(task.status);
    setIsStatusOpen(true);
  };

  const handleCardTap = () => {
    if (isMobile) {
      openStatusModal();
    }
  };

  const handleDoubleClick = () => {
    if (!isMobile) {
      openStatusModal();
    }
  };

  const handleStatusChange = async () => {
    if (!projectId || selectedStatus === task.status) {
      setIsStatusOpen(false);
      return;
    }

    try {
      if (canEdit) {
        await adminUpdateTaskStatus.mutateAsync({
          taskId: task.id,
          status: selectedStatus,
        });
      } else {
        await updateTaskStatus.mutateAsync({
          taskId: task.id,
          status: selectedStatus,
        });
      }
      setIsStatusOpen(false);
      message.success(t("projectDetailPage.updateTaskStatus.success"));
    } catch {
      message.error(t("error.Forbidden"));
    }
  };

  return (
    <>
      <div
        draggable={!isMobile}
        onDragStart={isMobile ? undefined : handleDragStart}
        onDragEnd={isMobile ? undefined : handleDragEnd}
        onClick={handleCardTap}
        onDoubleClick={isMobile ? undefined : handleDoubleClick}
        style={{
          opacity: isDragging ? 0.4 : 1,
          transition: "opacity 0.15s",
          cursor: isMobile ? "pointer" : canChangeStatus ? "grab" : undefined,
          userSelect: "none",
        }}
      >
        <AICard
          style={{
            marginTop: 8,
            background: isMyTask
              ? token.colorPrimaryBg
              : token.colorBgContainer,
          }}
        >
          <Flex vertical gap={4}>
            <Flex align="flex-start" justify="space-between" gap={6}>
              <Text strong style={{ fontSize: 13.5, flex: 1, lineHeight: 1.3 }}>
                {task.title}
              </Text>
              <Flex gap={4} align="center" style={{ flexShrink: 0 }}>
                <AITaskPriorityTag priority={task.priority} />
                {canEdit && (
                  <Flex gap={2}>
                    <Button
                      size="small"
                      type="text"
                      icon={<EditOutlined />}
                      onClick={(e) => {
                        e.stopPropagation();
                        setIsEditModalOpen(true);
                      }}
                    />
                    <Button
                      size="small"
                      type="text"
                      danger
                      icon={<DeleteOutlined />}
                      onClick={(e) => {
                        e.stopPropagation();
                        setIsDeleteModalOpen(true);
                      }}
                    />
                  </Flex>
                )}
              </Flex>
            </Flex>

            {task.description && (
              <Text
                type="secondary"
                style={{
                  fontSize: 12,
                  display: "-webkit-box",
                  WebkitLineClamp: 2,
                  WebkitBoxOrient: "vertical",
                  overflow: "hidden",
                  lineHeight: 1.3,
                }}
              >
                {task.description}
              </Text>
            )}

            <Flex
              justify="space-between"
              align="center"
              gap={8}
              style={{ marginTop: 1 }}
            >
              {task.assignedToName ? (
                <Text type="secondary" style={{ fontSize: 11 }}>
                  <UserOutlined style={{ marginRight: 4 }} />
                  {task.assignedToName}
                </Text>
              ) : (
                <span />
              )}
              {task.dueDate && (
                <Text
                  type={isOverdue ? "danger" : "secondary"}
                  style={{ fontSize: 11.5 }}
                >
                  <CalendarOutlined style={{ marginRight: 4 }} />
                  {formatIsoLocaleDate(task.dueDate)}
                </Text>
              )}
            </Flex>
          </Flex>
        </AICard>
      </div>

      {/* ── Delete confirmation modal ── */}
      <AIModal
        title={t("modal.delete")}
        open={isDeleteModalOpen}
        onCancel={() => setIsDeleteModalOpen(false)}
        onOk={handleDelete}
        isLoading={deleteTask.isPending}
        footer={[
          { type: "cancel", text: t("modal.cancel") },
          { type: "delete", text: t("modal.delete") },
        ]}
      >
        <Text>{t("projectDetailPage.deleteTask.confirmation")}</Text>
      </AIModal>

      {/* ── Edit Task Modal ── */}
      {isEditModalOpen && projectId && (
        <EditTaskModal
          isOpen={true}
          onClose={() => setIsEditModalOpen(false)}
          projectId={projectId}
          task={task}
        />
      )}

      {/* ── Status & Detail Modal (nâng cấp: hiển thị full task info) ── */}
      <AIModal
        title={task.title}
        open={isStatusOpen}
        onCancel={() => setIsStatusOpen(false)}
        onOk={handleStatusChange}
        isLoading={
          updateTaskStatus.isPending || adminUpdateTaskStatus.isPending
        }
        footer={[
          { type: "cancel", text: t("modal.cancel") },
          { type: "update", text: t("modal.update") },
        ]}
      >
        <Flex vertical gap={12} style={{ marginTop: 8 }}>
          {/* Full description */}
          {task.description && (
            <div>
              <Text type="secondary" style={{ fontSize: 12 }}>
                {t("projectDetailPage.taskDetail.description")}
              </Text>
              <Paragraph style={{ margin: "4px 0 0 0", fontSize: 13 }}>
                {task.description}
              </Paragraph>
            </div>
          )}

          {/* Info grid */}
          <Flex gap={16} wrap>
            <div>
              <Text type="secondary" style={{ fontSize: 12, display: "block" }}>
                {t("projectDetailPage.taskDetail.priority")}
              </Text>
              <AITaskPriorityTag priority={task.priority} />
            </div>
            <div>
              <Text type="secondary" style={{ fontSize: 12, display: "block" }}>
                {t("projectDetailPage.taskDetail.assignee")}
              </Text>
              <Text style={{ fontSize: 13 }}>{task.assignedToName ?? "—"}</Text>
            </div>
            {task.dueDate && (
              <div>
                <Text
                  type="secondary"
                  style={{ fontSize: 12, display: "block" }}
                >
                  {t("projectDetailPage.taskDetail.dueDate")}
                </Text>
                <Text
                  style={{ fontSize: 13 }}
                  type={isOverdue ? "danger" : undefined}
                >
                  {formatIsoLocaleDate(task.dueDate)}
                </Text>
              </div>
            )}
            <div>
              <Text type="secondary" style={{ fontSize: 12, display: "block" }}>
                {t("projectDetailPage.taskDetail.createdAt")}
              </Text>
              <Text style={{ fontSize: 13 }}>
                {formatIsoLocaleDate(task.createdAt)}
              </Text>
            </div>
          </Flex>

          <Divider style={{ margin: "4px 0" }} />

          {/* Status selector */}
          <div>
            <Text strong style={{ fontSize: 13 }}>
              {t("projectDetailPage.updateTaskStatus.statusLabel")}
            </Text>
            <Radio.Group
              value={selectedStatus}
              onChange={(e) => setSelectedStatus(e.target.value)}
              style={{ marginTop: 8, width: "100%" }}
            >
              <Space direction="vertical" style={{ width: "100%" }}>
                {TASK_STATUS.map((status) => (
                  <Radio key={status} value={status} style={{ fontSize: 13 }}>
                    {t(`taskStatusSelect.${status}`)}
                  </Radio>
                ))}
              </Space>
            </Radio.Group>
          </div>

          {/* Edit/Delete buttons */}
          {canEdit && (
            <Flex gap={8} style={{ marginTop: 4 }}>
              <Button
                size="small"
                icon={<EditOutlined />}
                onClick={() => {
                  setIsStatusOpen(false);
                  setIsEditModalOpen(true);
                }}
              >
                {t("modal.edit")}
              </Button>
              <Button
                size="small"
                danger
                icon={<DeleteOutlined />}
                onClick={() => {
                  setIsStatusOpen(false);
                  setIsDeleteModalOpen(true);
                }}
              >
                {t("modal.delete")}
              </Button>
            </Flex>
          )}
        </Flex>
      </AIModal>
    </>
  );
}
