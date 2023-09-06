namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class UpdateNames
{
    public static readonly Dictionary<byte, string> PlayerNames = new();
    public static readonly Dictionary<byte, string> ColorNames = new();

    public static void Postfix()
    {
        if (!SoundEffects.ContainsKey("Kill") && CustomPlayer.Local)
            SoundEffects.Add("Kill", CustomPlayer.Local.KillSfx);

        if (NoPlayers || IsHnS || Meeting || IsLobby)
            return;

        CustomPlayer.AllPlayers.ForEach(SetName);
    }

    private static void SetName(PlayerControl player)
    {
        if (!PlayerNames.ContainsKey(player.PlayerId))
            PlayerNames.Add(player.PlayerId, player.Data.PlayerName);

        if (!ColorNames.ContainsKey(player.PlayerId))
            ColorNames.Add(player.PlayerId, TranslationController.Instance.GetString(Palette.ColorNames[player.CurrentOutfit.ColorId]));

        (player.NameText().text, player.NameText().color) = UpdateGameName(player);
        player.ColorBlindText().text = UpdateColorblind(player);
    }

    private static string UpdateColorblind(PlayerControl player)
    {
        if (player.GetCustomOutfitType() == CustomPlayerOutfitType.Invis && player != CustomPlayer.Local)
            return "";
        
        if (!DataManager.Settings.Accessibility.ColorBlindMode)
            return "";

        var distance = Vector2.Distance(CustomPlayer.Local.GetTruePosition(), player.GetTruePosition());
        var vector = player.GetTruePosition() - CustomPlayer.Local.GetTruePosition();

        if (PhysicsHelpers.AnyNonTriggersBetween(CustomPlayer.Local.GetTruePosition(), vector.normalized, distance, Constants.ShipAndObjectsMask) && CustomGameOptions.ObstructNames &&
            player != CustomPlayer.Local && !CustomPlayer.LocalCustom.IsDead)
        {
            return "";
        }

        var name = ColorNames.FirstOrDefault(x => x.Key == player.PlayerId).Value;
        var ld = ColorUtils.LightDarkColors[player.CurrentOutfit.ColorId] == "Lighter" ? "L" : "D";

        if (ClientGameOptions.LighterDarker)
            name += $" ({ld})";

        return name;
    }

    public static (string, Color) UpdateGameName(PlayerControl player)
    {
        if (player.GetCustomOutfitType() == CustomPlayerOutfitType.Invis && player != CustomPlayer.Local)
            return ("", UColor.clear);

        var distance = Vector2.Distance(CustomPlayer.Local.GetTruePosition(), player.GetTruePosition());
        var vector = player.GetTruePosition() - CustomPlayer.Local.GetTruePosition();

        if (PhysicsHelpers.AnyNonTriggersBetween(CustomPlayer.Local.GetTruePosition(), vector.normalized, distance, Constants.ShipAndObjectsMask) && CustomGameOptions.ObstructNames &&
            player != CustomPlayer.Local && !CustomPlayer.LocalCustom.IsDead)
        {
            return ("", UColor.clear);
        }

        var name = "";
        var color = UColor.white;
        var info = player.AllPlayerInfo();
        var localinfo = CustomPlayer.Local.AllPlayerInfo();
        var roleRevealed = false;

        if (DoUndo.IsCamoed && player != CustomPlayer.Local && !CustomPlayer.LocalCustom.IsDead && !IsLobby)
            name = GetRandomisedName();
        else if (CustomGameOptions.NoNames && !IsLobby)
            name = "";
        else
            name = PlayerNames.FirstOrDefault(x => x.Key == player.PlayerId).Value;

        if (info[0] == null || localinfo[0] == null)
            return (name, color);

        if (player.CanDoTasks() && DeadSeeEverything)
        {
            var role = info[0] as Role;
            name += $" ({role.TasksCompleted}/{role.TotalTasks})";
            roleRevealed = true;
        }

        if (player.IsKnighted())
            name += " <color=#FF004EFF>κ</color>";

        if (player.IsMarked())
            name += " <color=#F1C40FFF>χ</color>";

        if (player == CachedFirstDead && ((player == CustomPlayer.Local && (int)CustomGameOptions.WhoSeesFirstKillShield == 1) || CustomGameOptions.WhoSeesFirstKillShield == 0))
            name += " <color=#C2185BFF>Γ</color>";

        if (player.Is(LayerEnum.Mayor) && !DeadSeeEverything && CustomPlayer.Local != player)
        {
            var mayor = info[0] as Mayor;

            if (mayor.Revealed)
            {
                roleRevealed = true;
                name += $"\n{mayor.Name}";
                color = mayor.Color;

                if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                {
                    var consigliere = localinfo[0] as Consigliere;

                    if (consigliere.Investigated.Contains(player.PlayerId))
                        consigliere.Investigated.Remove(player.PlayerId);
                }
                else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                {
                    var godfather = localinfo[0] as PromotedGodfather;

                    if (godfather.Investigated.Contains(player.PlayerId))
                        godfather.Investigated.Remove(player.PlayerId);
                }
                else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
                {
                    var inspector = localinfo[0] as Inspector;

                    if (inspector.Inspected.Contains(player.PlayerId))
                        inspector.Inspected.Remove(player.PlayerId);
                }
                else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                {
                    var retributionist = localinfo[0] as Retributionist;

                    if (retributionist.Inspected.Contains(player.PlayerId))
                        retributionist.Inspected.Remove(player.PlayerId);
                }
            }
        }
        else if (player.Is(LayerEnum.Dictator) && !DeadSeeEverything && CustomPlayer.Local != player)
        {
            var dict = info[0] as Dictator;

            if (dict.Revealed)
            {
                roleRevealed = true;
                name += $"\n{dict.Name}";
                color = dict.Color;

                if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                {
                    var consigliere = localinfo[0] as Consigliere;

                    if (consigliere.Investigated.Contains(player.PlayerId))
                        consigliere.Investigated.Remove(player.PlayerId);
                }
                else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                {
                    var godfather = localinfo[0] as PromotedGodfather;

                    if (godfather.Investigated.Contains(player.PlayerId))
                        godfather.Investigated.Remove(player.PlayerId);
                }
                else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
                {
                    var inspector = localinfo[0] as Inspector;

                    if (inspector.Inspected.Contains(player.PlayerId))
                        inspector.Inspected.Remove(player.PlayerId);
                }
                else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                {
                    var retributionist = localinfo[0] as Retributionist;

                    if (retributionist.Inspected.Contains(player.PlayerId))
                        retributionist.Inspected.Remove(player.PlayerId);
                }
            }
        }

        if (CustomPlayer.Local.Is(LayerEnum.Consigliere) && !DeadSeeEverything)
        {
            var consigliere = localinfo[0] as Consigliere;

            if (consigliere.Investigated.Contains(player.PlayerId) && !roleRevealed)
            {
                var role = info[0] as Role;
                roleRevealed = true;

                if (CustomGameOptions.ConsigInfo == ConsigInfo.Role)
                {
                    color = role.Color;
                    name += $"\n{role}";

                    if (consigliere.Player.GetSubFaction() == player.GetSubFaction() && player.GetSubFaction() != SubFaction.None)
                        consigliere.Investigated.Remove(player.PlayerId);
                }
                else if (CustomGameOptions.ConsigInfo == ConsigInfo.Faction)
                {
                    color = role.FactionColor;
                    name += $"\n{role.FactionName}";
                }
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather) && !DeadSeeEverything)
        {
            var godfather = localinfo[0] as PromotedGodfather;

            if (godfather.IsConsig && godfather.Investigated.Contains(player.PlayerId) && !roleRevealed)
            {
                var role = info[0] as Role;
                roleRevealed = true;

                if (CustomGameOptions.ConsigInfo == ConsigInfo.Role)
                {
                    color = role.Color;
                    name += $"\n{role}";

                    if (godfather.Player.GetSubFaction() == player.GetSubFaction() && player.GetSubFaction() != SubFaction.None)
                        godfather.Investigated.Remove(player.PlayerId);
                }
                else if (CustomGameOptions.ConsigInfo == ConsigInfo.Faction)
                {
                    color = role.FactionColor;
                    name += $"\n{role.FactionName}";
                }
            }
            else if (godfather.IsBM && godfather.BlackmailedPlayer == player)
                name += " <color=#02A752FF>Φ</color>";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Medic))
        {
            var medic = localinfo[0] as Medic;

            if (medic.ShieldedPlayer != null && medic.ShieldedPlayer == player && (int)CustomGameOptions.ShowShielded is 1 or 2)
                name += " <color=#006600FF>✚</color>";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
        {
            var ret = localinfo[0] as Retributionist;

            if (ret.ShieldedPlayer != null && ret.ShieldedPlayer == player && (int)CustomGameOptions.ShowShielded is 1 or 2)
                name += " <color=#006600FF>✚</color>";
            if (ret.Inspected.Contains(player.PlayerId) && !roleRevealed)
            {
                name += $"\n{player.GetInspResults()}";
                color = ret.Color;
                roleRevealed = true;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Arsonist) && !DeadSeeEverything)
        {
            var arsonist = localinfo[0] as Arsonist;

            if (arsonist.Doused.Contains(player.PlayerId))
                name += " <color=#EE7600FF>Ξ</color>";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Plaguebearer) && !DeadSeeEverything)
        {
            var plaguebearer = localinfo[0] as Plaguebearer;

            if (plaguebearer.Infected.Contains(player.PlayerId) && CustomPlayer.Local != player)
                name += " <color=#CFFE61FF>ρ</color>";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Cryomaniac) && !DeadSeeEverything)
        {
            var cryomaniac = localinfo[0] as Cryomaniac;

            if (cryomaniac.Doused.Contains(player.PlayerId))
                name += " <color=#642DEAFF>λ</color>";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Framer) && !DeadSeeEverything)
        {
            var framer = localinfo[0] as Framer;

            if (framer.Framed.Contains(player.PlayerId))
                name += " <color=#00FFFFFF>ς</color>";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Spellslinger) && !DeadSeeEverything)
        {
            var spellslinger = localinfo[0] as Spellslinger;

            if (spellslinger.Spelled.Contains(player.PlayerId))
                name += " <color=#0028F5FF>ø</color>";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Executioner) && !DeadSeeEverything)
        {
            var executioner = localinfo[0] as Executioner;

            if (player == executioner.TargetPlayer)
            {
                name += " <color=#CCCCCCFF>§</color>";

                if (CustomGameOptions.ExeKnowsTargetRole && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    roleRevealed = true;
                }
                else
                    color = executioner.Color;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Guesser) && !DeadSeeEverything)
        {
            var guesser = localinfo[0] as Guesser;

            if (player == guesser.TargetPlayer)
            {
                color = guesser.Color;
                name += " <color=#EEE5BEFF>π</color>";
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.GuardianAngel) && !DeadSeeEverything)
        {
            var guardianAngel = localinfo[0] as GuardianAngel;

            if (player == guardianAngel.TargetPlayer)
            {
                name += " <color=#FFFFFFFF>★</color>";

                if (player.IsProtected() && (int)CustomGameOptions.ShowProtect is 1 or 2)
                    name += " <color=#FFFFFFFF>η</color>";

                if (CustomGameOptions.GAKnowsTargetRole && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    roleRevealed = true;
                }
                else
                    color = guardianAngel.Color;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Whisperer) && !DeadSeeEverything)
        {
            var whisperer = localinfo[0] as Whisperer;

            if (whisperer.Persuaded.Contains(player.PlayerId) && player != CustomPlayer.Local)
            {
                if (CustomGameOptions.FactionSeeRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#F995FCFF>Λ</color>\n{role}";
                    roleRevealed = true;
                }
                else
                    color = whisperer.SubFactionColor;
            }
            else
            {
                foreach (var (key, value) in whisperer.PlayerConversion)
                {
                    if (player.PlayerId == key)
                        name += $" {value}%";
                }
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Dracula) && !DeadSeeEverything)
        {
            var dracula = localinfo[0] as Dracula;

            if (dracula.Converted.Contains(player.PlayerId) && player != CustomPlayer.Local)
            {
                if (CustomGameOptions.FactionSeeRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#7B8968FF>γ</color>\n{role}";
                    roleRevealed = true;
                }
                else
                    color = dracula.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Jackal) && !DeadSeeEverything)
        {
            var jackal = localinfo[0] as Jackal;

            if (jackal.Recruited.Contains(player.PlayerId) && player != CustomPlayer.Local)
            {
                if (CustomGameOptions.FactionSeeRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#575657FF>$</color>\n{role}";
                    roleRevealed = true;
                }
                else
                    color = jackal.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Necromancer) && !DeadSeeEverything)
        {
            var necromancer = localinfo[0] as Necromancer;

            if (necromancer.Resurrected.Contains(player.PlayerId) && player != CustomPlayer.Local)
            {
                if (CustomGameOptions.FactionSeeRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#E6108AFF>Σ</color>\n{role}";
                    roleRevealed = true;
                }
                else
                    color = necromancer.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.Is(Alignment.NeutralKill) && !DeadSeeEverything && CustomGameOptions.NKsKnow)
        {
            if (((player.GetRole() == CustomPlayer.Local.GetRole() && CustomGameOptions.NoSolo == NoSolo.SameNKs) || (player.GetAlignment() == CustomPlayer.Local.GetAlignment() &&
                CustomGameOptions.NoSolo == NoSolo.AllNKs)) && !roleRevealed)
            {
                var role = info[0] as Role;
                color = role.Color;
                name += $"\n{role}";
                roleRevealed = true;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Blackmailer))
        {
            var blackmailer = localinfo[0] as Blackmailer;

            if (blackmailer.BlackmailedPlayer == player)
            {
                name += " <color=#02A752FF>Φ</color>";
                color = blackmailer.Color;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
        {
            var inspector = localinfo[0] as Inspector;

            if (inspector.Inspected.Contains(player.PlayerId) && !roleRevealed)
            {
                name += $"\n{player.GetInspResults()}";
                color = inspector.Color;
                roleRevealed = true;
            }
        }

        if (CustomPlayer.Local.IsBitten() && !DeadSeeEverything)
        {
            var dracula = CustomPlayer.Local.GetDracula();

            if (dracula.Converted.Contains(player.PlayerId) && player != CustomPlayer.Local)
            {
                if (CustomGameOptions.FactionSeeRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#7B8968FF>γ</color>\n{role}";
                    roleRevealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.PlayerId))
                            consigliere.Investigated.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.PlayerId))
                            godfather.Investigated.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
                    {
                        var inspector = localinfo[0] as Inspector;

                        if (inspector.Inspected.Contains(player.PlayerId))
                            inspector.Inspected.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                    {
                        var retributionist = localinfo[0] as Retributionist;

                        if (retributionist.Inspected.Contains(player.PlayerId))
                            retributionist.Inspected.Remove(player.PlayerId);
                    }
                }
                else
                    color = dracula.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.IsRecruit() && !DeadSeeEverything)
        {
            var jackal = CustomPlayer.Local.GetJackal();

            if (jackal.Recruited.Contains(player.PlayerId) && player != CustomPlayer.Local)
            {
                if (CustomGameOptions.FactionSeeRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#575657FF>$</color>\n{role}";
                    roleRevealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.PlayerId))
                            consigliere.Investigated.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.PlayerId))
                            godfather.Investigated.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
                    {
                        var inspector = localinfo[0] as Inspector;

                        if (inspector.Inspected.Contains(player.PlayerId))
                            inspector.Inspected.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                    {
                        var retributionist = localinfo[0] as Retributionist;

                        if (retributionist.Inspected.Contains(player.PlayerId))
                            retributionist.Inspected.Remove(player.PlayerId);
                    }
                }
                else
                    color = jackal.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.IsResurrected() && !DeadSeeEverything)
        {
            var necromancer = CustomPlayer.Local.GetNecromancer();

            if (necromancer.Resurrected.Contains(player.PlayerId) && player != CustomPlayer.Local)
            {
                if (CustomGameOptions.FactionSeeRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#E6108AFF>Σ</color>\n{role}";
                    roleRevealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.PlayerId))
                            consigliere.Investigated.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.PlayerId))
                            godfather.Investigated.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
                    {
                        var inspector = localinfo[0] as Inspector;

                        if (inspector.Inspected.Contains(player.PlayerId))
                            inspector.Inspected.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                    {
                        var retributionist = localinfo[0] as Retributionist;

                        if (retributionist.Inspected.Contains(player.PlayerId))
                            retributionist.Inspected.Remove(player.PlayerId);
                    }
                }
                else
                    color = necromancer.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.IsPersuaded() && !DeadSeeEverything)
        {
            var whisperer = CustomPlayer.Local.GetWhisperer();

            if (whisperer.Persuaded.Contains(player.PlayerId) && player != CustomPlayer.Local)
            {
                if (CustomGameOptions.FactionSeeRoles)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#F995FCFF>Λ</color>\n{role}";
                    roleRevealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.PlayerId))
                            consigliere.Investigated.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.PlayerId))
                            godfather.Investigated.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
                    {
                        var inspector = localinfo[0] as Inspector;

                        if (inspector.Inspected.Contains(player.PlayerId))
                            inspector.Inspected.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                    {
                        var retributionist = localinfo[0] as Retributionist;

                        if (retributionist.Inspected.Contains(player.PlayerId))
                            retributionist.Inspected.Remove(player.PlayerId);
                    }
                }
                else
                    color = whisperer.SubFactionColor;
            }
            else
            {
                foreach (var (key, value) in whisperer.PlayerConversion)
                {
                    if (player.PlayerId == key)
                        name += $" {value}%";
                }
            }
        }

        if (CustomPlayer.Local.Is(LayerEnum.Lovers) && !DeadSeeEverything)
        {
            var lover = localinfo[3] as Objectifier;
            var otherLover = CustomPlayer.Local.GetOtherLover();

            if (otherLover == player)
            {
                name += $" {lover.ColoredSymbol}";

                if (CustomGameOptions.LoversRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    roleRevealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.PlayerId))
                            consigliere.Investigated.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.PlayerId))
                            godfather.Investigated.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
                    {
                        var inspector = localinfo[0] as Inspector;

                        if (inspector.Inspected.Contains(player.PlayerId))
                            inspector.Inspected.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                    {
                        var retributionist = localinfo[0] as Retributionist;

                        if (retributionist.Inspected.Contains(player.PlayerId))
                            retributionist.Inspected.Remove(player.PlayerId);
                    }
                }
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Rivals) && !DeadSeeEverything)
        {
            var rival = localinfo[3] as Objectifier;
            var otherRival = CustomPlayer.Local.GetOtherRival();

            if (otherRival == player)
            {
                name += $" {rival.ColoredSymbol}";

                if (CustomGameOptions.RivalsRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    roleRevealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.PlayerId))
                            consigliere.Investigated.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.PlayerId))
                            godfather.Investigated.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
                    {
                        var inspector = localinfo[0] as Inspector;

                        if (inspector.Inspected.Contains(player.PlayerId))
                            inspector.Inspected.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                    {
                        var retributionist = localinfo[0] as Retributionist;

                        if (retributionist.Inspected.Contains(player.PlayerId))
                            retributionist.Inspected.Remove(player.PlayerId);
                    }
                }
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Linked) && !DeadSeeEverything)
        {
            var link = localinfo[3] as Objectifier;
            var otherLink = CustomPlayer.Local.GetOtherLink();

            if (otherLink == player)
            {
                name += $" {link.ColoredSymbol}";

                if (CustomGameOptions.LinkedRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    roleRevealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.PlayerId))
                            consigliere.Investigated.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.PlayerId))
                            godfather.Investigated.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
                    {
                        var inspector = localinfo[0] as Inspector;

                        if (inspector.Inspected.Contains(player.PlayerId))
                            inspector.Inspected.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                    {
                        var retributionist = localinfo[0] as Retributionist;

                        if (retributionist.Inspected.Contains(player.PlayerId))
                            retributionist.Inspected.Remove(player.PlayerId);
                    }
                }
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Mafia) && !DeadSeeEverything)
        {
            var mafia = localinfo[3] as Mafia;

            if (player.Is(LayerEnum.Mafia) && player != CustomPlayer.Local)
            {
                name += $" {mafia.ColoredSymbol}";

                if (CustomGameOptions.MafiaRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    roleRevealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.PlayerId))
                            consigliere.Investigated.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.PlayerId))
                            godfather.Investigated.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
                    {
                        var inspector = localinfo[0] as Inspector;

                        if (inspector.Inspected.Contains(player.PlayerId))
                            inspector.Inspected.Remove(player.PlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                    {
                        var retributionist = localinfo[0] as Retributionist;

                        if (retributionist.Inspected.Contains(player.PlayerId))
                            retributionist.Inspected.Remove(player.PlayerId);
                    }
                }
            }
        }

        if (CustomPlayer.Local.Is(LayerEnum.Snitch) && !DeadSeeEverything)
        {
            var role = localinfo[0] as Role;

            if (role.TasksDone)
            {
                var role2 = info[0] as Role;

                if (CustomGameOptions.SnitchSeesRoles)
                {
                    if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals) ||
                        (player.Is(Faction.Crew) && CustomGameOptions.SnitchSeesCrew))
                    {
                        color = role2.Color;
                        name += $"\n{role2.Name}";
                        roleRevealed = true;
                    }
                }
                else if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals) || (player.Is(Faction.Crew) &&
                    CustomGameOptions.SnitchSeesCrew))
                {
                    if (!(player.Is(LayerEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor) && !(player.Is(LayerEnum.Fanatic) && CustomGameOptions.SnitchSeesFanatic))
                    {
                        color = role2.FactionColor;
                        name += $"\n{role2.FactionName}";
                    }
                    else
                    {
                        color = Colors.Crew;
                        name += "\nCrew";
                    }

                    roleRevealed = true;
                }
            }
        }

        if (player.Is(LayerEnum.Snitch) && !DeadSeeEverything && player != CustomPlayer.Local)
        {
            var role = info[0] as Role;

            if (role.TasksDone || role.TasksLeft <= CustomGameOptions.SnitchTasksRemaining)
            {
                var ability = info[2] as Ability;
                color = ability.Color;
                name += (name.Contains('\n') ? " " : "\n") + ability.Name;
                roleRevealed = true;
            }
        }

        if (CustomPlayer.Local.GetFaction() == player.GetFaction() && player != CustomPlayer.Local && (player.GetFaction() == Faction.Intruder || player.GetFaction() == Faction.Syndicate)
            && !DeadSeeEverything)
        {
            var role = info[0] as Role;

            if (CustomGameOptions.FactionSeeRoles && !roleRevealed)
            {
                color = role.Color;
                name += $"\n{role}";
                roleRevealed = true;

                if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                {
                    var consigliere = localinfo[0] as Consigliere;

                    if (consigliere.Investigated.Contains(player.PlayerId))
                        consigliere.Investigated.Remove(player.PlayerId);
                }
                else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                {
                    var godfather = localinfo[0] as PromotedGodfather;

                    if (godfather.Investigated.Contains(player.PlayerId))
                        godfather.Investigated.Remove(player.PlayerId);
                }
            }
            else
                color = role.FactionColor;

            if (player.SyndicateSided() || player.IntruderSided())
            {
                var objectifier = info[3] as Objectifier;
                name += $" {objectifier.ColoredSymbol}";
            }
            else
                name += $" {role.FactionColorString}ξ</color>";
        }

        if ((CustomPlayer.Local.Is(Faction.Syndicate) || DeadSeeEverything) && (player == Role.DriveHolder || (CustomGameOptions.GlobalDrive && Role.SyndicateHasChaosDrive &&
            player.Is(Faction.Syndicate))))
        {
            name += " <color=#008000FF>Δ</color>";
        }

        if (Role.GetRoles<Revealer>(LayerEnum.Revealer).Any(x => x.CompletedTasks) && CustomPlayer.Local.Is(Faction.Crew))
        {
            var role = info[0] as Role;

            if (CustomGameOptions.RevealerRevealsRoles)
            {
                if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.RevealerRevealsNeutrals) || (player.Is(Faction.Crew)
                    && CustomGameOptions.RevealerRevealsCrew))
                {
                    color = role.Color;
                    name += $"\n{role}";
                    roleRevealed = true;
                }
            }
            else if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.RevealerRevealsNeutrals) || (player.Is(Faction.Crew)
                && CustomGameOptions.RevealerRevealsCrew))
            {
                if (!(player.Is(LayerEnum.Traitor) && CustomGameOptions.RevealerRevealsTraitor) && !(player.Is(LayerEnum.Fanatic) && CustomGameOptions.RevealerRevealsFanatic))
                {
                    color = role.FactionColor;
                    name += $"\n{role.FactionName}";
                }
                else
                {
                    color = Colors.Crew;
                    name += "\nCrew";
                }

                roleRevealed = true;
            }
        }

        if ((player == CustomPlayer.Local || DeadSeeEverything) && player.IsVesting())
            name += " <color=#DDDD00FF>υ</color>";

        if (player == CustomPlayer.Local && !DeadSeeEverything)
        {
            if ((player.IsShielded() || player.IsRetShielded()) && (int)CustomGameOptions.ShowShielded is 0 or 2)
                name += " <color=#006600FF>✚</color>";

            if (player.IsProtected() && (int)CustomGameOptions.ShowProtect is 0 or 2)
                name += " <color=#FFFFFFFF>η</color>";

            if (player.IsBHTarget())
                name += " <color=#B51E39FF>Θ</color>";

            if (player.IsExeTarget() && CustomGameOptions.ExeTargetKnows)
                name += " <color=#CCCCCCFF>§</color>";

            if (player.IsGATarget() && CustomGameOptions.GATargetKnows)
                name += " <color=#FFFFFFFF>★</color>";

            if (player.IsGuessTarget() && CustomGameOptions.GuesserTargetKnows)
                name += " <color=#EEE5BEFF>π</color>";

            if (player.IsBitten())
                name += " <color=#7B8968FF>γ</color>";

            if (player.IsRecruit())
                name += " <color=#575657FF>$</color>";

            if (player.IsResurrected())
                name += " <color=#E6108AFF>Σ</color>";

            if (player.IsPersuaded())
                name += " <color=#F995FCFF>Λ</color>";
        }

        if (DeadSeeEverything)
        {
            if ((player.IsShielded() || player.IsRetShielded()) && CustomGameOptions.ShowShielded != ShieldOptions.Everyone)
                name += " <color=#006600FF>✚</color>";

            if (player.IsProtected() && CustomGameOptions.ShowProtect != ProtectOptions.Everyone)
                name += " <color=#FFFFFFFF>η</color>";

            if (player.IsBHTarget())
                name += " <color=#B51E39FF>Θ</color>";

            if (player.IsExeTarget())
                name += " <color=#CCCCCCFF>§</color>";

            if (player.IsGATarget())
                name += " <color=#FFFFFFFF>★</color>";

            if (player.IsGuessTarget())
                name += " <color=#EEE5BEFF>π</color>";

            if (player.IsBitten())
                name += " <color=#7B8968FF>γ</color>";

            if (player.IsRecruit())
                name += " <color=#575657FF>$</color>";

            if (player.IsResurrected())
                name += " <color=#E6108AFF>Σ</color>";

            if (player.IsPersuaded())
                name += " <color=#F995FCFF>Λ</color>";

            if (player.IsFramed())
                name += " <color=#00FFFFFF>ς</color>";

            if (player.IsInfected())
                name += " <color=#CFFE61FF>ρ</color>";

            if (player.IsArsoDoused())
                name += " <color=#EE7600FF>Ξ</color>";

            if (player.IsCryoDoused())
                name += " <color=#642DEAFF>λ</color>";

            if (player.IsSpelled())
                name += " <color=#0028F5FF>ø</color>";

            if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
            {
                var consigliere = localinfo[0] as Consigliere;
                consigliere.Investigated.Clear();
            }

            if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
            {
                var godfather = localinfo[0] as PromotedGodfather;
                godfather.Investigated.Clear();
            }

            if (CustomPlayer.Local.Is(LayerEnum.Inspector))
            {
                var inspector = localinfo[0] as Inspector;
                inspector.Inspected.Clear();
            }

            if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
            {
                var retributionist = localinfo[0] as Retributionist;
                retributionist.Inspected.Clear();
            }
        }

        if ((player.IsShielded() || player.IsRetShielded()) && (int)CustomGameOptions.ShowShielded is 3 && !DeadSeeEverything)
            name += " <color=#006600FF>✚</color>";

        if (player.IsProtected() && (int)CustomGameOptions.ShowProtect is 3 && !DeadSeeEverything)
            name += " <color=#FFFFFFFF>η</color>";

        if (DeadSeeEverything || player == CustomPlayer.Local)
        {
            if (info[3] != null)
            {
                var objectifier = info[3] as Objectifier;

                if (objectifier.Type != LayerEnum.None && !objectifier.Hidden)
                    name += $" {objectifier.ColoredSymbol}";
            }

            if (!roleRevealed)
            {
                var role = info[0] as Role;
                color = role.Color;
                name += $"\n{role}";
                roleRevealed = true;
            }
        }

        if (roleRevealed)
            player.NameText().transform.localPosition = new(0f, 0.15f, -0.5f);

        return (name, color);
    }
}