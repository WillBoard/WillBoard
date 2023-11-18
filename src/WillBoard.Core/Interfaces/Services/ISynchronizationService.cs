using System;
using System.Collections.Generic;
using WillBoard.Core.Classes;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Interfaces.Services
{
    public interface ISynchronizationService
    {
        void Notify<T>(T synchronizationMessage, params string[] synchronizationNameCollection) where T : class;
        SynchronizationSubscription<T> Subscribe<T>(IpVersion iPVersion, UInt128 ipNumber, params string[] synchronizationNameCollection) where T : class;
        void Unsubscribe(SynchronizationSubscription synchronizationSubscription);
        IDictionary<SynchronizationSubscription, string[]> GetSynchronizationSubscriptionCollection();
    }
}