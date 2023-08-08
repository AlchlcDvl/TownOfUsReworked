namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public class Defector : Objectifier
{
    public bool Turned { get; set; }
    public Faction Side { get; set; }
    public bool Defect => ((Side == Faction.Intruder && LastImp) || (Side == Faction.Syndicate && LastSyn)) && !IsDead && !Turned;

    public override Color32 Color
    {
        get
        {
            if (Turned)
            {
                if (Side == Faction.Crew)
                    return Colors.Crew;
                else if (Side == Faction.Syndicate)
                    return Colors.Syndicate;
                else if (Side == Faction.Intruder)
                    return Colors.Intruder;
                else if (Side == Faction.Neutral)
                    return Colors.Neutral;
                else
                    return ClientGameOptions.CustomObjColors ? Colors.Defector : Colors.Objectifier;
            }
            else
                return ClientGameOptions.CustomObjColors ? Colors.Defector : Colors.Objectifier;
        }
    }
    public override string Name => "Defector";
    public override string Symbol => "ε";
    public override LayerEnum Type => LayerEnum.Defector;
    public override Func<string> Description => () => "- Be the last one of your faction to switch sides";
    public override bool Hidden => !CustomGameOptions.DefectorKnows && !Turned;

    public Defector(PlayerControl player) : base(player) => Side = Player.GetFaction();

    public static void GetFactionChoice(out bool crew, out bool evil)
    {
        crew = CustomGameOptions.DefectorFaction == DefectorFaction.Crew;
        evil = CustomGameOptions.DefectorFaction == DefectorFaction.OpposingEvil;

        if (CustomGameOptions.DefectorFaction == DefectorFaction.Random)
        {
            var random = URandom.RandomRangeInt(0, 2);
            evil = random == 0;
            crew = random == 1;
        }
    }

    public void TurnSides(bool crew, bool evil)
    {
        Turned = true;

        var role = Role.GetRole(Player);

        if (crew)
        {
            role.Faction = Faction.Crew;
            role.FactionColor = Colors.Crew;
            role.IsCrewDefect = true;
        }
        else if (evil)
        {
            if (Side == Faction.Intruder)
            {
                role.Faction = Faction.Syndicate;
                role.FactionColor = Colors.Syndicate;
                role.IsSynDefect = true;
            }
            else if (Side == Faction.Intruder)
            {
                role.Faction = Faction.Intruder;
                role.FactionColor = Colors.Intruder;
                role.IsIntDefect = true;
            }
        }

        Side = role.Faction;
        role.RoleAlignment = role.RoleAlignment.GetNewAlignment(role.Faction);

        if (Local)
            Flash(Color);

        if (CustomPlayer.Local.Is(LayerEnum.Mystic))
            Flash(Colors.Mystic);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Defect && !Turned)
        {
            GetFactionChoice(out var crew, out var evil);
            CallRpc(CustomRPC.Change, TurnRPC.TurnSides, this, crew, evil);
            TurnSides(crew, evil);
        }
    }
}