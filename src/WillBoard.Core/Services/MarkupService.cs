using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using WillBoard.Core.Classes;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Services
{
    public class MarkupService
    {
        public MarkupService()
        {
        }

        private string MarkupStaticEscape(string text)
        {
            text = Regex.Replace(text, @"(\`|\*|\/|\-|_|\#)", @"\$1");
            text = Regex.Replace(text, @"(\&gt\;)", @"\&gt;");
            return text;
        }

        public string MarkupStaticEncode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            input = WebUtility.HtmlEncode(input);

            // <code>
            input = Regex.Replace(input, @"(?<!\\)\`\`(.+?)\`\`", match =>
            {
                return $@"<code>{MarkupStaticEscape(match.Groups[1].Value)}</code>";
            }, RegexOptions.Singleline | RegexOptions.Compiled);

            // <a href=""></a>
            input = Regex.Replace(input, @"(([a-zA-Z]+)(:\/\/(\.)?)[^\s\.]+\.[^\s]{2,})", match =>
            {
                string address = MarkupStaticEscape(match.Groups[1].Value);
                return $@"<a href='{address}' rel='noopener noreferrer' target='_blank'>{address}</a>";
            }, RegexOptions.Singleline | RegexOptions.Compiled);

            // <strong> <em> <s> <u> <mark>
            input = Regex.Replace(input, @"(?<!\\)\*\*((?:(?!<code>|<\/code>).)+?)\*\*", "<strong>$1</strong>", RegexOptions.Singleline | RegexOptions.Compiled);
            input = Regex.Replace(input, @"(?<!\\)\/\/((?:(?!<code>|<\/code>).)+?)\/\/", "<em>$1</em>", RegexOptions.Singleline | RegexOptions.Compiled);
            input = Regex.Replace(input, @"(?<!\\)\-\-((?:(?!<code>|<\/code>).)+?)\-\-", "<s>$1</s>", RegexOptions.Singleline | RegexOptions.Compiled);
            input = Regex.Replace(input, @"(?<!\\)__((?:(?!<code>|<\/code>).)+?)__", "<u>$1</u>", RegexOptions.Singleline | RegexOptions.Compiled);
            input = Regex.Replace(input, @"(?<!\\)\#\#((?:(?!<code>|<\/code>).)+?)\#\#", "<mark>$1</mark>", RegexOptions.Singleline | RegexOptions.Compiled);

            // >>
            input = Regex.Replace(input, @"(?<!\\)\&gt\;\&gt\;([1-9][0-9]*)(\<br\>|\n)?", match =>
            {
                return $@">>{match.Groups[1].Value}";
            }, RegexOptions.Singleline | RegexOptions.Compiled);

            // >
            input = Regex.Replace(input, @"(^\&gt\;(.+))", "<blockquote>&gt;$2</blockquote>", RegexOptions.Multiline | RegexOptions.Compiled);

            // safe escape ` * / - _ #
            input = Regex.Replace(input, @"\\(\`)", "$1", RegexOptions.Singleline | RegexOptions.Compiled);
            input = Regex.Replace(input, @"\\(\*)", "$1", RegexOptions.Singleline | RegexOptions.Compiled);
            input = Regex.Replace(input, @"\\(\/)", "$1", RegexOptions.Singleline | RegexOptions.Compiled);
            input = Regex.Replace(input, @"\\(\-)", "$1", RegexOptions.Singleline | RegexOptions.Compiled);
            input = Regex.Replace(input, @"\\(_)", "$1", RegexOptions.Singleline | RegexOptions.Compiled);
            input = Regex.Replace(input, @"\\(\#)", "$1", RegexOptions.Singleline | RegexOptions.Compiled);
            input = Regex.Replace(input, @"\\(\&gt\;)", "&gt;", RegexOptions.Singleline | RegexOptions.Compiled);

            // new line
            input = Regex.Replace(input, @"(\n)", "<br>", RegexOptions.Singleline | RegexOptions.Compiled);
            input = Regex.Replace(input, @"(\r)", "", RegexOptions.Singleline | RegexOptions.Compiled);

            return input;
        }

        public string MarkupDynamicEncode(string boardId, int postId, int? threadId, string input, IEnumerable<PostMention> outcomingPostMentionCollection)
        {
            input = Regex.Replace(input, @"\>\>([1-9][0-9]*)", match =>
            {
                if (int.TryParse(match.Groups[1].Value, out int outcomingPostMentionPostId))
                {
                    var outcomingPostMention = outcomingPostMentionCollection.FirstOrDefault(e => e.IncomingBoardId == boardId && e.IncomingPostId == outcomingPostMentionPostId && e.Active);
                    if (outcomingPostMention != null)
                    {
                        return $@"<a class=""outcoming-post-mention"" data-boardid=""{outcomingPostMention.IncomingBoardId}"" data-postid=""{outcomingPostMention.IncomingPostId}"" href=""/{outcomingPostMention.IncomingBoardId}/thread/{(outcomingPostMention.IncomingThreadId == null ? outcomingPostMention.IncomingPostId : outcomingPostMention.IncomingThreadId)}#{outcomingPostMention.IncomingPostId}"">>>{outcomingPostMention.IncomingPostId}</a>";
                    }
                }
                return $@"<a class=""outcoming-post-mention"" data-boardid=""{boardId}"" data-postid=""{match.Groups[1].Value}"">>>{match.Groups[1].Value}</a>";
            }, RegexOptions.Singleline | RegexOptions.Compiled);

            return input;
        }

        public string MarkupStaticCustomEncode(string input, MarkupCustom[] markupCustomCollection)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            try
            {
                foreach (var markup in markupCustomCollection)
                {
                    input = Regex.Replace(input, markup.Pattern, markup.Replacement);
                }
            }
            catch
            {
            }

            return input;
        }

        public string MarkupDynamicCustomEncode(string input, MarkupCustom[] markupCustomCollection)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            try
            {
                foreach (var markup in markupCustomCollection)
                {
                    input = Regex.Replace(input, markup.Pattern, markup.Replacement);
                }
            }
            catch
            {
            }

            return input;
        }
    }
}