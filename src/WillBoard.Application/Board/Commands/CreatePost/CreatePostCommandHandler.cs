using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Application.DataModels;
using WillBoard.Core.Classes;
using WillBoard.Core.Consts;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Managers;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;
using WillBoard.Core.Services;
using WillBoard.Core.Utilities;

namespace WillBoard.Application.Board.Commands.CreatePost
{
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, Result<CreatePostDataModel, InternalError>>
    {
        private const string RegexLine = @"\n";
        private const string RegexMention = @"\>\>([1-9][0-9]*)";
        private const string RegexLink = @"\<a(.)*?\>(.)*?\<\/a\>";

        private readonly ILogger _logger;
        private readonly IpManager _ipManager;
        private readonly BoardManager _boardManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPostRepository _postRepository;
        private readonly IBanCache _banCache;
        private readonly IPostCache _postCache;
        private readonly IPostService _postService;
        private readonly ILockManager _lockManager;
        private readonly IIpService _ipService;
        private readonly IStorageService _storageService;
        private readonly IFileService _fileService;
        private readonly IVerificationService _verificationService;
        private readonly ISynchronizationService _synchronizationService;
        private readonly MarkupService _markupService;

        public CreatePostCommandHandler(
            ILogger<CreatePostCommandHandler> logger,
            IpManager ipManager,
            BoardManager boardManager,
            IDateTimeProvider dateTimeProvider,
            IPostRepository postRepository,
            IBanCache banCache,
            IPostCache postCache,
            IPostService postService,
            ILockManager lockManager,
            IIpService ipService,
            IStorageService storageService,
            IFileService fileService,
            IVerificationService verificationService,
            ISynchronizationService synchronizationService,
            MarkupService markupService)
        {
            _logger = logger;
            _ipManager = ipManager;
            _boardManager = boardManager;
            _dateTimeProvider = dateTimeProvider;
            _postRepository = postRepository;
            _banCache = banCache;
            _postCache = postCache;
            _postService = postService;
            _lockManager = lockManager;
            _ipService = ipService;
            _storageService = storageService;
            _fileService = fileService;
            _verificationService = verificationService;
            _synchronizationService = synchronizationService;
            _markupService = markupService;
        }

        public async Task<Result<CreatePostDataModel, InternalError>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            var post = new Post();

            try
            {
                var board = _boardManager.GetBoard();

                if (!board.PostAvailability)
                {
                    return Result<CreatePostDataModel, InternalError>.ErrorResult(new InternalError(400, TranslationKey.ErrorPostAvailability));
                }

                post.BoardId = board.BoardId;
                post.ThreadId = request.ThreadId;
                post.IpVersion = _ipManager.GetIpVersion();
                post.IpNumber = _ipManager.GetIpNumber();
                post.UserAgent = request.UserAgent;

                post.Country = await _ipService.GetCountryIpAsync(post.IpVersion, post.IpNumber);

                if (post.IpVersion == IpVersion.IpVersion4)
                {
                    if (board.CountryIpVersion4BlockAvailability)
                    {
                        if (board.CountryIpVersion4BlockCollection.Any(x => x == post.Country))
                        {
                            if (!board.CountryIpVersion4ExclusionIpNumberCollection.Any(x => x == post.IpNumber))
                            {
                                return Result<CreatePostDataModel, InternalError>.ErrorResult(new InternalError(400, TranslationKey.ErrorCountryIpVersion4Block, post.Country));
                            }
                        }
                    }
                    if (board.BadIpVersion4BlockAvailability)
                    {
                        if (!board.BadIpVersion4BlockExclusionIpNumberCollection.Any(x => x == post.IpNumber))
                        {
                            if (await _ipService.GetBadIpAsync(post.IpVersion, post.IpNumber))
                            {
                                return Result<CreatePostDataModel, InternalError>.ErrorResult(new InternalError(400, TranslationKey.ErrorBadIpVersion4Block));
                            }
                        }
                    }
                    if (board.DnsBlockListIpVersion4Availability)
                    {
                        if (!board.DnsBlockListIpVersion4ExclusionIpNumberCollection.Any(x => x == post.IpNumber))
                        {
                            if (await _ipService.GetDnsBlockListIpVersion4Async(board.BoardId, (uint)post.IpNumber, board.DnsBlockListIpVersion4Collection, board.DnsBlockListIpVersion4Cache))
                            {
                                return Result<CreatePostDataModel, InternalError>.ErrorResult(new InternalError(400, TranslationKey.ErrorDnsBlockListIpVersion4));
                            }
                        }
                    }
                    if (board.ApiBlockListIpVersion4Availability)
                    {
                        if (!board.ApiBlockListIpVersion4ExclusionIpNumberCollection.Any(x => x == post.IpNumber))
                        {
                            if (await _ipService.GetApiBlockListIpVersion4Async(board.BoardId, (uint)post.IpNumber, board.ApiBlockListIpVersion4Collection, board.ApiBlockListIpVersion4Cache))
                            {
                                return Result<CreatePostDataModel, InternalError>.ErrorResult(new InternalError(400, TranslationKey.ErrorApiBlockListIpVersion4));
                            }
                        }
                    }
                }

                if (post.IpVersion == IpVersion.IpVersion6)
                {
                    if (board.CountryIpVersion6BlockAvailability)
                    {
                        if (board.CountryIpVersion6BlockCollection.Any(x => x == post.Country))
                        {
                            if (!board.CountryIpVersion6ExclusionIpNumberCollection.Any(x => x == post.IpNumber))
                            {
                                return Result<CreatePostDataModel, InternalError>.ErrorResult(new InternalError(400, TranslationKey.ErrorCountryIpVersion6Block, post.Country));
                            }
                        }
                    }
                    if (board.BadIpVersion6BlockAvailability)
                    {
                        if (!board.BadIpVersion6BlockExclusionIpNumberCollection.Any(x => x == post.IpNumber))
                        {
                            if (await _ipService.GetBadIpAsync(post.IpVersion, post.IpNumber))
                            {
                                return Result<CreatePostDataModel, InternalError>.ErrorResult(new InternalError(400, TranslationKey.ErrorBadIpVersion6Block));
                            }
                        }
                    }
                    if (board.DnsBlockListIpVersion6Availability)
                    {
                        if (!board.DnsBlockListIpVersion6ExclusionIpNumberCollection.Any(x => x == post.IpNumber))
                        {
                            if (await _ipService.GetDnsBlockListIpVersion6Async(board.BoardId, post.IpNumber, board.DnsBlockListIpVersion6Collection, board.DnsBlockListIpVersion6Cache))
                            {
                                return Result<CreatePostDataModel, InternalError>.ErrorResult(new InternalError(400, TranslationKey.ErrorDnsBlockListIpVersion6));
                            }
                        }
                    }
                    if (board.ApiBlockListIpVersion6Availability)
                    {
                        if (!board.ApiBlockListIpVersion6ExclusionIpNumberCollection.Any(x => x == post.IpNumber))
                        {
                            if (await _ipService.GetApiBlockListIpVersion6Async(board.BoardId, post.IpNumber, board.ApiBlockListIpVersion6Collection, board.ApiBlockListIpVersion6Cache))
                            {
                                return Result<CreatePostDataModel, InternalError>.ErrorResult(new InternalError(400, TranslationKey.ErrorApiBlockListIpVersion6));
                            }
                        }
                    }
                }

                var globalBanCollection = await _banCache.GetSystemUnexpiredCollectionAsync(post.IpVersion, post.IpNumber);
                if (globalBanCollection.Any(b => (b.Expiration == null || b.Expiration > _dateTimeProvider.UtcNow) && !b.ExclusionIpNumberCollection.Contains(post.IpNumber)))
                {
                    return Result<CreatePostDataModel, InternalError>.ErrorResult(new InternalError(400, TranslationKey.ErrorBan));
                }

                var localBanCollection = await _banCache.GetBoardUnexpiredCollectionAsync(board.BoardId, post.IpVersion, post.IpNumber);
                if (localBanCollection.Any(b => (b.Expiration == null || b.Expiration > _dateTimeProvider.UtcNow) && !b.ExclusionIpNumberCollection.Contains(post.IpNumber)))
                {
                    return Result<CreatePostDataModel, InternalError>.ErrorResult(new InternalError(400, TranslationKey.ErrorBan));
                }

                if (!await _verificationService.VerifyAsync(post.IsThread(), post.IpVersion, post.IpNumber, board, request.VerificationKey, request.VerificationValue))
                {
                    return Result<CreatePostDataModel, InternalError>.ErrorResult(new InternalError(400, TranslationKey.ErrorVerification));
                }

                if (post.IsThread())
                {
                    var createResult = await CreateThread(request, board, post);

                    if (!createResult.Success)
                    {
                        if (post.File)
                        {
                            _storageService.DeletePreviewFile(post.BoardId, post.FilePreviewName);
                            _storageService.DeleteSourceFile(post.BoardId, post.FileName);
                        }

                        return Result<CreatePostDataModel, InternalError>.ErrorResult(createResult.Error);
                    }
                }
                else
                {
                    var createResult = await CreateReply(request, board, post);

                    if (!createResult.Success)
                    {
                        if (post.File)
                        {
                            _storageService.DeletePreviewFile(post.BoardId, post.FilePreviewName);
                            _storageService.DeleteSourceFile(post.BoardId, post.FileName);
                        }

                        return Result<CreatePostDataModel, InternalError>.ErrorResult(createResult.Error);
                    }
                }

                _postService.Adapt(board, post);
                await _postCache.AddAdaptedAsync(board, post);

                var synchronizationMessage = new SynchronizationMessage()
                {
                    Event = SynchronizationEvent.Create,
                    Data = new PostDataModel(post, board, false)
                };

                var administrationSynchronizationMessage = new AdministrationSynchronizationMessage()
                {
                    Event = SynchronizationEvent.Create,
                    Data = new PostDataModel(post, board, true)
                };

                _synchronizationService.Notify(synchronizationMessage, board.BoardId);
                _synchronizationService.Notify(administrationSynchronizationMessage, board.BoardId);

                var postDto = new CreatePostDataModel()
                {
                    BoardId = post.BoardId,
                    PostId = post.PostId,
                    ThreadId = post.ThreadId
                };

                return Result<CreatePostDataModel, InternalError>.ValueResult(postDto);
            }
            catch (Exception exception)
            {
                if (post.File)
                {
                    _storageService.DeletePreviewFile(post.BoardId, post.FilePreviewName);
                    _storageService.DeleteSourceFile(post.BoardId, post.FileName);
                }

                _logger.LogCritical(exception, $"Exception occurred during {0}.", nameof(CreatePostCommandHandler));
                return Result<CreatePostDataModel, InternalError>.ErrorResult(new InternalError(400, TranslationKey.ErrorCreatePost));
            }
        }

