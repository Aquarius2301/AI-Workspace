import i18n from "@/i18n";
import type { ApiErrorResponse, ValidationError } from "@/types";
import { toCamelCase } from "./common.util";
import type { FormInstance } from "antd";
// import type { FieldData } from "rc-field-form/lib/interface";

function translateErrorMessage(code: string): string {
  const key = `error.${code}`;
  return i18n.exists(key) ? i18n.t(key) : i18n.t("error.InternalServerError");
}

function extractApiError(error: unknown): ApiErrorResponse {
  const err = error as ApiErrorResponse;
  return (
    err ?? {
      success: false,
      message: "InternalServerError",
      data: null,
    }
  );
}

/** Use to catch the error by message
 * @params error: the error
 * @return The error message or 'InternalServerError' message already translated
 */
export function getErrorMessage(error: unknown): string {
  const err = extractApiError(error);
  return translateErrorMessage(err.message ?? "InternalServerError");
}

type FieldData<T> = Parameters<FormInstance<T>["setFields"]>[0][number];
/**
 * Use to catch the validation error from form
 * @param error: the error
 * @returns The array contains name's field and the list of errors, return empty if there is no validation error
 */
export function getFormFieldErrors<T>(error: unknown): FieldData<T>[] {
  const err = extractApiError(error);

  if (!Array.isArray(err.data)) {
    return [];
  }

  return (err.data as ValidationError[]).map((e) => ({
    name: toCamelCase(e.field) as FieldData<T>["name"],
    errors: [translateErrorMessage(e.message)],
  }));
}
