import { Row, Col, theme } from "antd";
import { AICard } from "./AICard";

export interface AICardItemProps {
  header?: React.ReactNode;
  content?: React.ReactNode;
  footer?: React.ReactNode;
  leftSide?: React.ReactNode;
  rightSide?: React.ReactNode;
  leftSideSpan?: number;
  rightSideSpan?: number;
  isHoverable?: boolean;
  isLoading?: boolean;
  style?: React.CSSProperties;
}

export function AICardItem({
  header,
  content,
  footer,
  leftSide,
  rightSide,
  leftSideSpan = 3,
  rightSideSpan = 3,
  isHoverable = false,
  isLoading = false,
  style,
}: AICardItemProps) {
  const { token } = theme.useToken();

  // Kiểm tra xem có phần thân (header/content) để quyết định kẻ đường line cho footer hay không
  const hasBodyContent = header || content;

  const centerSpan = //Calculate the span for the center column based on the presence of leftSide and rightSide
    24 - (leftSide ? leftSideSpan : 0) - (rightSide ? rightSideSpan : 0);

  return (
    <AICard isHovering={isHoverable} isLoading={isLoading} style={style}>
      <Row gutter={[16, 12]} align="stretch">
        {/* ── LEFT SIDE ────────────────────────────────────────────── */}
        {leftSide && (
          <Col
            xs={leftSideSpan}
            md={leftSideSpan}
            style={{
              display: "flex",
              alignItems: "center",
              // justifyContent: "center",
            }}
          >
            {leftSide}
          </Col>
        )}

        {/* ── MAIN CONTENT (Header + Body + Footer) ────────────────── */}
        <Col
          xs={24}
          md={centerSpan}
          style={{ display: "flex", flexDirection: "column", minWidth: 0 }}
        >
          {header && (
            <div style={{ paddingBottom: content || footer ? 12 : 0 }}>
              {header}
            </div>
          )}

          {content && (
            <div style={{ flex: 1, paddingBottom: footer ? 12 : 0 }}>
              {content}
            </div>
          )}

          {footer && (
            <div
              style={{
                paddingTop: hasBodyContent ? 12 : 0,
                borderTop: hasBodyContent
                  ? `1px solid ${token.colorBorderSecondary}`
                  : undefined,
              }}
            >
              {footer}
            </div>
          )}
        </Col>

        {/* ── RIGHT SIDE ───────────────────────────────────────────── */}
        {rightSide && (
          <Col
            xs={24}
            md={rightSideSpan}
            style={{
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
            }}
          >
            {rightSide}
          </Col>
        )}
      </Row>
    </AICard>
  );
}
