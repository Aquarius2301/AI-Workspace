import { useEffect, useState } from "react";
import { Flex, Tabs } from "antd";
import {
  useParams,
  Link,
  useNavigate,
  useSearchParams,
} from "react-router-dom";
import { useTranslation } from "react-i18next";
import { AppLayout } from "@/layouts";
import { useProjectDetailById, useProjectDetailBySlug } from "@/hooks";
import { ROUTE } from "@/constants";
import { ProjectInfoCard } from "./components/ProjectInfoCard";
import { EditProjectModal } from "./modals/EditProjectModal";
import { MyTaskTab } from "./tabs/MyTaskTab";
import { MembersTab } from "./tabs/MembersTab";
import { TaskTab } from "./tabs/TaskTab";
import { NotFound } from "@/components";

export default function ProjectDetailPage() {
  const { t } = useTranslation();
  const { slug } = useParams<{ slug: string }>();
  const navigate = useNavigate();
  const [searchParams, setSearchParams] = useSearchParams();

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
  const [activeTab, setActiveTab] = useState(taskIdParam ? "myTasks" : "myTasks");

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
                children: <TaskTab projectId={projectId} />,
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
