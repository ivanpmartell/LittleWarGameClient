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
    function(mouseLock, clientVersion) {
        this.addExitButton();
        this.addClientVersion(clientVersion);
        this.addCustomHotkeysToTitles();
        this.replaceMouseLockCheckbox(mouseLock);
        var fullScreenButton = document.getElementById("optionsFullscreenButton");
        fullScreenButton.onclick = function () {
            addons.pressFullScreenButton(this);
        };
        console.log("Addons loaded");
    },

    addClientVersion: function (clientVersion) {
        var options = document.getElementById("optionsWindow");
        var title = options.getElementsByTagName('h2')[0];
        title.innerText = `${title.innerText} [Client v${clientVersion}]`;
    },

    addCustomHotkeysToTitles: function () {
        var menuButton = document.getElementById("ingameMenuButton");
        menuButton.title = `${menuButton.title} [F10]`;
        var friendsButton = document.getElementById("friendsButton");
        friendsButton.title = `${friendsButton.title} [F9]`;
        var chatHistoryButton = document.getElementById("ingameChatHistoryButton");
        chatHistoryButton.title = `${chatHistoryButton.title} [F11]`;
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

    replaceMouseLockCheckbox: function (mouseLock) {
        var mouseLockId = "mouseLockCheckbox";
        if (!document.getElementById(mouseLockId)) {
            var mouseLockCheckbox = document.createElement("input");
            mouseLockCheckbox.id = mouseLockId;
            mouseLockCheckbox.type = "checkbox";
            mouseLockCheckbox.checked = mouseLock;
            var lockContainer = document.getElementById("pointerLockLabel");
            lockContainer.title = "The mouse pointer will stay locked inside the game screen.";
            lockContainer.lastChild.remove();
            lockContainer.appendChild(mouseLockCheckbox);
            mouseLockCheckbox.onchange = function () {
                addons.pressMouseLockCheckbox(this);
            };
        }
    }
};