namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Amnesiac)]
public sealed class Amnesiac : Benign
{
    [ToggleOption]
    private static bool RememberArrows = false;

    [NumberOption(0f, 15f, 1f, Format.Time)]
    private static Number RememberArrowDelay = 5;

    [ToggleOption]
    private static bool AmneVent = false;

    [ToggleOption]
    private static bool AmneSwitchVent = false;

    [ToggleOption]
    private static bool AmneToThief = true;

    private readonly Dictionary<byte, PositionalArrow> BodyArrows = [];
    private CustomButton RememberButton;

    protected override UColor MainColor => CustomColorManager.Amnesiac;
    public override Layer Type => Layer.Amnesiac;
    public override string StartText => "You Forgor <i>:skull:</i>";
    public override string Description => "- You can copy over a player's role should you find their body" + (RememberArrows ? ("\n- When someone dies, you get an arrow pointing"
        + " to their body") : "") + "\n- If there are less than 4 players alive, you will become a <#80FF00FF>Thief</color>";
    public override bool CanVent => base.CanVent && AmneVent;
    public override bool CanSwitchVents => AmneSwitchVent;

    public override void Init()
    {
        Objectives = () => "- Find a dead body, remember their role and then fulfill the win condition for that role";
        BodyArrows.Clear();
        RememberButton ??= new(this, new SpriteName("Remember"), AbilityTypes.Body, KeybindType.ActionSecondary, (OnClickBody)Remember, "REMEMBER");
    }

    private void DestroyArrow(byte targetPlayerId)
    {
        if (BodyArrows.Remove(targetPlayerId, out var arrow))
            arrow.Destroy();
    }

    protected override void ClearArrows()
    {
        BodyArrows.Values.DestroyAll();
        BodyArrows.Clear();
    }

    private void TurnThief() => new Thief().RoleUpdate(this);

    private void Remember(DeadBody target)
    {
        var player = PlayerByBody(target);
        Spread(Player, player);
        PerformRpcAction(player);
        Remember(player);
    }

    public override void ReadRPC(RpcReader reader) => Remember(reader.ReadPlayer());

