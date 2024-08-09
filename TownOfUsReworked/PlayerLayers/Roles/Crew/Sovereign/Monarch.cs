namespace TownOfUsReworked.PlayerLayers.Roles;

public class Monarch : Crew
{
    public bool RoundOne { get; set; }
    public CustomButton KnightButton { get; set; }
    public List<byte> ToBeKnighted { get; set; }
    public List<byte> Knighted { get; set; }
    public bool Protected => Knighted.Any();

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Monarch : CustomColorManager.Crew;
    public override string Name => "Monarch";
    public override LayerEnum Type => LayerEnum.Monarch;
    public override Func<string> StartText => () => "Knight Those Who You Trust";
    public override Func<string> Description => () => $"- You can knight players\n- Knighted players will have their votes count {CustomGameOptions.KnightVoteCount + 1} times\n- As long as "
        + "a knight is alive, you cannot be killed";
    public override DefenseEnum DefenseVal => Knighted.Any(x => !PlayerById(x).HasDied()) ? DefenseEnum.Basic : DefenseEnum.None;

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.CrewSov;
        Knighted = [];
        ToBeKnighted = [];
        KnightButton = CreateButton(this, "KNIGHT", new SpriteName("Knight"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Knight, new Cooldown(CustomGameOptions.KnightingCd),
            (PlayerBodyExclusion)Exception, CustomGameOptions.KnightCount, (UsableFunc)Usable);
    }

    public void Knight()
    {
        var cooldown = Interact(Player, KnightButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, KnightButton.TargetPlayer.PlayerId);
            ToBeKnighted.Add(KnightButton.TargetPlayer.PlayerId);
        }

        KnightButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => ToBeKnighted.Contains(player.PlayerId) || player.IsKnighted();

    public override void ReadRPC(MessageReader reader) => ToBeKnighted.Add(reader.ReadByte());

    public bool Usable() => !RoundOne;

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        var remove = new List<byte>();

        foreach (var id in Knighted)
        {
            var knight = PlayerById(id);

            if (knight.HasDied())
            {
                remove.Add(id);
                Run("<color=#FF004EFF>〖 Alert 〗</color>", "A Knight as died!");
            }
        }

        remove.ForEach(x => Knighted.Remove(x));
    }
}