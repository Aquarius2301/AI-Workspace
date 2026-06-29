import { Flex, Grid } from "antd";
import { useTranslation } from "react-i18next";
import { AUTH_ME_QUERY_KEY, useGetCacheData, useSummary } from "@/hooks";
import { AppLayout } from "@/layouts";
import type { AuthResponse } from "@/types";
import {
  GreetingSection,
  StatCardsRow,
  TaskBreakdownCard,
  RecentActivitySection,
  TeamSummariesCard,
} from "./components";

const { useBreakpoint } = Grid;

export default function OverviewPage() {
  const { t } = useTranslation();
  const screens = useBreakpoint();
  const isMobile = !screens.md;

  const breadcrumbItems = [{ title: t("overview.title") }];

  // Data
  const user = useGetCacheData<AuthResponse>(AUTH_ME_QUERY_KEY);
  const { data: summaryData, isLoading } = useSummary();

  const totalTeams = summaryData?.totalTeams ?? 0;
  const totalProjects = summaryData?.totalProjects ?? 0;
  const taskSummary = summaryData?.myTasks ?? {
    total: 0,
    open: 0,
    inProgress: 0,
    done: 0,
    blocked: 0,
    overdue: 0,
  };
  const recentTasks = summaryData?.recentTasks ?? [];
  const recentComments = summaryData?.recentComments ?? [];
  const teamSummaries = summaryData?.teamSummaries ?? [];

  return (
    <AppLayout breadcrumbItems={breadcrumbItems}>
      <Flex vertical gap={isMobile ? 20 : 28}>
        <GreetingSection
          userName={user?.name}
          taskTotal={taskSummary.total}
          totalTeams={totalTeams}
        />

        <StatCardsRow
          isLoading={isLoading}
          totalTeams={totalTeams}
          totalProjects={totalProjects}
          taskTotal={taskSummary.total}
          overdue={taskSummary.overdue}
        />

        <TaskBreakdownCard
          isLoading={isLoading}
          taskSummary={taskSummary}
        />

        <RecentActivitySection
          isLoading={isLoading}
          recentTasks={recentTasks}
          recentComments={recentComments}
        />

        <TeamSummariesCard
          isLoading={isLoading}
          teamSummaries={teamSummaries}
        />
      </Flex>
    </AppLayout>
  );
}
