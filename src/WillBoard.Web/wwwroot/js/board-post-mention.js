let postMentionCache = {};

document.addEventListener("DOMContentLoaded", function (event) {
    let mentionHyperlinkElements = document.querySelectorAll("a.outcoming-post-mention[href][data-boardid][data-postid], a.incoming-post-mention[href][data-boardid][data-postid]");
    for (let i = 0; i < mentionHyperlinkElements.length; i++) {
        mentionHyperlinkElements[i].addEventListener("mouseenter", showPostMentionAsync);
    }

    window.addEventListener("after-create-post-section-element-event", function (event) {
        let post = event.detail.post;
        let postSectionElement = event.detail.element;

        let mentionHyperlinkElements = postSectionElement.querySelectorAll("a.outcoming-post-mention[href][data-boardid][data-postid], a.incoming-post-mention[href][data-boardid][data-postid]");
        for (let i = 0; i < mentionHyperlinkElements.length; i++) {
            mentionHyperlinkElements[i].addEventListener("mouseenter", showPostMentionAsync);
        }

        for (let i = 0; i < post.incomingPostMentionCollection.length; i++) {
            addSelfIncomingPostMentionHyperlinkElement(post.incomingPostMentionCollection[i].outcomingBoardId, post.incomingPostMentionCollection[i].outcomingThreadId, post.incomingPostMentionCollection[i].outcomingPostId, post.incomingPostMentionCollection[i].incomingBoardId, post.incomingPostMentionCollection[i].incomingThreadId, post.incomingPostMentionCollection[i].incomingPostId, postSectionElement);
        }
    }, false);

    window.addEventListener("after-create-synchronization-event", function (event) {
        let post = event.detail;
        postMentionCache[`${post.boardId}-${post.postId}`] = post;
        for (let i = 0; i < post.outcomingPostMentionCollection.length; i++) {
            addIncomingPostMentionHyperlinkElement(post.outcomingPostMentionCollection[i].outcomingBoardId, post.outcomingPostMentionCollection[i].outcomingThreadId, post.outcomingPostMentionCollection[i].outcomingPostId, post.outcomingPostMentionCollection[i].incomingBoardId, post.outcomingPostMentionCollection[i].incomingThreadId, post.outcomingPostMentionCollection[i].incomingPostId);
        }

        for (let i = 0; i < post.incomingPostMentionCollection.length; i++) {
            activateOutcomingPostMention(post.incomingPostMentionCollection[i].incomingBoardId, post.incomingPostMentionCollection[i].incomingThreadId, post.incomingPostMentionCollection[i].incomingPostId);
        }
    }, false);

    window.addEventListener("after-delete-synchronization-event", function (event) {
        let data = event.detail;

        deactivateOutcomingPostMention(data.boardId, data.postId);
        removeIncomingPostMentionHyperlinkElement(data.boardId, data.postId);
    }, false);

});

async function showPostMentionAsync(event) {
    let postMentionHyperlinkElement = event.currentTarget;
    let boardId = postMentionHyperlinkElement.getAttribute("data-boardid");
    let postId = postMentionHyperlinkElement.getAttribute("data-postid");
    let postSectionElement = document.querySelector(`section[data-boardid="${boardId}"][data-postid="${postId}"]`);

    if (postSectionElement == null) {
        let postMention = postMentionCache[`${boardId}-${postId}`];

        if (postMention == null) {
            postMentionHyperlinkElement.style.cursor = "wait";
            let post = await getPostAsync(boardId, postId);
            postMentionHyperlinkElement.style.cursor = null;

            if (post == null) {
                return;
            }

            postMention = post;
            postMentionCache[`${boardId}-${postId}`] = post;
        }

        if (postMentionHyperlinkElement.matches(":hover")) {
            let postSectionElement = createPostSectionElement(postMention)

            let postMentionDivElement = document.createElement("div");
            postMentionDivElement.classList.add("post-mention");
            postMentionDivElement.style.left = `${postMentionHyperlinkElement.getBoundingClientRect().left + window.pageXOffset + postMentionHyperlinkElement.getBoundingClientRect().width + 10}px`;
            postMentionDivElement.style.top = `${postMentionHyperlinkElement.getBoundingClientRect().top + window.pageYOffset}px`;
            postMentionDivElement.appendChild(postSectionElement);

            let handlePostMention = function (event) {
                if (event.target !== postMentionHyperlinkElement) {
                    postMentionDivElement.remove();
                    document.removeEventListener("mousemove", handlePostMention);
                }
                else {
                    postMentionDivElement.style.left = `${event.pageX + 15}px`;
                    postMentionDivElement.style.top = `${event.pageY - 15}px`;
                }
            };
            document.addEventListener("mousemove", handlePostMention);

            document.body.appendChild(postMentionDivElement);
        }
    }
    else {
        let boundingClientRect = postSectionElement.getBoundingClientRect();
        if (boundingClientRect.top >= 40 && boundingClientRect.left >= 0 && boundingClientRect.bottom <= (window.innerHeight || document.documentElement.clientHeight) && boundingClientRect.right <= (window.innerWidth || document.documentElement.clientWidth)) {
            postSectionElement.classList.add("hover");

            let handlePostMentionHover = function (event) {
                if (event.target !== postMentionHyperlinkElement) {
                    postSectionElement.classList.remove("hover");
                    document.removeEventListener("mousemove", handlePostMentionHover);
                }
            };
            document.addEventListener("mousemove", handlePostMentionHover);
        }
        else {
            let postMentionDivElement = document.createElement("div");
            postMentionDivElement.classList.add("post-mention");
            postMentionDivElement.style.left = `${postMentionHyperlinkElement.getBoundingClientRect().left + window.pageXOffset + postMentionHyperlinkElement.getBoundingClientRect().width + 10}px`;
            postMentionDivElement.style.top = `${postMentionHyperlinkElement.getBoundingClientRect().top + window.pageYOffset}px`;
            postMentionDivElement.appendChild(postSectionElement.cloneNode(true));

            let handlePostMention = function (event) {
                if (event.target !== postMentionHyperlinkElement) {
                    postMentionDivElement.remove();
                    document.removeEventListener("mousemove", handlePostMention);
                }
                else {
                    postMentionDivElement.style.left = `${event.pageX + 15}px`;
                    postMentionDivElement.style.top = `${event.pageY - 15}px`;
                }
            };
            document.addEventListener("mousemove", handlePostMention);

            document.body.appendChild(postMentionDivElement);
        }
    }
}

