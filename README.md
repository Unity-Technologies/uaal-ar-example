UAAL AR demo
============

This repository represents an example of using Unity's AR features in a library as part of a native iOS or Android app.

## Folder structure
### AndroidProject
The Android Studio project for the native Android component. Already preconfigured for a Unity output library built at /UnityAndroidLibrary.

### OriginalUnitySample
Original UAAL sample. To be removed.

### UnityProject
The Unity AR project.

### XcodeProject
The Xcode project for the native iOS component. Not currently functional.

## Build instructions

### Android

To build on Android, ensure that the Export Project checkbox is set, and, for convenience, save the output to a folder called UnityAndroidLibrary, in the root of this repository. The Android Studio project is already configured for an app at this location.

You can then open the AndroidProject folder in Android Studio and build it as normal.

To prevent a second app icon from appearing on your home screen, after building in Unity, make sure to remove the `<intent-filter>` block from the Unity player activity in the generated AndroidManifest.xml file in the unity library project. 