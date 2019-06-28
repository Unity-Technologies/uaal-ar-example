#import <UIKit/UIKit.h>

#include <UnityFramework/UnityFramework.h>
#include <UnityFramework/NativeCallProxy.h>

//Interface for the Objective C functions to initialize the UnityFramework obeject.
//These Objetive-C methods are called from AppDelegate.swift and used by Cube.cs and NativeCallsProxy 

@interface UnityFrameworkBridge : UIResponder<UIApplicationDelegate, UnityFrameworkListener, NativeCallsProtocol>

@property (strong, nonatomic) id someProperty;
@property UnityFramework* ufw;
- (UnityFramework*)InitUnityFramework:(char**)commandLineArguments :(int)argumentValue;
- (void)showHostMainWindow:(NSString*)color;

@end





