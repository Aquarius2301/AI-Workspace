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
  AIProjectRoleSelect,
} from "@/components";
import type { AvailableProjectMemberItem, ProjectRole } from "@/types";
import {
  useSearch,
  useProjectAvailableMembers,
  useAddProjectMembers,
} from "@/hooks";
import { useState } from "react";
import { getErrorMessage } from "@/utils";

const { Text } = Typography;

interface AddProjectMemberModalProps {
  isOpen: boolean;
  onClose: () => void;
  projectId: string;
  creatorId: string;
}

export function AddProjectMemberModal({
  isOpen,
  onClose,
  projectId,
  creatorId,
}: AddProjectMemberModalProps) {
  const { t } = useTranslation();
  const { message } = App.useApp();

  const { searchProps, paginationProps, queryParams } = useSearch({});

  const [selectedMap, setSelectedMap] = useState<Map<string, ProjectRole>>(
    new Map(),
  );

  const { data: availableMembers, isLoading } = useProjectAvailableMembers(
    projectId,
    queryParams.search || undefined,
    queryParams.page,
    queryParams.pageSize,
    isOpen,
  );

  const addMembers = useAddProjectMembers();

  const members = availableMembers?.items ?? [];
  const total = availableMembers?.total ?? 0;

  const selectedCount = selectedMap.size;

  // ── Handlers ──

  const handleToggleMember = (
    member: AvailableProjectMemberItem,
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

  const handleRoleChange = (userId: string, role: ProjectRole | undefined) => {
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
        projectId,
        data: { members: membersPayload },
      });
      message.success(t("projectDetailPage.members.addMember.success"));
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
      title={t("projectDetailPage.members.addMember.title")}
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
          placeholder={t(
            "projectDetailPage.members.addMember.searchPlaceholder",
          )}
          allowClear
          value={searchProps.search}
          onChange={(e) => searchProps.onSearchChange(e.target.value)}
        />

        {/* ── Selected count ── */}
        {selectedCount > 0 && (
          <Text type="secondary" style={{ fontSize: 13 }}>
            {t("projectDetailPage.members.addMember.selected", {
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
              description={t("projectDetailPage.members.addMember.noAvailable")}
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
                    src={member.avatarUrl ?? undefined}
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
                  {/* Role select: Admin/CoAdmin can assign roles */}
                  {isChecked && member.id === creatorId && (
                    <Text
                      type="warning"
                      style={{ fontSize: 13, fontStyle: "italic" }}
                    >
                      {t("projectDetailPage.members.addMember.creatorLabel")}
                    </Text>
                  )}
                  {isChecked && member.id !== creatorId && (
                    <AIProjectRoleSelect
                      value={selectedMap.get(member.id)}
                      onChange={(role) =>
                        handleRoleChange(member.id, role as ProjectRole)
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
