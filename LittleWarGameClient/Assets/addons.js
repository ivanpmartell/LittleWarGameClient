addons = {
  pressFullScreenButton: function (element) {
    CefSharp.PostMessage(
      JSON.stringify({
        Id: element.id,
        Value: "Toggled",
        Type: "FullScreen"
      })
    );
  },

  pressExitButton: function (element) {
    CefSharp.PostMessage(
      JSON.stringify({
        Id: element.id,
        Value: "Pressed",
        Type: "Exit"
      })
    );
  },

  pressReloadButton: function (element) {
    CefSharp.PostMessage(
      JSON.stringify({
        Id: element.id,
        Value: "Pressed",
        Type: "Reload"
      })
    );
  },

  pressMouseLockCheckbox: function (element) {
    CefSharp.PostMessage(
      JSON.stringify({
        Id: element.id,
        Value: element.checked.toString(),
        Type: "MouseLock"
      })
    );
  },

  pressDisablePluginsCheckbox: function (element) {
    CefSharp.PostMessage(
      JSON.stringify({
        Id: element.id,
        Value: element.checked.toString(),
        Type: "DisableAllPlugins"
      })
    );
  },

  pressInstalledPluginsTab: function (element) {
    var installedPluginsTabContent = document.getElementById("installedPluginsTabContent");
    installedPluginsTabContent.innerHTML = "";
    CefSharp.PostMessage(
      JSON.stringify({
        Id: element.id,
        Value: "Pressed",
        Type: "ViewInstalledPlugins"
      })
    );
  },

  pressAvailablePluginsTab: function (element) {
    var availablePluginsTabContent = document.getElementById("availablePluginsTabContent");
    availablePluginsTabContent.innerHTML = "";
    CefSharp.PostMessage(
      JSON.stringify({
        Id: element.id,
        Value: "Pressed",
        Type: "BrowseAvailablePlugins"
      })
    );
  },

  pressInstallPluginButton: function (element, plugin) {
    CefSharp.PostMessage(
      JSON.stringify({
        Id: element.id,
        Value: plugin.Folder,
        Type: "InstallPlugin"
      })
    );
  },

  pressUninstallPluginButton: function (element, plugin) {
    CefSharp.PostMessage(
      JSON.stringify({
        Id: element.id,
        Value: plugin.Folder,
        Type: "UninstallPlugin"
      })
    );
  },

  pressUpdatePluginButton: function (element, plugin) {
    CefSharp.PostMessage(
      JSON.stringify({
        Id: element.id,
        Value: plugin.Folder,
        Type: "UpdatePlugin"
      })
    );
  },

  pressEnablePluginCheckbox: function (element, plugin) {
    if (element.checked) {
      CefSharp.PostMessage(
        JSON.stringify({
          Id: element.id,
          Value: plugin.Folder,
          Type: "EnablePlugin"
        })
      );
    }
    else {
      CefSharp.PostMessage(
        JSON.stringify({
          Id: element.id,
          Value: plugin.Folder,
          Type: "DisablePlugin"
        })
      );
    }
  },

  overlaySelected: function (element) {
    CefSharp.PostMessage(
      JSON.stringify({
        Id: element.id,
        Value: element.value,
        Type: "OverlayType"
      })
    );
  },

  changeVolume: function (element, volumeLevel) {
    CefSharp.PostMessage(
      JSON.stringify({
        Id: element.id,
        Value: volumeLevel.toString(),
        Type: "VolumeChanging"
      })
    );
  },

  saveVolumeChange: function (element, volumeLevel) {
    CefSharp.PostMessage(
      JSON.stringify({
        Id: element.id,
        Value: volumeLevel.toString(),
        Type: "VolumeChanged"
      })
    );
  },

  fakeClick: function (element) {
    if (element.click) {
      element.click()
    } else if (document.createEvent) {
      var event = new MouseEvent('click', {
        'view': window
      });
      element.dispatchEvent(evt);
    }
  },

  toggleMenu: function () {
    document.getElementById("ingameMenuButton").click();
  },

  toggleFriends: function () {
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
    this.setElement("pluginOptions", "height: 405px;");
    this.setElement("optionsQuitButton", "margin-top: 3px;");
    this.setElement("startButton", "top: 600px;");
    this.setElement("backButton", "top: 600px;");
  },

  setNormalWindowSizes: function () {
    this.setElement("optionsChecklistDiv", "height: 510px;");
    this.setElement("pluginOptions", "height: 510px;");
    this.setElement("optionsWindow", "height: 720px");
    this.resetElement("queriesWindow", "height:");
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
  },

  displayNotification: function (msg, height = 150) {
    document.getElementById("infoWindow").style.height = height + "px";
    document.getElementById("infoWindow").style.display = "inline";
    document.getElementById("infoWindow").style.opacity = "100";
    var notificationText = document.createElement("div");
    notificationText.classList.add("infoWindowText");
    notificationText.innerText = msg;
    notificationText.style.fontSize = "x-large";
    notificationText.style.overflow = "auto";
    notificationText.style.height = "-webkit-fill-available";
    var notificationTextArea = document.getElementById("infoWindowTextArea");
    notificationTextArea.innerHTML = "";
    notificationTextArea.appendChild(notificationText);
  },

  cancelledEnablePlugin: function (pluginId) {
    var pluginCheckbox = document.getElementById(`EnablePlugin-${pluginId}`);
    pluginCheckbox.checked = false;
  },

  clearInstalledPluginsTabContents: function () {
    var installedPluginsTabContent = document.getElementById("installedPluginsTabContent");
    installedPluginsTabContent.innerHTML = "";
  },

  clearAvailablePluginsTabContents: function () {
    var availablePluginsTabContent = document.getElementById("availablePluginsTabContent");
    availablePluginsTabContent.innerHTML = "";
  },

  receiveAvailablePlugin: function (alreadyInstalled, pluginJson) {
    const plugin = JSON.parse(pluginJson.replace(/\n/g, "\\n"));

    var installPluginButton = document.createElement("button");
    installPluginButton.innerText = "Install";
    installPluginButton.disabled = alreadyInstalled;
    installPluginButton.style.float = "right";
    installPluginButton.onclick = function () {
      addons.pressInstallPluginButton(this, plugin);
    };
    var pluginRow = document.createElement("p");
    pluginRow.style.paddingBottom = "1em";
    pluginRow.style.borderBottom = "0.1em solid #fff";
    var pluginName = document.createElement("div");
    pluginName.title = "Click plugin name to show version and description";
    pluginName.innerText = plugin.Name;
    pluginName.style.display = "inline";
    pluginName.style.paddingLeft = "0.5em";
    pluginName.onclick = function () {
      addons.displayNotification(`${plugin.Name}\nVersion ${plugin.Version}\n${plugin.Description}`, 350);
    };
    pluginRow.appendChild(installPluginButton);
    pluginRow.appendChild(pluginName);
    var availablePluginsTabContent = document.getElementById("availablePluginsTabContent");
    availablePluginsTabContent.appendChild(pluginRow);
  },

  receiveInstalledPlugin: function (latestVersion, pluginJson) {
    const plugin = JSON.parse(pluginJson.replace(/\n/g, "\\n"));
    var alreadyLatestVersion = addons.compareVersions(plugin.Version, latestVersion) !== -1;

    var enablePluginCheckbox = document.createElement("input");
    enablePluginCheckbox.id = `EnablePlugin-${plugin.Folder}`;
    enablePluginCheckbox.type = "checkbox";
    enablePluginCheckbox.checked = plugin.Enabled;
    enablePluginCheckbox.onchange = function () {
      addons.pressEnablePluginCheckbox(this, plugin);
    };
    var updatePluginButton = document.createElement("button");
    updatePluginButton.innerText = "Update";
    updatePluginButton.disabled = alreadyLatestVersion;
    updatePluginButton.style.float = "right";
    updatePluginButton.onclick = function () {
      addons.pressUpdatePluginButton(this, plugin);
    };
    var uninstallPluginButton = document.createElement("button");
    uninstallPluginButton.innerText = "Uninstall";
    uninstallPluginButton.style.float = "right";
    uninstallPluginButton.onclick = function () {
      addons.pressUninstallPluginButton(this, plugin);
    };
    var pluginRow = document.createElement("p");
    pluginRow.style.paddingBottom = "1em";
    pluginRow.style.borderBottom = "0.1em solid #fff";
    var pluginName = document.createElement("div");
    pluginName.title = "Click plugin name to show version and description";
    pluginName.innerText = plugin.Name;
    pluginName.style.display = "inline";
    pluginName.style.paddingLeft = "0.5em";
    pluginName.onclick = function () {
      var msg = "";
      if (latestVersion == null || latestVersion == "null")
        msg = `Custom plugin\n${plugin.Name}\nInstalled version: ${plugin.Version}\n${plugin.Description}`;
      else
        msg = `${plugin.Name}\nInstalled version: ${plugin.Version}\nLatest version: ${latestVersion}\n${plugin.Description}`;
      addons.displayNotification(msg, 350);
    };
    pluginRow.appendChild(enablePluginCheckbox);
    pluginRow.appendChild(pluginName);
    pluginRow.appendChild(updatePluginButton);
    pluginRow.appendChild(uninstallPluginButton);
    var installedPluginsTabContent = document.getElementById("installedPluginsTabContent");
    installedPluginsTabContent.appendChild(pluginRow);
  },

  compareVersions: function (v1, v2) {
    if (v1 == null) return -1;
    if (v2 == null) return 1;
    const parts1 = v1.split('.').map(Number);
    const parts2 = v2.split('.').map(Number);
    for (let i = 0; i < Math.max(parts1.length, parts2.length); i++) {
      const num1 = parts1[i] || 0;
      const num2 = parts2[i] || 0;

      if (num1 > num2) return 1;  // v1 is greater
      if (num1 < num2) return -1; // v2 is greater
    }
    return 0; // Versions are equal
  }
};

