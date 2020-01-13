#import <UIKit/UIKit.h>

#include <UnityFramework/UnityFramework.h>
#include <UnityFramework/NativeCallProxy.h>

//Set up UnitFramework for use in AppDelegate.
UnityFramework* UnityFrameworkLoad() {
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

@interface MyViewController : UIViewController
@end

@interface AppDelegate : UIResponder<UIApplicationDelegate, UnityFrameworkListener, NativeCallsProtocol>

@property (strong, nonatomic) UIWindow *window;
@property (nonatomic, strong) UIButton *BackBtn;
@property (nonatomic, strong) UIButton *ColorBtn;
@property (nonatomic, strong) MyViewController *viewController;

@property UnityFramework* ufw;
- (void)initUnity;
- (void)SelectMug;
- (void)SelectShirt;
- (void)UnloadUnity;
- (NSString*)ChangeMugColor;
- (NSString*)ChangeShirtColor;

-(UIColor*)ChangeButtonColor:(int)itemIndex;
-(void)UpdateColorButton:(UIButton*)button :(UIColor*)colorToChange;
-(void)unityDidUnload:(NSNotification*)notification;

@end

AppDelegate* hostDelegate = NULL;

// -------------------------------
// -------------------------------
// -------------------------------

@interface MyViewController ()
@property (weak, nonatomic) IBOutlet UIButton *UnloadButton;
@property (weak, nonatomic) IBOutlet UIImageView *MugDisplayImage;
@property (weak, nonatomic) IBOutlet UIImageView *ShirtDisplayImage;
@property (weak, nonatomic) IBOutlet UIButton *shirtColorChangeBtn;
@property (weak, nonatomic) IBOutlet UIButton *mugColorChangeBtn;
@end

@implementation MyViewController
- (void)viewDidLoad {
    [super viewDidLoad];
    _UnloadButton.enabled = false;
    _UnloadButton.backgroundColor = UIColor.grayColor;
}

//"Show in AR" button under Mug, this will launch Unity AR with mug item selected.
- (IBAction)OnMugButtonPressed:(id)sender {
    AppDelegate *appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    [appDelegate SelectMug];
}

//"Show in AR" button under Shirt, this will launch Unity AR with shirt item selected.
- (IBAction)OnShirtButtonPressed:(id)sender {
    AppDelegate *appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    [appDelegate SelectShirt];
}

//Color change button for mug, this will change the color of the mug and
//_mugColorChangeBtn will be set to the color of the next mug.
- (IBAction)OnMugColorChange:(id)sender {
    AppDelegate *appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    NSString *imagePath = [appDelegate ChangeMugColor];
    _MugDisplayImage.image = [UIImage imageNamed:imagePath];
    
    UIColor *nextColourButtonColour = [appDelegate ChangeButtonColor:0];
    [appDelegate UpdateColorButton:_mugColorChangeBtn :nextColourButtonColour];
}

//Color change button for shirt, this will change the color of the shirt and
//_shirtColorChangeBtn will be set to the color of the next shirt.
- (IBAction)OnShirtColorChange:(id)sender {
    AppDelegate *appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    NSString *imagePath = [appDelegate ChangeShirtColor];
    _ShirtDisplayImage.image = [UIImage imageNamed:imagePath];
    
    UIColor *nextColourButtonColour = [appDelegate ChangeButtonColor:1];
    [appDelegate UpdateColorButton:_shirtColorChangeBtn :nextColourButtonColour];
}

- (IBAction)UnloadUnity:(id)sender {
        AppDelegate *appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    [appDelegate UnloadUnity];
    _UnloadButton.enabled = false;
    _UnloadButton.backgroundColor = UIColor.grayColor;
    
}

- (void)didReceiveMemoryWarning {
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

- (NSString*)getImagePath:(int)item {
    NSString *itemString = item == 0 ? @"Mug " : @"Shirt ";
    return [NSString stringWithFormat:@"%@%@.png", itemString, colorStringArray[colorIndex[item]]];
}

//Called when "Back" button from native iOS overlay on Unity AR view is pressed.
//Updates UI and sends a message to unity to clear item.
- (void)showHostMainWindow{
    MyViewController* mainController = (MyViewController*)  self.window.rootViewController;
    mainController.MugDisplayImage.image = [UIImage imageNamed:[self getImagePath: 0]];
    mainController.ShirtDisplayImage.image = [UIImage imageNamed:[self getImagePath: 1]];
    
    [self UpdateColorButton:mainController.mugColorChangeBtn :colorArray[(colorIndex[0] + 1) % colorArray.count]];
    [self UpdateColorButton:mainController.shirtColorChangeBtn :colorArray[(colorIndex[1] + 1) % colorArray.count]];
    
    [self.window makeKeyAndVisible];
    
    [[self ufw] sendMessageToGOWithName: "AR Session Origin" functionName: "ClearPlacedItem" message:""];
    self.ColorBtn.hidden = true;
}

//Changes the tint of the color select buttons in the main view.
-(void)UpdateColorButton:(UIButton*)button :(UIColor*)colorToChange {
    if(colorToChange == UIColor.whiteColor)
    {
        //Set button image to white button
        NSString* whiteButtonImagePath = @"colour_button_white.png";
        [button setImage:[UIImage imageNamed:whiteButtonImagePath] forState:UIControlStateNormal];
    }
    else
    {
        NSString* colourButtonImagePath = @"colour_button_colour.png";
        [button setImage:[UIImage imageNamed:colourButtonImagePath] forState:UIControlStateNormal];
        button.tintColor = colorToChange;
    }
}

//Message called from Unity, this will unhide the "Color" change button from the native iOS overlay
//when Unity AR is running in app.
- (void)itemPlacedInAR {
    self.ColorBtn.hidden = false;
}

//Sends message to Unity to update item in AR.
- (void)updateUnityShopItem {
    [self updateItem: currentItem];
}

//Sends message to Unity to update item in AR
- (void)updateItem:(int)itemIndex {
    const char * itemString = [[NSString stringWithFormat:@"%d",currentItem] UTF8String ];
    [[self ufw] sendMessageToGOWithName: "AR Session Origin" functionName: "SetProduct" message: itemString];
    
    int itemColorIndex = colorIndex[itemIndex];
    const char * colorString = [ colorStringArray[itemColorIndex] UTF8String ];
    [[self ufw] sendMessageToGOWithName: "AR Session Origin" functionName: "SetColor" message: colorString];
    self.ColorBtn.tintColor = colorArray[(colorIndex[itemIndex] + 1) % colorArray.count];
}

-(NSString*)ChangeShirtColor {
    int index = (colorIndex[1] + 1) % colorArray.count;
    colorIndex[1] = index;
    return [self getImagePath: 1];
}

-(NSString*)ChangeMugColor {
    int index = (colorIndex[0] + 1) % colorArray.count;
    colorIndex[0] = index;
    return [self getImagePath: 0];
}

-(UIColor*)ChangeButtonColor:(int)itemIndex {
    return colorArray[(colorIndex[itemIndex] + 1) % colorArray.count];
}

//Called from "Color" button in Native iOS overlay on Unity AR.
//This will send a message to Unity AR to update the item currently being viewed.
- (void)changeColor {
    int index = (colorIndex[currentItem] + 1) % 4;
    colorIndex[currentItem] = index;
    [self updateItem: currentItem];
}

- (void) SelectMug {
    currentItem = 0;
    [self initUnity];
}

- (void) SelectShirt {
    currentItem = 1;
    [self initUnity];
}

//Called from "Show in AR" buttons to initialize unity, or update item being viewed.
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
    
    NSString* backButtonImagePath = @"button_back.png";
    NSString* colourButtonPath = @"button_colour.png";
    
    self.BackBtn = [UIButton buttonWithType: UIButtonTypeSystem];
    [self.BackBtn setImage:[UIImage imageNamed:backButtonImagePath] forState:UIControlStateNormal];
    [self.BackBtn setTitle: @"BACK" forState: UIControlStateNormal];
    self.BackBtn.frame = CGRectMake(0, 0, 200, 110);
    self.BackBtn.center = CGPointMake(80, 40);
    self.BackBtn.backgroundColor = UIColor.clearColor;
    self.BackBtn.tintColor = UIColor.whiteColor;
    [view addSubview: self.BackBtn];
    [self.BackBtn addTarget: self action: @selector(showHostMainWindow) forControlEvents: UIControlEventPrimaryActionTriggered];
    
    self.ColorBtn = [UIButton buttonWithType: UIButtonTypeSystem];
    [self.ColorBtn setImage:[UIImage imageNamed:colourButtonPath] forState:UIControlStateNormal];

    [self.ColorBtn setTitle: @"COLOR" forState: UIControlStateNormal];
    self.ColorBtn.frame = CGRectMake(0, 0, 230, 110);
    self.ColorBtn.center = CGPointMake(280, 40);
    self.ColorBtn.backgroundColor = UIColor.clearColor;
    [view addSubview: self.ColorBtn];
    [self.ColorBtn addTarget: self action: @selector(changeColor) forControlEvents: UIControlEventPrimaryActionTriggered];
    self.ColorBtn.hidden = true;
    
    MyViewController* mainController = (MyViewController*)  self.window.rootViewController;
    mainController.UnloadButton.enabled = true;
    mainController.UnloadButton.backgroundColor = [UIColor colorWithRed:0.0f green:0.478f blue:1.0f alpha:1.0f];
}

- (void)UnloadUnity
{
    if([self unityIsInitialized]){
        [UnityFrameworkLoad() unloadApplication];
    }
}

- (void)unityDidUnload:(NSNotification*)notification
{
    NSLog(@"unityDidUnloaded called");
    
    [[self ufw] unregisterFrameworkListener: self];
    [self setUfw: nil];
    [self showHostMainWindow];
}

- (void)applicationWillResignActive:(UIApplication *)application { [[[self ufw] appController] applicationWillResignActive: application]; }
- (void)applicationDidEnterBackground:(UIApplication *)application { [[[self ufw] appController] applicationDidEnterBackground: application]; }
- (void)applicationWillEnterForeground:(UIApplication *)application { [[[self ufw] appController] applicationWillEnterForeground: application]; }
- (void)applicationDidBecomeActive:(UIApplication *)application { [[[self ufw] appController] applicationDidBecomeActive: application]; }
- (void)applicationWillTerminate:(UIApplication *)application { [[[self ufw] appController] applicationWillTerminate: application]; }

@end

int main(int argc, char* argv[]) {
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
