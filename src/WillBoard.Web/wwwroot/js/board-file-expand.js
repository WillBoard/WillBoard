let fileExpandConfiguration = {
    volume: 1,
    loop: false
};

if (localStorage.getItem("file-expand") == null) {
    localStorage.setItem("file-expand", JSON.stringify(fileExpandConfiguration));
}
else {
    fileExpandConfiguration = JSON.parse(localStorage.getItem("file-expand"));
}

function updatePlayerVolumeConfiguration(event) {
    fileExpandConfiguration.volume = event.currentTarget.value;
    localStorage.setItem("file-expand", JSON.stringify(fileExpandConfiguration));
}

function updatePlayerLoopConfiguration(event) {
    fileExpandConfiguration.loop = event.target.checked;
    localStorage.setItem("file-expand", JSON.stringify(fileExpandConfiguration));
}

document.addEventListener("DOMContentLoaded", function (e) {
    document.querySelector("aside .settings").appendChild(createInputRangeSettingElement("Player volume", fileExpandConfiguration.volume, 0, 1, 0.01, "change", updatePlayerVolumeConfiguration));
    document.querySelector("aside .settings").appendChild(createCheckboxSettingElement("Player loop", fileExpandConfiguration.loop, "change", updatePlayerLoopConfiguration));

    let fileDivElements = document.querySelectorAll(".file[data-mime]");
    for (let i = 0; i < fileDivElements.length; i++) {
        fileDivElements[i].addEventListener('click', fileExpand);
    }

    window.addEventListener("after-create-post-section-element-event", function (event) {
        let postSectionElement = event.detail.element;
        let fileDivElement = postSectionElement.querySelector(".file[data-mime]");

        if (fileDivElement == null) {
            return;
        }

        fileDivElement.addEventListener('click', fileExpand);
    }, false);
});

