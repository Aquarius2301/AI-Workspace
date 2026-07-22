import { useState } from "react";
import { Flex, Typography, Button } from "antd";
import {
  UserOutlined,
  CalendarOutlined,
  EditOutlined,
  DeleteOutlined,
} from "@ant-design/icons";
import { AICard, AITaskPriorityTag, AIModal } from "@/components";
import type { TaskItemResult } from "@/types";
import { formatIsoLocaleDate } from "@/utils";
import { useDeleteTask } from "@/hooks";
import { EditTaskModal } from "../modals/EditTaskModal";
import { App } from "antd";
import { useTranslation } from "react-i18next";

const { Text } = Typography;

interface TaskCardProps {
  task?: TaskItemResult;
  isLoading?: boolean;
  canEdit?: boolean;
  projectId?: string;
}

export function TaskCard({
  task,
  isLoading,
  canEdit = false,
  projectId,
}: TaskCardProps) {
  const { message } = App.useApp();
  const { t } = useTranslation();
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
  const deleteTask = useDeleteTask(projectId ?? "");
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

  return (
    <>
      <AICard style={{ marginTop: 8 }}>
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
                    onClick={() => setIsEditModalOpen(true)}
                  />
                  <Button
                    size="small"
                    type="text"
                    danger
                    icon={<DeleteOutlined />}
                    onClick={() => setIsDeleteModalOpen(true)}
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
              <Text type="secondary" style={{ fontSize: 11.5 }}>
                <CalendarOutlined style={{ marginRight: 4 }} />
                {formatIsoLocaleDate(task.dueDate)}
              </Text>
            )}
          </Flex>
        </Flex>
      </AICard>

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

      {isEditModalOpen && projectId && (
        <EditTaskModal
          isOpen={true}
          onClose={() => setIsEditModalOpen(false)}
          projectId={projectId}
          task={task}
        />
      )}
    </>
  );
}
