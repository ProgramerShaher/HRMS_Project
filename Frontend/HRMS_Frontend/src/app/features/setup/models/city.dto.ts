export interface CityDto {
  cityId: number;
  countryId: number;
  cityNameAr: string;
  cityNameEn: string;
  countryNameAr: string;
  countryNameEn: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string | null;
}

export interface CityListDto {
  cityId: number;
  cityNameAr: string;
  cityNameEn: string;
  countryNameAr: string;
  isActive: boolean;
}

export interface CreateCityCommand {
  cityNameAr: string;
  cityNameEn: string;
  countryId: number;
}

export interface UpdateCityCommand {
  cityId: number;
  countryId: number;
  cityNameAr: string;
  cityNameEn: string;
  isActive: boolean;
}
