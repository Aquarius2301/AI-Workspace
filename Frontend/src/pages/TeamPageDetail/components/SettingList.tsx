import {
  Button,
  Card,
  Space,
  Row,
  Col,
  Typography,
  Divider,
  Tag,
  Tooltip,
} from "antd";
import { useState } from "react";
import {
  FolderOpenOutlined,
  UserAddOutlined,
  TeamOutlined,
  EditOutlined,
  DeleteOutlined,
  PlusOutlined,
  SafetyCertificateOutlined,
} from "@ant-design/icons";
import { SettingOutlined } from "@ant-design/icons";
import type { TeamRole } from "@/types";
import {
  AddMemberModal,
  AssignMemberModal,
  CreateProjectModal,
  DeleteTeamModal,
  EditTeamModal,
} from "../modals";

const { Title, Text, Paragraph } = Typography;

interface SettingListProps {
  teamId: string;
  teamName: string;
  teamDescription: string;
  role: TeamRole;
}

type ModalItems =
  | "none"
  | "delete"
  | "edit"
  | "createProject"
  | "addMember"
  | "assignMember";

const cardStyle: React.CSSProperties = {
  borderRadius: 12,
  boxShadow: "0 2px 8px rgba(0,0,0,0.08)",
  border: "1px solid #f0f0f0",
  height: "100%",
};

const cardBodyStyle: React.CSSProperties = {
  padding: 24,
};

const iconWrapperStyle: React.CSSProperties = {
  width: 48,
  height: 48,
  borderRadius: 12,
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
  fontSize: 24,
  flexShrink: 0,
};

