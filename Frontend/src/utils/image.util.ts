const MAX_FILE_SIZE = 5 * 1024 * 1024; // 5 MB
const COMPRESS_THRESHOLD = 3 * 1024 * 1024; // 3 MB

function readFileAsDataURL(file: File): Promise<string> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => resolve(reader.result as string);
    reader.onerror = reject;
    reader.readAsDataURL(file);
  });
}

function loadImage(src: string): Promise<HTMLImageElement> {
  return new Promise((resolve, reject) => {
    const img = new Image();
    img.onload = () => resolve(img);
    img.onerror = reject;
    img.src = src;
  });
}

function canvasToBlob(
  img: HTMLImageElement,
  width: number,
  height: number,
  quality: number,
): Promise<Blob> {
  return new Promise((resolve, reject) => {
    const canvas = document.createElement("canvas");
    canvas.width = width;
    canvas.height = height;
    const ctx = canvas.getContext("2d")!;
    ctx.drawImage(img, 0, 0, width, height);

    canvas.toBlob(
      (blob) => {
        if (blob) resolve(blob);
        else reject(new Error("Canvas toBlob failed"));
      },
      "image/jpeg",
      quality,
    );
  });
}

function calculateDimensions(
  width: number,
  height: number,
  scale: number,
): { width: number; height: number } {
  return {
    width: Math.round(width * scale),
    height: Math.round(height * scale),
  };
}

export interface CompressResult {
  file: File;
  dataUrl: string;
}

/**
 * Compress an image file to ensure it's under 5MB.
 * Uses Canvas API to reduce quality and/or dimensions.
 *
 * - If file is ≤ 3MB: return as-is (no compression needed)
 * - If file is > 3MB: progressively reduce quality/dimensions
 */
export async function compressImage(file: File): Promise<CompressResult> {
  // No compression needed
  if (file.size <= COMPRESS_THRESHOLD) {
    return { file, dataUrl: URL.createObjectURL(file) };
  }

  const dataUrl = await readFileAsDataURL(file);
  const img = await loadImage(dataUrl);

  let quality = 0.85;
  let width = img.width;
  let height = img.height;
  let compressedBlob = await canvasToBlob(img, width, height, quality);

  // Reduce quality first
  while (compressedBlob.size > MAX_FILE_SIZE && quality > 0.2) {
    quality -= 0.1;
    compressedBlob = await canvasToBlob(img, width, height, quality);
  }

  // If still > 5MB, reduce dimensions progressively
  if (compressedBlob.size > MAX_FILE_SIZE) {
    quality = 0.7;
    let scale = 0.9;

    while (compressedBlob.size > MAX_FILE_SIZE && width > 100) {
      const dims = calculateDimensions(img.width, img.height, scale);
      width = dims.width;
      height = dims.height;
      compressedBlob = await canvasToBlob(img, width, height, quality);
      scale -= 0.1;
    }
  }

  return {
    file: new File([compressedBlob], file.name.replace(/\.[^.]+$/, ".jpg"), {
      type: "image/jpeg",
    }),
    dataUrl: URL.createObjectURL(compressedBlob),
  };
}

/**
 * Validate that a file is an allowed image type.
 */
export function isValidImageType(file: File): boolean {
  const allowedMimeTypes = [
    "image/jpeg",
    "image/png",
    "image/gif",
    "image/webp",
  ];
  const allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];

  // Check MIME type first (most reliable)
  if (file.type && allowedMimeTypes.includes(file.type)) return true;

  // Fallback to extension check when MIME type is empty (some browsers)
  const ext = "." + file.name.split(".").pop()?.toLowerCase();
  return allowedExtensions.includes(ext);
}

/**
 * Format file size for display.
 */
export function formatFileSize(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}
