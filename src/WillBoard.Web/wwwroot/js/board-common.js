const ViewType = {
    Other: 0,
    Application: 1,
    Board: 2
};

const BoardViewType = {
    Other: 0,
    ClassicBoard: 1,
    ClassicThread: 2,
    Catalog: 3,
    Search: 4
};

const VerificationType = {
    None: 0,
    ClassicCAPTCHA: 1,
    ReCAPTCHA: 2
};

function getBoardClassicPath(boardId) {
    return `/${boardId}`;
}

function getBoardClassicThreadPath(boardId, threadId, last = 0) {
    if (last > 0) {
        return `/${boardId}/thread/${threadId}/${last}`;
    }

    return `/${boardId}/thread/${threadId}`;
}

function getApiVerificationPath(boardId, thread) {
    return `/api/${boardId}/verification/${thread}`;
}

function getApiPostPath(boardId) {
    return `/api/${boardId}/post`;
}

function getApiPostsPath(boardId, postId) {
    return `/api/${boardId}/posts/${postId}`;
}

function getApiRepliesPath(boardId, threadId, last = 0) {
    if (last > 0) {
        return `/api/${boardId}/replies/${threadId}/${last}`;
    }

    return `/api/${boardId}/replies/${threadId}`;
}

function getApiSynchronizationPath(boardId) {
    return `/api/${boardId}/synchronization`;
}

async function getVerificationAsync(boardId, thread) {
    try {
        let path = getApiVerificationPath(boardId, thread);

        let fetchResponse = await fetch(path, {
            cache: "no-cache",
            headers: {
                "accept": "application/json"
            },
            credentials: "same-origin",
            method: "GET"
        });

        let response = await fetchResponse.json();

        if (response.error != null) {
            console.error(`Error with status ${response.error.status}.`, response.error);
            return null;
        }

        return response.data;
    }
    catch (error) {
        console.error("Unknown error.", error);
        return null;
    }
}

async function getPostAsync(boardId, postId) {
    try {
        let path = getApiPostsPath(boardId, postId);

        let fetchResponse = await fetch(path, {
            cache: "no-cache",
            headers: {
                "accept": "application/json"
            },
            credentials: "same-origin",
            method: "GET"
        });

        let response = await fetchResponse.json();

        if (response.error != null) {
            console.error(`Error with status ${response.error.status}.`, response.error);
            return null;
        }

        return response.data;
    }
    catch (error) {
        console.error("Unknown error.", error);
        return null;
    }
}

async function getRepliesAsync(boardId, threadId, last = 0) {
    try {
        let path = getApiRepliesPath(boardId, threadId, last);

        let fetchResponse = await fetch(path, {
            cache: "no-cache",
            headers: {
                "accept": "application/json"
            },
            credentials: "same-origin",
            method: "GET"
        });

        let response = await fetchResponse.json();

        if (response.error != null) {
            console.error(`Error with status ${response.error.status}.`, response.error);
            return null;
        }

        return response.data;
    }
    catch (error) {
        console.error("Unknown error.", error);
        return null;
    }
}

function isNullOrEmpty(value) {
    if (value == null) {
        return true;
    }

    if (value === "") {
        return true;
    }

    return false;
}

function parseUtcIso8601StringToRfc3339String(date) {
    let substring = date.substring(0, 19);
    let utcDate = `${substring}Z`;
    return new Date(utcDate).toISOString().replace("T", " ").substring(0, 19);
}

