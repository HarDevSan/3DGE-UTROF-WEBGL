//
// ShaderGraphEssentials for Unity
// (c) 2019 PH Graphics
// Source code may be used and modified for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 
// *** A NOTE ABOUT PIRACY ***
// 
// If you got this asset from a pirate site, please consider buying it from the Unity asset store. This asset is only legally available from the Unity Asset Store.
// 
// I'm a single indie dev supporting my family by spending hundreds and thousands of hours on this and other assets. It's very offensive, rude and just plain evil to steal when I (and many others) put so much hard work into the software.
// 
// Thank you.
//
// *** END NOTE ABOUT PIRACY ***
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.WSA;
using Application = UnityEngine.Application;

namespace ShaderGraphEssentials
{
    [InitializeOnLoad]
    public class GettingStartedWindowShow
    {
        private const string SettingsPath = "Assets/Plugins/ShaderGraphEssentials/Settings.asset";
        static GettingStartedWindowShow()
        {
            var settings = AssetDatabase.LoadAssetAtPath(SettingsPath, typeof(SGESettings)) as SGESettings;
            bool showWindow = !(settings && !settings.OpenGettingStartedWindow);
            if (showWindow)
            {
                EditorApplication.update += OnUpdate;

            }
        }

        private static void OnUpdate()
        {
            EditorApplication.update -= OnUpdate;
            
            SGESettings newSettings = ScriptableObject.CreateInstance<SGESettings>();
            AssetDatabase.CreateAsset(newSettings, SettingsPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            GettingStartedWindow window = (GettingStartedWindow)EditorWindow.GetWindow(typeof(GettingStartedWindow));
            window.Show();
        }
    }
    
    
    public class GettingStartedWindow : EditorWindow
    {
        private const string SGEVersion = "1.1.8";
        
        private const string SGEPath = "Assets/Plugins/ShaderGraphEssentials";
        private const string SGELogoFileName = "Plugin/Editor/GettingStarted/Data/SGE_key_128x128.png";
        private const string ManualFileName = "ShaderGraphEssentials_Documentation.pdf";
        private const string ChangeLogName = "ChangeLog.txt";

        private const string NoiseScene = BaseDemoPath + "/Scenes/ShaderGraphEssentials_Demo.unity";
        private const string SimpleLitScene = URPDemoPath + "/Scenes/ShaderGraphEssentials_Showcase_SimpleLit.unity";
        private const string ToonLitScene = URPDemoPath + "/Scenes/ShaderGraphEssentials_Showcase_ToonLit.unity";
        private const string WaterScene = URPDemoPath + "/Scenes/ShaderGraphEssentials_Showcase_Water.unity";

        private const string DemoFolderPath = "DemoScenes";
        private const string BaseDemoPath = "DemoScenes/Demo_Base";
        private const string URPDemoPath = "DemoScenes/Demo_URP";

        private const string BasePluginPath = "Plugin/Editor/Plugin_Base";
        private const string URPPluginPath = "Plugin/Editor/Plugin_URP";

        private const string URPPluginPackagePath = "Packages/SGE_URP.unitypackage";
        private const string URPDemoPackagePath = "Packages/SGE_URP_Demo.unitypackage";
        private const string HDRPPluginPackagePath = "Packages/SGE_HDRP.unitypackage";
        private const string HDRPDemoPackagePath = "Packages/SGE_HDRP_Demo.unitypackage";

        private GUIStyle _wrapLabelStyle;

        private Texture2D _logoTexture;

        private bool _hasError;
        
        // various sizes
        private const int LogoTextureSize = 128;
        private const int Margin = 10;
        private const int ButtonHeight = 30;
        private const int ButtonWidth = 120;
        private const int LargeButtonWidth = 160;
        
        private Vector2 _scrollPosition = Vector2.zero;
        private Vector2 _defaultWindowSize = Vector2.zero;

        private bool _initialized = false;
        
