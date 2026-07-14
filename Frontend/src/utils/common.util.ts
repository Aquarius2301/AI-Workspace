/**
 * Calculates the percentage of completed tasks out of total tasks.
 * @param value the value
 * @param total the total number
 * @returns the percentage of completed tasks, rounded to one decimal place. If total is 0, returns 0.
 */
export const getPercentage = (value: number, total: number) => {
  if (total === 0) {
    return 0;
  }

  const percentage = (value / total) * 100;

  return Math.round(percentage * 10) / 10;
};

export function toCamelCase(value: string): string {
  return value.charAt(0).toLowerCase() + value.slice(1);
}