        private async Task<Status<InternalError>> CreateThread(CreatePostCommand request, Core.Entities.Board board, Post post)
        {
            if (!board.ThreadFieldNameAvailability)
            {
                if (!string.IsNullOrEmpty(request.Name))
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldNameAvailability));
                }
            }
            else
            {
                if (string.IsNullOrEmpty(request.Name))
                {
                    if (board.ThreadFieldNameRequirement)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldNameRequirement));
                    }
                }
                else
                {
                    if (request.Name.Length < board.ThreadFieldNameLengthMin)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldNameLengthMin));
                    }
                    if (request.Name.Length > board.ThreadFieldNameLengthMax)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldNameLengthMax));
                    }
                }
            }

            if (!board.ThreadFieldEmailAvailability)
            {
                if (!string.IsNullOrEmpty(request.Email))
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldEmailAvailability));
                }
            }
            else
            {
                if (string.IsNullOrEmpty(request.Email))
                {
                    if (board.ThreadFieldEmailRequirement)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldEmailRequirement));
                    }
                }
                else
                {
                    if (request.Email.Length < board.ThreadFieldEmailLengthMin)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldEmailLengthMin));
                    }
                    if (request.Email.Length > board.ThreadFieldEmailLengthMax)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldEmailLengthMax));
                    }
                }
            }

            if (!board.ThreadFieldSubjectAvailability)
            {
                if (!string.IsNullOrEmpty(request.Subject))
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldSubjectAvailability));
                }
            }
            else
            {
                if (string.IsNullOrEmpty(request.Subject))
                {
                    if (board.ThreadFieldSubjectRequirement)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldSubjectRequirement));
                    }
                }
                else
                {
                    if (request.Subject.Length < board.ThreadFieldSubjectLengthMin)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldSubjectLengthMin));
                    }
                    if (request.Subject.Length > board.ThreadFieldSubjectLengthMax)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldSubjectLengthMax));
                    }
                }
            }

            if (!board.ThreadFieldPasswordAvailability)
            {
                if (!string.IsNullOrEmpty(request.Password))
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldPasswordAvailability));
                }
            }
            else
            {
                if (string.IsNullOrEmpty(request.Password))
                {
                    if (board.ThreadFieldPasswordRequirement)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldPasswordRequirement));
                    }
                }
                else
                {
                    if (request.Password.Length < board.ThreadFieldPasswordLengthMin)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldPasswordLengthMin));
                    }
                    if (request.Password.Length > board.ThreadFieldPasswordLengthMax)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldPasswordLengthMax));
                    }
                }
            }

            if (!board.ThreadFieldMessageAvailability)
            {
                if (!string.IsNullOrEmpty(request.Message))
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldMessageAvailability));
                }
            }
            else
            {
                if (string.IsNullOrEmpty(request.Message))
                {
                    if (board.ThreadFieldMessageRequirement)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldMessageRequirement));
                    }
                }
                else
                {
                    if (request.Message.Length < board.ThreadFieldMessageLengthMin)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldMessageLengthMin));
                    }
                    if (request.Message.Length > board.ThreadFieldMessageLengthMax)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldMessageLengthMax));
                    }

                    var lineMatchCollection = Regex.Matches(request.Message, RegexLine);
                    if (lineMatchCollection.Count > board.ThreadFieldMessageLineMax)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldMessageLinesMax));
                    }
                }
            }

            if (!board.ThreadFieldFileAvailability)
            {
                if (request.File != null)
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldFileAvailability));
                }
            }
            else
            {
                if (request.File == null)
                {
                    if (board.ThreadFieldFileRequirement)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldFileRequirement));
                    }
                }
                else
                {
                    if (request.File.Length < board.ThreadFieldFileSizeMin)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldFileSizeMin));
                    }
                    if (request.File.Length > board.ThreadFieldFileSizeMax)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldFileSizeMax));
                    }
                    if (request.File.FileName.Length < 1)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldFileNameLengthMin));
                    }
                    if (request.File.FileName.Length > 255)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldFileNameLengthMax));
                    }

                    post.FileMimeType = _fileService.GetMimeType(request.File);
                    if (!board.ThreadFieldFileTypeCollection.Contains(post.FileMimeType))
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldFileType));
                    }
                }
            }

            post.Creation = _dateTimeProvider.UtcNow;
            post.Bump = post.Creation;

            var postLastest = await _postCache.GetAdaptedThreadLastByIpNumberAsync(board, post.IpVersion, post.IpNumber);
            if (postLastest != null)
            {
                var threadTimeMin = postLastest.Creation.AddSeconds(board.ThreadTimeMin);
                if (post.Creation < threadTimeMin)
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadTimeMin));
                }
            }

            post.Name = request.Name;
            post.Email = request.Email;
            post.Subject = request.Subject;
            post.MessageStatic = _markupService.MarkupStaticCustomEncode(_markupService.MarkupStaticEncode(request.Message), board.MarkupStaticCustomCollection);
            post.MessageRaw = request.Message;
            post.Password = request.Password;

            post.UserId = IdentifierUtility.Generate(10);

            var linkMatchCollection = Regex.Matches(post.MessageStatic, RegexLink);
            if (linkMatchCollection.Count > board.ThreadFieldMessageLinkMax)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldMessageLinkMax));
            }

            var boardMentionMatchCollection = Regex.Matches(post.MessageStatic, RegexMention);
            if (boardMentionMatchCollection.Count > board.ThreadFieldMessageMentionMax)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldMessageMentionMax));
            }

            GenerateOutcomingMentionCollection(post);

            if (board.ThreadFieldOptionSpoilerAvailability && request.OptionSpoiler)
            {
                post.FileSpoiler = true;
            }

            if (board.ThreadFieldOptionSageAvailability && request.OptionSage || post.Email == "sage")
            {
                post.Sage = true;
                post.Email = "sage";
            }

            if (board.ThreadFieldOptionUserIdAvailability && request.OptionUserId)
            {
                post.ForceUserId = true;
            }

            if (board.ThreadFieldOptionCountryAvailability && request.OptionCountry)
            {
                post.ForceCountry = true;
            }

            if (request.File != null)
            {
                post.FileHash = _fileService.GetMD5(request.File);
                if (board.ThreadFieldFileOriginality)
                {
                    var postWithFileHash = await _postCache.GetAdaptedByFileHashAsync(board, post.FileHash);
                    if (postWithFileHash != null)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldFileOriginality));
                    }
                }

                var resultFile = await _fileService.AddFileAsync(board, post, request.File);

                if (!resultFile.Success)
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, resultFile.Error));
                }
            }

            using (await _lockManager.GetPostLockAsync($"Database_{board.BoardId}"))
            {
                post.Creation = _dateTimeProvider.UtcNow;
                post.Bump = post.Creation;

                var postLastestLock = await _postRepository.ReadThreadLastByIpAsync(board.BoardId, post.IpVersion, post.IpNumber);
                if (postLastestLock != null)
                {
                    var threadTimeMin = postLastestLock.Creation.AddSeconds(board.ThreadTimeMin);
                    if (post.Creation < threadTimeMin)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadTimeMin));
                    }
                }

                if (board.ThreadFieldFileOriginality && request.File != null)
                {
                    var postWithFileHashLock = await _postRepository.ReadByFileHashAsync(board.BoardId, post.FileHash);
                    if (postWithFileHashLock != null)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFieldFileOriginality));
                    }
                }

                await _postService.CreateAsync(post);

                var unmarkedExcessiveThreadCollection = await _postRepository.ReadUnmarkedExcessiveThreadCollectionAsync(board.BoardId, board.PageMax * board.PageThreadMax);

                var excessive = _dateTimeProvider.UtcNow;

                foreach (var unmarkedExcessiveThread in unmarkedExcessiveThreadCollection)
                {
                    await _postRepository.UpdateExcessiveAsync(unmarkedExcessiveThread.BoardId, unmarkedExcessiveThread.PostId, excessive);
                    await _postCache.UpdateAdaptedExcessiveAsync(board, unmarkedExcessiveThread.PostId, excessive);
                }
            }

            return Status<InternalError>.SuccessStatus();
        }

        private async Task<Status<InternalError>> CreateReply(CreatePostCommand request, Core.Entities.Board board, Post post)
        {
            if (!board.ReplyFieldNameAvailability)
            {
                if (!string.IsNullOrEmpty(request.Name))
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldNameAvailability));
                }
            }
            else
            {
                if (string.IsNullOrEmpty(request.Name))
                {
                    if (board.ReplyFieldNameRequirement)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldNameRequirement));
                    }
                }
                else
                {
                    if (request.Name.Length < board.ReplyFieldNameLengthMin)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldNameLengthMin));
                    }
                    if (request.Name.Length > board.ReplyFieldNameLengthMax)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldNameLengthMax));
                    }
                }
            }

            if (!board.ReplyFieldEmailAvailability)
            {
                if (!string.IsNullOrEmpty(request.Email))
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldEmailAvailability));
                }
            }
            else
            {
                if (string.IsNullOrEmpty(request.Email))
                {
                    if (board.ReplyFieldEmailRequirement)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldEmailRequirement));
                    }
                }
                else
                {
                    if (request.Email.Length < board.ReplyFieldEmailLengthMin)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldEmailLengthMin));
                    }
                    if (request.Email.Length > board.ReplyFieldEmailLengthMax)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldEmailLengthMax));
                    }
                }
            }

            if (!board.ReplyFieldSubjectAvailability)
            {
                if (!string.IsNullOrEmpty(request.Subject))
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldSubjectAvailability));
                }
            }
            else
            {
                if (string.IsNullOrEmpty(request.Subject))
                {
                    if (board.ReplyFieldSubjectRequirement)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldSubjectRequirement));
                    }
                }
                else
                {
                    if (request.Subject.Length < board.ReplyFieldSubjectLengthMin)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldSubjectLengthMin));
                    }
                    if (request.Subject.Length > board.ReplyFieldSubjectLengthMax)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldSubjectLengthMax));
                    }
                }
            }

            if (!board.ReplyFieldPasswordAvailability)
            {
                if (!string.IsNullOrEmpty(request.Password))
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldPasswordAvailability));
                }
            }
            else
            {
                if (string.IsNullOrEmpty(request.Password))
                {
                    if (board.ReplyFieldPasswordRequirement)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldPasswordRequirement));
                    }
                }
                else
                {
                    if (request.Password.Length < board.ReplyFieldPasswordLengthMin)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldPasswordLengthMin));
                    }
                    if (request.Password.Length > board.ReplyFieldPasswordLengthMax)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldPasswordLengthMax));
                    }
                }
            }

            if (!board.ReplyFieldMessageAvailability)
            {
                if (!string.IsNullOrEmpty(request.Message))
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldMessageAvailability));
                }
            }
            else
            {
                if (string.IsNullOrEmpty(request.Message))
                {
                    if (board.ReplyFieldMessageRequirement)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldMessageRequirement));
                    }
                }
                else
                {
                    if (request.Message.Length < board.ReplyFieldMessageLengthMin)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldMessageLengthMin));
                    }
                    if (request.Message.Length > board.ReplyFieldMessageLengthMax)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldMessageLengthMax));
                    }

                    var lineMatchCollection = Regex.Matches(request.Message, RegexLine);
                    if (lineMatchCollection.Count > board.ReplyFieldMessageLineMax)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldMessageLinesMax));
                    }
                }
            }

            if (!board.ReplyFieldFileAvailability)
            {
                if (request.File != null)
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldFileAvailability));
                }
            }
            else
            {
                if (request.File == null)
                {
                    if (board.ReplyFieldFileRequirement)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldFileRequirement));
                    }
                }
                else
                {
                    if (request.File.Length < board.ReplyFieldFileSizeMin)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldFileSizeMin));
                    }
                    if (request.File.Length > board.ReplyFieldFileSizeMax)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldFileSizeMax));
                    }
                    if (request.File.FileName.Length < 1)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldFileNameLengthMin));
                    }
                    if (request.File.FileName.Length > 255)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldFileNameLengthMax));
                    }

                    post.FileMimeType = _fileService.GetMimeType(request.File);
                    if (!board.ReplyFieldFileTypeCollection.Contains(post.FileMimeType))
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldFileType));
                    }
                }
            }

            post.Creation = _dateTimeProvider.UtcNow;

            var adaptedThread = await _postCache.GetAdaptedAsync(board, request.ThreadId.Value);

            if (adaptedThread == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyNotFoundThread));
            }

            if (!adaptedThread.IsThread())
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyNotFoundThread));
            }

            if (adaptedThread.ReplyLock)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyThreadReplyLock));
            }

            if (adaptedThread.Excessive != null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyThreadExcessive));
            }

            var adaptedReplyLast = await _postCache.GetAdaptedReplyLastByIpNumberAsync(board, post.IpVersion, post.IpNumber);
            if (adaptedReplyLast != null)
            {
                var replyTimeMin = adaptedReplyLast.Creation.AddSeconds(board.ReplyTimeMin);
                if (post.Creation < replyTimeMin)
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyTimeMin));
                }
            }

            post.Name = request.Name;
            post.Email = request.Email;
            post.Subject = request.Subject;
            post.MessageStatic = _markupService.MarkupStaticCustomEncode(_markupService.MarkupStaticEncode(request.Message), board.MarkupStaticCustomCollection);
            post.MessageRaw = request.Message;
            post.Password = request.Password;

            var linkMatchCollection = Regex.Matches(post.MessageStatic, RegexLink);
            if (linkMatchCollection.Count > board.ReplyFieldMessageLinkMax)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldMessageLinkMax));
            }

            var boardMentionMatchCollection = Regex.Matches(post.MessageStatic, RegexMention);
            if (boardMentionMatchCollection.Count > board.ReplyFieldMessageMentionMax)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldMessageMentionMax));
            }

            GenerateOutcomingMentionCollection(post);

            if (board.ReplyFieldOptionSpoilerAvailability && request.OptionSpoiler)
            {
                post.FileSpoiler = true;
            }

            if (board.ReplyFieldOptionSageAvailability && request.OptionSage || post.Email == "sage")
            {
                post.Sage = true;
                post.Email = "sage";
            }

            if (request.File != null)
            {
                post.FileHash = _fileService.GetMD5(request.File);
                if (board.ReplyFieldFileOriginality)
                {
                    var postWithFileHash = await _postCache.GetAdaptedByFileHashAsync(board, post.FileHash);
                    if (postWithFileHash != null)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldFileOriginality));
                    }
                }

                var resultFile = await _fileService.AddFileAsync(board, post, request.File);
                if (!resultFile.Success)
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, resultFile.Error));
                }
            }

            using (await _lockManager.GetPostLockAsync($"Database_{board.BoardId}"))
            {
                post.Creation = _dateTimeProvider.UtcNow;

                var thread = await _postRepository.ReadAsync(board.BoardId, request.ThreadId.Value);

                if (thread == null)
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyNotFoundThread));
                }

                if (!thread.IsThread())
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyNotFoundThread));
                }

                if (thread.ReplyLock)
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyThreadReplyLock));
                }

                if (thread.Excessive != null)
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyThreadExcessive));
                }

                var replyLast = await _postRepository.ReadReplyLastByIpAsync(board.BoardId, post.IpVersion, post.IpNumber);
                if (replyLast != null)
                {
                    var replyTimeMin = replyLast.Creation.AddSeconds(board.ReplyTimeMin);
                    if (post.Creation < replyTimeMin)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyTimeMin));
                    }
                }

                if (board.ReplyFieldFileOriginality && request.File != null)
                {
                    var postWithFileHash = await _postRepository.ReadByFileHashAsync(board.BoardId, post.FileHash);
                    if (postWithFileHash != null)
                    {
                        return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFieldFileOriginality));
                    }
                }

                if (thread.IpVersion == post.IpVersion && thread.IpNumber == post.IpNumber)
                {
                    post.UserId = thread.UserId;
                }
                else if (replyLast != null && replyLast.ThreadId == thread.PostId)
                {
                    post.UserId = replyLast.UserId;
                }
                else
                {
                    var postUserId = await _postCache.GetAdaptedReplyUserIdAsync(board, request.ThreadId.Value, post.IpVersion, post.IpNumber);

                    if (postUserId == null)
                    {
                        post.UserId = IdentifierUtility.Generate(10);
                    }
                    else
                    {
                        post.UserId = postUserId.UserId;
                    }
                }

                if (thread.BumpLock || thread.ReplyCount >= board.ThreadBumpLockReplyMax || thread.Creation.AddSeconds(board.ThreadBumpLockTimeMax) < post.Creation)
                {
                    post.Sage = true;
                }

                if (thread.ForceUserId)
                {
                    post.ForceUserId = true;
                }

                if (thread.ForceCountry)
                {
                    post.ForceCountry = true;
                }

                await _postService.CreateAsync(post);
            }

            return Status<InternalError>.SuccessStatus();
        }

        private static void GenerateOutcomingMentionCollection(Post post)
        {
            var mentionCollection = new List<PostMention>();
            var boardMentionMatcheCollection = Regex.Matches(post.MessageStatic, RegexMention);
            foreach (Match match in boardMentionMatcheCollection)
            {
                if (int.TryParse(match.Groups[1].Value, out int quotelinkPostId))
                {
                    var mention = new PostMention()
                    {
                        IncomingBoardId = post.BoardId,
                        IncomingPostId = quotelinkPostId
                    };

                    if (!mentionCollection.Any(e => e.IncomingBoardId == mention.IncomingBoardId && e.IncomingPostId == mention.IncomingPostId))
                    {
                        mentionCollection.Add(mention);
                    }
                }
            }
            post.OutcomingPostMentionCollection = mentionCollection;
        }
    }
}