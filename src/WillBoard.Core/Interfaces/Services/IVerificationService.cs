using System;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Interfaces.Services
{
    public interface IVerificationService
    {
        Task<bool> CheckAsync(bool thread, IpVersion ipVersion, UInt128 ipNumber, Board board);
        Task<bool> VerifyAsync(bool thread, IpVersion ipVersion, UInt128 ipNumber, Board board, string key, string value);
    }
}