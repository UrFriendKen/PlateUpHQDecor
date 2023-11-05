using Kitchen;
using KitchenData;
using KitchenMods;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQDecor
{
    public class FranchiseApplyDecor : FranchiseSystem, IModSystem
    {
        EntityQuery ChangeDecorEvents;

        List<Vector3> Positions
        {
            get 
            {
                List<Vector3> positions = new List<Vector3>(LobbyPositionAnchors.Bedrooms);
                positions.AddRange(new List<Vector3>()
                {
                    LobbyPositionAnchors.Contracts,
                    LobbyPositionAnchors.StartMarker,
                    LobbyPositionAnchors.Kitchen,
                    GetFrontDoor(),
                    LobbyPositionAnchors.Office,
                    LobbyPositionAnchors.Stats + Vector3.back,
                    LobbyPositionAnchors.Workshop
                });
                return positions;

            }
        }

        List<int> FloorPrefs
        {
            get
            {
                return new List<int>()
                {
                    Main.PrefManager.Get<int>(Main.BEDROOM1_FLOOR_ID),
                    Main.PrefManager.Get<int>(Main.BEDROOM2_FLOOR_ID),
                    Main.PrefManager.Get<int>(Main.BEDROOM3_FLOOR_ID),
                    Main.PrefManager.Get<int>(Main.BEDROOM4_FLOOR_ID) ,
                    Main.PrefManager.Get<int>(Main.CONTRACTS_FLOOR_ID) ,
                    Main.PrefManager.Get<int>(Main.GARAGE_FLOOR_ID) ,
                    Main.PrefManager.Get<int>(Main.KITCHEN_FLOOR_ID) ,
                    Main.PrefManager.Get<int>(Main.LOCATIONS_FLOOR_ID) ,
                    Main.PrefManager.Get<int>(Main.OFFICE_FLOOR_ID) ,
                    Main.PrefManager.Get<int>(Main.STATS_FLOOR_ID) ,
                    Main.PrefManager.Get<int>(Main.WORKSHOP_FLOOR_ID)
                };
            }
        }

        List<int> WallPrefs
        {
            get
            {
                return new List<int>()
                {
                    Main.PrefManager.Get<int>(Main.BEDROOM1_WALL_ID),
                    Main.PrefManager.Get<int>(Main.BEDROOM2_WALL_ID),
                    Main.PrefManager.Get<int>(Main.BEDROOM3_WALL_ID),
                    Main.PrefManager.Get<int>(Main.BEDROOM4_WALL_ID) ,
                    Main.PrefManager.Get<int>(Main.CONTRACTS_WALL_ID) ,
                    Main.PrefManager.Get<int>(Main.GARAGE_WALL_ID) ,
                    Main.PrefManager.Get<int>(Main.KITCHEN_WALL_ID) ,
                    Main.PrefManager.Get<int>(Main.LOCATIONS_WALL_ID) ,
                    Main.PrefManager.Get<int>(Main.OFFICE_WALL_ID) ,
                    Main.PrefManager.Get<int>(Main.STATS_WALL_ID) ,
                    Main.PrefManager.Get<int>(Main.WORKSHOP_WALL_ID)
                };
            }
        }

        protected override void Initialise()
        {
            base.Initialise();
            ChangeDecorEvents = GetEntityQuery(typeof(CChangeDecorEvent));
            RequireSingletonForUpdate<SLayout>();
        }

        protected override void OnUpdate()
        {
            if (!ChangeDecorEvents.IsEmpty && !Main.HasPreferenceChanged)
                return;
            Main.HandleChange();
            Dictionary<int, int> selectedFloors = new Dictionary<int, int>();
            Dictionary<int, int> selectedWallpapers = new Dictionary<int, int>();

            var prefs = FloorPrefs.Zip(WallPrefs, (f, w) => new { Floor = f, Wall = w });
            var zipped = Positions.Zip(prefs, (pos, pref) => new { Pos = pos, Prefs = pref });

            foreach (var item in zipped)
            {
                int roomID = GetTile(item.Pos).RoomID;
                if (selectedFloors.ContainsKey(roomID))
                    continue;
                selectedFloors.Add(roomID, item.Prefs.Floor);
                selectedWallpapers.Add(roomID, item.Prefs.Wall);
            }

            using NativeArray<Entity> entities = ChangeDecorEvents.ToEntityArray(Allocator.Temp);
            using NativeArray<CChangeDecorEvent> changeDecorEvents = ChangeDecorEvents.ToComponentDataArray<CChangeDecorEvent>(Allocator.Temp);
            for (int i = changeDecorEvents.Length - 1; i > -1; i--)
            {
                Entity e = entities[i];
                CChangeDecorEvent evt = changeDecorEvents[i];
                int decorID = 0;
                if (evt.Type == LayoutMaterialType.Floor && selectedFloors.TryGetValue(evt.RoomID, out decorID))
                {
                    selectedFloors.Remove(evt.RoomID);
                }
                else if (evt.Type == LayoutMaterialType.Wallpaper && selectedWallpapers.TryGetValue(evt.RoomID, out decorID))
                {
                    selectedWallpapers.Remove(evt.RoomID);
                }
                evt.DecorID = decorID;
                Set(e, evt);
            }

            foreach (KeyValuePair<int, int> remainingFloor in selectedFloors)
            {
                Entity changeFloorEntity = EntityManager.CreateEntity();
                Set(changeFloorEntity, new CChangeDecorEvent()
                {
                    RoomID = remainingFloor.Key,
                    DecorID = remainingFloor.Value,
                    Type = LayoutMaterialType.Floor
                });
            }
            foreach (KeyValuePair<int, int> remainingWallpaper in selectedWallpapers)
            {
                Entity changeFloorEntity = EntityManager.CreateEntity();
                Set(changeFloorEntity, new CChangeDecorEvent()
                {
                    RoomID = remainingWallpaper.Key,
                    DecorID = remainingWallpaper.Value,
                    Type = LayoutMaterialType.Wallpaper
                });
            }
        }
    }
}
