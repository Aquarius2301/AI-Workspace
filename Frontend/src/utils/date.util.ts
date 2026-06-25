export type DateFormatType =
  | "DD/MM/YYYY"
  | "DD/MM/YYYY HH:mm"
  | "YYYY-MM-DD"
  | "TEXT";

/**
 * Chuyển đổi chuỗi thời gian ISO sang các định dạng ngày tháng năm khác nhau.
 * @param isoString Chuỗi thời gian đầu vào (Ví dụ: 2026-06-16T08:34:12.0499568)
 * @param format Định dạng mong muốn đầu ra
 * @returns Chuỗi ngày tháng đã được định dạng
 */
export const formatIsoDate = (
  isoString: string | null | undefined,
  format: DateFormatType = "DD/MM/YYYY",
): string => {
  if (!isoString) return "";

  const date = new Date(isoString);

  // Kiểm tra nếu chuỗi truyền vào không phải là ngày hợp lệ
  if (isNaN(date.getTime())) return "";

  // Lấy ra các thành phần ngày, tháng, năm, giờ, phút
  const day = String(date.getDate()).padStart(2, "0");
  const month = String(date.getMonth() + 1).padStart(2, "0"); // Tháng trong JS chạy từ 0-11
  const year = date.getFullYear();
  const hours = String(date.getHours()).padStart(2, "0");
  const minutes = String(date.getMinutes()).padStart(2, "0");

  switch (format) {
    case "DD/MM/YYYY":
      return `${day}/${month}/${year}`; // Kết quả: 16/06/2026

    case "DD/MM/YYYY HH:mm":
      return `${day}/${month}/${year} ${hours}:${minutes}`; // Kết quả: 16/06/2026 08:34

    case "YYYY-MM-DD":
      return `${year}-${month}-${day}`; // Kết quả: 2026-06-16 (Thường dùng cho thẻ <input type="date">)

    case "TEXT":
      return `Ngày ${day} tháng ${month} năm ${year}`; // Kết quả: Ngày 16 tháng 06 năm 2026

    default:
      return `${day}/${month}/${year}`;
  }
};

// File: src/utils/dateUtils.ts

export type DateUnitType = 'day' | 'hour' | 'minute' | 'month' | 'year';

/**
 * Tính khoảng cách giữa một chuỗi ngày ISO với thời điểm hiện tại.
 * @param isoString Chuỗi thời gian cần kiểm tra (Ví dụ: 2026-06-16T08:34:12)
 * @param unit Đơn vị muốn tính (mặc định là 'day')
 * @param absolute Có lấy giá trị tuyệt đối (luôn dương) hay không (mặc định là true)
 * @returns Số lượng khoảng cách (ví dụ: số ngày, số giờ...) hoặc null nếu ngày không hợp lệ
 */
export const getDistanceToNow = (
  isoString: string | null | undefined,
  unit: DateUnitType = 'day',
  absolute: boolean = true
): number | null => {
  if (!isoString) return null;

  const pastDate = new Date(isoString);
  const now = new Date(); // Lấy thời gian hiện tại

  // Kiểm tra ngày hợp lệ
  if (isNaN(pastDate.getTime())) return null;

  // Tính khoảng cách bằng mili-giây
  const diffInMs = pastDate.getTime() - now.getTime();

  // Đổi mili-giây sang đơn vị mong muốn
  let diff = 0;
  switch (unit) {
    case 'minute':
      diff = diffInMs / (1000 * 60);
      break;
    case 'hour':
      diff = diffInMs / (1000 * 60 * 60);
      break;
    case 'day':
      diff = diffInMs / (1000 * 60 * 60 * 24);
      break;
    case 'month':
      // Tính tương đối số tháng
      diff = (pastDate.getFullYear() - now.getFullYear()) * 12 + (pastDate.getMonth() - now.getMonth());
      break;
    case 'year':
      diff = pastDate.getFullYear() - now.getFullYear();
      break;
    default:
      diff = diffInMs / (1000 * 60 * 60 * 24);
  }

  // Làm tròn xuống số nguyên (ví dụ: 2.7 ngày thành 2 ngày)
  // Bạn có thể dùng Math.round() nếu muốn làm tròn gần nhất
  const result = Math.floor(diff);

  return absolute ? Math.abs(result) : result;
};
