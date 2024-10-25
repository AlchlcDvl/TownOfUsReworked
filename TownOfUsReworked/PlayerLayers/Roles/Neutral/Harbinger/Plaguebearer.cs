namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Plaguebearer : Neutral
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number InfectCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool PBVent { get; set; } = false;

    public List<byte> Infected { get; set; }
    public bool CanTransform => AllPlayers().Count(x => !x.HasDied()) <= Infected.Count || NeutralApocalypseSettings.DirectSpawn;
    public CustomButton InfectButton { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Plaguebearer : CustomColorManager.Neutral;
    public override string Name => "Plaguebearer";
    public override LayerEnum Type => LayerEnum.Plaguebearer;
    public override Func<string> StartText => () => "Spread Disease To Summon <color=#424242FF>Pestilence</color>";
    public override Func<string> Description => () => "- You can infect players\n- When all players are infected, you will turn into <color=#424242FF>Pestilence</color>\n- Infections spread"
        + " via interaction between players";
    public override DefenseEnum DefenseVal => Infected.Count < AllPlayers().Count / 2 ? DefenseEnum.Basic : DefenseEnum.None;

    public override void Init()
    {
        BaseStart();
        Objectives = () => "- Infect everyone to become <color=#424242FF>Pestilence</color>\n- Kill off anyone who can oppose you";
        Alignment = Alignment.NeutralHarb;
        Infected = [Player.PlayerId];
        InfectButton ??= CreateButton(this, new SpriteName("Infect"), AbilityType.Alive, KeybindType.ActionSecondary, (OnClick)Infect, new Cooldown(InfectCd), "INFECT",
            (PlayerBodyExclusion)Exception);
    }

    public void RpcSpreadInfection(PlayerControl source, PlayerControl target)
    {
        if (Infected.Contains(source.PlayerId) == Infected.Contains(target.PlayerId))
            return;

        byte id = 0;
        var changed = false;

        if (Infected.Contains(source.PlayerId) || source.Is(LayerEnum.Plaguebearer))
        {
            id = target.PlayerId;
            changed = true;
        }
        else if (Infected.Contains(target.PlayerId) || target.Is(LayerEnum.Plaguebearer))
        {
            id = source.PlayerId;
            changed = true;
        }

        if (changed)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, id);
            Infected.Add(id);
        }
    }

    public void Infect() => InfectButton.StartCooldown(Interact(Player, InfectButton.GetTarget<PlayerControl>()));

    public void TurnPestilence()
    {
        new Pestilence().RoleUpdate(this, Player);

        if (NeutralApocalypseSettings.PlayersAlerted)
            Flash(Color);

        foreach (var player in AllPlayers())
        {
            if (!player.Is(Alignment.NeutralApoc) || !player.Is(Alignment.NeutralHarb))
                Pestilence.Infected[player.PlayerId] = 1;
        }
    }

    public bool Exception(PlayerControl player) => Infected.Contains(player.PlayerId);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (CanTransform && !Dead)
        {
            CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, this);
            TurnPestilence();
        }
    }

    public override void ReadRPC(MessageReader reader) => Infected.Add(reader.ReadByte());
}