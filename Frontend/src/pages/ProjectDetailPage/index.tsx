import { useState } from "react";
import { Flex, Tabs } from "antd";
import { useParams, Link, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { AppLayout } from "@/layouts";
import { useProjectDetailById, useProjectDetailBySlug } from "@/hooks";
import { ROUTE } from "@/constants";
import { ProjectInfoCard } from "./components/ProjectInfoCard";
import { EditProjectModal } from "./modals/EditProjectModal";
import { MyTaskTab } from "./tabs/MyTaskTab";
import { MembersTab } from "./tabs/MembersTab";
import { NotFound } from "@/components";

export default function ProjectDetailPage() {
  const { t } = useTranslation();
  const { slug } = useParams<{ slug: string }>();
  const navigate = useNavigate();

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

  // ── Tab state ──
  const [activeTab, setActiveTab] = useState("myTasks");

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
            onChange={setActiveTab}
            items={[
              {
                key: "myTasks",
                label: t("projectDetailPage.myTasks.title"),
                children: <MyTaskTab projectId={projectId} />,
              },
              {
                key: "taskList",
                label: t("projectDetailPage.taskList.title"),
                children: (
                  <Flex justify="center" style={{ padding: 40 }}>
                    <NotFound
                      title={t("projectDetailPage.taskList.comingSoon")}
                    />
                  </Flex>
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
