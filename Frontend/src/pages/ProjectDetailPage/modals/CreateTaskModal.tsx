import { Form, Input, DatePicker, Select, Typography, App } from "antd";
import { useTranslation } from "react-i18next";
import { AIModal, AITaskPrioritySelect } from "@/components";
import type { CreateTaskRequest, TaskPriority } from "@/types";
import { TASK_PRIORITY } from "@/types";
import { useCreateTask, useProjectMembers } from "@/hooks";
import { getErrorMessage, getFormFieldErrors } from "@/utils";
import dayjs from "dayjs";

const { Text } = Typography;

interface CreateTaskModalProps {
  isOpen: boolean;
  onClose: () => void;
  projectId: string;
}

export function CreateTaskModal({
  isOpen,
  onClose,
  projectId,
}: CreateTaskModalProps) {
  const { t } = useTranslation();
  const [form] = Form.useForm<CreateTaskRequest>();
  const { message } = App.useApp();
  const createTask = useCreateTask(projectId);

  // ── Watch due date for past-date warning ──
  const dueDateValue = Form.useWatch("dueDate", form);
  const isPastDueDate =
    dueDateValue && dayjs(dueDateValue).isBefore(dayjs(), "day");

  // ── Fetch project members for assignee dropdown ──
  const { data: memberData } = useProjectMembers(projectId, undefined, undefined, 1, 100, isOpen);
  const members = memberData?.items ?? [];

  // ── Create task ──
  const handleCreate = async (values: CreateTaskRequest) => {
    try {
      const dueDate = values.dueDate ? dayjs(values.dueDate) : null;

      const payload: CreateTaskRequest = {
        ...values,
        dueDate: dueDate?.toISOString() ?? undefined,
      };
      await createTask.mutateAsync(payload);
      message.success(t("projectDetailPage.createTask.success"));
      onClose();
      form.resetFields();
    } catch (error) {
      const fieldErrors = getFormFieldErrors<CreateTaskRequest>(error);
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
      isLoading={createTask.isPending}
      footer={[{ type: "cancel" }, { type: "create" }]}
    >
      <Form
        form={form}
        layout="vertical"
        autoComplete="off"
        onFinish={handleCreate}
        style={{ marginTop: 16 }}
        initialValues={{ priority: TASK_PRIORITY[1] }} // default: medium
      >
        {/* ── Title ── */}
        <Form.Item
          name="title"
          label={t("projectDetailPage.createTask.titleLabel")}
          rules={[
            {
              required: true,
              message: t("projectDetailPage.createTask.titleRequired"),
            },
            {
              max: 200,
              message: t("projectDetailPage.createTask.titleMax"),
            },
          ]}
        >
          <Input
            placeholder={t("projectDetailPage.createTask.titlePlaceholder")}
          />
        </Form.Item>

        {/* ── Description ── */}
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

        {/* ── Priority ── */}
        <Form.Item
          name="priority"
          label={t("projectDetailPage.createTask.priorityLabel")}
          rules={[{ required: true }]}
        >
          <AITaskPrioritySelect
            allowClear={false}
            style={{ width: "100%" }}
          />
        </Form.Item>

        {/* ── Assignee ── */}
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

        {/* ── Due Date ── */}
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
          <Text type="warning" style={{ fontSize: 13, marginTop: -12, marginBottom: 12, display: "block" }}>
            {t("projectDetailPage.createTask.dueDatePastWarning")}
          </Text>
        )}
      </Form>
    </AIModal>
  );
}
