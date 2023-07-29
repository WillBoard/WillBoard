let themeConfiguration = {
    value: "main"
};

if (localStorage.getItem("theme") == null) {
    localStorage.setItem("theme", JSON.stringify(themeConfiguration));
}
else {
    themeConfiguration = JSON.parse(localStorage.getItem("theme"));
    updateTheme(boardConfiguration.cssThemeCollection[themeConfiguration.value]);
}

function updateValueThemeConfiguration(event) {
    themeConfiguration.value = event.currentTarget.value;
    localStorage.setItem("theme", JSON.stringify(themeConfiguration));

    updateTheme(boardConfiguration.cssThemeCollection[themeConfiguration.value]);
}

document.addEventListener("DOMContentLoaded", function (event) {
    let themeSelectElement = document.createElement("select");
    for (theme in boardConfiguration.cssThemeCollection) {
        let themeOptionElement = document.createElement("option");
        themeOptionElement.setAttribute("value", theme);
        themeOptionElement.innerText = boardConfiguration.cssThemeCollection[theme].name;
        if (theme === themeConfiguration.value) {
            themeOptionElement.setAttribute('selected', true);
        }
        themeSelectElement.appendChild(themeOptionElement);
    }

    themeSelectElement.addEventListener("change", updateValueThemeConfiguration);

    let themeSettingElement = document.createElement("div");
    themeSettingElement.setAttribute("class", "setting");

    let h5Element = document.createElement("h5");
    h5Element.textContent = "Theme";

    themeSettingElement.appendChild(h5Element);
    themeSettingElement.appendChild(themeSelectElement);

    document.querySelector("aside .settings").appendChild(themeSettingElement);
});

function updateTheme(theme) {
    let currentThemeLinkElement = document.head.querySelector(`link[data-theme=true]`);

    if (theme.path === "") {
        if (currentThemeLinkElement != null) {
            currentThemeLinkElement.remove();
        }

        return;
    }

    if (currentThemeLinkElement != null) {
        currentThemeLinkElement.setAttribute("href", theme.path);
        return;
    }

    var themeLinkElement = document.createElement("link");
    themeLinkElement.setAttribute("href", theme.path);
    themeLinkElement.setAttribute("rel", "stylesheet");
    themeLinkElement.setAttribute("type", "text/css");
    themeLinkElement.setAttribute("data-theme", "true");
    document.head.appendChild(themeLinkElement);
}