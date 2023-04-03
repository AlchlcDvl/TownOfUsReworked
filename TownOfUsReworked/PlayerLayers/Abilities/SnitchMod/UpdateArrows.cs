using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Abilities.SnitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class UpdateArrows
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, AbilityEnum.Snitch))
                return;

            var snitch = Ability.GetAbility<Snitch>(PlayerControl.LocalPlayer);

            if (PlayerControl.LocalPlayer.Data.IsDead || snitch.Player.Data.IsDead)
            {
                snitch.SnitchArrows.Values.DestroyAll();
                snitch.SnitchArrows.Clear();
                snitch.ImpArrows.DestroyAll();
                snitch.ImpArrows.Clear();
            }

            foreach (var arrow in snitch.ImpArrows)
                arrow.target = snitch.Player.transform.position;

            foreach (var arrow in snitch.SnitchArrows)
            {
                var player = Utils.PlayerById(arrow.Key);

                if (player?.Data.IsDead == true || player?.Data.Disconnected == true)
                    snitch.DestroyArrow(arrow.Key);
                else
                    arrow.Value.target = player.transform.position;
            }
        }
    }
}