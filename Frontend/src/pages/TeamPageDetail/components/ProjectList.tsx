import { useTranslation } from "react-i18next";
import type { TeamProjectItem, TeamRole } from "@/types";
import { Button, Space, Tag } from "antd";
import Text from "antd/es/typography/Text";
import {
  SearchPagination,
  type CustomColumnsType,
  type PaginationProps,
  type SearchProps,
} from "@/components";
import Paragraph from "antd/es/typography/Paragraph";
import { useMemo, useState } from "react";
import { CreateProjectModal } from "../modals";

interface ProjectListProps {
  hasHadData: boolean;
  isLoading: boolean;
  data: TeamProjectItem[];
  searchProps: SearchProps;
  paginationProps: PaginationProps;
  teamId: string;
  role: TeamRole;
}

export function ProjectList({
  hasHadData,
  isLoading,
  data,
  searchProps,
  paginationProps,
  teamId,
  role,
}: ProjectListProps) {
  const { t } = useTranslation();
  const projectColumns = useMemo<CustomColumnsType<TeamProjectItem>>(
    () => [
      {
        title: t("project.listTitle"),
        dataIndex: "name",
        key: "name",
        render: (name: string) => <Text strong>{name}</Text>,
      },
      {
        title: t("project.listDescription"),
        dataIndex: "description",
        key: "description",
        render: (description: string) => (
          <Paragraph
            ellipsis={{ rows: 2, expandable: true }}
            style={{ margin: 0 }}
          >
            {description}
          </Paragraph>
        ),
      },
      {
        title: t("project.listVisibility"),
        dataIndex: "visibility",
        key: "visibility",
        render: (visibility: string) => (
          <Tag color={visibility === "Public" ? "blue" : "default"}>
            {visibility === "Public" ? t("home.public") : t("home.private")}
          </Tag>
        ),
      },
      {
        title: t("project.listActions"),
        noShowMobileTitle: true,
        render: (_, record) =>
          record.canView && (
            <Button style={{ width: "100%" }} onClick={() => {}}>
              {t("team.view")}
            </Button>
          ),
      },
    ],
    [t],
  );

  const [createProjectModal, setCreateProjectModal] = useState(false);

  return (
    <Space vertical style={{ width: "100%" }} size={12}>
      {(role == "Admin" || role == "Leader") && (
        <Button type="primary" onClick={() => setCreateProjectModal(true)}>
          {t("project.createProjectButton")}
        </Button>
      )}
      <SearchPagination<TeamProjectItem>
        search={
          hasHadData
            ? { ...searchProps, placeholder: t("project.searchPlaceholder") }
            : undefined
        }
        pagination={{ ...paginationProps }}
        tableProps={{
          dataSource: data,
          columns: projectColumns,
          rowKey: "id",
          loading: isLoading,
          locale: {
            emptyText: t("project.listEmpty"),
          },
        }}
      />

      <CreateProjectModal
        isOpen={createProjectModal}
        onClose={() => setCreateProjectModal(false)}
        teamId={teamId}
      />
    </Space>
  );
}
