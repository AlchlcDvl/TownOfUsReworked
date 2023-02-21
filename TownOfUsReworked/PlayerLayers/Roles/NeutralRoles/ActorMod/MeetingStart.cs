using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ActorMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    public class StartMeetingPatch
    {
        public static void Prefix(PlayerControl __instance)
        {
            if (__instance == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Actor))
                return;
            
            if (PlayerControl.LocalPlayer.Data.IsDead)
                return;

            var role = Role.GetRole<Actor>(PlayerControl.LocalPlayer);
            string something = "You have to pretend to be a(n)";
            string roles = "";
            int i = 0;

            foreach (var role2 in role.PretendRoles)
            {
                if (i < role.PretendRoles.Count - 1)
                    roles += $" {role2.Name}, ";
                else if (i == role.PretendRoles.Count - 1)
                    roles += $" or {role2.Name}!";
            }

            something += roles;

            //Ensures only the Actor sees this
            if (DestroyableSingleton<HudManager>.Instance && something != "")
            {
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, something);

                if (role.HasPretendTarget && role.PretendTarget != null)
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"You have to pretend {role.PretendTarget.name} is your target!");
            }
        }
    }
}