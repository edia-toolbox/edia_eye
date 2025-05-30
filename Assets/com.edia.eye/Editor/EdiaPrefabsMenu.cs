using UnityEditor;
using UnityEngine;

namespace com.edia.eye.Editor {
    /// <summary>
    /// Represents a utility menu for managing prefab-related functionality
    /// within the Edia framework's editor tools.
    /// </summary>
    /// <remarks>
    /// This class provides organizational functionality for configuring and
    /// interacting with prefabs through a specialized editor menu. It is
    /// intended to extend and support editor-based workflows within the
    /// Edia ecosystem.
    /// </remarks>
    public class EdiaPrefabsMenu {

#region Data

        [MenuItem("GameObject/EDIA/Eye/Data/Eye-DataHandler", false, 1)]
        private static void CreatePrefabEyeDataHandler(MenuCommand menuCommand) {
            InstantiatePrefab("Data", "Eye-DataHandler", menuCommand);
        }

        [MenuItem("GameObject/EDIA/Eye/Data/Eye-UxfTracker", false, 1)]
        private static void CreatePrefabEyeUxfTracker(MenuCommand menuCommand) {
            InstantiatePrefab("Data", "Eye-UxfTracker", menuCommand);
        }

        [MenuItem("GameObject/EDIA/Eye/Data/Eye-LslStreamer", false, 1)]
        private static void CreatePrefabEyeLslStreamer(MenuCommand menuCommand) {
            InstantiatePrefab("Data", "Eye-LslStreamer", menuCommand);
        }

#endregion
#region Interaction

        [MenuItem("GameObject/EDIA/Eye/Interaction/Eye-XRGazeInteractor", false, 1)]
        private static void CreatePrefabEyeXRGazeInteractor(MenuCommand menuCommand) {
            InstantiatePrefab("Interaction", "Eye-XRGazeInteractor", menuCommand);
        }

#endregion
#region Helpers

        [MenuItem("GameObject/EDIA/Eye/Helpers/Eye-AvatarWithEyeBalls", false, 1)]
        private static void CreatePrefabEyeAvatarWithEyeBalls(MenuCommand menuCommand) {
            InstantiatePrefab("Helpers", "Eye-AvatarWithEyeBalls", menuCommand);
        }

        [MenuItem("GameObject/EDIA/Eye/Helpers/Eye-DummyDataGenerator_AllEyes", false, 1)]
        private static void CreatePrefabEyeDummyDataGeneratorAllEyes(MenuCommand menuCommand) {
            InstantiatePrefab("Helpers", "Eye-DummyDataGenerator_AllEyes", menuCommand);
        }

        [MenuItem("GameObject/EDIA/Eye/Helpers/Eye-DummyDataGenerator_CenterEye", false, 1)]
        private static void CreatePrefabEyeDummyDataGeneratorCenterEye(MenuCommand menuCommand) {
            InstantiatePrefab("Helpers", "Eye-DummyDataGenerator_CenterEye", menuCommand);
        }

        [MenuItem("GameObject/EDIA/Eye/Helpers/Eye-GazeIntersectionVisualizer", false, 1)]
        private static void CreatePrefabEyeGazeIntersectionVisualizer(MenuCommand menuCommand) {
            InstantiatePrefab("Helpers", "Eye-GazeIntersectionVisualizer", menuCommand);
        }

        [MenuItem("GameObject/EDIA/Eye/Helpers/Eye-GazeVisualizer", false, 1)]
        private static void CreatePrefabEyeGazeVisualizer(MenuCommand menuCommand) {
            InstantiatePrefab("Helpers", "Eye-GazeVisualizer", menuCommand);
        }

#endregion
#region UI

        [MenuItem("GameObject/EDIA/Eye/UI/Eye-UIPanel", false, 1)]
        private static void CreatePrefabEyeUIPanel(MenuCommand menuCommand) {
            InstantiatePrefab("UI", "Eye-UIPanel", menuCommand);
        }


#endregion

        private static void InstantiatePrefab(string subFolder, string prefabName, MenuCommand command) {
            var prefab = Resources.Load<GameObject>($"Prefabs/{subFolder}/{prefabName}");
            if (prefab == null) {
                Debug.LogError($"Prefab '{prefabName}' not found in Resources/{subFolder}");
                return;
            }

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            GameObjectUtility.SetParentAndAlign(instance, command.context as GameObject);
            Undo.RegisterCreatedObjectUndo(instance, $"Create {prefabName}");
            Selection.activeObject = instance;
        }
    }
}