namespace TownOfUsReworked.PlayerLayers.Roles;

public class Shifter : Crew
{
    public CustomButton ShiftButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomCrewColors ? CustomColorManager.Shifter : CustomColorManager.Crew;
    public override string Name => "Shifter";
    public override LayerEnum Type => LayerEnum.Shifter;
    public override Func<string> StartText => () => "Shift Around Roles";
    public override Func<string> Description => () => "- You can steal another player's role\n- Shifting withn on-<color=#8CFFFFFF>Crew</color> will cause you to kill yourself";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.CrewSupport;
        ShiftButton = CreateButton(this, "SHIFT", new SpriteName("Shift"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Shift, new Cooldown(CustomGameOptions.ShiftCd));
    }

    public void Shift()
    {
        var cooldown = Interact(Player, ShiftButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, ShiftButton.TargetPlayer);
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
            LayerEnum.Altruist => new Altruist(),
            LayerEnum.Bastion => new Bastion() { BombedIDs = ((Bastion)role).BombedIDs },
            LayerEnum.Chameleon => new Chameleon(),
            LayerEnum.Coroner => new Coroner(),
            LayerEnum.Crewmate => new Crewmate(),
            LayerEnum.Detective => new Detective(),
            LayerEnum.Dictator => new Dictator(),
            LayerEnum.Engineer => new Engineer(),
            LayerEnum.Escort => new Escort(),
            LayerEnum.Mayor => new Mayor(),
            LayerEnum.Medic => new Medic() { ShieldedPlayer = ((Medic)role).ShieldedPlayer },
            LayerEnum.Medium => new Medium(),
            LayerEnum.Monarch => new Monarch()
            {
                ToBeKnighted = ((Monarch)role).ToBeKnighted,
                Knighted = ((Monarch)role).Knighted
            },
            LayerEnum.Mystic => new Mystic(),
            LayerEnum.Operative => new Operative(),
            LayerEnum.Retributionist => new Retributionist()
            {
                Selected = ((Retributionist)role).Selected,
                ShieldedPlayer = ((Retributionist)role).ShieldedPlayer,
                BombedIDs = ((Retributionist)role).BombedIDs,
                Trapped = ((Retributionist)role).Trapped
            },
            LayerEnum.Seer => new Seer(),
            LayerEnum.Sheriff => new Sheriff(),
            LayerEnum.Tracker => new Tracker(),
            LayerEnum.Transporter => new Transporter(),
            LayerEnum.Trapper => new Trapper() { Trapped = ((Trapper)role).Trapped },
            LayerEnum.VampireHunter => new VampireHunter(),
            LayerEnum.Veteran => new Veteran(),
            LayerEnum.Vigilante => new Vigilante(),
            LayerEnum.Shifter or _ => new Shifter(),
        };

        newRole.Start<Role>(player).RoleUpdate(this);
        Role newRole2 = CustomGameOptions.ShiftedBecomes == BecomeEnum.Shifter ? new Shifter() : new Crewmate();
        newRole2.Start<Role>(other).RoleUpdate(role);
    }

    public bool Exception(PlayerControl player) => (Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) || (SubFaction != SubFaction.None && player.Is(SubFaction));

    public override void ReadRPC(MessageReader reader) => Shift(reader.ReadPlayer());
}