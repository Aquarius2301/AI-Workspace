import { useState } from "react";
import { Flex, Tabs } from "antd";
import { useParams, Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useTeamDetail, useTeamMe } from "@/hooks";
import { AppLayout } from "@/layouts";
import { TeamInfoCard } from "./components/TeamInfoCard";
import { ProjectTab } from "./tabs/ProjectTab";
import { MembersTab } from "./tabs/MembersTab";

export default function TeamDetailPage() {
  const { t } = useTranslation();
  const { id } = useParams<{ id: string }>();

  // ── Team detail for breadcrumb ──
  const { data: team } = useTeamDetail(id!);
  const { data: myMembership } = useTeamMe(id!);

  const breadcrumbItems = [
    { title: <Link to="/teams">{t("team.title")}</Link> },
    { title: team?.name || "..." },
  ];

  // ── Tab state ──
  const [activeTab, setActiveTab] = useState("projects");

  return (
    <AppLayout breadcrumbItems={breadcrumbItems}>
      <Flex vertical gap={16}>
        {/* ── Team info card ── */}
        {team && (
          <TeamInfoCard
            teamId={id!}
            role={myMembership?.role || "Member"}
            name={team.name}
            description={team.description}
            currentUserRole={myMembership?.role}
          />
        )}

        {/* ── Tabs ── */}
        <Tabs
          activeKey={activeTab}
          onChange={setActiveTab}
          items={[
            {
              key: "projects",
              label: t("teamDetail.projects.default"),
              children: (
                <ProjectTab
                  teamId={id!}
                  role={myMembership?.role || "Member"}
                />
              ),
            },
            {
              key: "members",
              label: t("teamDetail.members.default"),
              children: (
                <MembersTab
                  userId={myMembership?.userId || ""}
                  teamId={id!}
                  role={myMembership?.role || "Member"}
                />
              ),
            },
          ]}
        />
      </Flex>
    </AppLayout>
  );
}
