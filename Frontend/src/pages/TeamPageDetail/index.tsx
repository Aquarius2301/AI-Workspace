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
import { useState } from "react";

const { Text, Title } = Typography;

export default function TeamPageDetail() {
  const { id } = useParams<{ id: string }>();
  const teamId = id!;

  // State kiểm soát Tab hiện tại để phục vụ Lazy Loading API
  const [activeTab, setActiveTab] = useState("projects");

  // ================= 1. CORE API (Luôn chạy khi vào trang) =================
  const { data: teamDetail, isLoading: isTeamLoading } =
    useTeam().getDetail(teamId);
  const { data: me, isLoading: isMeLoading } = useTeam().me(teamId);

  const userRole = me?.role || "Member";
  const isAdminOrLeader = userRole === "Admin" || userRole === "Leader";

  // ================= 2. TAB PROJECTS API (Chỉ chạy khi ở tab projects) =================
  const {
    searchProps: searchProjectProps,
    paginationProps: paginationProjectProps,
    queryParams: queryProjectParams,
  } = useSearch({});

  const { getByTeam } = useProject();
  const { data: projects, isLoading: isProjectLoading } = getByTeam(
    teamDetail?.id || "",
    queryProjectParams.search,
    queryProjectParams.page,
    queryProjectParams.pageSize,
    activeTab === "projects" && !!teamDetail?.id,
  );

  // ================= 3. TAB MEMBERS API (Chỉ chạy khi ở tab members) =================
  const {
    searchProps: searchMemberProps,
    paginationProps: paginationMemberProps,
    roleProps: roleMemberProps,
    queryParams: queryMemberParams,
  } = useSearch({});

  const { getMembers } = useTeam();
  const { data: members, isLoading: isMemberLoading } = getMembers(
    teamDetail?.id || "",
    queryMemberParams.search,
    queryMemberParams.role,
    queryMemberParams.page,
    queryMemberParams.pageSize,
    !!teamDetail?.id && activeTab === "members",
  );

  // ================= 4. BREADCRUMBS & NAVIGATION =================
  const breadcrumbItems = [
    { title: "Nhóm của tôi", href: "/teams" },
    { title: teamDetail?.name || "Chi tiết nhóm" },
  ];

  // ================= 5. ĐỊNH NGHĨA DANH SÁCH TABS =================
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
          role={userRole}
          teamId={teamId}
          data={projects?.items || []}
          isLoading={isProjectLoading}
          // Thay vì dùng useEffect tạo state riêng, ta check trực tiếp độ dài mảng dữ liệu
          hasHadData={
            (projects && projects?.total > 0) ||
            !!queryProjectParams.search ||
            queryProjectParams.page > 1
          }
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
          role={userRole}
          teamId={teamId}
          userId={me?.userId || ""}
          data={members?.items || []}
          isLoading={isMemberLoading}
          hasHadData={
            (members && members?.total > 0) ||
            !!queryMemberParams.search ||
            !!queryMemberParams.role ||
            queryMemberParams.page > 1
          }
          searchProps={searchMemberProps}
          roleProps={roleMemberProps}
          paginationProps={{
            ...paginationMemberProps,
            total: members?.total || 0,
          }}
        />
      ),
    },
  ];

  // Nếu là Admin/Leader thì đẩy thêm tab Cài đặt vào cấu trúc mảng
  if (isAdminOrLeader) {
    tabItems.push({
      key: "settings",
      label: (
        <span>
          <SettingOutlined /> Cài đặt
        </span>
      ),
      children: (
        <SettingList
          teamId={teamId}
          teamName={teamDetail?.name || ""}
          teamDescription={teamDetail?.description || ""}
          role={userRole}
        />
      ),
    });
  }

  return (
    <MainLayout breadcrumbItems={breadcrumbItems}>
      <Space vertical size={16} style={{ width: "100%" }}>
        {/* CARD TIÊU ĐỀ THÔNG TIN NHÓM */}
        <Card>
          {isTeamLoading || isMeLoading ? (
            <Skeleton active paragraph={{ rows: 1 }} />
          ) : (
            <Space vertical size={4}>
              <Title level={4} style={{ margin: 0 }}>
                {teamDetail?.name || "Chi tiết nhóm"}
              </Title>
              <Text type="secondary">{teamDetail?.description}</Text>
            </Space>
          )}
        </Card>

        {/* CARD NỘI DUNG TABS HOẠT ĐỘNG */}
        <Card>
          <Tabs
            activeKey={activeTab}
            onChange={(key) => setActiveTab(key)} // Cập nhật active tab để trigger lazy loading API
            defaultActiveKey="projects"
            items={tabItems}
            // Loại bỏ hoàn toàn hiệu ứng làm mờ (opacity) và khóa pointer khi loading
            // để người dùng vẫn có thể click chuyển tab thoải mái, tăng trải nghiệm mượt mà!
          />
        </Card>
      </Space>
    </MainLayout>
  );
}
