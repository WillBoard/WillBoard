let originalTittle = document.title;
let updateNumber = 0;

let sseCreateFunctionArray = [];


let synchronizationBoardConfiguration = {
    availability: true
};

let synchronizationThreadConfiguration = {
    availability: true
};


if (localStorage.getItem("synchronization-board") == null) {
    localStorage.setItem("synchronization-board", JSON.stringify(synchronizationBoardConfiguration));
}
else {
    synchronizationBoardConfiguration = JSON.parse(localStorage.getItem("synchronization-board"));
}

if (localStorage.getItem("synchronization-thread") == null) {
    localStorage.setItem("synchronization-thread", JSON.stringify(synchronizationThreadConfiguration));
}
else {
    synchronizationThreadConfiguration = JSON.parse(localStorage.getItem("synchronization-thread"));
}

function updateAvailabilitySynchronizationBoardConfiguration(event) {
    synchronizationBoardConfiguration.availability = event.target.checked;
    localStorage.setItem("synchronization-board", JSON.stringify(synchronizationBoardConfiguration));
}

function updateAvailabilitySynchronizationThreadConfiguration(event) {
    synchronizationThreadConfiguration.availability = event.target.checked;
    localStorage.setItem("synchronization-thread", JSON.stringify(synchronizationThreadConfiguration));
}


document.addEventListener("DOMContentLoaded", function (event) {
    document.querySelector("aside .settings").appendChild(createCheckboxSettingElement("Synchronize board", synchronizationBoardConfiguration.availability, "change", updateAvailabilitySynchronizationBoardConfiguration));
    document.querySelector("aside .settings").appendChild(createCheckboxSettingElement("Synchronize thread", synchronizationThreadConfiguration.availability, "change", updateAvailabilitySynchronizationThreadConfiguration));

    if (!((viewConfiguration.boardViewType === BoardViewType.ClassicBoard && viewConfiguration.pageCurrent === 1 && synchronizationBoardConfiguration.availability === true) || (viewConfiguration.boardViewType === BoardViewType.ClassicThread && synchronizationThreadConfiguration.availability === true))) {
        return;
    }

    document.addEventListener("visibilitychange", resetTittle, false);

    synchronize();
});

function synchronize() {
    let path = getApiSynchronizationPath(boardConfiguration.boardId);
    let eventSource = new EventSource(path);

    eventSource.addEventListener('open', handleOpenSynchronizationEvent);
    eventSource.addEventListener('close', handleCloseSynchronizationEvent);
    eventSource.addEventListener('error', handleErrorSynchronizationEvent);

    eventSource.addEventListener('heartbeat', handleHeartbeatSynchronizationEvent);
    eventSource.addEventListener('create', handleCreateSynchronizationEventAsync);
    eventSource.addEventListener('update', handleUpdateSynchronizationEventAsync);
    eventSource.addEventListener('delete', handleDeleteSynchronizationEventAsync);
}

function handleOpenSynchronizationEvent(event) {
    console.debug(`Handled "open" synchronization event.`, event);
    document.querySelector("body > nav > aside > label[for=synchronization]").classList.add("synchronized");
    document.querySelector("aside .synchronization p").textContent = `Connected`;
}

function handleCloseSynchronizationEvent(event) {
    console.warn(`Handled "close" synchronization event`, event);
    document.querySelector("body > nav > aside > label[for=synchronization]").classList.remove("synchronized");
    document.querySelector("aside .synchronization p").textContent = `Closed`;
    event.target.close();
}

function handleErrorSynchronizationEvent(event) {
    console.error(`Handled "error" synchronization event`, event);
    document.querySelector("body > nav > aside > label[for=synchronization]").classList.remove("synchronized");
    document.querySelector("aside .synchronization p").textContent = `Error`;
    event.target.close();
}

