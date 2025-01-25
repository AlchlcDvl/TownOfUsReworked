namespace TownOfUsReworked.PlayerLayers.Dispositions;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Defector : Disposition
{
    [ToggleOption]
    public static bool DefectorKnows = true;

    [StringOption<DefectorFaction>]
    public static DefectorFaction DefectorFaction = DefectorFaction.Random;

    public bool Turned { get; set; }
    public Faction Side { get; set; }
    private bool Defect => ((Side == Faction.Intruder && LastImp()) || (Side == Faction.Syndicate && LastSyn())) && !Dead && !Turned;

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
            else
                return ClientOptions.CustomDispColors ? CustomColorManager.Defector : CustomColorManager.Disposition;
        }
    }
    public override string Symbol => "ε";
    public override LayerEnum Type => LayerEnum.Defector;
    public override Func<string> Description => () => "- Be the last one of your faction to switch sides";
    public override bool Hidden => !DefectorKnows && !Turned;

    public override void Init()
    {
        base.Init();
        Side = Player.GetFaction();
    }

    public override void UpdateHud(HudManager __instance)
    {
        if (Defect && !Turned)
        {
            GetFactionChoice(out var crew, out var evil, out var neut);
            CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, this, crew, evil, neut);
            TurnSides(crew, evil, neut);
        }
    }

    public override void CheckWin()
    {
        if (DefectorWins())
        {
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
    }

    public static void GetFactionChoice(out bool crew, out bool evil, out bool neut)
    {
        crew = DefectorFaction == DefectorFaction.Crew;
        neut = DefectorFaction == DefectorFaction.Neutral;
        evil = DefectorFaction == DefectorFaction.OpposingEvil;

        if (DefectorFaction == DefectorFaction.Random)
        {
            var random = URandom.RandomRangeInt(0, 3);
            evil = random == 0;
            crew = random == 1;
            neut = random == 2;
        }
        else if (DefectorFaction == DefectorFaction.NonCrew)
        {
            var random = URandom.RandomRangeInt(0, 2);
            evil = random == 0;
            neut = random == 1;
        }
        else if (DefectorFaction == DefectorFaction.NonNeutral)
        {
            var random = URandom.RandomRangeInt(0, 2);
            evil = random == 0;
            crew = random == 1;
        }
        else if (DefectorFaction == DefectorFaction.NonFaction)
        {
            var random = URandom.RandomRangeInt(0, 2);
            crew = random == 0;
            neut = random == 1;
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
            if (Side == Faction.Intruder)
            {
                role.Faction = Faction.Syndicate;
                role.Objectives = () => Role.SyndicateWinCon;
            }
            else if (Side == Faction.Syndicate)
            {
                role.Faction = Faction.Intruder;
                role.Objectives = () => Role.IntrudersWinCon;
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