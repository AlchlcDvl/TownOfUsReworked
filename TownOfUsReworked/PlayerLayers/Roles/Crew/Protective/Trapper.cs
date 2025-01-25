namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Trapper : Crew, ITrapper
{
    [NumberOption(0, 15, 1, zeroIsInf: true)]
    public static Number MaxTraps = 5;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number BuildCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number BuildDur = 10;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number TrapCd = 25;

    private CustomButton BuildButton { get; set; }
    private CustomButton TrapButton { get; set; }
    public bool Building { get ; set; }
    public List<byte> Trapped { get; } = [];
    private List<Role> TriggeredRoles { get; } = [];
    private int TrapsMade { get; set; }
    private bool AttackedSomeone { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Trapper : FactionColor;
    public override LayerEnum Type => LayerEnum.Trapper;
    public override Func<string> StartText => () => "<size=90%>Use Your Tinkering Skills To Obstruct The <#FF0000FF>Evildoers</color></size>";
    public override Func<string> Description => () => "- You can build a trap, adding it to your armory\n- You can place these traps on players and either log the roles of interactors on " +
        "them\nor protect from an attack once and attack the attacker in return";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Support;
        Trapped.Clear();
        TriggeredRoles.Clear();
        BuildButton ??= new(this, "BUILD TRAP", new SpriteName("Build"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)StartBuildling, new Cooldown(BuildCd), MaxTraps,
            (UsableFunc)Usable, new Duration(BuildDur), (EffectEndVoid)EndBuildling, new CanClickAgain(false));
        TrapButton ??= new(this, "PLACE TRAP", new SpriteName("Trap"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)SetTrap, new Cooldown(TrapCd), MaxTraps,
            (PlayerBodyExclusion)Exception);
        TrapsMade = TrapButton.uses = 0;
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

    private void SetTrap(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, TrapperActionsRPC.Place, target.PlayerId);
            Trapped.Add(target.PlayerId);
        }

        TrapButton.StartCooldown(cooldown);
    }

    private bool Exception(PlayerControl player) => Trapped.Contains(player.PlayerId);

    public bool Usable() => TrapsMade <= MaxTraps;

    public void TriggerTrap(PlayerControl trapped, PlayerControl trigger, bool isAttack)
    {
        if (!Trapped.Contains(trapped.PlayerId))
            return;

        if (trigger.AmOwner)
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, TrapperActionsRPC.Trigger, trapped, trigger, isAttack);

        TriggeredRoles.Add(trigger.GetRole());
        AttackedSomeone = isAttack;
        Trapped.Remove(trapped.PlayerId);
    }

    public override void ReadRPC(MessageReader reader)
    {
        var trapAction = reader.ReadEnum<TrapperActionsRPC>();

        switch (trapAction)
        {
            case TrapperActionsRPC.Place:
            {
                Trapped.Add(reader.ReadByte());
                break;
            }
            case TrapperActionsRPC.Trigger:
            {
                TriggerTrap(reader.ReadPlayer(), reader.ReadPlayer(), reader.ReadBoolean());
                break;
            }
            default:
            {
                Failure($"Received unknown RPC - {trapAction}");
                break;
            }
        }
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        var message = "";

        if (AttackedSomeone)
            message = "Your trap attacked someone!";
        else if (TriggeredRoles.Any())
        {
            message = "Your trap detected the following roles: ";
            TriggeredRoles.Shuffle();
            TriggeredRoles.ForEach(x => message += $"{x}, ");
            message = message[..^2];
        }

        if (!IsNullEmptyOrWhiteSpace(message))
            Run("<#8D0F8CFF>〖 Trap Triggers 〗</color>", message);

        TriggeredRoles.Clear();
    }
}