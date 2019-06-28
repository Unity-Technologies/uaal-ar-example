#import <Foundation/Foundation.h>
#import "NativeCallProxy.h"


@implementation FrameworkLibAPI

id<NativeCallsProtocol> api = NULL;
+(void) registerAPIforNativeCalls:(id<NativeCallsProtocol>) aApi
{
    api = aApi;
}

@end


extern "C" {
     void updateUnityShopItem() {return [api updateUnityShopItem]; }
     void itemPlacedInAR() {return [api itemPlacedInAR]; }
}

