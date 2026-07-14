import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { ConfigProvider, App as AntdApp } from "antd";
import { aiWorkspaceTheme } from "@/constants";
import { useTheme } from "@/hooks";
import AppRouter from "./router";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5, // After 5 minutes, the data is considered stale
      refetchOnWindowFocus: false, // No refetch for switching tabs
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
        <AntdApp>
          <AppRouter />
        </AntdApp>
        <ReactQueryDevtools initialIsOpen={false} />
      </QueryClientProvider>
    </ConfigProvider>
  );
}
