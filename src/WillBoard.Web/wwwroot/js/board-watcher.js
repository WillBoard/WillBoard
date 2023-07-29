const WatcherStatus = {
    None: 0,
    Error: 1,
    New: 2
};

const WatcherId = "watcher";

let watcherCollection = [];

if (localStorage.getItem(WatcherId) == null) {
    localStorage.setItem(WatcherId, JSON.stringify(watcherCollection));
}
else {
    watcherCollection = JSON.parse(localStorage.getItem(WatcherId));
}

document.addEventListener("DOMContentLoaded", function (event) {
    if (viewConfiguration.boardViewType === BoardViewType.ClassicThread) {
        let threadElement = document.querySelector(`body > main > article.thread[data-boardid][data-threadid]`);
        let boardId = threadElement.getAttribute("data-boardid");
        let threadId = threadElement.getAttribute("data-threadid");
        let index = watcherCollection.findIndex(x => x.boardId === boardId && x.threadId === threadId);
        if (index > -1) {
            watcherCollection[index].status = WatcherStatus.None;
            updateWatcherStorage();
        }
    }

    let watcherUlElement = document.querySelector("body > nav > aside > section.watcher > ul");
    for (let i = 0; i < watcherCollection.length; i++) {
        let watcherLiElement = createWatcherLiElement(watcherCollection[i]);
        watcherUlElement.appendChild(watcherLiElement);
    }

    let threadArticleElements = document.querySelectorAll("body > main > article.thread");
    for (var i = 0; i < threadArticleElements.length; i++) {
        addWatcherOptionHyperlinkElement(threadArticleElements[i]);
    }

    window.addEventListener("after-create-thread-article-element-event", function (event) {
        let postArticleElement = event.detail.element;
        addWatcherOptionHyperlinkElement(postArticleElement);
    }, false);
});

function addLocalStorageWatcherItem(watcherItem) {
    watcherCollection.push(watcherItem);
    localStorage.setItem(WatcherId, JSON.stringify(watcherCollection));
}

function removeLocalStorageWatcherIndex(index) {
    watcherCollection.splice(index, 1);
    localStorage.setItem(WatcherId, JSON.stringify(watcherCollection));
}

function updateWatcherStorage() {
    localStorage.setItem(WatcherId, JSON.stringify(watcherCollection));
}

function addWatcherOptionHyperlinkElement(threadArticleElement) {
    let boardId = threadArticleElement.getAttribute("data-boardid");
    let threadId = threadArticleElement.getAttribute("data-threadid");
    let optionsAsideElement = threadArticleElement.querySelector("aside.options");

    let optionHyperlinkElement = document.createElement("a");
    optionHyperlinkElement.classList.add("watcher");
    if (watcherCollection.findIndex(x => x.boardId === boardId && x.threadId === threadId) === -1) {
        updateWatcherOptionHyperlinkElement(optionHyperlinkElement, false);
    }
    else {
        updateWatcherOptionHyperlinkElement(optionHyperlinkElement, true);
    }
    optionsAsideElement.appendChild(optionHyperlinkElement);
}

function updateWatcherOptionHyperlinkElement(optionHyperlinkElement, status) {
    if (status) {
        optionHyperlinkElement.textContent = localization.unwatch;
        optionHyperlinkElement.removeEventListener('click', watchWatcherItem);
        optionHyperlinkElement.addEventListener('click', unwatchWatcherItem);
    }
    else {
        optionHyperlinkElement.textContent = localization.watch;
        optionHyperlinkElement.removeEventListener('click', unwatchWatcherItem);
        optionHyperlinkElement.addEventListener('click', watchWatcherItem);
    }
}

function watchWatcherItem(event) {
    event.preventDefault();

    let optionHyperlinkElement = event.currentTarget;
    let threadArticleElement = optionHyperlinkElement.closest("article.thread[data-boardid][data-threadid]");

    let boardId = threadArticleElement.getAttribute("data-boardid");
    let threadId = threadArticleElement.getAttribute("data-threadid");

    let index = watcherCollection.findIndex(x => x.boardId === boardId && x.threadId === threadId);
    if (index === -1) {
        let thread = { boardId: boardId, threadId: threadId, replies: null, title: "", status: WatcherStatus.None };

        addWatcherItemLiElement(thread);
        updateWatcherOptionHyperlinkElement(optionHyperlinkElement, true);
        addLocalStorageWatcherItem(thread);

        updateWatcher(thread);
    }
}

function unwatchWatcherItem(event) {
    event.preventDefault();

    let optionHyperlinkElement = event.currentTarget;
    let threadArticleElement = optionHyperlinkElement.closest("article.thread[data-boardid][data-threadid]");

    let boardId = threadArticleElement.getAttribute("data-boardid");
    let threadId = threadArticleElement.getAttribute("data-threadid");

    let index = watcherCollection.findIndex(x => x.boardId === boardId && x.threadId === threadId);
    if (index > -1) {
        removeWatcherItemLiElement(watcherCollection[index]);

        let optionHyperlinkElement = document.querySelector(`body > main > article.thread[data-boardid='${boardId}'][data-threadid='${threadId}'] aside.options a.watcher`);
        if (optionHyperlinkElement !== null) {
            updateWatcherOptionHyperlinkElement(optionHyperlinkElement, false);
        }

        removeLocalStorageWatcherIndex(index);
    }
}

