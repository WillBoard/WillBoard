const AccountType = {
    User: 0,
    Administrator: 1
};

const IpVersion = {
    None: 0,
    IpVersion4: 1,
    IpVersion6: 2
};

function getBoardClassicPath(boardId) {
    return `/administration/board/${boardId}/view`;
}

function getBoardClassicThreadPath(boardId, threadId, last = 0) {
    if (last > 0) {
        return `/administration/board/${boardId}/view/thread/${threadId}/${last}`;
    }

    return `/administration/board/${boardId}/view/thread/${threadId}`;
}

function getApiPostsPath(boardId, postId) {
    return `/administration/api/${boardId}/posts/${postId}`;
}

function getApiRepliesPath(boardId, threadId, last = 0) {
    if (last > 0) {
        return `/administration/api/${boardId}/replies/${threadId}/${last}`;
    }

    return `/administration/api/${boardId}/replies/${threadId}`;
}

function getApiSynchronizationPath(boardId) {
    return `/administration/api/${boardId}/synchronization`;
}

document.addEventListener("DOMContentLoaded", function (event) {
    window.addEventListener("after-create-post-section-element-event", function (event) {
        let post = event.detail.post;
        let postSectionElement = event.detail.element;
        let infoDivElement = postSectionElement.querySelector("div.info");
        let optionsAsideElement = infoDivElement.querySelector("aside.options");
        let messageDivElement = postSectionElement.querySelector("div.message");

        let ipHyperlinkElement = document.createElement("a");
        ipHyperlinkElement.classList.add("ip");
        ipHyperlinkElement.setAttribute("title", post.userAgent);
        ipHyperlinkElement.setAttribute("href", `/administration/board/${post.boardId}/ip/${post.ipVersion}/${post.ipNumber}`);
        ipHyperlinkElement.textContent = `${ipNumberToIpAddressString(post.ipVersion, post.ipNumber)}`;
        optionsAsideElement.after(ipHyperlinkElement);

        let checkboxInputElement = document.createElement("input");
        checkboxInputElement.setAttribute("type", "checkbox");
        checkboxInputElement.setAttribute("id", `administration-${post.boardId}${post.postId}`);
        ipHyperlinkElement.after(checkboxInputElement);

        let labelElement = document.createElement("label");
        labelElement.setAttribute("for", `administration-${post.boardId}${post.postId}`);
        labelElement.textContent = "";
        checkboxInputElement.after(labelElement);

        let administrationAsideElement = document.createElement("aside");
        administrationAsideElement.classList.add("administration");

        if (authorization.permissionBanCreate === true) {
            let boardBanCreateHyperlinkElement = document.createElement("a");
            boardBanCreateHyperlinkElement.setAttribute("href", `/administration/board/${post.boardId}/ban/create?ipVersion=${post.ipVersion}&ipNumber=${post.ipNumber}`);
            boardBanCreateHyperlinkElement.textContent = "Board - Ban - Create";
            administrationAsideElement.appendChild(boardBanCreateHyperlinkElement);
        }

        if (authorization.permissionPostDelete === true) {
            let boardPostDeleteHyperlinkElement = document.createElement("a");
            boardPostDeleteHyperlinkElement.setAttribute("href", `/administration/board/${post.boardId}/post/${post.postId}/delete`);
            boardPostDeleteHyperlinkElement.textContent = "Board - Post - Delete";
            administrationAsideElement.appendChild(boardPostDeleteHyperlinkElement);
        }

        if (post.file === true && post.fileDeleted === false && authorization.permissionPostDeleteFile === true) {
            let boardPostDeleteFileHyperlinkElement = document.createElement("a");
            boardPostDeleteFileHyperlinkElement.setAttribute("href", `/administration/board/${post.boardId}/post/${post.postId}/delete-file`);
            boardPostDeleteFileHyperlinkElement.textContent = "Board - Post Delete - File";
            administrationAsideElement.appendChild(boardPostDeleteFileHyperlinkElement);
        }

        if (authorization.permissionPostEdit === true) {
            let boardPostUpdateHyperlinkElement = document.createElement("a");
            boardPostUpdateHyperlinkElement.setAttribute("href", `/administration/board/${post.boardId}/post/${post.postId}/update`);
            boardPostUpdateHyperlinkElement.textContent = "Board - Post - Update";
            administrationAsideElement.appendChild(boardPostUpdateHyperlinkElement);

            if (post.threadId == null) {
                let boardThreadBumpLockHyperlinkElement = document.createElement("a");
                boardThreadBumpLockHyperlinkElement.setAttribute("href", `/administration/board/${post.boardId}/thread/${post.postId}/bump-lock`);
                boardThreadBumpLockHyperlinkElement.textContent = "Board - Thread - Bump Lock";
                administrationAsideElement.appendChild(boardThreadBumpLockHyperlinkElement);

                let boardThreadReplyLockHyperlinkElement = document.createElement("a");
                boardThreadReplyLockHyperlinkElement.setAttribute("href", `/administration/board/${post.boardId}/thread/${post.postId}/reply-lock`);
                boardThreadReplyLockHyperlinkElement.textContent = "Board - Thread - Reply Lock";
                administrationAsideElement.appendChild(boardThreadReplyLockHyperlinkElement);

                let boardThreadPinHyperlinkElement = document.createElement("a");
                boardThreadPinHyperlinkElement.setAttribute("href", `/administration/board/${post.boardId}/thread/${post.postId}/pin`);
                boardThreadPinHyperlinkElement.textContent = "Board - Thread - Pin";
                administrationAsideElement.appendChild(boardThreadPinHyperlinkElement);

                let boardThreadExcessiveHyperlinkElement = document.createElement("a");
                boardThreadExcessiveHyperlinkElement.setAttribute("href", `/administration/board/${post.boardId}/thread/${post.postId}/excessive`);
                boardThreadExcessiveHyperlinkElement.textContent = "Board - Thread - Excessive";
                administrationAsideElement.appendChild(boardThreadExcessiveHyperlinkElement);

                let boardThreadCopyHyperlinkElement = document.createElement("a");
                boardThreadCopyHyperlinkElement.setAttribute("href", `/administration/board/${post.boardId}/thread/${post.postId}/copy`);
                boardThreadCopyHyperlinkElement.textContent = "Board Thread - Copy";
                administrationAsideElement.appendChild(boardThreadCopyHyperlinkElement);
            }
        }

        if (authorization.permissionIpDeletePosts === true) {
            let boardIpDeletePostsHyperlinkElement = document.createElement("a");
            boardIpDeletePostsHyperlinkElement.setAttribute("href", `/administration/board/${post.boardId}/ip/${post.ipVersion}/${post.ipNumber}/delete-posts`);
            boardIpDeletePostsHyperlinkElement.textContent = "Board - Ip - Delete Posts";
            administrationAsideElement.appendChild(boardIpDeletePostsHyperlinkElement);
        }

        if (account.type === AccountType.Administrator) {
            let banCreateHyperlinkElement = document.createElement("a");
            banCreateHyperlinkElement.setAttribute("href", `/administration/ban/create?ipVersion=${post.ipVersion}&ipNumber=${post.ipNumber}`);
            banCreateHyperlinkElement.textContent = "Ban - Create";
            administrationAsideElement.appendChild(banCreateHyperlinkElement);
        }

        labelElement.after(administrationAsideElement);
    }, false);
});

