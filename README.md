# UIMirror
UIMirror is a Unity plugin that provides live and interactive remoting of Unity canvas UI to one or more external devices.

## Setup

### Server (Host) Setup

The UIMirror server can be installed via .unitypackage from the [releases](https://github.com/sereilly/uimirror/releases) page, or via source by copying the UIMirror folder into your assets.

1. Add the UIMirror package to your project.
2. If prompted, install TextMesh Pro.
3. Add the UIMirror prefab to your scene.
4. (optional) Set the UIMirrorSource Canvas field to the canvas that you would like to remote.
5. (UWP only) Under Project Settings > Capabilities, enable PrivateNetworkClientServer.

### Client (Remote) Setup
An Android apk of the UIMirror client can be found in the [releases](https://github.com/sereilly/uimirror/releases) page. If you would like to build the client app yourself then follow the steps below.

1. Open the UIMirror Unity project.
2. Under Build Settings, set the build scene to UIMirrorClient/Scenes/UIMirrorClient.
3. Choose the desired target platform and build the project.

## Supported Canvas Elements
- Text
- TextMeshProUGUI (partial)
- Image
- RawImage
- Button
- Slider

## Supported Platforms
UIMirror has been tested on the following platforms:
- PC (Standalone)
- UWP (IL2CPP and .NET backend)
- Android

## External Dependencies and Attributions
- FossilDelta by <https://github.com/endel/FossilDelta>
- protobuf-net by <https://github.com/mgravell/protobuf-net>
- Telepathy by <https://github.com/vis2k/Telepathy>. UWP port by sereilly.
- Gear icon by [Gregor-Cresnar](https://www.flaticon.com/authors/gregor-cresnar) from [www.flaticon.com](https://www.flaticon.com)