function activateOutcomingPostMention(boardId, threadId, postId) {
    let inactiveOutcomingPostMentionHyperlinkElements = document.querySelectorAll(`a.outcoming-post-mention[data-boardid="${boardId}"][data-postid="${postId}"]:not([href])`);
    for (let i = 0; i < inactiveOutcomingPostMentionHyperlinkElements.length; i++) {
        let path = getBoardClassicThreadPath(boardId, threadId);
        inactiveOutcomingPostMentionHyperlinkElements[i].setAttribute("href", `${path}#${postId}`);
        inactiveOutcomingPostMentionHyperlinkElements[i].addEventListener("mouseenter", showPostMentionAsync);
    }
}

function deactivateOutcomingPostMention(boardId, postId) {
    let activeOutcomingPostMentionHyperlinkElements = document.querySelectorAll(`a.outcoming-post-mention[data-boardid="${boardId}"][data-postid="${postId}"][href]`);
    for (let i = 0; i < activeOutcomingPostMentionHyperlinkElements.length; i++) {
        activeOutcomingPostMentionHyperlinkElements[i].removeAttribute("href");
        activeOutcomingPostMentionHyperlinkElements[i].removeEventListener("mouseenter", showPostMentionAsync);
    }
}

function addIncomingPostMentionHyperlinkElement(outcomingBoardId, outcomingThreadId, outcomingPostId, incomingBoardId, incomingThreadId, incomingPostId) {
    let postSectionElement = document.querySelector(`section[data-boardid="${incomingBoardId}"][data-postid="${incomingPostId}"]`);

    if (postSectionElement == null) {
        return;
    }

    if (viewConfiguration.boardViewType === BoardViewType.ClassicBoard && postSectionElement.classList.contains("thread")) {
        return;
    }

    let existingIncomingPostMentionHyperlinkElement = postSectionElement.querySelector(`a.incoming-post-mention[data-boardid="${outcomingBoardId}"][data-postid="${outcomingPostId}"]`)
    if (existingIncomingPostMentionHyperlinkElement != null) {
        return;
    }

    let infoDivElement = postSectionElement.querySelector(".info");
    let incomingPostMentionHyperlinkElement = createIncomingPostMentionHyperlinkElement(outcomingBoardId, outcomingThreadId, outcomingPostId);

    infoDivElement.appendChild(incomingPostMentionHyperlinkElement);
}

function addSelfIncomingPostMentionHyperlinkElement(outcomingBoardId, outcomingThreadId, outcomingPostId, incomingBoardId, incomingThreadId, incomingPostId, postSectionElement) {
    if (postSectionElement == null) {
        return;
    }

    if (viewConfiguration.boardViewType === BoardViewType.ClassicBoard && postSectionElement.classList.contains("thread")) {
        return;
    }

    let boardId = postSectionElement.getAttribute("data-boardid");
    let postId = postSectionElement.getAttribute("data-postid");
    if (boardId != incomingBoardId || postId != incomingPostId) {
        return;
    }

    let existingIncomingPostMentionHyperlinkElement = postSectionElement.querySelector(`a.incoming-post-mention[data-boardid="${outcomingBoardId}"][data-postid="${outcomingPostId}"]`)
    if (existingIncomingPostMentionHyperlinkElement != null) {
        return;
    }

    let infoDivElement = postSectionElement.querySelector(".info");
    let incomingPostMentionHyperlinkElement = createIncomingPostMentionHyperlinkElement(outcomingBoardId, outcomingThreadId, outcomingPostId);

    infoDivElement.appendChild(incomingPostMentionHyperlinkElement);
}

function createIncomingPostMentionHyperlinkElement(boardId, threadId, postId) {
    let incomingPostMentionHyperlinkElement = document.createElement("a");
    incomingPostMentionHyperlinkElement.classList.add("incoming-post-mention");
    incomingPostMentionHyperlinkElement.setAttribute("data-boardid", boardId);
    incomingPostMentionHyperlinkElement.setAttribute("data-postid", postId);

    let path = getBoardClassicThreadPath(boardId, threadId ?? postId);
    incomingPostMentionHyperlinkElement.setAttribute("href", `${path}#${postId}`);
    incomingPostMentionHyperlinkElement.innerText = `>>` + `${postId}`;

    incomingPostMentionHyperlinkElement.addEventListener("mouseenter", showPostMentionAsync);

    return incomingPostMentionHyperlinkElement;
}

function removeIncomingPostMentionHyperlinkElement(boardId, postId) {
    let incomingPostMentionHyperlinkElements = document.querySelectorAll(`a.incoming-post-mention[data-boardid="${boardId}"][data-postid="${postId}"]`);
    for (let i = 0; i < incomingPostMentionHyperlinkElements.length; i++) {
        incomingPostMentionHyperlinkElements[i].remove();
    }
}