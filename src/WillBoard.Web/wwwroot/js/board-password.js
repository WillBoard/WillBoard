let passwordConfiguration = {
    value: (Math.random() + 1).toString(36).substr(2, 10)
};

if (localStorage.getItem("passwordConfiguration") == null) {
    localStorage.setItem("passwordConfiguration", JSON.stringify(passwordConfiguration));
}
else {
    passwordConfiguration = JSON.parse(localStorage.getItem("passwordConfiguration"));
}

function updateValuePasswordConfiguration(event) {
    console.log(event);
    passwordConfiguration.value = event.currentTarget.value;
    localStorage.setItem("passwordConfiguration", JSON.stringify(passwordConfiguration));
    updatePasswordInputs();
}

document.addEventListener("DOMContentLoaded", function (event) {
    document.querySelector("aside .settings").appendChild(createInputSettingElement("Password", passwordConfiguration.value, "input", updateValuePasswordConfiguration));
    updatePasswordInputs();
});

function updatePasswordInputs() {
    var passwordInputElements = document.querySelectorAll(`input[name=password]`);
    for (var i = 0; i < passwordInputElements.length; i++) {
        passwordInputElements[i].value = passwordConfiguration.value;
    }
}