function fileExpand(event) {
    event.preventDefault();
    let src = event.currentTarget.querySelector("a").getAttribute("href");
    let mime = event.currentTarget.getAttribute("data-mime");

    const image = ["image/jpeg", "image/png", "image/gif", "image/svg+xml"];
    const audio = ["audio/webm", "audio/ogg", "audio/mpeg"];
    const video = ["video/webm", "video/ogg", "video/mp4"];

    let fileExpandDivElement = document.createElement("div");
    fileExpandDivElement.classList.add("file-expand");
    fileExpandDivElement.addEventListener('click', closeFileExpand);

    let figureElement = document.createElement("figure");

    let infoDivElement = document.createElement("div");
    infoDivElement.classList.add("header");

    let nameHyperlinkElement = document.createElement("a");
    nameHyperlinkElement.textContent = mime;
    infoDivElement.appendChild(nameHyperlinkElement);

    let closeButtonElement = document.createElement("button");
    closeButtonElement.textContent = '';
    closeButtonElement.classList.add("icon");
    closeButtonElement.classList.add("close");
    closeButtonElement.addEventListener('click', closeFileExpand);
    infoDivElement.appendChild(closeButtonElement);

    figureElement.appendChild(infoDivElement);

    if (image.indexOf(mime) > -1) {
        let img = document.createElement("img");
        img.setAttribute("src", src);
        figureElement.appendChild(img);
    }
    else if (audio.indexOf(mime) > -1) {
        let mediaElement = document.createElement("audio");
        mediaElement.volume = fileExpandConfiguration.volume;
        mediaElement.loop = fileExpandConfiguration.loop;
        mediaElement.setAttribute("src", src);
        mediaElement.setAttribute("preload", "metadata");
        mediaElement.addEventListener('loadedmetadata', handlePlayerLoadedMetadata);
        mediaElement.addEventListener('timeupdate', handlePlayerTimeUpdate);
        mediaElement.addEventListener('ended', handleEndedPlayer);
        figureElement.appendChild(mediaElement);
    }
    else if (video.indexOf(mime) > -1) {
        let mediaElement = document.createElement("video");
        mediaElement.volume = fileExpandConfiguration.volume;
        mediaElement.loop = fileExpandConfiguration.loop;
        mediaElement.setAttribute("src", src);
        mediaElement.setAttribute("preload", "metadata");
        mediaElement.addEventListener('loadedmetadata', handlePlayerLoadedMetadata);
        mediaElement.addEventListener('timeupdate', handlePlayerTimeUpdate);
        mediaElement.addEventListener('ended', handleEndedPlayer);
        figureElement.appendChild(mediaElement);
    }

    let controls = document.createElement("div");
    controls.classList.add("controls");

    if (image.indexOf(mime) > -1) {
        let unscaleButtonElement = document.createElement("button");
        unscaleButtonElement.textContent = '';
        unscaleButtonElement.classList.add("icon");
        unscaleButtonElement.classList.add("unscale");
        unscaleButtonElement.addEventListener('click', toggleUnscaleImage);
        controls.appendChild(unscaleButtonElement);
    }

    if (audio.indexOf(mime) > -1 || video.indexOf(mime) > -1) {
        let progress = document.createElement("progress");
        progress.setAttribute("value", "0");
        progress.setAttribute("min", "0");
        progress.classList.add("progress");
        progress.addEventListener('click', updatePlayerProgress);
        controls.appendChild(progress);

        let playButton = document.createElement("button");
        playButton.textContent = '';
        playButton.classList.add("icon");
        playButton.classList.add("play");
        playButton.addEventListener('click', playPlayer);
        controls.appendChild(playButton);

        let currentTime = document.createElement("time");
        currentTime.textContent = "00:00";
        currentTime.classList.add("current-time");
        controls.appendChild(currentTime);

        let durationTime = document.createElement("time");
        durationTime.textContent = "00:00";
        durationTime.classList.add("duration-time");
        controls.appendChild(durationTime);

        let stopButton = document.createElement("button");
        stopButton.textContent = '';
        stopButton.classList.add("icon");
        stopButton.classList.add("stop");
        stopButton.addEventListener('click', stopPlayer);
        controls.appendChild(stopButton);

        let loopButton = document.createElement("button");
        loopButton.checked = fileExpandConfiguration.loop;
        loopButton.textContent = '';
        loopButton.classList.add("icon");
        loopButton.classList.add("loop");
        loopButton.addEventListener('click', toggleLoopPlayer);
        controls.appendChild(loopButton);

        let muteButton = document.createElement("button");
        muteButton.textContent = '';
        muteButton.classList.add("icon");
        muteButton.classList.add("mute");
        muteButton.addEventListener('click', toggleMutePlayer);
        controls.appendChild(muteButton);

        let volume = document.createElement("progress");
        volume.setAttribute("value", fileExpandConfiguration.volume);
        volume.setAttribute("min", "0");
        volume.setAttribute("max", "1");
        volume.classList.add("volume");
        volume.addEventListener('click', updatePlayerVolume);
        controls.appendChild(volume);
    }

    figureElement.appendChild(controls);
    fileExpandDivElement.appendChild(figureElement);
    document.querySelector("body").appendChild(fileExpandDivElement);
}

function updatePlayerProgress(event) {
    let figureElement = document.querySelector("div.file-expand > figure");
    let mediaElement = figureElement.querySelector("video, audio");
    let progressElement = figureElement.querySelector("progress.progress");

    let position = (event.pageX - progressElement.offsetLeft) / progressElement.offsetWidth;
    mediaElement.currentTime = position * mediaElement.duration;
}

function updatePlayerVolume(event) {
    let figureElement = document.querySelector("div.file-expand > figure");
    let mediaElement = figureElement.querySelector("video, audio");
    let volumeProgressElement = figureElement.querySelector("progress.volume");

    let position = (event.pageX - volumeProgressElement.offsetLeft) / volumeProgressElement.offsetWidth;
    volumeProgressElement.setAttribute('value', position);
    mediaElement.volume = position;
}

function playPlayer(event) {
    let figureElement = document.querySelector("div.file-expand > figure");
    let mediaElement = figureElement.querySelector("video, audio");
    let playButtonElement = figureElement.querySelector("button.play");

    if (mediaElement.paused || mediaElement.ended) {
        mediaElement.play();
        playButtonElement.textContent = '';
    }
    else {
        mediaElement.pause();
        playButtonElement.textContent = '';
    }
}

