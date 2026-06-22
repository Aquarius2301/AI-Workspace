import {
  SearchPagination,
  type PaginationProps,
  type RoleProps,
  type SearchProps,
} from "@/components";
import type { TeamMemberItem } from "@/types";
import { getDistanceToNow } from "@/utils/date.util";
import { Badge, Space, Tooltip } from "antd";
import type { ColumnsType } from "antd/es/table";
import Text from "antd/es/typography/Text";
import { useMemo } from "react";

interface MemberListProps {
  hasHadData: boolean;
  isLoading: boolean;
  data: TeamMemberItem[];
  userId: string;
  searchProps: SearchProps;
  roleProps: RoleProps;
  paginationProps: PaginationProps;
}

export function MemberList({
  hasHadData,
  isLoading,
  data,
  userId,
  searchProps,
  roleProps,
  paginationProps,
}: MemberListProps) {
  const memberColumns = useMemo<ColumnsType<TeamMemberItem>>(
    () => [
      {
        title: "Tên người dùng",
        dataIndex: "userName",
        key: "userName",
        render: (userName: string, record: TeamMemberItem) => {
          const timeDistance = getDistanceToNow(
            record.lastActiveAt,
            "minute",
            false,
          );
          const isActive =
            timeDistance !== null &&
            timeDistance <= 10 &&
            userId != record.userId;

          return (
            <Space>
              <Text strong>{userName}</Text>
              {isActive && (
                <Tooltip title="Đang hoạt động">
                  <Badge status="processing" />
                </Tooltip>
              )}
              {userId === record.userId && <Text strong>(Bạn)</Text>}
            </Space>
          );
        },
      },
      {
        title: "Email",
        dataIndex: "email",
        key: "email",
        render: (email: string) => email || <Text type="secondary">—</Text>,
      },

      {
        title: "Vai trò",
        dataIndex: "role",
        key: "role",
        render: (role: string) => role || <Text type="secondary">—</Text>,
      },
      {
        title: "Ngày tham gia",
        dataIndex: "joinedAt",
        key: "joinedAt",
        render: (joinedAt: string) =>
          new Date(joinedAt).toLocaleDateString("vi-VN"),
      },
    ],
    [userId],
  );

  // const { searchProps, roleProps, paginationProps, queryParams } = useSearch(
  //   {},
  // );

  // const [hasHadData, setHasHadData] = useState(false);

  // const { getMembers } = useTeam();

  // const { data, isLoading } = getMembers(
  //   teamId,
  //   queryParams.search,
  //   queryParams.role ?? undefined,
  //   queryParams.page,
  //   queryParams.pageSize,
  // );

  // useEffect(() => {
  //   if (!isLoading && data && data.total > 0 && !hasHadData) {
  //     setHasHadData(true);
  //   }
  // }, [data, isLoading, hasHadData]);

  return (
    <SearchPagination
      search={
        hasHadData
          ? { ...searchProps, placeholder: "Tên người dùng, email..." }
          : undefined
      }
      role={hasHadData ? roleProps : undefined}
      pagination={{ ...paginationProps }}
      tableProps={{
        dataSource: data,
        columns: memberColumns,
        rowKey: "id",
        loading: isLoading,
        locale: {
          emptyText: "Không có thành viên nào",
        },
      }}
    />
  );
}
