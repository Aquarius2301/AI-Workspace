import { useState } from "react";
import { Button, Input, Flex, Typography, Empty, Grid, Progress } from "antd";
import { TeamOutlined } from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useTeamProjects, useSearch } from "@/hooks";
import { AIList, AIVisibilitySelect, AIVisibilityTag } from "@/components";
import type { TeamProjectItem, TeamRole } from "@/types";
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

  const { data: projectsData, isLoading } = useTeamProjects(
    teamId,
    queryParams.search,
    queryParams.visibility,
    queryParams.page,
    queryParams.pageSize,
  );

  const hasSearchQuery = !!queryParams.search;
  const isDataEmpty = !isLoading && projectsData && projectsData.total === 0;
  const showSearchBar = !(
    isDataEmpty &&
    !hasSearchQuery &&
    !queryParams.visibility
  );

  return (
    <Flex vertical gap={16}>
      {showSearchBar && (
        <Flex wrap="wrap" gap={12} align="center">
          <Input.Search
            placeholder={t("teamDetail.projects.searchProjects")}
            allowClear
            value={searchProps.search}
            onChange={(e) => searchProps.onSearchChange(e.target.value)}
            onSearch={(val) => searchProps.onSearchChange(val)}
            style={{
              maxWidth: isMobile ? "100%" : 360,
              flex: isMobile ? 1 : undefined,
            }}
          />
          <AIVisibilitySelect
            value={queryParams.visibility}
            onChange={(visibility) =>
              visibilityProps.onVisibilityChange(visibility)
            }
          />
        </Flex>
      )}
      {(role === "Admin" || role === "CoAdmin") && (
        <Button
          style={{ maxWidth: isMobile ? "100%" : 100 }}
          type="primary"
          onClick={() => setCreateModalOpen(true)}
        >
          {t("teamDetail.projects.create.title")}
        </Button>
      )}

      <AIList<TeamProjectItem>
        data={projectsData?.items ?? []}
        itemKey={(item) => item.id}
        renderItem={(project) => ({
          header: (
            <Text strong style={{ fontSize: 15 }}>
              {project.name}
            </Text>
          ),
          content: (
            <Flex vertical gap={4}>
              {project.description && (
                <Text type="secondary">{project.description}</Text>
              )}
              <Flex align="center" gap={8}>
                <Text type="secondary" style={{ fontSize: 13 }}>
                  {t("teamDetail.projects.createdBy")} {project.creatorName}
                </Text>
                <AIVisibilityTag visibility={project.visibility} size={12} />
              </Flex>
            </Flex>
          ),
          footer: (
            <Flex justify="space-between" align="center" wrap="wrap" gap={8}>
              <Text type="secondary" style={{ fontSize: 13 }}>
                <TeamOutlined /> {project.memberCount}{" "}
                {t("teamDetail.projects.memberCount")}
              </Text>
              {project.totalTaskCount > 0 ? (
                <Flex align="center" gap={8} style={{ minWidth: 160 }}>
                  <Progress
                    percent={Math.round(
                      (project.completedTaskCount / project.totalTaskCount) *
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
                    {t("teamDetail.projects.completed")}
                  </Text>
                </Flex>
              ) : (
                <Text type="secondary" style={{ fontSize: 13 }}>
                  {t("teamDetail.projects.noTasks")}
                </Text>
              )}
            </Flex>
          ),
          rightSide: project.canView && (
            <Button
              onClick={() =>
                navigate(`/teams/${teamId}/projects/${project.id}`)
              }
            >
              {t("teamDetail.projects.view")}
            </Button>
          ),
        })}
        isLoading={isLoading}
        paginationProps={{
          ...paginationProps,
          total: projectsData?.total ?? 0,
        }}
        hasSearchQuery={hasSearchQuery}
        empty={{
          icon: Empty.PRESENTED_IMAGE_SIMPLE,
          title: t("teamDetail.projects.empty"),
          subTitle: t("teamDetail.projects.emptyDescription"),
        }}
        notFound={{
          icon: Empty.PRESENTED_IMAGE_SIMPLE,
          title: t("teamDetail.projects.notFound"),
          subTitle: t("teamDetail.projects.notFoundDescription"),
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
