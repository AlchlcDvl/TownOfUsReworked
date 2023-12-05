namespace TownOfUsReworked.PlayerLayers.Roles;

public class Monarch : Crew
{
    public bool RoundOne { get; set; }
    public CustomButton KnightButton { get; set; }
    public List<byte> ToBeKnighted { get; set; }
    public List<byte> Knighted { get; set; }
    public bool Protected => Knighted.Count > 0;

    public override Color Color => ClientGameOptions.CustomCrewColors ? Colors.Monarch : Colors.Crew;
    public override string Name => "Monarch";
    public override LayerEnum Type => LayerEnum.Monarch;
    public override Func<string> StartText => () => "Knight Those Who You Trust";
    public override Func<string> Description => () => $"- You can knight players\n- Knighted players will have their votes count {CustomGameOptions.KnightVoteCount + 1} times\n- As long as "
        + "a knight is alive, you cannot be killed";

    public Monarch(PlayerControl player) : base(player)
    {
        Alignment = Alignment.CrewSov;
        Knighted = new();
        ToBeKnighted = new();
        KnightButton = new(this, "Knight", AbilityTypes.Target, "ActionSecondary", Knight, CustomGameOptions.KnightingCd, Exception, CustomGameOptions.KnightCount);
    }

    public void Knight()
    {
        var interact = Interact(Player, KnightButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, KnightButton.TargetPlayer.PlayerId);
            ToBeKnighted.Add(KnightButton.TargetPlayer.PlayerId);
        }

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        KnightButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => ToBeKnighted.Contains(player.PlayerId) || player.IsKnighted();

    public override void ReadRPC(MessageReader reader) => ToBeKnighted.Add(reader.ReadByte());

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        KnightButton.Update2("KNIGHT", !RoundOne);
    }
}