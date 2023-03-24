using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SeerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDSee
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Seer))
                return;

            var role = Role.GetRole<Seer>(PlayerControl.LocalPlayer);

            if (role.SeerButton == null)
                role.SeerButton = Utils.InstantiateButton();

            role.SeerButton.UpdateButton(role, "SEER", role.SeerTimer(), CustomGameOptions.SeerCooldown, AssetManager.Placeholder, AbilityTypes.Direct, "ActionSecondary");

            if (role.ChangedDead && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                role.TurnSheriff();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnSheriff);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}