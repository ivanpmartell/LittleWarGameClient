addons = {
    pressFullScreenButton: function(element) {
        window.chrome.webview.postMessage(
            JSON.stringify({
                ElementId: element.id,
                Type: "Button",
                Value: "FullScreen"
            })
        );
    },

    pressExitButton: function (element) {
        window.chrome.webview.postMessage(
            JSON.stringify({
                ElementId: element.id,
                Type: "Button",
                Value: "Exit"
            })
        );
    },

    toggleMenu: function() {
        document.getElementById("ingameMenuButton").click();
    },

    toggleFriends: function() {
        document.getElementById("friendsButton").click();
    }

    //passKey: function (key) {
    //    var e = jQuery.Event("keydown");
    //    e.which = Number(key);
    //    $(document).trigger(e);
    //}
};

addons.init = {
    function() {
        this.addCustomHotkeysToTitles();
        this.addExitButton();
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
    }
};