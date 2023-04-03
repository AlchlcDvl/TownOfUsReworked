using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Abilities.RadarMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class UpdateArrow
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, AbilityEnum.Radar))
                return;

            var radar = Ability.GetAbility<Radar>(PlayerControl.LocalPlayer);

            if (radar.Player.Data.IsDead)
            {
                radar.RadarArrow.DestroyAll();
                radar.RadarArrow.Clear();
            }

            foreach (var arrow in radar.RadarArrow)
            {
                radar.ClosestPlayer = CustomButtons.GetClosestPlayer(PlayerControl.LocalPlayer, null, float.MaxValue);
                arrow.target = radar.ClosestPlayer.transform.position;
            }
        }
    }
}