let customCssConfiguration = {
    value: ""
};

let customJsConfiguration = {
    value: ""
};

if (localStorage.getItem("custom-css") == null) {
    localStorage.setItem("custom-css", JSON.stringify(customCssConfiguration));
}
else {
    customCssConfiguration = JSON.parse(localStorage.getItem("custom-css"));
}

if (localStorage.getItem("custom-js") == null) {
    localStorage.setItem("custom-js", JSON.stringify(customJsConfiguration));
}
else {
    customJsConfiguration = JSON.parse(localStorage.getItem("custom-js"));
}

function updateValueCustomCssConfiguration(event) {
    customCssConfiguration.value = event.currentTarget.value;
    localStorage.setItem("custom-css", JSON.stringify(customCssConfiguration));
    document.querySelector("body style[data-custom=css]").innerHTML = customCssConfiguration.value;
}

function updateValueCustomJsConfiguration(event) {
    customJsConfiguration.value = event.currentTarget.value;
    localStorage.setItem("custom-js", JSON.stringify(customJsConfiguration));
    document.querySelector("body script[data-custom=js]").innerHTML = customJsConfiguration.value;
}

document.addEventListener("DOMContentLoaded", function (event) {
    document.querySelector("aside .settings").appendChild(createTextAreaSettingElement("Custom CSS", customCssConfiguration.value, "input", updateValueCustomCssConfiguration));
    document.querySelector("aside .settings").appendChild(createTextAreaSettingElement("Custom JS", customJsConfiguration.value, "input", updateValueCustomJsConfiguration));

    let customStyleElement = document.createElement("style");
    customStyleElement.setAttribute("data-custom", "css");
    customStyleElement.innerHTML = customCssConfiguration.value;
    document.querySelector("body").appendChild(customStyleElement);

    let customScriptElement = document.createElement("script");
    customScriptElement.setAttribute("data-custom", "js");
    customScriptElement.innerHTML = customJsConfiguration.value;
    document.querySelector("body").appendChild(customScriptElement);
});