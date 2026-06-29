import { AICard } from "@/components";
import { Flex, Statistic, theme } from "antd";

interface StatCardProps {
  isLoading: boolean;
  icon: React.ReactNode;
  value: number;
  label: string;
  color: string;
}

export function StatCard({
  isLoading,
  icon,
  value,
  label,
  color,
}: StatCardProps) {
  const { token } = theme.useToken();

  return (
    <AICard isLoading={isLoading} style={{ height: "100%" }}>
      <Flex align="center" gap={16}>
        <Flex
          align="center"
          justify="center"
          style={{
            width: 48,
            height: 48,
            borderRadius: token.borderRadius,
            background: `${color}14`,
            fontSize: 22,
            color,
            flexShrink: 0,
          }}
        >
          {icon}
        </Flex>
        <Statistic
          value={value}
          title={label}
          styles={{ content: { fontSize: 26, fontWeight: 700 } }}
        />
      </Flex>
    </AICard>
  );
}
