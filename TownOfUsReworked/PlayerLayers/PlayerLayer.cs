namespace TownOfUsReworked.PlayerLayers
{
    public abstract class PlayerLayer
    {
        public virtual Color32 Color => Colors.Layer;
        public virtual string Name => "None";
        public string Short => Info.AllInfo.Find(x => x.Name == Name)?.Short;
        public virtual PlayerLayerEnum LayerType => PlayerLayerEnum.None;
        public virtual LayerEnum Type => LayerEnum.None;

        public bool Local => Player == CustomPlayer.Local;

        public static bool NobodyWins;

        public bool IsBlocked;
        public bool Winner;

        public static readonly List<PlayerLayer> AllLayers = new();
        public static List<PlayerLayer> LocalLayers => GetLayers(CustomPlayer.Local);

        public virtual void OnLobby() => EndGame.Reset();

        public virtual void UpdateHud(HudManager __instance)
        {
            Player.RegenTask();
            var Vent = __instance.ImpostorVentButton.graphic.sprite;

            if (Player.Is(Faction.Intruder))
                Vent = GetSprite("IntruderVent");
            else if (Player.Is(Faction.Syndicate))
                Vent = GetSprite("SyndicateVent");
            else if (Player.Is(Faction.Crew))
                Vent = GetSprite("CrewVent");
            else if (Player.Is(Faction.Neutral))
                Vent = GetSprite("NeutralVent");

            if (__instance.ImpostorVentButton.currentTarget == null || IsBlocked)
                __instance.ImpostorVentButton.SetDisabled();
            else
                __instance.ImpostorVentButton.SetEnabled();

            __instance.ImpostorVentButton.graphic.sprite = Vent;
            __instance.ImpostorVentButton.buttonLabelText.text = IsBlocked ? "BLOCKED" : "VENT";
            __instance.ImpostorVentButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;
            __instance.ImpostorVentButton.gameObject.SetActive(Player.CanVent() || Player.inVent);

            var closestDead = Player.GetClosestBody(CustomGameOptions.ReportDistance);

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
                Sabo = GetSprite("SyndicateSabotage");
            else if (Player.Is(Faction.Intruder))
                Sabo = GetSprite("Sabotage");

            __instance.SabotageButton.graphic.sprite = Sabo;
            __instance.SabotageButton.buttonLabelText.text = IsBlocked ? "BLOCKED" : "SABOTAGE";

            if (IsBlocked && Minigame.Instance)
                Minigame.Instance.Close();

            if (Map && IsBlocked)
                Map.Close();
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
            EndGame.Reset();
            Ash.DestroyAll();
        }

        public virtual void OnMeetingEnd(MeetingHud __instance)
        {
            Player.EnableButtons();
            EndGame.Reset();
            ButtonUtils.ResetCustomTimers(false, true);
        }

        public virtual void OnBodyReport(GameData.PlayerInfo info) => EndGame.Reset();

        public virtual void UponTaskComplete(uint taskId) {}

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
        public int TotalTasks => Player.Data.Tasks.Count;
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
                    CallRpc(CustomRPC.WinLose, WinLoseRPC.PhantomWin, this);
                    EndGame();
                }
                else if (LayerType == PlayerLayerEnum.Role && ((Role)this).RoleAlignment == RoleAlignment.NeutralEvil && CustomGameOptions.NeutralEvilsEndGame)
                {
                    if (Type == LayerEnum.Jester && ((Jester)this).VotedOut)
                    {
                        Role.JesterWins = true;
                        Winner = true;
                        EndGame();
                    }
                    else if (Type == LayerEnum.Executioner && ((Executioner)this).TargetVotedOut)
                    {
                        Role.ExecutionerWins = true;
                        Winner = true;
                        EndGame();
                    }
                    else if (Type == LayerEnum.Actor && ((Actor)this).Guessed)
                    {
                        Role.ActorWins = true;
                        Winner = true;
                        EndGame();
                    }
                    else if (Type == LayerEnum.Troll && ((Troll)this).Killed)
                    {
                        Role.TrollWins = true;
                        Winner = true;
                        EndGame();
                    }
                }

                return;
            }
            else if (LayerType == PlayerLayerEnum.Objectifier)
            {
                if (Type == LayerEnum.Corrupted && CorruptedWin(Player))
                {
                    Objectifier.CorruptedWins = true;

                    if (CustomGameOptions.AllCorruptedWin)
                        Objectifier.GetObjectifiers(ObjectifierEnum.Corrupted).ForEach(x => x.Winner = true);

                    Winner = true;
                    CallRpc(CustomRPC.WinLose, WinLoseRPC.CorruptedWin, this);
                    EndGame();
                }
                else if (Type == LayerEnum.Lovers && LoversWin(Player))
                {
                    Objectifier.LoveWins = true;
                    Winner = true;
                    Objectifier.GetObjectifier(((Lovers)this).OtherLover).Winner = true;
                    CallRpc(CustomRPC.WinLose, WinLoseRPC.LoveWin, this);
                    EndGame();
                }
                else if (Type == LayerEnum.Rivals && RivalsWin(Player))
                {
                    Objectifier.RivalWins = true;
                    Winner = true;
                    CallRpc(CustomRPC.WinLose, WinLoseRPC.RivalWin, this);
                    EndGame();
                }
                else if (Type == LayerEnum.Taskmaster && TasksDone)
                {
                    Objectifier.TaskmasterWins = true;
                    Winner = true;
                    CallRpc(CustomRPC.WinLose, WinLoseRPC.TaskmasterWin, this);
                    EndGame();
                }
                else if (Type == LayerEnum.Mafia && MafiaWin)
                {
                    Objectifier.MafiaWins = true;
                    Winner = true;
                    CallRpc(CustomRPC.WinLose, WinLoseRPC.MafiaWins);
                    EndGame();
                }
                else if (Type == LayerEnum.Overlord && MeetingPatches.MeetingCount >= CustomGameOptions.OverlordMeetingWinCount && !IsDead && !Disconnected)
                {
                    Objectifier.OverlordWins = true;
                    Objectifier.GetObjectifiers<Overlord>(ObjectifierEnum.Overlord).Where(ov => ov.IsAlive).ToList().ForEach(x => x.Winner = true);
                    CallRpc(CustomRPC.WinLose, WinLoseRPC.OverlordWin);
                    EndGame();
                }
            }
            else if (LayerType == PlayerLayerEnum.Role)
            {
                var role = (Role)this;

                if ((role.IsRecruit || Type == LayerEnum.Jackal) && CabalWin)
                {
                    Role.CabalWin = true;
                    CallRpc(CustomRPC.WinLose, WinLoseRPC.CabalWin, this);
                    EndGame();
                }
                else if ((role.IsPersuaded || Type == LayerEnum.Whisperer) && SectWin)
                {
                    Role.SectWin = true;
                    CallRpc(CustomRPC.WinLose, WinLoseRPC.SectWin);
                    EndGame();
                }
                else if ((role.IsBitten || Type == LayerEnum.Dracula) && UndeadWin)
                {
                    Role.UndeadWin = true;
                    CallRpc(CustomRPC.WinLose, WinLoseRPC.UndeadWin);
                    EndGame();
                }
                else if ((role.IsResurrected || Type == LayerEnum.Necromancer) && ReanimatedWin)
                {
                    Role.ReanimatedWin = true;
                    CallRpc(CustomRPC.WinLose, WinLoseRPC.ReanimatedWin);
                    EndGame();
                }
                else if (role.Faction == Faction.Syndicate && (role.Faithful || Type == LayerEnum.Betrayer || role.IsSynAlly || role.IsSynDefect || role.IsSynFanatic ||
                    role.IsSynTraitor) && SyndicateWins)
                {
                    Role.SyndicateWin = true;
                    CallRpc(CustomRPC.WinLose, WinLoseRPC.SyndicateWin);
                    EndGame();
                }
                else if (role.Faction == Faction.Intruder && (role.Faithful || Type == LayerEnum.Betrayer || role.IsIntDefect || role.IsIntAlly || role.IsIntFanatic ||
                    role.IsIntTraitor) && IntrudersWin)
                {
                    Role.IntruderWin = true;
                    CallRpc(CustomRPC.WinLose, WinLoseRPC.IntruderWin);
                    EndGame();
                }
                else if (role.Faction == Faction.Crew && (role.Faithful || role.IsCrewAlly || role.IsCrewDefect) && CrewWins)
                {
                    Role.CrewWin = true;
                    CallRpc(CustomRPC.WinLose, WinLoseRPC.CrewWin);
                    EndGame();
                }
                else if (role.Faithful && PestOrPBWins && Type is LayerEnum.Plaguebearer or LayerEnum.Pestilence)
                {
                    Role.InfectorsWin = true;
                    CallRpc(CustomRPC.WinLose, WinLoseRPC.InfectorsWin);
                    EndGame();
                }
                else if (AllNeutralsWin && role.Faithful)
                {
                    Role.AllNeutralsWin = true;
                    CallRpc(CustomRPC.WinLose, WinLoseRPC.AllNeutralsWin);
                    EndGame();
                }
                else if (AllNKsWin && role.Faithful && role.RoleAlignment == RoleAlignment.NeutralKill)
                {
                    Role.NKWins = true;
                    CallRpc(CustomRPC.WinLose, WinLoseRPC.AllNKsWin);
                    EndGame();
                }
                else if (role.Faithful && role.RoleAlignment == RoleAlignment.NeutralKill && (SameNKWins(role.RoleType) || SoloNKWins(Player)))
                {
                    switch (role.RoleType)
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
                        foreach (var role2 in Role.GetRoles(role.RoleType))
                        {
                            if (!role2.Disconnected && role2.Faithful)
                                role2.Winner = true;
                        }
                    }

                    Winner = true;
                    CallRpc(CustomRPC.WinLose, CustomGameOptions.NoSolo == NoSolo.SameNKs ? WinLoseRPC.SameNKWins : WinLoseRPC.SoloNKWins, this);
                    EndGame();
                }
                else if (CustomGameOptions.NeutralEvilsEndGame && role.RoleAlignment == RoleAlignment.NeutralEvil)
                {
                    if (Type == LayerEnum.Executioner && ((Executioner)role).TargetVotedOut)
                    {
                        Role.ExecutionerWins = true;
                        Winner = true;
                        EndGame();
                    }
                    else if (Type == LayerEnum.BountyHunter && ((BountyHunter)role).TargetKilled)
                    {
                        Role.BountyHunterWins = true;
                        Winner = true;
                        EndGame();
                    }
                    else if (Type == LayerEnum.Cannibal && ((Cannibal)role).Eaten)
                    {
                        Role.CannibalWins = true;
                        Winner = true;
                        EndGame();
                    }
                    else if (Type == LayerEnum.Guesser && ((Guesser)role).TargetGuessed)
                    {
                        Role.GuesserWins = true;
                        Winner = true;
                        EndGame();
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

            return a.Type == b.Type && a.Player == b.Player && a.LayerType == b.LayerType;
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

        public override int GetHashCode() => HashCode.Combine(Player, Type, LayerType);

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
            AllLayers.Clear();
        }

        public static List<PlayerLayer> GetLayers(PlayerControl player) => AllLayers.Where(x => x.Player == player).ToList();

        public static List<PlayerLayer> GetLayers(LayerEnum type, PlayerLayerEnum layer = PlayerLayerEnum.None) => AllLayers.Where(x => x.Type == type && (layer == PlayerLayerEnum.None ||
            x.LayerType == layer)).ToList();

        public static List<T> GetLayers<T>(LayerEnum type, PlayerLayerEnum layer = PlayerLayerEnum.None) where T : PlayerLayer => GetLayers(type, layer).Cast<T>().ToList();
    }
}