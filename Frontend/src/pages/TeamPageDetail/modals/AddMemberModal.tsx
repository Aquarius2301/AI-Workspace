import { useTranslation } from "react-i18next";
import { TEAM_AVAILABLE_MEMBERS_QUERY_KEY, useSearch, useTeam } from "@/hooks";
import { Modal, App, Button, Select, Tag } from "antd";
import type { AvailableTeamMemberItem, TeamRole } from "@/types";
import { useState, useMemo } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { SearchPagination, Table } from "@/components";

interface AddMemberModalProps {
  teamId: string;
  isOpen: boolean;
  onClose: () => void;
}

interface SelectedMember extends AvailableTeamMemberItem {
  role: TeamRole;
}

const DEFAULT_ROLE: TeamRole = "Member";

export function AddMemberModal({
  teamId,
  isOpen,
  onClose,
}: AddMemberModalProps) {
  const { t } = useTranslation();
  const queryClient = useQueryClient();
  const { getAvailableMembersByTeam, addMembers } = useTeam();

  const roleOptions = useMemo(
    () => [
      { value: "Admin" as const, label: t("roleSelect.admin") },
      { value: "Leader" as const, label: t("roleSelect.leader") },
      { value: "Member" as const, label: t("roleSelect.member") },
    ],
    [t],
  );

  const { paginationProps, searchProps, queryParams } = useSearch({});

  const [selectedMembers, setSelectedMembers] = useState<SelectedMember[]>([]);

  const { data: availableMembers = [], isLoading: isAvailableMembersLoading } =
    getAvailableMembersByTeam(
      teamId,
      queryParams.search,
      queryParams.page,
      queryParams.pageSize,
      isOpen,
    );

  const { message } = App.useApp();

  const selectedUserIds = useMemo(
    () => new Set(selectedMembers.map((m) => m.userId)),
    [selectedMembers],
  );

  const handleSelectMember = (member: AvailableTeamMemberItem) => {
    if (!selectedUserIds.has(member.userId)) {
      setSelectedMembers((prev) => [
        ...prev,
        { ...member, role: DEFAULT_ROLE },
      ]);
    }
  };

  const handleRemoveMember = (userId: string) => {
    setSelectedMembers((prev) => prev.filter((m) => m.userId !== userId));
  };

  const handleRoleChange = (userId: string, role: TeamRole) => {
    setSelectedMembers((prev) =>
      prev.map((m) => (m.userId === userId ? { ...m, role } : m)),
    );
  };

  const handleClose = () => {
    queryClient.removeQueries({
      queryKey: [...TEAM_AVAILABLE_MEMBERS_QUERY_KEY],
    });
    setSelectedMembers([]);
    onClose();
  };

  const handleAddMembers = async () => {
    try {
      await addMembers.mutateAsync({
        id: teamId,
        data: {
          members: selectedMembers.map((m) => ({
            userId: m.userId,
            role: m.role,
          })),
        },
      });
      message.success(t("team.addMemberSuccess"));
      handleClose();
    } catch (error) {
      console.error("Failed to add members:", error);
      message.error(t("team.addMemberError"));
    }
  };

  const availableColumns = [
    {
      title: t("team.memberName"),
      dataIndex: "userName",
      key: "userName",
    },
    {
      title: t("team.email"),
      dataIndex: "email",
      key: "email",
    },
    {
      title: "",
      key: "action",
      width: 60,
      render: (_: unknown, record: AvailableTeamMemberItem) => {
        const isSelected = selectedUserIds.has(record.userId);
        return isSelected ? (
          <Tag color="success" style={{ margin: 0 }}>
            ✓
          </Tag>
        ) : (
          <Button
            type="link"
            size="small"
            onClick={() => handleSelectMember(record)}
          >
            +
          </Button>
        );
      },
    },
  ];

  const selectedColumns = [
    {
      title: t("team.memberName"),
      dataIndex: "userName",
      key: "userName",
    },
    {
      title: t("team.email"),
      dataIndex: "email",
      key: "email",
    },
    {
      title: t("team.role"),
      key: "role",
      width: 130,
      render: (_: unknown, record: SelectedMember) => (
        <Select
          value={record.role}
          onChange={(value: TeamRole) => handleRoleChange(record.userId, value)}
          size="small"
          style={{ width: 110 }}
          options={roleOptions}
        />
      ),
    },
    {
      title: "",
      key: "action",
      width: 60,
      render: (_: unknown, record: SelectedMember) => (
        <Button
          type="link"
          danger
          size="small"
          onClick={() => handleRemoveMember(record.userId)}
        >
          {t("team.delete")}
        </Button>
      ),
    },
  ];

  return (
    <Modal
      title={t("team.addMemberTitle")}
      open={isOpen}
      onOk={handleAddMembers}
      onCancel={handleClose}
      width={768}
      centered
      closeIcon={null}
      confirmLoading={addMembers.isPending}
      mask={{ closable: false }}
      styles={{
        body: { paddingTop: "16px" },
      }}
      destroyOnHidden
      footer={[
        <Button
          key="back"
          onClick={handleClose}
          disabled={addMembers.isPending}
        >
          {t("team.addMemberCancel")}
        </Button>,
        <Button
          key="submit"
          type="primary"
          onClick={handleAddMembers}
          disabled={selectedMembers.length === 0 || addMembers.isPending}
          loading={addMembers.isPending}
        >
          {t("team.addMemberConfirm")}
        </Button>,
      ]}
    >
      <div style={{ marginBottom: 16 }}>
        <SearchPagination
          search={{
            ...searchProps,
            placeholder: t("team.addMemberSearch"),
          }}
          pagination={{ ...paginationProps, total: availableMembers.length }}
          tableProps={{
            dataSource: availableMembers,
            columns: availableColumns,
            loading: isAvailableMembersLoading,
            rowKey: "userId",
            size: "small",
            locale: { emptyText: t("team.noMembers") },
          }}
        ></SearchPagination>
      </div>

      {selectedMembers.length > 0 && (
        <div style={{ marginTop: 24 }}>
          <h4 style={{ marginBottom: 8 }}>
            {t("team.selectedMembers", { count: selectedMembers.length })}
          </h4>
          <Table
            dataSource={selectedMembers}
            columns={selectedColumns}
            pagination={false}
            rowKey="userId"
            size="small"
            locale={{ emptyText: t("team.noSelectedMembers") }}
          />
        </div>
      )}
    </Modal>
  );
}
