import { useParams } from "react-router-dom";
import { Tabs, Typography, Space, Skeleton, Card } from "antd";
import {
  FolderOutlined,
  UserOutlined,
  SettingOutlined,
} from "@ant-design/icons";
import { useProject, useSearch, useTeam } from "@/hooks";
import MainLayout from "@/layouts";
import { ProjectList, MemberList, SettingList } from "./components";
import { useEffect, useState } from "react";

const { Text } = Typography;

export default function TeamPageDetail() {
  const { id } = useParams<{ id: string }>();

  const teamDetailQuery = useTeam().getDetail(id!);

  const meQuery = useTeam().me(id!);

  const { data: teamDetail, isLoading: isTeamLoading } = teamDetailQuery;

  const { data: me } = meQuery;

  const {
    searchProps: searchProjectProps,
    paginationProps: paginationProjectProps,
    queryParams: queryProjectParams,
  } = useSearch({});
  const [hasProjectHadData, setHasProjectHadData] = useState(false);

  const { getByTeam } = useProject();

  const { data: projects, isLoading: isProjectLoading } = getByTeam(
    teamDetail?.id || "",
    queryProjectParams.search,
    queryProjectParams.page,
    queryProjectParams.pageSize,
  );
  useEffect(() => {
    if (
      !isProjectLoading &&
      projects &&
      projects.total > 0 &&
      !hasProjectHadData
    ) {
      setHasProjectHadData(true);
    }
  }, [projects, isProjectLoading, hasProjectHadData]);

  const {
    searchProps: searchMemberProps,
    paginationProps: paginationMemberProps,
    roleProps: roleMemberProps,
    queryParams: queryMemberParams,
  } = useSearch({});
  const [hasMemberHadData, setHasMemberHadData] = useState(false);

  const { getMembers } = useTeam();

  const { data: members, isLoading: isMemberLoading } = getMembers(
    teamDetail?.id || "",
    queryMemberParams.search,
    queryMemberParams.role ?? undefined,
    queryMemberParams.page,
    queryMemberParams.pageSize,
  );

  useEffect(() => {
    if (!isMemberLoading && members && members.total > 0 && !hasMemberHadData) {
      setHasMemberHadData(true);
    }
  }, [members, isMemberLoading, hasMemberHadData]);

  const breadcrumbItems = [
    { title: "Nhóm của tôi", href: "/teams" },
    { title: teamDetail?.name || "Chi tiết nhóm" },
  ];

  const isLoading = isTeamLoading || isProjectLoading || isMemberLoading;

  const tabItems = [
    {
      key: "projects",
      label: (
        <span>
          <FolderOutlined /> Dự án
        </span>
      ),
      children: (
        <ProjectList
          data={projects?.items || []}
          isLoading={isProjectLoading}
          hasHadData={hasProjectHadData}
          searchProps={searchProjectProps}
          paginationProps={{
            ...paginationProjectProps,
            total: projects?.total || 0,
          }}
        />
      ),
    },
    {
      key: "members",
      label: (
        <span>
          <UserOutlined /> Thành viên
        </span>
      ),
      children: (
        <MemberList
          data={members?.items || []}
          isLoading={isMemberLoading}
          hasHadData={hasMemberHadData}
          searchProps={searchMemberProps}
          roleProps={roleMemberProps}
          paginationProps={{
            ...paginationMemberProps,
            total: members?.total || 0,
          }}
          userId={me?.userId || ""}
        />
      ),
    },
  ];

  if (me && (me.role == "Admin" || me.role == "Leader")) {
    tabItems.push({
      key: "settings",
      label: (
        <span>
          <SettingOutlined /> Cài đặt
        </span>
      ),
      children: (
        <SettingList
          teamId={id!}
          teamName={teamDetail?.name || ""}
          teamDescription={teamDetail?.description || ""}
          role={me.role}
        />
      ),
    });
  }

  return (
    <MainLayout breadcrumbItems={breadcrumbItems}>
      <Space vertical size={16} style={{ width: "100%" }}>
        <Card>
          {isTeamLoading ? (
            <Skeleton active={true} />
          ) : (
            <>
              <Typography.Title level={4} style={{ margin: 0 }}>
                {teamDetail?.name || "Chi tiết nhóm"}
              </Typography.Title>
              <Text type="secondary">{teamDetail?.description}</Text>
            </>
          )}
        </Card>
        <Card>
          <Tabs
            style={{
              pointerEvents: isLoading ? "none" : "auto",
              opacity: isLoading ? 0.5 : 1,
            }}
            defaultActiveKey="projects"
            items={tabItems}
          />
        </Card>
      </Space>
    </MainLayout>
  );
}
