namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public class Defector : Objectifier
{
    private bool Turned { get; set; }
    public Faction Side { get; set; }
    private bool Defect => ((Side == Faction.Intruder && LastImp) || (Side == Faction.Syndicate && LastSyn)) && !IsDead && !Turned;

    public override UColor Color
    {
        get
        {
            if (Turned)
            {
                if (Side == Faction.Crew)
                    return CustomColorManager.Crew;
                else if (Side == Faction.Syndicate)
                    return CustomColorManager.Syndicate;
                else if (Side == Faction.Intruder)
                    return CustomColorManager.Intruder;
                else if (Side == Faction.Neutral)
                    return CustomColorManager.Neutral;
                else
                    return ClientGameOptions.CustomObjColors ? CustomColorManager.Defector : CustomColorManager.Objectifier;
            }
            else
                return ClientGameOptions.CustomObjColors ? CustomColorManager.Defector : CustomColorManager.Objectifier;
        }
    }
    public override string Name => "Defector";
    public override string Symbol => "ε";
    public override LayerEnum Type => LayerEnum.Defector;
    public override Func<string> Description => () => "- Be the last one of your faction to switch sides";
    public override bool Hidden => !CustomGameOptions.DefectorKnows && !Turned;

    public Defector() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        Side = Player.GetFaction();
        return this;
    }

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
        var role = Player.GetRole();

        if (crew)
        {
            role.Faction = Faction.Crew;
            role.FactionColor = CustomColorManager.Crew;
        }
        else if (evil)
        {
            if (Side == Faction.Intruder)
            {
                role.Faction = Faction.Syndicate;
                role.FactionColor = CustomColorManager.Syndicate;
            }
            else if (Side == Faction.Intruder)
            {
                role.Faction = Faction.Intruder;
                role.FactionColor = CustomColorManager.Intruder;
            }
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
            GetFactionChoice(out var crew, out var evil);
            CallRpc(CustomRPC.Change, TurnRPC.TurnSides, this, crew, evil);
            TurnSides(crew, evil);
        }
    }
}