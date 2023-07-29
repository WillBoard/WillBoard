using Microsoft.AspNetCore.Routing;

namespace WillBoard.Core.Extensions
{
    public static class LinkGeneratorExtension
    {
        public static string GetPathToClassicThread(this LinkGenerator linkGenerator, string boardId, int? threadId, int postId, AnchorType anchorType = AnchorType.None, int? last = null, bool administration = false)
        {
            var action = administration ? "BoardViewClassicThread" : "ClassicThread";
            var controller = administration ? "Administration" : "Board";

            var link = linkGenerator.GetPathByAction(action, controller, new { boardId = boardId, threadId = threadId ?? postId, last = last });

            if (link == null)
            {
                return null;
            }

            if (anchorType == AnchorType.None)
            {
                return link;
            }

            var anchor = GetAnchor(anchorType);

            if (anchor == null)
            {
                return link;
            }

            return $"{link}{anchor}{postId}";
        }

        private static string GetAnchor(AnchorType anchorType)
        {
            switch (anchorType)
            {
                case AnchorType.Default:
                    return "#";

                case AnchorType.Reply:
                    return "#r";

                default:
                    return null;
            }
        }
    }

    public enum AnchorType
    {
        None = 0,
        Default = 1,
        Reply = 2
    }
}