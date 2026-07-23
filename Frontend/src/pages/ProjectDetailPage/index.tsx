import { useEffect, useState } from "react";
import { Flex, Tabs, Typography, Input, App } from "antd";
import {
  useParams,
  Link,
  useNavigate,
  useSearchParams,
} from "react-router-dom";
import { useTranslation } from "react-i18next";
import { AppLayout } from "@/layouts";
import {
  useProjectDetailById,
  useProjectDetailBySlug,
  useDeleteProject,
} from "@/hooks";
import { ROUTE } from "@/constants";
import { AIModal } from "@/components";
import { ProjectInfoCard } from "./components/ProjectInfoCard";
import { EditProjectModal } from "./modals/EditProjectModal";
import { MyTaskTab } from "./tabs/MyTaskTab";
import { MembersTab } from "./tabs/MembersTab";
import { TaskTab } from "./tabs/TaskTab";
import { NotFound } from "@/components";
import { getErrorMessage } from "@/utils";

const { Text } = Typography;

export default function ProjectDetailPage() {
  const { t } = useTranslation();
  const { slug } = useParams<{ slug: string }>();
  const navigate = useNavigate();
  const { message } = App.useApp();
  const [searchParams, setSearchParams] = useSearchParams();
  const deleteProject = useDeleteProject();

  // ── Get project ID from slug ──
  const { data: slugData, isLoading: isSlugLoading } = useProjectDetailBySlug(
    slug!,
  );
  const projectId = slugData?.id;

  // ── Get project detail ──
  const { data: project, isLoading: isProjectLoading } = useProjectDetailById(
    projectId!,
    !!projectId,
  );

  const isLoading = isSlugLoading || isProjectLoading;

  const breadcrumbItems = [
    { title: <Link to={ROUTE.PROJECT}>{t("projectPage.title")}</Link> },
    { title: project?.name || "..." },
  ];

  // ── Read taskId from URL query params ──
  const taskIdParam = searchParams.get("taskId");

  // ── Tab state ──
  const [activeTab, setActiveTab] = useState(
    taskIdParam ? "myTasks" : "myTasks",
  );

  // ── Selected task ──
  const [selectedTaskId, setSelectedTaskId] = useState<string | undefined>(
    taskIdParam ?? undefined,
  );

  // ── Clean up taskId from URL after consuming ──
  useEffect(() => {
    if (taskIdParam) {
      searchParams.delete("taskId");
      setSearchParams(searchParams, { replace: true });
    }
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  // ── Edit modal state ──
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  // ── Delete project state ──
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
  const [deleteStep, setDeleteStep] = useState<"confirm" | "verify">("confirm");
  const [verifyName, setVerifyName] = useState("");

  const handleDeleteStart = () => {
    setDeleteStep("confirm");
    setIsDeleteModalOpen(true);
  };

  const handleDeleteConfirm = () => {
    setDeleteStep("verify");
  };

  const handleDeleteExecute = async () => {
    if (!projectId) return;

    try {
      await deleteProject.mutateAsync(projectId);
      message.success(t("projectDetailPage.deleteProject.success"));
      setIsDeleteModalOpen(false);
      navigate(ROUTE.PROJECT);
    } catch (error) {
      message.error(getErrorMessage(error));
    }
  };

  // ── Not found state ──
  if (!isLoading && !slugData) {
    return (
      <AppLayout breadcrumbItems={breadcrumbItems}>
        <Flex justify="center" style={{ minHeight: 400 }}>
          <NotFound
            title={t("projectDetailPage.notFound.title")}
            description={t("projectDetailPage.notFound.description")}
            buttonText={t("projectDetailPage.notFound.button")}
            onBack={() => navigate(ROUTE.PROJECT)}
          />
        </Flex>
      </AppLayout>
    );
  }

  return (
    <AppLayout
      breadcrumbItems={breadcrumbItems}
      title={`${t("projectPage.title")} ${project?.name ? " - " + project.name : ""}`}
    >
      <Flex vertical gap={16}>
        {/* ── Project info card ── */}
        <ProjectInfoCard
          isLoading={isLoading}
          name={project?.name!}
          description={project?.description!}
          visibility={project?.visibility!}
          creatorName={project?.creatorName!}
          teamName={project?.teamName!}
          canEdit={project?.canEdit}
          onEdit={() => setIsEditModalOpen(true)}
          onDelete={handleDeleteStart}
        />

        {/* ── Edit project modal ── */}
        {projectId && project && (
          <EditProjectModal
            isOpen={isEditModalOpen}
            onClose={() => setIsEditModalOpen(false)}
            projectId={projectId}
            initialName={project.name}
            initialDescription={project.description}
            initialVisibility={project.visibility}
          />
        )}

        {/* ── Delete project modal (2-step verification) ── */}
        {project && (
          <AIModal
            title={t("projectDetailPage.deleteProject.confirmTitle")}
            open={isDeleteModalOpen}
            onCancel={() => {
              setIsDeleteModalOpen(false);
              setVerifyName("");
              setDeleteStep("confirm");
            }}
            onOk={
              deleteStep === "confirm"
                ? handleDeleteConfirm
                : handleDeleteExecute
            }
            isLoading={deleteProject.isPending}
            footer={
              deleteStep === "confirm"
                ? [
                    { type: "cancel", text: t("modal.cancel") },
                    { type: "delete", text: t("modal.delete") },
                  ]
                : [
                    { type: "cancel", text: t("modal.cancel") },
                    {
                      type: "delete",
                      text: t("modal.delete"),
                      disabled: verifyName !== project.name,
                    },
                  ]
            }
          >
            {deleteStep === "confirm" ? (
              <Flex vertical gap={12}>
                <Text>
                  {t("projectDetailPage.deleteProject.confirmWarning")}
                </Text>
              </Flex>
            ) : (
              <Flex vertical gap={12}>
                <Text>
                  {t("projectDetailPage.deleteProject.verifyLabel", {
                    projectName: project.name,
                  })}
                </Text>
                <Input
                  placeholder={t(
                    "projectDetailPage.deleteProject.verifyPlaceholder",
                  )}
                  value={verifyName}
                  onChange={(e) => setVerifyName(e.target.value)}
                  status={
                    verifyName && verifyName !== project.name
                      ? "error"
                      : undefined
                  }
                />
                {verifyName && verifyName !== project.name && (
                  <Text type="danger" style={{ fontSize: 13 }}>
                    {t("projectDetailPage.deleteProject.verifyError")}
                  </Text>
                )}
              </Flex>
            )}
          </AIModal>
        )}

        {/* ── Tabs ── */}
        {projectId && (
          <Tabs
            activeKey={activeTab}
            onChange={(key) => {
              setActiveTab(key);
              setSelectedTaskId(undefined);
            }}
            items={[
              {
                key: "myTasks",
                label: t("projectDetailPage.myTasks.title"),
                children: (
                  <MyTaskTab
                    projectId={projectId}
                    selectedTaskId={selectedTaskId}
                  />
                ),
              },
              {
                key: "taskList",
                label: t("projectDetailPage.taskList.title"),
                children: (
                  <TaskTab projectId={projectId} canEdit={project?.canEdit} />
                ),
              },
              {
                key: "members",
                label: t("projectDetailPage.members.title"),
                children: <MembersTab projectId={projectId} />,
              },
            ]}
          />
        )}
      </Flex>
    </AppLayout>
  );
}
