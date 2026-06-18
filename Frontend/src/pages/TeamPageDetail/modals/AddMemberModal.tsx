import { TEAM_AVAILABLE_MEMBERS_QUERY_KEY, useTeam } from "@/hooks";
import { Modal, App, Form, Transfer, Button } from "antd";
import type { AvailableTeamMemberItem } from "@/types";
import { useState } from "react";
import { useQueryClient } from "@tanstack/react-query";

interface AddMemberModalProps {
  teamId: string;
  isOpen: boolean;
  onClose: () => void;
}

export function AddMemberModal({
  teamId,
  isOpen,
  onClose,
}: AddMemberModalProps) {
  const queryClient = useQueryClient();
  const { getAvailableMembers, addMembers } = useTeam();

  const { data: availableMembers = [], isLoading: isAvailableMembersLoading } =
    getAvailableMembers();

  const [targetKeys, setTargetKeys] = useState<string[]>([]);

  const { message } = App.useApp();

  const handleClose = () => {
    setTargetKeys([]);
    queryClient.removeQueries({ queryKey: TEAM_AVAILABLE_MEMBERS_QUERY_KEY });
    onClose();
  };

  const handleAddMembers = async () => {
    try {
      await addMembers.mutateAsync({
        id: teamId,
        data: {
          members: targetKeys.map((userId) => ({
            userId,
            role: "Member",
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

  const renderMemberOption = (member: AvailableTeamMemberItem) => (
    <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
      <img
        src={member.email ? `/avatar/${member.email}` : "/default-avatar.png"}
        alt={member.userName}
        style={{ width: 32, height: 32, borderRadius: "50%" }}
      />
      <div>
        <div style={{ fontWeight: 500 }}>{member.userName}</div>
        <div style={{ fontSize: 12, color: "#888" }}>{member.email || ""}</div>
      </div>
    </div>
  );

  return (
    <Modal
      title="Thêm thành viên vào nhóm"
      open={isOpen}
      onOk={handleAddMembers}
      onCancel={handleClose}
      okText="Thêm thành viên"
      cancelText="Hủy"
      confirmLoading={addMembers.isPending || isAvailableMembersLoading}
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
          disabled={targetKeys.length === 0 || addMembers.isPending}
        >
          Thêm thành viên
        </Button>,
      ]}
    >
      <Form.Item>
        <Transfer
          dataSource={availableMembers.map((member) => ({
            key: member.userId,
            title: member.userName,
            description: member.email || "",
          }))}
          targetKeys={targetKeys}
          onChange={(targetKeys) => setTargetKeys(targetKeys as string[])}
          showSearch={true}
          filterOption={(inputValue, item) =>
            (item.title ?? "")
              .toLowerCase()
              .includes(inputValue.toLowerCase()) ||
            (item.description ?? "")
              .toLowerCase()
              .includes(inputValue.toLowerCase())
          }
          listStyle={{ height: 200 }}
          render={(member) =>
            renderMemberOption(
              availableMembers.find(
                (m) => m.userId === member.key,
              ) as AvailableTeamMemberItem,
            )
          }
        />
      </Form.Item>
    </Modal>
  );
}
