namespace TownOfUsReworked.Custom
{
    [HarmonyPatch]
    public static class ArrowUtils
    {
        public static void DestroyAll(this List<CustomArrow> list)
        {
            foreach (var arrow in list)
                arrow.Destroy();
        }
    }
}