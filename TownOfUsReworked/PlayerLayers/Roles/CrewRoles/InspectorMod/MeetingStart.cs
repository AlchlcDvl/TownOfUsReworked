using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.InspectorMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead || !PlayerControl.LocalPlayer.Is(RoleEnum.Inspector))
                return;

            var role = Role.GetRole<Inspector>(PlayerControl.LocalPlayer);

            foreach (var (player, results) in role.InspectResults)
            {
                string roles = "";
                var position = 0;

                foreach (var result in results)
                {
                    if (position < results.Count - 1)
                        roles += $" {result.Name},";
                    else if (position == results.Count - 1)
                        roles += $" or {result.Name}.";
                    
                    position++;
                }
                
                string something = $"{player.name} could be" + roles;
                
                //Ensures only the Inspector sees this
                if (DestroyableSingleton<HudManager>.Instance)
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, something);

            }

            role.InspectResults.Clear();
            role.InspectedPlayers.Clear();
        }
    }
}
