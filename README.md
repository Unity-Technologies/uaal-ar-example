Using Unity as a Library AR demo
============

This repository represents an example of using Unity's AR features in a library as part of a native iOS or Android app.

### Unity version 
Minimal Unity version 2019.3.0f1 

## Folder structure

* **AndroidProject**: The Android Studio project for the native Android component. Already preconfigured for a Unity output library built at /UnityAndroidLibrary.
* **Swift XCodeProject**: A Swift project version of the original Unity project.
* **UnityProject**: The Unity AR project.
* **XcodeProject**: The Xcode project for the native iOS component.
* **both.xcworkspace**: The workspace used to build the XcodeProject and the Unity iOS project.

## Build instructions
Refer to Unity's documentation for more information.

* Unity as a library general info https://docs.unity3d.com/2019.3/Documentation/Manual/UnityasaLibrary.html 
* Android [details](https://docs.unity3d.com/2019.3/Documentation/Manual/UnityasaLibrary-Android.html), [sample]( https://github.com/Unity-Technologies/uaal-example/blob/master/docs/android.md)
* iOS [details](https://docs.unity3d.com/2019.3/Documentation/Manual/UnityasaLibrary-iOS.html). [sample](iOS - https://github.com/Unity-Technologies/uaal-example/blob/master/docs/ios.md)

## Setting up the Unity as a library AR example for Android and iOS
### Application Walkthrough

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

## How to setup the demo in iOS and Android

Refer to "Build Instructions" section above for setup details.

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


