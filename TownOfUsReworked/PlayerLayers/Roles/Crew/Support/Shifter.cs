namespace TownOfUsReworked.PlayerLayers.Roles;

public class Shifter : Crew
{
    public CustomButton ShiftButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomCrewColors ? CustomColorManager.Shifter : CustomColorManager.Crew;
    public override string Name => "Shifter";
    public override LayerEnum Type => LayerEnum.Shifter;
    public override Func<string> StartText => () => "Shift Around Roles";
    public override Func<string> Description => () => "- You can steal another player's role\n- Shifting withn on-<color=#8CFFFFFF>Crew</color> will cause you to kill yourself";

    public Shifter() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Alignment = Alignment.CrewSupport;
        ShiftButton = new(this, "Shift", AbilityTypes.Alive, "ActionSecondary", Shift, CustomGameOptions.ShiftCd);
        return this;
    }

    public void Shift()
    {
        var cooldown = Interact(Player, ShiftButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, ShiftButton.TargetPlayer);
            Shift(ShiftButton.TargetPlayer);
        }
        else
            ShiftButton.StartCooldown(cooldown);
    }

    public void Shift(PlayerControl other)
    {
        var role = other.GetRole();

        if (!other.Is(Faction.Crew) || other.IsFramed())
        {
            if (AmongUsClient.Instance.AmHost)
                RpcMurderPlayer(Player);

            return;
        }

        var player = Player;

        if (CustomPlayer.Local == other || CustomPlayer.Local == player)
        {
            Flash(CustomColorManager.Shifter);
            role.OnLobby();
            OnLobby();
            ButtonUtils.Reset();
        }

        Role newRole = role.Type switch
        {
            LayerEnum.Crewmate => new Crewmate(),
            LayerEnum.Detective => new Detective(),
            LayerEnum.Escort => new Escort(),
            LayerEnum.Sheriff => new Sheriff(),
            LayerEnum.Medium => new Medium(),
            LayerEnum.VampireHunter => new VampireHunter(),
            LayerEnum.Mystic => new Mystic(),
            LayerEnum.Seer => new Seer(),
            LayerEnum.Altruist => new Altruist(),
            LayerEnum.Engineer => new Engineer(),
            LayerEnum.Transporter => new Transporter(),
            LayerEnum.Mayor => new Mayor(),
            LayerEnum.Operative => new Operative(),
            LayerEnum.Veteran => new Veteran(),
            LayerEnum.Vigilante => new Vigilante(),
            LayerEnum.Chameleon => new Chameleon(),
            LayerEnum.Dictator => new Dictator(),
            LayerEnum.Tracker => new Tracker(),
            LayerEnum.Coroner => new Coroner(),
            LayerEnum.Bastion => new Bastion() { BombedIDs = ((Bastion)role).BombedIDs },
            LayerEnum.Medic => new Medic() { ShieldedPlayer = ((Medic)role).ShieldedPlayer },
            LayerEnum.Monarch => new Monarch()
            {
                ToBeKnighted = ((Monarch)role).ToBeKnighted,
                Knighted = ((Monarch)role).Knighted
            },
            LayerEnum.Retributionist => new Retributionist()
            {
                Selected = ((Retributionist)role).Selected,
                ShieldedPlayer = ((Retributionist)role).ShieldedPlayer,
                BombedIDs = ((Retributionist)role).BombedIDs
            },
            LayerEnum.Shifter or _ => new Shifter(),
        };

        newRole.Start<Role>(player).RoleUpdate(this);
        Role newRole2 = CustomGameOptions.ShiftedBecomes == BecomeEnum.Shifter ? new Shifter() : new Crewmate();
        newRole2.Start<Role>(other).RoleUpdate(role);
    }

    public bool Exception(PlayerControl player) => (Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) || (SubFaction != SubFaction.None && player.Is(SubFaction));

    public override void ReadRPC(MessageReader reader) => Shift(reader.ReadPlayer());

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        ShiftButton.Update2("SHIFT");
    }
}