using System;
using System.Collections.Generic;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Entities
{
    public class Post
    {
        public string BoardId { get; set; }
        public int PostId { get; set; }

        public int? ThreadId { get; set; }

        public DateTime Creation { get; set; }
        public DateTime CreationLocal { get; set; }

        public string Subject { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string MessageRaw { get; set; }
        public string MessageStatic { get; set; }
        public string MessageDynamic { get; set; }

        public bool File { get; set; }
        public bool FileSpoiler { get; set; }
        public bool FileDeleted { get; set; }
        public string FileNameOriginal { get; set; }
        public string FileName { get; set; }
        public string FileMimeType { get; set; }
        public byte[] FileHash { get; set; }
        public long FileSize { get; set; }
        public int FileWidth { get; set; }
        public int FileHeight { get; set; }
        public double FileDuration { get; set; }

        public bool FilePreview { get; set; }
        public string FilePreviewName { get; set; }
        public int FilePreviewWidth { get; set; }
        public int FilePreviewHeight { get; set; }

        public string Password { get; set; }

        public IpVersion IpVersion { get; set; }
        public UInt128 IpNumber { get; set; }

        public string Country { get; set; }
        public string UserAgent { get; set; }

        public string UserId { get; set; }

        public bool Sage { get; set; }
        public bool Pin { get; set; }
        public bool ReplyLock { get; set; }
        public bool BumpLock { get; set; }
        public DateTime? Excessive { get; set; }

        public bool ForceUserId { get; set; }
        public bool ForceCountry { get; set; }

        public DateTime Bump { get; set; }
        public int ReplyCount { get; set; }

        public IEnumerable<PostMention> IncomingPostMentionCollection { get; set; }
        public IEnumerable<PostMention> OutcomingPostMentionCollection { get; set; }

        public Post()
        {
            PostId = 0;
            ThreadId = null;
            BoardId = "";

            Creation = DateTime.UtcNow;

            Subject = "";
            Email = "";
            Name = "";
            MessageRaw = "";
            MessageStatic = "";
            MessageDynamic = "";

            File = false;
            FileSpoiler = false;
            FileDeleted = false;
            FileNameOriginal = "";
            FileName = "";
            FileMimeType = "";
            FileHash = new byte[0];
            FileSize = 0;
            FileWidth = 0;
            FileHeight = 0;
            FileDuration = 0;

            FilePreview = false;
            FilePreviewName = "";
            FilePreviewWidth = 0;
            FilePreviewHeight = 0;

            Password = "";

            IpVersion = 0;
            IpNumber = 0;

            Country = "";
            UserAgent = "";

            UserId = "";

            Sage = false;
            Pin = false;
            ReplyLock = false;
            BumpLock = false;
            Excessive = null;

            ForceUserId = false;
            ForceCountry = false;

            Bump = DateTime.UtcNow;
            ReplyCount = 0;

            IncomingPostMentionCollection = new List<PostMention>();
            OutcomingPostMentionCollection = new List<PostMention>();
        }

        public Post(Post post)
        {
            BoardId = post.BoardId;
            PostId = post.PostId;

            ThreadId = post.ThreadId;

            Creation = post.Creation;
            CreationLocal = post.CreationLocal;

            Subject = post.Subject;
            Email = post.Email;
            Name = post.Name;
            MessageRaw = post.MessageRaw;
            MessageStatic = post.MessageStatic;
            MessageDynamic = post.MessageDynamic;

            File = post.File;
            FileSpoiler = post.FileSpoiler;
            FileDeleted = post.FileDeleted;
            FileNameOriginal = post.FileNameOriginal;
            FileName = post.FileName;
            FileMimeType = post.FileMimeType;
            FileHash = post.FileHash;
            FileSize = post.FileSize;
            FileWidth = post.FileWidth;
            FileHeight = post.FileHeight;
            FileDuration = post.FileDuration;

            FilePreview = post.FilePreview;
            FilePreviewName = post.FilePreviewName;
            FilePreviewWidth = post.FilePreviewWidth;
            FilePreviewHeight = post.FilePreviewHeight;

            Password = post.Password;

            IpVersion = post.IpVersion;
            IpNumber = post.IpNumber;

            Country = post.Country;
            UserAgent = post.UserAgent;

            UserId = post.UserId;

            Sage = post.Sage;
            Pin = post.Pin;
            ReplyLock = post.ReplyLock;
            BumpLock = post.BumpLock;
            Excessive = post.Excessive;

            ForceUserId = post.ForceUserId;
            ForceCountry = post.ForceCountry;

            Bump = post.Bump;
            ReplyCount = post.ReplyCount;

            IncomingPostMentionCollection = post.IncomingPostMentionCollection;
            OutcomingPostMentionCollection = post.OutcomingPostMentionCollection;
        }

        public void Merge(Post post)
        {
            BoardId = post.BoardId;
            PostId = post.PostId;

            ThreadId = post.ThreadId;

            Creation = post.Creation;
            CreationLocal = post.CreationLocal;

            Subject = post.Subject;
            Email = post.Email;
            Name = post.Name;
            MessageRaw = post.MessageRaw;
            MessageStatic = post.MessageStatic;
            MessageDynamic = post.MessageDynamic;

            File = post.File;
            FileSpoiler = post.FileSpoiler;
            FileDeleted = post.FileDeleted;
            FileNameOriginal = post.FileNameOriginal;
            FileName = post.FileName;
            FileMimeType = post.FileMimeType;
            FileHash = post.FileHash;
            FileSize = post.FileSize;
            FileWidth = post.FileWidth;
            FileHeight = post.FileHeight;
            FileDuration = post.FileDuration;

            FilePreview = post.FilePreview;
            FilePreviewName = post.FilePreviewName;
            FilePreviewWidth = post.FilePreviewWidth;
            FilePreviewHeight = post.FilePreviewHeight;

            Password = post.Password;

            IpVersion = post.IpVersion;
            IpNumber = post.IpNumber;

            Country = post.Country;
            UserAgent = post.UserAgent;

            UserId = post.UserId;

            Sage = post.Sage;
            Pin = post.Pin;
            ReplyLock = post.ReplyLock;
            BumpLock = post.BumpLock;
            Excessive = post.Excessive;

            ForceUserId = post.ForceUserId;
            ForceCountry = post.ForceCountry;

            Bump = post.Bump;
            ReplyCount = post.ReplyCount;

            IncomingPostMentionCollection = post.IncomingPostMentionCollection;
            OutcomingPostMentionCollection = post.OutcomingPostMentionCollection;
        }

        public bool IsThread()
        {
            return ThreadId == null;
        }

        public bool IsReply()
        {
            return ThreadId != null;
        }
    }
}