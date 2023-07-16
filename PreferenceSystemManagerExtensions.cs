using PreferenceSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KitchenHQDecor
{
    public static  class PreferenceSystemManagerExtensions
    {
        public static PreferenceSystemManager GenerateRoomPreferences(this PreferenceSystemManager prefManager,
                List<(string roomName, string floorPrefID, string wallpaperPrefID)> roomPreferencesToGenerate, Action<int> onPreferenceChanged)
        {
            int[] floorIDs = new List<int>() { 0 }.Concat(Main.Floors.Keys).ToArray();
            string[] floorStrings = new List<string>() { "Default" }.Concat(Main.Floors.Values).ToArray();
            int[] wallpaperIDs = new List<int>() { 0 }.Concat(Main.Wallpapers.Keys).ToArray();
            string[] wallpaperStrings = new List<string>() { "Default" }.Concat(Main.Wallpapers.Values).ToArray();

            return roomPreferencesToGenerate.Aggregate(
                prefManager, (prefManager, element) => prefManager
                    .AddSubmenu(element.roomName, element.roomName.ToLowerInvariant().Replace(" ", ""))
                        .AddLabel(element.roomName)
                        .AddSpacer()
                        .AddLabel("Floor")
                        .AddOption<int>(
                            element.floorPrefID,
                            0,
                            floorIDs,
                            floorStrings,
                            onPreferenceChanged)
                        .AddLabel("Wallpaper")
                        .AddOption<int>(
                            element.wallpaperPrefID,
                            0,
                            wallpaperIDs,
                            wallpaperStrings,
                            onPreferenceChanged)
                        .AddSpacer()
                        .AddSpacer()
                    .SubmenuDone());
        }
    }
}
