import { useEffect, useState } from "react";
import { useDebounce } from "./useDebounce.hook";
import type { PageSize, TeamRole } from "@/types";

interface SearchProps {
  debounceTime?: number;
  initialSearch?: string;
  initialRole?: TeamRole | undefined;
  initialPage?: number;
  initialPageSize?: PageSize;
  total?: number;
  isLoading?: boolean;
}

export function useSearch({
  debounceTime = 500,
  initialSearch = "",
  initialRole = undefined,
  initialPage = 1,
  initialPageSize = 10,
  total = 0,
  isLoading,
}: SearchProps) {
  const [role, setRole] = useState<TeamRole | undefined>(initialRole);
  const [search, setSearch] = useState<string>(initialSearch);
  const [page, setPage] = useState<number>(initialPage);
  const [pageSize, setPageSize] = useState<PageSize>(initialPageSize);
  const [isFirstLoad, setIsFirstLoad] = useState(true);
  const debounce = useDebounce(search, debounceTime);

  useEffect(() => {
    if (isLoading && total > 0 && isFirstLoad) {
      setIsFirstLoad(false);
    }
  }, [isLoading, total, isFirstLoad]);

  const onSearchChange = (value: string) => {
    setSearch(value);
    setPage(1);
  };

  const onPageChange = (newPage: number) => setPage(newPage);

  const onPageSizeChange = (newSize: PageSize) => {
    setPageSize(newSize);
    setPage(1);
  };

  const onRoleChange = (value: TeamRole | undefined) => {
    setRole(value);
    setPage(1);
  };

  return {
    searchProps: { search, onSearchChange },
    roleProps: { role, onRoleChange },
    paginationProps: {
      page,
      pageSize,
      onPageChange,
      onPageSizeChange,
    },
    queryParams: { search: debounce, role, page, pageSize },
  };
}
