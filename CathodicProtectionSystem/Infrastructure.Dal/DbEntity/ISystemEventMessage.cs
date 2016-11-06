using System;

namespace Infrastructure.Dal.DbEntity
{
    public interface ISystemEventMessage
    {
        Category Category { get; set; }
        DateTime Created { get; set; }
        bool HasRead { get; set; }
        string Message { get; set; }
        int MessageId { get; set; }
        SystemEventCodes SystemEventCode { get; set; }
    }
}
