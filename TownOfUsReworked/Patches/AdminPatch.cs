namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.Update))]
public static class AdminPatch
{
    private static void SetSabotaged(MapCountOverlay __instance, bool sabotaged)
    {
        __instance.isSab = sabotaged;
        __instance.BackgroundColor.SetColor(sabotaged ? Palette.DisabledGrey : UColor.green);
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

            if (HudHandler.Instance.IsCamoed)
                sprite.color = UColor.grey;

            if (sprite != null)
            {
                if (isOp)
                    PlayerMaterial.SetColors(colorMapping[i], sprite);
                else
                    PlayerMaterial.SetColors(new UColor(0.8793f, 1, 0, 1), sprite);
            }

            if (text != null && isOp && DataManager.Settings.Accessibility.ColorBlindMode)
            {
                text.gameObject.SetActive(true);
                text.text = colorMapping[i].ToString();

                // Show first row numbers below player icons
                // Show second row numbers above player icons
                // Show all icons on player icons when there are three rows

                if (useCompactText)
                    text.transform.localPosition = new(0, 0, -20);
                else if (i / area.MaxWidth == 0)
                    text.transform.localPosition = new(0, -area.YOffset, -20);
                else
                    text.transform.localPosition = new(0, area.YOffset, -20);
            }
        }
    }

    private static void UpdateBlips(MapCountOverlay __instance, bool isOp)
    {
        var rooms = Ship.FastRooms;
        var colorMapDuplicate = new List<byte>();

        foreach (var area in __instance.CountAreas)
        {
            if (!rooms.TryGetValue(area.RoomType, out var room) || room.roomArea == null)
                continue;

            var objectsInRoom = room.roomArea.OverlapCollider(__instance.filter, __instance.buffer);
            var colorMap = new List<byte>();

            for (var i = 0; i < objectsInRoom; i++)
            {
                var collider = __instance.buffer[i];

                var player = collider.GetComponent<PlayerControl>();
                var data = player?.Data;

                if (collider.tag == "DeadBody" && ((isOp && (int)CustomGameOptions.WhoSeesDead is 1) || (!isOp && (int)CustomGameOptions.WhoSeesDead is 2) || DeadSeeEverything ||
                    CustomGameOptions.WhoSeesDead == 0))
                {
                    var playerId = collider.GetComponent<DeadBody>().ParentId;
                    colorMap.Add(playerId);
                    colorMapDuplicate.Add(playerId);
                }
                else
                {
                    var component = collider.GetComponent<PlayerControl>();

                    if (component && component.Data != null && !component.HasDied() && (__instance.showLivePlayerPosition || !component.AmOwner) && !colorMapDuplicate.Contains(data.PlayerId))
                    {
                        colorMap.Add(data.PlayerId);
                        colorMapDuplicate.Add(data.PlayerId);
                    }
                }
            }

            UpdateBlips(area, colorMap, isOp);
        }
    }

    public static bool Prefix(MapCountOverlay __instance)
    {
        var localPlayer = CustomPlayer.Local;
        var isOp = localPlayer.Is(LayerEnum.Operative) || DeadSeeEverything;

        if (!isOp)
            isOp = localPlayer.Is(LayerEnum.Retributionist) && ((Retributionist)Role.LocalRole).IsOp;

        __instance.timer += Time.deltaTime;

        if (__instance.timer < 0.1f)
            return false;

        __instance.timer = 0f;
        var sabotaged = PlayerTask.PlayerHasTaskOfType<IHudOverrideTask>(localPlayer);

        if (sabotaged != __instance.isSab)
            SetSabotaged(__instance, sabotaged);

        if (!sabotaged)
            UpdateBlips(__instance, isOp);

        return false;
    }
}