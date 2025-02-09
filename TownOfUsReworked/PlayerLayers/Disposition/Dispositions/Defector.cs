namespace TownOfUsReworked.PlayerLayers.Dispositions;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Defector : Disposition
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool DefectorKnows { get; set; } = true;

    [StringOption(MultiMenu.LayerSubOptions)]
    public static DefectorFaction DefectorFaction { get; set; } = DefectorFaction.Random;

    private bool Turned { get; set; }
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
    public override string Name => "Defector";
    public override string Symbol => "ε";
    public override LayerEnum Type => LayerEnum.Defector;
    public override Func<string> Description => () => "- Be the last one of your faction to switch sides";
    public override bool Hidden => !DefectorKnows && !Turned;

    public override void Init()
    {
        base.Init();
        Side = Player.GetFaction();
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
            role.FactionColor = CustomColorManager.Crew;
            role.Objectives = () => Role.CrewWinCon;
        }
        else if (evil)
        {
            if (Side == Faction.Intruder)
            {
                role.Faction = Faction.Syndicate;
                role.FactionColor = CustomColorManager.Syndicate;
                role.Objectives = () => Role.SyndicateWinCon;
            }
            else if (Side == Faction.Syndicate)
            {
                role.Faction = Faction.Intruder;
                role.FactionColor = CustomColorManager.Intruder;
                role.Objectives = () => Role.IntrudersWinCon;
            }
        }
        else if (neutral)
        {
            role.Faction = Faction.Neutral;
            role.FactionColor = CustomColorManager.Neutral;
            role.Objectives = () => "- Be the last one standing";
        }

        Side = role.Faction;
        role.Alignment = role.Alignment.GetNewAlignment(role.Faction);

        if (Local)
            Flash(Color);

        if (CustomPlayer.Local.Is(LayerEnum.Mystic))
            Flash(CustomColorManager.Mystic);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Defect && !Turned)
        {
            GetFactionChoice(out var crew, out var evil, out var neut);
            CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, this, crew, evil, neut);
            TurnSides(crew, evil, neut);
        }
    }
}