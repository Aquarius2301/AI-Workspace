import { Form, Input, DatePicker, Select, Typography, App } from "antd";
import { useTranslation } from "react-i18next";
import { AIModal, AITaskPrioritySelect } from "@/components";
import type { TaskItemResult, UpdateTaskRequest } from "@/types";
import { useProjectMembers, useUpdateTask } from "@/hooks";
import { getErrorMessage, getFormFieldErrors } from "@/utils";
import dayjs from "dayjs";

const { Text } = Typography;

interface EditTaskModalProps {
  isOpen: boolean;
  onClose: () => void;
  projectId: string;
  task: TaskItemResult;
}

export function EditTaskModal({
  isOpen,
  onClose,
  projectId,
  task,
}: EditTaskModalProps) {
  const { t } = useTranslation();
  const [form] = Form.useForm<UpdateTaskRequest>();
  const { message } = App.useApp();
  const updateTask = useUpdateTask(projectId);

  const dueDateValue = Form.useWatch("dueDate", form);
  const isPastDueDate =
    dueDateValue && dayjs(dueDateValue).isBefore(dayjs(), "day");

  const { data: memberData } = useProjectMembers(
    projectId,
    undefined,
    undefined,
    1,
    100,
    isOpen,
  );
  const members = memberData?.items ?? [];

  const handleUpdate = async (values: UpdateTaskRequest) => {
    try {
      const dueDate = values.dueDate ? dayjs(values.dueDate) : null;

      const payload: UpdateTaskRequest = {
        ...values,
        dueDate: dueDate?.toISOString() ?? undefined,
      };

      await updateTask.mutateAsync({ taskId: task.id, data: payload });
      message.success(t("projectDetailPage.createTask.success"));
      onClose();
      form.resetFields();
    } catch (error) {
      const fieldErrors = getFormFieldErrors<UpdateTaskRequest>(error);
      if (fieldErrors.length > 0) {
        form.setFields(fieldErrors);
      } else {
        message.error(getErrorMessage(error));
      }
    }
  };

  return (
    <AIModal
      title={t("projectDetailPage.createTask.title")}
      open={isOpen}
      onOk={() => form.submit()}
      onCancel={() => {
        onClose();
        form.resetFields();
      }}
      isLoading={updateTask.isPending}
      footer={[{ type: "cancel" }, { type: "update" }]}
    >
      <Form
        form={form}
        layout="vertical"
        autoComplete="off"
        onFinish={handleUpdate}
        style={{ marginTop: 16 }}
        initialValues={{
          title: task.title,
          description: task.description ?? undefined,
          priority: task.priority,
          assignedToId: task.assignedToId ?? undefined,
          dueDate: task.dueDate ? dayjs(task.dueDate) : undefined,
        }}
      >
        <Form.Item
          name="title"
          label={t("projectDetailPage.createTask.titleLabel")}
          rules={[
            {
              required: true,
              message: t("projectDetailPage.createTask.titleRequired"),
            },
            { max: 200, message: t("projectDetailPage.createTask.titleMax") },
          ]}
        >
          <Input
            placeholder={t("projectDetailPage.createTask.titlePlaceholder")}
          />
        </Form.Item>

        <Form.Item
          name="description"
          label={t("projectDetailPage.createTask.descriptionLabel")}
          rules={[
            {
              max: 2000,
              message: t("projectDetailPage.createTask.descriptionMax"),
            },
          ]}
        >
          <Input.TextArea
            placeholder={t(
              "projectDetailPage.createTask.descriptionPlaceholder",
            )}
            rows={3}
          />
        </Form.Item>

        <Form.Item
          name="priority"
          label={t("projectDetailPage.createTask.priorityLabel")}
          rules={[{ required: true }]}
        >
          <AITaskPrioritySelect allowClear={false} style={{ width: "100%" }} />
        </Form.Item>

        <Form.Item
          name="assignedToId"
          label={t("projectDetailPage.createTask.assigneeLabel")}
        >
          <Select
            allowClear
            placeholder={t("projectDetailPage.createTask.assigneePlaceholder")}
            options={members.map((m) => ({
              label: m.userName,
              value: m.userId,
            }))}
          />
        </Form.Item>

        <Form.Item
          name="dueDate"
          label={t("projectDetailPage.createTask.dueDateLabel")}
        >
          <DatePicker
            style={{ width: "100%" }}
            placeholder={t("projectDetailPage.createTask.dueDatePlaceholder")}
          />
        </Form.Item>

        {isPastDueDate && (
          <Text
            type="warning"
            style={{
              fontSize: 13,
              marginTop: -12,
              marginBottom: 12,
              display: "block",
            }}
          >
            {t("projectDetailPage.createTask.dueDatePastWarning")}
          </Text>
        )}
      </Form>
    </AIModal>
  );
}
