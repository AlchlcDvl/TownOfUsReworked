using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MysticMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDReveal
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Mystic))
                return;

            var role = Role.GetRole<Mystic>(PlayerControl.LocalPlayer);

            if (role.RevealButton == null)
                role.RevealButton = Utils.InstantiateButton();

            role.RevealButton.UpdateButton(role, "REVEAL", role.RevealTimer(), CustomGameOptions.RevealCooldown, AssetManager.Reveal, AbilityTypes.Direct, "ActionSecondary");

            if (role.ConvertedDead && !PlayerControl.LocalPlayer.Data.IsDead)
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