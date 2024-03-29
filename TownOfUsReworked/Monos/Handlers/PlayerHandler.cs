namespace TownOfUsReworked.Monos;

public class PlayerHandler : MonoBehaviour
{
    public readonly Dictionary<byte, string> PlayerNames = [];
    public readonly Dictionary<byte, string> ColorNames = [];
    private static Vector3? NamePos;

    public static PlayerHandler Instance { get; private set; }

    public PlayerHandler(IntPtr ptr) : base(ptr) => Instance = this;

    public void Update()
    {
        if (NoPlayers || IsHnS || Meeting)
            return;

        CustomPlayer.AllPlayers.ForEach(UpdatePlayer);
    }

    private static void UpdatePlayer(PlayerControl player)
    {
        NamePos ??= player.NameText().transform.localPosition;
        Instance.PlayerNames[player.PlayerId] = player.Data.PlayerName;
        Instance.ColorNames[player.PlayerId] = player.Data.ColorName.Replace("(", "").Replace(")", "");

        if (IsInGame)
        {
            (player.NameText().text, player.NameText().color) = UpdateGameName(player);
            (player.ColorBlindText().text, player.ColorBlindText().color) = (UpdateColorblind(player), player.NameText().color);
            player.transform.localScale = CustomPlayer.Custom(player).SizeFactor;
        }
    }

    private static string UpdateColorblind(PlayerControl player)
    {
        if (!DataManager.Settings.Accessibility.ColorBlindMode)
            return "";

        var distance = Vector2.Distance(CustomPlayer.Local.transform.position, player.transform.position);
        var vector = player.transform.position - CustomPlayer.Local.transform.position;

        if (PhysicsHelpers.AnyNonTriggersBetween(CustomPlayer.Local.transform.position, vector.normalized, distance, Constants.ShipAndObjectsMask) && player != CustomPlayer.Local &&
            !CustomPlayer.LocalCustom.Dead && CustomGameOptions.PlayerNames == Data.PlayerNames.Obstructed)
        {
            return "";
        }

        if (!TransitioningSize.ContainsKey(player.PlayerId))
            player.IsMimicking(out player);

        var name = "";

        if (HudHandler.Instance.IsCamoed && player != CustomPlayer.Local && !CustomPlayer.LocalCustom.Dead && !ClientGameOptions.OptimisationMode)
            name = GetRandomisedName();
        else if (CustomGameOptions.PlayerNames == Data.PlayerNames.NotVisible && !IsLobby)
            name = "";
        else if (CachedMorphs.TryGetValue(player.PlayerId, out var cache) && player != CustomPlayer.Local && !CustomPlayer.LocalCustom.Dead)
            name = Instance.ColorNames.FirstOrDefault(x => x.Key == cache).Value;
        else if (player.GetCustomOutfitType() is CustomPlayerOutfitType.Invis or CustomPlayerOutfitType.Colorblind && player != CustomPlayer.Local && !CustomPlayer.LocalCustom.Dead)
            name = "";
        else
            name = Instance.ColorNames.FirstOrDefault(x => x.Key == player.PlayerId).Value;

        var ld = player.CurrentOutfit.ColorId.IsLighter() ? "L" : "D";

        if (ClientGameOptions.LighterDarker)
            name += $" ({ld})";

        return name;
    }

