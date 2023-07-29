using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.CacheClear
{
    public class CacheClearCommand : IRequest<Status<InternalError>>
    {
        public bool Account { get; set; }
        public bool Authentication { get; set; }
        public Guid? AuthenticationAccountId { get; set; }
        public bool Authorization { get; set; }
        public bool BadIp { get; set; }
        public bool BanAppeal { get; set; }
        public bool Ban { get; set; }
        public bool Board { get; set; }
        public bool Configuration { get; set; }
        public bool CountryIp { get; set; }
        public bool Invitation { get; set; }
        public bool Navigation { get; set; }
        public bool Post { get; set; }
        public bool Report { get; set; }
        public bool Translation { get; set; }
        public string TranslationLanguage { get; set; }
        public bool Verification { get; set; }
        public bool BlockList { get; set; }
    }
}