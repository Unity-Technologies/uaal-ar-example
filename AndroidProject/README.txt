1. Export Android Project, from UnityProject
- open UnityProject
- switch to Android platform
- export project check box
- Export

2. Add Unity Module by modifying settings.gradle of NativeAndroidApp
- open Android Studio for NativeAndroidApp
- add to settings.gradle at the end of file, change path to {Exported project}\unityLibrary from step #1
include ':unityLibrary'
project(':unityLibrary').projectDir=new File('C:\\Projects\\UaaLExample\\UnityProject\\Export\\unityLibrary')

3. Add dependencie on :unityLibrary for :app
- add next to dependencies block in build.gradle for :app module
  implementation project(':unityLibrary')

Project prepared