    private void Remember(PlayerControl other)
    {
        var role = other.GetRole();
        var player = Player;

        Role newRole = role switch
        {
            // Crew roles
            Altruist => new Altruist(),
            Bastion => new Bastion(),
            Chameleon => new Chameleon(),
            Coroner => new Coroner(),
            Crewmate => new Crewmate(),
            Detective => new Detective(),
            Dictator => new Dictator(),
            Engineer => new Engineer(),
            Escort => new Escort(),
            Mayor or Democrat => new Democrat(),
            Medic => new Medic(),
            Medium => new Medium(),
            Monarch => new Monarch(),
            Mystic => new Mystic(),
            Operative => new Operative(),
            Retributionist => new Retributionist(),
            Sheriff => new Sheriff(),
            Seer => new Seer(),
            Shifter => new Shifter(),
            Tracker => new Tracker(),
            Transporter => new Transporter(),
            Trapper => new Trapper(),
            Veteran => new Veteran(),
            Vigilante => new Vigilante(),

            // Outcast roles
            Actor => new Actor(),
            Arsonist => new Arsonist(),
            Betrayer => new Betrayer(),
            BountyHunter bh => new BountyHunter() { TargetPlayer = bh.TargetPlayer },
            Cannibal => new Cannibal(),
            Cryomaniac => new Cryomaniac(),
            Dracula => new Dracula(),
            Executioner exe => new Executioner() { TargetPlayer = exe.TargetPlayer },
            Glitch => new Glitch(),
            GuardianAngel ga => new GuardianAngel() { TargetPlayer = ga.TargetPlayer },
            Guesser guesser => new Guesser() { TargetPlayer = guesser.TargetPlayer },
            Jackal => new Jackal(),
            Jester => new Jester(),
            Juggernaut => new Juggernaut(),
            Murderer => new Murderer(),
            Necromancer => new Necromancer(),
            SerialKiller => new SerialKiller(),
            Survivor => new Survivor(),
            Thief => new Thief(),
            Troll => new Troll(),
            Werewolf => new Werewolf(),
            Whisperer => new Whisperer(),

            // Intruder roles
            Ambusher => new Ambusher(),
            Blackmailer => new Blackmailer(),
            Camouflager => new Camouflager(),
            Consigliere => new Consigliere(),
            Consort => new Consort(),
            Disguiser => new Disguiser(),
            Enforcer => new Enforcer(),
            Godfather => new Godfather(),
            Grenadier => new Grenadier(),
            Impostor => new Impostor(),
            Janitor => new Janitor(),
            Miner => new Miner(),
            Morphling => new Morphling(),
            Teleporter => new Teleporter(),
            Wraith => new Wraith(),

            // Syndicate roles
            Anarchist => new Anarchist(),
            Bomber => new Bomber(),
            Collider => new Collider(),
            Concealer => new Concealer(),
            Crusader => new Crusader(),
            Drunkard => new Drunkard(),
            Framer => new Framer(),
            Poisoner => new Poisoner(),
            Rebel => new Rebel(),
            Shapeshifter => new Shapeshifter(),
            Silencer => new Silencer(),
            Spellslinger => new Spellslinger(),
            Stalker => new Stalker(),
            Timekeeper => new Timekeeper(),
            Warper => new Warper(),

            // Apocalypse roles
            Plaguebearer or Pestilence => new Plaguebearer(),
            Cultist or Void => new Cultist(),

            // Whatever else
            Amnesiac or _ => new Amnesiac(),
        };

        newRole.RoleUpdate(this, player, Handler.CurrentFaction == Faction.Outcast);

        switch (role)
        {
            case Neophyte neophyte when newRole is Neophyte neophyte1:
            {
                neophyte1.Members.AddRange(neophyte.Members);

                switch (role)
                {
                    case Jackal jackal1 when newRole is Jackal jackal2:
                    {
                        jackal2.Recruit1 = jackal1.Recruit1;
                        jackal2.Recruit2 = jackal1.Recruit2;
                        jackal2.Recruit3 = jackal1.Recruit3;
                        break;
                    }
                    case Whisperer whisperer1 when newRole is Whisperer whisperer2:
                    {
                        whisperer2.PlayerConversion.AddRange(whisperer1.PlayerConversion);
                        break;
                    }
                }

                break;
            }
            case Actor act1 when newRole is Actor act2:
            {
                act2.PretendRoles.AddRange(act1.PretendRoles);
                break;
            }
        }

        var local = LayerHandler.Handlers[LocalPlayer.PlayerId];
        var faction = local.CurrentFaction;

        if (faction != Faction.Crew || (faction == Faction.Outcast && (Snitch.SnitchSeesOutcasts || Revealer.RevealerRevealsOutcasts)))
        {
            var neut = faction == Faction.Outcast;

            if (!neut || Snitch.SnitchSeesOutcasts)
            {
                foreach (var snitch in GetLayers<Snitch>())
                {
                    if (snitch.TasksLeft <= Snitch.SnitchTasksRemaining && player.AmOwner)
                        local.AllArrows.Add(snitch.PlayerId, new(player, snitch.Player, snitch.Color));
                    else if (snitch.TasksDone && snitch.Local)
                        LayerHandler.Handlers[snitch.PlayerId].AllArrows.Add(player.PlayerId, new(snitch.Player, player, snitch.Color));
                }
            }

            if (!neut || Revealer.RevealerRevealsOutcasts)
            {
                foreach (var revealer in GetLayers<Revealer>())
                {
                    if (revealer.Revealed && player.AmOwner)
                        local.AllArrows.Add(revealer.PlayerId, new(player, revealer.Player, revealer.Color));
                }
            }
        }

        if (!other.AmOwner)
            return;

        Flash(Color);
        ButtonUtils.Reset(player: other);
    }

    public override void UpdateHud(HudManager __instance)
    {
        if (!RememberArrows || Dead)
            return;

        var validBodies = AllBodies().Where(x => KilledPlayers.Any(y => y.PlayerId == x.ParentId && y.KillAge <= RememberArrowDelay));
        BodyArrows.Keys.Where(bodyArrow => validBodies.All(x => x.ParentId != bodyArrow)).Do(DestroyArrow);

        foreach (var body in validBodies)
        {
            if (!BodyArrows.ContainsKey(body.ParentId))
                BodyArrows[body.ParentId] = new(Player, body.TruePosition, Color);
        }
    }

    public override void UpdatePlayer()
    {
        base.UpdatePlayer();

        if (AmneToThief && AllPlayers().Count(x => !x.HasDied()) <= 4 && !Dead)
            TurnThief();
    }
}