using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
using HRMS.Core.Entities.Core;

namespace HRMS.Infrastructure.Data
{
    public class AuditEntry
    {
        public EntityEntry Entry { get; }
        public string UserId { get; set; }
        public string TableName { get; set; }
        public string ActionType { get; set; }
        public string? IpAddress { get; set; }
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();

        public bool HasTemporaryProperties => TemporaryProperties.Any();

        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }

        public AuditLog ToAuditLog()
        {
            var audit = new AuditLog
            {
                TableName = TableName,
                ActionType = ActionType,
                PerformedBy = UserId,
                PerformedAt = DateTime.Now, // Use server time
                IpAddress = IpAddress,
                OldValue = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues),
                NewValue = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues)
            };
            
            // We'll try to get the PK, but it might be composite or part of KeyValues
            // For simplicity, we take the first key value or 0
            if (KeyValues.Count > 0)
            {
                // Assuming single PK for now, or just storing the JSON of keys could be an option
                // But AuditLog.RecordId is long. If PK is int/long, we use it.
                var pkValue = KeyValues.FirstOrDefault().Value;
                if (pkValue is int i) audit.RecordId = i;
                else if (pkValue is long l) audit.RecordId = l;
                // else: leave as 0 or handle logic for non-numeric keys if any
            }

            return audit;
        }
    }
}
