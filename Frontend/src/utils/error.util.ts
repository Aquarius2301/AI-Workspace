import i18n from "@/i18n";

/**
 * Extracts an error code/message from an API error response, then translates it.
 *
 * Supports two response formats from the backend:
 *   - New format: { success: false, message: "SomeErrorCode" }
 *   - Legacy format: plain string "SomeErrorCode"
 *
 * The error code is used as a key `error.{code}` in the i18n translation file.
 * If the key is not found, falls back to "error.InternalServerError".
 */
export function getTranslatedErrorMessage(error: unknown): string {
  const axiosError = error as import("axios").AxiosError<unknown>;
  const data = axiosError.response?.data;

  let errorCode: string | undefined;

  // New format: { success: false, message: "ErrorCode" }
  if (data && typeof data === "object" && "message" in data) {
    const msg = (data as { message: string }).message;
    if (msg && typeof msg === "string") {
      errorCode = msg;
    }
  }

  if (errorCode) {
    const translationKey = `error.${errorCode}`;

    // Return translated message if the key exists; otherwise fall back
    if (i18n.exists(translationKey)) {
      return i18n.t(translationKey);
    }
  }

  // Fallback: return the default server error message
  return i18n.t("error.InternalServerError");
}
