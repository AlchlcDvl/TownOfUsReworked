namespace TownOfUsReworked.Classes
{
    [HarmonyPatch]
    public static class Utils
    {
        public readonly static List<PlayerControl> RecentlyKilled = new();
        public readonly static Dictionary<PlayerControl, PlayerControl> CachedMorphs = new();
        public readonly static List<DeadPlayer> KilledPlayers = new();
        public static List<DeadBody> AllBodies => UObject.FindObjectsOfType<DeadBody>().ToList();
        private static bool Shapeshifted;
        public static PlayerControl FirstDead;
        public static bool RoundOne;

        public static TextMeshPro NameText(this PlayerControl p) => p.cosmetics.nameText;

        public static TextMeshPro ColorBlindText(this PlayerControl p) => p.cosmetics.colorBlindText;

        public static TextMeshPro NameText(this PoolablePlayer p) => p.cosmetics.nameText;

        public static SpriteRenderer MyRend(this PlayerControl p) => p.cosmetics.currentBodySprite.BodySprite;

        public static VisualAppearance GetAppearance(this PlayerControl player) => VisualAppearance.AllAppearances.Find(x => x.Player == player) ?? new(player);

        public static bool IsImpostor(this GameData.PlayerInfo playerinfo) => playerinfo?.Role?.TeamType == RoleTeamTypes.Impostor;

        public static bool IsImpostor(this PlayerVoteArea playerinfo) => PlayerByVoteArea(playerinfo).Data?.Role?.TeamType == RoleTeamTypes.Impostor;

        public static GameData.PlayerOutfit GetDefaultOutfit(this PlayerControl playerControl) => playerControl.Data.DefaultOutfit;

        public static void SetOutfit(this PlayerControl playerControl, CustomPlayerOutfitType CustomOutfitType, GameData.PlayerOutfit outfit)
        {
            playerControl.Data.SetOutfit((PlayerOutfitType)CustomOutfitType, outfit);
            playerControl.SetOutfit(CustomOutfitType);
        }

        public static void SetOutfit(this PlayerControl playerControl, CustomPlayerOutfitType CustomOutfitType)
        {
            if (playerControl == null)
                return;

            var outfitType = (PlayerOutfitType)CustomOutfitType;

            if (!playerControl.Data.Outfits.ContainsKey(outfitType))
                return;

            var newOutfit = playerControl.Data.Outfits[outfitType];
            playerControl.CurrentOutfitType = outfitType;
            playerControl.RawSetName(newOutfit.PlayerName);
            playerControl.RawSetColor(newOutfit.ColorId);
            playerControl.RawSetHat(newOutfit.HatId, newOutfit.ColorId);
            playerControl.RawSetVisor(newOutfit.VisorId, newOutfit.ColorId);
            playerControl.RawSetPet(newOutfit.PetId, newOutfit.ColorId);
            playerControl.RawSetSkin(newOutfit.SkinId, newOutfit.ColorId);
            playerControl.cosmetics.colorBlindText.color = Color.white;
        }

        public static CustomPlayerOutfitType GetCustomOutfitType(this PlayerControl playerControl) => (CustomPlayerOutfitType)playerControl.CurrentOutfitType;

        public static void Morph(PlayerControl player, PlayerControl MorphedPlayer)
        {
            if (DoUndo.IsCamoed)
                return;

            if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph)
                player.SetOutfit(CustomPlayerOutfitType.Morph, MorphedPlayer.Data.DefaultOutfit);
        }

        public static void DefaultOutfit(PlayerControl player) => Coroutines.Start(DefaultOutfitCoro(player));

        public static IEnumerator DefaultOutfitCoro(PlayerControl player)
        {
            if (player.GetCustomOutfitType() == CustomPlayerOutfitType.Invis)
            {
                HudManager.Instance.StartCoroutine(Effects.Lerp(1, new Action<float>(p =>
                {
                    var rend = player.MyRend();
                    rend.color = new(255, 255, 255, p);
                    var text = player.NameText();
                    text.color = new(text.color.a, text.color.a, text.color.a, p);
                    var cbtext = player.ColorBlindText();
                    cbtext.color = new(cbtext.color.a, cbtext.color.a, cbtext.color.a, p);
                })));

                yield return new WaitForSeconds(1f);
            }

            player.SetOutfit(CustomPlayerOutfitType.Default);
            CachedMorphs.Remove(player);
            yield return null;
        }

        public static void Camouflage() => PlayerControl.AllPlayerControls.ForEach(CamoSingle);

        public static void CamoSingle(PlayerControl player) => Coroutines.Start(CamoSingleCoro(player));

        public static IEnumerator CamoSingleCoro(PlayerControl player)
        {
            if (player.GetCustomOutfitType() is not CustomPlayerOutfitType.Camouflage and not CustomPlayerOutfitType.Invis and not CustomPlayerOutfitType.PlayerNameOnly &&
                !player.Data.IsDead && !PlayerControl.LocalPlayer.Data.IsDead && player != PlayerControl.LocalPlayer)
            {
                player.SetOutfit(CustomPlayerOutfitType.Camouflage, CamoOutfit(player));
                PlayerMaterial.SetColors(Color.grey, player.MyRend());

                HudManager.Instance.StartCoroutine(Effects.Lerp(1, new Action<float>(p =>
                {
                    var cbtext = player.ColorBlindText();
                    cbtext.color = new(cbtext.color.a, cbtext.color.a, cbtext.color.a, 1 - p);
                })));
            }

            yield return null;
        }

        public static void Conceal() => PlayerControl.AllPlayerControls.ForEach(x => Invis(x, PlayerControl.LocalPlayer.Is(Faction.Syndicate)));

        public static void Invis(PlayerControl player, bool condition = false) => Coroutines.Start(InvisCoro(player, condition));

        public static IEnumerator InvisCoro(PlayerControl player, bool condition = false)
        {
            var color = Color.clear;
            color.a = condition || PlayerControl.LocalPlayer.Data.IsDead || player == PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.Is(AbilityEnum.Torch) ? 0.1f : 0f;

            if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Invis && !player.Data.IsDead)
            {
                player.SetOutfit(CustomPlayerOutfitType.Invis, InvisOutfit(player));

                HudManager.Instance.StartCoroutine(Effects.Lerp(1, new Action<float>(p =>
                {
                    var rend = player.MyRend();
                    var a = Mathf.Clamp(1 - p, color.a, 1);
                    var r = rend.color.r * (1 - p);
                    var g = rend.color.g * (1 - p);
                    var b = rend.color.b * (1 - p);
                    rend.color = new(r, g, b, a);
                    var text = player.NameText();
                    text.color = new(text.color.a, text.color.a, text.color.a, 1 - p);
                    var cbtext = player.ColorBlindText();
                    cbtext.color = new(cbtext.color.a, cbtext.color.a, cbtext.color.a, 1 - p);
                })));
            }

            yield return null;
        }

        public static GameData.PlayerOutfit InvisOutfit(PlayerControl player) => new()
        {
            ColorId = player.CurrentOutfit.ColorId,
            HatId = "",
            SkinId = "",
            VisorId = "",
            PlayerName = " "
        };

        public static GameData.PlayerOutfit CamoOutfit(PlayerControl player) => new()
        {
            ColorId = player.GetDefaultOutfit().ColorId,
            HatId = "",
            SkinId = "",
            VisorId = "",
            PlayerName = " "
        };

        public static void Shapeshift()
        {
            if (!Shapeshifted)
            {
                Shapeshifted = true;
                var allPlayers = PlayerControl.AllPlayerControls;
                var shuffledPlayers = PlayerControl.AllPlayerControls.Il2CppToSystem();
                shuffledPlayers.Shuffle();

                for (var i = 0; i < allPlayers.Count; i++)
                {
                    var morphed = allPlayers[i];
                    var morphTarget = shuffledPlayers[i];
                    Morph(morphed, morphTarget);
                    CachedMorphs.Add(morphed, morphTarget);
                }
            }
            else
            {
                PlayerControl.AllPlayerControls.ForEach(x =>
                {
                    if (CachedMorphs.ContainsKey(x))
                        Morph(x, CachedMorphs[x]);
                });
            }
        }

        public static void DefaultOutfitAll() => PlayerControl.AllPlayerControls.ForEach(DefaultOutfit);

        public static void AddUnique<T>(this Il2CppSystem.Collections.Generic.List<T> self, T item) where T : IDisconnectHandler
        {
            if (!self.Contains(item))
                self.Add(item);
        }

        public static Color GetPlayerColor(this PlayerControl player, bool camoCondition = true, bool otherCondition = false)
        {
            if ((DoUndo.IsCamoed && camoCondition) || otherCondition)
                return Color.grey;
            else if (ColorUtils.IsRainbow(player.GetDefaultOutfit().ColorId))
                return ColorUtils.Rainbow;
            else if (ColorUtils.IsChroma(player.GetDefaultOutfit().ColorId))
                return ColorUtils.Chroma;
            else if (ColorUtils.IsMonochrome(player.GetDefaultOutfit().ColorId))
                return ColorUtils.Monochrome;
            else if (ColorUtils.IsMantle(player.GetDefaultOutfit().ColorId))
                return ColorUtils.Mantle;
            else if (ColorUtils.IsFire(player.GetDefaultOutfit().ColorId))
                return ColorUtils.Fire;
            else if (ColorUtils.IsGalaxy(player.GetDefaultOutfit().ColorId))
                return ColorUtils.Galaxy;
            else
                return Palette.PlayerColors[player.GetDefaultOutfit().ColorId];
        }

        public static List<PlayerControl> GetCrewmates(List<PlayerControl> impostors) => PlayerControl.AllPlayerControls.Where(player => !impostors.Any(imp => imp ==
            player)).ToList();

        public static List<PlayerControl> GetImpostors(List<GameData.PlayerInfo> infected)
        {
            var impostors = new List<PlayerControl>();

            foreach (var impData in infected)
                impostors.Add(impData.Object);

            return impostors;
        }

        public static PlayerControl PlayerById(byte id) => PlayerControl.AllPlayerControls.ToArray().ToList().Find(x => x.PlayerId == id);

        public static PlayerVoteArea VoteAreaById(byte id) => MeetingHud.Instance.playerStates.ToList().Find(x => x.TargetPlayerId == id);

        public static DeadBody BodyById(byte id) => AllBodies.Find(x => x.ParentId == id);

        public static DeadBody BodyByPlayer(PlayerControl player) => BodyById(player.PlayerId);

        public static PlayerControl PlayerByBody(DeadBody body) => PlayerById(body.ParentId);

        public static PlayerVoteArea VoteAreaByPlayer(PlayerControl player) => VoteAreaById(player.PlayerId);

        public static Vent VentById(int id) => UObject.FindObjectsOfType<Vent>().ToArray().ToList().Find(x => x.Id == id);

        public static PlayerControl PlayerByVoteArea(PlayerVoteArea state) => PlayerById(state.TargetPlayerId);

        public static Vector2 GetSize() => Vector2.Scale(UObject.FindObjectsOfType<Vent>()[0].GetComponent<BoxCollider2D>().size, UObject.FindObjectsOfType<Vent>()[0].transform.localScale)
            * 0.75f;

        public static double GetDistBetweenPlayers(PlayerControl player, PlayerControl refplayer)
        {
            if (player == null || refplayer == null)
                return double.MaxValue;

            var truePosition = refplayer.GetTruePosition();
            var truePosition2 = player.GetTruePosition();
            return Vector2.Distance(truePosition, truePosition2);
        }

        public static double GetDistBetweenPlayers(PlayerControl player, Vent refplayer)
        {
            if (player == null || refplayer == null)
                return double.MaxValue;

            var truePosition = refplayer.transform.position;
            var truePosition2 = player.GetTruePosition();
            return Vector2.Distance(truePosition, truePosition2);
        }

        public static double GetDistBetweenPlayers(PlayerControl player, DeadBody refplayer)
        {
            if (player == null || refplayer == null)
                return double.MaxValue;

            var truePosition = refplayer.TruePosition;
            var truePosition2 = player.GetTruePosition();
            return Vector2.Distance(truePosition, truePosition2);
        }

        public static void RpcMurderPlayer(PlayerControl killer, PlayerControl target, DeathReasonEnum reason = DeathReasonEnum.Killed, bool lunge = true)
        {
            if (killer == null || target == null)
                return;

            MurderPlayer(killer, target, reason, lunge);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.BypassKill);
            writer.Write(killer.PlayerId);
            writer.Write(target.PlayerId);
            writer.Write((byte)reason);
            writer.Write(lunge);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void MurderPlayer(PlayerControl killer, PlayerControl target, DeathReasonEnum reason, bool lunge)
        {
            if (killer == null || target == null)
                return;

            var data = target.Data;

            if (data == null)
                return;

            lunge = !killer.Is(AbilityEnum.Ninja) && lunge && killer != target;

            if (data.IsDead)
                return;

            if (killer == PlayerControl.LocalPlayer)
                SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false);

            if (FirstDead == null)
                FirstDead = target;

            target.gameObject.layer = LayerMask.NameToLayer("Ghost");
            target.Visible = false;

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Coroner) && !PlayerControl.LocalPlayer.Data.IsDead)
                Flash(Colors.Coroner);

            if (PlayerControl.LocalPlayer.Data.IsDead)
                Flash(Colors.Stalemate);

            var targetRole = Role.GetRole(target);

            if (target.Is(ModifierEnum.VIP))
            {
                Flash(targetRole.Color);

                if (!Role.LocalRole.AllArrows.ContainsKey(target.PlayerId))
                    Role.LocalRole.AllArrows.Add(target.PlayerId, new(PlayerControl.LocalPlayer, Colors.VIP));
                else
                    Role.LocalRole.AllArrows[target.PlayerId].Update(Colors.VIP);
            }

            var killerRole = Role.GetRole(killer);

            if (target.AmOwner)
            {
                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (MapBehaviour.Instance)
                    MapBehaviour.Instance.Close();

                HudManager.Instance.KillOverlay.ShowKillAnimation(killer.Data, data);
                HudManager.Instance.ShadowQuad.gameObject.SetActive(false);
                target.NameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                target.RpcSetScanner(false);
            }

            killer.MyPhysics.StartCoroutine(killer.KillAnimations.Random().CoPerformKill(lunge ? killer : target, target));

            if (killer != target)
            {
                targetRole.KilledBy = " By " + killerRole.PlayerName;
                targetRole.DeathReason = reason;
            }
            else
                targetRole.DeathReason = DeathReasonEnum.Suicide;

            if (target.Is(RoleEnum.Framer))
                ((Framer)killerRole).Framed.Clear();

            RecentlyKilled.Add(target);
            KilledPlayers.Add(new(killer.PlayerId, target.PlayerId));
            ReassignPostmortals(target);

            if (target == Role.DriveHolder)
                RoleGen.AssignChaosDrive();

            if (target.Is(ObjectifierEnum.Lovers) && AmongUsClient.Instance.AmHost)
            {
                var lover = target.GetOtherLover();

                if (!lover.Is(RoleEnum.Pestilence) && CustomGameOptions.BothLoversDie && !lover.Data.IsDead)
                    RpcMurderPlayer(lover, lover);
            }

            if (target.Is(ModifierEnum.Diseased))
                killerRole.Diseased = true;
            else if (target.Is(ModifierEnum.Bait))
                BaitReport(killer, target);

            if (killer.Is(AbilityEnum.Politician))
                Ability.GetAbility<Politician>(killer).VoteBank++;

            if (target.Is(RoleEnum.Troll) && AmongUsClient.Instance.AmHost)
            {
                var troll = Role.GetRole<Troll>(target);
                troll.Killed = true;
                RpcMurderPlayer(target, killer, DeathReasonEnum.Trolled, false);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.TrollWin);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            target.RegenTask();
            killer.RegenTask();
        }

        public static void MarkMeetingDead(PlayerControl target, PlayerControl killer, bool doesKill = true)
        {
            try
            {
                SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false);
            } catch {}

            if (target == PlayerControl.LocalPlayer)
            {
                HudManager.Instance.KillOverlay.ShowKillAnimation(killer.Data, target.Data);
                HudManager.Instance.ShadowQuad.gameObject.SetActive(false);
                target.NameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                target.RpcSetScanner(false);
                MeetingHud.Instance.SetForegroundForDead();

                if (target.Is(AbilityEnum.Swapper))
                {
                    var swapper = Ability.GetAbility<Swapper>(PlayerControl.LocalPlayer);
                    swapper.Swap1 = null;
                    swapper.Swap2 = null;
                    swapper.HideButtons();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.SetSwaps);
                    writer.Write(target.PlayerId);
                    writer.Write(255);
                    writer.Write(255);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                if (target.Is(RoleEnum.Guesser))
                    Role.GetRole<Guesser>(target).HideButtons();

                if (target.Is(RoleEnum.Dictator))
                {
                    var dict = Role.GetRole<Dictator>(target);
                    dict.HideButtons();
                    dict.ToBeEjected.Clear();
                    dict.ToDie = false;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.SetExiles);
                    writer.Write(target.PlayerId);
                    writer.Write(false);
                    writer.WriteBytesAndSize(new List<byte>() { 255, 255, 255 }.ToArray());
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                if (target.Is(RoleEnum.Retributionist))
                    Role.GetRole<Retributionist>(target).HideButtons();

                if (target.Is(AbilityEnum.Assassin))
                {
                    Ability.GetAbility<Assassin>(target).Exit(MeetingHud.Instance);
                    Ability.GetAbility<Assassin>(target).HideButtons();
                }
            }

            target.Die(DeathReason.Kill, false);
            KilledPlayers.Add(new(killer.PlayerId, target.PlayerId));
            var voteArea = VoteAreaByPlayer(target);

            if (voteArea == null)
                return;

            if (voteArea.DidVote)
                voteArea.UnsetVote();

            voteArea.AmDead = true;
            voteArea.Overlay.gameObject.SetActive(true);
            voteArea.Overlay.color = UColor.white;
            voteArea.XMark.gameObject.SetActive(true);
            voteArea.XMark.transform.localScale = Vector3.one;

            foreach (var role in Role.GetRoles<Blackmailer>(RoleEnum.Blackmailer))
            {
                if (target.PlayerId == role.BlackmailedPlayer?.PlayerId && role.PrevOverlay != null)
                {
                    voteArea.Overlay.sprite = role.PrevOverlay;
                    voteArea.Overlay.color = role.PrevColor;
                }
            }

            foreach (var role in Role.GetRoles<Silencer>(RoleEnum.Silencer))
            {
                if (target.PlayerId == role.SilencedPlayer?.PlayerId && role.PrevOverlay != null)
                {
                    voteArea.Overlay.sprite = role.PrevOverlay;
                    voteArea.Overlay.color = role.PrevColor;
                }
            }

            foreach (var role in Role.GetRoles<PromotedGodfather>(RoleEnum.PromotedGodfather))
            {
                if (target.PlayerId == role.BlackmailedPlayer?.PlayerId && role.PrevOverlay != null && role.IsBM)
                {
                    voteArea.Overlay.sprite = role.PrevOverlay;
                    voteArea.Overlay.color = role.PrevColor;
                }
            }

            foreach (var role in Role.GetRoles<PromotedRebel>(RoleEnum.PromotedRebel))
            {
                if (target.PlayerId == role.SilencedPlayer?.PlayerId && role.PrevOverlay != null && role.IsSil)
                {
                    voteArea.Overlay.sprite = role.PrevOverlay;
                    voteArea.Overlay.color = role.PrevColor;
                }
            }

            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Assassin) && !PlayerControl.LocalPlayer.Data.IsDead)
                Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer).HideSingle(target.PlayerId);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Guesser) && !PlayerControl.LocalPlayer.Data.IsDead)
                Role.GetRole<Guesser>(PlayerControl.LocalPlayer).HideSingle(target.PlayerId);

            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Swapper) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var swapper = Ability.GetAbility<Swapper>(PlayerControl.LocalPlayer);
                var active = swapper.Actives[target.PlayerId];

                if (active)
                {
                    if (swapper.Swap1 == voteArea)
                        swapper.Swap1 = null;
                    else if (swapper.Swap2 == voteArea)
                        swapper.Swap2 = null;

                    swapper.Actives[target.PlayerId] = false;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.SetSwaps);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(255);
                    writer.Write(255);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                swapper.HideSingle(target.PlayerId);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Dictator) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var dictator = Role.GetRole<Dictator>(PlayerControl.LocalPlayer);
                var active = dictator.Actives[target.PlayerId];

                if (active)
                {
                    dictator.ToBeEjected.Clear();

                    for (var i = 0; i < dictator.Actives.Count; i++)
                        dictator.Actives[(byte)i] = false;

                    dictator.Actives[target.PlayerId] = false;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.SetExiles);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(false);
                    writer.WriteBytesAndSize(new List<byte>() { 255, 255, 255 }.ToArray());
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                dictator.HideSingle(target.PlayerId);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Retributionist) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var ret = Role.GetRole<Retributionist>(PlayerControl.LocalPlayer);
                ret.MoarButtons.Remove(target.PlayerId);
                ret.GenButtons(voteArea, MeetingHud.Instance);
            }

            foreach (var area in MeetingHud.Instance.playerStates)
            {
                if (area.VotedFor != target.PlayerId)
                    continue;

                area.UnsetVote();

                if (target == PlayerControl.LocalPlayer)
                    MeetingHud.Instance.ClearVote();
            }

            if (AmongUsClient.Instance.AmHost)
            {
                foreach (var mayor in Ability.GetAbilities<Politician>(AbilityEnum.Politician))
                {
                    if (mayor.Player == target)
                        mayor.ExtraVotes.Clear();
                    else
                    {
                        var votesRegained = mayor.ExtraVotes.RemoveAll(x => x == target.PlayerId);

                        if (mayor.Player == PlayerControl.LocalPlayer)
                            mayor.VoteBank += votesRegained;

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AddVoteBank, SendOption.Reliable);
                        writer.Write(mayor.PlayerId);
                        writer.Write(votesRegained);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }

                if (SetPostmortals.RevealerOn && SetPostmortals.WillBeRevealer == null && target.Is(Faction.Crew))
                {
                    SetPostmortals.WillBeRevealer = target;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetRevealer, SendOption.Reliable);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                if (SetPostmortals.PhantomOn && SetPostmortals.WillBePhantom == null && target.Is(Faction.Neutral) && !LayerExtentions.NeutralHasUnfinishedBusiness(target))
                {
                    SetPostmortals.WillBePhantom = target;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetPhantom, SendOption.Reliable);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                if (SetPostmortals.BansheeOn && SetPostmortals.WillBeBanshee == null && target.Is(Faction.Syndicate))
                {
                    SetPostmortals.WillBeBanshee = target;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetBanshee, SendOption.Reliable);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                if (SetPostmortals.GhoulOn && SetPostmortals.WillBeGhoul == null && target.Is(Faction.Intruder))
                {
                    SetPostmortals.WillBeGhoul = target;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGhoul, SendOption.Reliable);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                SetPostmortals.AssassinatedPlayers.Add(target);
                MeetingHud.Instance.CheckForEndVoting();
            }

            var role2 = Role.GetRole(target);

            if ((killer != target && doesKill) || !doesKill)
            {
                role2.DeathReason = DeathReasonEnum.Guessed;
                role2.KilledBy = " By " + killer.name;
            }
            else
                role2.DeathReason = DeathReasonEnum.Misfire;
        }

        public static void BaitReport(PlayerControl killer, PlayerControl target) => Coroutines.Start(BaitReportDelay(killer, target));

        public static IEnumerator BaitReportDelay(PlayerControl killer, PlayerControl target)
        {
            if (killer == null || target == null || killer == target)
                yield break;

            var extraDelay = URandom.RandomRangeInt(0, (int)((100 * (CustomGameOptions.BaitMaxDelay - CustomGameOptions.BaitMinDelay)) + 1));

            if (CustomGameOptions.BaitMaxDelay <= CustomGameOptions.BaitMinDelay)
                yield return new WaitForSeconds(CustomGameOptions.BaitMaxDelay + 0.01f);
            else
                yield return new WaitForSeconds(CustomGameOptions.BaitMinDelay + 0.01f + (extraDelay / 100f));

            var body = BodyById(target.PlayerId);

            if (body != null)
            {
                if (AmongUsClient.Instance.AmHost)
                    killer.ReportDeadBody(target.Data);
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.BaitReport);
                    writer.Write(killer.PlayerId);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }

        public static void EndGame()
        {
            Ash.DestroyAll();
            Objects.Range.DestroyAll();
            GameManager.Instance.RpcEndGame(GameOverReason.ImpostorByVote, false);
        }

        public static object TryCast(this Il2CppObjectBase self, Type type) => AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self,
            Array.Empty<object>());

        public static List<bool> Interact(PlayerControl player, PlayerControl target, bool toKill = false, bool toConvert = false, bool bypass = false)
        {
            var fullReset = false;
            var gaReset = false;
            var survReset = false;
            var abilityUsed = false;
            bypass = bypass || player.Is(AbilityEnum.Ruthless);
            Spread(player, target);

            if (target.IsOnAlert() || ((target.IsAmbushed() || target.IsGFAmbushed()) && (!player.Is(Faction.Intruder) || (player.Is(Faction.Intruder) &&
                CustomGameOptions.AmbushMates))) || target.Is(RoleEnum.Pestilence) || (target.Is(RoleEnum.VampireHunter) && player.Is(SubFaction.Undead)) ||
                (target.Is(RoleEnum.SerialKiller) && (player.Is(RoleEnum.Escort) || player.Is(RoleEnum.Consort) || (player.Is(RoleEnum.Glitch) && !toKill)) && !bypass))
            {
                if (player.Is(RoleEnum.Pestilence))
                {
                    if (target.IsShielded() && (toKill || toConvert))
                    {
                        var medic = target.GetMedic().PlayerId;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                        writer.Write(medic);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        fullReset = CustomGameOptions.ShieldBreaks;
                        Medic.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    else if (target.IsRetShielded() && (toKill || toConvert))
                    {
                        var medic = target.GetRetMedic().PlayerId;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                        writer.Write(medic);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        fullReset = CustomGameOptions.ShieldBreaks;
                        Retributionist.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    else if (target.IsProtected())
                        gaReset = true;
                }
                else if (player.IsShielded() && !target.Is(AbilityEnum.Ruthless))
                {
                    var medic = player.GetMedic().PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                    writer.Write(medic);
                    writer.Write(player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Medic.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (player.IsRetShielded() && !target.Is(AbilityEnum.Ruthless))
                {
                    var medic = target.GetRetMedic().PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                    writer.Write(medic);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Retributionist.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (player.IsProtected() && !target.Is(AbilityEnum.Ruthless))
                    gaReset = true;
                else
                    RpcMurderPlayer(target, player, target.IsAmbushed() ? DeathReasonEnum.Ambushed : DeathReasonEnum.Killed);

                if (target.IsShielded() && (toKill || toConvert))
                {
                    var medic = target.GetMedic().PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                    writer.Write(medic);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Medic.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (target.IsRetShielded() && (toKill || toConvert))
                {
                    var medic = target.GetRetMedic().PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                    writer.Write(medic);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Retributionist.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                }
            }
            else if ((target.IsCrusaded() || target.IsRebCrusaded()) && (!player.Is(Faction.Syndicate) || (player.Is(Faction.Syndicate) && CustomGameOptions.CrusadeMates)) && !bypass)
            {
                if (player.Is(RoleEnum.Pestilence))
                {
                    if (target.IsShielded() && (toKill || toConvert))
                    {
                        var medic = target.GetMedic().PlayerId;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                        writer.Write(medic);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        fullReset = CustomGameOptions.ShieldBreaks;
                        Medic.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    else if (target.IsRetShielded() && (toKill || toConvert))
                    {
                        var medic = target.GetRetMedic().PlayerId;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                        writer.Write(medic);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        fullReset = CustomGameOptions.ShieldBreaks;
                        Retributionist.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    else if (target.IsProtected())
                        gaReset = true;
                }
                else if (player.IsShielded() && !target.Is(AbilityEnum.Ruthless))
                {
                    var medic = player.GetMedic().PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                    writer.Write(medic);
                    writer.Write(player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Medic.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (player.IsRetShielded() && !target.Is(AbilityEnum.Ruthless))
                {
                    var medic = player.GetRetMedic().PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                    writer.Write(medic);
                    writer.Write(player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Retributionist.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (player.IsProtected() && !target.Is(AbilityEnum.Ruthless))
                    gaReset = true;
                else
                {
                    var crus = target.GetCrusader();
                    var reb = target.GetRebCrus();

                    if (crus?.HoldsDrive == true || reb?.HoldsDrive == true)
                        Crusader.RadialCrusade(target);
                    else
                        RpcMurderPlayer(target, player, DeathReasonEnum.Crusaded);
                }

                if (target.IsShielded() && (toKill || toConvert))
                {
                    var medic = target.GetMedic().PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                    writer.Write(medic);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Medic.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (target.IsRetShielded() && (toKill || toConvert))
                {
                    var medic = target.GetRetMedic().PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                    writer.Write(medic);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Retributionist.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                }
            }
            else if (target.IsShielded() && (toKill || toConvert) && !bypass)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                writer.Write(target.GetMedic().PlayerId);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                fullReset = CustomGameOptions.ShieldBreaks;
                Medic.BreakShield(target.GetMedic().PlayerId, target.PlayerId, CustomGameOptions.ShieldBreaks);
            }
            else if (target.IsRetShielded() && (toKill || toConvert) && !bypass)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                writer.Write(target.GetRetMedic().PlayerId);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                fullReset = CustomGameOptions.ShieldBreaks;
                Retributionist.BreakShield(target.GetMedic().PlayerId, target.PlayerId, CustomGameOptions.ShieldBreaks);
            }
            else if (target.IsVesting() && (toKill || toConvert) && !bypass)
                survReset = true;
            else if (target.IsProtected() && (toKill || toConvert) && !bypass)
                gaReset = true;
            else if (target.IsProtectedMonarch() && (toKill || toConvert) && !bypass)
                gaReset = true;
            else if (player.IsOtherRival(target) && (toKill || toConvert))
                fullReset = true;
            else
            {
                if (toKill)
                {
                    if (target.Is(ObjectifierEnum.Fanatic) && (player.Is(Faction.Intruder) || player.Is(Faction.Syndicate)) && target.IsUnturnedFanatic() && !bypass)
                    {
                        var role = Role.GetRole(player);
                        Objectifier.GetObjectifier<Fanatic>(target).TurnFanatic(role.Faction);
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                        writer.Write((byte)TurnRPC.TurnFanatic);
                        writer.Write(target.PlayerId);
                        writer.Write((byte)role.Faction);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                    else
                        RpcMurderPlayer(player, target);
                }

                abilityUsed = true;
                fullReset = true;
            }

            return new() { fullReset, gaReset, survReset, abilityUsed };
        }

        public static List<PlayerControl> GetClosestPlayers(Vector2 truePosition, float radius) => PlayerControl.AllPlayerControls.Where(x => Vector2.Distance(truePosition,
            x.GetTruePosition()) <= radius).ToList();

        public static bool IsTooFar(PlayerControl player, PlayerControl target)
        {
            if (player == null || target == null)
                return true;

            var maxDistance = CustomGameOptions.InteractionDistance;
            return GetDistBetweenPlayers(player, target) > maxDistance;
        }

        public static bool IsTooFar(PlayerControl player, DeadBody target)
        {
            if (player == null || target == null)
                return true;

            var maxDistance = CustomGameOptions.InteractionDistance;
            return GetDistBetweenPlayers(player, target) > maxDistance;
        }

        public static bool IsTooFar(PlayerControl player, Vent target)
        {
            if (player == null || target == null)
                return true;

            var maxDistance = CustomGameOptions.InteractionDistance;
            return GetDistBetweenPlayers(player, target) > maxDistance;
        }

        public static bool NoButton(PlayerControl target, RoleEnum role) => PlayerControl.AllPlayerControls.Count <= 1 || target == null || target.Data == null || !target.CanMove ||
            !target.Is(role) || !ConstantVariables.IsRoaming || MeetingHud.Instance || target != PlayerControl.LocalPlayer;

        public static bool NoButton(PlayerControl target, ModifierEnum mod) => PlayerControl.AllPlayerControls.Count <= 1 || target == null || target.Data == null || !target.CanMove ||
            !target.Is(mod) || !ConstantVariables.IsRoaming || MeetingHud.Instance || target != PlayerControl.LocalPlayer;

        public static bool NoButton(PlayerControl target, Faction faction) => PlayerControl.AllPlayerControls.Count <= 1 || target == null || target.Data == null || !target.CanMove ||
            !target.Is(faction) || !ConstantVariables.IsRoaming || MeetingHud.Instance || target != PlayerControl.LocalPlayer;

        public static bool NoButton(PlayerControl target, ObjectifierEnum obj) => PlayerControl.AllPlayerControls.Count <= 1 || target == null || target.Data == null || !target.CanMove ||
            !target.Is(obj) || !ConstantVariables.IsRoaming || MeetingHud.Instance || target != PlayerControl.LocalPlayer;

        public static bool NoButton(PlayerControl target, AbilityEnum ability) => PlayerControl.AllPlayerControls.Count <= 1 || target == null || target.Data == null || !target.CanMove ||
            !target.Is(ability) || !ConstantVariables.IsRoaming || MeetingHud.Instance || target != PlayerControl.LocalPlayer;

        public static void Spread(PlayerControl interacter, PlayerControl target)
        {
            foreach (var pb in Role.GetRoles<Plaguebearer>(RoleEnum.Plaguebearer))
                pb.RpcSpreadInfection(interacter, target);

            foreach (var arso in Role.GetRoles<Arsonist>(RoleEnum.Arsonist))
                arso.RpcSpreadDouse(target, interacter);

            foreach (var cryo in Role.GetRoles<Cryomaniac>(RoleEnum.Cryomaniac))
                cryo.RpcSpreadDouse(target, interacter);
        }

        public static bool Check(int probability)
        {
            if (probability == 0)
                return false;

            if (probability == 100)
                return true;

            var num = URandom.RandomRangeInt(1, 100);
            return num <= probability;
        }

        public static void StopDragging(byte id)
        {
            foreach (var janitor in Role.GetRoles<Janitor>(RoleEnum.Janitor).Where(x => x.CurrentlyDragging != null && x.CurrentlyDragging.ParentId == id))
                janitor.Drop();

            foreach (var godfather in Role.GetRoles<PromotedGodfather>(RoleEnum.PromotedGodfather).Where(x => x.CurrentlyDragging != null && x.CurrentlyDragging.ParentId == id))
                godfather.Drop();
        }

        public static void LogSomething(object message) => PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage(message);

        public static string CreateText(string itemName, string folder = "")
        {
            try
            {
                string resourceName;

                if (folder != "")
                    resourceName = $"{TownOfUsReworked.Resources}{folder}.{itemName}";
                else
                    resourceName = TownOfUsReworked.Resources + itemName;

                var stream = TownOfUsReworked.Executing.GetManifestResourceStream(resourceName);
                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            catch
            {
                LogSomething($"Error Loading {itemName}");
                return "";
            }
        }

        public static bool IsInRange(this float num, float min, float max, bool minInclusive = false, bool maxInclusive = false)
        {
            if (minInclusive && maxInclusive)
                return num >= min && num <= max;
            else if (minInclusive)
                return num >= min && num < max;
            else if (maxInclusive)
                return num > min && num <= max;
            else
                return num > min && num < max;
        }

        public static void ShareGameVersion()
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VersionHandshake, Hazel.SendOption.Reliable);
            writer.Write((byte)TownOfUsReworked.Version.Major);
            writer.Write((byte)TownOfUsReworked.Version.Minor);
            writer.Write((byte)TownOfUsReworked.Version.Build);
            writer.Write(TownOfUsReworked.Version.Revision);
            writer.Write(TownOfUsReworked.Executing.ManifestModule.ModuleVersionId.ToByteArray());
            writer.WritePacked(AmongUsClient.Instance.ClientId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            VersionHandshake(TownOfUsReworked.Version.Major, TownOfUsReworked.Version.Minor, TownOfUsReworked.Version.Build, TownOfUsReworked.Version.Revision,
                TownOfUsReworked.Executing.ManifestModule.ModuleVersionId, AmongUsClient.Instance.ClientId);
        }

        public static void VersionHandshake(int major, int minor, int build, int revision, Guid guid, int clientId)
        {
            var ver = new Version(major, minor, build, revision);

            if (!GameStartManagerPatch.PlayerVersions.ContainsKey(clientId))
                GameStartManagerPatch.PlayerVersions.Add(clientId, new(ver, guid));
        }

        public static string GetRandomisedName()
        {
            const string everything = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()|{}[],.<>;':\"-+=*/`~_\\ ⟡☆♡♧♤ø▶❥✔εΔΓικνστυφψΨωχӪζδ♠♥βαµ♣✚Ξρλς§π" +
                "★ηΛγΣΦΘξ✧¢";
            var length = URandom.RandomRangeInt(1, 11);
            var position = 0;
            var name = "";

            while (position < length)
            {
                var random = URandom.RandomRangeInt(0, everything.Length);
                name += everything[random];
                position++;
            }

            return name;
        }

        public static IEnumerator FadeBody(DeadBody body)
        {
            if (body == null)
                yield break;

            foreach (var renderer in body.bodyRenderers)
            {
                var backColor = renderer.material.GetColor(Shader.PropertyToID("_BackColor"));
                var bodyColor = renderer.material.GetColor(Shader.PropertyToID("_BodyColor"));
                var newColor = new Color(1f, 1f, 1f, 0f);

                for (var i = 0; i < 60; i++)
                {
                    renderer.color = Color.Lerp(backColor, newColor, i / 60f);
                    renderer.color = Color.Lerp(bodyColor, newColor, i / 60f);
                    yield return null;
                }
            }

            body.gameObject.Destroy();
            Role.Cleaned.Add(PlayerById(body.ParentId));
        }

        public static void SpawnVent(int ventId, Role role, Vector2 position, float zAxis)
        {
            var ventPrefab = UObject.FindObjectOfType<Vent>();
            var vent = UObject.Instantiate(ventPrefab, ventPrefab.transform.parent);

            vent.Id = ventId;
            vent.transform.position = new(position.x, position.y, zAxis);

            if (role.RoleType is not RoleEnum.Godfather and not RoleEnum.Miner)
                return;

            if (role.Vents.Count > 0)
            {
                var leftVent = role.Vents[^1];
                vent.Left = leftVent;
                leftVent.Right = vent;
            }
            else
                vent.Left = null;

            vent.Right = null;
            vent.Center = null;

            var allVents = ShipStatus.Instance.AllVents.ToList();
            allVents.Add(vent);
            ShipStatus.Instance.AllVents = allVents.ToArray();

            role.Vents.Add(vent);

            if (ModCompatibility.IsSubmerged)
            {
                vent.gameObject.layer = 12;
                vent.gameObject.AddSubmergedComponent(ModCompatibility.ElevatorMover); //Just in case elevator vent is not blocked

                if (vent.gameObject.transform.position.y > -7)
                    vent.gameObject.transform.position = new(vent.gameObject.transform.position.x, vent.gameObject.transform.position.y, 0.03f);
                else
                {
                    vent.gameObject.transform.position = new(vent.gameObject.transform.position.x, vent.gameObject.transform.position.y, 0.0009f);
                    vent.gameObject.transform.localPosition = new(vent.gameObject.transform.localPosition.x, vent.gameObject.transform.localPosition.y, -0.003f);
                }
            }
        }

        public static int GetAvailableId()
        {
            var id = 0;

            while (true)
            {
                if (ShipStatus.Instance.AllVents.All(v => v.Id != id))
                    return id;

                id++;
            }
        }

        public static void Flash(Color32 color, float duration = 2f, string message = "", float size = 100f) => Coroutines.Start(FlashCoroutine(color, duration, message, size));

        public static IEnumerator FlashCoroutine(Color color, float duration, string message, float size)
        {
            color.a = 0.3f;

            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = HudManager.Instance.FullScreen;
                fullscreen.enabled = true;
                fullscreen.gameObject.active = true;
                fullscreen.color = color;
            }

            // Message Text
            var messageText = UObject.Instantiate(HudManager.Instance.KillButton.cooldownTimerText, HudManager.Instance.transform);
            messageText.text = $"<size={size}%>{message}</size>";
            messageText.enableWordWrapping = false;
            messageText.transform.localScale = Vector3.one * 0.5f;
            messageText.transform.localPosition = new(0, 0, 0);
            messageText.gameObject.SetActive(true);

            yield return new WaitForSeconds(duration);

            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = HudManager.Instance.FullScreen;

                if (fullscreen.color.Equals(color))
                    fullscreen.color = new(1f, 0f, 0f, 0.37254903f);

                var fs = false;

                switch (TownOfUsReworked.VanillaOptions.MapId)
                {
                    case 0:
                    case 1:
                    case 3:
                        var reactor1 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                        var oxygen1 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                        fs = reactor1.IsActive || oxygen1.IsActive;
                        break;

                    case 2:
                        var seismic = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                        fs = seismic.IsActive;
                        break;

                    case 4:
                        var reactor = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<HeliSabotageSystem>();
                        fs = reactor.IsActive;
                        break;

                    case 5:
                        fs = PlayerControl.LocalPlayer.myTasks.Any(x => x.TaskType == ModCompatibility.RetrieveOxygenMask);
                        break;

                    case 6:
                        var reactor3 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                        var oxygen3 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                        var seismic2 = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                        fs = reactor3.IsActive || seismic2.IsActive || oxygen3.IsActive;
                        break;
                }

                fullscreen.enabled = fs;
                fullscreen.gameObject.active = fs;
                messageText.gameObject.SetActive(false);
                messageText.gameObject.Destroy();
            }
        }

        public static void Warp()
        {
            var coordinates = GenerateWarpCoordinates();
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.WarpAll);
            writer.Write((byte)coordinates.Count);

            foreach (var (key, value) in coordinates)
            {
                writer.Write(key);
                writer.Write(value);
            }

            AmongUsClient.Instance.FinishRpcImmediately(writer);
            WarpPlayersToCoordinates(coordinates);
        }

        public static void WarpPlayersToCoordinates(Dictionary<byte, Vector2> coordinates)
        {
            if (coordinates.ContainsKey(PlayerControl.LocalPlayer.PlayerId))
            {
                Flash(Colors.Warper);

                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (MapBehaviour.Instance)
                    MapBehaviour.Instance.Close();

                if (PlayerControl.LocalPlayer.inVent)
                {
                    PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
                    PlayerControl.LocalPlayer.MyPhysics.ExitAllVents();
                }
            }

            foreach (var (key, value) in coordinates)
            {
                var player = PlayerById(key);
                player.transform.position = value;
                LogSomething($"Warping {player.Data.PlayerName} to ({value.x}, {value.y})");
            }

            foreach (var janitor in Role.GetRoles<Janitor>(RoleEnum.Janitor).Where(x => x.CurrentlyDragging != null))
                janitor.Drop();

            foreach (var godfather in Role.GetRoles<PromotedGodfather>(RoleEnum.PromotedGodfather).Where(x => x.CurrentlyDragging != null))
                godfather.Drop();
        }

        public static Dictionary<byte, Vector2> GenerateWarpCoordinates()
        {
            var targets = PlayerControl.AllPlayerControls.Where(player => !player.Data.IsDead && !player.Data.Disconnected).ToList();
            var vents = UObject.FindObjectsOfType<Vent>();
            var coordinates = new Dictionary<byte, Vector2>(targets.Count);

            var SkeldPositions = new List<Vector3>()
            {
                new(-2.2f, 2.2f, 0f), //cafeteria. botton. top left.
                new(0.7f, 2.2f, 0f), //caffeteria. button. top right.
                new(-2.2f, -0.2f, 0f), //caffeteria. button. bottom left.
                new(0.7f, -0.2f, 0f), //caffeteria. button. bottom right.
                new(10.0f, 3.0f, 0f), //weapons top
                new(9.0f, 1.0f, 0f), //weapons bottom
                new(6.5f, -3.5f, 0f), //O2
                new(11.5f, -3.5f, 0f), //O2-nav hall
                new(17.0f, -3.5f, 0f), //navigation top
                new(18.2f, -5.7f, 0f), //navigation bottom
                new(11.5f, -6.5f, 0f), //nav-shields top
                new(9.5f, -8.5f, 0f), //nav-shields bottom
                new(9.2f, -12.2f, 0f), //shields top
                new(8.0f, -14.3f, 0f), //shields bottom
                new(2.5f, -16f, 0f), //coms left
                new(4.2f, -16.4f, 0f), //coms middle
                new(5.5f, -16f, 0f), //coms right
                new(-1.5f, -10.0f, 0f), //storage top
                new(-1.5f, -15.5f, 0f), //storage bottom
                new(-4.5f, -12.5f, 0f), //storrage left
                new(0.3f, -12.5f, 0f), //storrage right
                new(4.5f, -7.5f, 0f), //admin top
                new(4.5f, -9.5f, 0f), //admin bottom
                new(-9.0f, -8.0f, 0f), //elec top left
                new(-6.0f, -8.0f, 0f), //elec top right
                new(-8.0f, -11.0f, 0f), //elec bottom
                new(-12.0f, -13.0f, 0f), //elec-lower hall
                new(-17f, -10f, 0f), //lower engine top
                new(-17.0f, -13.0f, 0f), //lower engine bottom
                new(-21.5f, -3.0f, 0f), //reactor top
                new(-21.5f, -8.0f, 0f), //reactor bottom
                new(-13.0f, -3.0f, 0f), //security top
                new(-12.6f, -5.6f, 0f), // security bottom
                new(-17.0f, 2.5f, 0f), //upper engibe top
                new(-17.0f, -1.0f, 0f), //upper engine bottom
                new(-10.5f, 1.0f, 0f), //upper-mad hall
                new(-10.5f, -2.0f, 0f), //medbay top
                new(-6.5f, -4.5f, 0f) //medbay bottom
            };

            var MiraPositions = new List<Vector3>()
            {
                new(-4.5f, 3.5f, 0f), //launchpad top
                new(-4.5f, -1.4f, 0f), //launchpad bottom
                new(8.5f, -1f, 0f), //launchpad- med hall
                new(14f, -1.5f, 0f), //medbay
                new(16.5f, 3f, 0f), // comms
                new(10f, 5f, 0f), //lockers
                new(6f, 1.5f, 0f), //locker room
                new(2.5f, 13.6f, 0f), //reactor
                new(6f, 12f, 0f), //reactor middle
                new(9.5f, 13f, 0f), //lab
                new(15f, 9f, 0f), //bottom left cross
                new(17.9f, 11.5f, 0f), //middle cross
                new(14f, 17.3f, 0f), //office
                new(19.5f, 21f, 0f), //admin
                new(14f, 24f, 0f), //greenhouse left
                new(22f, 24f, 0f), //greenhouse right
                new(21f, 8.5f, 0f), //bottom right cross
                new(28f, 3f, 0f), //caf right
                new(22f, 3f, 0f), //caf left
                new(19f, 4f, 0f), //storage
                new(22f, -2f, 0f), //balcony
            };

            var PolusPositions = new List<Vector3>()
            {
                new(16.6f, -1f, 0f), //dropship top
                new(16.6f, -5f, 0f), //dropship bottom
                new(20f, -9f, 0f), //above storrage
                new(22f, -7f, 0f), //right fuel
                new(25.5f, -6.9f, 0f), //drill
                new(29f, -9.5f, 0f), //lab lockers
                new(29.5f, -8f, 0f), //lab weather notes
                new(35f, -7.6f, 0f), //lab table
                new(40.4f, -8f, 0f), //lab scan
                new(33f, -10f, 0f), //lab toilet
                new(39f, -15f, 0f), //specimen hall top
                new(36.5f, -19.5f, 0f), //specimen top
                new(36.5f, -21f, 0f), //specimen bottom
                new(28f, -21f, 0f), //specimen hall bottom
                new(24f, -20.5f, 0f), //admin tv
                new(22f, -25f, 0f), //admin books
                new(16.6f, -17.5f, 0f), //office coffe
                new(22.5f, -16.5f, 0f), //office projector
                new(24f, -17f, 0f), //office figure
                new(27f, -16.5f, 0f), //office lifelines
                new(32.7f, -15.7f, 0f), //lavapool
                new(31.5f, -12f, 0f), //snowmad below lab
                new(10f, -14f, 0f), //below storrage
                new(21.5f, -12.5f, 0f), //storrage vent
                new(19f, -11f, 0f), //storrage toolrack
                new(12f, -7f, 0f), //left fuel
                new(5f, -7.5f, 0f), //above elec
                new(10f, -12f, 0f), //elec fence
                new(9f, -9f, 0f), //elec lockers
                new(5f, -9f, 0f), //elec window
                new(4f, -11.2f, 0f), //elec tapes
                new(5.5f, -16f, 0f), //elec-O2 hall
                new(1f, -17.5f, 0f), //O2 tree hayball
                new(3f, -21f, 0f), //O2 middle
                new(2f, -19f, 0f), //O2 gas
                new(1f, -24f, 0f), //O2 water
                new(7f, -24f, 0f), //under O2
                new(9f, -20f, 0f), //right outside of O2
                new(7f, -15.8f, 0f), //snowman under elec
                new(11f, -17f, 0f), //comms table
                new(12.7f, -15.5f, 0f), //coms antenna pult
                new(13f, -24.5f, 0f), //weapons window
                new(15f, -17f, 0f), //between coms-office
                new(17.5f, -25.7f, 0f), //snowman under office
            };

            var dlekSPositions = new List<Vector3>()
            {
                new(2.2f, 2.2f, 0f), //cafeteria. botton. top left.
                new(-0.7f, 2.2f, 0f), //caffeteria. button. top right.
                new(2.2f, -0.2f, 0f), //caffeteria. button. bottom left.
                new(-0.7f, -0.2f, 0f), //caffeteria. button. bottom right.
                new(-10.0f, 3.0f, 0f), //weapons top
                new(-9.0f, 1.0f, 0f), //weapons bottom
                new(-6.5f, -3.5f, 0f), //O2
                new(-11.5f, -3.5f, 0f), //O2-nav hall
                new(-17.0f, -3.5f, 0f), //navigation top
                new(-18.2f, -5.7f, 0f), //navigation bottom
                new(-11.5f, -6.5f, 0f), //nav-shields top
                new(-9.5f, -8.5f, 0f), //nav-shields bottom
                new(-9.2f, -12.2f, 0f), //shields top
                new(-8.0f, -14.3f, 0f), //shields bottom
                new(-2.5f, -16f, 0f), //coms left
                new(-4.2f, -16.4f, 0f), //coms middle
                new(-5.5f, -16f, 0f), //coms right
                new(1.5f, -10.0f, 0f), //storage top
                new(1.5f, -15.5f, 0f), //storage bottom
                new(4.5f, -12.5f, 0f), //storrage left
                new(-0.3f, -12.5f, 0f), //storrage right
                new(-4.5f, -7.5f, 0f), //admin top
                new(-4.5f, -9.5f, 0f), //admin bottom
                new(9.0f, -8.0f, 0f), //elec top left
                new(6.0f, -8.0f, 0f), //elec top right
                new(8.0f, -11.0f, 0f), //elec bottom
                new(12.0f, -13.0f, 0f), //elec-lower hall
                new(17f, -10f, 0f), //lower engine top
                new(17.0f, -13.0f, 0f), //lower engine bottom
                new(21.5f, -3.0f, 0f), //reactor top
                new(21.5f, -8.0f, 0f), //reactor bottom
                new(13.0f, -3.0f, 0f), //security top
                new(12.6f, -5.6f, 0f), // security bottom
                new(17.0f, 2.5f, 0f), //upper engibe top
                new(17.0f, -1.0f, 0f), //upper engine bottom
                new(10.5f, 1.0f, 0f), //upper-mad hall
                new(10.5f, -2.0f, 0f), //medbay top
                new(6.5f, -4.5f, 0f) //medbay bottom
            };

            var allLocations = new List<Vector3>();

            foreach (var player in PlayerControl.AllPlayerControls)
                allLocations.Add(player.transform.position);

            foreach (var vent in vents)
                allLocations.Add(GetVentPosition(vent));

            switch (TownOfUsReworked.VanillaOptions.MapId)
            {
                case 0:
                    allLocations.AddRange(SkeldPositions);
                    break;

                case 1:
                    allLocations.AddRange(MiraPositions);
                    break;

                case 2:
                    allLocations.AddRange(PolusPositions);
                    break;

                case 3:
                    allLocations.AddRange(dlekSPositions);
                    break;
            }

            foreach (var target in targets)
            {
                var destination = allLocations.Random();
                coordinates.Add(target.PlayerId, destination);
            }

            return coordinates;
        }

        public static Vector3 GetVentPosition(Vent vent)
        {
            var destination = vent.transform.position;
            destination.y += 0.3636f;
            return destination;
        }

        public static void Revive(DeadBody body)
        {
            if (body == null)
                return;

            var player = PlayerByBody(body);

            if (!player.Data.IsDead)
                return;

            player.Revive();
            var position = body.TruePosition;
            KilledPlayers.Remove(KilledPlayers.Find(x => x.PlayerId == player.PlayerId));
            RecentlyKilled.Remove(player);
            Role.Cleaned.Remove(player);
            ReassignPostmortals(player);
            player.Data.SetImpostor(player.Data.IsImpostor());
            player.NetTransform.SnapTo(new(position.x, position.y + 0.3636f));

            if (ModCompatibility.IsSubmerged && PlayerControl.LocalPlayer == player)
                ModCompatibility.ChangeFloor(player.transform.position.y > -7);

            if (player.Data.IsImpostor())
                RoleManager.Instance.SetRole(player, RoleTypes.Impostor);
            else
                RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);

            body?.gameObject.Destroy();
        }

        public static void Revive(PlayerControl player) => Revive(BodyById(player.PlayerId));

        public static IEnumerator Fade(bool fadeAway, bool enableAfterFade)
        {
            HudManager.Instance.FullScreen.enabled = true;

            if (fadeAway)
            {
                for (float i = 1; i >= 0; i -= Time.deltaTime)
                {
                    HudManager.Instance.FullScreen.color = new(0, 0, 0, i);
                    yield return null;
                }
            }
            else
            {
                for (float i = 0; i <= 1; i += Time.deltaTime)
                {
                    HudManager.Instance.FullScreen.color = new(0, 0, 0, i);
                    yield return null;
                }
            }

            if (enableAfterFade)
            {
                var fs = false;
                var reactor = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<HeliSabotageSystem>();

                if (reactor.IsActive)
                    fs = true;

                HudManager.Instance.FullScreen.enabled = fs;
            }
        }

        public static void Teleport(PlayerControl player, Vector3 position)
        {
            player.MyPhysics.ResetMoveState();
            player.NetTransform.SnapTo(new(position.x, position.y));

            if (ModCompatibility.IsSubmerged && PlayerControl.LocalPlayer == player)
            {
                ModCompatibility.ChangeFloor(player.GetTruePosition().y > -7);
                ModCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
            }

            if (PlayerControl.LocalPlayer == player)
            {
                Flash(Colors.Teleporter);

                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (MapBehaviour.Instance)
                    MapBehaviour.Instance.Close();
            }

            player.moveable = true;
            player.Collider.enabled = true;
            player.NetTransform.enabled = true;
        }

        public static Dictionary<byte, int> CalculateAllVotes(this MeetingHud __instance, out bool tie, out KeyValuePair<byte, int> max)
        {
            var dictionary = new Dictionary<byte, int>();

            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerVoteArea = __instance.playerStates[i];

                if (!playerVoteArea.DidVote || playerVoteArea.AmDead || playerVoteArea.VotedFor == PlayerVoteArea.MissedVote || playerVoteArea.VotedFor == PlayerVoteArea.DeadVote)
                    continue;

                if (dictionary.TryGetValue(playerVoteArea.VotedFor, out var num))
                    dictionary[playerVoteArea.VotedFor] = num + 1;
                else
                    dictionary[playerVoteArea.VotedFor] = 1;
            }

            foreach (var role in Ability.GetAbilities<Politician>(AbilityEnum.Politician))
            {
                foreach (var number in role.ExtraVotes)
                {
                    if (dictionary.TryGetValue(number, out var num))
                        dictionary[number] = num + 1;
                    else
                        dictionary[number] = 1;
                }
            }

            foreach (var role in Role.GetRoles<Mayor>(RoleEnum.Mayor))
            {
                if (role.Revealed)
                {
                    if (dictionary.TryGetValue(role.Voted, out var num))
                        dictionary[role.Voted] = num + CustomGameOptions.MayorVoteCount;
                    else
                        dictionary[role.Voted] = CustomGameOptions.MayorVoteCount;
                }
            }

            var knighted = new List<byte>();

            foreach (var role in Role.GetRoles<Monarch>(RoleEnum.Monarch))
            {
                foreach (var id in role.Knighted)
                {
                    if (!knighted.Contains(id))
                    {
                        var area = VoteAreaById(id);

                        if (dictionary.TryGetValue(area.VotedFor, out var num))
                            dictionary[area.VotedFor] = num + CustomGameOptions.KnightVoteCount;
                        else
                            dictionary[area.VotedFor] = CustomGameOptions.KnightVoteCount;

                        knighted.Add(id);
                    }
                }
            }

            foreach (var swapper in Ability.GetAbilities<Swapper>(AbilityEnum.Swapper))
            {
                if (swapper.IsDead || swapper.Disconnected || swapper.Swap1 == null || swapper.Swap2 == null)
                    continue;

                var swapPlayer1 = PlayerByVoteArea(swapper.Swap1);
                var swapPlayer2 = PlayerByVoteArea(swapper.Swap2);

                if (swapPlayer1 == null || swapPlayer2 == null || swapPlayer1.Data.IsDead || swapPlayer1.Data.Disconnected || swapPlayer2.Data.IsDead || swapPlayer2.Data.Disconnected)
                    continue;

                var swap1 = 0;
                var swap2 = 0;

                if (dictionary.TryGetValue(swapper.Swap1.TargetPlayerId, out var value))
                    swap1 = value;

                if (dictionary.TryGetValue(swapper.Swap2.TargetPlayerId, out var value2))
                    swap2 = value2;

                dictionary[swapper.Swap2.TargetPlayerId] = swap1;
                dictionary[swapper.Swap1.TargetPlayerId] = swap2;
            }

            max = dictionary.MaxPair(out tie);

            if (tie)
            {
                foreach (var player in __instance.playerStates)
                {
                    if (!player.DidVote || player.AmDead || player.VotedFor == PlayerVoteArea.MissedVote || player.VotedFor == PlayerVoteArea.DeadVote)
                        continue;

                    var ability = Ability.GetAbility(player);

                    if (ability.AbilityType == AbilityEnum.Tiebreaker)
                    {
                        if (dictionary.TryGetValue(player.VotedFor, out var num))
                            dictionary[player.VotedFor] = num + 1;
                        else
                            dictionary[player.VotedFor] = 1;
                    }
                }
            }

            dictionary.MaxPair(out tie);
            return dictionary;
        }

        public static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie)
        {
            tie = true;
            var result = new KeyValuePair<byte, int>(255, int.MinValue);

            foreach (var keyValuePair in self)
            {
                if (keyValuePair.Value > result.Value)
                {
                    result = keyValuePair;
                    tie = false;
                }
                else if (keyValuePair.Value == result.Value)
                    tie = true;
            }

            return result;
        }

        public static void ReassignPostmortals(PlayerControl player)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                if (SetPostmortals.WillBeRevealer == player)
                {
                    var toChooseFrom = PlayerControl.AllPlayerControls.Where(x => x.Is(Faction.Crew) && x.Data.IsDead && !x.Data.Disconnected).ToList();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetRevealer, SendOption.Reliable);
                    SetPostmortals.WillBeRevealer = null;

                    if (toChooseFrom.Count == 0)
                        writer.Write(255);
                    else
                    {
                        var rand = URandom.RandomRangeInt(0, toChooseFrom.Count);
                        var pc = toChooseFrom[rand];
                        SetPostmortals.WillBeRevealer = pc;
                        writer.Write(pc.PlayerId);
                    }

                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                else if (SetPostmortals.WillBePhantom == player)
                {
                    var toChooseFrom = PlayerControl.AllPlayerControls.Where(x => x.Is(Faction.Neutral) && x.Data.IsDead && !x.Data.Disconnected).ToList();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetPhantom, SendOption.Reliable);
                    SetPostmortals.WillBePhantom = null;

                    if (toChooseFrom.Count == 0)
                        writer.Write(255);
                    else
                    {
                        var rand = URandom.RandomRangeInt(0, toChooseFrom.Count);
                        var pc = toChooseFrom[rand];
                        SetPostmortals.WillBePhantom = pc;
                        writer.Write(pc.PlayerId);
                    }

                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                else if (SetPostmortals.WillBeBanshee == player)
                {
                    var toChooseFrom = PlayerControl.AllPlayerControls.Where(x => x.Is(Faction.Syndicate) && x.Data.IsDead && !x.Data.Disconnected).ToList();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetBanshee, SendOption.Reliable);
                    SetPostmortals.WillBeBanshee = null;

                    if (toChooseFrom.Count == 0)
                        writer.Write(255);
                    else
                    {
                        var rand = URandom.RandomRangeInt(0, toChooseFrom.Count);
                        var pc = toChooseFrom[rand];
                        SetPostmortals.WillBeBanshee = pc;
                        writer.Write(pc.PlayerId);
                    }

                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                else if (SetPostmortals.WillBeGhoul == player)
                {
                    var toChooseFrom = PlayerControl.AllPlayerControls.Where(x => x.Is(Faction.Neutral) && x.Data.IsDead && !x.Data.Disconnected).ToList();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGhoul, SendOption.Reliable);
                    SetPostmortals.WillBeGhoul = null;

                    if (toChooseFrom.Count == 0)
                        writer.Write(255);
                    else
                    {
                        var rand = URandom.RandomRangeInt(0, toChooseFrom.Count);
                        var pc = toChooseFrom[rand];
                        SetPostmortals.WillBeGhoul = pc;
                        writer.Write(pc.PlayerId);
                    }

                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }

        public static PlayerControl GetClosestPlayer(this PlayerControl refPlayer, List<PlayerControl> AllPlayers = null, float maxDistance = 0f, bool ignoreWalls = false)
        {
            if (refPlayer.Data.IsDead && !refPlayer.Is(RoleEnum.Jester) && !refPlayer.Is(RoleEnum.Ghoul))
                return null;

            var truePosition = refPlayer.GetTruePosition();
            var closestDistance = double.MaxValue;
            PlayerControl closestPlayer = null;
            AllPlayers ??= PlayerControl.AllPlayerControls.Il2CppToSystem();

            if (maxDistance == 0f)
                maxDistance = CustomGameOptions.InteractionDistance;

            foreach (var player in AllPlayers)
            {
                if (player.Data.IsDead || player == refPlayer || !player.Collider.enabled || player.inMovingPlat || (player.inVent && !CustomGameOptions.VentTargetting))
                    continue;

                var distance = Vector2.Distance(truePosition, player.GetTruePosition());
                var vector = player.GetTruePosition() - truePosition;

                if (distance > closestDistance || distance > maxDistance)
                    continue;

                if (PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, distance, Constants.ShipAndObjectsMask) && !ignoreWalls)
                    continue;

                closestPlayer = player;
                closestDistance = distance;
            }

            return closestPlayer;
        }

        public static Vent GetClosestVent(this PlayerControl refPlayer, bool ignoreWalls = false)
        {
            var truePosition = refPlayer.GetTruePosition();
            var maxDistance = CustomGameOptions.InteractionDistance / 2;
            var closestDistance = double.MaxValue;
            Vent closestVent = null;

            foreach (var vent in UObject.FindObjectsOfType<Vent>())
            {
                var distance = Vector2.Distance(truePosition, new(vent.transform.position.x, vent.transform.position.y));
                var vector = new Vector2(vent.transform.position.x, vent.transform.position.y) - truePosition;

                if (distance > maxDistance || distance > closestDistance)
                    continue;

                if (PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, distance, Constants.ShipAndObjectsMask) && !ignoreWalls)
                    continue;

                closestVent = vent;
                closestDistance = distance;
            }

            return closestVent;
        }

        public static DeadBody GetClosestDeadPlayer(this PlayerControl refPlayer, float maxDistance = 0f, bool ignoreWalls = false)
        {
            var truePosition = refPlayer.GetTruePosition();
            var closestDistance = double.MaxValue;
            DeadBody closestBody = null;

            if (maxDistance == 0f)
                maxDistance = CustomGameOptions.InteractionDistance;

            foreach (var body in AllBodies)
            {
                var distance = Vector2.Distance(truePosition, body.TruePosition);
                var vector = body.TruePosition - truePosition;

                if (distance > maxDistance || distance > closestDistance)
                    continue;

                if (PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, distance, Constants.ShipAndObjectsMask) && !ignoreWalls)
                    continue;

                closestBody = body;
                closestDistance = distance;
            }

            return closestBody;
        }

        /*public static void CallRpc(params object[] data)
        {
            if (data[0] is not CustomRPC)
                throw new ArgumentException("First param should be a custom rpc");

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)(CustomRPC)data[0], SendOption.Reliable);

            if (data.Length > 1)
            {
                foreach (var item in data[1..])
                {
                    if (item is bool boolean)
                        writer.Write(boolean);
                    else if (item is int integer)
                        writer.Write(integer);
                    else if (item is uint uinteger)
                        writer.Write(uinteger);
                    else if (item is float Float)
                        writer.Write(Float);
                    else if (item is byte Byte)
                        writer.Write(Byte);
                    else if (item is sbyte sByte)
                        writer.Write(sByte);
                    else if (item is Vector2 vector)
                        writer.Write(vector);
                    else if (item is Vector3 vector3)
                        writer.Write(vector3);
                    else if (item is ulong Ulong)
                        writer.Write(Ulong);
                    else if (item is ushort Ushort)
                        writer.Write(Ushort);
                    else if (item is short Short)
                        writer.Write(Short);
                    else if (item is long Long)
                        writer.Write(Long);
                    else if (item is byte[] array)
                        writer.WriteBytesAndSize(array);
                    else if (item is TargetRPC target)
                        writer.Write((byte)target);
                    else if (item is ActionsRPC action)
                        writer.Write((byte)action);
                    else if (item is TurnRPC turn)
                        writer.Write((byte)turn);
                    else if (item is Faction faction)
                        writer.Write((byte)faction);
                    else if (item is RoleAlignment alignment)
                        writer.Write((byte)alignment);
                    else if (item is SubFaction subfaction)
                        writer.Write((byte)subfaction);
                    else if (item is PlayerLayerEnum layer)
                        writer.Write((byte)layer);
                    else if (item is InspectorResults results)
                        writer.Write((byte)results);
                    else if (item is DeathReasonEnum death)
                        writer.Write((byte)death);
                    else if (item is WinLoseRPC winlose)
                        writer.Write((byte)winlose);
                    else if (item is RetributionistActionsRPC retAction)
                        writer.Write((byte)retAction);
                    else if (item is GodfatherActionsRPC gfAction)
                        writer.Write((byte)gfAction);
                    else if (item is RebelActionsRPC rebAction)
                        writer.Write((byte)rebAction);
                    else
                        LogSomething($"Unknown data type used in the rpc: item - {nameof(item)}, rpc - {data[0]}");
                }
            }

            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }*/
    }
}