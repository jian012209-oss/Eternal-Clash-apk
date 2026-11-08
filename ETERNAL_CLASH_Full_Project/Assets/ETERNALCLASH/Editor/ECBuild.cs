using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Adds an "Eternal Clash" menu to the Unity top bar with one-click build buttons.
public static class ECBuild {
    const string ScenePath = "Assets/ETERNALCLASH/Scenes/Main.unity";

    [MenuItem("Eternal Clash/1. Create Main Scene")]
    public static void CreateScene() {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        new GameObject("Game").AddComponent<EternalClash.ECBootstrap>();
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        Debug.Log("Eternal Clash: scene created at " + ScenePath + ". Press Play to test.");
    }

    static void Configure(bool appBundle) {
        if (!System.IO.File.Exists(ScenePath)) CreateScene();
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        PlayerSettings.companyName = "EternalClash";
        PlayerSettings.productName = "Eternal Clash";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.eternalclash.game");
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        EditorUserBuildSettings.buildAppBundle = appBundle;
    }

    static void Build(string path, bool appBundle) {
        Configure(appBundle);
        System.IO.Directory.CreateDirectory("Builds");
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = new[] { ScenePath },
            locationPathName = path,
            target = BuildTarget.Android,
            options = BuildOptions.None
        });
        Debug.Log("Eternal Clash build: " + report.summary.result + " -> " + path);
        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            EditorUtility.RevealInFinder(path);
    }

    [MenuItem("Eternal Clash/2. Build APK (GitHub / direct install)")]
    public static void BuildApk() { Build("Builds/EternalClash.apk", false); }

    [MenuItem("Eternal Clash/3. Build AAB (Google Play Store)")]
    public static void BuildAab() { Build("Builds/EternalClash.aab", true); }
}