function createOmittedRepliesDivElement(threadArticleElement, replyCount, replyPreviewMax) {
    let boardId = threadArticleElement.getAttribute("data-boardid");
    let threadId = threadArticleElement.getAttribute("data-threadid");

    let omittedRepliesDivElement = document.createElement("div");
    omittedRepliesDivElement.classList.add("replies-omitted");

    let countSpanElement = document.createElement("span");
    countSpanElement.innerText = replyCount - replyPreviewMax;
    omittedRepliesDivElement.appendChild(countSpanElement);

    let textSpanElement = document.createElement("span");
    textSpanElement.innerText = (replyCount - replyPreviewMax) > 1 ? `${localization.repliesOmitted}.` : `${localization.replyOmitted}.`;
    omittedRepliesDivElement.appendChild(textSpanElement);

    let showHyperlinkElement = document.createElement("a");
    let threadPath = getBoardClassicThreadPath(boardId, threadId);

    showHyperlinkElement.setAttribute("href", threadPath);
    showHyperlinkElement.innerText = localization.show;
    showHyperlinkElement.addEventListener('click', toggleExpandRepliesAsync);
    omittedRepliesDivElement.appendChild(showHyperlinkElement);

    return omittedRepliesDivElement;
}

function addOrUpdateOmittedRepliesDivElement(threadArticleElement) {
    let pinnedAttribute = threadArticleElement.getAttribute("data-pinned");
    let replyCount = parseInt(threadArticleElement.getAttribute("data-reply-count"));
    let replyPreviewMax = pinnedAttribute === "true" ? boardConfiguration.threadPinReplyPreviewMax : boardConfiguration.threadReplyPreviewMax;

    let omittedRepliesDivElement = threadArticleElement.querySelector("section.thread div.replies-omitted");

    if (omittedRepliesDivElement == null) {
        if (replyCount > replyPreviewMax) {
            let createdOmittedRepliesDivElement = createOmittedRepliesDivElement(threadArticleElement, replyCount, replyPreviewMax);
            threadArticleElement.querySelector("section.thread").appendChild(createdOmittedRepliesDivElement);
        }
    }
    else {
        if (replyCount <= replyPreviewMax) {
            omittedRepliesDivElement.remove();
        }
        else {
            threadArticleElement.querySelector("section.thread .replies-omitted span:nth-of-type(1)").textContent = replyCount - replyPreviewMax;

            if (threadArticleElement.getAttribute("data-replies-expanded") === "true") {
                threadArticleElement.querySelector("section.thread .replies-omitted span:nth-of-type(2)").textContent = (replyCount - replyPreviewMax) > 1 ? `${localization.repliesExpanded}.` : `${localization.replyExpanded}.`;
            }
            else {
                threadArticleElement.querySelector("section.thread .replies-omitted span:nth-of-type(2)").textContent = (replyCount - replyPreviewMax) > 1 ? `${localization.repliesOmitted}.` : `${localization.replyOmitted}.`;
            }
        }
    }
}

function addOrUpdateLastRepliesHyperlinkElement(threadArticleElement) {
    let boardIdAttribute = threadArticleElement.getAttribute("data-boardid");
    let threadIdAttribute = threadArticleElement.getAttribute("data-threadid");
    let replyCount = parseInt(threadArticleElement.getAttribute("data-reply-count"));

    let lastRepliesHyperlinkElement = threadArticleElement.querySelector("section.thread div.info a.last-replies");

    if (lastRepliesHyperlinkElement == null) {
        if (replyCount > 50) {
            let infoLastRepliesHyperlinkElement = document.createElement("a");
            infoLastRepliesHyperlinkElement.textContent = `${localization.reply} (50)`;
            infoLastRepliesHyperlinkElement.classList.add("button", "last-replies");
            infoLastRepliesHyperlinkElement.setAttribute("href", `/${boardIdAttribute}/thread/${threadIdAttribute}/50`);
            threadArticleElement.querySelector("section.thread div.info").appendChild(infoLastRepliesHyperlinkElement);
        }
    }
    else {
        if (replyCount <= 50) {
            lastRepliesHyperlinkElement.remove();
        }
    }
}

