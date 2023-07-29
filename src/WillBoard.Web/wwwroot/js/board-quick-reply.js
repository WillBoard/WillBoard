let quickReplyConfiguration = {
    availability: true
};

let quickReplyState = false;

if (localStorage.getItem("quick-reply") == null) {
    localStorage.setItem("quick-reply", JSON.stringify(quickReplyConfiguration));
}
else {
    quickReplyConfiguration = JSON.parse(localStorage.getItem("quick-reply"));
}

function updateAvailabilityQuickReplyConfiguration(event) {
    quickReplyConfiguration.availability = event.target.checked;
    localStorage.setItem("quick-reply", JSON.stringify(quickReplyConfiguration));
}

document.addEventListener("DOMContentLoaded", function (event) {
    document.querySelector("aside .settings").appendChild(createCheckboxSettingElement("Quick reply", quickReplyConfiguration.availability, "change", updateAvailabilityQuickReplyConfiguration));

    if (quickReplyConfiguration.availability !== true) {
        return;
    }

    if (!(viewConfiguration.boardViewType === BoardViewType.ClassicBoard)) {
        return;
    }

    let replyHyperlinkElements = document.querySelectorAll("section > .info > a.reply");
    for (var i = 0; i < replyHyperlinkElements.length; i++) {
        replyHyperlinkElements[i].addEventListener("click", activateQuickReply);
    }

    window.addEventListener("after-create-post-section-element-event", function (event) {
        let postSectionElement = event.detail.element;
        let replyHyperlinkElement = postSectionElement.querySelector("section > .info > a.reply");
        replyHyperlinkElement.addEventListener("click", activateQuickReply);
    }, false);
});

function recalculateCloseQuickReply() {
    var postFormElement = document.querySelector("section.post form");
    var submitInputElement = postFormElement.querySelector("input[type=submit]");

    if (submitInputElement == null) {
        return;
    }

    var closeQuickReplyAsideElement = postFormElement.querySelector("aside.close-quick");

    if (closeQuickReplyAsideElement == null) {
        return;
    }

    closeQuickReplyAsideElement.style.cssText = `right: ${submitInputElement.offsetWidth - 40}px;`;
}

function activateQuickReply(event) {
    let threadArticleElement = event.target.closest("article.thread");
    let boardId = threadArticleElement.getAttribute("data-boardid");
    let threadId = threadArticleElement.getAttribute("data-threadid");

    if (quickReplyState === true) {
        removeQuickReply(false);
        activateQuickReply(event);
        return;
    }

    quickReplyState = true;

    var postForm = document.querySelector("section.post form");
    var postButton = document.querySelector("section.post input[type=submit]");

    postButton.classList.add("fast");
    postButton.value = `${localization.replyIn} ${boardId}/${threadId}`;

    document.querySelector("section.post form input[name=threadId]").setAttribute("value", threadId);
    document.querySelector("section.post form input[name=boardId]").setAttribute("value", boardId);

    var closeQuickReply = document.createElement("aside");
    closeQuickReply.classList.add("close-quick");
    closeQuickReply.style.cssText = `right: ${postButton.offsetWidth - 40}px;`;
    closeQuickReply.innerText = "";
    closeQuickReply.addEventListener("click", removeQuickReply);

    postForm.appendChild(closeQuickReply);

    chceckVerificationAsync();
    updatePostForm();

    event.preventDefault();
}

function removeQuickReply(verification = true) {
    quickReplyState = false;
    document.querySelector("section.post input[type=submit]").value = localization.newThread;
    document.querySelector("section.post input[type=submit]").classList.remove("fast");
    document.querySelector("section.post form").removeChild(document.querySelector("section.post form aside.close-quick"));
    document.querySelector("section.post form input[name=threadId]").removeAttribute("value");
    document.querySelector("section.post form input[name=boardId]").setAttribute("value", boardConfiguration.boardId);

    if (verification) {
        chceckVerificationAsync();
    }
    updatePostForm();
}

