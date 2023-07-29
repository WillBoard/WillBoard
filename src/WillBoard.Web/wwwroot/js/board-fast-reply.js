let fastReplyConfiguration = {
    availability: true
};

let fastReplyState = false;

let formPositionX = 0;
let formPositionY = 0;

if (localStorage.getItem("fast-reply") == null) {
    localStorage.setItem("fast-reply", JSON.stringify(fastReplyConfiguration));
}
else {
    fastReplyConfiguration = JSON.parse(localStorage.getItem("fast-reply"));
}

function updateAvailabilityFastReplyConfiguration(event) {
    fastReplyConfiguration.availability = event.target.checked;
    localStorage.setItem("fast-reply", JSON.stringify(fastReplyConfiguration));
}

document.addEventListener("DOMContentLoaded", function (event) {
    document.querySelector("aside .settings").appendChild(createCheckboxSettingElement("Fast reply", fastReplyConfiguration.availability, "change", updateAvailabilityFastReplyConfiguration));

    if (fastReplyConfiguration.availability !== true) {
        return;
    }

    if (!(viewConfiguration.boardViewType === BoardViewType.ClassicBoard || viewConfiguration.boardViewType === BoardViewType.ClassicThread)) {
        return;
    }

    let replyHyperlinkElements = document.querySelectorAll("section > .info > a.reply");
    for (var i = 0; i < replyHyperlinkElements.length; i++) {
        replyHyperlinkElements[i].addEventListener("click", activateFastReply);
    }

    window.addEventListener("after-create-post-section-element-event", function (event) {
        let postSectionElement = event.detail.element;
        let replyHyperlinkElement = postSectionElement.querySelector("section > .info > a.reply");
        replyHyperlinkElement.addEventListener("click", activateFastReply);
    }, false);
});

function activateFastReply(event) {
    if (fastReplyState === false && window.pageYOffset > document.querySelector("header").offsetHeight) {
        fastReplyState = true;
        document.querySelector("body > header").setAttribute("style", `padding-top:${document.querySelector("section.post").offsetHeight + parseInt(getComputedStyle(document.querySelector("section.post")).marginTop) + parseInt(getComputedStyle(document.querySelector("section.post")).marginBottom)}px`);
        document.querySelector("section.post").classList.add("fixed");


        var form = document.querySelector("section.post form");
        var closeFastReply = document.createElement("aside");
        closeFastReply.classList.add("close-fast");
        closeFastReply.innerText = "";
        closeFastReply.addEventListener('click', removeFastReply);
        form.appendChild(closeFastReply);

        let moveFastReply = document.createElement("aside");
        moveFastReply.classList.add("move-fast");
        moveFastReply.innerText = "";
        moveFastReply.addEventListener("mousedown", formDragStart);
        moveFastReply.addEventListener("touchstart", formDragStart);
        form.appendChild(moveFastReply);

        document.addEventListener("scroll", chceckRemoveFastReply);
    }

    event.preventDefault();
}

function removeFastReply() {
    fastReplyState = false;
    document.querySelector("body > header").removeAttribute("style");
    document.querySelector("section.post").classList.remove("fixed");
    document.querySelector("section.post").style.top = null;
    document.querySelector("section.post").style.left = null;
    document.querySelector("section.post form").removeChild(document.querySelector("section.post form aside.close-fast"));
    document.querySelector("section.post form").removeChild(document.querySelector("section.post form aside.move-fast"));
    document.removeEventListener("scroll", chceckRemoveFastReply);
}

function chceckRemoveFastReply() {
    if (fastReplyState === true && window.pageYOffset < document.querySelector("header").offsetHeight) {
        removeFastReply();
    }
}

function formDragStart(event) {
    event.preventDefault();

    if (event.type === "mousedown") {
        formPositionX = event.clientX;
        formPositionY = event.clientY;
    }

    if (event.type === "touchstart") {
        formPositionX = event.changedTouches[0].clientX;
        formPositionY = event.changedTouches[0].clientY;
    }

    document.addEventListener("mouseup", formDragEnd);
    document.addEventListener("touchend", formDragEnd);
    document.addEventListener("touchcancel", formDragEnd);
    document.addEventListener("mousemove", formDragMove);
    document.addEventListener("touchmove", formDragMove);
}

function formDragMove(event) {
    let form = document.querySelector("body > header > section.post");

    let newFormPositionX = 0;
    let newFormPositionY = 0;


    if (event.type === "mousemove") {
        event.preventDefault();

        newFormPositionX = formPositionX - event.clientX;
        newFormPositionY = formPositionY - event.clientY;
        formPositionX = event.clientX;
        formPositionY = event.clientY;
    }

    if (event.type === "touchmove") {
        newFormPositionX = formPositionX - event.touches[0].clientX;
        newFormPositionY = formPositionY - event.touches[0].clientY;
        formPositionX = event.changedTouches[0].clientX;
        formPositionY = event.changedTouches[0].clientY;
    }

    form.style.top = (form.offsetTop - newFormPositionY) + "px";
    form.style.left = (form.offsetLeft - newFormPositionX) + "px";
}

function formDragEnd() {
    document.removeEventListener("mouseup", formDragEnd);
    document.removeEventListener("touchend", formDragEnd);
    document.removeEventListener("touchcancel", formDragEnd);
    document.removeEventListener("mousemove", formDragMove);
    document.removeEventListener("touchmove", formDragMove);
}