function createThreadArticleElement(thread, replies) {
    let threadArticleElement = document.createElement("article");
    threadArticleElement.setAttribute("class", `thread`);
    threadArticleElement.setAttribute("data-boardid", thread.boardId);
    threadArticleElement.setAttribute("data-threadid", thread.postId);
    threadArticleElement.setAttribute("data-reply-count", `${thread.replyCount}`);

    if (threadArticleElement.Pin) {
        threadArticleElement.setAttribute("data-pinned", true);
    }

    let threadPostSectionElement = createPostSectionElement(thread);

    threadArticleElement.appendChild(threadPostSectionElement);

    if (replies != null) {
        for (let i = 0; i < replies.length; i++) {
            threadArticleElement.appendChild(createPostSectionElement(replies[i]));
        }
    }

    addOrUpdateOmittedRepliesDivElement(threadArticleElement);
    addOrUpdateLastRepliesHyperlinkElement(threadArticleElement);

    var afterEvent = new CustomEvent("after-create-thread-article-element-event", {
        detail: { thread: thread, element: threadArticleElement }
    });
    window.dispatchEvent(afterEvent);

    return threadArticleElement;
}

function createPostSectionElement(post) {
    let section = document.createElement("section");

    if (post.threadId == null) {
        section.classList.add("thread");
    }
    else {
        section.classList.add("reply");
    }

    section.setAttribute("data-boardid", post.boardId);
    section.setAttribute("data-postid", post.postId);

    let info = document.createElement("div");
    info.classList.add("info");

    let infoOptionsInput = document.createElement("input");
    infoOptionsInput.setAttribute("type", "checkbox");
    infoOptionsInput.setAttribute("id", `options-${post.boardId}${post.postId}`);
    info.appendChild(infoOptionsInput);

    let infoOptionsLabel = document.createElement("label");
    infoOptionsLabel.setAttribute("for", `options-${post.boardId}${post.postId}`);
    infoOptionsLabel.setAttribute("title", localization.options);
    infoOptionsLabel.textContent = "";
    infoOptionsLabel.classList.add("optionsLabel");
    info.appendChild(infoOptionsLabel);

    let optionsAside = document.createElement("aside");
    optionsAside.classList.add("options");

    let optionReport = document.createElement("a");
    optionReport.setAttribute("href", `/${post.boardId}/report/${post.postId}`);
    optionReport.setAttribute("target", "_blank");
    optionReport.textContent = localization.report;

    let optionDelete = document.createElement("a");
    optionDelete.setAttribute("href", `/${post.boardId}/delete/${post.postId}`);
    optionDelete.setAttribute("target", "_blank");
    optionDelete.textContent = localization.delete;

    optionsAside.appendChild(optionReport);
    optionsAside.appendChild(optionDelete);

    info.appendChild(optionsAside);


    if (post.subject != null && post.subject != "") {
        let infoSubject = document.createElement("span");
        infoSubject.classList.add("subject");
        infoSubject.textContent = post.subject;
        info.appendChild(infoSubject);
    }

    let infoName = document.createElement("a");
    infoName.classList.add("name");
    infoName.textContent = post.name;
    if (post.email != null && post.email != "") {
        infoName.setAttribute("href", `mailto:${post.email}`);
    }
    info.appendChild(infoName);

    if (!isNullOrEmpty(post.userId)) {
        let infoUserId = document.createElement("span");
        infoUserId.classList.add("user-id");
        infoUserId.textContent = post.userId;
        info.appendChild(infoUserId);
    }

    if (!isNullOrEmpty(post.country)) {
        let infoCountry = document.createElement("span");
        infoCountry.classList.add("country");
        infoCountry.classList.add(post.country.toLowerCase());
        infoCountry.setAttribute("title", post.country);
        info.appendChild(infoCountry);
    }

    if (post.file) {
        let infoFile = document.createElement("a");
        infoFile.classList.add("file");

        if (post.fileDeleted === true) {
            infoFile.textContent = localization.fileDeleted;
        }

        else {
            infoFile.setAttribute("title", `${sizeSuffix(post.fileSize)}${post.fileHeight == null || post.fileWidth == null ? "" : ` ${post.fileHeight}x${post.fileWidth}`}${post.fileDuration == null || post.fileDuration == 0 ? "" : ` ${post.fileDuration.toFixed(2)}s`} "${post.fileNameOriginal}" (${post.fileMimeType})`);
            infoFile.setAttribute("href", `/boards/${post.boardId}/source/${post.fileName}`);

            let infoFileName = document.createElement("span");
            infoFileName.textContent = post.fileNameOriginal.substr(0, post.fileNameOriginal.lastIndexOf("."));
            infoFile.appendChild(infoFileName);

            let infoFileExtension = document.createElement("span");
            infoFileExtension.textContent = post.fileNameOriginal.substr(post.fileNameOriginal.lastIndexOf("."));
            infoFile.appendChild(infoFileExtension);
        }

        info.appendChild(infoFile);
    }


    let infoCreated = document.createElement("time");
    infoCreated.setAttribute("datetime", post.creation);
    infoCreated.textContent = parseUtcIso8601StringToRfc3339String(post.creationLocal);
    info.appendChild(infoCreated);

    let infoNumberLink = document.createElement("a");
    infoNumberLink.textContent = "#";
    if (post.threadId == null) {
        let threadPath = getBoardClassicThreadPath(post.boardId, post.postId);
        infoNumberLink.setAttribute("href", `${threadPath}#${post.postId}`);
    }
    else {
        let threadPath = getBoardClassicThreadPath(post.boardId, post.threadId);
        infoNumberLink.setAttribute("href", `${threadPath}#${post.postId}`);
    }
    info.appendChild(infoNumberLink);

    let infoNumberReply = document.createElement("a");
    infoNumberReply.textContent = post.postId;
    infoNumberReply.classList.add("reply");
    if (post.threadId == null) {
        let threadPath = getBoardClassicThreadPath(post.boardId, post.postId);
        infoNumberReply.setAttribute("href", `${threadPath}#r${post.postId}`);
    }
    else {
        let threadPath = getBoardClassicThreadPath(post.boardId, post.threadId);
        infoNumberReply.setAttribute("href", `${threadPath}#r${post.postId}`);
    }
    info.appendChild(infoNumberReply);

    if (post.threadId == null) {
        let infoReplyCountSpanElement = document.createElement("span");
        infoReplyCountSpanElement.setAttribute("class", "reply-count icon-before");
        infoReplyCountSpanElement.textContent = post.replyCount;
        info.appendChild(infoReplyCountSpanElement);

        if (post.replyLock === true) {
            let infoIconReplyLock = document.createElement("span");
            infoIconReplyLock.classList.add("icon");
            infoIconReplyLock.textContent = "";
            info.appendChild(infoIconReplyLock);
        }

        if (post.excessive != null) {
            let infoIconExcessive = document.createElement("span");
            infoIconExcessive.classList.add("icon");
            infoIconExcessive.textContent = "";
            infoIconExcessive.setAttribute("title", `${parseUtcIso8601StringToRfc3339String(post.excessive)} (UTC)`);
            info.appendChild(infoIconExcessive);
        }

        if (post.pin === true) {
            let infoIconPin = document.createElement("span");
            infoIconPin.classList.add("icon");
            infoIconPin.textContent = "";
            info.appendChild(infoIconPin);
        }

        if (post.forceCountry === true) {
            let infoIconForceCountry = document.createElement("span");
            infoIconForceCountry.classList.add("icon");
            infoIconForceCountry.textContent = "";
            info.appendChild(infoIconForceCountry);
        }

        if (post.forceUserId === true) {
            let infoIconForceUserId = document.createElement("span");
            infoIconForceUserId.classList.add("icon");
            infoIconForceUserId.textContent = "";
            info.appendChild(infoIconForceUserId);
        }

        let infoReplyButton = document.createElement("a");
        infoReplyButton.textContent = localization.reply;
        infoReplyButton.classList.add("button");
        infoReplyButton.setAttribute("href", getBoardClassicThreadPath(post.boardId, post.postId));
        info.appendChild(infoReplyButton);

        if (post.replyCount > 50) {
            let infoReplyButton = document.createElement("a");
            infoReplyButton.textContent = `${localization.reply} (50)`;
            infoReplyButton.classList.add("button", "last-replies");
            infoReplyButton.setAttribute("href", getBoardClassicThreadPath(post.boardId, post.postId, 50));
            info.appendChild(infoReplyButton);
        }
    }

    section.appendChild(info);

    let content = document.createElement("div");
    content.classList.add("content");

    if (post.file === true && post.filePreview === true && !post.fileDeleted) {
        let fileDiv = document.createElement("div");
        fileDiv.classList.add("file");
        fileDiv.setAttribute("data-mime", post.fileMimeType);

        fileDiv.setAttribute("data-spoiler", post.fileSpoiler === true ? true : false);

        let fileA = document.createElement("a");
        fileA.setAttribute("href", `/boards/${post.boardId}/source/${post.fileName}`);

        let fileImg = document.createElement("img");
        fileImg.setAttribute("alt", "");

        if (post.fileSpoiler === true) {
            if (post.threadId == null) {
                fileImg.setAttribute("height", boardConfiguration.threadFilePreviewHeightMax);
                fileImg.setAttribute("width", boardConfiguration.threadFilePreviewWidthMax);
            }
            else {
                fileImg.setAttribute("height", boardConfiguration.replyFilePreviewHeightMax);
                fileImg.setAttribute("width", boardConfiguration.replyFilePreviewWidthMax);
            }

            fileImg.setAttribute("src", `/img/spoiler.svg`);
            fileImg.setAttribute("data-preview-src", `/boards/${post.boardId}/preview/${post.filePreviewName}`);
            fileImg.setAttribute("data-preview-height", post.filePreviewHeight);
            fileImg.setAttribute("data-preview-width", post.filePreviewWidth);
        }
        else {
            fileImg.setAttribute("height", post.filePreviewHeight);
            fileImg.setAttribute("width", post.filePreviewWidth);
            fileImg.setAttribute("src", `/boards/${post.boardId}/preview/${post.filePreviewName}`);
        }

        fileA.appendChild(fileImg);
        fileDiv.appendChild(fileA);

        content.appendChild(fileDiv);
    }

    let message = document.createElement("div");
    message.classList.add("message");

    if (post.message) {
        message.innerHTML = post.message;
    }

    content.appendChild(message);
    section.appendChild(content);

    var afterEvent = new CustomEvent("after-create-post-section-element-event", {
        detail: { post: post, element: section }
    });
    window.dispatchEvent(afterEvent);

    return section;
}


