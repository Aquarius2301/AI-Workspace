import {
  TEAM_AVAILABLE_MEMBERS_QUERY_KEY,
  useDebounce,
  useTeam,
} from "@/hooks";
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

const ROLE_OPTIONS: { value: TeamRole; label: string }[] = [
  { value: "Admin", label: "Admin" },
  { value: "Leader", label: "Leader" },
  { value: "Member", label: "Member" },
];

const DEFAULT_ROLE: TeamRole = "Member";

export function AddMemberModal({
  teamId,
  isOpen,
  onClose,
}: AddMemberModalProps) {
  const queryClient = useQueryClient();
  const { getAvailableMembersByTeam, addMembers } = useTeam();

  const [searchText, setSearchText] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(5);
  const [selectedMembers, setSelectedMembers] = useState<SelectedMember[]>([]);

  const debounceText = useDebounce(searchText, 500);

  const { data: availableMembers = [], isLoading: isAvailableMembersLoading } =
    getAvailableMembersByTeam(teamId, debounceText);

  const { message } = App.useApp();

  // Client-side pagination over the API result
  const paginatedMembers = useMemo(() => {
    const start = (page - 1) * pageSize;
    return availableMembers.slice(start, start + pageSize);
  }, [availableMembers, page, pageSize]);

  // Set of selected userIds for quick lookup
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
    setSearchText("");
    setPage(1);
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
      message.success("Thành viên đã được thêm vào nhóm thành công!");
      handleClose();
    } catch (error) {
      console.error("Failed to add members:", error);
      message.error("Đã xảy ra lỗi khi thêm thành viên. Vui lòng thử lại.");
    }
  };

  const handleSearchChange = (value: string) => {
    setSearchText(value);
    setPage(1);
  };

  // Columns for Table 1 – Available members
  const availableColumns = [
    {
      title: "Thành viên",
      dataIndex: "userName",
      key: "userName",
    },
    {
      title: "Email",
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

  // Columns for Table 2 – Selected members with role assignment
  const selectedColumns = [
    {
      title: "Thành viên",
      dataIndex: "userName",
      key: "userName",
    },
    {
      title: "Email",
      dataIndex: "email",
      key: "email",
    },
    {
      title: "Vai trò",
      key: "role",
      width: 130,
      render: (_: unknown, record: SelectedMember) => (
        <Select
          value={record.role}
          onChange={(value: TeamRole) => handleRoleChange(record.userId, value)}
          size="small"
          style={{ width: 110 }}
          options={ROLE_OPTIONS}
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
          Xóa
        </Button>
      ),
    },
  ];

  return (
    <Modal
      title="Thêm thành viên vào nhóm"
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
      footer={[
        <Button
          key="back"
          onClick={handleClose}
          disabled={addMembers.isPending}
        >
          Hủy
        </Button>,
        <Button
          key="submit"
          type="primary"
          onClick={handleAddMembers}
          disabled={selectedMembers.length === 0 || addMembers.isPending}
          loading={addMembers.isPending}
        >
          Thêm thành viên
        </Button>,
      ]}
    >
      {/* ─── Table 1: Available Members ─── */}
      <div style={{ marginBottom: 16 }}>
        <SearchPagination
          search={{
            placeholder: "Tên hoặc email thành viên...",
            search: searchText,
            onSearchChange: handleSearchChange,
          }}
          pagination={{
            page,
            pageSize,
            total: availableMembers.length,
            onPageChange: (p: number) => setPage(p),
            onPageSizeChange: (size: number) => {
              setPageSize(size);
              setPage(1);
            },
          }}
          tableProps={{
            dataSource: paginatedMembers,
            columns: availableColumns,
            loading: isAvailableMembersLoading,
            rowKey: "userId",
            size: "small",
            locale: { emptyText: "Không có thành viên nào" },
          }}
        ></SearchPagination>
      </div>

      {/* ─── Table 2: Selected Members ─── */}
      {selectedMembers.length > 0 && (
        <div style={{ marginTop: 24 }}>
          <h4 style={{ marginBottom: 8 }}>
            Thành viên được chọn ({selectedMembers.length})
          </h4>
          <Table
            dataSource={selectedMembers}
            columns={selectedColumns}
            pagination={false}
            rowKey="userId"
            size="small"
            locale={{ emptyText: "Chưa chọn thành viên nào" }}
          />
        </div>
      )}
    </Modal>
  );
}
