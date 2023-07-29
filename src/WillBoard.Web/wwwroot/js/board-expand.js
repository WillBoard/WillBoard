document.addEventListener("DOMContentLoaded", function (event) {
    if (!(viewConfiguration.boardViewType === BoardViewType.ClassicBoard)) {
        return;
    }

    let threadArticleElements = document.querySelectorAll(`article.thread`);
    for (let i = 0; i < threadArticleElements.length; i++) {
        addExpandReplies(threadArticleElements[i]);
    }

    window.addEventListener("after-create-thread-article-element-event", function (event) {
        let threadArticleElement = event.detail.element;
        addExpandReplies(threadArticleElement);
    }, false);
});

function addExpandReplies(threadArticleElement) {
    let replyPreviewMax = threadArticleElement.getAttribute("data-pinned") === "true" ? boardConfiguration.threadPinReplyPreviewMax : boardConfiguration.threadReplyPreviewMax;
    let replyCount = parseInt(threadArticleElement.getAttribute("data-reply-count"));
    threadArticleElement.setAttribute("data-replies-expanded", `false`);

    if (replyCount > replyPreviewMax) {
        let omittedRepliesDivElement = threadArticleElement.querySelector("section.thread .replies-omitted");
        let showHyperlinkElement = omittedRepliesDivElement.querySelector("a");
        showHyperlinkElement.addEventListener('click', toggleExpandRepliesAsync);
    }
}

async function toggleExpandRepliesAsync(event) {
    event.preventDefault();

    let currentTargetElement = event.currentTarget;
    let threadArticleElement = currentTargetElement.closest("article.thread");
    let boardId = threadArticleElement.getAttribute("data-boardid");
    let threadId = threadArticleElement.getAttribute("data-threadid");

    let skip = threadArticleElement.getAttribute("data-pinned") === "true" ? boardConfiguration.threadPinReplyPreviewMax : boardConfiguration.threadReplyPreviewMax;

    if (threadArticleElement.getAttribute("data-replies-expanded") === "false") {
        currentTargetElement.style.cursor = "wait";

        let replies = await getRepliesAsync(boardId, threadId);

        currentTargetElement.style.cursor = null;

        if (replies != null) {
            let replySectionElements = threadArticleElement.querySelectorAll(`section.reply`);
            for (var i = 0; i < replySectionElements.length; i++) {
                replySectionElements[i].remove();
            }

            for (var i = 0; i < replies.length; i++) {
                let replySection = createPostSectionElement(replies[i]);
                threadArticleElement.appendChild(replySection);
            }

            currentTargetElement.textContent = localization.hide;
            currentTargetElement.previousElementSibling.textContent = parseInt(currentTargetElement.previousElementSibling.previousElementSibling.textContent) > 1 ? `${localization.repliesExpanded}.` : `${localization.replyExpanded}.`;
            threadArticleElement.setAttribute("data-replies-expanded", "true");
        }
    }
    else {
        let replies = threadArticleElement.querySelectorAll("section.reply");
        for (var i = 0; i < replies.length - skip; i++) {
            replies[i].remove();
        }
        threadArticleElement.setAttribute("data-replies-expanded", "false");
        currentTargetElement.textContent = localization.show;
        currentTargetElement.previousElementSibling.textContent = parseInt(currentTargetElement.previousElementSibling.previousElementSibling.textContent) > 1 ? `${localization.repliesOmitted}.` : `${localization.replyOmitted}.`;
    }
}