function sizeSuffix(value) {
    let sizeSuffixArray = ["B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB"];

    if (value < 0) {
        return `-${sizeSuffix(-value)}`;
    }
    if (value === 0) {
        return `0.00 ${sizeSuffixArray[0]}`;
    }

    let mag = parseInt(Math.log(value) / Math.log(1024), 10);
    let adjustedSize = value / (1 << (mag * 10));

    return `${adjustedSize.toFixed(2)} ${sizeSuffixArray[mag]}`;
}


function addMarkup(element, replace = false) {
    const textArea = document.querySelector("section.post textarea[name=message]");
    const startPosition = textArea.selectionStart;
    const endPosition = textArea.selectionEnd;
    const dataMarkup = element;

    if (replace) {
        if (startPosition === endPosition) {
            textArea.value = textArea.value.substring(0, startPosition) + dataMarkup + textArea.value.substring(endPosition, textArea.value.length);
            textArea.selectionStart = startPosition + (dataMarkup.length);
            textArea.selectionEnd = startPosition + (dataMarkup.length);
        } else {
            textArea.value = textArea.value.substring(0, startPosition) + dataMarkup + textArea.value.substring(endPosition, textArea.value.length);
            textArea.selectionStart = startPosition;
            textArea.selectionEnd = startPosition + (dataMarkup.length);
        }
    }
    else {
        if (startPosition === endPosition) {
            textArea.value = textArea.value.substring(0, startPosition) + dataMarkup + dataMarkup + textArea.value.substring(endPosition, textArea.value.length);
            textArea.selectionStart = startPosition;
            textArea.selectionEnd = endPosition;
        } else {
            textArea.value = textArea.value.substring(0, startPosition) + dataMarkup + textArea.value.substring(startPosition, endPosition) + dataMarkup + textArea.value.substring(endPosition, textArea.value.length);
            textArea.selectionStart = startPosition;
            textArea.selectionEnd = endPosition + (dataMarkup.length * 2);
        }
    }
    textArea.focus();
}

