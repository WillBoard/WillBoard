let classicCaptchaStart;
let classicCaptchaEnd;
let classicCaptchaTimerInterval;

document.addEventListener("DOMContentLoaded", function (event) {
    if (!(viewConfiguration.boardViewType === BoardViewType.ClassicBoard || viewConfiguration.boardViewType === BoardViewType.ClassicThread)) {
        return;
    }

    addClassicCaptcha();
});

function addClassicCaptcha() {
    let captchaTdElement = document.querySelector("section.post tr.captcha td");

    let captchaKeyInputElement = document.createElement("input");
    captchaKeyInputElement.setAttribute("type", "hidden");
    captchaKeyInputElement.setAttribute("name", "verificationKey");
    captchaKeyInputElement.setAttribute("autocomplete", "off");

    let captchaLabelElement = document.createElement("label");
    captchaLabelElement.setAttribute("for", "verificationValue");
    captchaLabelElement.textContent = "";
    captchaLabelElement.addEventListener("click", realoadClassicCaptchaAsync);

    let captchaProgressElement = document.createElement("progress");
    captchaProgressElement.setAttribute("value", "0");
    captchaProgressElement.setAttribute("max", "300");

    let captchaValueInputElement = document.createElement("input");
    captchaValueInputElement.setAttribute("type", "text");
    captchaValueInputElement.setAttribute("name", "verificationValue");
    captchaValueInputElement.setAttribute("id", "verificationValue");
    captchaValueInputElement.setAttribute("autocomplete", "off");
    captchaValueInputElement.setAttribute("placeholder", "Captcha");

    captchaTdElement.appendChild(captchaKeyInputElement);
    captchaTdElement.appendChild(captchaLabelElement);
    captchaTdElement.appendChild(captchaProgressElement);
    captchaTdElement.appendChild(captchaValueInputElement);
}

function resetClassicCaptcha() {
    stopClassicCaptchaTimer();

    let captchaLabelElement = document.querySelector("section.post .captcha label");
    captchaLabelElement.style.color = "";
    captchaLabelElement.style.backgroundImage = "";
    document.querySelector("section.post .captcha input[name='verificationValue']").value = "";
}

async function realoadClassicCaptchaAsync() {
    let data = await getClassicCaptchaAsync();

    if (data == null) {
        return;
    }

    let captchaLabelElement = document.querySelector("section.post .captcha label");
    captchaLabelElement.style.color = "transparent";
    captchaLabelElement.style.backgroundImage = `url(${data.captchaImage})`;
    document.querySelector("section.post .captcha input[name='verificationValue']").value = "";
    document.querySelector("section.post .captcha input[name='verificationKey']").value = data.captchaKey;

    classicCaptchaEnd = new Date(data.captchaEnd);

    stopClassicCaptchaTimer();
    startClassicCaptchaTimer();
}

async function getClassicCaptchaAsync() {
    try {
        let fetchResponse = await fetch(`/api/captcha`, {
            cache: 'no-cache',
            headers: {
                'accept': 'application/json'
            },
            credentials: "same-origin",
            method: 'GET'
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

function startClassicCaptchaTimer() {
    classicCaptchaTimer();
    classicCaptchaTimerInterval = setInterval(classicCaptchaTimer, 1000);
}

function stopClassicCaptchaTimer() {
    let captchaProgressElement = document.querySelector(".captcha progress");
    captchaProgressElement.value = 0;

    clearInterval(classicCaptchaTimerInterval);
}

function classicCaptchaTimer() {
    let captchaProgressElement = document.querySelector(".captcha progress");

    let now = new Date().getTime();
    let distance = classicCaptchaEnd.getTime() - now;
    let seconds = Math.floor(distance / 1000);
    captchaProgressElement.value = seconds;

    if (distance < 0) {
        resetClassicCaptcha();
    }
}