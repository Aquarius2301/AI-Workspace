import {
  MeQueryKey,
  useGetCacheData,
  useProject,
  useTask,
  useTeam,
} from "@/hooks";
import MainLayout from "@/layouts";
import type {
  MyTaskItemResponse,
  TeamProjectItem,
  UserResponse,
} from "@/types";
import { Col, Row, Space } from "antd";
import {
  TaskStatusOverview,
  ProjectList,
  UserGreeting,
  UpcomingTasks,
} from "./components";

export default function HomePage() {
  // Get user info from cache (already fetched by ProtectedRoute)
  const user = useGetCacheData<UserResponse>(MeQueryKey);

  // Fetch teams to get the first team's ID
  const teamsQuery = useTeam().getList({
    myTeams: true,
    page: 1,
    pageSize: 10,
  });
  // Runtime: axios interceptor unwraps response.data, but TS sees AxiosResponse<T>
  const teamsList = (teamsQuery.data as any)?.items as
    | { id: string; name: string }[]
    | undefined;
  const teamId = teamsList?.[0]?.id ?? "";

  // Fetch tasks and projects for the current team
  const myTasksQuery = useTask().getMyTasks(teamId);
  const projectsQuery = useProject().getByTeam(teamId, "", 1, 10);

  // Runtime: axios interceptor unwraps response.data, so .data contains the actual result
  const tasks = (myTasksQuery.data as MyTaskItemResponse[]) ?? [];
  const projects = (projectsQuery.data?.items as TeamProjectItem[]) ?? [];

  return (
    <MainLayout breadcrumbItems={[{ title: "Tổng quan" }]}>
      <Space
        vertical
        size={24}
        style={{
          width: "100%",
          maxWidth: "100%",
        }}
      >
        {/* Row 1: User Greeting */}
        <UserGreeting
          name={user?.name ?? ""}
          email={user?.email ?? ""}
          avatar={user?.avatar}
        />

        {/* Row 2: Task Status and Project List */}
        <Row gutter={[24, 24]}>
          <Col xs={24} lg={12}>
            <TaskStatusOverview
              tasks={tasks}
              isLoading={myTasksQuery.isLoading}
            />
          </Col>
          <Col xs={24} lg={12}>
            <ProjectList
              projects={projects}
              isLoading={projectsQuery.isLoading}
            />
          </Col>
        </Row>

        {/* Row 3: Upcoming Tasks */}
        <UpcomingTasks tasks={tasks} isLoading={myTasksQuery.isLoading} />
      </Space>
    </MainLayout>
  );
}
