namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Trapper : Crew
{
    [NumberOption(MultiMenu.LayerSubOptions, 0, 15, 1, ZeroIsInfinity = true)]
    public static Number MaxTraps { get; set; } = new(5);

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number BuildCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static Number BuildDur { get; set; } = new(10);

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number TrapCd { get; set; } = new(25);

    private CustomButton BuildButton { get; set; }
    private CustomButton TrapButton { get; set; }
    public bool Building { get ; set; }
    public List<byte> Trapped { get; set; }
    private List<Role> TriggeredRoles { get; set; }
    private int TrapsMade { get; set; }
    private bool AttackedSomeone { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Trapper : CustomColorManager.Crew;
    public override string Name => "Trapper";
    public override LayerEnum Type => LayerEnum.Trapper;
    public override Func<string> StartText => () => "<size=90%>Use Your Tinkering Skills To Obstruct The <color=#FF0000FF>Evildoers</color></size>";
    public override Func<string> Description => () => "- You can build a trap, adding it to your armory\n- You can place these traps on players and either log the roles of interactors on " +
        "them\nor protect from an attack once and attack the attacker in return";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.IntruderSupport;
        Trapped = [];
        TriggeredRoles = [];
        BuildButton = CreateButton(this, "BUILD TRAP", new SpriteName("Build"), AbilityType.Targetless, KeybindType.Secondary, (OnClick)StartBuildling, new Cooldown(BuildCd),
            new Duration(BuildDur), (EffectEndVoid)EndBuildling, new CanClickAgain(false), (UsableFunc)Usable);
        TrapButton = CreateButton(this, "PLACE TRAP", new SpriteName("Trap"), AbilityType.Alive, KeybindType.ActionSecondary, (OnClick)SetTrap, new Cooldown(TrapCd), MaxTraps,
            (PlayerBodyExclusion)Exception);
        TrapsMade = 0;
        TrapButton.Uses = 0;
    }

    private void StartBuildling()
    {
        BuildButton.Begin();
        Building = true;
    }

    private void EndBuildling()
    {
        TrapButton.Uses++;
        TrapsMade++;
        Building = false;
    }

    private void SetTrap()
    {
        var cooldown = Interact(Player, TrapButton.GetTarget<PlayerControl>());

        if (cooldown != CooldownType.Fail)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, TrapperActionsRPC.Place, TrapButton.GetTarget<PlayerControl>().PlayerId);
            Trapped.Add(TrapButton.GetTarget<PlayerControl>().PlayerId);
        }

        TrapButton.StartCooldown(cooldown);
    }

    private bool Exception(PlayerControl player) => Trapped.Contains(player.PlayerId);

    public bool Usable() => TrapsMade < MaxTraps;

    public void TriggerTrap(PlayerControl trapped, PlayerControl trigger, bool isAttack)
    {
        if (!Trapped.Contains(trapped.PlayerId))
            return;

        if (!isAttack)
        {
            TriggeredRoles.Add(trigger.GetRole());
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, TrapperActionsRPC.Trigger, trapped, trigger, isAttack);
        }
        else
            AttackedSomeone = true;

        Trapped.Remove(trapped.PlayerId);
    }

    public override void ReadRPC(MessageReader reader)
    {
        var trapAction = reader.ReadEnum<TrapperActionsRPC>();

        switch (trapAction)
        {
            case TrapperActionsRPC.Place:
                Trapped.Add(reader.ReadByte());
                break;

            case TrapperActionsRPC.Trigger:
                TriggerTrap(reader.ReadPlayer(), reader.ReadPlayer(), reader.ReadBoolean());
                break;

            default:
                Error($"Received unknown RPC - {(int)trapAction}");
                break;
        }
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);

        if (!AttackedSomeone && TriggeredRoles.Any())
        {
            var message = "Your trap detected the following roles: ";
            TriggeredRoles.Shuffle();
            TriggeredRoles.ForEach(x => message += $"{x}, ");
            message = message.Remove(message.Length - 2);

            if (IsNullEmptyOrWhiteSpace(message))
                return;

            // Only Trapper can see this
            if (HUD())
                Run("<color=#BE1C8CFF>〖 Trap Triggers 〗</color>", message);
        }
        else if (AttackedSomeone && HUD())
            Run("<color=#BE1C8CFF>〖 Trap Triggers 〗</color>", "Your trap attacked someone!");

        TriggeredRoles.Clear();
    }
}