function stopPlayer(event) {
    let figureElement = document.querySelector("div.file-expand > figure");
    let mediaElement = figureElement.querySelector("video, audio");
    let playButtonElement = figureElement.querySelector("button.play");
    let progressButtonElement = figureElement.querySelector("progress.progress");

    mediaElement.pause();
    mediaElement.currentTime = 0;
    progressButtonElement.value = 0;
    playButtonElement.textContent = '';
}

function toggleLoopPlayer(event) {
    let figureElement = document.querySelector("div.file-expand > figure");
    let mediaElement = figureElement.querySelector("video, audio");

    if (mediaElement.loop) {
        mediaElement.loop = false;
    }
    else {
        mediaElement.loop = true;
    }
}

function toggleMutePlayer(event) {
    let figureElement = document.querySelector("div.file-expand > figure");
    let mediaElement = figureElement.querySelector("video, audio");
    let muteButtonElement = figureElement.querySelector("button.mute");

    if (mediaElement.muted) {
        mediaElement.muted = false;
        muteButtonElement.textContent = '';
    }
    else {
        mediaElement.muted = true;
        muteButtonElement.textContent = '';
    }
}

function handlePlayerLoadedMetadata(event) {
    let figureElement = document.querySelector("div.file-expand > figure");
    let mediaElement = figureElement.querySelector("video, audio");
    let playButtonElement = figureElement.querySelector("button.play");
    let progressButtonElement = figureElement.querySelector("progress.progress");
    let durationTimeElement = figureElement.querySelector("time.duration-time");

    progressButtonElement.setAttribute('max', mediaElement.duration);
    durationTimeElement.textContent = generatePlayerTimeDuration(mediaElement.duration);
    mediaElement.play();
    playButtonElement.textContent = '';
}

function handlePlayerTimeUpdate(event) {
    let figureElement = document.querySelector("div.file-expand > figure");

    if (figureElement == null) {
        return;
    }

    let mediaElement = figureElement.querySelector("video, audio");

    if (mediaElement == null) {
        return;
    }

    let progressButtonElement = figureElement.querySelector("progress.progress");
    let currentTimeElement = figureElement.querySelector("time.current-time");

    progressButtonElement.value = mediaElement.currentTime;
    currentTimeElement.textContent = generatePlayerTimeDuration(mediaElement.currentTime);
}

function handleEndedPlayer(event) {
    let figureElement = document.querySelector("div.file-expand > figure");
    let playButtonElement = figureElement.querySelector("button.play");

    playButtonElement.textContent = '';
}

function closeFileExpand(event) {
    if (event.target === document.querySelector("div.file-expand") || event.target === document.querySelector("div.file-expand button.close")) {
        if (document.querySelector("div.file-expand > figure > video, div.file-expand > figure > audio") != null) {
            stopPlayer(event);
        }
        document.querySelector("div.file-expand").remove();
    }
}

function generatePlayerTimeDuration(time) {
    var secondsNumber = parseInt(time, 10);
    var minutes = Math.floor(secondsNumber / 60);
    var seconds = secondsNumber - (minutes * 60);

    if (minutes < 10) { minutes = "0" + minutes; }
    if (seconds < 10) { seconds = "0" + seconds; }
    return minutes + ':' + seconds;
}

function toggleUnscaleImage(event) {
    let buttonElement = event.target;
    let figureElement = buttonElement.closest("figure");
    let imgElement = figureElement.querySelector("img");
    if (imgElement.hasAttribute("data-unscale")) {
        imgElement.removeAttribute("data-unscale");
        imgElement.removeEventListener("mousemove", moveUnscaleImage);
        imgElement.style.objectPosition = null;
        imgElement.style.width = null;
        imgElement.style.height = null;
    }
    else {
        imgElement.setAttribute("data-unscale", "true");
        imgElement.addEventListener("mousemove", moveUnscaleImage);
        imgElement.style.width = `${imgElement.naturalWidth}px`;
        imgElement.style.height = `${imgElement.naturalHeight}px`;
    }
}

function moveUnscaleImage(event) {
    let imgElement = event.target;
    let width = (event.clientX - imgElement.offsetLeft) / imgElement.offsetWidth * 100;
    let height = (event.clientY - imgElement.offsetTop) / imgElement.offsetHeight * 100;
    imgElement.style.objectPosition = `${width}% ${height}%`;
}


