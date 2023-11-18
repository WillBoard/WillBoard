using System;
using System.Collections.Generic;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;

namespace WillBoard.Application.DataModels
{
    public class PostDataModel
    {
        public string BoardId { get; set; }
        public int PostId { get; set; }

        public int? ThreadId { get; set; }

        public DateTime Creation { get; set; }
        public DateTime CreationLocal { get; set; }

        public string Subject { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }

        public bool File { get; set; }
        public bool? FileSpoiler { get; set; }
        public bool? FileDeleted { get; set; }
        public string FileNameOriginal { get; set; }
        public string FileName { get; set; }
        public string FileMimeType { get; set; }
        public long? FileSize { get; set; }
        public int? FileWidth { get; set; }
        public int? FileHeight { get; set; }
        public double? FileDuration { get; set; }

        public bool FilePreview { get; set; }
        public string FilePreviewName { get; set; }
        public int? FilePreviewWidth { get; set; }
        public int? FilePreviewHeight { get; set; }

        public IpVersion? IPVersion { get; set; }
        public UInt128? IPNumber { get; set; }

        public string Country { get; set; }
        public string UserAgent { get; set; }

        public string UserId { get; set; }

        public bool Sage { get; set; }
        public bool? Pin { get; set; }
        public bool? ReplyLock { get; set; }
        public bool? BumpLock { get; set; }
        public DateTime? Excessive { get; set; }

        public bool? ForceUserId { get; set; }
        public bool? ForceCountry { get; set; }

        public int? ReplyCount { get; set; }

        public IEnumerable<PostMention> IncomingPostMentionCollection { get; set; }
        public IEnumerable<PostMention> OutcomingPostMentionCollection { get; set; }

        public PostDataModel(Post post, Core.Entities.Board board, bool administration = false)
        {
            BoardId = post.BoardId;
            PostId = post.PostId;
            ThreadId = post.ThreadId;

            Creation = post.Creation;
            CreationLocal = post.CreationLocal;

            Subject = post.Subject;
            Name = string.IsNullOrEmpty(post.Name) ? board.Anonymous : post.Name;
            Email = post.Email;
            Message = post.MessageDynamic;

            File = post.File;
            FileSpoiler = post.File ? post.FileSpoiler : null;
            FileDeleted = post.File ? post.FileDeleted : null;
            FileNameOriginal = post.File ? post.FileNameOriginal : null;
            FileName = post.File ? post.FileName : null;
            FileMimeType = post.File ? post.FileMimeType : null;
            FileSize = post.File ? post.FileSize : null;
            FileWidth = post.File ? post.FileWidth : null;
            FileHeight = post.File ? post.FileHeight : null;
            FileDuration = post.File ? post.FileDuration : null;

            FilePreview = post.FilePreview;
            FilePreviewName = post.FilePreview ? post.FilePreviewName : null;
            FilePreviewWidth = post.FilePreview ? post.FilePreviewWidth : null;
            FilePreviewHeight = post.FilePreview ? post.FilePreviewHeight : null;

            IPVersion = administration ? post.IpVersion : null;
            IPNumber = administration ? post.IpNumber : null;
            UserAgent = administration ? post.UserAgent : null;

            Country = board.CountryRequirement || post.ForceCountry ? post.Country : null;
            UserId = board.UserIdRequirement || post.ForceUserId ? post.UserId : null;

            Sage = post.Sage;
            Pin = post.IsThread() ? post.Pin : null;
            ReplyLock = post.IsThread() ? post.ReplyLock : null;
            BumpLock = post.IsThread() ? post.BumpLock : null;
            Excessive = post.IsThread() ? post.Excessive : null;

            ForceUserId = post.IsThread() ? post.ForceUserId : null;
            ForceCountry = post.IsThread() ? post.ForceCountry : null;

            ReplyCount = post.IsThread() ? post.ReplyCount : null;

            IncomingPostMentionCollection = post.IncomingPostMentionCollection;
            OutcomingPostMentionCollection = post.OutcomingPostMentionCollection;
        }
    }
}