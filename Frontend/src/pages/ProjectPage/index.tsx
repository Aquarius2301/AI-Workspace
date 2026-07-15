import { Input, Flex, Empty, Typography, Space } from "antd";
import {
  UserOutlined,
  CheckCircleOutlined,
  TeamOutlined,
} from "@ant-design/icons";
import {
  AICardItem,
  AIList,
  AIVisibilitySelect,
  AIVisibilityTag,
} from "@/components";
import { useSearch, useMyProjectList } from "@/hooks";
import { useTranslation } from "react-i18next";
import { AppLayout } from "@/layouts";
import type { MyProjectItem } from "@/types";
import { useNavigate } from "react-router-dom";
import { ROUTE } from "@/constants";

const { Text } = Typography;

export default function ProjectPage() {
  const { t } = useTranslation();
  const breadcrumbItems = [{ title: t("projectPage.title") }];
  const { searchProps, visibilityProps, paginationProps, queryParams } =
    useSearch({ hasVisibilityFilter: true });
  const navigate = useNavigate();

  const { data, isLoading } = useMyProjectList(
    queryParams.search,
    queryParams.visibility,
    queryParams.page,
    queryParams.pageSize,
  );

  const hasSearchQuery = !!queryParams.search || !!queryParams.visibility;
  const isDataEmpty = !isLoading && data && data.total === 0;
  const showSearchBar = !(isDataEmpty && !hasSearchQuery);

  return (
    <AppLayout breadcrumbItems={breadcrumbItems} title={t("projectPage.title")}>
      <Flex vertical gap={16}>
        {/* ── Header: search + visibility filter ── */}
        {showSearchBar && (
          <Flex align="center" justify="space-between" gap={12} wrap>
            <Flex gap={12} align="center" wrap>
              <Input
                placeholder={t("projectPage.searchProjects")}
                allowClear
                value={searchProps.search}
                onChange={(e) => searchProps.onSearchChange(e.target.value)}
                style={{ maxWidth: 360 }}
              />
              <AIVisibilitySelect
                value={queryParams.visibility}
                onChange={visibilityProps.onVisibilityChange}
                allowClear
              />
            </Flex>
          </Flex>
        )}

        {/* ── Project list ── */}
        <AIList<MyProjectItem>
          data={data?.items ?? []}
          itemKey={(item) => item.id}
          renderItem={(project) => (
            <AICardItem
              header={
                <Flex align="center" gap={8}>
                  <Text strong style={{ fontSize: 15 }}>
                    {project.name}
                  </Text>
                  <AIVisibilityTag visibility={project.visibility} />
                </Flex>
              }
              content={
                <Flex vertical gap={4}>
                  {project.description && (
                    <Text type="secondary">{project.description}</Text>
                  )}
                  <Text type="secondary" style={{ fontSize: 13 }}>
                    <TeamOutlined /> {project.teamName}
                  </Text>
                </Flex>
              }
              footer={
                <Flex justify="space-between">
                  <Space size={16}>
                    <Text type="secondary" style={{ fontSize: 13 }}>
                      <UserOutlined /> {project.memberCount}{" "}
                      {t("projectPage.memberCount")}
                    </Text>
                    <Text type="secondary" style={{ fontSize: 13 }}>
                      <CheckCircleOutlined /> {project.completedTaskCount}/
                      {project.totalTaskCount} {t("projectPage.completed")}
                    </Text>
                  </Space>
                  <Text type="secondary" style={{ fontSize: 13 }}>
                    {t("projectPage.view")}
                  </Text>
                </Flex>
              }
              isHoverable
              onClick={() => navigate(`${ROUTE.PROJECT}/${project.slug}`)}
            />
          )}
          isLoading={isLoading}
          paginationProps={{
            ...paginationProps,
            total: data?.total ?? 0,
          }}
          notFound={{
            icon: Empty.PRESENTED_IMAGE_SIMPLE,
            title: t("projectPage.notFound.title"),
            subTitle: t("projectPage.notFound.description"),
          }}
          empty={{
            icon: Empty.PRESENTED_IMAGE_SIMPLE,
            title: t("projectPage.empty.title"),
            subTitle: t("projectPage.empty.description"),
          }}
          hasSearchQuery={hasSearchQuery}
        />
      </Flex>
    </AppLayout>
  );
}
