using HarmonyLib;
using Kitchen;
using Kitchen.Layouts;
using KitchenData;
using System;
using UnityEngine;

namespace KitchenHQDecor.Patches
{
    [HarmonyPatch]
    static class LayoutDecorView_Patch
    {
        [HarmonyPatch(typeof(LayoutDecorView), "UpdateDecorations")]
        [HarmonyPrefix]
        static bool UpdateDecorations_Prefix(int room, LayoutMaterialType type, int id, ref LayoutView ___LayoutView)
        {
            if (GameInfo.CurrentScene == SceneType.Franchise && id == 0)
            {
                RoomType roomType = RoomType.Unassigned;
                foreach (Room layoutRoom in ___LayoutView.Builder.Blueprint.Rooms())
                {
                    if (layoutRoom.ID == room)
                    {
                        roomType = layoutRoom.Type;
                        break;
                    }
                }
                
                LayoutPrefabSet.MaterialType materialType = new LayoutPrefabSet.MaterialType()
                {
                    Room = roomType,
                    Type = type
                };
                try
                {
                    Material newMaterial = new Material(___LayoutView.Builder.Materials.Defaults[materialType]);
                    Material currentMaterial = ___LayoutView.Builder.Materials.Get(room, type, RoomType.NoRoom);
                    currentMaterial.shader = newMaterial.shader;
                    currentMaterial.CopyPropertiesFromMaterial(newMaterial);
                }
                catch (Exception ex)
                {
                    Main.LogError($"{ex.Message}\n{ex.StackTrace}");
                }
                return false;
            }
            return true;
        }
    }
}
