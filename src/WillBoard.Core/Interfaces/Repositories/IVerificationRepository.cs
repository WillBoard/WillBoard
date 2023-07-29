using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Interfaces.Repositories
{
    public interface IVerificationRepository
    {
        Task CreateAsync(Verification verification);

        Task<IEnumerable<Verification>> ReadSystemUnexpiredCollectionAsync(IpVersion ipVersion, BigInteger ipNumber);
        Task<IEnumerable<Verification>> ReadBoardUnexpiredCollectionAsync(string boardId, IpVersion ipVersion, BigInteger ipNumber);

        Task DeleteAsync(Guid verificationId);
    }
}