export function SettingList({
  teamId,
  teamName,
  teamDescription,
  role,
}: SettingListProps) {
  const [isModalOpen, setIsModalOpen] = useState<ModalItems>("none");

  return (
    <div style={{ padding: "8px 0" }}>
      <div style={{ marginBottom: 32 }}>
        <Title level={3} style={{ margin: 0 }}>
          <SettingOutlined style={{ marginRight: 12, color: "#1677ff" }} />
          Cài đặt nhóm
        </Title>
        <Text
          type="secondary"
          style={{ fontSize: 15, marginTop: 8, display: "block" }}
        >
          Admin và leader quản lý dự án, thành viên và thông tin nhóm {teamName}
        </Text>
      </div>

      <Row gutter={[24, 24]}>
        {/* Dự án */}
        <Col lg={24} xl={12}>
          <Card style={cardStyle} styles={{ body: cardBodyStyle }} hoverable>
            <Space vertical size={20} style={{ width: "100%" }}>
              <Space align="start" size={16} style={{ width: "100%" }}>
                <div
                  style={{
                    ...iconWrapperStyle,
                    background:
                      "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
                    color: "#fff",
                  }}
                >
                  <FolderOpenOutlined />
                </div>
                <div style={{ flex: 1 }}>
                  <Title level={5} style={{ margin: 0, marginBottom: 4 }}>
                    Dự án
                  </Title>
                  <Paragraph
                    type="secondary"
                    style={{ margin: 0, fontSize: 13, lineHeight: 1.6 }}
                  >
                    Tạo dự án cho nhóm{" "}
                    <Tag color="geekblue" style={{ margin: 0 }}>
                      {teamName}
                    </Tag>{" "}
                    và mời các thành viên trong team vào nhóm để cùng hợp tác
                    làm việc hiệu quả.
                  </Paragraph>
                </div>
              </Space>

              <Divider style={{ margin: 0 }} />

              <div style={{ display: "flex", justifyContent: "flex-end" }}>
                <Tooltip title="Tạo dự án mới cho nhóm">
                  <Button
                    type="primary"
                    onClick={() => setIsModalOpen("createProject")}
                    icon={<PlusOutlined />}
                    size="large"
                  >
                    Thêm dự án
                  </Button>
                </Tooltip>
              </div>
            </Space>
          </Card>
        </Col>

        {/* Thành viên */}
        <Col lg={24} xl={12}>
          <Card style={cardStyle} styles={{ body: cardBodyStyle }} hoverable>
            <Space vertical size={20} style={{ width: "100%" }}>
              <Space align="start" size={16} style={{ width: "100%" }}>
                <div
                  style={{
                    ...iconWrapperStyle,
                    background:
                      "linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)",
                    color: "#fff",
                  }}
                >
                  <TeamOutlined />
                </div>
                <div style={{ flex: 1 }}>
                  <Title level={5} style={{ margin: 0, marginBottom: 4 }}>
                    Thành viên
                  </Title>
                  <Paragraph
                    type="secondary"
                    style={{ margin: 0, fontSize: 13, lineHeight: 1.6 }}
                  >
                    Thêm thành viên vào nhóm{" "}
                    <Tag color="geekblue" style={{ margin: 0 }}>
                      {teamName}
                    </Tag>{" "}
                    và gán quyền Admin, Leader, Member để phân quyền quản lý phù
                    hợp.
                  </Paragraph>
                </div>
              </Space>

              <Divider style={{ margin: 0 }} />

              <Row gutter={[12, 12]} style={{ width: "100%" }}>
                <Col span={12}>
                  <Tooltip title="Thêm thành viên mới vào nhóm">
                    <Button
                      type="primary"
                      icon={<UserAddOutlined />}
                      block
                      size="large"
                      onClick={() => setIsModalOpen("addMember")}
                    >
                      Thêm thành viên
                    </Button>
                  </Tooltip>
                </Col>
                <Col span={12}>
                  <Tooltip title="Phân quyền cho các thành viên trong nhóm">
                    <Button
                      type="default"
                      icon={<SafetyCertificateOutlined />}
                      block
                      size="large"
                      onClick={() => setIsModalOpen("assignMember")}
                    >
                      Gán quyền
                    </Button>
                  </Tooltip>
                </Col>
              </Row>
            </Space>
          </Card>
        </Col>

        {/* Chỉnh sửa & Xóa nhóm */}
        {role == "Admin" && (
          <Col lg={24} xl={12}>
            <Card
              style={{
                ...cardStyle,
                border: "1px solid #fff1f0",
              }}
              styles={{ body: cardBodyStyle }}
              hoverable
            >
              <Space vertical size={20} style={{ width: "100%" }}>
                <Space align="start" size={16} style={{ width: "100%" }}>
                  <div
                    style={{
                      ...iconWrapperStyle,
                      background:
                        "linear-gradient(135deg, #f093fb 0%, #f5576c 100%)",
                      color: "#fff",
                    }}
                  >
                    <EditOutlined />
                  </div>
                  <div style={{ flex: 1 }}>
                    <Title level={5} style={{ margin: 0, marginBottom: 4 }}>
                      Chỉnh sửa nhóm
                    </Title>
                    <Paragraph
                      type="secondary"
                      style={{ margin: 0, fontSize: 13, lineHeight: 1.6 }}
                    >
                      Cập nhật tên, mô tả nhóm hoặc xóa toàn bộ dữ liệu nhóm (dự
                      án, task,...).
                    </Paragraph>
                  </div>
                </Space>

                <Divider style={{ margin: 0 }} />

                <Row gutter={[12, 12]} style={{ width: "100%" }}>
                  <Col xs={24} sm={12}>
                    <Tooltip title="Chỉnh sửa tên và mô tả nhóm">
                      <Button
                        type="primary"
                        icon={<EditOutlined />}
                        block
                        size="large"
                        onClick={() => setIsModalOpen("edit")}
                      >
                        Chỉnh sửa nhóm
                      </Button>
                    </Tooltip>
                  </Col>
                  <Col xs={24} sm={12}>
                    <Tooltip title="Xóa vĩnh viễn nhóm và tất cả dữ liệu liên quan">
                      <Button
                        danger
                        icon={<DeleteOutlined />}
                        block
                        size="large"
                        onClick={() => setIsModalOpen("delete")}
                      >
                        Xóa nhóm
                      </Button>
                    </Tooltip>
                  </Col>
                </Row>
              </Space>
            </Card>
          </Col>
        )}
      </Row>

      <DeleteTeamModal
        teamId={teamId}
        teamName={teamName}
        isOpen={isModalOpen == "delete"}
        onClose={() => setIsModalOpen("none")}
      />

      <EditTeamModal
        teamId={teamId}
        teamName={teamName}
        teamDescription={teamDescription}
        isOpen={isModalOpen == "edit"}
        onClose={() => setIsModalOpen("none")}
      />

      <CreateProjectModal
        teamId={teamId}
        isOpen={isModalOpen == "createProject"}
        onClose={() => setIsModalOpen("none")}
      />

      <AddMemberModal
        teamId={teamId}
        isOpen={isModalOpen == "addMember"}
        onClose={() => setIsModalOpen("none")}
      />

      <AssignMemberModal
        isOpen={isModalOpen == "assignMember"}
        onClose={() => setIsModalOpen("none")}
        teamId={teamId}
      />
    </div>
  );
}
