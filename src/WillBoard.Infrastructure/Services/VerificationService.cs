using System;
using System.Linq;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Interfaces.Services;

namespace WillBoard.Infrastructure.Services
{
    public class VerificationService : IVerificationService
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IVerificationRepository _verificationRepository;
        private readonly IVerificationCache _verificationCache;
        private readonly IReCaptchaV2Service _reCaptchaV2Service;
        private readonly IClassicCaptchaService _classicCaptchaService;

        public VerificationService(IDateTimeProvider dateTimeProvider, IVerificationRepository verificationRepository, IVerificationCache verificationCache, IReCaptchaV2Service reCaptchaV2Service, IClassicCaptchaService classicCaptchaService)
        {
            _dateTimeProvider = dateTimeProvider;
            _verificationRepository = verificationRepository;
            _verificationCache = verificationCache;
            _reCaptchaV2Service = reCaptchaV2Service;
            _classicCaptchaService = classicCaptchaService;
        }

        public async Task<bool> CheckAsync(bool thread, IpVersion ipVersion, UInt128 ipNumber, Board board)
        {
            if (thread)
            {
                if (board.ThreadFieldVerificationMode == VerificationMode.Always)
                {
                    return true;
                }

                if (board.ThreadFieldVerificationMode == VerificationMode.Local || board.ThreadFieldVerificationMode == VerificationMode.Global)
                {
                    if (board.ThreadFieldVerificationMode == VerificationMode.Local)
                    {
                        var verificationCollection = await _verificationCache.GetBoardUnexpiredCollectionAsync(board.BoardId, ipVersion, ipNumber);

                        if (verificationCollection.Any(v => v.Expiration > _dateTimeProvider.UtcNow))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (board.ThreadFieldVerificationMode == VerificationMode.Global)
                    {
                        var verificationCollection = await _verificationCache.GetSystemUnexpiredCollectionAsync(ipVersion, ipNumber);

                        if (verificationCollection.Any(v => v.Expiration > _dateTimeProvider.UtcNow))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                if (board.ReplyFieldVerificationMode == VerificationMode.Always)
                {
                    return true;
                }

                if (board.ReplyFieldVerificationMode == VerificationMode.Local || board.ReplyFieldVerificationMode == VerificationMode.Global)
                {
                    if (board.ReplyFieldVerificationMode == VerificationMode.Local)
                    {
                        var verificationCollection = await _verificationCache.GetBoardUnexpiredCollectionAsync(board.BoardId, ipVersion, ipNumber);

                        if (verificationCollection.Any(v => v.Expiration > _dateTimeProvider.UtcNow))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (board.ReplyFieldVerificationMode == VerificationMode.Global)
                    {
                        var verificationCollection = await _verificationCache.GetSystemUnexpiredCollectionAsync(ipVersion, ipNumber);

                        if (verificationCollection.Any(v => v.Expiration > _dateTimeProvider.UtcNow))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public async Task<bool> VerifyAsync(bool thread, IpVersion ipVersion, UInt128 ipNumber, Board board, string key, string value)
        {
            if ((board.ReplyFieldVerificationMode == VerificationMode.None && !thread) || (board.ThreadFieldVerificationMode == VerificationMode.None && thread))
            {
                return true;
            }

            if ((board.ReplyFieldVerificationMode == VerificationMode.Always && !thread) || (board.ThreadFieldVerificationMode == VerificationMode.Always && thread))
            {
                var verifyTypeResult = await VerifyTypeAsync(board.FieldVerificationType, key, value, board.VerificationSecretKey);
                return verifyTypeResult;
            }
            else if ((board.ReplyFieldVerificationMode == VerificationMode.Local && !thread) ||
                (board.ReplyFieldVerificationMode == VerificationMode.Global && !thread) ||
                (board.ThreadFieldVerificationMode == VerificationMode.Local && thread) ||
                (board.ThreadFieldVerificationMode == VerificationMode.Global && thread))
            {
                if (!await CheckAsync(thread, ipVersion, ipNumber, board))
                {
                    return true;
                }

                var verifyTypeResult = await VerifyTypeAsync(board.FieldVerificationType, key, value, board.VerificationSecretKey);
                if (!verifyTypeResult)
                {
                    return false;
                }

                var verification = new Verification
                {
                    VerificationId = Guid.NewGuid(),
                    IpNumber = ipNumber,
                    IpVersion = ipVersion,
                    Creation = _dateTimeProvider.UtcNow
                };

                if (board.ReplyFieldVerificationMode == VerificationMode.Local || board.ThreadFieldVerificationMode == VerificationMode.Local)
                {
                    verification.BoardId = board.BoardId;
                    if (thread)
                    {
                        verification.Expiration = verification.Creation.AddSeconds(board.ThreadFieldVerificationLocalTime);
                    }
                    else
                    {
                        verification.Expiration = verification.Creation.AddSeconds(board.ReplyFieldVerificationLocalTime);
                    }
                }
                else
                {
                    verification.BoardId = null;
                    verification.Expiration = verification.Creation.AddSeconds(60 * 60 * 24);
                }

                await _verificationRepository.CreateAsync(verification);

                if (verification.BoardId == null)
                {
                    await _verificationCache.RemoveSystemUnexpiredCollectionAsync(verification.IpVersion, verification.IpNumber);
                }
                else
                {
                    await _verificationCache.RemoveBoardUnexpiredCollectionAsync(verification.BoardId, verification.IpVersion, verification.IpNumber);
                }

                return true;
            }

            return false;
        }

        private async Task<bool> VerifyTypeAsync(VerificationType verificationType, string key, string value, string verificationSecretKey)
        {
            if (verificationType == VerificationType.ReCaptcha)
            {
                return await _reCaptchaV2Service.VerifyAsync(verificationSecretKey, value);
            }
            else if (verificationType == VerificationType.ClassicCaptcha)
            {
                if (string.IsNullOrEmpty(key))
                {
                    return _classicCaptchaService.Verify(value, value);
                }
                else
                {
                    return _classicCaptchaService.Verify(key, value);
                }
            }
            else
            {
                return false;
            }
        }
    }
}