function handleHeartbeatSynchronizationEvent(event) {
    console.debug(`Handled "heartbeat" synchronization event.`, event);

    let beforeEvent = new CustomEvent("before-heartbeat-synchronization-event", {
        event: event
    });
    window.dispatchEvent(beforeEvent);

    let data = JSON.parse(event.data, customReviver);

    if (boardConfiguration.onlineCounterAvailability === true) {
        let onlineSpanElement = document.querySelector("body > nav > span.online-counter");

        if (onlineSpanElement == null) {
            return;
        }

        onlineSpanElement.textContent = `${data.online} online`;
    }

    let afterEvent = new CustomEvent("after-heartbeat-synchronization-event", {
        detail: data
    });
    window.dispatchEvent(afterEvent);
}

async function handleCreateSynchronizationEventAsync(event) {
    console.debug(`Handled "create" synchronization event`, event);

    let beforeEvent = new CustomEvent("before-create-synchronization-event", {
        event: event
    });
    window.dispatchEvent(beforeEvent);

    let post = JSON.parse(event.data, customReviver);

    if (viewConfiguration.boardViewType === BoardViewType.ClassicThread) {
        if (post.threadId != null) {
            let threadArticleElement = document.querySelector(`main article.thread[data-boardid='${post.boardId}'][data-threadid='${post.threadId}']`);
            if (threadArticleElement != null) {
                let createdPostElement = createPostSectionElement(post);
                createdPostElement.setAttribute("id", post.postId);

                threadArticleElement.appendChild(createdPostElement);

                let repliesQuantity = parseInt(threadArticleElement.getAttribute("data-reply-count"));
                threadArticleElement.setAttribute("data-reply-count", repliesQuantity + 1);

                let threadReplyCount = threadArticleElement.querySelector(".reply-count");
                threadReplyCount.textContent = repliesQuantity + 1;

                incrementTittle();
            }
        }
    }

    if (viewConfiguration.boardViewType === BoardViewType.ClassicBoard) {
        if (post.threadId == null) {
            let threadArticleElement = createThreadArticleElement(post, []);
            document.querySelector("main").insertBefore(threadArticleElement, document.querySelector("main article.thread:not([data-pinned])"));

            incrementTittle();
        }
        else {
            let threadArticleElement = document.querySelector(`main article.thread[data-boardid='${post.boardId}'][data-threadid='${post.threadId}']`);
            if (threadArticleElement != null) {

                if (threadArticleElement.getAttribute("data-replies-expanded") !== "true") {
                    let replySectionElements = threadArticleElement.querySelectorAll("section.reply");

                    let pinnedAttribute = threadArticleElement.getAttribute("data-pinned");
                    let previewMax = pinnedAttribute === "true" ? boardConfiguration.threadPinReplyPreviewMax : boardConfiguration.threadReplyPreviewMax;

                    if (replySectionElements.length >= previewMax) {
                        threadArticleElement.removeChild(replySectionElements[0]);
                    }
                }

                let repliesQuantity = parseInt(threadArticleElement.getAttribute("data-reply-count"));
                threadArticleElement.setAttribute("data-reply-count", repliesQuantity + 1);

                let threadReplyCount = threadArticleElement.querySelector(".reply-count");
                threadReplyCount.textContent = repliesQuantity + 1;


                let replySectionElement = createPostSectionElement(post);

                threadArticleElement.appendChild(replySectionElement);

                if (post.sage !== true) {
                    if (threadArticleElement.previousElementSibling) {
                        let pinnedAttribute = threadArticleElement.getAttribute("data-pinned");
                        if (pinnedAttribute === "true") {
                            document.querySelector("main").insertBefore(threadArticleElement, document.querySelector("main article.thread"));
                        }
                        else {
                            document.querySelector("main").insertBefore(threadArticleElement, document.querySelector("main article.thread:not([data-pinned])"));
                        }
                    }
                }

                addOrUpdateOmittedRepliesDivElement(threadArticleElement);
                addOrUpdateLastRepliesHyperlinkElement(threadArticleElement);

                incrementTittle();
            }
            else {
                if (post.sage !== true) {
                    if (post.threadId == null) {
                        let threadArticleElement = createThreadArticleElement(post, []);
                        document.querySelector("main").insertBefore(threadArticleElement, document.querySelector("main article.thread:not([data-pinned])"));
                    }
                    else {
                        let thread = await getPostAsync(post.boardId, post.threadId);
                        if (thread != null) {

                            let previewMax = thread.pin === true ? boardConfiguration.threadPinReplyPreviewMax : boardConfiguration.threadReplyPreviewMax;

                            let replies = await getRepliesAsync(post.boardId, post.threadId, previewMax);

                            let threadArticleElement = createThreadArticleElement(thread, replies);

                            document.querySelector("main").insertBefore(threadArticleElement, document.querySelector("main article.thread:not([data-pinned])"));
                        }

                        incrementTittle();
                    }
                }
            }
        }
    }

    let afterEvent = new CustomEvent("after-create-synchronization-event", {
        detail: post
    });
    window.dispatchEvent(afterEvent);
}