function createWatcherLiElement(thread) {
    let liElement = document.createElement("li");
    liElement.setAttribute("data-boardid", thread.boardId);
    liElement.setAttribute("data-threadid", thread.threadId);

    let boardIdElement = document.createElement("a");
    boardIdElement.setAttribute("href", getBoardClassicPath(thread.boardId));
    boardIdElement.textContent = thread.boardId;
    liElement.appendChild(boardIdElement);

    let threadIdElement = document.createElement("a");
    threadIdElement.setAttribute("href", getBoardClassicThreadPath(thread.boardId, thread.threadId));
    threadIdElement.textContent = thread.threadId;
    liElement.appendChild(threadIdElement);

    let titleElement = document.createElement("input");
    titleElement.setAttribute("type", "text");
    titleElement.value = thread.title;
    titleElement.addEventListener('change', updateWatcherItemTitle);
    liElement.appendChild(titleElement);

    let replyCountSpanElement = document.createElement("span");
    replyCountSpanElement.textContent = thread.replyCount;
    updateWatcherItemSpanElement(replyCountSpanElement, thread.status);
    liElement.appendChild(replyCountSpanElement);

    let refreshElement = document.createElement("a");
    refreshElement.classList.add("icon");
    refreshElement.textContent = "";
    refreshElement.addEventListener('click', updateWatcherItemAction);
    liElement.appendChild(refreshElement);

    let removeElement = document.createElement("a");
    removeElement.classList.add("icon");
    removeElement.textContent = "";
    removeElement.addEventListener('click', removeWatcherItem);
    liElement.appendChild(removeElement);

    return liElement;
}

function addWatcherItemLiElement(watcherItem) {
    let ulElement = document.querySelector("body > nav > aside > section.watcher > ul");
    let liElement = createWatcherLiElement(watcherItem);
    ulElement.appendChild(liElement);
}

function removeWatcherItemLiElement(watcherItem) {
    let liElement = document.querySelector(`nav  > aside > section.watcher > ul > li[data-boardid='${watcherItem.boardId}'][data-threadid='${watcherItem.threadId}']`);
    if (liElement !== null) {
        liElement.remove();
    }
}

function removeWatcherItem(event) {
    event.preventDefault();

    let removeElement = event.currentTarget;
    let liElement = removeElement.closest("li[data-boardid][data-threadid]");

    let boardId = liElement.getAttribute("data-boardid");
    let threadId = liElement.getAttribute("data-threadid");

    let index = watcherCollection.findIndex(x => x.boardId === boardId && x.threadId === threadId);
    if (index > -1) {
        removeWatcherItemLiElement(watcherCollection[index]);

        let optionElement = document.querySelector(`body > main > article.thread[data-boardid='${boardId}'][data-threadid='${threadId}'] aside.options a.watcher`);
        if (optionElement !== null) {
            updateWatcherOptionHyperlinkElement(optionElement, false);
        }

        removeWatcherItemLiElement(watcherCollection[index]);
        removeLocalStorageWatcherIndex(index);
    }
}

async function updateWatcherItemAction(event) {
    event.preventDefault();

    let removeElement = event.currentTarget;
    let liElement = removeElement.closest("li[data-boardid][data-threadid]");

    let boardId = liElement.getAttribute("data-boardid");
    let threadId = liElement.getAttribute("data-threadid");

    let index = watcherCollection.findIndex(x => x.boardId === boardId && x.threadId === threadId);
    if (index > -1) {
        await updateWatcher(watcherCollection[index]);
    }
}

async function updateWatcherItemTitle(event) {
    let titleInputElement = event.currentTarget;
    let title = titleInputElement.value;
    let liElement = titleInputElement.closest("li[data-boardid][data-threadid]");

    let boardId = liElement.getAttribute("data-boardid");
    let threadId = liElement.getAttribute("data-threadid");

    let index = watcherCollection.findIndex(x => x.boardId === boardId && x.threadId === threadId);
    if (index > -1) {
        watcherCollection[index].title = title;
        updateWatcherStorage();
    }
}

async function updateWatcherItemSpanElement(spanElement, status) {
    switch (status) {
        case WatcherStatus.None:
            if (spanElement.classList.contains("error")) {
                spanElement.classList.remove("error");
            }
            if (spanElement.classList.contains("new")) {
                spanElement.classList.remove("new");
            }
            break;
        case WatcherStatus.Error:
            if (!spanElement.classList.contains("error")) {
                spanElement.classList.add("error");
            }
            break;
        case WatcherStatus.New:
            if (!spanElement.classList.contains("new")) {
                spanElement.classList.add("new");
            }
            break;
    }
}

async function updateWatcher(thread) {
    let replyCount = thread.replyCount;
    try {
        let post = await getPostAsync(thread.boardId, thread.threadId);
        replyCount = post.replyCount;
    }
    catch (error) {
        console.error("Error occured during updateWatcher function.", error);
        replyCount = null;
    }

    let liElement = document.querySelector(`nav  > aside > section.watcher > ul > li[data-boardid='${thread.boardId}'][data-threadid='${thread.threadId}']`);
    let replyCountSpanElement = liElement.querySelector("span");


    if (thread.replyCount === null) {
        thread.replyCount = replyCount;
    }

    if (replyCount === null) {
        thread.status = WatcherStatus.Error;
    }
    else if (replyCount > thread.replyCount) {
        thread.status = WatcherStatus.New;
        thread.replyCount = replyCount;
    }
    else {
        thread.status = WatcherStatus.None;
        thread.replyCount = replyCount;
    }

    updateWatcherItemSpanElement(replyCountSpanElement, thread.status);
    replyCountSpanElement.textContent = thread.replyCount;

    updateWatcherStorage();
}