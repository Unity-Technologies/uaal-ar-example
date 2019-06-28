import UIKit
import CoreData
import UnityFramework

@UIApplicationMain
class AppDelegate: UIResponder, UIApplicationDelegate , UnityFrameworkListener{
    
    
    var window:UIWindow?
    var showUnityOffButton:UIButton?
    var btnSendMsg:UIButton?
    var navVC:UINavigationController?
    var unloadBtn:UIButton?
    var viewController:ViewController?
    
    var unitOC: UnityFrameworkBridge?
    var ufw:UnityFramework?

    var backgroundColor: String?
    
    func application(_ application: UIApplication, didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?) -> Bool
    {
        // Override point for customization after application launch.
        self.unitOC = UnityFrameworkBridge();
        self.window = UIWindow(frame: UIScreen.main.bounds)
        self.window?.backgroundColor = UIColor.red
        self.viewController = ViewController()
        if let viewController = viewController {
            navVC = UINavigationController(rootViewController: viewController)
        }
        self.window?.rootViewController = navVC
        self.window?.makeKeyAndVisible()
        
        
        
        return true
    }
    
    func unityIsInitialized() -> Bool { return (self.ufw != nil) && (self.ufw!.appController() != nil) }

    //Show the unity instance from the native iOS view.
    @objc func ShowUnityMainView()
    {
        if(unityIsInitialized())
        {
            self.ufw?.showUnityWindow()
        }
        else
        {
            showAlert(title: "Unity is not initialized",msg: "Initialize Unity")
        }
    }
    
    //Called from Unity C# when "Show Main With Color" button in unity scene is pressed
    @objc func showHostMainWindow()
    {
        showHostMainWindow("")
    }
    
    //Sets the unpauseBtn background colour in the Native iOS ViewController to the color of the cube in unity.
    //The iOS view will then return to the starting ViewController.
    //This is the method that gets the message from Unity to iOS.
    //Called via Cube.cs -> UnityFrameworkBridge.m
    @objc func showHostMainWindow(_ color: String!)
    {
        if (color == "blue") {
            self.viewController?.unpauseBtn?.backgroundColor = UIColor.blue
        } else if (color == "red") {
            self.viewController?.unpauseBtn?.backgroundColor = UIColor.red
        } else if (color == "yellow") {
            self.viewController?.unpauseBtn?.backgroundColor = UIColor.yellow
        }
        self.window?.makeKeyAndVisible()
    }
    
    //Attached to native iOS "Send Msg" button overlayed ontop of Unity. This will print the cube colour in unity
    //from an iOS button press.
    @objc func sendMsgToUnity()
    {
        self.ufw!.sendMessageToGO(withName: "Cube", functionName: "ChangeColor", message: "yellow")
    }
    
