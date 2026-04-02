using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class DesktopPlayModeGuard
{
    static DesktopPlayModeGuard()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode)
            return;

        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        bool isMobileTarget =
            target == BuildTarget.Android ||
            target == BuildTarget.iOS;

        if (!isMobileTarget)
            return;

        EditorApplication.isPlaying = false;

        EditorUtility.DisplayDialog(
            "Desktop-only project",
            "This project is intended for desktop play mode only.\n\nSwitch the active build target to Windows or macOS before pressing Play.",
            "OK");
    }
}