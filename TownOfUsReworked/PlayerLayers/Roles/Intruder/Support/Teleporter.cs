namespace TownOfUsReworked.PlayerLayers.Roles;

public class Teleporter : Intruder
{
    public CustomButton TeleportButton { get; set; }
    public DateTime LastTeleported { get; set; }
    public DateTime LastMarked { get; set; }
    public CustomButton MarkButton { get; set; }
    public bool CanMark { get; set; }
    public Vector3 TeleportPoint { get; set; }

    public override Color32 Color => ClientGameOptions.CustomIntColors ? Colors.Teleporter : Colors.Intruder;
    public override string Name => "Teleporter";
    public override LayerEnum Type => LayerEnum.Teleporter;
    public override Func<string> StartText => () => "X Marks The Spot";
    public override Func<string> Description => () => $"- You can mark a spot to teleport to later\n{CommonAbilities}";
    public override InspectorResults InspectorResults => InspectorResults.MovesAround;
    public float TeleportTimer => ButtonUtils.Timer(Player, LastTeleported, CustomGameOptions.TeleportCd);
    public float MarkTimer => ButtonUtils.Timer(Player, LastMarked, CustomGameOptions.MarkCooldown);

    public Teleporter(PlayerControl player) : base(player)
    {
        TeleportPoint = Vector3.zero;
        MarkButton = new(this, "Mark", AbilityTypes.Effect, "Secondary", Mark);
        TeleportButton = new(this, "Teleport", AbilityTypes.Effect, "Secondary", Teleport);
    }

    public void Mark()
    {
        if (!CanMark || MarkTimer != 0f || TeleportPoint == Player.transform.position)
            return;

        TeleportPoint = Player.transform.position;
        LastMarked = DateTime.UtcNow;

        if (CustomGameOptions.TeleCooldownsLinked)
            LastTeleported = DateTime.UtcNow;
    }

    public void Teleport()
    {
        if (TeleportTimer != 0f || TeleportPoint == Player.transform.position || TeleportPoint == Vector3.zero)
            return;

        CallRpc(CustomRPC.Action, ActionsRPC.Teleport, this, TeleportPoint);
        LastTeleported = DateTime.UtcNow;
        Utils.Teleport(Player, TeleportPoint);

        if (CustomGameOptions.TeleCooldownsLinked)
            LastMarked = DateTime.UtcNow;
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        var hits = Physics2D.OverlapBoxAll(Player.transform.position, GetSize(), 0);
        hits = hits.Where(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer is not 8 and not 5).ToArray();
        CanMark = hits.Count == 0 && Player.moveable && !GetPlayerElevator(Player).IsInElevator && TeleportPoint != Player.transform.position;
        MarkButton.Update("MARK", MarkTimer, CustomGameOptions.MarkCooldown, CanMark);
        TeleportButton.Update("TELEPORT", TeleportTimer, CustomGameOptions.TeleportCd, true, TeleportPoint != Vector3.zero && TeleportPoint != Player.transform.position);
    }
}