let autoscrollThreadConfiguration = {
    availability: true
};

if (localStorage.getItem("autoscroll-thread") == null) {
    localStorage.setItem("autoscroll-thread", JSON.stringify(autoscrollThreadConfiguration));
}
else {
    autoscrollThreadConfiguration = JSON.parse(localStorage.getItem("autoscroll-thread"));
}

function updateAvailabilityAutoscrollThreadConfiguration(event) {
    autoscrollThreadConfiguration.availability = event.target.checked;
    localStorage.setItem("autoscroll-thread", JSON.stringify(autoscrollThreadConfiguration));
}

document.addEventListener("DOMContentLoaded", function (event) {
    document.querySelector("aside .settings").appendChild(createCheckboxSettingElement("Autoscroll thread when bottom", autoscrollThreadConfiguration.availability, "change", updateAvailabilityAutoscrollThreadConfiguration));

    if (autoscrollThreadConfiguration.availability !== true) {
        return;
    }

    if (!(viewConfiguration.boardViewType === BoardViewType.ClassicThread)) {
        return;
    }

    window.addEventListener("after-create-synchronization-event", function (event) {
        let post = event.detail;
        scrollThreadToBottom(post);
    }, false);
});

function scrollThreadToBottom(post) {
    let threadArticleElement = document.querySelector(`main article.thread[data-boardid='${post.boardId}'][data-threadid='${post.threadId}']`);

    if (threadArticleElement == null) {
        return;
    }

    let replySectionElement = threadArticleElement.querySelector(`section.reply[data-postid='${post.postId}']`);

    if (replySectionElement == null) {
        return;
    }

    let offsetWidth = replySectionElement.offsetWidth;
    let computedStyle = getComputedStyle(replySectionElement);
    let fullHeight = offsetWidth + parseInt(computedStyle.marginTop) + parseInt(computedStyle.marginBottom);
    if ((window.innerHeight + window.scrollY + fullHeight) >= document.body.querySelector("main").offsetHeight) {
        window.scrollTo(0, document.body.scrollHeight);
    }
}