namespace TownOfUsReworked.Custom;

[HarmonyPatch]
public static class ArrowUtils
{
    public static void DestroyAll(this List<CustomArrow> list) => list.ForEach(x => x.Destroy());

    public static void DisableArrows(this PlayerControl player) => CustomArrow.AllArrows.Where(x => x.Owner == player).ToList().ForEach(x => x.Disable());

    public static void EnableArrows(this PlayerControl player) => CustomArrow.AllArrows.Where(x => x.Owner == player).ToList().ForEach(x => x.Enable());
}