function ipVersion4NumberToIpVersion4AddressString(ipNumber) {
    return parseInt((ipNumber / 16777216n) % 256n) + "." + parseInt((ipNumber / 65536n) % 256n) + "." + parseInt((ipNumber / 256n) % 256n) + "." + parseInt(ipNumber % 256n);
}

function ipVersion6NumberToIpVersion6AddressString(ipNumber) {
    return (parseInt(ipNumber / (65536n ** 7n) % 65536n)).toString(16) + ":" + (parseInt(ipNumber / (65536n ** 6n) % 65536n)).toString(16) + ":" + (parseInt(ipNumber / (65536n ** 5n) % 65536n)).toString(16) + ":" + (parseInt(ipNumber / (65536n ** 4n) % 65536n)).toString(16) + ":" + (parseInt(ipNumber / (65536n ** 3n) % 65536n)).toString(16) + ":" + (parseInt(ipNumber / (65536n ** 2n) % 65536n)).toString(16) + ":" + (parseInt(ipNumber / 65536n % 65536n)).toString(16) + ":" + (parseInt(ipNumber % 65536n)).toString(16);
}

function ipNumberToIpAddressString(ipVersion, ipNumber) {
    switch (ipVersion) {
        case IpVersion.IpVersion4:
            return ipVersion4NumberToIpVersion4AddressString(ipNumber);
        case IpVersion.IpVersion6:
            return ipVersion6NumberToIpVersion6AddressString(ipNumber);
        default:
            return "-";
    }
}