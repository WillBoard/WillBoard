document.addEventListener("DOMContentLoaded", function (event) {
    let submitInputElements = document.querySelectorAll("section.post input[type=submit]");
    for (let i = 0; i < submitInputElements.length; i++) {
        submitInputElements[i].addEventListener("click", postAsynchronous);
    }

    if (viewConfiguration.boardViewType === BoardViewType.ClassicThread) {
        var indexOfReplyNumber = window.location.href.lastIndexOf("#r");
        if (indexOfReplyNumber !== -1) {
            let replyNumber = window.location.href.substring(indexOfReplyNumber + 2);
            if (replyNumber != "") {
                addMarkup(`>>${replyNumber}\n`, true);
            }
        }
    }

    if (viewConfiguration.boardViewType === BoardViewType.ClassicThread ||
        (viewConfiguration.boardViewType === BoardViewType.ClassicBoard && ((fastReplyConfiguration != null && fastReplyConfiguration.availability === true) || (quickReplyConfiguration != null && quickReplyConfiguration.availability === true)))) {

        let replyHyperlinkElements = document.querySelectorAll("section > .info > a.reply");
        for (var i = 0; i < replyHyperlinkElements.length; i++) {
            replyHyperlinkElements[i].addEventListener("click", activateReply);
        }

        window.addEventListener("after-create-post-section-element-event", function (event) {
            let postSectionElement = event.detail.element;
            let replyHyperlinkElement = postSectionElement.querySelector("section > .info > a.reply");
            replyHyperlinkElement.addEventListener("click", activateReply);
        }, false);
    }
});

function postAsynchronous(event) {
    let submitInputElement = event.currentTarget;
    let postFormElement = submitInputElement.form;

    let validity = postFormElement.reportValidity();
    if (validity) {
        submitInputElement.disabled = true;
        let submitInputElementInitialValue = submitInputElement.value;

        let formData = new FormData(postFormElement);
        let xmlHttpRequest = new XMLHttpRequest();

        xmlHttpRequest.addEventListener("readystatechange", handleReadyStateChange.bind(this, submitInputElement, submitInputElementInitialValue), false);
        xmlHttpRequest.upload.addEventListener("progress", updateUploadProgress.bind(this, submitInputElement), false);

        let boardId = postFormElement.querySelector("input[name=boardId]").value;
        let path = getApiPostPath(boardId);

        xmlHttpRequest.open(postFormElement.method, path);
        xmlHttpRequest.setRequestHeader("Accept", "application/json");
        xmlHttpRequest.send(formData);
    }

    event.preventDefault();
}

function handleReadyStateChange(submitInputElement, submitInputElementInitialValue, event) {
    let xmlHttpRequest = event.currentTarget;
    let error = false;

    if (xmlHttpRequest.readyState === 4) {
        try {
            let response = JSON.parse(xmlHttpRequest.responseText);

            if (response.error != null) {
                if (response.error.message !== "" && response.error.code !== "") {
                    alert(`${response.error.message}\n(${response.error.code})`);
                    console.error(`${response.error.message} (${response.error.code}) with status ${response.error.status}`, response.error);
                }
                else {
                    alert(`Error with status ${response.error.status}`);
                    console.error(`Error with status ${response.error.status}`, response.error);
                }

                error = true;
                submitInputElement.value = submitInputElementInitialValue;

                if (quickReplyState === true) {
                    recalculateCloseQuickReply();
                }
            }

            if (response.data != null) {
                clearPostFormFields();

                if ((viewConfiguration.boardViewType === BoardViewType.ClassicBoard && synchronizationBoardConfiguration.availability === false) || (viewConfiguration.boardViewType === BoardViewType.ClassicThread && synchronizationThreadConfiguration.availability === false)) {
                    if (response.data.threadId == null) {
                        window.location.href = getBoardClassicThreadPath(response.data.boardId, response.data.postId);
                    }
                    else {
                        window.location.href = `${getBoardClassicThreadPath(response.data.boardId, response.data.threadId)}#${response.data.postId}`;

                        if (viewConfiguration.boardViewType === BoardViewType.ClassicThread) {
                            window.location.reload(true);
                        }
                    }
                }
                else {
                    if (viewConfiguration.boardViewType === BoardViewType.ClassicThread) {
                        window.scrollTo(0, document.body.scrollHeight);
                    }
                }

                switch (viewConfiguration.boardViewType) {
                    case BoardViewType.ClassicBoard:
                        submitInputElement.value = localization.newThread;
                        break;
                    case BoardViewType.ClassicThread:
                        submitInputElement.value = localization.reply;
                        break;
                }
            }
        }
        catch (error) {
            alert(`Unknown error with status ${xmlHttpRequest.status}`);
            console.error(`Unknown error with status ${xmlHttpRequest.status}`, xmlHttpRequest.responseText);

            error = true;
            submitInputElement.value = submitInputElementInitialValue;

            if (quickReplyState === true) {
                recalculateCloseQuickReply();
            }
        }

        submitInputElement.disabled = false;

        if (error === false && quickReplyState === true) {
            removeQuickReply();
        }
        else {
            chceckVerificationAsync();
        }

        switch (boardConfiguration.fieldVerificationType) {
            case VerificationType.ClassicCAPTCHA:
                resetClassicCaptcha();
                break;
            case VerificationType.ReCAPTCHA:
                grecaptcha.reset();
                break;
        }
    }
};

function updateUploadProgress(submitInputElement, event) {
    if (event.lengthComputable) {
        let progress = ((event.loaded / event.total) * 100).toFixed(0) + "%";
        submitInputElement.value = progress;
        if (quickReplyState === true) {
            document.querySelector("section.post form aside.close-quick").style.cssText = `right: ${submitInputElement.offsetWidth - 40}px;`;
        }
    }
}

function clearPostFormFields() {
    let fieldElemetCollection = document.querySelectorAll("section.post table tr[data-field]");
    for (let i = 0; i < fieldElemetCollection.length; i++) {
        if (fieldElemetCollection[i].getAttribute("data-field") !== "password" &&
            fieldElemetCollection[i].getAttribute("data-field") !== "options" &&
            fieldElemetCollection[i].getAttribute("data-field") !== "captcha") {
            let field = fieldElemetCollection[i].querySelector("input, textarea");
            if (field != null) {
                field.value = "";
            }
        }

        if (fieldElemetCollection[i].getAttribute("data-field") === "options") {
            let optionsArray = fieldElemetCollection[i].querySelectorAll("input[type=checkbox]");
            for (let z = 0; z < optionsArray.length; z++) {
                optionsArray[z].checked = false;
            }
        }
    }
}

function activateReply(event) {
    addMarkup(`>>${event.target.textContent}\n`, true);
    event.preventDefault();
}