function createCheckboxSettingElement(name, checked, type, func) {
    var newSetting = document.createElement("div");
    newSetting.setAttribute("class", "setting");


    var settingH5 = document.createElement("h5");
    settingH5.textContent = name;
    newSetting.appendChild(settingH5);

    var setting = document.createElement("label");
    setting.classList.add("checkbox");

    var settingInput = document.createElement("input");
    settingInput.setAttribute("type", "checkbox");
    if (checked) {
        settingInput.checked = true;
    }
    settingInput.addEventListener(type, func);
    setting.appendChild(settingInput);

    var settingSpanTrue = document.createElement("span");
    settingSpanTrue.classList.add("true");
    settingSpanTrue.textContent = localization.yes;
    setting.appendChild(settingSpanTrue);

    var settingSpanFalse = document.createElement("span");
    settingSpanFalse.classList.add("false");
    settingSpanFalse.textContent = localization.no;
    setting.appendChild(settingSpanFalse);

    newSetting.appendChild(setting);

    return newSetting;
}

function createInputSettingElement(name, value, type, func) {
    var newSetting = document.createElement("div");
    newSetting.setAttribute("class", "setting");

    var settingH5 = document.createElement("h5");
    settingH5.textContent = name;
    newSetting.appendChild(settingH5);

    var settingInput = document.createElement("input");
    settingInput.setAttribute("type", "text");
    settingInput.value = value;
    settingInput.addEventListener(type, func);

    newSetting.appendChild(settingInput);

    return newSetting;
}

