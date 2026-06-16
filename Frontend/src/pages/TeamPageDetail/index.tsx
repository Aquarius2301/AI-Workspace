import { useParams } from "react-router-dom";
import { Tabs, Typography, Empty, Space } from "antd";
import {
  FolderOutlined,
  UserOutlined,
  SettingOutlined,
} from "@ant-design/icons";
import { useTeam, useProject } from "@/hooks";
import MainLayout from "@/layouts";
import { ProjectList, MemberList } from "./components";

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
    { title: "Nhóm của tôi", href: "/team" },
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
        <ProjectList
          projects={projects}
          isProjectsLoading={isProjectsLoading}
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
          members={members}
          isMembersLoading={isMembersLoading}
          userId={me?.userId || ""}
        />
      ),
    },
    {
      key: "settings",
      label: (
        <span>
          <SettingOutlined /> Cài đặt
        </span>
      ),
      children: <Empty description="Tính năng đang phát triển" />,
    },
  ];

  return (
    <MainLayout isLoading={isTeamLoading} breadcrumbItems={breadcrumbItems}>
      <Space vertical size={16} style={{ width: "100%" }}>
        <Typography.Title level={4} style={{ margin: 0 }}>
          {teamDetail?.name || "Chi tiết nhóm"}
        </Typography.Title>
        {teamDetail?.description && (
          <Text type="secondary">{teamDetail.description}</Text>
        )}
        <Tabs defaultActiveKey="projects" items={tabItems} />
      </Space>
    </MainLayout>
  );
}
