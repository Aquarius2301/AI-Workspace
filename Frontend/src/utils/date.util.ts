import i18n from "@/i18n";

/**
 * Formats an ISO date string into locale date format.
 * @param isoString ISO date string (Example: 2026-06-16T08:34:12)
 * @param options Date display format
 */
export const formatIsoLocaleDate = (
  isoString: string | null | undefined,
  hourIncluded: boolean = false,
): string => {
  if (!isoString) return "";

  const date = new Date(isoString);

  const options: Intl.DateTimeFormatOptions = !hourIncluded
    ? {
        year: "numeric",
        month: "short",
        day: "2-digit",
      }
    : {
        year: "numeric",
        month: "short",
        day: "2-digit",
        hour: "2-digit",
        minute: "2-digit",
      };

  if (isNaN(date.getTime())) return "";

  return date.toLocaleDateString(i18n.language, options);
};

export type DateUnitType = "day" | "hour" | "minute" | "month" | "year";

/**
 * Calculate the distance between an ISO date string and the current time.
 *
 * @param isoString The date string to compare (Example: 2026-06-16T08:34:12)
 * @param unit The unit to calculate (default: "day")
 * @param absolute Whether to return absolute value (always positive) (default: true)
 * @returns The distance value (days, hours, etc.) or null if invalid
 */
export const getDistanceToNow = (
  isoString: string | null | undefined,
  unit: DateUnitType = "day",
  absolute: boolean = true,
): number | null => {
  if (!isoString) return null;

  const targetDate = new Date(isoString);
  const now = new Date();

  // Validate date
  if (isNaN(targetDate.getTime())) return null;

  // Calculate difference in milliseconds
  const diffInMs = targetDate.getTime() - now.getTime();

  // Convert milliseconds to selected unit
  let diff = 0;

  switch (unit) {
    case "minute":
      diff = diffInMs / (1000 * 60);
      break;

    case "hour":
      diff = diffInMs / (1000 * 60 * 60);
      break;

    case "day":
      diff = diffInMs / (1000 * 60 * 60 * 24);
      break;

    case "month":
      // Approximate month difference
      diff =
        (targetDate.getFullYear() - now.getFullYear()) * 12 +
        (targetDate.getMonth() - now.getMonth());
      break;

    case "year":
      diff = targetDate.getFullYear() - now.getFullYear();
      break;

    default:
      diff = diffInMs / (1000 * 60 * 60 * 24);
  }

  const result = Math.floor(diff); // 2.7 => 2

  return absolute ? Math.abs(result) : result;
};