    //Set up the UnityFramework and add native iOS buttons over the instance of unity.
    @objc func initUnity()
    {
        if(unityIsInitialized())
        {
            showAlert(title: "Unity already initialized", msg: "Unload Unity first")
            return
        }
        
        let gArgc = CommandLine.argc
        let gArgv = CommandLine.unsafeArgv
        self.ufw = self.unitOC?.initUnityFramework(gArgv, gArgc)
        
        let view = self.ufw?.appController()?.rootView
       
        showUnityOffButton = UIButton(type: .system)
        showUnityOffButton?.setTitle("Show Main", for: .normal)
        showUnityOffButton?.frame = CGRect(x: 0, y: 0, width: 100, height: 44)
        showUnityOffButton?.center = CGPoint(x: 50, y: 300)
        showUnityOffButton?.backgroundColor = UIColor.green
        view?.addSubview(showUnityOffButton!)
        showUnityOffButton?.addTarget(self, action: #selector(AppDelegate.showHostMainWindow as (AppDelegate) -> () -> ()), for: .touchUpInside)

        btnSendMsg = UIButton(type: .system)
        btnSendMsg?.setTitle("Send Msg", for: .normal)
        btnSendMsg?.frame = CGRect(x: 0, y: 0, width: 100, height: 44)
        btnSendMsg?.center = CGPoint(x: 150, y: 300)
        btnSendMsg?.backgroundColor = UIColor.yellow
        view?.addSubview(btnSendMsg!)
        btnSendMsg?.addTarget(self, action: #selector(AppDelegate.sendMsgToUnity), for: .touchUpInside)
        
        unloadBtn = UIButton(type: .system)
        unloadBtn?.setTitle("Unload", for: .normal)
        unloadBtn?.frame = CGRect(x: 250, y: 0, width: 100, height: 44)
        unloadBtn?.center = CGPoint(x: 250, y: 300)
        unloadBtn?.backgroundColor = UIColor.red
        view?.addSubview(unloadBtn!)
        unloadBtn?.addTarget(self, action: #selector(AppDelegate.unloadButtonTouched), for: .touchUpInside)
    }
    
    //Unload annd unregister UnityFramework, then return to the main ViewController screen.
    @objc func unloadButtonTouched(){
        if(!unityIsInitialized())
        {
            showAlert(title: "Unity is not initialized", msg: "Initialize Unity first")
        }
        else
        {
            self.ufw?.unloadApplicaion(true)
            self.ufw?.unregisterFrameworkListener(self)
            self.ufw = nil
            showHostMainWindow("")
        }
    }
    
    func showAlert(title: String?, msg: String?) {
        let alert = UIAlertController(title: title, message: msg, preferredStyle: .alert)
        let defaultAction = UIAlertAction(title: "Ok", style: .default, handler: { action in
        })
        alert.addAction(defaultAction)
        let delegate = UIApplication.shared.delegate
        delegate?.window??.rootViewController?.present(alert, animated: true)
    }
    
    //////////////////////////////////////////////////////////
    //Generated AppDelegate functions when project was created
    //////////////////////////////////////////////////////////
    
    func applicationWillResignActive(_ application: UIApplication) {
        // Sent when the application is about to move from active to inactive state. This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) or when the user quits the application and it begins the transition to the background state.
        // Use this method to pause ongoing tasks, disable timers, and invalidate graphics rendering callbacks. Games should use this method to pause the game.
        self.ufw?.appController().applicationWillResignActive(application)
    }

    func applicationDidEnterBackground(_ application: UIApplication) {
        // Use this method to release shared resources, save user data, invalidate timers, and store enough application state information to restore your application to its current state in case it is terminated later.
        // If your application supports background execution, this method is called instead of applicationWillTerminate: when the user quits.
        self.ufw?.appController().applicationDidEnterBackground(application)
    }

    func applicationWillEnterForeground(_ application: UIApplication) {
        // Called as part of the transition from the background to the active state; here you can undo many of the changes made on entering the background.
        self.ufw?.appController().applicationWillEnterForeground(application)
    }

    func applicationDidBecomeActive(_ application: UIApplication) {
        // Restart any tasks that were paused (or not yet started) while the application was inactive. If the application was previously in the background, optionally refresh the user interface.
        self.ufw?.appController().applicationDidBecomeActive(application)
    }

    func applicationWillTerminate(_ application: UIApplication) {
        // Called when the application is about to terminate. Save data if appropriate. See also applicationDidEnterBackground:.
        // Saves changes in the application's managed object context before the application terminates.
        self.saveContext()
        self.ufw?.appController().applicationWillTerminate(application)
    }

    // MARK: - Core Data stack

    lazy var persistentContainer: NSPersistentContainer = {
        /*
         The persistent container for the application. This implementation
         creates and returns a container, having loaded the store for the
         application to it. This property is optional since there are legitimate
         error conditions that could cause the creation of the store to fail.
        */
        let container = NSPersistentContainer(name: "SwiftObjective")
        container.loadPersistentStores(completionHandler: { (storeDescription, error) in
            if let error = error as NSError? {
                // Replace this implementation with code to handle the error appropriately.
                // fatalError() causes the application to generate a crash log and terminate. You should not use this function in a shipping application, although it may be useful during development.
                 
                /*
                 Typical reasons for an error here include:
                 * The parent directory does not exist, cannot be created, or disallows writing.
                 * The persistent store is not accessible, due to permissions or data protection when the device is locked.
                 * The device is out of space.
                 * The store could not be migrated to the current model version.
                 Check the error message to determine what the actual problem was.
                 */
                fatalError("Unresolved error \(error), \(error.userInfo)")
            }
        })
        return container
    }()

    // MARK: - Core Data Saving support

    func saveContext () {
        let context = persistentContainer.viewContext
        if context.hasChanges {
            do {
                try context.save()
            } catch {
                // Replace this implementation with code to handle the error appropriately.
                // fatalError() causes the application to generate a crash log and terminate. You should not use this function in a shipping application, although it may be useful during development.
                let nserror = error as NSError
                fatalError("Unresolved error \(nserror), \(nserror.userInfo)")
            }
        }
    }

}

