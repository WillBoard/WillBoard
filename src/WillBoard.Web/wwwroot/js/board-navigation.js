document.addEventListener("DOMContentLoaded", function (event) {
    let panelLabelElements = document.querySelectorAll("body > nav > aside > label");
    for (let i = 0; i < panelLabelElements.length; i++) {
        panelLabelElements[i].addEventListener("click", closeOtherPanels);
    }

});

function closeOtherPanels(event) {
    let panelLabelElement = event.currentTarget;
    let forAttribute = panelLabelElement.getAttribute("for");
    let otherCheckedInputElements = document.querySelectorAll(`body > nav > aside > input[type=checkbox]:checked:not([id='${forAttribute}'])`);
    for (let i = 0; i < otherCheckedInputElements.length; i++) {
        otherCheckedInputElements[i].checked = false;
    }
}