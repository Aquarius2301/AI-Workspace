import { Modal as AntdModal, Button, Drawer, Grid, Flex } from "antd";
import { useTranslation } from "react-i18next";

const { useBreakpoint } = Grid;

export type FooterType =
  | "ok"
  | "cancel"
  | "create"
  | "update"
  | "delete"
  | "close";

export interface AIModalProps {
  children?: React.ReactNode;
  title?: string;
  open?: boolean;
  onOk?: () => void;
  onCancel?: () => void;
  width?: number | string;
  centered?: boolean;
  isLoading?: boolean;
  footer?: {
    type: FooterType;
    text?: string;
    disabled?: boolean;
    onClick?: () => void;
  }[];
}

type FooterItem = {
  type: FooterType;
  text?: string;
  disabled?: boolean;
  onClick?: () => void;
};

function renderFooterButton(
  item: FooterItem,
  props: { onOk?: () => void; onCancel?: () => void; isLoading?: boolean },
  t: (key: string) => string,
) {
  const { onOk, onCancel, isLoading } = props;
  switch (item.type) {
    case "ok":
      return (
        <Button
          key="ok"
          type="primary"
          onClick={item.onClick || onOk}
          loading={isLoading}
          disabled={item.disabled}
          block
        >
          {item.text || t("modal.ok")}
        </Button>
      );
    case "cancel":
      return (
        <Button
          key="cancel"
          onClick={item.onClick || onCancel}
          disabled={item.disabled}
          block
        >
          {item.text || t("modal.cancel")}
        </Button>
      );
    case "close":
      return (
        <Button
          key="close"
          onClick={item.onClick || onCancel}
          disabled={item.disabled}
          block
        >
          {item.text || t("modal.close")}
        </Button>
      );
    case "create":
      return (
        <Button
          key="create"
          type="primary"
          onClick={item.onClick || onOk}
          loading={isLoading}
          disabled={item.disabled}
          block
        >
          {item.text || t("modal.create")}
        </Button>
      );
    case "update":
      return (
        <Button
          key="update"
          type="primary"
          onClick={item.onClick || onOk}
          loading={isLoading}
          disabled={item.disabled}
          block
        >
          {item.text || t("modal.update")}
        </Button>
      );
    case "delete":
      return (
        <Button
          key="delete"
          type="primary"
          danger
          onClick={item.onClick || onOk}
          loading={isLoading}
          disabled={item.disabled}
          block
        >
          {item.text || t("modal.delete")}
        </Button>
      );
  }
}

export function AIModal({
  children,
  title,
  open,
  onCancel,
  onOk,
  width,
  centered,
  isLoading,
  footer,
}: AIModalProps) {
  const { t } = useTranslation();
  const screens = useBreakpoint();
  const isMobile = !screens.md;

  const buttonProps = { onOk, onCancel, isLoading };

  // ── Mobile: Drawer bottom sheet ──
  if (isMobile) {
    return (
      <Drawer
        title={title}
        open={open}
        onClose={onCancel}
        placement="bottom"
        size="auto"
        mask={{ closable: false }}
        closable={false}
        styles={{
          body: { padding: "16px" },
          footer: { padding: " 8px" },
        }}
        destroyOnHidden
        footer={
          footer ? (
            <Flex vertical gap={8}>
              {footer.map((item) => renderFooterButton(item, buttonProps, t))}
            </Flex>
          ) : undefined
        }
      >
        {children}
      </Drawer>
    );
  }

  // ── Desktop: Modal ──
  return (
    <AntdModal
      destroyOnHidden
      closeIcon={null}
      title={title}
      open={open}
      onOk={onOk}
      onCancel={onCancel}
      closable={false}
      width={width ?? 520}
      centered={centered}
      confirmLoading={isLoading}
      mask={{ closable: false }}
      footer={
        footer
          ? () => (
              <Flex justify="end" gap={8}>
                {footer.map((item) => renderFooterButton(item, buttonProps, t))}
              </Flex>
            )
          : undefined
      }
    >
      {children}
    </AntdModal>
  );
}
