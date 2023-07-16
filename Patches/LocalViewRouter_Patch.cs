using HarmonyLib;
using Kitchen;
using System.Reflection;
using UnityEngine;

namespace KitchenHQDecor.Patches
{
    [HarmonyPatch]
    static class LocalViewRouter_Patch
    {
        [HarmonyPatch(typeof(LocalViewRouter), "GetPrefab")]
        [HarmonyPostfix]
        static void GetPrefab_Postfix(ViewType view_type, ref GameObject __result)
        {
            FieldInfo f_LayoutView = typeof(LayoutDecorView).GetField("LayoutView", BindingFlags.NonPublic | BindingFlags.Instance);

            if (view_type == ViewType.FranchiseFloorplan && __result.GetComponent<LayoutDecorView>() == null)
            {
                LayoutView layoutView = __result.GetComponent<LayoutView>();
                if (layoutView != null)
                {
                    LayoutDecorView layoutDecorView = __result.AddComponent<LayoutDecorView>();
                    f_LayoutView.SetValue(layoutDecorView, layoutView);
                }
            }
        }
    }
}
