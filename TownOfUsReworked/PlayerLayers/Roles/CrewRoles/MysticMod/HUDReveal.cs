using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using Hazel;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MysticMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDReveal
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Mystic))
                return;

            var role = Role.GetRole<Mystic>(PlayerControl.LocalPlayer);

            if (role.RevealButton == null)
                role.RevealButton = CustomButtons.InstantiateButton();

            role.RevealButton.UpdateButton(role, "REVEAL", role.RevealTimer(), CustomGameOptions.RevealCooldown, AssetManager.Reveal, AbilityTypes.Direct, "ActionSecondary");

            if (Mystic.ConvertedDead && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                role.TurnSeer();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnSeer);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}