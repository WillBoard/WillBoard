using System;

namespace WillBoard.Core.Interfaces.Providers
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}