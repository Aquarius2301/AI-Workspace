import { useState } from "react";
import { Flex, Typography, Divider, Form, App } from "antd";
import { AIModal, AITaskStatusSelect, AITaskPriorityTag } from "@/components";
import { useUpdateTaskStatus } from "@/hooks";
import { useTranslation } from "react-i18next";
import type { TaskItemResult, TaskStatus } from "@/types";
import { formatIsoLocaleDate, getErrorMessage } from "@/utils";

const { Text } = Typography;

interface UpdateTaskStatusModalProps {
  isOpen: boolean;
  onClose: () => void;
  projectId: string;
  task: TaskItemResult;
}

export function UpdateTaskStatusModal({
  isOpen,
  onClose,
  projectId,
  task,
}: UpdateTaskStatusModalProps) {
  const { t } = useTranslation();
  const { message } = App.useApp();
  const [selectedStatus, setSelectedStatus] = useState<TaskStatus>(task.status);
  const updateTaskStatus = useUpdateTaskStatus(projectId);

  const isStatusChanged = selectedStatus !== task.status;

  const handleSave = async () => {
    try {
      await updateTaskStatus.mutateAsync({
        taskId: task.id,
        status: selectedStatus,
      });
      message.success(t("projectDetailPage.updateTaskStatus.success"));
      onClose();
    } catch (error) {
      message.error(getErrorMessage(error));
    }
  };

  return (
    <AIModal
      title={t("projectDetailPage.updateTaskStatus.title")}
      open={isOpen}
      onOk={handleSave}
      onCancel={onClose}
      isLoading={updateTaskStatus.isPending}
      footer={[
        { type: "cancel" },
        { type: "update", disabled: !isStatusChanged },
      ]}
    >
      <Flex vertical gap={8} style={{ marginTop: 16 }}>
        {/* Title */}
        <div>
          <Text type="secondary">
            {t("projectDetailPage.updateTaskStatus.titleLabel")}
          </Text>
          <div>
            <Text strong>{task.title}</Text>
          </div>
        </div>

        {/* Description */}
        {task.description && (
          <div>
            <Text type="secondary">
              {t("projectDetailPage.updateTaskStatus.descriptionLabel")}
            </Text>
            <div>
              <Text>{task.description}</Text>
            </div>
          </div>
        )}

        {/* Priority & Assignee */}
        <Flex gap={24}>
          <div>
            <Text type="secondary">
              {t("projectDetailPage.updateTaskStatus.priorityLabel")}
            </Text>
            <div>
              <AITaskPriorityTag priority={task.priority} />
            </div>
          </div>
          <div>
            <Text type="secondary">
              {t("projectDetailPage.updateTaskStatus.assigneeLabel")}
            </Text>
            <div>
              <Text>{task.assignedToName ?? "—"}</Text>
            </div>
          </div>
        </Flex>

        {/* Due date */}
        {task.dueDate && (
          <div>
            <Text type="secondary">
              {t("projectDetailPage.updateTaskStatus.dueDateLabel")}
            </Text>
            <div>
              <Text>{formatIsoLocaleDate(task.dueDate)}</Text>
            </div>
          </div>
        )}

        <Divider style={{ margin: "12px 0" }} />

        {/* Status selector */}
        <Form.Item
          label={t("projectDetailPage.updateTaskStatus.statusLabel")}
          style={{ marginBottom: 0 }}
        >
          <AITaskStatusSelect
            allowClear={false}
            value={selectedStatus}
            onChange={(value) => setSelectedStatus(value ?? task.status)}
          />
        </Form.Item>
      </Flex>
    </AIModal>
  );
}
