import { Button, Result } from "antd";
import { CloseCircleOutlined } from "@ant-design/icons";

interface NotFoundProps {
  icon?: React.ReactNode;
  title?: string;
  description?: string;
  onHandleClick?: () => void;
  buttonText?: string;
}

export function NotFound({
  icon,
  title,
  description,
  onHandleClick,
  buttonText,
}: NotFoundProps) {
  return (
    <Result
      icon={icon || <CloseCircleOutlined />}
      title={title}
      subTitle={description}
      extra={
        buttonText && (
          <Button type="primary" onClick={onHandleClick}>
            {buttonText}
          </Button>
        )
      }
    />
  );
}
