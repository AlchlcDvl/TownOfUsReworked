namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Role : PlayerLayer
{
    protected override UColor MainColor => CustomColorManager.Role;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Role;
    public override Layer Type => Layer.NoneRole;
    protected override UColor LayerColor => FactionColor;
    protected override bool UseMainColor => false;

    public abstract Faction BaseFaction { get; }
    public abstract Alignment Alignment { get; }

    public virtual string StartText => "Woah The Game Started";
    public virtual bool RoleBlockImmune => false;
    public virtual bool AffectedByLights => true;
    public virtual bool CanSwitchVents => true;

    public UColor FactionColor { get; set; }
    public string FactionColorString => $"<#{FactionColor.ToHtmlStringRGBA()}>";
    public virtual string FactionName => $"{Handler.CurrentFaction}";

    public Func<string> Objectives { get; set; } = () => "- None";

    public virtual List<PlayerControl> Team()
    {
        var team = new List<PlayerControl>() { Player };

        switch (Handler.CurrentDisposition)
        {
            case Paired pair:
            {
                team.Add(pair.Other);
                break;
            }
            case Mafia:
            {
                team.AddRange(AllPlayers().Where(x => x != Player && x.Is<Mafia>()));
                break;
            }
        }

        if (Handler.CurrentFaction == Faction.Cabal && Alignment != Alignment.Neophyte)
        {
            var jackal = (Jackal)Player.GetNeophyte();
            team.Add(jackal.Player);
            team.AddRange(jackal.GetOtherRecruits(Player));
        }

        return team;
    }

    public virtual void Reset(bool meeting, bool start) {}

    public virtual void UponRoleChanged(Role former) {}

    public void RoleUpdate(Role former, PlayerControl player = null, bool inherit = false)
    {
        player ??= former.Player;
        CustomButton.AllButtons.Where(x => x.Owner == former || !x.Owner.Player).Do(x => x.Destroy());
        CustomArrow.AllArrows.Where(x => x.Owner == player).Do(x => x.Disable());
        former.End();
        Start(player);

        if (Local)
        {
            ButtonUtils.Reset();
            Player.RegenTask();
            Flash(Color);
        }

        if (LocalPlayer.Is<Seer>(out var seer))
            Flash(seer.Color);

        Handler.SetUpLayers(inherit, PlayerLayerEnum.Role);
        UponRoleChanged(former);
    }

    public override void UpdateMap(MapBehaviour __instance) => __instance.ColorControl.SetColor(Color);

    public static IEnumerable<Role> GetRoles(Faction faction) => GetLayers<Role>().Where(x => x.Handler.CurrentFaction == faction && !x.Deinitialised);

    public static IEnumerable<Role> GetBaseFactionRoles(Faction faction) => GetLayers<Role>().Where(x => x.BaseFaction == faction && !x.Deinitialised);
}