import { useState } from "react";
import {
  Button,
  Input,
  Flex,
  Typography,
  Empty,
  Grid,
  Progress,
  Space,
} from "antd";
import { TeamOutlined } from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useProjectList, useSearch } from "@/hooks";
import {
  AICardItem,
  AIList,
  AIVisibilitySelect,
  AIVisibilityTag,
} from "@/components";
import type { ProjectItem, TeamRole } from "@/types";
import { CreateProjectModal } from "../modals/CreateProjectModal";
import { getPercentage } from "@/utils";

const { useBreakpoint } = Grid;

const { Text } = Typography;

interface ProjectTabProps {
  teamId: string;
  role: TeamRole;
}

export function ProjectTab({ teamId, role }: ProjectTabProps) {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const screens = useBreakpoint();
  const isMobile = !screens.md;

  const [createModalOpen, setCreateModalOpen] = useState(false);

  const { searchProps, visibilityProps, paginationProps, queryParams } =
    useSearch({
      hasVisibilityFilter: true,
    });

  const { data: projectsData, isLoading } = useProjectList(
    teamId,
    queryParams.search,
    queryParams.visibility,
    queryParams.page,
    queryParams.pageSize,
  );

  const hasSearchQuery = !!queryParams.search || !!queryParams.visibility;
  const isDataEmpty = !isLoading && projectsData && projectsData.total === 0;
  const showSearchBar = !(
    isDataEmpty &&
    !hasSearchQuery &&
    !queryParams.visibility
  );

  return (
    <Flex vertical gap={16}>
      <Flex wrap justify="space-between" gap={8}>
        {showSearchBar && (
          <Flex gap={12} align="center">
            <Input
              placeholder={t("teamDetailPage.projects.searchProjects")}
              allowClear
              value={searchProps.search}
              onChange={(e) => searchProps.onSearchChange(e.target.value)}
              style={{
                maxWidth: isMobile ? "100%" : 360,
                flex: isMobile ? 1 : undefined,
              }}
            />
            <AIVisibilitySelect
              value={queryParams.visibility}
              allowClear
              onChange={(visibility) =>
                visibilityProps.onVisibilityChange(visibility)
              }
            />
          </Flex>
        )}

        {(role === "admin" || role === "coAdmin") && (
          <Button
            style={{ maxWidth: isMobile ? "100%" : 100 }}
            type="primary"
            onClick={() => setCreateModalOpen(true)}
          >
            {t("teamDetailPage.projects.createProject.title")}
          </Button>
        )}
      </Flex>
      <AIList<ProjectItem>
        data={projectsData?.items ?? []}
        itemKey={(item) => item.id}
        renderItem={(project) => (
          <AICardItem
            header={
              <Flex justify="space-between" wrap>
                <Text strong style={{ fontSize: 15 }}>
                  {project.name}
                </Text>
                <AIVisibilityTag visibility={project.visibility} size={12} />
              </Flex>
            }
            content={
              <Flex vertical gap={4}>
                {project.description && (
                  <Text type="secondary">{project.description}</Text>
                )}

                <Text type="secondary" style={{ fontSize: 13 }}>
                  {t("teamDetailPage.projects.createdBy")} {project.creatorName}
                </Text>
              </Flex>
            }
            footer={
              <Flex justify="space-between" align="center" wrap="wrap" gap={8}>
                <Space vertical>
                  {project.totalTaskCount > 0 ? (
                    <Flex align="center" gap={8} style={{ minWidth: 160 }}>
                      <Progress
                        percent={Math.round(
                          (project.completedTaskCount /
                            project.totalTaskCount) *
                            100,
                        )}
                        size="small"
                        style={{ flex: 1, marginBottom: 0 }}
                        format={() => {
                          return `${project.completedTaskCount}/${project.totalTaskCount} (${getPercentage(project.completedTaskCount, project.totalTaskCount)}%)`;
                        }}
                      />
                      <Text
                        type="secondary"
                        style={{ fontSize: 12, whiteSpace: "nowrap" }}
                      >
                        {t("teamDetailPage.projects.completed")}
                      </Text>
                    </Flex>
                  ) : (
                    <Text type="secondary" style={{ fontSize: 13 }}>
                      {t("teamDetailPage.projects.noTasks")}
                    </Text>
                  )}
                  <Text type="secondary" style={{ fontSize: 13 }}>
                    <TeamOutlined /> {project.memberCount}{" "}
                    {t("teamDetailPage.projects.memberCount")}
                  </Text>
                </Space>

                {project.canView ? (
                  <Text type="secondary" style={{ fontSize: 13 }}>
                    {t("teamDetailPage.projects.view")}
                  </Text>
                ) : (
                  <Text type="danger" style={{ fontSize: 13 }}>
                    {t("teamDetailPage.projects.unviewable")}
                  </Text>
                )}
              </Flex>
            }
            isHoverable={project.canView}
            onClick={() =>
              project.canView && (
                <Button
                  onClick={() =>
                    navigate(`/teams/${teamId}/projects/${project.slug}`)
                  }
                >
                  {t("teamDetailPage.projects.view")}
                </Button>
              )
            }
          />
        )}
        isLoading={isLoading}
        paginationProps={{
          ...paginationProps,
          total: projectsData?.total ?? 0,
        }}
        hasSearchQuery={hasSearchQuery}
        empty={{
          icon: Empty.PRESENTED_IMAGE_SIMPLE,
          title: t("teamDetailPage.projects.empty.title"),
          subTitle: t("teamDetailPage.projects.empty.description"),
        }}
        notFound={{
          icon: Empty.PRESENTED_IMAGE_SIMPLE,
          title: t("teamDetailPage.projects.notFound.title"),
          subTitle: t("teamDetailPage.projects.notFound.description"),
        }}
      />
      <CreateProjectModal
        isOpen={createModalOpen}
        onClose={() => setCreateModalOpen(false)}
        teamId={teamId}
      />
    </Flex>
  );
}
