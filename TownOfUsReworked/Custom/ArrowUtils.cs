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

        public static void DisableArrows(this PlayerControl player)
        {
            foreach (var arrow in CustomArrow.AllArrows.Where(x => x.Owner == player))
                arrow.Disable();
        }

        public static void EnableArrows(this PlayerControl player)
        {
            foreach (var arrow in CustomArrow.AllArrows.Where(x => x.Owner == player))
                arrow.Enable();
        }
    }
}