/**
 * File Upload Models and Interfaces
 */

export interface FileUploadRequest {
  file: File;
  category: 'profile' | 'document' | 'qualification' | 'certification';
  metadata?: {
    documentTypeId?: number;
    documentNumber?: string;
    expiryDate?: string;
    qualificationId?: number;
    certificationId?: number;
  };
}

export interface UploadProgress {
  fileName: string;
  progress: number;
  status: 'pending' | 'uploading' | 'success' | 'error';
  error?: string;
}

export interface FilePreview {
  file: File;
  url: string;
  type: 'image' | 'pdf' | 'document';
  size: number;
  name: string;
}

export interface FileValidationResult {
  isValid: boolean;
  errors: string[];
}

/**
 * File validation configuration
 */
export const FILE_UPLOAD_CONFIG = {
  images: {
    maxSize: 5 * 1024 * 1024, // 5MB
    allowedTypes: ['image/jpeg', 'image/jpg', 'image/png'],
    allowedExtensions: ['.jpg', '.jpeg', '.png']
  },
  documents: {
    maxSize: 10 * 1024 * 1024, // 10MB
    allowedTypes: ['application/pdf', 'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'],
    allowedExtensions: ['.pdf', '.doc', '.docx']
  }
};

/**
 * Helper function to validate file
 */
export function validateFile(file: File, category: 'image' | 'document'): FileValidationResult {
  const errors: string[] = [];
  const config = category === 'image' ? FILE_UPLOAD_CONFIG.images : FILE_UPLOAD_CONFIG.documents;

  // Check file size
  if (file.size > config.maxSize) {
    const maxSizeMB = config.maxSize / (1024 * 1024);
    errors.push(`حجم الملف يجب أن لا يتجاوز ${maxSizeMB} ميجابايت`);
  }

  // Check file type
  if (!config.allowedTypes.includes(file.type)) {
    errors.push(`نوع الملف غير مدعوم. الأنواع المسموحة: ${config.allowedExtensions.join(', ')}`);
  }

  return {
    isValid: errors.length === 0,
    errors
  };
}

/**
 * Helper function to create file preview
 */
export function createFilePreview(file: File): FilePreview {
  const url = URL.createObjectURL(file);
  let type: 'image' | 'pdf' | 'document' = 'document';

  if (file.type.startsWith('image/')) {
    type = 'image';
  } else if (file.type === 'application/pdf') {
    type = 'pdf';
  }

  return {
    file,
    url,
    type,
    size: file.size,
    name: file.name
  };
}
