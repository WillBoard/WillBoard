using System;
using WillBoard.Core.Interfaces.Providers;

namespace WillBoard.Infrastructure.Providers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}