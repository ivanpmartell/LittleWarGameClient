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

    fakeClick: function (anchorObj) {
        if (anchorObj.click) {
            anchorObj.click()
        } else if (document.createEvent) {
            var event = new MouseEvent('click', {
                'view': window
            });
            anchorObj.dispatchEvent(evt);
        }
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
    },

    addOptionsMenuHotkey: function (hotkey) {
        this.addCustomHotkeyToTitle("ingameMenuButton", hotkey);
    },

    addFriendsMenuHotkey: function (hotkey) {
        this.addCustomHotkeyToTitle("friendsButton", hotkey);
    },

    addChatHistoryHotkey: function (hotkey) {
        this.addCustomHotkeyToTitle("ingameChatHistoryButton", hotkey);
    },

    addFullscreenBtnHotkey: function (hotkey) {
        this.addCustomHotkeyToInnerText("optionsFullscreenButton", hotkey);
    },

    setSmallWindowSizes: function () {
        this.setElement("optionsWindow", "height: 600px;");
        this.setElement("queriesWindow", "height: 600px;");
        this.setElement("optionsChecklistDiv", "height: 405px;");
        this.setElement("optionsQuitButton", "margin-top: 3px;");
        this.setElement("startButton", "top: 600px;");
        this.setElement("backButton", "top: 600px;");
    },

    setNormalWindowSizes: function () {
        this.resetElement("optionsWindow", "height:");
        this.resetElement("queriesWindow", "height:");
        this.resetElement("optionsChecklistDiv", "height:");
        this.resetElement("optionsQuitButton", "margin-top:");
        this.resetElement("startButton", "top:");
        this.resetElement("backButton", "top:");
    },

    resetElement: function (elementName, styleProperty) {
        var element = document.getElementById(elementName);
        var smallIdx = element.style.cssText.indexOf(styleProperty)
        if (smallIdx != -1) {
            element.style.cssText = element.style.cssText.substring(0, smallIdx)
        }
    },

    setElement: function (elementName, styleProperty) {
        var element = document.getElementById(elementName);
        var smallIdx = element.style.cssText.indexOf(styleProperty)
        if (smallIdx == -1) {
            element.style.cssText += styleProperty;
        }
    },

    addCustomHotkeyToTitle: function (elementId, hotkey) {
        var element = document.getElementById(elementId);
        var hotkeyIdx = element.title.indexOf('[')
        if (hotkeyIdx != -1) {
            element.title = element.title.substring(0, hotkeyIdx).trim();
        }
        element.title = `${element.title} [${hotkey}]`;
    },

    addCustomHotkeyToInnerText: function (elementId, hotkey) {
        var element = document.getElementById(elementId);
        var hotkeyIdx = element.innerText.indexOf('[')
        if (hotkeyIdx != -1) {
            element.innerText = element.innerText.substring(0, hotkeyIdx).trim();
        }
        element.innerText = `${element.innerText} [${hotkey}]`;
    }
};

addons.init = {
    function(mouseLock, clientVersion) {
        this.addExitButton();
        this.addClientVersion(clientVersion);
        this.replaceMouseLockCheckbox(mouseLock);
        var fullScreenButton = document.getElementById("optionsFullscreenButton");
        fullScreenButton.onclick = function () {
            addons.pressFullScreenButton(this);
        };
        this.addClientMadeBy();
        console.log("Addons loaded");
    },

    addClientMadeBy: function () {
        var imprintLink = document.getElementById("imprintLink");
        imprintLink.onclick = function () {
            var breakElement = document.createElement("br");
            var divElement = document.createElement("div");
            divElement.innerHTML = "Windows client made by SunNoise<br>© 2024";
            var imprint = document.getElementById("addScrollableSubDivTextArea2");
            imprint.appendChild(breakElement);
            imprint.appendChild(divElement);
        };
    },

    addClientVersion: function (clientVersion) {
        var options = document.getElementById("optionsWindow");
        var title = options.getElementsByTagName('h2')[0];
        title.innerText = `${title.innerText} [Client v${clientVersion}]`;
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