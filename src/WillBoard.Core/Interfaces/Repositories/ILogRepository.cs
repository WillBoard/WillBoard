using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Repositories
{
    public interface ILogRepository
    {
        void CreateLog(Log log);
        IEnumerable<Log> ReadLogCollection(DateTime dateTimeFrom, DateTime dateTimeTo);

        Task CreateLogAsync(Log log);
        Task<IEnumerable<Log>> ReadLogCollectionAsync(DateTime dateTimeFrom, DateTime dateTimeTo);
    }
}