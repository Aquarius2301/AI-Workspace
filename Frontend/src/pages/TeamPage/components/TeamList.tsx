import type { PageResponse, TeamItem } from "@/types";
import { Button, Pagination, Space, Table } from "antd";
import { useNavigate } from "react-router-dom";

interface TeamListProps {
  teamsData: PageResponse<TeamItem>;
  handlePageChange: (page: number) => void;
}

export function TeamList({ teamsData, handlePageChange }: TeamListProps) {
  const navigate = useNavigate();
  return (
    <Space vertical size={16} style={{ width: "100%" }}>
      {teamsData.items.length ? (
        <>
          <Table
            dataSource={teamsData.items}
            rowKey={"id"}
            columns={[
              {
                title: "Name",
                dataIndex: "name",
                key: "name",
              },
              {
                title: "Description",
                dataIndex: "description",
                key: "description",
              },
              {
                title: "Actions",

                render: (_, record) => (
                  <Button onClick={() => navigate(`/team/${record.id}`)}>
                    View
                  </Button>
                ),
              },
            ]}
            pagination={false}
          />
          <Pagination
            total={teamsData.total}
            current={teamsData.page}
            pageSize={teamsData.pageSize}
            onChange={handlePageChange}
            showSizeChanger={false}
            showQuickJumper={false}
          />
        </>
      ) : (
        <p>No teams found</p>
      )}
    </Space>
  );
}
