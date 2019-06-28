import UIKit

class ViewController: UIViewController {

    public var unityInitBtn:UIButton?
    public var unpauseBtn:UIButton?
    public var unloadBtn:UIButton?
    
    public var appDelegate: AppDelegate?
    
    override func viewDidLoad()
    {
        super.viewDidLoad()
    
        appDelegate = UIApplication.shared.delegate as? AppDelegate
        
        self.view.backgroundColor = UIColor.blue
       
        // INIT UNITY
        unityInitBtn = UIButton(type: .system)
        unityInitBtn?.setTitle("Init", for: .normal)
        unityInitBtn?.frame = CGRect(x: 0, y: 0, width: 100, height: 44)
        unityInitBtn?.center = CGPoint(x: 50, y: 120)
        unityInitBtn?.backgroundColor = UIColor.green
        unityInitBtn?.addTarget(self, action:#selector(initUnity(_:)), for: .touchUpInside)
        view.addSubview(unityInitBtn!)
        
        // SHOW UNITY
        unpauseBtn = UIButton(type: .system)
        unpauseBtn?.setTitle("Show Unity", for: .normal)
        unpauseBtn?.frame = CGRect(x: 100, y: 0, width: 100, height: 44)
        unpauseBtn?.center = CGPoint(x: 150, y: 120)
        unpauseBtn?.backgroundColor = UIColor.lightGray
        unpauseBtn?.addTarget(self, action:#selector(ShowUnityView(_:)), for: .touchUpInside)
        view.addSubview(unpauseBtn!)
        
        // UNLOAD UNITY
        unloadBtn = UIButton(type: .system)
        unloadBtn?.setTitle("Unload", for: .normal)
        unloadBtn?.frame = CGRect(x: 300, y: 0, width: 100, height: 44)
        unloadBtn?.center = CGPoint(x: 250, y: 120)
        unloadBtn?.backgroundColor = UIColor.red
        unloadBtn?.addTarget(self, action:#selector(unloadButtonTouched(_:)), for: .touchUpInside)
        view.addSubview(unloadBtn!)
    }

    //Button actions that call methods in the AppDelegate class.
    
    @objc func initUnity(_ sender: UIButton)
    {
        appDelegate?.initUnity()
    }
    
    @objc func ShowUnityView(_ sender: UIButton)
    {
        appDelegate?.ShowUnityMainView()
    }
    
    @objc func unloadButtonTouched(_ sender: UIButton)
    {
        appDelegate?.unloadButtonTouched()
    }
}

