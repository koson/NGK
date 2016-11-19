using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Dal.DbEntity;

namespace Infrastructure.Api.Services
{
    public interface ISystemEventLogService
    {
        void AddEvent(SystemEventCodes eventCode, Category category, string message, DateTime created);
    }
}