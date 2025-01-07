namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(MapCountOverlay))]
public static class AdminPatch
{
    private static void SetSabotaged(MapCountOverlay __instance, bool sabotaged, Role role)
    {
        __instance.isSab = sabotaged;
        __instance.BackgroundColor.SetColor(sabotaged ? Palette.DisabledGrey : role.Color);
        __instance.SabotageText.gameObject.SetActive(sabotaged);

        if (sabotaged)
            __instance.CountAreas.ForEach(x => x.UpdateCount(0));
    }

    private static void UpdateBlips(CounterArea area, List<byte> colorMapping, bool isOp)
    {
        area.UpdateCount(colorMapping.Count);
        var icons = area.myIcons.ToArray();
        colorMapping.Sort();
        var useCompactText = icons.Count > 2 * area.MaxWidth;

        for (var i = 0; i < colorMapping.Count; i++)
        {
            var icon = icons[i];
            var sprite = icon.GetComponent<SpriteRenderer>();
            var text = icon.GetComponentInChildren<TextMeshPro>(true);

            if (sprite)
            {
                if (HudHandler.Instance.IsCamoed)
                    sprite.color = UColor.grey;

                if (isOp)
                    PlayerMaterial.SetColors(colorMapping[i], sprite);
                else
                    PlayerMaterial.SetColors(new UColor(0.8793f, 1, 0, 1), sprite);
            }

            if (text && isOp && DataManager.Settings.Accessibility.ColorBlindMode)
            {
                text.gameObject.SetActive(true);
                text.SetText($"{colorMapping[i]}");

                // Show first row numbers below player icons
                // Show second row numbers above player icons
                // Show all icons on player icons when there are three rows

                if (useCompactText)
                    text.transform.localPosition = new(0, 0, -20);
                else if ((i / area.MaxWidth) == 0)
                    text.transform.localPosition = new(0, -area.YOffset, -20);
                else
                    text.transform.localPosition = new(0, area.YOffset, -20);
            }
        }
    }

    private static void UpdateBlips(MapCountOverlay __instance, bool isOp)
    {
        var colorMapDuplicate = new List<byte>();

        foreach (var area in __instance.CountAreas)
        {
            if (!Ship().FastRooms.TryGetValue(area.RoomType, out var room) || !room.roomArea)
                continue;

            var objectsInRoom = room.roomArea.OverlapCollider(__instance.filter, __instance.buffer);
            var colorMap = new List<byte>();

            for (var i = 0; i < objectsInRoom; i++)
            {
                var collider = __instance.buffer[i];

                if (collider.tag == "DeadBody" && ((isOp && (int)Operative.WhoSeesDead is 1) || (!isOp && (int)Operative.WhoSeesDead is 2) || DeadSeeEverything() ||  Operative.WhoSeesDead ==
                    0))
                {
                    var playerId = collider.GetComponent<DeadBody>().ParentId;
                    colorMap.Add(playerId);
                    colorMapDuplicate.Add(playerId);
                }
                else
                {
                    var component = collider.GetComponent<PlayerControl>();
                    var data = component?.Data;

                    if (!component.HasDied() && (__instance.showLivePlayerPosition || !component.AmOwner) && !colorMapDuplicate.Contains(data.PlayerId))
                    {
                        colorMap.Add(data.PlayerId);
                        colorMapDuplicate.Add(data.PlayerId);
                    }
                }
            }

            UpdateBlips(area, colorMap, isOp);
        }
    }

    [HarmonyPatch(nameof(MapCountOverlay.Update))]
    public static bool Prefix(MapCountOverlay __instance)
    {
        var role = CustomPlayer.Local.GetRole();
        var isOp = role is Operative || DeadSeeEverything();

        if (!isOp)
            isOp = role is Retributionist ret && ret.IsOp;

        __instance.timer += Time.deltaTime;

        if (__instance.timer < 0.1f)
            return false;

        __instance.timer = 0f;
        var sabotaged = PlayerTask.PlayerHasTaskOfType<IHudOverrideTask>(role.Player);

        if (sabotaged != __instance.isSab)
            SetSabotaged(__instance, sabotaged, role);

        if (!sabotaged)
            UpdateBlips(__instance, isOp);

        return false;
    }

    [HarmonyPatch(nameof(MapCountOverlay.OnEnable))]
    public static void Postfix(MapCountOverlay __instance)
    {
        __instance.BackgroundColor.SetColor(PlayerTask.PlayerHasTaskOfType<IHudOverrideTask>(CustomPlayer.Local)
            ? Palette.DisabledGrey
            : (CustomPlayer.Local.GetRole()?.Color ?? Palette.AcceptedGreen));
    }
}