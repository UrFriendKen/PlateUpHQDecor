using HarmonyLib;
using Kitchen;
using KitchenData;
using KitchenMods;
using PreferenceSystem;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

// Namespace should have "Kitchen" in the beginning
namespace KitchenHQDecor
{
    public class Main : IModInitializer
    {
        public const string MOD_GUID = $"IcedMilo.PlateUp.{MOD_NAME}";
        public const string MOD_NAME = "HQ Decor";
        public const string MOD_VERSION = "0.1.1";

        public const string BEDROOM1_FLOOR_ID = "bedroom1Floor";
        public const string BEDROOM1_WALL_ID = "bedroom1Wall";
        public const string BEDROOM2_FLOOR_ID = "bedroom2Floor";
        public const string BEDROOM2_WALL_ID = "bedroom2Wall";
        public const string BEDROOM3_FLOOR_ID = "bedroom3Floor";
        public const string BEDROOM3_WALL_ID = "bedroom3Wall";
        public const string BEDROOM4_FLOOR_ID = "bedroom4Floor";
        public const string BEDROOM4_WALL_ID = "bedroom4Wall";
        public const string CONTRACTS_FLOOR_ID = "contractsFloor";
        public const string CONTRACTS_WALL_ID = "contractsWall";
        public const string GARAGE_FLOOR_ID = "garageFloor";
        public const string GARAGE_WALL_ID = "garageWall";
        public const string KITCHEN_FLOOR_ID = "kitchenFloor";
        public const string KITCHEN_WALL_ID = "kitchenWall";
        public const string LOCATIONS_FLOOR_ID = "locationsFloor";
        public const string LOCATIONS_WALL_ID = "locationsWall";
        public const string OFFICE_FLOOR_ID = "officeFloor";
        public const string OFFICE_WALL_ID = "officeWall";
        public const string STATS_FLOOR_ID = "statsFloor";
        public const string STATS_WALL_ID = "statsWall";
        public const string WORKSHOP_FLOOR_ID = "workshopFloor";
        public const string WORKSHOP_WALL_ID = "workshopWall";

        internal static PreferenceSystemManager PrefManager;

        private static Queue<bool> ChangeQueue = new Queue<bool>();
        internal static bool HasPreferenceChanged => ChangeQueue.Count > 0;
        internal static void HandleChange()
        {
            if (ChangeQueue.Count > 0)
                ChangeQueue.Dequeue();
        }

        public Main()
        {
            Harmony harmony = new Harmony(MOD_GUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        internal static Dictionary<int, string> Floors;
        internal static Dictionary<int, string> Wallpapers;

        public void PostActivate(KitchenMods.Mod mod)
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");

        }

        public void PreInject()
        {
            IEnumerable<Decor> decors = GameData.Main.Get<Decor>();
            Floors = decors.Where(x => x.Type == LayoutMaterialType.Floor).ToDictionary(x => x.ID, x => x.name);
            Wallpapers = decors.Where(x => x.Type == LayoutMaterialType.Wallpaper).ToDictionary(x => x.ID, x => x.name);

            List<(string roomName, string floorPrefID, string wallpaperPrefID)> roomPreferencesToGenerate = new List<(string, string, string)>()
            {
                ("Bedroom 1", BEDROOM1_FLOOR_ID, BEDROOM1_WALL_ID),
                ("Bedroom 2", BEDROOM2_FLOOR_ID, BEDROOM2_WALL_ID),
                ("Bedroom 3", BEDROOM3_FLOOR_ID, BEDROOM3_WALL_ID),
                ("Bedroom 4", BEDROOM4_FLOOR_ID, BEDROOM4_WALL_ID),
                ("Contracts", CONTRACTS_FLOOR_ID, CONTRACTS_WALL_ID),
                ("Garage", GARAGE_FLOOR_ID, GARAGE_WALL_ID),
                ("Kitchen", KITCHEN_FLOOR_ID, KITCHEN_WALL_ID),
                ("Locations", LOCATIONS_FLOOR_ID, LOCATIONS_WALL_ID),
                ("Office", OFFICE_FLOOR_ID, OFFICE_WALL_ID),
                ("Stats", STATS_FLOOR_ID, STATS_WALL_ID),
                ("Workshop", WORKSHOP_FLOOR_ID, WORKSHOP_WALL_ID)
            };

            PrefManager = new PreferenceSystemManager(MOD_GUID, MOD_NAME);
            PrefManager
                .AddConditionalBlocker(() => GameInfo.CurrentScene == SceneType.Franchise)
                    .AddInfo("Only Available for Host and in HQ")
                .ConditionalBlockerDone()
                .AddConditionalBlocker(() => GameInfo.CurrentScene != SceneType.Franchise)
                    .AddConditionalBlocker(() => Session.CurrentGameNetworkMode != GameNetworkMode.Host)
                    .GenerateRoomPreferences(roomPreferencesToGenerate, OnPreferenceChanged)
                    .ConditionalBlockerDone()
                    .AddConditionalBlocker(() => Session.CurrentGameNetworkMode == GameNetworkMode.Host)
                        .AddInfo("Only Available for Host")
                    .ConditionalBlockerDone()
                    .AddSpacer()
                    .AddSpacer()
                .ConditionalBlockerDone();

            PrefManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);
        }

        private void OnPreferenceChanged(int _)
        {
            ChangeQueue?.Enqueue(true);
        }

        public void PostInject()
        {
        }

        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
