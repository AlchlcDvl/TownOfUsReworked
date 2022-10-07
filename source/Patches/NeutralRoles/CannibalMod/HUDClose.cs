/*using HarmonyLib;
using System.Linq;
using TownOfUs.Roles;
using Object = UnityEngine.Object;

namespace TownOfUs.NeutralRoles.CannibalMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            foreach (var role in Role.AllRoles.Where(x => x.RoleType == RoleEnum.Cannibal))
            {
                var cannibal = (Cannibal) role;
                cannibal.bodyArrows.DestroyAll();
                cannibal.bodyArrows.Clear();
                if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            }
        }
    }
}*/