import { useEffect, useState } from "react";

/**
 * A custom hook that debounces a value over a specified delay. It returns the debounced value, which only updates after the specified delay has passed since the last change to the input value.
 * @param value  The input value to be debounced.
 * @param delay  The delay in milliseconds to wait before updating the debounced value.
 * @returns The debounced value that updates after the specified delay.
 */
export function useDebounce<T>(value: T, delay: number): T {
  const [debouncedValue, setDebouncedValue] = useState<T>(value);

  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);

    return () => {
      clearTimeout(handler); //Remove the timeout if value or delay changes before the timeout completes
    };
  }, [value, delay]);

  return debouncedValue;
}
