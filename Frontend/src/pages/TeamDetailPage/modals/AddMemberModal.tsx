import {
  Flex,
  Input,
  Typography,
  App,
  Checkbox,
  Empty,
  Spin,
  Space,
} from "antd";
import { useTranslation } from "react-i18next";
import {
  AIModal,
  UserAvatar,
  AIPagination,
  AITeamRoleSelect,
} from "@/components";
import type { AvailableTeamMemberItem, TeamRole } from "@/types";
import { useSearch, useTeamAvailableMembers, useAddMembers } from "@/hooks";
import { useState } from "react";
import { getErrorMessage } from "@/utils";

const { Text } = Typography;

interface AddMemberModalProps {
  isOpen: boolean;
  onClose: () => void;
  teamId: string;
  currentUserRole: TeamRole;
}

export function AddMemberModal({
  isOpen,
  onClose,
  teamId,
  currentUserRole,
}: AddMemberModalProps) {
  const { t } = useTranslation();
  const { message } = App.useApp();

  const { searchProps, paginationProps, queryParams } = useSearch({});

  const [selectedMap, setSelectedMap] = useState<Map<string, TeamRole>>(
    new Map(),
  );

  const { data: availableMembers, isLoading } = useTeamAvailableMembers(
    teamId,
    queryParams.search || undefined,
    queryParams.page,
    queryParams.pageSize,
    isOpen,
  );

  const addMembers = useAddMembers();

  const members = availableMembers?.items ?? [];
  const total = availableMembers?.total ?? 0;

  // Admin: can add CoAdmin and Member → exceptRoles = ["Admin"]
  // CoAdmin: can only add Member → no role select needed
  const isAdmin = currentUserRole === "admin";
  const exceptRoles: TeamRole[] = isAdmin ? ["admin"] : ["admin", "coAdmin"];

  const selectedCount = selectedMap.size;

  // ── Handlers ──

  const handleToggleMember = (
    member: AvailableTeamMemberItem,
    checked: boolean,
  ) => {
    const next = new Map(selectedMap);
    if (checked) {
      next.set(member.id, "member");
    } else {
      next.delete(member.id);
    }
    setSelectedMap(next);
  };

  const handleRoleChange = (userId: string, role: TeamRole | undefined) => {
    if (!role) return;
    const next = new Map(selectedMap);
    next.set(userId, role);
    setSelectedMap(next);
  };

  const handleSubmit = async () => {
    if (selectedCount === 0) return;

    const membersPayload = Array.from(selectedMap.entries()).map(
      ([userId, role]) => ({ userId, role }),
    );

    try {
      await addMembers.mutateAsync({
        id: teamId,
        data: { members: membersPayload },
      });
      message.success(t("teamDetailPage.members.addMember.success"));
      handleClose();
    } catch (error) {
      message.error(getErrorMessage(error));
    }
  };

  const handleClose = () => {
    setSelectedMap(new Map());
    searchProps.onSearchChange("");
    onClose();
  };

  // ── Render ──

  return (
    <AIModal
      title={t("teamDetailPage.members.addMember.title")}
      open={isOpen}
      onOk={handleSubmit}
      onCancel={handleClose}
      isLoading={addMembers.isPending}
      width={700}
      footer={[
        { type: "cancel" },
        { type: "create", disabled: selectedCount === 0 },
      ]}
    >
      <Flex vertical gap={12} style={{ marginTop: 16 }}>
        {/* ── Search ── */}
        <Input
          placeholder={t("teamDetailPage.members.addMember.searchPlaceholder")}
          allowClear
          value={searchProps.search}
          onChange={(e) => searchProps.onSearchChange(e.target.value)}
        />

        {/* ── Selected count ── */}
        {selectedCount > 0 && (
          <Text type="secondary" style={{ fontSize: 13 }}>
            {t("teamDetailPage.members.addMember.selected", {
              count: selectedCount,
            })}
          </Text>
        )}

        {/* ── Available members list ── */}
        <Flex
          vertical
          gap={8}
          style={{
            maxHeight: 360,
            overflowY: "auto",
            marginTop: 4,
          }}
        >
          {isLoading ? (
            <Flex justify="center" style={{ padding: 24 }}>
              <Spin />
            </Flex>
          ) : members.length === 0 ? (
            <Empty
              image={Empty.PRESENTED_IMAGE_SIMPLE}
              description={t("teamDetailPage.members.addMember.noAvailable")}
            />
          ) : (
            members.map((member) => {
              const isChecked = selectedMap.has(member.id);
              return (
                <Flex
                  key={member.id}
                  wrap="wrap"
                  align="center"
                  gap={8}
                  style={{
                    padding: "8px 12px",
                    borderRadius: 8,
                    background: isChecked
                      ? "rgba(22, 119, 255, 0.06)"
                      : undefined,
                    transition: "background 0.2s",
                  }}
                >
                  <Checkbox
                    checked={isChecked}
                    onChange={(e) =>
                      handleToggleMember(member, e.target.checked)
                    }
                  />
                  <UserAvatar
                    userId={member.id}
                    userName={member.name}
                    src={member.avatarUrl}
                    size={44}
                  />
                  <Space vertical size={0} style={{ flex: 1, minWidth: 80 }}>
                    <Text ellipsis strong>
                      {member.name}
                    </Text>
                    <Text ellipsis type="secondary">
                      {member.email}
                    </Text>
                  </Space>
                  {/* Admin: show role select (CoAdmin/Member); CoAdmin: no select, always Member */}
                  {isChecked && isAdmin && (
                    <AITeamRoleSelect
                      value={selectedMap.get(member.id)}
                      exceptRoles={exceptRoles}
                      onChange={(role) =>
                        handleRoleChange(member.id, role as TeamRole)
                      }
                      style={{ width: 140 }}
                      allowClear={false}
                    />
                  )}
                </Flex>
              );
            })
          )}
        </Flex>

        {/* ── Pagination ── */}
        {total > 0 && (
          <AIPagination
            page={paginationProps.page}
            pageSize={paginationProps.pageSize}
            total={total}
            onPageChange={paginationProps.onPageChange}
            onPageSizeChange={paginationProps.onPageSizeChange}
            small
          />
        )}
      </Flex>
    </AIModal>
  );
}
