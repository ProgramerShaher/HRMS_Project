import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { Employee } from '../models/employee.model';
import { CreateEmployeeDto } from '../models/create-employee.dto';
import { EmployeeProfile } from '../models/employee-profile.model';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private apiUrl = `${environment.apiUrl}/Employees`;

  constructor(private http: HttpClient) { }

  /**
   * Get paginated employees
   */
  getAll(pageNumber: number = 1, pageSize: number = 10, search?: string): Observable<any> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (search) {
      params = params.set('search', search);
    }

    return this.http.get<any>(this.apiUrl, { params });
  }

  /**
   * Get employee basic details by ID
   */
  getById(id: number): Observable<EmployeeProfile> {
    return this.http.get<EmployeeProfile>(`${this.apiUrl}/${id}`);
  }

  /**
   * Get full employee profile (Aggregated view)
   */
  getFullProfile(id: number): Observable<EmployeeProfile> {
    return this.http.get<any>(`${environment.apiUrl}/employee-profile/${id}/full-profile`).pipe(
      map(response => response.data)
    );
  }

  /**
   * Create a new employee with full details
   */
  create(employee: CreateEmployeeDto): Observable<number> {
    return this.http.post<number>(this.apiUrl, employee);
  }

  /**
   * Update employee main info
   */
  update(id: number, employee: Partial<CreateEmployeeDto>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, employee);
  }

  /**
   * Upload an employee document
   */
  uploadDocument(employeeId: number, file: File, documentTypeId: number, documentNumber?: string, expiryDate?: string): Observable<any> {
    const formData = new FormData();
    formData.append('EmployeeId', employeeId.toString());
    formData.append('DocumentTypeId', documentTypeId.toString());
    formData.append('File', file);
    
    if (documentNumber) formData.append('DocumentNumber', documentNumber);
    if (expiryDate) formData.append('ExpiryDate', expiryDate);

    // Using the correct endpoint structure: api/employee-profile/{employeeId}/documents
    return this.http.post(`${environment.apiUrl}/employee-profile/${employeeId}/documents`, formData);
  }

  /**
   * Upload profile picture
   */
  uploadProfilePicture(employeeId: number, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('photo', file);

    // Using the correct endpoint: api/employee-profile/{id}/photo
    return this.http.put(`${environment.apiUrl}/employee-profile/${employeeId}/photo`, formData);
  }
  // ====================================================================================================
  // 1. Qualifications
  // ====================================================================================================
  getQualifications(employeeId: number): Observable<any> {
    return this.http.get<any>(`${environment.apiUrl}/employee-profile/${employeeId}/qualifications`);
  }
  
  addQualification(employeeId: number, data: any, file?: File): Observable<any> {
    const formData = new FormData();
    formData.append('EmployeeId', employeeId.toString());
    Object.keys(data).forEach(key => {
        if (data[key] !== null && data[key] !== undefined) formData.append(key, data[key]);
    });
    if (file) formData.append('Attachment', file);
    return this.http.post(`${environment.apiUrl}/employee-profile/${employeeId}/qualifications`, formData);
  }

  deleteQualification(employeeId: number, id: number): Observable<any> {
      return this.http.delete(`${environment.apiUrl}/employee-profile/${employeeId}/qualifications/${id}`);
  }

  // ====================================================================================================
  // 2. Certifications
  // ====================================================================================================
  getCertifications(employeeId: number): Observable<any> {
      return this.http.get<any>(`${environment.apiUrl}/employee-profile/${employeeId}/certifications`);
  }

  addCertification(employeeId: number, data: any, file?: File): Observable<any> {
      const formData = new FormData();
      formData.append('EmployeeId', employeeId.toString());
      Object.keys(data).forEach(key => {
        if (data[key] !== null && data[key] !== undefined) formData.append(key, data[key]);
      });
      if (file) formData.append('Attachment', file);
      return this.http.post(`${environment.apiUrl}/employee-profile/${employeeId}/certifications`, formData);
  }

  deleteCertification(employeeId: number, id: number): Observable<any> {
      return this.http.delete(`${environment.apiUrl}/employee-profile/${employeeId}/certifications/${id}`);
  }

  // ====================================================================================================
  // 3. Experience
  // ====================================================================================================
  getExperiences(employeeId: number): Observable<any> {
      return this.http.get<any>(`${environment.apiUrl}/employee-profile/${employeeId}/experiences`);
  }

  addExperience(employeeId: number, data: any): Observable<any> {
      return this.http.post(`${environment.apiUrl}/employee-profile/${employeeId}/experiences`, data);
  }

  deleteExperience(employeeId: number, id: number): Observable<any> {
      return this.http.delete(`${environment.apiUrl}/employee-profile/${employeeId}/experiences/${id}`);
  }

  // ====================================================================================================
  // 4. Emergency Contacts
  // ====================================================================================================
  getEmergencyContacts(employeeId: number): Observable<any> {
      return this.http.get<any>(`${environment.apiUrl}/employee-profile/${employeeId}/emergency-contacts`);
  }

  addEmergencyContact(employeeId: number, data: any): Observable<any> {
      return this.http.post(`${environment.apiUrl}/employee-profile/${employeeId}/emergency-contacts`, data);
  }

  updateEmergencyContact(employeeId: number, id: number, data: any): Observable<any> {
      return this.http.put(`${environment.apiUrl}/employee-profile/${employeeId}/emergency-contacts/${id}`, data);
  }

  deleteEmergencyContact(employeeId: number, id: number): Observable<any> {
      return this.http.delete(`${environment.apiUrl}/employee-profile/${employeeId}/emergency-contacts/${id}`); // Note: Verify if ID is needed in path for DELETE based on swagger, usually yes.
  }

  // ====================================================================================================
  // 5. Dependents
  // ====================================================================================================
  getDependents(employeeId: number): Observable<any> {
      return this.http.get<any>(`${environment.apiUrl}/employee-profile/${employeeId}/dependents`);
  }

  addDependent(employeeId: number, data: any): Observable<any> {
      return this.http.post(`${environment.apiUrl}/employee-profile/${employeeId}/dependents`, data);
  }

  updateDependent(employeeId: number, id: number, data: any): Observable<any> {
      return this.http.put(`${environment.apiUrl}/employee-profile/${employeeId}/dependents/${id}`, data);
  }

  deleteDependent(employeeId: number, id: number): Observable<any> {
      return this.http.delete(`${environment.apiUrl}/employee-profile/${employeeId}/dependents/${id}`);
  }

  // ====================================================================================================
  // 6. Bank Accounts
  // ====================================================================================================
  getBankAccounts(employeeId: number): Observable<any> {
      return this.http.get<any>(`${environment.apiUrl}/employee-profile/${employeeId}/bank-accounts`);
  }

  addBankAccount(employeeId: number, data: any): Observable<any> {
      return this.http.post(`${environment.apiUrl}/employee-profile/${employeeId}/bank-accounts`, data);
  }

  updateBankAccount(employeeId: number, id: number, data: any): Observable<any> {
      return this.http.put(`${environment.apiUrl}/employee-profile/${employeeId}/bank-accounts/${id}`, data);
  }

  deleteBankAccount(employeeId: number, id: number): Observable<any> {
      return this.http.delete(`${environment.apiUrl}/employee-profile/${employeeId}/bank-accounts/${id}`);
  }

  // ====================================================================================================
  // 7. Addresses
  // ====================================================================================================
  getAddresses(employeeId: number): Observable<any> {
      return this.http.get<any>(`${environment.apiUrl}/employee-profile/${employeeId}/addresses`);
  }

  addAddress(employeeId: number, data: any): Observable<any> {
      return this.http.post(`${environment.apiUrl}/employee-profile/${employeeId}/addresses`, data);
  }

  updateAddress(employeeId: number, id: number, data: any): Observable<any> {
      return this.http.put(`${environment.apiUrl}/employee-profile/${employeeId}/addresses/${id}`, data);
  }

  deleteAddress(employeeId: number, id: number): Observable<any> {
      return this.http.delete(`${environment.apiUrl}/employee-profile/${employeeId}/addresses/${id}`);
  }

  // ====================================================================================================
  // 8. Contracts
  // ====================================================================================================
  getContracts(employeeId: number): Observable<any> {
      return this.http.get<any>(`${environment.apiUrl}/employee-profile/${employeeId}/contracts`);
  }

  addContract(employeeId: number, data: any): Observable<any> {
      return this.http.post(`${environment.apiUrl}/employee-profile/${employeeId}/contracts`, data);
  }

  renewContract(employeeId: number, data: any): Observable<any> {
      return this.http.put(`${environment.apiUrl}/employee-profile/${employeeId}/contracts/renew`, data);
  }
}