        [MenuItem("Tools/ShaderGraph Essentials/Getting Started")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            GettingStartedWindow window = (GettingStartedWindow)EditorWindow.GetWindow(typeof(GettingStartedWindow));
            window.Show();
        }

        private bool GetInternalFile(string pathFromSGE, out string fullPath)
        {
            fullPath = Path.Combine(SGEPath, pathFromSGE);
            if (File.Exists(fullPath)) 
                return true;
            
            Debug.LogError("File " + fullPath + " doesn't exist. Did you move the ShaderGraphEssentials root folder from Assets/ ? Unfortunately this isn't supported yet.");
            _hasError = true;
            return false;
        }

        private void Awake()
        {
            string fullLogoPath;
            if (!GetInternalFile(SGELogoFileName, out fullLogoPath))
            {
                return;
            }

            _logoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(fullLogoPath);
        }

        private bool OpenFileWithDefaultEditor(string path)
        {
            string fullPath;
            if (!GetInternalFile(path, out fullPath))
            {
                return false;
            }

            fullPath = Path.GetFullPath(fullPath);

            if (!File.Exists(fullPath))
            {
                Debug.LogError("File " + fullPath + " doesn't exist. Did you move the ShaderGraphEssentials root folder from Assets/ ? Unfortunately this isn't supported yet.");
                _hasError = true;
                return false;
            }
            
#if UNITY_EDITOR_WIN
            System.Diagnostics.Process.Start($@"{fullPath}");
#elif UNITY_EDITOR_OSX
            EditorUtility.RevealInFinder($@"{fullPath}");
#endif

            return true;
        }

        private bool CheckForErrors()
        {
            if (_hasError)
            {
                GUILayout.Label(
                    "There was an error constructing this window. Please check your console for errors. If you can't fix it, please don't hesitate to ask for support");
                return true;
            }

            return false;
        }

        private void InitializeWindow()
        {
            titleContent.text = "Getting Started";
            minSize = new Vector2(250, 400);
            
            _defaultWindowSize = new Vector2(520, 800);
            Vector2 initialPosition = 0.5f * (new Vector2(Screen.currentResolution.width, Screen.currentResolution.height) - _defaultWindowSize);
            position = new Rect(initialPosition, _defaultWindowSize);
            
            _wrapLabelStyle = new GUIStyle(EditorStyles.label) {wordWrap = true};
        }

        void OnGUI()
        {
            if (!_initialized)
            {
                InitializeWindow();
                _initialized = true;
            }

            if (CheckForErrors()) return;

            _scrollPosition = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), _scrollPosition,
                new Rect(0, 0, _defaultWindowSize.x - 10, _defaultWindowSize.y - 10), false, false);

            float yOffset = 0;
            float defaultXSize = position.width - Margin - Margin;
            
            // Header
            GUI.BeginGroup(new Rect(Margin, Margin, defaultXSize, LogoTextureSize));

            float xOffset = 0;
            GUI.DrawTexture(new Rect(xOffset, 0, LogoTextureSize, LogoTextureSize), _logoTexture);
            xOffset += LogoTextureSize + Margin;
            
            GUI.Label(new Rect(xOffset, 0, 100, 30), "Version: " + SGEVersion);
            if (GUI.Button(new Rect(xOffset, 30, ButtonWidth, ButtonHeight), "View Changelog"))
            {
                OpenChangelog();
                if (CheckForErrors()) return;
            }
            
            if (GUI.Button(new Rect(xOffset, 30 + ButtonHeight + Margin, ButtonWidth, ButtonHeight), "View Manual"))
            {
                OpenManual();
                if (CheckForErrors()) return;
            }

            xOffset += ButtonWidth + Margin;
            
            if (GUI.Button(new Rect(xOffset, 30, LargeButtonWidth, ButtonHeight), "View Offline Changelog"))
            {
                OpenFileWithDefaultEditor(ChangeLogName);
                if (CheckForErrors()) return;
            }
            
            if (GUI.Button(new Rect(xOffset, 30 + ButtonHeight + Margin, LargeButtonWidth, ButtonHeight), "View Offline Manual"))
            {
                OpenFileWithDefaultEditor(ManualFileName);
                if (CheckForErrors()) return;
            }
            
