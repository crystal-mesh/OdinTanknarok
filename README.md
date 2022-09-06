# Integrating ODIN into an existing multiplayer project

In this guide we'll show you how to integrate ODIN into an existing multiplayer project. We'll use the Photon Fusion Tanknarok project as a base multiplayer project without any Voice Chat. In just a few steps you'll learn how to take your project to the next level by integrating ODIN and adding proximity voice chat.

Photon Fusion is a multiplayer framework used in many Unity games. We use it and the Tanknarok Sample Project to give you an idea of how you can integrate ODIN into an existing, fully functional project. The same principles will of course also work for projets developed with Mirror or Unity Netcode. We've even created a guide showing you how to [set up a simple Mirror multiplayer game with ODIN integration](https://www.4players.io/odin/guides/unity/unity-mirror/).

If you're completely new to ODIN and would like to learn more about our features and what makes us special, take a look at our [introduction](https://www.4players.io/odin/introduction/).

## Project Setup

First, we'll download the base project [from the project website](https://doc.photonengine.com/en-us/fusion/current/samples/game-samples/fusion-tanknarok#download). Choose the most up to date version - we've used Version 1.1.1 - download and then unzip the project files. We can then open the project using the Unity Hub. Just select __Open__ and navigate to the folder that contains the project files. 

__Important:__ You need to select the directory that contains the __Assets__, __Packages__ and __ProjectSettings__ directories, otherwise the Unity Hub won't recognize it as a valid project.

Select the Editor version you'd like to use and confirm the __Change Editor version?__ prompt. Please note that ODIN requires Unity version 2019.4 or later, but the Tanknarok project was created in 2020.3.35f1 - so we'll need to use that or any later version. In this guide we used version 2021.3.9f1.

<img src="Documentation/OpenTanknarok.png" alt="Create a new project with the Unity Hub." width="50%">

If you see another prompt __Opening Project in Non-Matching Editor Installation__, click the __Continue__ button to convert your project.

After opening up, you'll be greeted by the __Welcome to Photon Fusion__ prompt, asking you to supply your Fusion App Id. This is required for Photon multiplayer to work. Don't worry, Photon allows a small contingent of 20CCU for test projects, so you won't be billed anything during development. Open up the Dashboard and select the __Create a new app__ button. When prompted for a `Photon Type`, select `Fusion`. Finally, copy the App ID of your newly created Application into the field in the Unity Editor and press enter. You should see a small, green check mark, confirming your successful Fusion setup.

If you've accidentally closed the Photon Fusion Hub you can open it up again by selection `Fusion > Fusion Hub` from the Unity Editor menus or press __Alt+F__.

<img src="Documentation/FusionId.png" alt="The Photon Hub with a valid Fusion Id" width="50%">

Great - now we've got the base project set up, let's take a look at the interesting stuff: Getting Voice Chat into a game.

## ODIN installation

First, let's install ODIN into our project. ODIN can be imported using either a `.unitypackage` or by using Unity's Package Manager and Git. We recommend the latter, because it is easier to keep ODIN up to date. If you don't have Git set up, you can still fall back to the Unity package.

### Package Manager

Select `Window > PackageManager` to open up Unity's package manager. In the top left, select the `+` symbol to add a new package and select __Add package from git URL__. Use the URL `https://github.com/4Players/odin-sdk-unity.git` and select __Add__. The Package Manager will now download the newest release and resolve any dependencies.

### Unity Package

