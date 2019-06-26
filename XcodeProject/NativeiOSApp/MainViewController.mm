#import <UIKit/UIKit.h>

#include <UnityFramework/UnityFramework.h>
#include <UnityFramework/NativeCallProxy.h>

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

void showAlert(NSString* title, NSString* msg) {
    UIAlertController* alert = [UIAlertController alertControllerWithTitle:title message:msg                                                         preferredStyle:UIAlertControllerStyleAlert];
    UIAlertAction* defaultAction = [UIAlertAction actionWithTitle:@"Ok" style:UIAlertActionStyleDefault
                                                          handler:^(UIAlertAction * action) {}];
    [alert addAction:defaultAction];
    auto delegate = [[UIApplication sharedApplication] delegate];
    [delegate.window.rootViewController presentViewController:alert animated:YES completion:nil];
}
@interface MyViewController : UIViewController
@end

@interface AppDelegate : UIResponder<UIApplicationDelegate, UnityFrameworkListener, NativeCallsProtocol>

@property (strong, nonatomic) UIWindow *window;
@property (nonatomic, strong) UIButton *BackBtn;
@property (nonatomic, strong) UIButton *ColorBtn;
@property (nonatomic, strong) MyViewController *viewController;


@property UnityFramework* ufw;
- (void)initUnity;
- (void)ShowMainView;
- (void)SelectMug;
- (void)SelectShirt;
- (NSString*)ChangeMugColor;
- (NSString*)ChangeShirtColor;
- (void)didFinishLaunching:(NSNotification*)notification;
- (void)didBecomeActive:(NSNotification*)notification;
- (void)willResignActive:(NSNotification*)notification;
- (void)didEnterBackground:(NSNotification*)notification;
- (void)willEnterForeground:(NSNotification*)notification;
- (void)willTerminate:(NSNotification*)notification;
- (void)unityDidUnloaded:(NSNotification*)notification;

@end

AppDelegate* hostDelegate = NULL;

// -------------------------------
// -------------------------------
// -------------------------------


@interface MyViewController ()
@property (nonatomic, strong) UIButton *mugBtn;
@property (nonatomic, strong) UIButton *shirtBtn;
@property (weak, nonatomic) IBOutlet UIImageView *MugDisplayImage;
@property (weak, nonatomic) IBOutlet UIImageView *ShirtDisplayImage;
@end

@implementation MyViewController
- (void)viewDidLoad
{
    [super viewDidLoad];
}

- (IBAction)OnMugButtonPressed:(id)sender {
    AppDelegate *appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    [appDelegate SelectMug];
}


- (IBAction)OnShirtButtonPressed:(id)sender {
    AppDelegate *appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    [appDelegate SelectShirt];
}

- (IBAction)OnMugColorChange:(id)sender {
    AppDelegate *appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    NSString *imagePath = [appDelegate ChangeMugColor];
    _MugDisplayImage.image = [UIImage imageNamed:imagePath];
}

- (IBAction)OnShirtColorChange:(id)sender {
    AppDelegate *appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    NSString *imagePath = [appDelegate ChangeShirtColor];
    _ShirtDisplayImage.image = [UIImage imageNamed:imagePath];
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
}

@end


// keep arg for unity init from non main
int gArgc = 0;
char** gArgv = nullptr;
NSDictionary* appLaunchOpts;

int currentItem = 0; // Mug = 0, Shirt = 1
int colorIndex[] = {0, 0}; // Mug, Shirt
NSArray *colorArray = [[NSArray alloc] initWithObjects:[UIColor whiteColor], //white
                                                       [UIColor colorWithRed:0.929f green:0.094f blue:0.278f alpha:1.0f], //magenta
                                                       [UIColor colorWithRed:0.019f green:0.733f blue:0.827f alpha:1.0f], //cyan
                                                       [UIColor colorWithRed:0.796f green:0.858f blue:0.164f alpha:1.0f],nil]; //lime

NSArray *colorStringArray = @[@"White", @"Magenta", @"Cyan", @"Lime"];

@implementation AppDelegate

- (bool)unityIsInitialized { return [self ufw] && [[self ufw] appController]; }

- (void)ShowMainView
{
    if(![self unityIsInitialized]) {
        showAlert(@"Unity is not initialized", @"Initialize Unity first");
    } else {
        [[self ufw] showUnityWindow];
    }
}

- (NSString*)getImagePath:(int)item
{
    NSString *itemString = item == 0 ? @"Mug " : @"Shirt ";
    return [NSString stringWithFormat:@"%@%@.png", itemString, colorStringArray[colorIndex[item]]];
}

- (void)showHostMainWindow
{
    [self showHostMainWindow:@""];
}