            GUI.EndGroup();

            yOffset += Margin + LogoTextureSize; 
            
            GUI.Label(new Rect(Margin, yOffset + Margin, defaultXSize, 10), "", GUI.skin.horizontalSlider);

            yOffset += Margin + Margin;
            
            // Getting started title
            yOffset += Margin;
            GUI.BeginGroup(new Rect(Margin, yOffset, defaultXSize, 20));
            
            GUI.Label(new Rect(defaultXSize / 2f - 50, 0, 100, 20), "Getting Started", EditorStyles.largeLabel);
            
            GUI.EndGroup();

            yOffset += Margin + 20;
            
            // URP

            GUI.BeginGroup(new Rect(Margin, yOffset, defaultXSize, 260));
            
            GUI.Label(new Rect(0, 0, defaultXSize, 30), "URP", EditorStyles.boldLabel);

            if (GUI.Button(new Rect(0, 30, defaultXSize, ButtonHeight), "Import URP plugin"))
            {
                ImportURPPlugin();
            }

            bool isURPImported = IsURPImported();

            using (new EditorGUI.DisabledScope(!isURPImported))
            {
                if (GUI.Button(new Rect(0, ButtonHeight * 2 + Margin, defaultXSize, ButtonHeight), "Import URP Demo scenes"))
                {
                    ImportURPScenes();
                }
            }
            
            bool areURPScenesImported = AreURPScenesImported();

            using (new EditorGUI.DisabledScope(!areURPScenesImported))
            {
                if (GUI.Button(new Rect(0, ButtonHeight * 3 + Margin * 2, defaultXSize, ButtonHeight), "Open noise scene"))
                {
                    OpenScene(NoiseScene);
                }
                if (GUI.Button(new Rect(0, ButtonHeight * 4 + Margin * 3, defaultXSize, ButtonHeight), "Open simple lit scene"))
                {
                    OpenScene(SimpleLitScene);
                }
                if (GUI.Button(new Rect(0, ButtonHeight * 5 + Margin * 4, defaultXSize, ButtonHeight), "Open toon lit scene"))
                {
                    OpenScene(ToonLitScene);
                }
                if (GUI.Button(new Rect(0, ButtonHeight * 6 + Margin * 5, defaultXSize, ButtonHeight), "Open water scene"))
                {
                    OpenScene(WaterScene);
                }
            }

            GUI.EndGroup();

            yOffset += 260;
            
            // HDRP
            yOffset += Margin;
            
            GUI.BeginGroup(new Rect(Margin, yOffset, defaultXSize, 140));
            
            GUI.Label(new Rect(0, 0, defaultXSize, 30), "HDRP", EditorStyles.boldLabel);

            if (GUI.Button(new Rect(0, 30, defaultXSize, ButtonHeight), "Import HDRP plugin"))
            {
                ImportHDRPPlugin();
            }

            bool isHDRPImported = IsHDRPImported();

            using (new EditorGUI.DisabledScope(!isHDRPImported))
            {
                if (GUI.Button(new Rect(0, ButtonHeight * 2 + Margin, defaultXSize, ButtonHeight), "Import HDRP Demo scenes"))
                {
                    ImportHDRPScenes();
                }
            }
            
            bool areHDRPScenesImported = AreHDRPScenesImported();

            using (new EditorGUI.DisabledScope(!areHDRPScenesImported))
            {
                if (GUI.Button(new Rect(0, ButtonHeight * 3 + Margin * 2, defaultXSize, ButtonHeight), "Open noise scene"))
                {
                    OpenScene(NoiseScene);
                }
            }

            GUI.EndGroup();
            
            yOffset += 140;
            
            // switch URP / HDRP
            yOffset += Margin;
            
            GUI.BeginGroup(new Rect(Margin, yOffset, defaultXSize, 90));
            
            GUI.Label(new Rect(0, 0, defaultXSize, 30), "Switch between URP / HDRP", EditorStyles.boldLabel);
            
