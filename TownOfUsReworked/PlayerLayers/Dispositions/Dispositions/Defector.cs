namespace TownOfUsReworked.PlayerLayers.Dispositions;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Defector : Disposition
{
    [ToggleOption]
    private static bool DefectorKnows = true;

    [StringOption<DefectorFaction>]
    private static DefectorFaction DefectorFaction = DefectorFaction.Random;

    public bool Turned { get; private set; }
    public Faction Side { get; private set; }

    public override UColor Color
    {
        get
        {
            if (Turned)
            {
                return Side switch
                {
                    Faction.Crew => CustomColorManager.Crew,
                    Faction.Intruder => CustomColorManager.Intruder,
                    Faction.Neutral => CustomColorManager.Neutral,
                    Faction.Syndicate => CustomColorManager.Syndicate,
                    _ => ClientOptions.CustomDispColors ? CustomColorManager.Defector : CustomColorManager.Disposition
                };
            }

            return ClientOptions.CustomDispColors ? CustomColorManager.Defector : CustomColorManager.Disposition;
        }
    }
    public override string Symbol => "ε";
    public override LayerEnum Type => LayerEnum.Defector;
    public override Func<string> Description => () => "- Be the last one of your faction to switch sides";
    public override bool Hidden => !DefectorKnows && !Turned;

    protected override void Init()
    {
        base.Init();
        Side = Player.GetFaction();
    }

    public override void UpdateHud(HudManager __instance)
    {
        if (Dead || Turned || !Last(Side))
            return;

        GetFactionChoice(out var crew, out var evil, out var neut);
        CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, this, crew, evil, neut);
        TurnSides(crew, evil, neut);
    }

    protected override void CheckWin()
    {
        if (!DefectorWins())
            return;

        if (Side == Faction.Neutral)
        {
            WinState = NeutralSettings.NoSolo switch
            {
                NoSolo.AllNKs => WinLose.AllNKsWin,
                NoSolo.AllNeutrals => WinLose.AllNeutralsWin,
                _ => WinLose.None
            };
        }

        if (WinState == WinLose.None)
            WinState = WinLose.DefectorWins;

        CallRpc(CustomRPC.WinLose, WinState, this);
    }

    private static void GetFactionChoice(out bool crew, out bool evil, out bool neut)
    {
        crew = DefectorFaction == DefectorFaction.Crew;
        neut = DefectorFaction == DefectorFaction.Neutral;
        evil = DefectorFaction == DefectorFaction.OpposingEvil;

        switch (DefectorFaction)
        {
            case DefectorFaction.Random:
            {
                var random = URandom.RandomRangeInt(0, 3);
                evil = random == 0;
                crew = random == 1;
                neut = random == 2;
                break;
            }
            case DefectorFaction.NonCrew:
            {
                var random = URandom.RandomRangeInt(0, 2);
                evil = random == 0;
                neut = random == 1;
                break;
            }
            case DefectorFaction.NonNeutral:
            {
                var random = URandom.RandomRangeInt(0, 2);
                evil = random == 0;
                crew = random == 1;
                break;
            }
            case DefectorFaction.NonFaction:
            {
                var random = URandom.RandomRangeInt(0, 2);
                crew = random == 0;
                neut = random == 1;
                break;
            }
        }
    }

    public void TurnSides(bool crew, bool evil, bool neutral)
    {
        Turned = true;
        var role = Player.GetRole();

        if (crew)
        {
            role.Faction = Faction.Crew;
            role.Objectives = () => Role.CrewWinCon;
        }
        else if (evil)
        {
            switch (Side)
            {
                case Faction.Intruder:
                {
                    role.Faction = Faction.Syndicate;
                    role.Objectives = () => Role.SyndicateWinCon;
                    break;
                }
                case Faction.Syndicate:
                {
                    role.Faction = Faction.Intruder;
                    role.Objectives = () => Role.IntrudersWinCon;
                    break;
                }
            }
        }
        else if (neutral)
        {
            role.Faction = Faction.Neutral;
            role.Objectives = () => "- Be the last one standing";
        }

        Side = role.Faction;

        if (Local)
            Flash(Color);

        if (CustomPlayer.Local.Is<Mystic>())
            Flash(CustomColorManager.Mystic);
    }
}