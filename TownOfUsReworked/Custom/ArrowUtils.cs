namespace TownOfUsReworked.Custom;

public static class ArrowUtils
{
    public static void DestroyAll(this List<CustomArrow> list) => list.ForEach(x => x.Destroy());

    public static void DisableArrows(this PlayerControl player) => CustomArrow.AllArrows.Where(x => x.Owner == player).ForEach(x => x.Disable());

    public static void EnableArrows(this PlayerControl player) => CustomArrow.AllArrows.Where(x => x.Owner == player).ForEach(x => x.Enable());
}