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
  const [modalState, setModalState] = useState<ModalState>(null);

  const [selectedMember, setSelectedMember] = useState<TeamMemberItem | null>(
    null,
  );

  const memberColumns = useMemo<CustomColumnsType<TeamMemberItem>>(
    () => [
      {
        title: "Tên người dùng",
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
                {userId === record.userId && <Text strong>(Bạn)</Text>}
              </Text>
              {isActive && (
                <Tooltip title="Đang hoạt động">
                  <Badge status="processing" />
                </Tooltip>
              )}
            </Space>
          );
        },
      },
      {
        title: "Email",
        dataIndex: "email",
        key: "email",
        render: (email: string) => email || <Text type="secondary">—</Text>,
      },

      {
        title: "Vai trò",
        dataIndex: "role",
        key: "role",
        render: (role: string) => role || <Text type="secondary">—</Text>,
      },
      {
        title: "Ngày tham gia",
        dataIndex: "joinedAt",
        key: "joinedAt",
        render: (joinedAt: string) =>
          new Date(joinedAt).toLocaleDateString("vi-VN"),
      },
      {
        title: "Hành động",
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
              Chỉnh sửa
            </Button>
          ),
      },
    ],
    [userId],
  );

  return (
    <Space vertical style={{ width: "100%" }} size={12}>
      {(role == "Admin" || role == "Leader") && (
        <Button type="primary" onClick={() => setModalState("addMember")}>
          Thêm thành viên
        </Button>
      )}
      <SearchPagination<TeamMemberItem>
        search={
          hasHadData
            ? { ...searchProps, placeholder: "Tên người dùng, email..." }
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
            emptyText: "Không có thành viên nào",
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
