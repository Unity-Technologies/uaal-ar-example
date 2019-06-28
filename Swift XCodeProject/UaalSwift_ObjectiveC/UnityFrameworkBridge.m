#import <Foundation/Foundation.h>
#import "UnityFrameworkBridge.h"
#import "SwiftObjective-Swift.h"
#import <UIKit/UIKit.h>
#include <UnityFramework/UnityFramework.h>

UnityFramework* UnityFrameworkLoad()
{
    NSString* bundlePath = nil;
    bundlePath = [[NSBundle mainBundle] bundlePath];
    bundlePath = [bundlePath stringByAppendingString: @"/Frameworks/UnityFramework.framework"];
    
    NSBundle* bundle = [NSBundle bundleWithPath: bundlePath];
    if ([bundle isLoaded] == false) [bundle load];
    
    UnityFramework* ufw = [bundle.principalClass getInstance];
    if (![ufw appController])
    {
        // unity is not initialized
        [ufw setExecuteHeader: &_mh_execute_header];
    }
    return ufw;
}

NSDictionary* appLaunchOpts;

@implementation UnityFrameworkBridge

//Used to pass messages from Unity Cube.cs and NativeCallsProxy to AppDelegate.swift
- (void)showHostMainWindow:(NSString*)color
{
    AppDelegate *appDel = (AppDelegate*)[[UIApplication sharedApplication]delegate];
    [appDel showHostMainWindow : color];
}

//Loads the UnityFramework and passes it back to AppDelegate.swift
- (UnityFramework*)InitUnityFramework:(char**)commandLineArguments :(int) argumentValue
{
    [self setUfw: UnityFrameworkLoad()];
   
    // Set UnityFramework target for Unity-iPhone/Data folder to make Data part of a UnityFramework.framework and uncomment call to setDataBundleId
    // ODR is not supported in this case, ( if you need embedded and ODR you need to copy data )
    [[self ufw] setDataBundleId: "com.unity3d.framework"];
    [[self ufw] registerFrameworkListener: self];
    [NSClassFromString(@"FrameworkLibAPI") registerAPIforNativeCalls:self];
    [[self ufw] runEmbeddedWithArgc: argumentValue argv: commandLineArguments appLaunchOpts: appLaunchOpts];
    
    return self.ufw;
}

@end
