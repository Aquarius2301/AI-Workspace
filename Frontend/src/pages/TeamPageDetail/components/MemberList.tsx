import { useTranslation } from "react-i18next";
import {
  SearchPagination,
  type CustomColumnsType,
  type PaginationProps,
  type RoleProps,
  type SearchProps,
} from "@/components";
import type { TeamMemberItem, TeamRole } from "@/types";
import { getDistanceToNow } from "@/utils/date.util";
import { Badge, Button, Space, Tooltip } from "antd";
import Text from "antd/es/typography/Text";
import { useMemo, useState } from "react";
import { AddMemberModal, EditMemberModal } from "../modals";
import i18n from "@/i18n";

interface MemberListProps {
  hasHadData: boolean;
  isLoading: boolean;
  data: TeamMemberItem[];
  searchProps: SearchProps;
  roleProps: RoleProps;
  paginationProps: PaginationProps;
  role: TeamRole;
  teamId: string;
  userId: string;
}

type ModalState = "addMember" | "editMember" | null;

export function MemberList({
  hasHadData,
  isLoading,
  data,
  searchProps,
  roleProps,
  paginationProps,
  role,
  teamId,
  userId,
}: MemberListProps) {
  const { t } = useTranslation();
  const [modalState, setModalState] = useState<ModalState>(null);

  const [selectedMember, setSelectedMember] = useState<TeamMemberItem | null>(
    null,
  );

  const memberColumns = useMemo<CustomColumnsType<TeamMemberItem>>(
    () => [
      {
        title: t("team.memberName"),
        dataIndex: "userName",
        key: "userName",
        render: (userName: string, record: TeamMemberItem) => {
          const timeDistance = getDistanceToNow(
            record.lastActiveAt,
            "minute",
            false,
          );
          const isActive =
            timeDistance !== null &&
            timeDistance <= 10 &&
            userId != record.userId;

          return (
            <Space>
              <Text strong>
                {userName}{" "}
                {userId === record.userId && (
                  <Text strong>{t("team.you")}</Text>
                )}
              </Text>
              {isActive && (
                <Tooltip title={t("team.active")}>
                  <Badge status="processing" />
                </Tooltip>
              )}
            </Space>
          );
        },
      },
      {
        title: t("team.email"),
        dataIndex: "email",
        key: "email",
        render: (email: string) => email || <Text type="secondary">—</Text>,
      },

      {
        title: t("team.role"),
        dataIndex: "role",
        key: "role",
        render: (role: string) => role || <Text type="secondary">—</Text>,
      },
      {
        title: t("team.joinDate"),
        dataIndex: "joinedAt",
        key: "joinedAt",
        render: (joinedAt: string) =>
          new Date(joinedAt).toLocaleDateString(
            i18n.language === "vi" ? "vi-VN" : "en-US",
          ),
      },
      {
        title: t("team.actions"),
        noShowMobileTitle: true,
        render: (_: any, record: TeamMemberItem) =>
          (role == "Admin" || role == "Leader") &&
          record.userId !== userId && (
            <Button
              onClick={() => {
                setSelectedMember(record);
                setModalState("editMember");
              }}
            >
              {t("team.editMember")}
            </Button>
          ),
      },
    ],
    [userId, t],
  );

  return (
    <Space vertical style={{ width: "100%" }} size={12}>
      {(role == "Admin" || role == "Leader") && (
        <Button type="primary" onClick={() => setModalState("addMember")}>
          {t("team.addMemberButton")}
        </Button>
      )}
      <SearchPagination<TeamMemberItem>
        search={
          hasHadData
            ? { ...searchProps, placeholder: t("team.searchMembers") }
            : undefined
        }
        role={hasHadData ? roleProps : undefined}
        pagination={{ ...paginationProps }}
        tableProps={{
          dataSource: data,
          columns: memberColumns,
          rowKey: "userId",
          loading: isLoading,
          locale: {
            emptyText: t("team.noMembers"),
          },
        }}
      />

      <AddMemberModal
        isOpen={modalState === "addMember"}
        onClose={() => setModalState(null)}
        teamId={teamId}
      />

      {selectedMember && (
        <EditMemberModal
          isOpen={modalState === "editMember"}
          onClose={() => {
            setModalState(null);
            setSelectedMember(null);
          }}
          memberId={selectedMember.userId}
          teamId={teamId}
          userName={selectedMember.userName}
          email={selectedMember.email}
          currentRole={selectedMember.role}
        />
      )}
    </Space>
  );
}
