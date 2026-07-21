import { useState } from "react";
import { Flex, Input, Grid, Empty, Button } from "antd";
import { PlusOutlined } from "@ant-design/icons";
import { useTranslation } from "react-i18next";
import { AITaskPrioritySelect } from "@/components";
import { useSearch, useTasksByProject } from "@/hooks";
import { TaskList } from "../components/TaskList";
import { TaskListMobile } from "../components/TaskListMobile";
import { CreateTaskModal } from "../modals/CreateTaskModal";

const { useBreakpoint } = Grid;

interface TaskTabProps {
  projectId: string;
  canEdit?: boolean;
}

export function TaskTab({ projectId, canEdit }: TaskTabProps) {
  const { t } = useTranslation();
  const screens = useBreakpoint();
  const isMobile = !screens.md;

  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);

  const { searchProps, priorityProps, queryParams } = useSearch({
    hasPriorityFilter: true,
  });

  const { data: tasks, isLoading } = useTasksByProject(
    projectId,
    queryParams.search,
    queryParams.priority,
  );

  const hasSearchQuery = !!queryParams.search || !!queryParams.priority;
  const isDataEmpty = !isLoading && tasks && tasks.length === 0;
  const showSearchBar = !(isDataEmpty && !hasSearchQuery);

  // ── Not found state ──
  if (!isLoading && tasks && tasks.length === 0 && hasSearchQuery) {
    return (
      <Flex vertical gap={16}>
        {showSearchBar && (
          <Flex gap={12} align="center" wrap>
            <Input
              placeholder={t("projectDetailPage.taskList.search")}
              allowClear
              value={searchProps.search}
              onChange={(e) => searchProps.onSearchChange(e.target.value)}
              style={{ maxWidth: 360 }}
            />
            <AITaskPrioritySelect
              value={queryParams.priority}
              onChange={priorityProps.onPriorityChange}
              allowClear
            />
            {canEdit && (
              <Button
                type="primary"
                icon={<PlusOutlined />}
                onClick={() => setIsCreateModalOpen(true)}
              >
                {t("projectDetailPage.createTask.title")}
              </Button>
            )}
          </Flex>
        )}
        <Flex justify="center" style={{ padding: 40 }}>
          <Empty
            description={
              <Flex vertical gap={4}>
                <span>{t("projectDetailPage.taskList.notFound.title")}</span>
                <span style={{ fontSize: 13 }}>
                  {t("projectDetailPage.taskList.notFound.description")}
                </span>
              </Flex>
            }
          />
        </Flex>

        {isCreateModalOpen && (
          <CreateTaskModal
            isOpen={true}
            onClose={() => setIsCreateModalOpen(false)}
            projectId={projectId}
          />
        )}
      </Flex>
    );
  }

  // ── Empty state ──
  if (!isLoading && tasks && tasks.length === 0 && !hasSearchQuery) {
    return (
      <Flex vertical gap={16}>
        {showSearchBar && (
          <Flex gap={12} align="center" wrap>
            <Input
              placeholder={t("projectDetailPage.taskList.search")}
              allowClear
              value={searchProps.search}
              onChange={(e) => searchProps.onSearchChange(e.target.value)}
              style={{ maxWidth: 360 }}
            />
            <AITaskPrioritySelect
              value={queryParams.priority}
              onChange={priorityProps.onPriorityChange}
              allowClear
            />
            {canEdit && (
              <Button
                type="primary"
                icon={<PlusOutlined />}
                onClick={() => setIsCreateModalOpen(true)}
              >
                {t("projectDetailPage.createTask.title")}
              </Button>
            )}
          </Flex>
        )}
        <Flex justify="center" style={{ padding: 40 }}>
          <Empty
            description={
              <Flex vertical gap={4}>
                <span>{t("projectDetailPage.taskList.empty.title")}</span>
                <span style={{ fontSize: 13 }}>
                  {t("projectDetailPage.taskList.empty.description")}
                </span>
              </Flex>
            }
          />
        </Flex>

        {isCreateModalOpen && (
          <CreateTaskModal
            isOpen={true}
            onClose={() => setIsCreateModalOpen(false)}
            projectId={projectId}
          />
        )}
      </Flex>
    );
  }

  return (
    <Flex vertical gap={16}>
      {/* ── Filter bar ── */}
      {showSearchBar && (
        <Flex gap={12} align="center" wrap>
          <Input
            placeholder={t("projectDetailPage.taskList.search")}
            allowClear
            value={searchProps.search}
            onChange={(e) => searchProps.onSearchChange(e.target.value)}
            style={{ maxWidth: 360 }}
          />
          <AITaskPrioritySelect
            value={queryParams.priority}
            onChange={priorityProps.onPriorityChange}
            allowClear
          />
          {canEdit && (
            <Button
              type="primary"
              icon={<PlusOutlined />}
              onClick={() => setIsCreateModalOpen(true)}
            >
              {t("projectDetailPage.createTask.title")}
            </Button>
          )}
        </Flex>
      )}

      {/* ── Kanban board ── */}
      {isMobile ? (
        <TaskListMobile tasks={tasks ?? []} isLoading={isLoading} />
      ) : (
        <TaskList tasks={tasks ?? []} isLoading={isLoading} />
      )}

      {/* ── Create task modal ── */}
      {isCreateModalOpen && (
        <CreateTaskModal
          isOpen={true}
          onClose={() => setIsCreateModalOpen(false)}
          projectId={projectId}
        />
      )}
    </Flex>
  );
}
