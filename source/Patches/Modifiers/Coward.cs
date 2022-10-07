using HarmonyLib;

namespace TownOfUs.Modifiers
{
    public class Coward
    {
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class HudManagerUpdate
        {
            public static void Postfix(HudManager __instance)
            {
                if (PlayerControl.LocalPlayer.Is(ModifierEnum.Coward))
                {
                    try
                    {
                        DestroyableSingleton<HudManager>.Instance.ReportButton.SetActive(false);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}