async function handleUpdateSynchronizationEventAsync(event) {
    console.debug(`Handled "update" synchronization event`, event);

    let beforeEvent = new CustomEvent("before-update-synchronization-event", {
        event: event
    });
    window.dispatchEvent(beforeEvent);

    let data = JSON.parse(event.data, customReviver);

    let afterEvent = new CustomEvent("after-update-synchronization-event", {
        detail: data
    });
    window.dispatchEvent(afterEvent);
}

async function handleDeleteSynchronizationEventAsync(event) {
    console.debug(`Handled "delete" synchronization event`, event);

    let beforeEvent = new CustomEvent("before-delete-synchronization-event", {
        event: event
    });
    window.dispatchEvent(beforeEvent);

    let data = JSON.parse(event.data, customReviver);

    if (data.threadId == null) {
        let threadArticleElement = document.querySelector(`article.thread[data-boardid='${data.boardId}'][data-threadid='${data.postId}']`);
        if (threadArticleElement != null) {
            threadArticleElement.remove();
        }
    }
    else {
        let threadArticleElement = document.querySelector(`article.thread[data-boardid='${data.boardId}'][data-threadid='${data.threadId}']`);
        if (threadArticleElement != null) {

            let repliesQuantity = parseInt(threadArticleElement.getAttribute("data-reply-count"));
            threadArticleElement.setAttribute("data-reply-count", repliesQuantity - 1);

            let threadReplyCount = threadArticleElement.querySelector(".reply-count");
            threadReplyCount.textContent = repliesQuantity - 1;

            let pinnedAttribute = threadArticleElement.getAttribute("data-pinned");

            let replySectionElement = threadArticleElement.querySelector(`section.reply[data-postid='${data.postId}']`);
            if (replySectionElement != null) {
                replySectionElement.remove();

                if (threadArticleElement.getAttribute("data-replies-expanded") !== "true" && viewConfiguration.boardViewType === BoardViewType.ClassicBoard) {
                    let replySectionElements = threadArticleElement.querySelectorAll(`section.reply`);
                    for (let i = 0; i < replySectionElements.length; i++) {
                        replySectionElements[i].remove();
                    }

                    let previewMax = pinnedAttribute === "true" ? boardConfiguration.threadPinReplyPreviewMax : boardConfiguration.threadReplyPreviewMax;

                    let replies = await getRepliesAsync(data.boardId, data.threadId, previewMax);

                    for (let i = 0; i < replies.length; i++) {
                        threadArticleElement.appendChild(createPostSectionElement(replies[i]));
                    }
                }
            }

            if (viewConfiguration.boardViewType === BoardViewType.ClassicBoard) {
                addOrUpdateOmittedRepliesDivElement(threadArticleElement);
                addOrUpdateLastRepliesHyperlinkElement(threadArticleElement);
            }
        }
    }

    let afterEvent = new CustomEvent("after-delete-synchronization-event", {
        detail: data
    });
    window.dispatchEvent(afterEvent);
}

function resetTittle() {
    if (document.hidden === false) {
        updateNumber = 0;
        document.title = originalTittle;
    }
}

function incrementTittle() {
    if (document.hidden === true) {
        if (viewConfiguration.boardViewType === BoardViewType.ClassicBoard) {
            document.title = `(*) ${originalTittle}`;
        }

        if (viewConfiguration.boardViewType === BoardViewType.ClassicThread) {
            updateNumber++;
            document.title = `(${updateNumber}) ${originalTittle}`;
        }
    }
}