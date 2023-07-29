document.addEventListener("DOMContentLoaded", function (event) {
    if (viewConfiguration.boardViewType === BoardViewType.ClassicBoard || viewConfiguration.boardViewType === BoardViewType.ClassicThread) {
        let postSectionElements = document.querySelectorAll("section.thread, section.reply");
        for (var i = 0; i < postSectionElements.length; i++) {
            addLongContent(postSectionElements[i]);
        }
    }

    window.addEventListener("after-create-post-section-element-event", function (event) {
        let postSectionElement = event.detail.element;

        // Temporary adding and removing postSectionElement in DOM for calculation of height
        postSectionElement.style.visibility = "hidden";
        let temporaryPostSectionElement = document.body.querySelector("main").appendChild(postSectionElement);
        
        addLongContent(temporaryPostSectionElement);

        temporaryPostSectionElement.remove();
        temporaryPostSectionElement.style.visibility = null;
    }, false);
});

function addLongContent(postSectionElement) {
    let contentDivElement = postSectionElement.querySelector("div.content");
    let contentDivElementBoundingClientRect = contentDivElement.getBoundingClientRect();
    let contentHeight = contentDivElementBoundingClientRect.height;

    if (contentHeight > 448) {
        if (postSectionElement.hasAttribute("data-long-content-expanded")) {
            return;
        }
        
        postSectionElement.setAttribute("data-long-content-expanded", false);

        let contnentLongLabelElement = document.createElement("div");
        contnentLongLabelElement.classList.add("long-content-omitted");

        let textSpanElement = document.createElement("span");
        textSpanElement.innerText = `${localization.longContentOmitted}.`;
        contnentLongLabelElement.appendChild(textSpanElement);

        let showHyperlinkElement = document.createElement("a");
        showHyperlinkElement.innerText = localization.show;
        showHyperlinkElement.addEventListener('click', toggleLongContent);
        contnentLongLabelElement.appendChild(showHyperlinkElement);

        contentDivElement.after(contnentLongLabelElement);
    }
}

function toggleLongContent(event) {
    let postSectionElement = event.target.closest("section.thread, section.reply");
    let longContentExpandedAttribute = postSectionElement.getAttribute("data-long-content-expanded");
    let textSpanElement = postSectionElement.querySelector("div.long-content-omitted span");
    let showHyperlinkElement = postSectionElement.querySelector("div.long-content-omitted a");

    if (longContentExpandedAttribute === "false") {
        postSectionElement.setAttribute("data-long-content-expanded", true);
        textSpanElement.textContent = `${localization.longContentExpanded}.`;
        showHyperlinkElement.textContent = localization.hide;
    }
    else if (longContentExpandedAttribute === "true") {
        postSectionElement.setAttribute("data-long-content-expanded", false);
        textSpanElement.textContent = `${localization.longContentOmitted}.`;
        showHyperlinkElement.textContent = localization.show;
    }
}