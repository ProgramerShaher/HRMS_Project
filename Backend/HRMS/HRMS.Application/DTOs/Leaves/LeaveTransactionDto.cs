using System;

namespace HRMS.Application.DTOs.Leaves
{
    public class LeaveTransactionDto
    {
        public int TransactionId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        
        /// <summary>
        /// نوع الحركة (ACCRUAL, DEDUCTION, ADJUSTMENT, etc.)
        /// </summary>
        public string TransactionType { get; set; } = string.Empty;
        
        /// <summary>
        /// عدد الأيام (موجب أو سالب)
        /// </summary>
        public decimal Days { get; set; }
        
        public DateTime TransactionDate { get; set; }
        public string? Notes { get; set; }
        public int? ReferenceId { get; set; }
    }
}