            GUI.Label(new Rect(0, 30, defaultXSize, 30), "To switch between URP / HDRP, hit the button below and reimport the correct plugin"
            , _wrapLabelStyle);

            if (GUI.Button(new Rect(0, 60, defaultXSize, ButtonHeight), "Remove plugins and demo scenes"))
            {
                CleanEverything();
            }

            GUI.EndGroup();
            
            yOffset += 90;
            
            GUI.Label(new Rect(Margin, yOffset + Margin, defaultXSize, 10), "", GUI.skin.horizontalSlider);

            yOffset += Margin + Margin;
            
            // Help title
            yOffset += Margin;
            GUI.BeginGroup(new Rect(Margin, yOffset, defaultXSize, 30));
            
            GUI.Label(new Rect(defaultXSize / 2f - 15, 0, 100, 20), "Help", EditorStyles.largeLabel);
            
            GUI.EndGroup();

            yOffset += Margin + 20;
            
            // Help
            GUI.BeginGroup(new Rect(Margin, yOffset, defaultXSize, ButtonHeight));
            
            if (GUI.Button(new Rect(0, 0, defaultXSize / 2 - Margin, ButtonHeight), "Discord"))
            {
                OpenDiscordHelp();
            }
            
            if (GUI.Button(new Rect(defaultXSize / 2 + Margin, 0, defaultXSize / 2 - Margin, ButtonHeight), "Email"))
            {
                OpenEmailHelp();
            }
            
            
            GUI.EndGroup();
            
            GUI.EndScrollView();
        }

        private void OpenEmailHelp()
        {
            Application.OpenURL("mailto:ph.graphics.unity@gmail.com");
        }

        private void OpenDiscordHelp()
        {
            Application.OpenURL("https://discord.gg/ksURBah");
        }

        private void OpenManual()
        {
            Application.OpenURL("http://assetstore.phbarralis.com/sge/features.html");
        }

        private void OpenChangelog()
        {
            Application.OpenURL("http://assetstore.phbarralis.com/sge/changelog.html");
        }

        private void ImportHDRPScenes()
        {
            ImportPackage(HDRPDemoPackagePath);
        }

        private bool AreHDRPScenesImported()
        {
            return Directory.Exists(Path.Combine(SGEPath, BaseDemoPath));
        }

        private bool IsHDRPImported()
        {
            return Directory.Exists(Path.Combine(SGEPath, BasePluginPath));
        }

        private void ImportHDRPPlugin()
        {
            ImportPackage(HDRPPluginPackagePath);
        }

        private void OpenScene(string sceneName)
        {
            string fullPath;
            if (!GetInternalFile(sceneName, out fullPath))
                return;
            
            EditorSceneManager.OpenScene(fullPath, OpenSceneMode.Single);
        }

        private bool AreURPScenesImported()
        {
            return Directory.Exists(Path.Combine(SGEPath, URPDemoPath));
        }

        private bool IsURPImported()
        {
            return Directory.Exists(Path.Combine(SGEPath, URPPluginPath));
        }

        private void ImportURPScenes()
        {
            ImportPackage(URPDemoPackagePath);
        }

        private void ImportURPPlugin()
        {
            ImportPackage(URPPluginPackagePath);
        }

        private void ImportPackage(string packagePath)
        {
            string fullPath;
            if (!GetInternalFile(packagePath, out fullPath))
                return;
            
            AssetDatabase.ImportPackage(fullPath, true);
        }

        private void DeleteIfFolderExist(string directoryPath)
        {
            string fullpath = Path.Combine(SGEPath, directoryPath);
            if (Directory.Exists(fullpath))
                FileUtil.DeleteFileOrDirectory(fullpath);
        }

        private void CleanEverything()
        {
            DeleteIfFolderExist(DemoFolderPath);

            DeleteIfFolderExist(BasePluginPath);
            DeleteIfFolderExist(URPPluginPath);
            
            AssetDatabase.Refresh();
        }
    }
}