- (void)showHostMainWindow:(NSString*)color
{
    MyViewController* mainController = (MyViewController*)  self.window.rootViewController;
    mainController.MugDisplayImage.image = [UIImage imageNamed:[self getImagePath: 0]];
    mainController.ShirtDisplayImage.image = [UIImage imageNamed:[self getImagePath: 1]];

    [self.window makeKeyAndVisible];
}

- (void)updateUnityShopItem
{
    [self updateItem: currentItem];
}

- (void)updateItem:(int)itemIndex
{
    const char * itemString = [[NSString stringWithFormat:@"%d",currentItem] UTF8String ];
    [[self ufw] sendMessageToGOWithName: "AR Session Origin" functionName: "SetProduct" message: itemString];
    
    int itemColorIndex = colorIndex[itemIndex];
    const char * colorString = [ colorStringArray[itemColorIndex] UTF8String ];
    [[self ufw] sendMessageToGOWithName: "AR Session Origin" functionName: "SetColor" message: colorString];
    self.ColorBtn.backgroundColor = colorArray[itemColorIndex];
}

-(NSString*)ChangeShirtColor
{
    int index = (colorIndex[1] + 1) % 4;
    colorIndex[1] = index;
    return [self getImagePath: 1];
}

-(NSString*)ChangeMugColor
{
    int index = (colorIndex[0] + 1) % 4;
    colorIndex[0] = index;
    return [self getImagePath: 0];
}

- (void)changeColor
{
    int index = (colorIndex[currentItem] + 1) % 4;
    colorIndex[currentItem] = index;
    [self updateItem: currentItem];
}


- (void) SelectMug
{
    currentItem = 0;
    [self initUnity];
}

- (void) SelectShirt
{
    currentItem = 1;
    [self initUnity];
}

- (void)initUnity
{
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
    
    self.BackBtn = [UIButton buttonWithType: UIButtonTypeSystem];
    [self.BackBtn setTitle: @"BACK" forState: UIControlStateNormal];
    self.BackBtn.frame = CGRectMake(0, 0, 100, 44);
    self.BackBtn.center = CGPointMake(50, 25);
    self.BackBtn.backgroundColor = [UIColor whiteColor];
    [view addSubview: self.BackBtn];
    [self.BackBtn addTarget: self action: @selector(showHostMainWindow) forControlEvents: UIControlEventPrimaryActionTriggered];
    
    self.ColorBtn = [UIButton buttonWithType: UIButtonTypeSystem];
    [self.ColorBtn setTitle: @"COLOR" forState: UIControlStateNormal];
    self.ColorBtn.frame = CGRectMake(0, 0, 100, 44);
    self.ColorBtn.center = CGPointMake(325, 25);
    self.ColorBtn.backgroundColor = colorArray[colorIndex[currentItem]];
    [view addSubview: self.ColorBtn];
    [self.ColorBtn addTarget: self action: @selector(changeColor) forControlEvents: UIControlEventPrimaryActionTriggered];
}

- (void)unloadButtonTouched:(UIButton *)sender
{
    if(![self unityIsInitialized]) {
        showAlert(@"Unity is not initialized", @"Initialize Unity first");
    } else {
        [UnityFrameworkLoad() unloadApplicaion: true];
    }
}

- (void)unityDidUnload:(NSNotification*)notification
{
    NSLog(@"unityDidUnloaded called");
    
    [[self ufw] unregisterFrameworkListener: self];
    [self setUfw: nil];
    [self showHostMainWindow:@""];
}

- (void)applicationWillResignActive:(UIApplication *)application { [[[self ufw] appController] applicationWillResignActive: application]; }
- (void)applicationDidEnterBackground:(UIApplication *)application { [[[self ufw] appController] applicationDidEnterBackground: application]; }
- (void)applicationWillEnterForeground:(UIApplication *)application { [[[self ufw] appController] applicationWillEnterForeground: application]; }
- (void)applicationDidBecomeActive:(UIApplication *)application { [[[self ufw] appController] applicationDidBecomeActive: application]; }
- (void)applicationWillTerminate:(UIApplication *)application { [[[self ufw] appController] applicationWillTerminate: application]; }

@end


int main(int argc, char* argv[])
{
    gArgc = argc;
    gArgv = argv;
    
    @autoreleasepool
    {
        if (false)
        {
            // run UnityFramework as main app
            id ufw = UnityFrameworkLoad();
            
            // Set UnityFramework target for Unity-iPhone/Data folder to make Data part of a UnityFramework.framework and call to setDataBundleId
            // ODR is not supported in this case, ( if you need embedded and ODR you need to copy data )
            [ufw setDataBundleId: "com.unity3d.framework"];
            [ufw runUIApplicationMainWithArgc: argc argv: argv];
        } else {
            // run host app first and then unity later
            UIApplicationMain(argc, argv, nil, [NSString stringWithUTF8String: "AppDelegate"]);
        }
    }
    
    return 0;
}
