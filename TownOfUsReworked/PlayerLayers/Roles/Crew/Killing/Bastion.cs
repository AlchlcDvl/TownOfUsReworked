namespace TownOfUsReworked.PlayerLayers.Roles;

public class Bastion : Crew
{
    public CustomButton BombButton { get; set; }
    public List<int> BombedIDs { get; set; }

    public override Color Color => ClientGameOptions.CustomCrewColors ? Colors.Bastion : Colors.Crew;
    public override string Name => "Bastion";
    public override LayerEnum Type => LayerEnum.Bastion;
    public override Func<string> StartText => () => "Place Traps To Deter Venters";
    public override Func<string> Description => () => "- You can place traps in vents, which trigger and kill whenever someone uses the vent the trap is in";

    public Bastion(PlayerControl player) : base(player)
    {
        Alignment = Alignment.CrewKill;
        BombedIDs = new();
        BombButton = new(this, $"{SpriteName}VentBomb", AbilityTypes.Vent, "ActionSecondary", Bomb, CustomGameOptions.BastionCd, CustomGameOptions.MaxBombs, Exception);
    }

    public static string SpriteName => MapPatches.CurrentMap switch
    {
        2 => "Polus",
        5 => "Plant",
        _ => "Metal"
    };

    public bool Exception(Vent vent) => BombedIDs.Contains(vent.Id);

    public void Bomb()
    {
        var interact = Interact(Player, BombButton.TargetVent);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
        {
            BombedIDs.Add(BombButton.TargetVent.Id);
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, BombButton.TargetVent);
        }

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        BombButton.StartCooldown(cooldown);
    }

    public override void ReadRPC(MessageReader reader) => BombedIDs.Add(reader.ReadInt32());

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        BombButton.Update2("PLACE BOMB");
    }
}