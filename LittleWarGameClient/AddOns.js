addons = {
    pressFullScreenButton: function(element) {
        window.chrome.webview.postMessage(
            JSON.stringify({
                Id: element.id,
                Value: "Toggled",
                Type: "FullScreen"
            })
        );
    },

    pressExitButton: function (element) {
        window.chrome.webview.postMessage(
            JSON.stringify({
                Id: element.id,
                Value: "Pressed",
                Type: "Exit"
            })
        );
    },

    pressMouseLockCheckbox: function (element) {
        window.chrome.webview.postMessage(
            JSON.stringify({
                Id: element.id,
                Value: element.checked.toString(),
                Type: "MouseLock"
            })
        );
    },

    toggleMenu: function() {
        document.getElementById("ingameMenuButton").click();
    },

    toggleFriends: function() {
        document.getElementById("friendsButton").click();
    },

    toggleChat: function () {
        var chat = document.getElementById("ingameChatHistoryButton");
        if (chat.style.visibility != "hidden") {
            chat.click();
        }
    }
};

addons.init = {
    function() {
        this.addCustomHotkeysToTitles();
        this.addExitButton();
        this.replaceMouseLockCheckbox();
        var fullScreenButton = document.getElementById("optionsFullscreenButton");
        fullScreenButton.onclick = function () {
            addons.pressFullScreenButton(this);
        };
        console.log("Addons loaded");
    },

    addCustomHotkeysToTitles: function () {
        document.getElementById("ingameMenuButton").title = "Options [F10]";
        document.getElementById("friendsButton").title = "Friends & Messages [F9]";
    },

    addExitButton: function () {
        var exitId = "exitButton";
        if (!document.getElementById(exitId)) {
            var exitButton = document.createElement("button");
            exitButton.id = exitId;
            exitButton.title = "Exit Game";
            exitButton.innerText = "Exit"
            exitButton.setAttribute("style", `font-size: 36px;
                                height: 54px;
                                margin: 0 2px;
                                padding: 0 10px;
                                background: gray;`);
            var mainButtonContainer = document.getElementById("buttonDaddy");
            mainButtonContainer.appendChild(exitButton);
            exitButton.onclick = function () {
                addons.pressExitButton(this);
            };
        }
    },

    replaceMouseLockCheckbox: function () {
        var mouseLockId = "mouseLockCheckbox";
        if (!document.getElementById(mouseLockId)) {
            var mouseLockCheckbox = document.createElement("input");
            mouseLockCheckbox.id = mouseLockId;
            mouseLockCheckbox.type = "checkbox";
            var lockContainer = document.getElementById("pointerLockLabel");
            lockContainer.lastChild.remove();
            lockContainer.appendChild(mouseLockCheckbox);
            mouseLockCheckbox.onchange = function () {
                addons.pressMouseLockCheckbox(this);
            };
        }
    }
};