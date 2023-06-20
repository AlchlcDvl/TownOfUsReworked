namespace TownOfUsReworked.PlayerLayers
{
    public abstract class PlayerLayer
    {
        public Color32 Color = Colors.Layer;
        public string Name = "Layerless";
        public string Short => Info.AllInfo.Find(x => x.Name == Name)?.Short;

        public PlayerLayerEnum LayerType = PlayerLayerEnum.None;
        public RoleEnum RoleType = RoleEnum.None;
        public ModifierEnum ModifierType = ModifierEnum.None;
        public ObjectifierEnum ObjectifierType = ObjectifierEnum.None;
        public AbilityEnum AbilityType = AbilityEnum.None;
        public LayerEnum Type = LayerEnum.None;

        public bool Local => Player == CustomPlayer.Local;

        public bool IsBlocked;

        public static bool NobodyWins;

        public bool Winner;

        public readonly static List<PlayerLayer> AllLayers = new();
        public static List<PlayerLayer> LocalLayers => GetLayers(CustomPlayer.Local);

        public virtual void OnLobby() => EndGame.Reset();

        public virtual void UpdateHud(HudManager __instance)
        {
            var Vent = __instance.ImpostorVentButton.graphic.sprite;

            if (Player.Is(Faction.Intruder))
                Vent = AssetManager.GetSprite("IntruderVent");
            else if (Player.Is(Faction.Syndicate))
                Vent = AssetManager.GetSprite("SyndicateVent");
            else if (Player.Is(Faction.Crew))
                Vent = AssetManager.GetSprite("CrewVent");
            else if (Player.Is(Faction.Neutral))
                Vent = AssetManager.GetSprite("NeutralVent");

            if (__instance.ImpostorVentButton.currentTarget == null || IsBlocked)
                __instance.ImpostorVentButton.SetDisabled();
            else
                __instance.ImpostorVentButton.SetEnabled();

            __instance.ImpostorVentButton.graphic.sprite = Vent;
            __instance.ImpostorVentButton.buttonLabelText.text = IsBlocked ? "BLOCKED" : "VENT";
            __instance.ImpostorVentButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;
            __instance.ImpostorVentButton.gameObject.SetActive(Player.CanVent() || Player.inVent);

            var closestDead = Player.GetClosestDeadPlayer(CustomGameOptions.ReportDistance);

            if (closestDead == null || Player.CannotUse())
                __instance.ReportButton.SetDisabled();
            else
                __instance.ReportButton.SetEnabled();

            __instance.ReportButton.buttonLabelText.text = IsBlocked ? "BLOCKED" : "REPORT";
            __instance.ReportButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;

            if (Player.closest == null || IsBlocked)
                __instance.UseButton.SetDisabled();
            else
                __instance.UseButton.SetEnabled();

            __instance.UseButton.buttonLabelText.text = IsBlocked ? "BLOCKED" : "USE";
            __instance.UseButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;

            if (IsBlocked)
                __instance.PetButton.SetDisabled();
            else
                __instance.PetButton.SetEnabled();

            __instance.PetButton.buttonLabelText.text = IsBlocked ? "BLOCKED" : "PET";
            __instance.PetButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;

            if (Player.CannotUse())
                __instance.SabotageButton.SetDisabled();
            else
                __instance.SabotageButton.SetEnabled();

            var Sabo = __instance.SabotageButton.graphic.sprite;

            if (Player.Is(Faction.Syndicate))
                Sabo = AssetManager.GetSprite("SyndicateSabotage");
            else if (Player.Is(Faction.Intruder))
                Sabo = AssetManager.GetSprite("Sabotage");

            __instance.SabotageButton.graphic.sprite = Sabo;
            __instance.SabotageButton.buttonLabelText.text = IsBlocked ? "BLOCKED" : "SABOTAGE";

            if (IsBlocked && Minigame.Instance)
                Minigame.Instance.Close();

            if (MapBehaviour.Instance && IsBlocked)
                MapBehaviour.Instance.Close();
        }

        public virtual void UpdateMeeting(MeetingHud __instance) => Player.DisableButtons();

        public virtual void VoteComplete(MeetingHud __instance) {}

        public virtual void ConfirmVotePrefix(MeetingHud __instance) {}

        public virtual void ConfirmVotePostfix(MeetingHud __instance) {}

        public virtual void ClearVote(MeetingHud __instance) {}

        public virtual void SelectVote(MeetingHud __instance, int id) {}

        public virtual void UpdateMap(MapBehaviour __instance) {}

        public virtual void OnMeetingStart(MeetingHud __instance)
        {
            Player.DisableButtons();
            Player.RegenTask();
            EndGame.Reset();
            Ash.DestroyAll();
        }

        public virtual void OnMeetingEnd(MeetingHud __instance)
        {
            Player.EnableButtons();
            Player.RegenTask();
            EndGame.Reset();
            ButtonUtils.ResetCustomTimers(false);
        }

        public virtual void OnBodyReport(GameData.PlayerInfo info) => EndGame.Reset();

        protected PlayerLayer(PlayerControl player)
        {
            Player = player;
            AllLayers.Add(this);
        }

        public PlayerControl Player;

        public bool IsDead => Player.Data.IsDead;
        public bool Disconnected => Player.Data.Disconnected;

        public string PlayerName => Player?.Data.PlayerName;
        public byte PlayerId => (byte)Player?.PlayerId;
        public int TasksLeft => Player.Data.Tasks.Count(x => !x.Complete);
        public int TasksCompleted => Player.Data.Tasks.Count(x => x.Complete);
        public int TotalTasks => Player.Data.Tasks.ToArray().Length;
        public bool TasksDone => Player != null && (TasksLeft <= 0 || TasksCompleted >= TotalTasks);

        public string ColorString => $"<color=#{Color.ToHtmlStringRGBA()}>";

        public void GameEnd()
        {
            if (Player == null || Disconnected)
                return;
            else if (IsDead)
            {
                if (Type == LayerEnum.Phantom && TasksDone)
                {
                    Role.PhantomWins = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.PhantomWin);
                    writer.Write(PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
                else if (LayerType == PlayerLayerEnum.Role && ((Role)this).RoleAlignment == RoleAlignment.NeutralEvil && CustomGameOptions.NeutralEvilsEndGame)
                {
                    if (Type == LayerEnum.Jester && ((Jester)this).VotedOut)
                    {
                        Role.JesterWins = true;
                        Winner = true;
                        Utils.EndGame();
                    }
                    else if (Type == LayerEnum.Executioner && ((Executioner)this).TargetVotedOut)
                    {
                        Role.ExecutionerWins = true;
                        Winner = true;
                        Utils.EndGame();
                    }
                    else if (Type == LayerEnum.Actor && ((Actor)this).Guessed)
                    {
                        Role.ActorWins = true;
                        Winner = true;
                        Utils.EndGame();
                    }
                    else if (Type == LayerEnum.Troll && ((Troll)this).Killed)
                    {
                        Role.TrollWins = true;
                        Winner = true;
                        Utils.EndGame();
                    }
                }

                return;
            }
            else if (LayerType == PlayerLayerEnum.Objectifier)
            {
                if (Type == LayerEnum.Corrupted && ConstantVariables.CorruptedWin(Player))
                {
                    Objectifier.CorruptedWins = true;

                    if (CustomGameOptions.AllCorruptedWin)
                    {
                        foreach (var corr in Objectifier.GetObjectifiers<Corrupted>(ObjectifierEnum.Corrupted))
                            corr.Winner = true;
                    }

                    Winner = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.CorruptedWin);
                    writer.Write(PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
                else if (Type == LayerEnum.Lovers && ConstantVariables.LoversWin(Player))
                {
                    Objectifier.LoveWins = true;
                    Winner = true;
                    Objectifier.GetObjectifier(((Lovers)this).OtherLover).Winner = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.LoveWin);
                    writer.Write(PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
                else if (Type == LayerEnum.Rivals && ConstantVariables.RivalsWin(Player))
                {
                    Objectifier.RivalWins = true;
                    Winner = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.RivalWin);
                    writer.Write(PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
                else if (Type == LayerEnum.Taskmaster && TasksDone)
                {
                    Objectifier.TaskmasterWins = true;
                    Winner = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.TaskmasterWin);
                    writer.Write(PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
                else if (Type == LayerEnum.Mafia && ConstantVariables.MafiaWin)
                {
                    Objectifier.MafiaWins = true;
                    Winner = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.MafiaWins);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
                else if (Type == LayerEnum.Overlord && MeetingPatches.MeetingCount >= CustomGameOptions.OverlordMeetingWinCount && !IsDead && !Disconnected)
                {
                    Objectifier.OverlordWins = true;

                    foreach (var ov in Objectifier.GetObjectifiers<Overlord>(ObjectifierEnum.Overlord))
                    {
                        if (ov.IsAlive)
                            ov.Winner = true;
                    }

                    Winner = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.OverlordWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
            }
            else if (LayerType == PlayerLayerEnum.Role)
            {
                var role = (Role)this;

                if ((role.IsRecruit || Type == LayerEnum.Jackal) && ConstantVariables.CabalWin)
                {
                    Role.CabalWin = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.CabalWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
                else if ((role.IsPersuaded || Type == LayerEnum.Whisperer) && ConstantVariables.SectWin)
                {
                    Role.SectWin = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.SectWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
                else if ((role.IsBitten || Type == LayerEnum.Dracula) && ConstantVariables.UndeadWin)
                {
                    Role.UndeadWin = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.UndeadWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
                else if ((role.IsResurrected || Type == LayerEnum.Necromancer) && ConstantVariables.ReanimatedWin)
                {
                    Role.ReanimatedWin = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.ReanimatedWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
                else if (role.Faction == Faction.Syndicate && (role.Faithful || Type == LayerEnum.Betrayer || role.IsSynAlly  || role.IsSynDefect || role.IsSynFanatic ||
                    role.IsSynTraitor) && ConstantVariables.SyndicateWins)
                {
                    Role.SyndicateWin = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.SyndicateWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
                else if (role.Faction == Faction.Intruder && (role.Faithful || Type == LayerEnum.Betrayer || role.IsIntDefect  || role.IsIntAlly || role.IsIntFanatic ||
                    role.IsIntTraitor) && ConstantVariables.IntrudersWin)
                {
                    Role.IntruderWin = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.IntruderWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
                else if (role.Faction == Faction.Crew && (role.Faithful || role.IsCrewAlly || role.IsCrewDefect) && ConstantVariables.CrewWins)
                {
                    Role.CrewWin = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.CrewWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
                else if (role.Faithful && ConstantVariables.PestOrPBWins && Type is LayerEnum.Plaguebearer or LayerEnum.Pestilence)
                {
                    Role.InfectorsWin = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.InfectorsWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
                else if (ConstantVariables.AllNeutralsWin && role.Faithful)
                {
                    Role.AllNeutralsWin = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.AllNeutralsWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
                else if (ConstantVariables.AllNKsWin && role.Faithful && role.RoleAlignment == RoleAlignment.NeutralKill)
                {
                    Role.NKWins = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.AllNKsWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
                else if (role.Faithful && role.RoleAlignment == RoleAlignment.NeutralKill && (ConstantVariables.SameNKWins(RoleType) || ConstantVariables.SoloNKWins(Player)))
                {
                    switch (RoleType)
                    {
                        case RoleEnum.Glitch:
                            Role.GlitchWins = true;
                            break;

                        case RoleEnum.Arsonist:
                            Role.ArsonistWins = true;
                            break;

                        case RoleEnum.Cryomaniac:
                            Role.CryomaniacWins = true;
                            break;

                        case RoleEnum.Juggernaut:
                            Role.JuggernautWins = true;
                            break;

                        case RoleEnum.Murderer:
                            Role.MurdererWins = true;
                            break;

                        case RoleEnum.Werewolf:
                            Role.WerewolfWins = true;
                            break;

                        case RoleEnum.SerialKiller:
                            Role.SerialKillerWins = true;
                            break;
                    }

                    if (CustomGameOptions.NoSolo == NoSolo.SameNKs)
                    {
                        foreach (var role2 in Role.GetRoles(RoleType))
                        {
                            if (!role2.Disconnected && role2.Faithful)
                                role2.Winner = true;
                        }
                    }

                    Winner = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)(CustomGameOptions.NoSolo == NoSolo.SameNKs ? WinLoseRPC.SameNKWins : WinLoseRPC.SoloNKWins));
                    writer.Write(PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
                else if (CustomGameOptions.NeutralEvilsEndGame && role.RoleAlignment == RoleAlignment.NeutralEvil)
                {
                    if (Type == LayerEnum.Executioner && ((Executioner)role).TargetVotedOut)
                    {
                        Role.ExecutionerWins = true;
                        Winner = true;
                        Utils.EndGame();
                    }
                    else if (Type == LayerEnum.BountyHunter && ((BountyHunter)role).TargetKilled)
                    {
                        Role.BountyHunterWins = true;
                        Winner = true;
                        Utils.EndGame();
                    }
                    else if (Type == LayerEnum.Cannibal && ((Cannibal)role).Eaten)
                    {
                        Role.CannibalWins = true;
                        Winner = true;
                        Utils.EndGame();
                    }
                    else if (Type == LayerEnum.Guesser && ((Guesser)role).TargetGuessed)
                    {
                        Role.GuesserWins = true;
                        Winner = true;
                        Utils.EndGame();
                    }
                }
            }
        }

        public static bool operator ==(PlayerLayer a, PlayerLayer b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Type == b.Type && a.Player == b.Player;
        }

        public static bool operator !=(PlayerLayer a, PlayerLayer b) => !(a == b);

        public static implicit operator bool(PlayerLayer exists) => exists != null;

        private bool Equals(PlayerLayer other) => Equals(Player, other.Player) && Type == other.Type && GetHashCode() == other.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != typeof(PlayerLayer))
                return false;

            return Equals((PlayerLayer)obj);
        }

        public override int GetHashCode() => HashCode.Combine(Player, (int)Type);

        public override string ToString() => Name;

        public static void DeleteAll()
        {
            foreach (var layer in AllLayers)
            {
                layer.OnLobby();
                layer.Player = null;
            }

            Role.AllRoles.Clear();
            Objectifier.AllObjectifiers.Clear();
            Modifier.AllModifiers.Clear();
            Ability.AllAbilities.Clear();
        }

        public static List<PlayerLayer> GetLayers(PlayerControl player) => AllLayers.Where(x => x.Player == player).ToList();
    }
}