function createTextAreaSettingElement(name, value, type, func) {
    var newSetting = document.createElement("div");
    newSetting.setAttribute("class", "setting");

    var settingH5 = document.createElement("h5");
    settingH5.textContent = name;
    newSetting.appendChild(settingH5);

    var settingTextArea = document.createElement("textarea");
    settingTextArea.value = value;
    settingTextArea.addEventListener(type, func);

    newSetting.appendChild(settingTextArea);

    return newSetting;
}

function createProgressSettingElement(name, value, min, max, type, func) {
    var newSetting = document.createElement("div");
    newSetting.setAttribute("class", "setting");

    var settingH5 = document.createElement("h5");
    settingH5.textContent = name;
    newSetting.appendChild(settingH5);

    var settingProgress = document.createElement("progress");
    settingProgress.setAttribute("value", value);
    settingProgress.setAttribute("min", min);
    settingProgress.setAttribute("max", max);
    settingProgress.addEventListener(type, func);

    newSetting.appendChild(settingProgress);

    return newSetting;
}

function createInputRangeSettingElement(name, value, min, max, step, type, func) {
    var newSetting = document.createElement("div");
    newSetting.setAttribute("class", "setting");

    var settingH5 = document.createElement("h5");
    settingH5.textContent = name;
    newSetting.appendChild(settingH5);

    var settingInput = document.createElement("input");
    settingInput.type = "range";
    settingInput.min = min;
    settingInput.max = max;
    settingInput.step = step;
    settingInput.value = value;
    settingInput.addEventListener(type, func);

    newSetting.appendChild(settingInput);

    return newSetting;
}

