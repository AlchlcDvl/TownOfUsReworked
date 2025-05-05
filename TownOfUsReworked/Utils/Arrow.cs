namespace TownOfUsReworked.Utils;

public static class ArrowUtils
{
    public static void DestroyAll(this IEnumerable<CustomArrow> list) => list.ForEach(x => x.Destroy());
}