    public static (string, UColor) UpdateGameName(PlayerControl player)
    {
        var distance = Vector2.Distance(CustomPlayer.Local.transform.position, player.transform.position);
        var vector = player.transform.position - CustomPlayer.Local.transform.position;

        if (PhysicsHelpers.AnyNonTriggersBetween(CustomPlayer.Local.transform.position, vector.normalized, distance, Constants.ShipAndObjectsMask) && player != CustomPlayer.Local &&
            !CustomPlayer.LocalCustom.Dead && CustomGameOptions.PlayerNames == Data.PlayerNames.Obstructed)
        {
            return ("", UColor.clear);
        }

        if (player != CustomPlayer.Local && !DeadSeeEverything && !TransitioningSize.ContainsKey(player.PlayerId))
            player.IsMimicking(out player);

        var name = "";
        var color = UColor.white;
        var info = player.GetLayers();
        var localinfo = CustomPlayer.Local.GetLayers();
        var revealed = false;

        if (HudHandler.Instance.IsCamoed && player != CustomPlayer.Local && !CustomPlayer.LocalCustom.Dead)
            name = ClientGameOptions.OptimisationMode ? "" : GetRandomisedName();
        else if (CustomGameOptions.PlayerNames == Data.PlayerNames.NotVisible && !IsLobby)
            name = "";
        else if (CachedMorphs.TryGetValue(player.PlayerId, out var cache) && player != CustomPlayer.Local && !CustomPlayer.LocalCustom.Dead)
            name = Instance.PlayerNames.FirstOrDefault(x => x.Key == cache).Value;
        else if (player.GetCustomOutfitType() is CustomPlayerOutfitType.Invis or CustomPlayerOutfitType.Colorblind && player != CustomPlayer.Local && !CustomPlayer.LocalCustom.Dead)
            name = "";
        else
            name = Instance.PlayerNames.FirstOrDefault(x => x.Key == player.PlayerId).Value;

        if (info.Count != 4 || localinfo.Count != 4)
            return (name, color);

        if (player.CanDoTasks() && (DeadSeeEverything || player == CustomPlayer.Local || IsCustomHnS || IsTaskRace))
        {
            var role = info[0] as Role;
            name += $" ({role.TasksCompleted}/{role.TotalTasks})";
        }

        if (player.IsKnighted())
            name += " <color=#FF004EFF>κ</color>";

        if (player.IsMarked())
            name += " <color=#F1C40FFF>χ</color>";

        if (player.Data.PlayerName == CachedFirstDead && ((player == CustomPlayer.Local && (int)CustomGameOptions.WhoSeesFirstKillShield == 1) || CustomGameOptions.WhoSeesFirstKillShield ==
            0))
        {
            name += " <color=#C2185BFF>Γ</color>";
        }

        if (player.Is(LayerEnum.Mayor) && !DeadSeeEverything && CustomPlayer.Local != player)
        {
            var mayor = info[0] as Mayor;

            if (mayor.Revealed)
            {
                revealed = true;
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
            }
        }
        else if (player.Is(LayerEnum.Dictator) && !DeadSeeEverything && CustomPlayer.Local != player)
        {
            var dict = info[0] as Dictator;

            if (dict.Revealed)
            {
                revealed = true;
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
            }
        }

        if (CustomPlayer.Local.Is(LayerEnum.Consigliere) && !DeadSeeEverything)
        {
            var consigliere = localinfo[0] as Consigliere;

            if (consigliere.Investigated.Contains(player.PlayerId) && !revealed)
            {
                var role = info[0] as Role;
                revealed = true;

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

            if (godfather.IsConsig && godfather.Investigated.Contains(player.PlayerId) && !revealed)
            {
                var role = info[0] as Role;
                revealed = true;

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
            {
                name += " <color=#02A752FF>Φ</color>";
                color = godfather.Color;
            }
            else if (godfather.IsAmb && godfather.AmbushedPlayer == player)
            {
                name += " <color=#2BD29CFF>人</color>";
                color = godfather.Color;
            }
            if (godfather.FlashedPlayers.Contains(player.PlayerId) && CustomGameOptions.GrenadierIndicators)
            {
                name += " <color=#85AA5BFF>ㅇ</color>";
                color = godfather.Color;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.PromotedRebel) && !DeadSeeEverything)
        {
            var rebel = localinfo[0] as PromotedRebel;

            if (rebel.IsSil && rebel.SilencedPlayer == player)
            {
                name += " <color=#AAB43EFF>乂</color>";
                color = rebel.Color;
            }
            else if (rebel.IsCrus && rebel.CrusadedPlayer == player)
            {
                name += " <color=#DF7AE8FF>τ</color>";
                color = rebel.Color;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Medic))
        {
            var medic = localinfo[0] as Medic;

            if (medic.ShieldedPlayer != null && medic.ShieldedPlayer == player && (int)CustomGameOptions.ShowShielded is 1 or 2)
                name += " <color=#006600FF>✚</color>";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Trapper))
        {
            var trapper = localinfo[0] as Trapper;

            if (trapper.Trapped.Contains(player.PlayerId))
                name += " <color=#BE1C8CFF>∮</color>";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
        {
            var ret = localinfo[0] as Retributionist;

            if (ret.ShieldedPlayer != null && ret.ShieldedPlayer == player && (int)CustomGameOptions.ShowShielded is 1 or 2)
            {
                name += " <color=#006600FF>✚</color>";
                color = ret.Color;
            }

            if (ret.Trapped.Contains(player.PlayerId))
            {
                name += " <color=#BE1C8CFF>∮</color>";
                color = ret.Color;
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

                if (CustomGameOptions.ExeKnowsTargetRole && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    revealed = true;
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

                if (CustomGameOptions.GAKnowsTargetRole && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    revealed = true;
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
                if (CustomGameOptions.FactionSeeRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#F995FCFF>Λ</color>\n{role}";
                    revealed = true;
                }
                else
                    color = whisperer.SubFactionColor;
            }
            else if (whisperer.PlayerConversion.TryGetValue(player.PlayerId, out var value))
                name += $" {value}%";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Dracula) && !DeadSeeEverything)
        {
            var dracula = localinfo[0] as Dracula;

            if (dracula.Converted.Contains(player.PlayerId) && player != CustomPlayer.Local)
            {
                if (CustomGameOptions.FactionSeeRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#7B8968FF>γ</color>\n{role}";
                    revealed = true;
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
                if (CustomGameOptions.FactionSeeRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#575657FF>$</color>\n{role}";
                    revealed = true;
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
                if (CustomGameOptions.FactionSeeRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#E6108AFF>Σ</color>\n{role}";
                    revealed = true;
                }
                else
                    color = necromancer.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.Is(Alignment.NeutralKill) && !DeadSeeEverything && CustomGameOptions.NKsKnow)
        {
            if (((player.GetRole().Type == Role.LocalRole.Type && CustomGameOptions.NoSolo == NoSolo.SameNKs) || (player.GetAlignment() == CustomPlayer.Local.GetAlignment() &&
                CustomGameOptions.NoSolo == NoSolo.AllNKs)) && !revealed)
            {
                var role = info[0] as Role;
                color = role.Color;
                name += $"\n{role}";
                revealed = true;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Grenadier))
        {
            var grenadier = localinfo[0] as Grenadier;

            if (grenadier.FlashedPlayers.Contains(player.PlayerId) && CustomGameOptions.GrenadierIndicators)
            {
                name += " <color=#85AA5BFF>ㅇ</color>";
                color = grenadier.Color;
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
        else if (CustomPlayer.Local.Is(LayerEnum.Silencer))
        {
            var silencer = localinfo[0] as Silencer;

            if (silencer.SilencedPlayer == player)
            {
                name += " <color=#AAB43EFF>乂</color>";
                color = silencer.Color;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Ambusher))
        {
            var ambusher = localinfo[0] as Ambusher;

            if (ambusher.AmbushedPlayer == player)
            {
                name += " <color=#2BD29CFF>人</color>";
                color = ambusher.Color;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Crusader))
        {
            var crusader = localinfo[0] as Crusader;

            if (crusader.CrusadedPlayer == player)
            {
                name += " <color=#DF7AE8FF>τ</color>";
                color = crusader.Color;
            }
        }
        else if ((IsCustomHnS || IsTaskRace) && player != CustomPlayer.Local)
        {
            name += $"\n{info[0]}";
            color = info[0].Color;
            revealed = true;
        }

        if (CustomPlayer.Local.IsBitten() && !DeadSeeEverything && !CustomPlayer.Local.Is(LayerEnum.Dracula))
        {
            var dracula = CustomPlayer.Local.GetDracula();

            if (dracula.Converted.Contains(player.PlayerId) && player != CustomPlayer.Local)
            {
                if (CustomGameOptions.FactionSeeRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#7B8968FF>γ</color>\n{role}";
                    revealed = true;

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
                    color = dracula.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.IsRecruit() && !DeadSeeEverything && !CustomPlayer.Local.Is(LayerEnum.Jackal))
        {
            var jackal = CustomPlayer.Local.GetJackal();

            if (jackal.Recruited.Contains(player.PlayerId) && player != CustomPlayer.Local)
            {
                if (CustomGameOptions.FactionSeeRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#575657FF>$</color>\n{role}";
                    revealed = true;

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
                    color = jackal.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.IsResurrected() && !DeadSeeEverything && !CustomPlayer.Local.Is(LayerEnum.Necromancer))
        {
            var necromancer = CustomPlayer.Local.GetNecromancer();

            if (necromancer.Resurrected.Contains(player.PlayerId) && player != CustomPlayer.Local)
            {
                if (CustomGameOptions.FactionSeeRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#E6108AFF>Σ</color>\n{role}";
                    revealed = true;

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
                    color = necromancer.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.IsPersuaded() && !DeadSeeEverything && !CustomPlayer.Local.Is(LayerEnum.Whisperer))
        {
            var whisperer = CustomPlayer.Local.GetWhisperer();

            if (whisperer.Persuaded.Contains(player.PlayerId) && player != CustomPlayer.Local)
            {
                if (CustomGameOptions.FactionSeeRoles)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#F995FCFF>Λ</color>\n{role}";
                    revealed = true;

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
                    color = whisperer.SubFactionColor;
            }
            else if (whisperer.PlayerConversion.TryGetValue(player.PlayerId, out var value))
                name += $" {value}%";
        }

        if (CustomPlayer.Local.Is(LayerEnum.Lovers) && !DeadSeeEverything)
        {
            var lover = localinfo[3] as Objectifier;
            var otherLover = CustomPlayer.Local.GetOtherLover();

            if (otherLover == player)
            {
                name += $" {lover.ColoredSymbol}";

                if (CustomGameOptions.LoversRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    revealed = true;

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
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Rivals) && !DeadSeeEverything)
        {
            var rival = localinfo[3] as Objectifier;
            var otherRival = CustomPlayer.Local.GetOtherRival();

            if (otherRival == player)
            {
                name += $" {rival.ColoredSymbol}";

                if (CustomGameOptions.RivalsRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    revealed = true;

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
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Linked) && !DeadSeeEverything)
        {
            var link = localinfo[3] as Objectifier;
            var otherLink = CustomPlayer.Local.GetOtherLink();

            if (otherLink == player)
            {
                name += $" {link.ColoredSymbol}";

                if (CustomGameOptions.LinkedRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    revealed = true;

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
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Mafia) && !DeadSeeEverything)
        {
            var mafia = localinfo[3] as Mafia;

            if (player.Is(LayerEnum.Mafia) && player != CustomPlayer.Local)
            {
                name += $" {mafia.ColoredSymbol}";

                if (CustomGameOptions.MafiaRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    revealed = true;

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
                        revealed = true;
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
                        color = CustomColorManager.Crew;
                        name += "\nCrew";
                    }

                    revealed = true;
                }
            }
        }

        if (player.Is(LayerEnum.Snitch) && !DeadSeeEverything && player != CustomPlayer.Local && (CustomPlayer.Local.Is(Faction.Syndicate) || CustomPlayer.Local.Is(Faction.Intruder) ||
            (CustomPlayer.Local.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals)))
        {
            var role = info[0] as Role;

            if (role.TasksDone || role.TasksLeft <= CustomGameOptions.SnitchTasksRemaining)
            {
                var ability = info[2] as Ability;
                color = ability.Color;
                name += (name.Contains('\n') ? " " : "\n") + ability.Name;
                revealed = true;
            }
        }

        if (CustomPlayer.Local.GetFaction() == player.GetFaction() && player != CustomPlayer.Local && player.GetFaction() is Faction.Intruder or Faction.Syndicate && !DeadSeeEverything)
        {
            var role = info[0] as Role;

            if (CustomGameOptions.FactionSeeRoles && !revealed)
            {
                color = role.Color;
                name += $"\n{role}";
                revealed = true;

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

        if (PlayerLayer.GetLayers<Revealer>().Any(x => x.TasksDone && !x.Caught) && CustomPlayer.Local.Is(Faction.Crew))
        {
            var role = info[0] as Role;

            if (CustomGameOptions.RevealerRevealsRoles)
            {
                if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.RevealerRevealsNeutrals) || (player.Is(Faction.Crew)
                    && CustomGameOptions.RevealerRevealsCrew))
                {
                    color = role.Color;
                    name += $"\n{role}";
                    revealed = true;
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
                    color = CustomColorManager.Crew;
                    name += "\nCrew";
                }

                revealed = true;
            }
        }

        if ((player == CustomPlayer.Local || DeadSeeEverything) && player.IsVesting())
            name += " <color=#DDDD00FF>υ</color>";

        if ((player == CustomPlayer.Local || DeadSeeEverything) && player.IsOnAlert())
            name += " <color=#998040FF>σ</color>";

        if (player == CustomPlayer.Local && !DeadSeeEverything)
        {
            if (player.IsShielded() && (int)CustomGameOptions.ShowShielded is 0 or 2)
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
            if (player.IsShielded() && CustomGameOptions.ShowShielded != ShieldOptions.Everyone)
                name += " <color=#006600FF>✚</color>";

            if (player.IsProtected() && CustomGameOptions.ShowProtect != ProtectOptions.Everyone)
                name += " <color=#FFFFFFFF>η</color>";

            if (player.IsTrapped())
                name += " <color=#BE1C8CFF>∮</color>";

            if (player.IsAmbushed())
                name += " <color=#2BD29CFF>人</color>";

            if (player.IsCrusaded())
                name += " <color=#DF7AE8FF>τ</color>";

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

            if (player.IsSpellbound())
                name += " <color=#0028F5FF>ø</color>";
        }

        if (player.IsShielded() && (int)CustomGameOptions.ShowShielded is 3 && !DeadSeeEverything)
            name += " <color=#006600FF>✚</color>";

        if (player.IsProtected() && (int)CustomGameOptions.ShowProtect is 3 && !DeadSeeEverything)
            name += " <color=#FFFFFFFF>η</color>";

        if ((DeadSeeEverything || CustomPlayer.Local.Is(LayerEnum.Pestilence)) && Pestilence.Infected.TryGetValue(player.PlayerId, out var count))
        {
            for (var i = 0; i < count; i++)
                name += " <color=#424242FF>米</color>";
        }

        if (DeadSeeEverything || player == CustomPlayer.Local)
        {
            var objectifier = info[3] as Objectifier;

            if (objectifier.Type != LayerEnum.NoneObjectifier && !objectifier.Hidden)
                name += $" {objectifier.ColoredSymbol}";

            if (!revealed)
            {
                var role = info[0] as Role;
                color = role.Color;
                name += $"\n{role}";
                revealed = true;
            }
        }

        if (revealed)
            player.NameText().transform.localPosition = new(0f, 0.15f, -0.5f);
        else
            player.NameText().transform.localPosition = NamePos.Value;

        return (name, color);
    }
}