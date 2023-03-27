using HarmonyLib;
using Reactor.Utilities.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.AltruistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class UpdateArrows
    {
        public static void Postfix()
        {
            if (Coroutine.Arrow != null)
            {
                if (LobbyBehaviour.Instance || MeetingHud.Instance || PlayerControl.LocalPlayer.Data.IsDead || Coroutine.Target.Data.IsDead)
                {
                    Coroutine.Arrow.gameObject.Destroy();
                    Coroutine.Target = null;
                }
                else
                    Coroutine.Arrow.target = Coroutine.Target.transform.position;
            }
        }
    }
}