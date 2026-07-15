import { useState } from "react";
import { useDebounce } from "./useDebounce.hook";
import type {
  PageSize,
  ProjectRole,
  ProjectVisibility,
  TaskPriority,
  TaskStatus,
  TeamRole,
} from "@/types";

interface SearchProps {
  /**
   * Whether to include a role filter in the search functionality. Default is false.
   */
  hasRoleFilter?: boolean;
  /**
   * Whether to include a project role filter in the search functionality. Default is false.
   */
  hasProjectRoleFilter?: boolean;
  /**
   * Whether to include a visibility filter in the search functionality. Default is false.
   */
  hasVisibilityFilter?: boolean;
  /**
   * Whether to include a task status filter in the search functionality. Default is false.
   */
  hasTaskStatusFilter?: boolean;
  /**
   * Whether to include a task priority filter in the search functionality. Default is false.
   */
  hasPriorityFilter?: boolean;
  /**
   * The debounce time in milliseconds for the search input. Default is 500ms.
   */
  debounceTime?: number;
  /**
   * The initial search string to populate the search input. Default is an empty string.
   */
  initialSearch?: string;
  /**
   * The initial role to filter the search results. Default is undefined (no role filter).
   */
  initialRole?: TeamRole | undefined;
  /**
   * The initial project role to filter the search results. Default is undefined (no role filter).
   */
  initialProjectRole?: ProjectRole | undefined;
  /**
   * The initial visibility to filter the search results. Default is undefined (no visibility filter).
   */
  initialVisibility?: ProjectVisibility | undefined;
  /**
   * The initial task status to filter the search results. Default is undefined (no task status filter).
   */
  initialTaskStatus?: TaskStatus | undefined;
  /**
   * The initial task priority to filter the search results. Default is undefined (no priority filter).
   */
  initialPriority?: TaskPriority | undefined;
  /**
   * The initial page number for pagination. Default is 1.
   */
  initialPage?: number;
  /**
   * The initial page size for pagination. Default is 10.
   */
  initialPageSize?: PageSize;
}

/**
 *  A custom hook that manages search functionality with debouncing, role filtering, and pagination.
 * @param props - The properties to configure the search behavior, including debounce time, initial search string, initial role filter, initial page number, and initial page size.
 * @returns
 * An object containing:
 * - `searchProps`: An object with the current search string and a function to update it.
 * - `roleProps`: An object with the current role filter and a function to update it.
 * - `projectRoleProps`: An object with the current project role filter and a function to update it.
 * - `paginationProps`: An object with the current page number, page size, and functions to update them.
 * - `queryParams`: An object containing the debounced search string, current role filter, current page number, and current page size for use in API queries.
 */
export function useSearch({
  hasRoleFilter = false,
  hasProjectRoleFilter = false,
  hasVisibilityFilter = false,
  hasTaskStatusFilter = false,
  hasPriorityFilter = false,
  debounceTime = 500,
  initialSearch = "",
  initialRole = undefined,
  initialProjectRole = undefined,
  initialVisibility = undefined,
  initialTaskStatus = undefined,
  initialPriority = undefined,
  initialPage = 1,
  initialPageSize = 10,
}: SearchProps) {
  const [role, setRole] = useState<TeamRole | undefined>(initialRole);
  const [projectRole, setProjectRole] = useState<ProjectRole | undefined>(
    initialProjectRole,
  );
  const [visibility, setVisibility] = useState<ProjectVisibility | undefined>(
    initialVisibility,
  );
  const [taskStatus, setTaskStatus] = useState<TaskStatus | undefined>(
    initialTaskStatus,
  );
  const [priority, setPriority] = useState<TaskPriority | undefined>(
    initialPriority,
  );
  const [search, setSearch] = useState<string>(initialSearch);
  const [page, setPage] = useState<number>(initialPage);
  const [pageSize, setPageSize] = useState<PageSize>(initialPageSize);
  const debounce = useDebounce(search, debounceTime);

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

  const onProjectRoleChange = (value: ProjectRole | undefined) => {
    setProjectRole(value);
    setPage(1);
  };

  const onVisibilityChange = (value: ProjectVisibility | undefined) => {
    setVisibility(value);
    setPage(1);
  };

  const onTaskStatusChange = (value: TaskStatus | undefined) => {
    setTaskStatus(value);
    setPage(1);
  };

  const onPriorityChange = (value: TaskPriority | undefined) => {
    setPriority(value);
    setPage(1);
  };

  return {
    searchProps: { search, onSearchChange },
    roleProps: { role, onRoleChange },
    projectRoleProps: { projectRole, onProjectRoleChange },
    visibilityProps: { visibility, onVisibilityChange },
    taskStatusProps: { taskStatus, onTaskStatusChange },
    priorityProps: { priority, onPriorityChange },
    paginationProps: {
      page,
      pageSize,
      onPageChange,
      onPageSizeChange,
    },
    queryParams: {
      search: debounce,
      role: hasRoleFilter ? role : undefined,
      projectRole: hasProjectRoleFilter ? projectRole : undefined,
      visibility: hasVisibilityFilter ? visibility : undefined,
      taskStatus: hasTaskStatusFilter ? taskStatus : undefined,
      priority: hasPriorityFilter ? priority : undefined,
      page,
      pageSize,
    },
  };
}