async function chceckVerificationAsync() {
    var boardId = document.querySelector("section.post form input[name=boardId]").getAttribute("value");
    var thread = true;

    if (document.querySelector("section.post form input[name=threadId]").getAttribute("value") !== null) {
        thread = false;
    }

    let verification = await getVerificationAsync(boardId, thread);

    if (verification === true) {
        if (document.querySelector("section.post form tr.captcha.hide")) {
            document.querySelector("section.post form tr.captcha").classList.remove("hide");
        }
    }
    else {
        if (document.querySelector("section.post form tr.captcha")) {
            document.querySelector("section.post form tr.captcha").classList.add("hide");
        }
    }
}

async function updatePostForm() {
    let postForm = document.querySelector("section.post form");
    let postFormTable = postForm.querySelector("table tbody");

    let reply = document.querySelector("section.post form input[name=threadId]").getAttribute("value") !== null;

    let fieldCollection = boardConfiguration.fieldCollection[reply ? "reply" : "thread"];

    for (let i = 0; i < fieldCollection.length; i++) {
        let tr = postFormTable.querySelector(`tr[data-field='${fieldCollection[i].name}']`);
        if (fieldCollection[i].availability === true) {
            if (tr == null) {
                postFormTable.insertAdjacentElement("afterbegin", createField(fieldCollection[i]));
            }
            else {
                updateField(fieldCollection[i], tr);
            }
        }
        else {
            if (tr != null) {
                tr.remove();
            }
        }
    }
}

function updateField(field, node) {
    if (field.name === "subject" || field.name === "name" || field.name === "email" || field.name === "password") {
        let input = node.querySelector("input");
        input.setAttribute("maxlength", field.lengthMax);
        input.setAttribute("minlength", field.lengthMin);
        if (input.hasAttribute("required")) {
            if (!field.requirement) {
                input.removeAttribute("required");
            }
        }
        else {
            if (field.requirement) {
                input.setAttribute("required", "");
            }
        }
    }
    if (field.name === "message") {
        let input = node.querySelector("textarea");
        input.setAttribute("maxlength", field.lengthMax);
        input.setAttribute("minlength", field.lengthMin);
        if (input.hasAttribute("required")) {
            if (!field.requirement) {
                input.removeAttribute("required");
            }
        }
        else {
            if (field.requirement) {
                input.setAttribute("required", "");
            }
        }
    }
    if (field.name === "file") {
        let input = node.querySelector("input");
        input.setAttribute("accept", field.typeCollection);
        if (input.hasAttribute("required")) {
            if (!field.requirement) {
                input.removeAttribute("required");
            }
        }
        else {
            if (field.requirement) {
                input.setAttribute("required", "");
            }
        }

    }
    if (field.name === "captcha") {
    }
}

function createField(field) {
    let tr = document.createElement("tr");
    tr.setAttribute("data-field", field.name);

    if (field.name === "subject" || field.name === "name" || field.name === "email" || field.name === "password") {
        let th = document.createElement("th");
        let label = document.createElement("label");
        label.setAttribute("for", field.name);
        label.textContent = localization[field.name];
        th.appendChild(label);
        tr.appendChild(th);

        let td = document.createElement("td");
        let input = document.createElement("input");
        input.type = field.name === "password" ? "password" : "text";
        input.name = field.name;
        input.id = field.name;
        input.maxlength = field.lengthMax;
        input.minlength = field.lengthMin;
        input.required = field.requirement;
        td.appendChild(input);
        tr.appendChild(td);
    }
    if (field.name === "message") {
        let td = document.createElement("th");
        td.setAttribute("colspan", "2");
        let textarea = document.createElement("textarea");
        textarea.setAttribute("name", field.name);
        textarea.setAttribute("placeholder", localization[field.name]);
        textarea.setAttribute("minlength", field.lengthMin);
        textarea.setAttribute("maxlength", field.lengthMax);
        textarea.required = field.requirement;
        td.appendChild(textarea);
        tr.appendChild(td);
    }
    if (field.name === "file") {
        let td = document.createElement("th");
        td.setAttribute("colspan", "2");
        let input = document.createElement("input");
        input.setAttribute("type", field.name);
        input.setAttribute("name", field.name);
        input.setAttribute("id", field.name);
        input.setAttribute("accept", field.typeCollection);
        input.required = field.requirement;
        td.appendChild(input);
        tr.appendChild(td);
    }
    if (field.name === "captcha") {

    }

    return tr;
}