import { useParams } from "react-router-dom";
import { Tabs, Typography, Space, Skeleton, Card, Spin } from "antd";
import {
  FolderOutlined,
  UserOutlined,
  SettingOutlined,
} from "@ant-design/icons";
import { useTeam, useProject } from "@/hooks";
import MainLayout from "@/layouts";
import { ProjectList, MemberList, SettingList } from "./components";

const { Text } = Typography;

export default function TeamPageDetail() {
  const { id } = useParams<{ id: string }>();

  const teamDetailQuery = useTeam().getDetail(id!);
  const projectsQuery = useProject().getByTeam(id!);
  const membersQuery = useTeam().getMembers(id!);
  const meQuery = useTeam().me(id!);

  const { data: teamDetail, isLoading: isTeamLoading } = teamDetailQuery;
  const { data: projects, isLoading: isProjectsLoading } = projectsQuery;
  const { data: members, isLoading: isMembersLoading } = membersQuery;
  const { data: me } = meQuery;

  const breadcrumbItems = [
    { title: "Nhóm của tôi", href: "/teams" },
    { title: teamDetail?.name || "Chi tiết nhóm" },
  ];

  const tabItems = [
    {
      key: "projects",
      label: (
        <span>
          <FolderOutlined /> Dự án
        </span>
      ),
      children: (
        <ProjectList projects={projects} isLoading={isProjectsLoading} />
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
          members={members}
          isLoading={isMembersLoading}
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
      children: <SettingList teamId={id!} teamName={teamDetail?.name || ""} />,
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
          <Spin spinning={isProjectsLoading || isMembersLoading}>
            <Tabs defaultActiveKey="projects" items={tabItems} />
          </Spin>
        </Card>
      </Space>
    </MainLayout>
  );
}
