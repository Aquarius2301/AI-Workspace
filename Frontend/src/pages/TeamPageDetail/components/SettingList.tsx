import { Button } from "antd";
import { useState } from "react";
import { DeleteTeamModal } from "./DeleteTeamModal";

interface SettingListProps {
  teamId: string;
  teamName: string;
}

export function SettingList({ teamId, teamName }: SettingListProps) {
  const [isModalOpen, setIsModalOpen] = useState(false);
  return (
    <div>
      <Button type="primary" danger onClick={() => setIsModalOpen(true)}>
        Xóa nhóm
      </Button>

      <DeleteTeamModal
        teamId={teamId}
        teamName={teamName}
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
      />
    </div>
  );
}