Download the latest ODIN version as a `.unitypackage` from [https://github.com/4Players/odin-sdk-unity/releases](https://github.com/4Players/odin-sdk-unity/releases). Use the `Assets > Import Package > Custom Package...` option and navigate to the downloaded file. Make sure that all Assets are selected and press __Import__.

## Quick Setup
 
Next, we'll perform the basic setup for ODIN. Let's open the `MainScene` in the directory `Assets > Scenes`. This is the startup scene in which Fusion lets you choose the Network Modes. This scene will also persist in all other scenes - any lobby or gameplay scenes will be loaded in addition to the main scene. 

In this scene we will now add the __OdinManager__ prefab. This prefab contains scripts which handle communication with ODIN servers and allow you to adjust settings. You can find all ODIN files under `Packages > 4Players ODIN` in the Project Window. Navigate to `Packages > 4Players ODIN > Runtime` and drag the __OdinManager__ into the scene. Your Scene should now look something like this:

<img src="Documentation/QuickSetup.png" alt="The OdinManager in the Main Scene." width="100%" />

For ODIN to work, we need an Access Key. Select the __OdinManager__ object in the scene and open the __Client Authentication__ drop-down in the Inspector window. ODIN is free to use for up to 25 concurrent users, without requiring an account. Simply press the __Manage Access__ button, click on __Generate Access Key__ and we're good to go. When you get to a point where the Free Tier isn't enough anymore, [take a look at the Start and Pro Tiers](https://www.4players.io/odin/pricing/#starter).

We'll do a quick test to see if everything was set up correctly. Let's create a new folder `ODIN` in the `Assets` directory and then add the new script `OdinConnectionTest`. This script will contain the following:

```csharp
public class OdinConnectionTest : MonoBehaviour
{
    [SerializeField] private string roomName;
    void Start()
    {
        OdinHandler.Instance.JoinRoom(roomName);    
    }
}
```

We use the `OdinHandler.Instance` singleton to join an ODIN room with the name given by the field `roomName`. The __OdinHandler__ script is the main entry point for interacting with the ODIN Api, persists through scene changes and can be accessed anywhere in your code by using `OdinHandler.Instance`. 

Every client connects to an ODIN server, authenticates with an access token and joins a room. Once the client has joined a room, they are a peer inside the ODIN room. Every peer can add a media stream to that room to transmit their microphone input. Clients can join multiple rooms at the same time and can add multiple media streams at the same time. 

Only clients in the same room can actually hear each other, so you can implement features like a global voice chat for all players and sepearte team voice chats, in which only members of the same team can communicate which each other.

To find more information on the basic ODIN topology, [take a look at the Basic Concepts documentation](https://developers.4players.io/odin/introduction/structure/).

For now, we only want to join a room, so the __OdinConnectionTest__ script is enough for our purposes. Let's create a new empty GameObject in the scene hierarchy and add the __OdinConnectionTest__ component to it. Finally, enter a creative room name (like "Test") and our test setup is complete. Your project should now look something like this:

<img src="Documentation/ConnectionTest.png">

To test the project, we'll need create a Build and run it in parallel to the editor. This way we can test everything on the same computer. Press `Ctrl+Shift+B` or use the `File > Build Settings...` menu to show the Build Settings window. Make sure that the __MainScene__, __Lobby__, __Level1__ and __Level2__ scenes are shown and selected in the build list. Click on __Build And Run__, select a directory in which your binaries will be created and wait until the Build was started. Switch to the Editor, press play and you should now be able to hear your own voice transmitted via ODIN.

<img src="Documentation/BuildSettings.png" width="50%">

Congratulations, you've officially added Voice Chat to a Multiplayer Game! But right now it doesn't matter where the players are positioned - in fact, we can hear all players in the Start Screen, without having to enter the game. In a real game this would probably become quite chaotic quite fast, so let's improve that and switch to a Proximity Voice Chat.

### Proximity Voice Chat

Because the Player object is alive during all gameplay situations, in which we want to allow proximity voice chat - i.e. the lobby and the game levels - TODO: Put logic on there.

Switch from automatic playback creation to manual playback creation in odin manager

Create a playback prefab
Add Playback Component script
Adjust Audio Source Settings to 3D

Create a new script on player prefab
Reference Network Object
Create Custom User Data
If local Player:
    Add Fusion Network Id to User Data
    Join Room

Reference Playback Prefab
On Created Media Object: Instantiate playback prefab
OnDisable and local player: Leave Room.

Test

### Audio Listener
Watch out for the position of the AudioListener! In the Tanknarok example it lags behind the player object a bit and does not rotate with the the player tank. 

Write a simple script, which solves this issue.
