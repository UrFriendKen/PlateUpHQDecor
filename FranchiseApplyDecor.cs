using Kitchen;
using KitchenData;
using KitchenMods;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenHQDecor
{
    public class FranchiseApplyDecor : FranchiseSystem, IModSystem
    {
        EntityQuery ChangeDecorEvents;

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
            Dictionary<int, int> selectedFloors = new Dictionary<int, int>()
            {
                { GetTile(new Vector3(8f, 0f, 6f)).RoomID, Main.PrefManager.Get<int>(Main.BEDROOM1_FLOOR_ID) },
                { GetTile(new Vector3(8f, 0f, 3f)).RoomID, Main.PrefManager.Get<int>(Main.BEDROOM2_FLOOR_ID) },
                { GetTile(new Vector3(8f, 0f, 0f)).RoomID, Main.PrefManager.Get<int>(Main.BEDROOM3_FLOOR_ID) },
                { GetTile(new Vector3(8f, 0f, -3f)).RoomID, Main.PrefManager.Get<int>(Main.BEDROOM4_FLOOR_ID) },
                { GetTile(new Vector3(-4f, 0f, 5f)).RoomID, Main.PrefManager.Get<int>(Main.CONTRACTS_FLOOR_ID) },
                { GetTile(new Vector3(-8f, 0f, -6f)).RoomID, Main.PrefManager.Get<int>(Main.GARAGE_FLOOR_ID) },
                { GetTile(new Vector3(1f, 0f, 6f)).RoomID, Main.PrefManager.Get<int>(Main.KITCHEN_FLOOR_ID) },
                { GetTile(new Vector3(0f, 0f, 0f)).RoomID, Main.PrefManager.Get<int>(Main.LOCATIONS_FLOOR_ID) },
                { GetTile(new Vector3(-1f, 0f, -6f)).RoomID, Main.PrefManager.Get<int>(Main.OFFICE_FLOOR_ID) },
                { GetTile(new Vector3(7f, 0f, -6f)).RoomID, Main.PrefManager.Get<int>(Main.STATS_FLOOR_ID) },
                { GetTile(new Vector3(-8f, 0f, 1f)).RoomID, Main.PrefManager.Get<int>(Main.WORKSHOP_FLOOR_ID) }
            };
            Dictionary<int, int> selectedWallpapers = new Dictionary<int, int>()
            {
                { GetTile(new Vector3(8f, 0f, 6f)).RoomID, Main.PrefManager.Get<int>(Main.BEDROOM1_WALL_ID) },
                { GetTile(new Vector3(8f, 0f, 3f)).RoomID, Main.PrefManager.Get<int>(Main.BEDROOM2_WALL_ID) },
                { GetTile(new Vector3(8f, 0f, 0f)).RoomID, Main.PrefManager.Get<int>(Main.BEDROOM3_WALL_ID) },
                { GetTile(new Vector3(8f, 0f, -3f)).RoomID, Main.PrefManager.Get<int>(Main.BEDROOM4_WALL_ID) },
                { GetTile(new Vector3(-4f, 0f, 5f)).RoomID, Main.PrefManager.Get<int>(Main.CONTRACTS_WALL_ID) },
                { GetTile(new Vector3(-8f, 0f, -6f)).RoomID, Main.PrefManager.Get<int>(Main.GARAGE_WALL_ID) },
                { GetTile(new Vector3(1f, 0f, 6f)).RoomID, Main.PrefManager.Get<int>(Main.KITCHEN_WALL_ID) },
                { GetTile(new Vector3(0f, 0f, 0f)).RoomID, Main.PrefManager.Get<int>(Main.LOCATIONS_WALL_ID) },
                { GetTile(new Vector3(-1f, 0f, -6f)).RoomID, Main.PrefManager.Get<int>(Main.OFFICE_WALL_ID) },
                { GetTile(new Vector3(7f, 0f, -6f)).RoomID, Main.PrefManager.Get<int>(Main.STATS_WALL_ID) },
                { GetTile(new Vector3(-8f, 0f, 1f)).RoomID, Main.PrefManager.Get<int>(Main.WORKSHOP_WALL_ID) }
            };

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
