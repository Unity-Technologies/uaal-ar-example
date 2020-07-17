Using Unity as a Library AR demo
============

This repository represents an example of using Unity's AR features in a library as part of a native iOS or Android app.

### Unity version 
Minimal Unity version 2019.3.0f1 

## Folder structure

* **AndroidProject**: The Android Studio project for the native Android component. Already preconfigured for a Unity output library built at /UnityAndroidLibrary.
* **UnityProject**: The Unity AR project.
* **XcodeProject**: The Xcode project for the native iOS component.
* **both.xcworkspace**: The workspace used to build the XcodeProject and the Unity iOS project.

## Build instructions
Follow build steps taking into account aditional build notes. Refer to Unity's documentation for more information.

* Unity as a library general info https://docs.unity3d.com/2019.3/Documentation/Manual/UnityasaLibrary.html 
* Android [build steps]( https://github.com/Unity-Technologies/uaal-example/blob/master/docs/android.md), aditional build notes:
    * Export Gradle project to {example_root_dir}/UnityAndroidLibrary or change path in settings.gradle, and sync
    * Please use supported Gradle Plugin version (depends on Android API and Unity Version). This example tested with: Gradle Plugin versions: 3.1, 2.3, 3.3, **3.4** and Gradle Versions: 4.6, **5.1**, 6.5 
    * If needed you can set SDK Location in Project Structure to Editor/.../PlaybackEngines/AndroidPlayer/NDK of installed Unity Editor
    * When changing backend or arch, also change "ndk { abifilters=" value in build.gralde for module:App to be the same as in build.gradle of module:unityLibrary
    * [documentation](https://docs.unity3d.com/2019.3/Documentation/Manual/UnityasaLibrary-Android.html) 
* iOS [build steps]( https://github.com/Unity-Technologies/uaal-example/blob/master/docs/ios.md), aditional build notes:
    * Xcode Workspace file is provided {example_root_dir}/both.xcworkspace, open it and: 
        * add Generated Xcode Project
        * Set Data folder membership
        * Set Public visibility for header
        * Add UnityFramework.framework
    * [documentation](https://docs.unity3d.com/2019.3/Documentation/Manual/UnityasaLibrary-iOS.html).

## Application Walkthrough
This is the Unity as a library demo menu screen:

<img src="https://github.com/Unity-Technologies/uaal-ar-example/blob/master/docs/images/Shopfront.png" width="250" height="444">

There are two items to choose from: a mug and a shirt. There is a color cycle button for each item and a button to enter into the AR Demo, ie launch the embedded Unity project.

<img src="https://github.com/Unity-Technologies/uaal-ar-example/blob/master/docs/images/AR_Placement.jpg" width="250" height="444">
Upon pressing the Show in AR button the Unity AR Demo will initialize and launch.
Follow the prompts to setup the AR environment. Once it is set up, a Tap to Place AR prompt will appear. 

<img src="https://github.com/Unity-Technologies/uaal-ar-example/blob/master/docs/images/AR_Mug.jpg" width="250" height="444">

Tapping will place the selected item in the world with the selected color. After the item has been placed a color cycle button will appear.

The Back and Colour button are native buttons that are drawn over the Unity Activity.

<img src="https://github.com/Unity-Technologies/uaal-ar-example/blob/master/docs/images/AR_Mug2.jpg" width="250" height="444">
<img src="https://github.com/Unity-Technologies/uaal-ar-example/blob/master/docs/images/AR_Shirt.jpg" width="250" height="444">

The back button can be used to return to the shopfront. The Unload Unity button will now be active.

<img src="https://github.com/Unity-Technologies/uaal-ar-example/blob/master/docs/images/Shopfront2.png" width="250" height="444">

## Code snippets/examples 
### Launching Unity Activity
Android - Java
``` java
    // Start the Unity Activity.
    private void selectShopItem(int i) {
        CurrentSelectedItem = i;
        isUnityLoaded = true;
        Intent intent = new Intent(this, MainUnityActivity.class);
        intent.putExtra("product",CurrentSelectedItem+"");
        intent.putExtra("productColor", getColorStringForCurrentItem());
        intent.putExtra("nextProductColor",getNextColorForCurrentItem()+"");
        intent.setFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
        startActivityForResult(intent,1);
    }
```
iOS - Objective C
``` objective-c
- (void)initUnity {
    if([self unityIsInitialized]) {
         [[self ufw] showUnityWindow];
         [self updateUnityShopItem];
        return;
    }
    
    [self setUfw: UnityFrameworkLoad()];
    // Set UnityFramework target for Unity-iPhone/Data folder to make Data part of a UnityFramework.framework and uncomment call to setDataBundleId
    // ODR is not supported in this case, ( if you need embedded and ODR you need to copy data )
    [[self ufw] setDataBundleId: "com.unity3d.framework"];
    [[self ufw] registerFrameworkListener: self];
    [NSClassFromString(@"FrameworkLibAPI") registerAPIforNativeCalls:self];
    
    [[self ufw] runEmbeddedWithArgc: gArgc argv: gArgv appLaunchOpts: appLaunchOpts];
    
    auto view = [[[self ufw] appController] rootView];
}
```

### Sending a message to Unity from Native

The example of this is when cycling the item color within AR using the top right button.

Android - Java
``` java
 // Send messages to Unity to update the AR item.
    @Override
    protected void updateUnityShopItem() {
        mUnityPlayer.UnitySendMessage("AR Session Origin", "SetProduct", currentProduct);
        mUnityPlayer.UnitySendMessage("AR Session Origin", "SetColor",currentProductColor);
    }
```

iOS - Objective C
``` objective-c
- (void)updateItem:(int)itemIndex {
    const char * itemString = [[NSString stringWithFormat:@"%d",currentItem] UTF8String ];
    [[self ufw] sendMessageToGOWithName: "AR Session Origin" functionName: "SetProduct" message: itemString];
    
    int itemColorIndex = colorIndex[itemIndex];
    const char * colorString = [ colorStringArray[itemColorIndex] UTF8String ];
    [[self ufw] sendMessageToGOWithName: "AR Session Origin" functionName: "SetColor" message: colorString];
    self.ColorBtn.tintColor = colorArray[(colorIndex[itemIndex] + 1) % colorArray.count];
}
```

### Sending a message to native from Unity

This method is called from within the Unity Activity when it starts in order to update the item to display.

Unity - C#
``` c#
  private void UpdateShopItem()
    {
#if UNITY_ANDROID
            try
            {
                AndroidJavaClass jc = new AndroidJavaClass("com.company.product.OverrideUnityActivity");
                AndroidJavaObject overrideActivity = jc.GetStatic<AndroidJavaObject>("instance");
                overrideActivity.Call("updateUnityShopItem");
            } 
            catch(Exception e)
            {
                Debug.LogError(e.Message);
            }
#elif UNITY_IOS
            NativeAPI.updateUnityShopItem();
#endif
    }
```

Closing the Unity Activity
Android - Java
``` java
   public void unloadUnity(View view) {
        if(isUnityLoaded) {
            Intent intent = new Intent(this, MainUnityActivity.class);
            intent.setFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
            intent.putExtra("unload", true);
            startActivity(intent);
            isUnityLoaded = false;
        }
    }
```

iOS - Objective C
``` objective-c
- (void)UnloadUnity
{
    if([self unityIsInitialized]){
        [UnityFrameworkLoad() unloadApplication];
    }
}
```