addons.init = {
  function(clientVersion, mouseLock, volume, disablePlugins, overlayType, overlayOptions) {
    this.handleConnectionError();
    this.addExitButton();
    this.addPluginsButton();
    this.addPluginsDiv();
    this.addRefreshButton();
    this.resizeInfoWindow();
    this.moveLoadingText();
    this.changeQuitButtonText();
    this.addVolumeSlider(volume);
    this.addClientVersion(clientVersion);
    this.replaceMouseLockCheckbox(mouseLock);
    this.addDisablePluginsCheckbox(disablePlugins);
    this.addOverlaySelectDropdown(overlayType, overlayOptions);
    this.replaceFullscreenButton();
    this.addSaveReplayNotification();
    this.addClientMadeBy();
    console.log("Addons loaded");
    this.jsInitComplete();
  },

  replaceFullscreenButton: function () {
    var fullScreenButton = document.getElementById("optionsFullscreenButton");
    fullScreenButton.onclick = function () {
      addons.pressFullScreenButton(this);
    };
  },

  addSaveReplayNotification: function () {
    var replayButton = document.getElementById("replayButton");
    const existingReplayButtonHandler = replayButton.onclick;
    replayButton.onclick = function () {
      if (existingReplayButtonHandler)
        existingReplayButtonHandler();
      var replays = document.getElementsByClassName("replayRider");
      for (let i = 0; i < replays.length; i++) {
        var buttons = replays[i].getElementsByTagName("button");
        var saveButton = Array.from(buttons).find(button => button.innerHTML.trim() === "save");
        const existingSaveButtonHandler = saveButton.onclick;
        saveButton.onclick = function () {
          if (existingSaveButtonHandler)
            existingSaveButtonHandler();
          addons.displayNotification("Replay has been saved");
        }
      }
    }
  },

  moveLoadingText: function () {
    var loadingText = document.getElementById("loadingText");
    loadingText.style.cssText = "top: 50px;";
  },

  resizeInfoWindow: function () {
    var infoWindow = document.getElementById("playerInfoWindow");
    infoWindow.style.cssText += "top: 50px; height: 580px;";
  },

  handleConnectionError: function () {
    var connectionErrorWindow = document.getElementById("NoConnectionWindow");
    if (connectionErrorWindow != null) {
      this.addReconnect(connectionErrorWindow);
    }
    else {
      var observer = new MutationObserver(function (mutations) {
        mutations.forEach(function (mutation) {
          for (let i = 0; i < mutation.addedNodes.length; i++) {
            var element = mutation.addedNodes[i];
            if (element.id == "NoConnectionWindow") {
              addons.init.addReconnect(element);
            }
          }
        });
      });
      observer.observe(document.body, {
        childList: true
      });
    }
  },

  addReconnect: function (element) {
    element.style = "position: absolute; top: 50px; width: 300px;"
    var windowTitle = element.getElementsByTagName('h2')[0];
    windowTitle.style = "position: relative;";
    var reconnectButton = document.createElement("h1");
    reconnectButton.innerText = "Reconnect";
    reconnectButton.style = "position: relative; left: 30%; width: fit-content;";
    reconnectButton.onmouseover = function () { this.style.color = 'darkorange' };
    reconnectButton.onmouseout = function () { this.style.color = 'inherit' };
    reconnectButton.onclick = function () { addons.pressReloadButton(this); };
    windowTitle.insertAdjacentElement('afterend', reconnectButton);
  },

  addVolumeSlider: function (volume) {
    var globalVolumeId = "globalVolumeLabel";
    if (!document.getElementById(globalVolumeId)) {
      var globalVolume = document.createElement("p");
      globalVolume.id = globalVolumeId;
      globalVolume.innerText = "Master Volume";
      var globalVolumeDiv = document.createElement("div");
      globalVolumeDiv.id = "optionsMasterSoundButton";
      globalVolume.appendChild(globalVolumeDiv);
      document.getElementById("optionsChecklistDiv").prepend(globalVolume);
      $(globalVolumeDiv).slider({ slide: (event, ui) => addons.changeVolume(globalVolumeDiv, ui.value / 100) });
      $(globalVolumeDiv).slider({ change: (event, ui) => addons.saveVolumeChange(globalVolumeDiv, ui.value / 100) });
      $(globalVolumeDiv).slider("value", volume * 100);
    }
  },

  addClientMadeBy: function () {
    var imprintLink = document.getElementById("imprintLink");
    imprintLink.onclick = function () {
      var breakElement = document.createElement("br");
      var divElement = document.createElement("div");
      divElement.innerHTML = "Windows client made by Ivan Martell<br>© " + new Date().getFullYear();
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

  addRefreshButton: function () {
    var refreshId = "refreshButton";
    if (!document.getElementById(refreshId)) {
      var refreshButton = document.createElement("button");
      refreshButton.id = refreshId;
      refreshButton.title = "Reload Game";
      refreshButton.innerText = "↻";
      refreshButton.setAttribute("style", "color: lightgreen; padding-top: 1.5px;");
      var optionButtons = document.getElementById("optionsButtonsDiv");
      optionButtons.insertBefore(refreshButton, optionButtons.firstChild);
      refreshButton.onclick = function () { addons.pressReloadButton(this); };
    }
  },

  addPluginsButton: function () {
    var pluginId = "pluginButton";
    if (!document.getElementById(pluginId)) {
      var pluginButton = document.createElement("button");
      pluginButton.id = pluginId;
      pluginButton.innerText = "Plugins";
      var optionButtons = document.getElementById("optionsButtonsDiv");
      optionButtons.insertBefore(pluginButton, optionButtons.firstChild);
      pluginButton.onclick = function () {
        document.getElementById("optionsChecklistDiv").style.display = "None";
        document.getElementById("pluginOptions").style.display = "block";
      };
    }
  },

  addPluginsDiv: function () {
    const cssString = `
      .wrapper {
        width: 85%;
        margin: 0 auto;
        height: inherit;
      }
      .tabs {
        position: relative;
        margin: 1rem 0;
        height: 90%;
      }
      .tabs::before,
      .tabs::after {
        content: "";
        display: table;
      }
      .tabs::after {
        clear: both;
      }
      .tab {
        float: left;
      }
      .tab-switch {
        display: none;
      }
      .tab-label {
        position: relative;
        display: block;
        line-height: 2.75em;
        height: 3em;
        padding: 0 6.289em;
        background: #273e94;
        border-left: 0.125rem solid #273e94;
        color: #fff;
        cursor: pointer;
        top: 0;
        transition: all 0.25s;
      }
      .tab-label:hover {
        top: -0.25rem;
        transition: top 0.25s;
      }
      .tab-content {
        height: 80%;
        position: absolute;
        z-index: 1;
        top: 2.75em;
        left: 0;
        padding: 1rem;
        color: #fff;
        border-top: 0.25rem solid #fff;
        border-bottom: 0.15rem solid #fff;
        opacity: 0;
        transition: all 0.35s;
        width: -webkit-fill-available;
        overflow: auto;
      }
      .tab-switch:checked + .tab-label {
        background: #277094;
        color: #fff;
        border-bottom: 0;
        border-left: 0.25rem solid #fff;
        transition: all 0.35s;
        z-index: 1;
        top: -0.0625rem;
      }
      .tab-switch:checked + label + .tab-content {
        z-index: 2;
        opacity: 1;
        transition: all 0.35s;
      }`;
    const styleElement = document.createElement('style');
    styleElement.appendChild(document.createTextNode(cssString));
    document.head.appendChild(styleElement);
    var pluginsDiv = document.createElement("div");
    pluginsDiv.id = "pluginOptions";
    pluginsDiv.style.display = "None";
    pluginsDiv.innerHTML = `
    <div class="wrapper">
      <div class="tabs">
        <div class="tab">
          <input type="radio" name="css-tabs" id="tab-installed" checked class="tab-switch">
          <label for="tab-installed" class="tab-label">Installed</label>
          <div id="installedPluginsTabContent" class="tab-content">test 1</div>
        </div>
        <div class="tab">
          <input type="radio" name="css-tabs" id="tab-available" class="tab-switch">
          <label for="tab-available" class="tab-label">Browse</label>
          <div id="availablePluginsTabContent" class="tab-content">test 2</div>
        </div>
      </div>
      <button id="pluginsBackButton" title="Go back to the main options">Back</button>
    </div>`;
    var optionsChecklist = document.getElementById("optionsChecklistDiv");
    optionsChecklist.insertAdjacentElement('afterend', pluginsDiv);
    var availablePluginsTab = document.getElementById("tab-available");
    availablePluginsTab.onclick = function () {
      console.log("Loading available plugins");
      addons.pressAvailablePluginsTab(this);
    };
    var installedPluginsTab = document.getElementById("tab-installed");
    installedPluginsTab.onclick = function () {
      console.log("Loading installed plugins");
      addons.pressInstalledPluginsTab(this);
    };
    var pluginsBackButton = document.getElementById("pluginsBackButton");
    pluginsBackButton.onclick = function () {
      pluginsDiv.style.display = "none";
      optionsChecklist.style.display = "block";
    };
    addons.pressInstalledPluginsTab(pluginsDiv);
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
  },

  addDisablePluginsCheckbox: function (disablePlugins) {
    var disablePluginsId = "disablePluginsCheckbox";
    if (!document.getElementById(disablePluginsId)) {
      var disablePluginsCheckbox = document.createElement("input");
      disablePluginsCheckbox.id = disablePluginsId;
      disablePluginsCheckbox.type = "checkbox";
      disablePluginsCheckbox.checked = disablePlugins;
      var disablePluginsContainer = document.createElement("p");
      disablePluginsContainer.id = "disableLabel";
      disablePluginsContainer.title = "Plugins will stop being loaded for the game client";
      disablePluginsContainer.innerText = "Disable plugin functionality";
      disablePluginsContainer.appendChild(disablePluginsCheckbox);
      var optionsList = document.getElementById("optionsChecklistDiv");
      optionsList.appendChild(disablePluginsContainer);
      disablePluginsCheckbox.onchange = function () {
        addons.pressDisablePluginsCheckbox(this);
      };
    }
  },

  addOverlaySelectDropdown: function (overlayType, options) {
    var overlaySelectId = "selectOverlayDropdown";
    if (!document.getElementById(overlaySelectId)) {
      var overlayDropdown = document.createElement("select");
      overlayDropdown.id = overlaySelectId;
      optionsArray = options.split(",");
      for (let i = 0; i < optionsArray.length; i++) {
        const optionElement = document.createElement('option');
        optionElement.value = i;
        optionElement.text = optionsArray[i];
        overlayDropdown.appendChild(optionElement);
      }
      overlayDropdown.selectedIndex = overlayType;
      var overlayContainer = document.createElement("p");
      overlayContainer.id = "overlayLabel";
      overlayContainer.title = "Set the overlay graphics renderer";
      overlayContainer.innerText = "Overlay graphics engine";
      overlayContainer.appendChild(overlayDropdown);
      var scrollOption = document.getElementById("scrollSpeedLabel");
      scrollOption.insertAdjacentElement('afterend', overlayContainer);
      overlayDropdown.onchange = function () {
        addons.overlaySelected(this);
      };
    }
  },

  changeQuitButtonText: function () {
    document.getElementById("optionsQuitButton").innerText = "Surrender";
    addons.addCustomHotkeyToInnerText("optionsQuitButton", "N");
  },

  jsInitComplete: function () {
    CefSharp.PostMessage(
      JSON.stringify({
        Id: "",
        Value: "",
        Type: "InitComplete"
      })
    );
  }
};