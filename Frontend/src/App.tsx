import AppRouter from "@/router/AppRouter";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { ConfigProvider } from "antd";
import { aiWorkspaceTheme } from "@/components/types";
import { useTheme } from "@/hooks";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5, // Dữ liệu được coi là "fresh" trong 5 phút trước khi cần refetch ngầm
      refetchOnWindowFocus: false, // Không tự động refetch khi người dùng chuyển tab
    },
  },
});

export default function App() {
  const { theme } = useTheme();

  return (
    <ConfigProvider
      theme={theme === "dark" ? aiWorkspaceTheme.dark : aiWorkspaceTheme.light}
    >
      <QueryClientProvider client={queryClient}>
        <AppRouter />
        <ReactQueryDevtools initialIsOpen={false} />
      </QueryClientProvider>
    </ConfigProvider>
  );
}
