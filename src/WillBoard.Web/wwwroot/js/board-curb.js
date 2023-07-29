let curbs = {};

if (localStorage.getItem("curbs") == null) {
    localStorage.setItem("curbs", JSON.stringify(curbs));
}
else {
    curbs = JSON.parse(localStorage.curbs);
}

document.addEventListener("DOMContentLoaded", function (event) {
    if (!(viewConfiguration.boardViewType === BoardViewType.ClassicBoard || viewConfiguration.boardViewType === BoardViewType.ClassicThread)) {
        return;
    }

    checkCurbs();

    let optionElements = document.querySelectorAll("article.thread aside.options");
    for (var i = 0; i < optionElements.length; i++) {
        addCurbOption(optionElements[i]);
    }

    var replySectionElements = document.querySelectorAll("section.reply");
    for (let i = 0; i < replySectionElements.length; i++) {
        enableReplyCurb(replySectionElements[i]);
    }

    var threadArticleElements = document.querySelectorAll("article.thread");
    for (let i = 0; i < threadArticleElements.length; i++) {
        enableThreadCurb(threadArticleElements[i]);
    }

    window.addEventListener("after-create-post-section-element-event", function (event) {
        let postSectionElement = event.detail.element;

        if (postSectionElement.classList.contains("thread")) {
            return;
        }

        let optionsAsideElement = postSectionElement.querySelector("aside.options");
        addCurbOption(optionsAsideElement);
        enableReplyCurb(postSectionElement);
    }, false);

    window.addEventListener("after-create-thread-article-element-event", function (event) {
        let postArticleElement = event.detail.element;
        let threadSectionElement = postArticleElement.querySelector("section.thread");
        let optionsAsideElement = threadSectionElement.querySelector("aside.options");
        addCurbOption(optionsAsideElement);
        enableThreadCurb(postArticleElement);
    }, false);
});

function checkCurbs() {
    for (let i = 0; i < curbs.length; i++) {
        let boardId = curbs[i];
        for (let i = 0; i < boardId.length; i++) {
            let postId = boardId[i];
            if (curbs[boardId][postId] < Date.now() - 60 * 60 * 24 * 180) {
                curbs[boardId][postId] = undefined;
                let jsonCurbs = JSON.stringify(curbs);
                localStorage.setItem(curbs, jsonCurbs);
            }
        }
    }
}

function addCurbOption(optionElement) {
    let curbOptionHyperlinkElement = document.createElement("a");
    curbOptionHyperlinkElement.classList.add("curb");
    curbOptionHyperlinkElement.textContent = localization.curb;
    curbOptionHyperlinkElement.addEventListener("click", toggleCurb);
    optionElement.appendChild(curbOptionHyperlinkElement);
}

function enableReplyCurb(replySectionElement) {
    let boardId = replySectionElement.getAttribute("data-boardid");
    let postId = replySectionElement.getAttribute("data-postid");

    if (curbs[boardId] == null) {
        return;
    }

    if (curbs[boardId][postId] == null) {
        return;
    }

    replySectionElement.classList.add("curb");

    let curbOptionHyperlinkElement = replySectionElement.querySelector(".options a.curb")
    curbOptionHyperlinkElement.textContent = localization.uncurb;

    curbs[boardId][postId] = Date.now();
}

function enableThreadCurb(threadSectionElement) {
    let boardId = threadSectionElement.getAttribute("data-boardid");
    let postId = threadSectionElement.getAttribute("data-threadid");

    if (curbs[boardId] == null) {
        return;
    }

    if (curbs[boardId][postId] == null) {
        return;
    }

    threadSectionElement.classList.add("curb");

    let curbOptionHyperlinkElement = threadSectionElement.querySelector("section.thread .options a.curb")
    curbOptionHyperlinkElement.textContent = localization.uncurb;

    curbs[boardId][postId] = Date.now();
}

function toggleCurb(event) {
    let threadArticleElement = event.target.closest("article.thread");
    let postSectionElement = event.target.closest("section.thread, section.reply");
    let boardId = postSectionElement.getAttribute("data-boardid");
    let postId = postSectionElement.getAttribute("data-postid");

    if (event.target.textContent === localization.curb) {
        if (postSectionElement.classList.contains("thread")) {
            threadArticleElement.classList.add("curb");
        }
        else if (postSectionElement.classList.contains("reply")) {
            postSectionElement.classList.add("curb");
        }

        event.target.textContent = localization.uncurb;

        if (curbs[boardId] == null) {
            curbs[boardId] = {};
        }

        curbs[boardId][postId] = Date.now();
        localStorage.curbs = JSON.stringify(curbs);
    }
    else if (event.target.textContent === localization.uncurb) {
        if (postSectionElement.classList.contains("thread")) {
            threadArticleElement.classList.remove("curb");
        }
        else if (postSectionElement.classList.contains("reply")) {
            postSectionElement.classList.remove("curb");
        }

        event.target.textContent = localization.curb;

        curbs[boardId][postId] = undefined;
        localStorage.curbs = JSON.stringify(curbs);
    }
}