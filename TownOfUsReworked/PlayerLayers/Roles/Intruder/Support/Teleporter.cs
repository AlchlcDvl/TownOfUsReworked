namespace TownOfUsReworked.PlayerLayers.Roles;

public class Teleporter : Intruder
{
    public CustomButton TeleportButton { get; set; }
    public CustomButton MarkButton { get; set; }
    public Vector3 TeleportPoint { get; set; }

    public override Color Color => ClientGameOptions.CustomIntColors ? Colors.Teleporter : Colors.Intruder;
    public override string Name => "Teleporter";
    public override LayerEnum Type => LayerEnum.Teleporter;
    public override Func<string> StartText => () => "X Marks The Spot";
    public override Func<string> Description => () => $"- You can mark a spot to teleport to later\n{CommonAbilities}";

    public Teleporter(PlayerControl player) : base(player)
    {
        TeleportPoint = Vector3.zero;
        MarkButton = new(this, "Mark", AbilityTypes.Targetless, "Secondary", Mark, CustomGameOptions.TeleMarkCd);
        TeleportButton = new(this, "Teleport", AbilityTypes.Targetless, "Secondary", Teleport, CustomGameOptions.TeleportCd);
    }

    public void Mark()
    {
        TeleportPoint = Player.transform.position;
        MarkButton.StartCooldown();

        if (CustomGameOptions.TeleCooldownsLinked)
            TeleportButton.StartCooldown();
    }

    public void Teleport()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.Teleport, Player, TeleportPoint);
        Utils.Teleport(Player, TeleportPoint);
        TeleportButton.StartCooldown();

        if (CustomGameOptions.TeleCooldownsLinked)
            MarkButton.StartCooldown();
    }

    public bool Condition()
    {
        var hits = Physics2D.OverlapBoxAll(Player.transform.position, GetSize(), 0);
        hits = hits.Where(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer is not (8 or 5)).ToArray();
        return hits.Count == 0 && Player.moveable && !GetPlayerElevator(Player).IsInElevator && TeleportPoint != Player.transform.position;
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        MarkButton.Update2("MARK SPOT", condition: Condition());
        TeleportButton.Update2("TELEPORT", TeleportPoint != Vector3.zero);
    }
}