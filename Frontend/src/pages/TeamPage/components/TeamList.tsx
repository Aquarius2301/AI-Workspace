import type { PageResponse, TeamItem } from "@/types";
import { Button, Empty, Pagination, Space, Table } from "antd";
import { useNavigate } from "react-router-dom";
import { EyeOutlined } from "@ant-design/icons";

interface TeamListProps {
  teamsData: PageResponse<TeamItem>;
  handlePageChange: (page: number) => void;
  isLoading?: boolean;
}

export function TeamList({
  teamsData,
  handlePageChange,
  isLoading = false,
}: TeamListProps) {
  const navigate = useNavigate();
  return (
    <Space vertical size={24} style={{ width: "100%" }}>
      <Table
        dataSource={teamsData.items}
        rowKey={"id"}
        loading={isLoading}
        locale={{
          emptyText: <Empty description="Không tìm thấy nhóm nào" />,
        }}
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
            title: "View",

            render: (_, record) => (
              <Button onClick={() => navigate(`/teams/${record.id}`)}>
                <EyeOutlined />
              </Button>
            ),
          },
        ]}
        pagination={false}
      />

      {!isLoading && teamsData.total > 0 && (
        <Pagination
          total={teamsData.total}
          current={teamsData.page}
          pageSize={teamsData.pageSize}
          onChange={handlePageChange}
          showSizeChanger={false}
          showQuickJumper={true}
        />
      )}
    </Space>
  );
}
