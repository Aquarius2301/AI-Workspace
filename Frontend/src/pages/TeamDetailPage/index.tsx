import { useState } from "react";
import { Flex, Tabs } from "antd";
import { useParams, Link, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { AppLayout } from "@/layouts";
import {
  AUTH_ME_QUERY_KEY,
  useGetCacheData,
  useTeamDetailBySlug,
} from "@/hooks";
import { ROUTE } from "@/constants";
import { TeamInfoCard } from "./components/TeamInfoCard";

import type { AuthResponse } from "@/types";
import { ProjectTab } from "./tabs/ProjectTab";
import { NotFound } from "@/components";
import { MembersTab } from "./tabs/MembersTab";

export default function TeamDetailPage() {
  const { t } = useTranslation();
  const { slug } = useParams<{ slug: string }>();
  const navigate = useNavigate();

  // ── Team detail for breadcrumb ──
  const { data: team, isLoading } = useTeamDetailBySlug(slug!);

  const me = useGetCacheData<AuthResponse>(AUTH_ME_QUERY_KEY);

  const breadcrumbItems = [
    { title: <Link to={ROUTE.TEAM}>{t("teamPage.title")}</Link> },
    { title: team?.name || "..." },
  ];

  // ── Tab state ──
  const [activeTab, setActiveTab] = useState("projects");

  // ── Not found state ──
  if (!isLoading && !team) {
    return (
      <AppLayout breadcrumbItems={breadcrumbItems}>
        <Flex justify="center" style={{ minHeight: 400 }}>
          <NotFound
            title={t("teamDetailPage.notFound.title")}
            description={t("teamDetailPage.notFound.description")}
            buttonText={t("teamDetailPage.notFound.button")}
            onBack={() => navigate(ROUTE.TEAM)}
          />
        </Flex>
      </AppLayout>
    );
  }

  return (
    <AppLayout
      breadcrumbItems={breadcrumbItems}
      title={`${t("teamPage.title")} ${team?.name ? " - " + team.name : ""}`}
    >
      <Flex vertical gap={16}>
        {/* ── Team info card ── */}
        <TeamInfoCard
          isLoading={isLoading}
          teamId={team?.id!}
          name={team?.name!}
          description={team?.description!}
          role={team?.role!}
        />
        {/* ── Tabs ── */}
        {team && me && (
          <Tabs
            activeKey={activeTab}
            onChange={setActiveTab}
            items={[
              {
                key: "projects",
                label: t("teamDetailPage.projects.title"),
                children: <ProjectTab teamId={team.id} role={team.role} />,
              },
              {
                key: "members",
                label: t("teamDetailPage.members.title"),
                children: (
                  <MembersTab
                    userId={me.id}
                    teamId={team.id}
                    role={team.role}
                  />
                ),
              },
            ]}
          />
        )}
      </Flex>
    </AppLayout>
  );
}
