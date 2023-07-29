let filePreviewConfiguration = {
    gifAnimationAvailability: false,
    hidden: {}
};

if (localStorage.getItem("file-preview") == null) {
    localStorage.setItem("file-preview", JSON.stringify(filePreviewConfiguration));
}
else {
    filePreviewConfiguration = JSON.parse(localStorage.getItem("file-preview"));
}

function updateGifAnimationAvailability(event) {
    filePreviewConfiguration.gifAnimationAvailability = event.target.checked;
    localStorage.setItem("file-preview", JSON.stringify(filePreviewConfiguration));
}

document.addEventListener("DOMContentLoaded", function (event) {
    document.querySelector("aside .settings").appendChild(createCheckboxSettingElement("Animated GIFs (source)", filePreviewConfiguration.gifAnimationAvailability, "change", updateGifAnimationAvailability));

    let filePreviewDivElements = document.querySelectorAll(".file[data-mime]");
    for (let i = 0; i < filePreviewDivElements.length; i++) {
        addAndEnableHideFilePreviewOption(filePreviewDivElements[i]);

        if (filePreviewConfiguration.gifAnimationAvailability === true) {
            let mimeAttribute = filePreviewDivElements[i].getAttribute("data-mime");
            let spoilerAttribute = filePreviewDivElements[i].getAttribute("data-spoiler");
            if (mimeAttribute === "image/gif" && spoilerAttribute == "false") {
                enableFilePreviewGifAnimation(filePreviewDivElements[i]);
            }
        }
    }

    window.addEventListener("after-create-post-section-element-event", function (event) {
        let postSectionElement = event.detail.element;
        let filePreviewDivElement = postSectionElement.querySelector(".file[data-mime]");

        if (filePreviewDivElement == null) {
            return;
        }

        addAndEnableHideFilePreviewOption(filePreviewDivElement);

        if (filePreviewConfiguration.gifAnimationAvailability === true) {
            let mimeAttribute = filePreviewDivElement.getAttribute("data-mime");
            let spoilerAttribute = filePreviewDivElement.getAttribute("data-spoiler");
            if (mimeAttribute === "image/gif" && spoilerAttribute == "false") {
                enableFilePreviewGifAnimation(filePreviewDivElement);
            }
        }
    }, false);
});

function addAndEnableHideFilePreviewOption(filePreviewDivElement) {
    let postSectionElement = filePreviewDivElement.closest("section.thread, section.reply");
    let optionsAsideElement = postSectionElement.querySelector("aside.options");
    let boardId = postSectionElement.getAttribute("data-boardid");
    let postId = postSectionElement.getAttribute("data-postid");

    if (filePreviewDivElement.querySelector("img, video") != null) {
        let hidden = false;
        if (filePreviewConfiguration.hidden[boardId] != null) {
            if (filePreviewConfiguration.hidden[boardId][postId] != null) {
                filePreviewDivElement.classList.add("hide");
                filePreviewConfiguration.hidden[boardId][postId] = Date.now();
                hidden = true;
            }
        }

        if (!(viewConfiguration.boardViewType === BoardViewType.ClassicBoard || viewConfiguration.boardViewType === BoardViewType.ClassicThread)) {
            return;
        }

        let hideFilePreviewOptionHyperlinkElement = document.createElement("a");
        hideFilePreviewOptionHyperlinkElement.innerText = hidden ? localization.unhidePreview : localization.hidePreview;
        hideFilePreviewOptionHyperlinkElement.classList.add("file-preview");
        hideFilePreviewOptionHyperlinkElement.addEventListener('click', toggleHideFilePreview);
        optionsAsideElement.appendChild(hideFilePreviewOptionHyperlinkElement);
    }
}

function toggleHideFilePreview(event) {
    let hideFilePreviewOptionHyperlinkElement = event.currentTarget;
    let postSectionElement = hideFilePreviewOptionHyperlinkElement.closest("section.thread, section.reply");
    let boardId = postSectionElement.getAttribute("data-boardid");
    let postId = postSectionElement.getAttribute("data-postid");

    let fileDivElement = postSectionElement.querySelector(".file[data-mime]");

    if (fileDivElement.classList.contains("hide")) {
        fileDivElement.classList.remove("hide");

        filePreviewConfiguration.hidden[boardId][postId] = undefined;
        localStorage.setItem("file-preview", JSON.stringify(filePreviewConfiguration));

        hideFilePreviewOptionHyperlinkElement.innerText = localization.hidePreview;
    }
    else {
        fileDivElement.classList.add("hide");

        if (filePreviewConfiguration.hidden[boardId] == null) {
            filePreviewConfiguration.hidden[boardId] = {};
        }

        filePreviewConfiguration.hidden[boardId][postId] = Date.now();
        localStorage.setItem("file-preview", JSON.stringify(filePreviewConfiguration));

        hideFilePreviewOptionHyperlinkElement.innerText = localization.unhidePreview;
    }
}

function enableFilePreviewGifAnimation(element) {
    element.firstElementChild.firstElementChild.src = element.firstElementChild.getAttribute("href");
}