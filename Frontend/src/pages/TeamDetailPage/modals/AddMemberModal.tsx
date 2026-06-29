import { Flex, Input, Typography, App, Checkbox, Empty, Spin } from "antd";
import { useTranslation } from "react-i18next";
import { AIModal, UserAvatar, AIRoleSelect, AIPagination } from "@/components";
import { useTeam, useTeamAvailableMembers, useSearch } from "@/hooks";
import type { TeamRole, AvailableTeamMemberItem } from "@/types";
import { getTranslatedErrorMessage } from "@/utils";
import { useState } from "react";

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
  const { addMembers } = useTeam();

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

  const members = availableMembers?.items ?? [];
  const total = availableMembers?.total ?? 0;

  const exceptRoles: TeamRole[] =
    currentUserRole === "Admin" ? ["Admin"] : ["Admin", "CoAdmin"];

  const selectedCount = selectedMap.size;

  // ── Handlers ──

  const handleToggleMember = (
    member: AvailableTeamMemberItem,
    checked: boolean,
  ) => {
    const next = new Map(selectedMap);
    if (checked) {
      next.set(member.userId, "Member");
    } else {
      next.delete(member.userId);
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
      message.success(t("teamDetail.members.addMemberModal.success"));
      handleClose();
    } catch (error) {
      message.error(getTranslatedErrorMessage(error));
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
      title={t("teamDetail.members.addMemberModal.title")}
      open={isOpen}
      onOk={handleSubmit}
      onCancel={handleClose}
      isLoading={addMembers.isPending}
      footer={[
        { type: "cancel" },
        { type: "create", disabled: selectedCount === 0 },
      ]}
    >
      <Flex vertical gap={12} style={{ marginTop: 16 }}>
        {/* ── Search ── */}
        <Input.Search
          placeholder={t("teamDetail.members.addMemberModal.searchPlaceholder")}
          allowClear
          value={searchProps.search}
          onChange={(e) => searchProps.onSearchChange(e.target.value)}
          onSearch={(val) => searchProps.onSearchChange(val)}
        />

        {/* ── Selected count ── */}
        {selectedCount > 0 && (
          <Text type="secondary" style={{ fontSize: 13 }}>
            {t("teamDetail.members.addMemberModal.selected", {
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
              description={t("teamDetail.members.addMemberModal.noAvailable")}
            />
          ) : (
            members.map((member) => {
              const isChecked = selectedMap.has(member.userId);
              return (
                <Flex
                  key={member.userId}
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
                    userId={member.userId}
                    userName={member.userName}
                    size={36}
                  />
                  <Text ellipsis style={{ flex: 1, fontSize: 14, minWidth: 0 }}>
                    {member.userName}
                  </Text>

                  {isChecked && (
                    <AIRoleSelect
                      value={selectedMap.get(member.userId)}
                      exceptRoles={exceptRoles}
                      onChange={(role) =>
                        handleRoleChange(member.userId, role as TeamRole)
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
