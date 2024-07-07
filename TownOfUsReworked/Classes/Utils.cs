namespace TownOfUsReworked.Classes;

public static class Utils
{
    public static bool HasDied(this PlayerControl player) => !player || player.Data == null || player.Data.IsDead || player.Data.Disconnected;

    public static TextMeshPro NameText(this PlayerControl p) => p.cosmetics.nameText;

    public static TextMeshPro ColorBlindText(this PlayerControl p) => p.cosmetics.colorBlindText;

    public static TextMeshPro NameText(this PoolablePlayer p) => p.cosmetics.nameText;

    public static SpriteRenderer MyRend(this PlayerControl p) => p?.cosmetics?.currentBodySprite?.BodySprite;

    public static SpriteRenderer MyRend(this Vent v) => v?.myRend;

    public static SpriteRenderer MyRend(this Console c) => c?.Image;

    public static SpriteRenderer MyRend(this DeadBody d) => d?.bodyRenderers?.FirstOrDefault();

    public static bool IsImpostor(this NetworkedPlayerInfo playerinfo) => playerinfo?.Role?.TeamType == RoleTeamTypes.Impostor;

    public static bool IsImpostor(this PlayerControl playerinfo) => playerinfo?.Data?.IsImpostor() == true;

    public static bool IsImpostor(this PlayerVoteArea playerinfo) => PlayerByVoteArea(playerinfo).IsImpostor();

    public static void SetOutfit(this PlayerControl playerControl, CustomPlayerOutfitType customOutfitType, NetworkedPlayerInfo.PlayerOutfit outfit)
    {
        playerControl.Data.SetOutfit((PlayerOutfitType)customOutfitType, outfit);
        playerControl.SetOutfit(customOutfitType);
    }

    public static void SetOutfit(this PlayerControl playerControl, CustomPlayerOutfitType customOutfit)
    {
        if (!playerControl)
            return;

        var outfitType = (PlayerOutfitType)customOutfit;

        if (playerControl.Data.Outfits.TryGetValue(outfitType, out var newOutfit))
            playerControl.RawSetOutfit(newOutfit, outfitType);
    }

    public static CustomPlayerOutfitType GetCustomOutfitType(this PlayerControl playerControl) => (CustomPlayerOutfitType)playerControl.CurrentOutfitType;

    public static void Morph(PlayerControl player, PlayerControl morphTarget) => Coroutines.Start(MorphCoro(player, morphTarget));

    public static IEnumerator MorphCoro(PlayerControl player, PlayerControl morphTarget)
    {
        if ((int)player.GetCustomOutfitType() is not (3 or 4 or 5 or 6 or 7))
        {
            if (CachedMorphs.TryGetValue(player.PlayerId, out var morphId))
                morphTarget = PlayerById(morphId);

            CachedMorphs.TryAdd(player.PlayerId, morphTarget.PlayerId);

            HUD.StartCoroutine(PerformTimedAction(1, p =>
            {
                var color = UColor.Lerp(player.Data.DefaultOutfit.ColorId.GetColor(false), morphTarget.Data.DefaultOutfit.ColorId.GetColor(false), p);
                PlayerMaterial.SetColors(color, player.MyRend());
                TransitioningSize[player.PlayerId] = Mathf.Lerp(player.GetSize(), morphTarget.GetSize(), p);
                TransitioningSpeed[player.PlayerId] = Mathf.Lerp(player.GetSpeed(), morphTarget.GetSpeed(), p);

                if (p == 1)
                {
                    TransitioningSize.Remove(player.PlayerId);
                    TransitioningSpeed.Remove(player.PlayerId);
                }
            }));

            yield return PerformTimedAction(0.5f, p =>
            {
                player.SetHatAndVisorAlpha(1 - p);
                var color = player.cosmetics.skin.layer.color;
                player.cosmetics.skin.layer.color = new(color.r, color.g, color.b, 1 - p);
            });

            yield return PerformTimedAction(0.5f, p =>
            {
                player.SetHatAndVisorAlpha(p);
                var color = player.cosmetics.skin.layer.color;
                player.cosmetics.skin.layer.color = new(color.r, color.g, color.b, p);
            });

            player.SetOutfit(CustomPlayerOutfitType.Morph, morphTarget.Data.DefaultOutfit);
        }

        yield break;
    }

    public static void DefaultOutfit(PlayerControl player) => Coroutines.Start(DefaultOutfitCoro(player));

    public static IEnumerator DefaultOutfitCoro(PlayerControl player)
    {
        if (player.GetCustomOutfitType() == CustomPlayerOutfitType.Invis)
        {
            player.SetOutfit(CustomPlayerOutfitType.Invis, InvisOutfit1(player));
            var rend = player.MyRend();
            var a = rend.color.a;
            yield return PerformTimedAction(1, p =>
            {
                rend.color = new(1f, 1f, 1f, Mathf.Clamp(p, a, 1));

                if (player != CustomPlayer.Local)
                {
                    var text = player.NameText();
                    text.color = new(text.color.a, text.color.a, text.color.a, p);
                    var cbtext = player.ColorBlindText();
                    cbtext.color = new(cbtext.color.a, cbtext.color.a, cbtext.color.a, p);
                }

                if (!HudHandler.Instance.IsCamoed)
                {
                    player.SetHatAndVisorAlpha(p);
                    var color2 = player.cosmetics.skin.layer.color;
                    player.cosmetics.skin.layer.color = new(color2.r, color2.g, color2.b, p);
                }

                var color = UColor.Lerp(37.GetColor(false), HudHandler.Instance.IsCamoed ? UColor.grey : player.Data.DefaultOutfit.ColorId.GetColor(false), p);
                PlayerMaterial.SetColors(color, rend);
            });
        }
        else if (player.GetCustomOutfitType() == CustomPlayerOutfitType.Camouflage)
        {
            yield return PerformTimedAction(1, p =>
            {
                var color = UColor.Lerp(UColor.grey, player.Data.DefaultOutfit.ColorId.GetColor(false), p);
                PlayerMaterial.SetColors(color, player.MyRend());
                player.SetHatAndVisorAlpha(p);
                var color2 = player.cosmetics.skin.layer.color;
                player.cosmetics.skin.layer.color = new(color2.r, color2.g, color2.b, p);

                if (CustomGameOptions.CamoHideSize)
                    TransitioningSize[player.PlayerId] = Mathf.Lerp(1f, player.GetSize(), p);

                if (CustomGameOptions.CamoHideSpeed)
                    TransitioningSpeed[player.PlayerId] = Mathf.Lerp(1f, player.GetSpeed(), p);

                if (p == 1)
                {
                    TransitioningSize.Remove(player.PlayerId);
                    TransitioningSpeed.Remove(player.PlayerId);
                }
            });
        }
        else if (player.GetCustomOutfitType() == CustomPlayerOutfitType.Morph)
        {
            var morphTarget = CachedMorphs.TryGetValue(player.PlayerId, out var otherId) ? PlayerById(otherId) : player;

            HUD.StartCoroutine(PerformTimedAction(1, p =>
            {
                var color = UColor.Lerp(morphTarget.Data.DefaultOutfit.ColorId.GetColor(false), player.Data.DefaultOutfit.ColorId.GetColor(false), p);
                PlayerMaterial.SetColors(color, player.MyRend());
                TransitioningSize[player.PlayerId] = Mathf.Lerp(morphTarget.GetSize(), player.GetSize(), p);
                TransitioningSpeed[player.PlayerId] = Mathf.Lerp(morphTarget.GetSpeed(), player.GetSpeed(), p);

                if (p == 1)
                {
                    TransitioningSize.Remove(player.PlayerId);
                    TransitioningSpeed.Remove(player.PlayerId);
                }
            }));

            yield return PerformTimedAction(0.5f, p =>
            {
                player.SetHatAndVisorAlpha(1 - p);
                var color2 = player.cosmetics.skin.layer.color;
                player.cosmetics.skin.layer.color = new(color2.r, color2.g, color2.b, 1 - p);
            });

            player.SetOutfit(CustomPlayerOutfitType.Default);

            yield return PerformTimedAction(0.5f, p =>
            {
                player.SetHatAndVisorAlpha(p);
                var color2 = player.cosmetics.skin.layer.color;
                player.cosmetics.skin.layer.color = new(color2.r, color2.g, color2.b, p);
            });

            CachedMorphs.Remove(player.PlayerId);
            player.SetOutfit(CustomPlayerOutfitType.Default);
        }

        if (!HudHandler.Instance.IsCamoed && player.GetCustomOutfitType() != CustomPlayerOutfitType.Default)
            player.SetOutfit(CustomPlayerOutfitType.Default);

        Shapeshifted = false;
        yield break;
    }

    public static void Camouflage() => CustomPlayer.AllPlayers.ForEach(CamoSingle);

    public static void CamoSingle(PlayerControl player)
    {
        if ((int)player.GetCustomOutfitType() is not (4 or 5 or 6 or 7) && !player.Data.IsDead && !CustomPlayer.LocalCustom.Dead && player != CustomPlayer.Local)
        {
            player.SetOutfit(CustomPlayerOutfitType.Camouflage, CamoOutfit(player));
            HUD.StartCoroutine(PerformTimedAction(1, p =>
            {
                var color = UColor.Lerp(player.Data.DefaultOutfit.ColorId.GetColor(false), UColor.grey, p);
                PlayerMaterial.SetColors(color, player.MyRend());
                player.SetHatAndVisorAlpha(1 - p);
                var color2 = player.cosmetics.skin.layer.color;
                player.cosmetics.skin.layer.color = new(color2.r, color2.g, color2.b, 1 - p);

                if (CustomGameOptions.CamoHideSize)
                    TransitioningSize[player.PlayerId] = Mathf.Lerp(player.GetSpeed(), 1f, p);

                if (CustomGameOptions.CamoHideSpeed)
                    TransitioningSpeed[player.PlayerId] = Mathf.Lerp(player.GetSpeed(), 1f, p);

                if (p == 1)
                {
                    TransitioningSize.Remove(player.PlayerId);
                    TransitioningSpeed.Remove(player.PlayerId);
                }
            }));
        }
    }

    public static void Invis(PlayerControl player, bool condition = false)
    {
        var ca = condition || CustomPlayer.LocalCustom.Dead || player == CustomPlayer.Local || CustomPlayer.Local.Is(LayerEnum.Torch) ? 0.1f : 0f;

        if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Invis && !player.Data.IsDead)
        {
            player.SetOutfit(CustomPlayerOutfitType.Invis, InvisOutfit1(player));
            HUD.StartCoroutine(PerformTimedAction(1, p =>
            {
                var rend = player.MyRend();
                rend.color = new(1f, 1f, 1f, Mathf.Clamp(1 - p, ca, 1));
                player.SetHatAndVisorAlpha(1 - p);
                var color2 = player.cosmetics.skin.layer.color;
                player.cosmetics.skin.layer.color = new(color2.r, color2.g, color2.b, 1 - p);
                var color = UColor.Lerp(HudHandler.Instance.IsCamoed ? UColor.grey : player.Data.DefaultOutfit.ColorId.GetColor(false), 37.GetColor(false), p);
                PlayerMaterial.SetColors(color, player.MyRend());

                if (player != CustomPlayer.Local)
                {
                    var text = player.NameText();
                    text.color = new(text.color.r, text.color.g, text.color.b, 1 - p);
                    var cbtext = player.ColorBlindText();
                    cbtext.color = new(cbtext.color.r, cbtext.color.g, cbtext.color.b, 1 - p);
                }

                if (p == 1)
                    player.SetOutfit(CustomPlayerOutfitType.Invis, InvisOutfit2());
            }));
        }
    }

    public static IEnumerator Wait(float duration)
    {
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            yield return EndFrame();
        }

        yield break;
    }

    public static NetworkedPlayerInfo.PlayerOutfit InvisOutfit1(PlayerControl player) => new()
    {
        ColorId = player.CurrentOutfit.ColorId,
        HatId = player.CurrentOutfit.HatId,
        SkinId = player.CurrentOutfit.SkinId,
        VisorId = player.CurrentOutfit.VisorId,
        NamePlateId = "nameplate_NoPlate",
        PlayerName = " ",
        PetId = "pet_EmptyPet"
    };

    public static NetworkedPlayerInfo.PlayerOutfit InvisOutfit2() => new()
    {
        ColorId = 37,
        HatId = "hat_NoHat",
        SkinId = "skin_None",
        VisorId = "visor_EmptyVisor",
        NamePlateId = "nameplate_NoPlate",
        PlayerName = " ",
        PetId = "pet_EmptyPet"
    };

    public static NetworkedPlayerInfo.PlayerOutfit BlankOutfit(PlayerControl player) => new()
    {
        ColorId = player.Data.DefaultOutfit.ColorId,
        HatId = "hat_NoHat",
        SkinId = "skin_None",
        VisorId = "visor_EmptyVisor",
        NamePlateId = "nameplate_NoPlate",
        PlayerName = " ",
        PetId = "pet_EmptyPet"
    };

    public static NetworkedPlayerInfo.PlayerOutfit CamoOutfit(PlayerControl player) => new()
    {
        ColorId = player.CurrentOutfit.ColorId,
        HatId = player.CurrentOutfit.HatId,
        SkinId = player.CurrentOutfit.SkinId,
        VisorId = player.CurrentOutfit.VisorId,
        NamePlateId = "nameplate_NoPlate",
        PlayerName = GetRandomisedName(),
        PetId = "pet_EmptyPet"
    };

    public static NetworkedPlayerInfo.PlayerOutfit ColorblindOutfit() => new()
    {
        ColorId = 7,
        HatId = "hat_NoHat",
        SkinId = "skin_None",
        VisorId = "visor_EmptyVisor",
        NamePlateId = "nameplate_NoPlate",
        PlayerName = " ",
        PetId = "pet_EmptyPet"
    };

    public static NetworkedPlayerInfo.PlayerOutfit NightVisonOutfit() => new()
    {
        ColorId = 11,
        HatId = "hat_NoHat",
        SkinId = "skin_None",
        VisorId = "visor_EmptyVisor",
        PlayerName = " ",
        NamePlateId = "nameplate_NoPlate",
        PetId = "pet_EmptyPet"
    };

    public static void DefaultOutfitAll() => CustomPlayer.AllPlayers.ForEach(DefaultOutfit);

    public static void AddUnique<T>(this ISystem.List<T> self, T item) where T : IDisconnectHandler
    {
        if (!self.Contains(item))
            self.Add(item);
    }

    public static UColor GetShadowColor(this PlayerControl player, bool camoCondition = true, bool otherCondition = false)
    {
        if ((HudHandler.Instance.IsCamoed && camoCondition) || otherCondition)
            return UColor.grey.Shadow();
        else
            return player.Data.DefaultOutfit.ColorId.GetColor(true);
    }

    public static UColor GetPlayerColor(this PlayerControl player, bool camoCondition = true, bool otherCondition = false)
    {
        if ((HudHandler.Instance.IsCamoed && camoCondition) || otherCondition)
            return UColor.grey;
        else
            return player.Data.DefaultOutfit.ColorId.GetColor(false);
    }

    public static PlayerControl PlayerById(byte id) => CustomPlayer.AllPlayers.Find(x => x.PlayerId == id);

    public static PlayerVoteArea VoteAreaById(byte id) => AllVoteAreas.Find(x => x.TargetPlayerId == id);

    public static DeadBody BodyById(byte id) => AllBodies.Find(x => x.ParentId == id);

    public static DeadBody BodyByPlayer(PlayerControl player) => BodyById(player.PlayerId);

    public static PlayerControl PlayerByBody(DeadBody body) => PlayerById(body.ParentId);

    public static PlayerVoteArea VoteAreaByPlayer(PlayerControl player) => VoteAreaById(player.PlayerId);

    public static Vent VentById(int id) => AllVents.Find(x => x.Id == id);

    public static PlayerControl PlayerByVoteArea(PlayerVoteArea state) => PlayerById(state.TargetPlayerId);

    public static Vector2 GetSize() => Vector2.Scale(AllVents[0].GetComponent<BoxCollider2D>().size, AllVents[0].transform.localScale) * 0.75f;

    public static double GetDistance(PlayerControl player, PlayerControl refplayer)
    {
        if (!player || !refplayer)
            return double.MaxValue;

        return Vector2.Distance(refplayer.transform.position, player.transform.position);
    }

    public static void RpcMurderPlayer(PlayerControl self, DeathReasonEnum reason = DeathReasonEnum.Killed) => RpcMurderPlayer(self, self, reason, false);

    public static void RpcMurderPlayer(PlayerControl killer, PlayerControl target, bool lunge) => RpcMurderPlayer(killer, target, DeathReasonEnum.Killed, lunge);

    public static void RpcMurderPlayer(PlayerControl killer, PlayerControl target, DeathReasonEnum reason = DeathReasonEnum.Killed, bool lunge = true)
    {
        if (!killer || !target)
            return;

        MurderPlayer(killer, target, reason, lunge);
        CallRpc(CustomRPC.Action, ActionsRPC.BypassKill, killer, target, reason, lunge);
    }

    public static void MurderPlayer(PlayerControl killer, PlayerControl target, DeathReasonEnum reason, bool lunge)
    {
        if (!killer || !target)
            return;

        var data = target.Data;

        if (data == null || data.IsDead || killer.Data == null)
            return;

        AchievementManager.Instance.OnMurder(killer == CustomPlayer.Local, target == CustomPlayer.Local, CachedMorphs.ContainsKey(killer.PlayerId),
            CachedMorphs.TryGetValue(killer.PlayerId, out var id) ? id : 255, target.PlayerId);
        lunge &= !killer.Is(LayerEnum.Ninja) && killer != target;

        if (IsCustomHnS || CustomPlayer.LocalCustom.Dead)
            UObject.Instantiate(GameManagerCreator.Instance.HideAndSeekManagerPrefab.DeathPopupPrefab, HUD.transform.parent).Show(target, 0);

        if (IsCustomHnS)
            GameData.Instance.RecomputeTaskCounts();

        if (killer == CustomPlayer.Local || target == CustomPlayer.Local)
            Play("Kill");

        if (target == CustomPlayer.Local)
        {
            var tracker = HUD.roomTracker.text;
            var location = tracker.transform.localPosition.y != -3.25f ? tracker.text : "an unknown location";
            BodyLocations[target.PlayerId] = location;
            CallRpc(CustomRPC.Misc, MiscRPC.BodyLocation, target, location);

            if (Vent.currentVent && Vent.currentVent.Buttons != null)
                Vent.currentVent.Buttons.ForEach(x => x.gameObject.SetActive(false));
        }

        if (FirstDead == null)
            FirstDead = target.Data.PlayerName;

        target.gameObject.layer = LayerMask.NameToLayer("Ghost");

        if (CustomPlayer.Local.Is(LayerEnum.Coroner) && !CustomPlayer.LocalCustom.Dead)
            Flash(CustomColorManager.Coroner);

        if (CustomPlayer.LocalCustom.Dead)
            Flash(CustomColorManager.Stalemate);

        if (killer == CustomPlayer.Local && killer.Is(LayerEnum.VampireHunter) && target.Is(SubFaction.Undead))
            Flash(CustomColorManager.Undead);

        if (CustomPlayer.Local.TryGetLayer<Monarch>(out var mon) && mon.Knighted.Contains(target.PlayerId))
            Flash(CustomColorManager.Monarch);

        var targetRole = target.GetRole();

        if (target.Is(LayerEnum.VIP))
        {
            Flash(targetRole.Color);
            Role.LocalRole.AllArrows.TryAdd(target.PlayerId, new(CustomPlayer.Local, CustomColorManager.VIP));
            Role.LocalRole.AllArrows[target.PlayerId].Update(CustomColorManager.VIP);
        }

        var killerRole = killer.GetRole();

        if (target.AmOwner)
        {
            if (ActiveTask)
                ActiveTask.Close();

            if (MapPatch.MapActive)
                Map.Close();

            HUD.KillOverlay.ShowKillAnimation(killer.Data, data);
            HUD.ShadowQuad.gameObject.SetActive(false);
            Chat.SetVisible(true);
            target.NameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
            target.RpcSetScanner(false);
        }

        killer.MyPhysics.StartCoroutine(killer.KillAnimations.Random().CoPerformKill(lunge ? killer : target, target));

        if (killer != target || (killer == target && reason != DeathReasonEnum.Killed))
        {
            targetRole.KilledBy = " By " + killerRole.PlayerName;
            targetRole.DeathReason = reason;
        }
        else
            targetRole.DeathReason = DeathReasonEnum.Suicide;

        if (!PlayerLayer.GetLayers<Altruist>().Any() && !PlayerLayer.GetLayers<Necromancer>().Any())
            targetRole.TrulyDead |= targetRole.Type != LayerEnum.GuardianAngel;

        RecentlyKilled.Add(target.PlayerId);
        KilledPlayers.RemoveAll(x => x.PlayerId == target.PlayerId);
        KilledPlayers.Add(new(killer.PlayerId, target.PlayerId));
        SetPostmortals.BeginPostmortals(target, false);

        if (target == Role.DriveHolder)
            RoleGen.AssignChaosDrive();

        if (target.Is(LayerEnum.Lovers) && AmongUsClient.Instance.AmHost)
        {
            var lover = target.GetOtherLover();

            if (!lover.Is(Alignment.NeutralApoc) && CustomGameOptions.BothLoversDie && !lover.HasDied())
                RpcMurderPlayer(lover);
        }

        if (target.Is(LayerEnum.Diseased) && killer != target)
            killerRole.Diseased = true;
        else if (target.Is(LayerEnum.Bait) && killer != target)
            BaitReport(killer, target);

        if (killer.TryGetLayer<Politician>(out var poli))
            poli.VoteBank++;

        if (target.TryGetLayer<Troll>(out var troll) && AmongUsClient.Instance.AmHost)
        {
            // troll.Killed = true;
            // CallRpc(CustomRPC.WinLose, WinLoseRPC.TrollWin, troll);

            if (!CustomGameOptions.AvoidNeutralKingmakers)
                RpcMurderPlayer(target, killer, DeathReasonEnum.Trolled, false);
        }

        if (Meeting)
            MarkMeetingDead(target, killer);
        else
            target.GetLayers().ForEach(x => x.OnDeath());
    }

    public static void MarkMeetingDead(PlayerControl target, bool doesKill = true, bool noReason = false) => MarkMeetingDead(target, target, doesKill, noReason);

    public static void MarkMeetingDead(PlayerControl target, PlayerControl killer, bool doesKill = true, bool noReason = false)
    {
        Play("Kill");

        if (target == CustomPlayer.Local)
        {
            HUD.KillOverlay.ShowKillAnimation(killer.Data, target.Data);
            HUD.ShadowQuad.gameObject.SetActive(false);
            target.NameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
            target.RpcSetScanner(false);
            Meeting.SetForegroundForDead();

            if (target.TryGetLayer<Swapper>(out var swapper))
            {
                swapper.Swap1 = null;
                swapper.Swap2 = null;
                swapper.SwapMenu.HideButtons();
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, swapper, 255, 255);
            }
            else if (target.TryGetLayer<Dictator>(out var dict))
            {
                dict.DictMenu.HideButtons();
                dict.ToBeEjected.Clear();
                dict.ToDie = false;
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, dict, false, dict.ToBeEjected);
            }
            else if (target.TryGetLayer<Retributionist>(out var ret))
                ret.RetMenu.HideButtons();
            else if (target.IsAssassin())
            {
                var assassin = target.GetLayer<Assassin>();
                assassin.Exit(Meeting);
                assassin.AssassinMenu.HideButtons();
            }
            else if (target.TryGetLayer<Guesser>(out var guesser))
            {
                guesser.Exit(Meeting);
                guesser.GuessMenu.HideButtons();
            }
            else if (target.TryGetLayer<Thief>(out var thief))
            {
                thief.Exit(Meeting);
                thief.GuessMenu.HideButtons();
            }
        }

        target.Die(DeathReason.Kill, false);
        KilledPlayers.Add(new(killer.PlayerId, target.PlayerId));
        var voteArea = VoteAreaByPlayer(target);

        if (voteArea.DidVote)
            voteArea.UnsetVote();

        voteArea.AmDead = true;
        voteArea.Overlay.gameObject.SetActive(true);
        voteArea.Overlay.color = UColor.white;
        voteArea.XMark.gameObject.SetActive(true);
        voteArea.XMark.transform.localScale = Vector3.one;

        foreach (var role in PlayerLayer.GetLayers<Blackmailer>())
        {
            if (target == role.BlackmailedPlayer && role.PrevOverlay)
            {
                voteArea.Overlay.sprite = role.PrevOverlay;
                voteArea.Overlay.color = role.PrevColor.Value;
            }
        }

        foreach (var role in PlayerLayer.GetLayers<Silencer>())
        {
            if (target == role.SilencedPlayer && role.PrevOverlay)
            {
                voteArea.Overlay.sprite = role.PrevOverlay;
                voteArea.Overlay.color = role.PrevColor.Value;
            }
        }

        foreach (var role in PlayerLayer.GetLayers<PromotedGodfather>())
        {
            if (target == role.BlackmailedPlayer && role.PrevOverlay && role.IsBM)
            {
                voteArea.Overlay.sprite = role.PrevOverlay;
                voteArea.Overlay.color = role.PrevColor.Value;
            }
        }

        foreach (var role in PlayerLayer.GetLayers<PromotedRebel>())
        {
            if (target == role.SilencedPlayer && role.PrevOverlay && role.IsSil)
            {
                voteArea.Overlay.sprite = role.PrevOverlay;
                voteArea.Overlay.color = role.PrevColor.Value;
            }
        }

        if (!CustomPlayer.LocalCustom.Dead)
        {
            if (CustomPlayer.Local.IsAssassin())
            {
                var assassin = Ability.LocalAbilityAs<Assassin>();
                assassin.Exit(Meeting);
                assassin.AssassinMenu.HideSingle(target.PlayerId);
            }
            else if (CustomPlayer.Local.TryGetLayer<Guesser>(out var guesser))
            {
                guesser.Exit(Meeting);
                guesser.GuessMenu.HideSingle(target.PlayerId);
            }
            else if (CustomPlayer.Local.TryGetLayer<Thief>(out var thief))
            {
                thief.Exit(Meeting);
                thief.GuessMenu.HideSingle(target.PlayerId);
            }
            else if (CustomPlayer.Local.TryGetLayer<Swapper>(out var swapper))
            {
                if (swapper.SwapMenu.Actives.Any(x => x.Key == target.PlayerId && x.Value))
                {
                    if (swapper.Swap1 == voteArea)
                        swapper.Swap1 = null;
                    else if (swapper.Swap2 == voteArea)
                        swapper.Swap2 = null;

                    swapper.SwapMenu.Actives[target.PlayerId] = false;
                    CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, swapper, 255, 255);
                }

                swapper.SwapMenu.HideSingle(target.PlayerId);
            }
            else if (CustomPlayer.Local.TryGetLayer<Dictator>(out var dict))
            {
                if (dict.DictMenu.Actives.Any(x => x.Key == target.PlayerId && x.Value))
                {
                    dict.ToBeEjected.Clear();
                    dict.DictMenu.Actives.Keys.ForEach(x => dict.DictMenu.Actives[x] = false);
                    CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, dict, false, dict.ToBeEjected);
                }

                dict.DictMenu.HideSingle(target.PlayerId);
            }
        }

        foreach (var area in AllVoteAreas)
        {
            if (area.VotedFor != target.PlayerId)
                continue;

            area.UnsetVote();

            if (target == CustomPlayer.Local)
                Meeting.ClearVote();
        }

        if (AmongUsClient.Instance.AmHost)
        {
            foreach (var pol in PlayerLayer.GetLayers<Politician>())
            {
                if (pol.Player == target)
                    pol.ExtraVotes.Clear();
                else
                {
                    var votesRegained = pol.ExtraVotes.RemoveAll(x => x == target.PlayerId);
                    pol.VoteBank += votesRegained;
                    CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, pol, PoliticianActionsRPC.Add, votesRegained);
                }
            }

            SetPostmortals.AssassinatedPlayers.Add(target.PlayerId);
            Meeting.CheckForEndVoting();
        }

        if (!noReason)
        {
            var role2 = target.GetRole();
            role2.TrulyDead = true;

            if ((killer != target && doesKill) || !doesKill)
            {
                role2.DeathReason = DeathReasonEnum.Guessed;
                role2.KilledBy = " By " + killer.name;
            }
            else
                role2.DeathReason = DeathReasonEnum.Misfire;
        }

        SetPostmortals.BeginPostmortals(target, false);

        if (FirstDead == null)
            FirstDead = target.Data.PlayerName;

        target.GetLayers().ForEach(x => x.OnDeath());
    }

    public static void BaitReport(PlayerControl killer, PlayerControl target) => Coroutines.Start(BaitReportDelay(killer, target));

    public static IEnumerator BaitReportDelay(PlayerControl killer, PlayerControl target)
    {
        if (!killer || !target || killer == target)
            yield break;

        yield return Wait(URandom.RandomRange(CustomGameOptions.BaitMinDelay, CustomGameOptions.BaitMaxDelay));

        if (BodyById(target.PlayerId))
        {
            if (AmongUsClient.Instance.AmHost)
                killer.ReportDeadBody(target.Data);
            else
                CallRpc(CustomRPC.Action, ActionsRPC.BaitReport, killer, target);
        }

        yield break;
    }

    public static void EndGame()
    {
        if (AmongUsClient.Instance.AmHost)
            GameManager.Instance.RpcEndGame(GameOverReason.ImpostorByVote, false);
    }

    public static List<PlayerControl> GetClosestPlayers(Vector2 truePosition, float radius, bool includeDead = false) => [ .. CustomPlayer.AllPlayers.Where(x =>
        Vector2.Distance(truePosition, x.GetTruePosition()) <= radius && (!x.Data.IsDead || (x.Data.IsDead && includeDead))) ];

    public static void StopDragging(byte id)
    {
        PlayerLayer.GetLayers<Janitor>().Where(x => x.CurrentlyDragging && x.CurrentlyDragging.ParentId == id).ForEach(x => x.Drop());
        PlayerLayer.GetLayers<PromotedGodfather>().Where(x => x.CurrentlyDragging && x.CurrentlyDragging.ParentId == id).ForEach(x => x.Drop());
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

    public static string GetRandomisedName()
    {
        var length = URandom.RandomRangeInt(1, 11);
        var name = "";

        while (name.Length < length)
        {
            var random = URandom.RandomRangeInt(0, Everything.Length);
            name += Everything[random];
        }

        return name;
    }

    public static void FadeBody(DeadBody body)
    {
        if (!body)
            return;

        HUD.StartCoroutine(PerformTimedAction(1f, p =>
        {
            foreach (var renderer in body.bodyRenderers)
            {
                if (!renderer)
                    continue;

                var color = renderer.color;
                renderer.color = new(color.r, color.g, color.b, 1 - p);
            }

            if (p == 1)
            {
                body?.gameObject?.Destroy();
                Role.Cleaned.Add(body.ParentId);
            }
        }));
    }

    public static void RpcSpawnVent(Role role)
    {
        if (role.Type is not (LayerEnum.Godfather or LayerEnum.Miner))
            return;

        var position = (Vector2)role.Player.transform.position;
        CallRpc(CustomRPC.Action, ActionsRPC.Mine, role, position);
        AddVent(role, position);
    }

    public static void AddVent(Role role, Vector2 position)
    {
        if (role is Miner miner)
            miner.Vents.Add(SpawnVent(miner.Vents, position, miner.Player.transform.position.z, "Miner"));
        else if (role is PromotedGodfather gf)
            gf.Vents.Add(SpawnVent(gf.Vents, position, gf.Player.transform.position.z, "Godfather"));
    }

    public static Vent SpawnVent(List<Vent> vents, Vector2 position, float zAxis, string name)
    {
        var ventPrefab = UObject.FindObjectOfType<Vent>();
        var vent = UObject.Instantiate(ventPrefab, ventPrefab.transform.parent);

        vent.Id = GetAvailableId();
        vent.transform.position = new(position.x, position.y, zAxis + 0.001f);

        if (vents.Any())
        {
            var leftVent = vents[^1];
            vent.Left = leftVent;
            leftVent.Right = vent;
        }
        else
            vent.Left = null;

        vent.Right = null;
        vent.Center = null;
        vent.name = $"{name}{vents.Count}";

        var allVents = AllMapVents;
        allVents.Add(vent);
        Ship.AllVents = allVents.ToArray();

        if (IsSubmerged())
        {
            vent.gameObject.layer = 12;
            vent.gameObject.AddSubmergedComponent("ElevatorMover"); // Just in case elevator vent is not blocked

            if (vent.transform.position.y > -7)
                vent.transform.position = new(position.x, position.y, 0.03f);
            else
            {
                vent.transform.position = new(position.x, position.y, 0.0009f);
                vent.transform.localPosition = new(vent.transform.localPosition.x, vent.transform.localPosition.y, -0.003f);
            }
        }

        return vent;
    }

    public static int GetAvailableId()
    {
        var id = 0;

        while (true)
        {
            if (AllMapVents.All(v => v.Id != id))
                return id;

            id++;
        }
    }

    public static void Flash(UColor color, float duration = 0.5f) => Coroutines.Start(FlashCoro(color, duration));

    public static IEnumerator FlashCoro(UColor color, float duration)
    {
        if (IntroCutscene.Instance)
            yield break;

        color.a = 0.3f;

        if (HudManager.InstanceExists && HUD.FullScreen)
        {
            var fullscreen = HUD.FullScreen;
            fullscreen.enabled = true;
            fullscreen.gameObject.active = true;
            fullscreen.color = color;
        }

        yield return Wait(duration);

        if (HudManager.InstanceExists && HUD.FullScreen)
            SetFullScreenHUD();

        yield break;
    }

    public static void SetFullScreenHUD()
    {
        if (!HudManager.InstanceExists || !HUD.FullScreen)
            return;

        var fullscreen = HUD.FullScreen;
        fullscreen.color = new(0.6f, 0.6f, 0.6f, 0f);
        fullscreen.enabled = true;
        fullscreen.gameObject.active = true;
        var fs = false;

        if (Ship && !Lobby)
        {
            switch (MapPatches.CurrentMap)
            {
                case 0 or 1 or 3:
                    var reactor1 = Ship.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                    var oxygen1 = Ship.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                    fs = reactor1.IsActive || oxygen1.IsActive;
                    break;

                case 2:
                    var seismic = Ship.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                    fs = seismic.IsActive;
                    break;

                case 4:
                    var reactor = Ship.Systems[SystemTypes.HeliSabotage].Cast<HeliSabotageSystem>();
                    fs = reactor.IsActive;
                    break;

                case 5:
                    var reactor2 = Ship.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                    fs = reactor2.IsActive;
                    break;

                case 6:
                    if (!SubLoaded)
                        break;

                    fs = HasTask(RetrieveOxygenMask);
                    break;

                case 7:
                    if (!LILoaded)
                        break;

                    var reactor3 = Ship.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                    var oxygen3 = Ship.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                    fs = reactor3.IsActive || oxygen3.IsActive;
                    break;
            }
        }

        if (fs)
            fullscreen.color = new(1f, 0f, 0f, 0.37254903f);
    }

    public static Dictionary<byte, Vector2> GenerateWarpCoordinates()
    {
        var targets = CustomPlayer.AllPlayers.Where(player => !player.HasDied() && !UninteractiblePlayers.ContainsKey(player.PlayerId) && !player.inVent);
        var coordinates = new Dictionary<byte, Vector2>();
        var allLocations = new List<Vector2>();
        targets.ForEach(x => allLocations.Add(x.transform.position));
        AllVents.ForEach(x => allLocations.Add(GetVentPosition(x)));
        AllConsoles.ForEach(x => allLocations.Add(GetConsolePosition(x)));
        AllBodies.ForEach(x => allLocations.Add(x.transform.position));
        var tobeadded = MapPatches.CurrentMap switch
        {
            0 => SkeldSpawns,
            1 => MiraSpawns,
            2 => PolusSpawns,
            3 => dlekSSpawns,
            _ => null
        };

        if (tobeadded != null)
            allLocations.AddRange(tobeadded);

        allLocations.Shuffle();
        targets.ForEach(x => coordinates.Add(x.PlayerId, allLocations.Random()));
        AllBodies.ForEach(x => coordinates.Add(x.ParentId, allLocations.Random()));
        return coordinates;
    }

    public static Vector2 GetVentPosition(Vent vent) => new(vent.transform.position.x, vent.transform.position.y + 0.3636f);

    public static Vector2 GetConsolePosition(Console console) => new(console.transform.position.x, console.transform.position.y - 0.5f);

    public static IEnumerator Fade(bool fadeAway)
    {
        HUD.FullScreen.enabled = true;

        if (fadeAway)
        {
            for (var i = 1f; i >= 0; i -= Time.deltaTime)
            {
                HUD.FullScreen.color = new(0, 0, 0, i);
                yield return EndFrame();
            }
        }
        else
        {
            for (var i = 0f; i <= 1; i += Time.deltaTime)
            {
                HUD.FullScreen.color = new(0, 0, 0, i);
                yield return EndFrame();
            }
        }

        SetFullScreenHUD();
        yield break;
    }

    public static IEnumerator CoTeleportPlayer(PlayerControl __instance, Vector2 position)
    {
        yield return Fade(false);
        yield return Wait(0.25f);
        __instance.RpcCustomSnapTo(position);
        yield return Wait(0.25f);
        yield return Fade(true);
        yield break;
    }

    public static PlayerControl GetClosestPlayer(this PlayerControl refPlayer, IEnumerable<PlayerControl> allPlayers = null, float maxDistance = 0f, bool ignoreWalls = false) =>
        GetClosestPlayer(refPlayer.transform.position, allPlayers, maxDistance, ignoreWalls);

    public static PlayerControl GetClosestPlayer(Vector3 position, IEnumerable<PlayerControl> allPlayers = null, float maxDistance = 0f, bool ignoreWalls = false)
    {
        var closestDistance = double.MaxValue;
        PlayerControl closestPlayer = null;
        allPlayers ??= CustomPlayer.AllPlayers;

        if (maxDistance == 0f)
            maxDistance = CustomGameOptions.InteractionDistance;

        foreach (var player in allPlayers)
        {
            if (player.Data.IsDead || !player.Collider.enabled || player.onLadder || player.inMovingPlat || (player.inVent && !CustomGameOptions.VentTargeting) || player.walkingToVent)
                continue;

            var distance = Vector2.Distance(position, player.transform.position);
            var vector = (Vector2)player.transform.position - (Vector2)position;

            if (distance > closestDistance || distance > maxDistance)
                continue;

            if (PhysicsHelpers.AnyNonTriggersBetween(position, vector.normalized, distance, Constants.ShipAndObjectsMask) && !ignoreWalls)
                continue;

            closestPlayer = player;
            closestDistance = distance;
        }

        return closestPlayer;
    }

    public static Vent GetClosestVent(this PlayerControl refPlayer, IEnumerable<Vent> allVents = null, float maxDistance = 0f, bool ignoreWalls = false) =>
        GetClosestVent(refPlayer.transform.position, allVents, maxDistance, ignoreWalls);

    public static Vent GetClosestVent(Vector3 position, IEnumerable<Vent> allVents = null, float maxDistance = 0f, bool ignoreWalls = false)
    {
        var closestDistance = double.MaxValue;
        Vent closestVent = null;
        allVents ??= AllMapVents;

        if (maxDistance == 0f)
            maxDistance = AllMapVents[0].UsableDistance;

        foreach (var vent in allVents)
        {
            var distance = Vector2.Distance(position, vent.transform.position);
            var vector = (Vector2)vent.transform.position - (Vector2)position;

            if (distance > maxDistance || distance > closestDistance)
                continue;

            if (PhysicsHelpers.AnyNonTriggersBetween(position, vector.normalized, distance, Constants.ShipAndObjectsMask) && !ignoreWalls)
                continue;

            closestVent = vent;
            closestDistance = distance;
        }

        return closestVent;
    }

    public static DeadBody GetClosestBody(this PlayerControl refPlayer, IEnumerable<DeadBody> allBodies = null, float maxDistance = 0f, bool ignoreWalls = false) =>
        GetClosestBody(refPlayer.transform.position, allBodies, maxDistance, ignoreWalls);

    public static DeadBody GetClosestBody(Vector3 position, IEnumerable<DeadBody> allBodies = null, float maxDistance = 0f, bool ignoreWalls = false)
    {
        var closestDistance = double.MaxValue;
        DeadBody closestBody = null;
        allBodies ??= AllBodies;

        if (maxDistance == 0f)
            maxDistance = CustomGameOptions.InteractionDistance;

        foreach (var body in allBodies)
        {
            if (Role.Cleaned.Any(x => x == body.ParentId))
                continue;

            var distance = Vector2.Distance(position, body.transform.position);
            var vector = (Vector2)body.transform.position - (Vector2)position;

            if (distance > maxDistance || distance > closestDistance)
                continue;

            if (PhysicsHelpers.AnyNonTriggersBetween(position, vector.normalized, distance, Constants.ShipAndObjectsMask) && !ignoreWalls)
                continue;

            closestBody = body;
            closestDistance = distance;
        }

        return closestBody;
    }

    public static Console GetClosestConsole(this PlayerControl refPlayer, IEnumerable<Console> allConsoles = null, float maxDistance = 0f, bool ignoreWalls = false) =>
        GetClosestConsole(refPlayer.transform.position, allConsoles, maxDistance, ignoreWalls);

    public static Console GetClosestConsole(Vector3 position, IEnumerable<Console> allConsoles = null, float maxDistance = 0f, bool ignoreWalls = false)
    {
        var closestDistance = double.MaxValue;
        Console closestConsole = null;
        allConsoles ??= AllConsoles;

        if (maxDistance == 0f)
            maxDistance = AllConsoles[0].UsableDistance;

        foreach (var console in allConsoles)
        {
            var distance = Vector2.Distance(position, console.transform.position);
            var vector = (Vector2)console.transform.position - (Vector2)position;

            if (distance > maxDistance || distance > closestDistance)
                continue;

            if (PhysicsHelpers.AnyNonTriggersBetween(position, vector.normalized, distance, Constants.ShipAndObjectsMask) && !ignoreWalls)
                continue;

            closestConsole = console;
            closestDistance = distance;
        }

        return closestConsole;
    }

    public static void RemoveTasks(PlayerControl player)
    {
        foreach (var task in player.myTasks)
        {
            var normalPlayerTask = task.TryCast<NormalPlayerTask>();

            if (normalPlayerTask)
            {
                var updateArrow = normalPlayerTask.taskStep > 0;
                normalPlayerTask.taskStep = 0;
                normalPlayerTask.Initialize();

                if (normalPlayerTask.TaskType == TaskTypes.PickUpTowels)
                    UObject.FindObjectsOfType<TowelTaskConsole>().ForEach(x => x.Image.color = UColor.white);

                normalPlayerTask.taskStep = 0;

                if (normalPlayerTask.TaskType == TaskTypes.UploadData)
                    normalPlayerTask.taskStep = 1;

                if ((normalPlayerTask.TaskType is TaskTypes.EmptyGarbage or TaskTypes.EmptyChute) && (MapPatches.CurrentMap is 0 or 3 or 4 or 7))
                    normalPlayerTask.taskStep = 1;

                if (updateArrow)
                    normalPlayerTask.UpdateArrowAndLocation();

                player.Data.FindTaskById(task.Id).Complete = false;
            }
        }
    }

    public static float CycleFloat(float max, float min, float currentVal, bool increment, float change = 1f)
    {
        var value = change * (increment ? 1 : -1);
        currentVal += value;

        if (currentVal > max)
            currentVal = min;
        else if (currentVal < min)
            currentVal = max;

        return currentVal;
    }

    public static int CycleInt(int max, int min, int currentVal, bool increment, int change = 1) => (int)CycleFloat(max, min, currentVal, increment, change);

    public static byte CycleByte(int max, int min, int currentVal, bool increment, int change = 1) => (byte)CycleInt(max, min, currentVal, increment, change);

    public static string WrapText(string text, int width = 90, bool overflow = true)
    {
        var result = new StringBuilder();
        var startIndex = 0;
        var column = 0;

        while (startIndex < text.Length)
        {
            var num = text.IndexOfAny([' ', '\t', '\r'], startIndex);

            if (num != -1)
            {
                if (num == startIndex)
                    ++startIndex;
                else if (text[startIndex] == '\n')
                    startIndex++;
                else
                {
                    AddWord(text[startIndex..num]);
                    startIndex = num + 1;
                }
            }
            else
                break;
        }

        if (startIndex < text.Length)
            AddWord(text[startIndex..]);

        return result.ToString();

        void AddWord(string word)
        {
            var word1 = "";

            if (!overflow && word.Length > width)
            {
                for (var startIndex = 0; startIndex < word.Length; startIndex += word1.Length)
                {
                    word1 = word.Substring(startIndex, Math.Min(width, word.Length - startIndex));
                    AddWord(word1);
                }
            }
            else
            {
                if (column + word.Length >= width)
                {
                    if (column > 0)
                    {
                        result.AppendLine();
                        column = 0;
                    }
                }
                else if (column > 0)
                {
                    result.Append(' ');
                    column++;
                }

                result.Append(word);
                column += word.Length;
            }
        }
    }

    public static string WrapTexts(IEnumerable<string> texts, int width = 90, bool overflow = true)
    {
        if (!texts.Any())
            return "";

        var result = WrapText(texts.First(), width, overflow);
        texts.Skip(1).ForEach(x => result += $"\n{WrapText(x, width, overflow)}");
        return result;
    }

    public static bool IsNullEmptyOrWhiteSpace(string text) => text is null or "" || text.All(x => x is ' ' or '\n');

    public static void RpcBreakShield(PlayerControl target)
    {
        Role.BreakShield(target, CustomGameOptions.ShieldBreaks);
        CallRpc(CustomRPC.Misc, MiscRPC.AttemptSound, target);
    }

    // public static T EnsureComponent<T>(this Component component) where T : Component => component?.gameObject?.EnsureComponent<T>();

    public static T EnsureComponent<T>(this GameObject gameObject) where T : Component => gameObject?.GetComponent<T>() ?? gameObject?.AddComponent<T>();

    public static void CallMeeting(PlayerControl player)
    {
        if (player == CustomPlayer.Local)
            player.RemainingEmergencies++;

        if (AmongUsClient.Instance.AmHost)
        {
            MeetingRoomManager.Instance.reporter = player;
            MeetingRoomManager.Instance.target = null;
            AmongUsClient.Instance.DisconnectHandlers.AddUnique(MeetingRoomManager.Instance.Cast<IDisconnectHandler>());
            HUD.OpenMeetingRoom(player);
            player.RpcStartMeeting(null);
        }
        else
            CallRpc(CustomRPC.Action, ActionsRPC.CallMeeting, player);
    }

    public static void SetOutlineColor(this Renderer renderer, UColor? color)
    {
        if (!renderer)
            return;

        renderer.material.SetFloat("_Outline", color.HasValue ? 1f : 0f);
        renderer.material.SetColor("_OutlineColor", color ?? UColor.clear);
        renderer.material.SetColor("_AddColor", color ?? UColor.clear);
    }

    public static byte ClampByte(float value, float min, float max) => (byte)Mathf.Clamp(value, min, max);

    /*public static string AddSpaces(this string @string)
    {
        Uppercase.ForEach(x =>
        {
            var index = @string.IndexOf(x);

            if (index > 0)
                @string = @string.Insert(index, " ");
        });
        return @string;
    }*/

    public static string SanitisePath(this string path)
    {
        path = path.Replace(".png", "");
        path = path.Replace(".raw", "");
        path = path.Replace(".wav", "");
        path = path.Replace(".txt", "");
        path = path.Replace(TownOfUsReworked.Resources, "");
        path = path.Split('/')[^1];
        path = path.Split('\\')[^1];
        path = path.Split('.')[^1];
        return path;
    }

    public static IEnumerator EndFrame()
    {
        yield return new WaitForEndOfFrame();
        yield break;
    }

    public static void RpcCustomSnapTo(this PlayerControl player, Vector2 pos)
    {
        player.CustomSnapTo(pos);
        CallRpc(CustomRPC.Vanilla, VanillaRPC.SnapTo, player, pos);
    }

    public static void CustomSnapTo(this PlayerControl player, Vector2 pos)
    {
        player.moveable = false;
        player.Collider.enabled = false;
        player.NetTransform.Halt();

        if (player.inVent)
            player.MyPhysics.ExitAllVents();

        player.NetTransform.SnapTo(pos);

        if (player.walkingToVent)
        {
            player.inVent = false;
            Vent.currentVent = null;
            player.MyPhysics.StopAllCoroutines();
            player.walkingToVent = false;
        }

        if (player == CustomPlayer.Local)
        {
            if (ActiveTask)
                ActiveTask.Close();

            if (MapPatch.MapActive)
                Map.Close();

            if (IsSubmerged())
            {
                ChangeFloor(CustomPlayer.Local.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }

            var role = player.GetRole();

            if (role is Janitor jani)
                jani.Drop();
            else if (role is PromotedGodfather gf)
                gf.Drop();
        }

        player.moveable = true;
        player.Collider.enabled = !player.Data.IsDead;
        player.MyPhysics.ResetMoveState();
    }

    public static NetworkedPlayerInfo.PlayerOutfit GetCurrentOutfit(this PlayerControl player)
    {
        if (!player.Data.Outfits.TryGetValue(player.CurrentOutfitType, out var outfit))
            return player.Data.DefaultOutfit;
        else
            return outfit;
    }

    public static IEnumerator PerformTimedAction(float duration, Action<float> action)
    {
        for (var t = 0f; t < duration; t += Time.deltaTime)
        {
            action(t / duration);
            yield return EndFrame();
        }

        action(1f);
        yield break;
    }

    public static void OverrideOnClickListeners(this PassiveButton passive, Action action, bool enabled = true)
    {
        passive.OnClick.RemoveAllListeners();
        passive.OnClick = new();
        passive.OnClick.AddListener(action);
        passive.enabled = enabled;
    }

    public static void OverrideOnMouseOverListeners(this PassiveButton passive, Action action, bool enabled = true)
    {
        passive.OnMouseOver.RemoveAllListeners();
        passive.OnMouseOver = new();
        passive.OnMouseOver.AddListener(action);
        passive.enabled = enabled;
    }

    public static void OverrideOnMouseOutListeners(this PassiveButton passive, Action action, bool enabled = true)
    {
        passive.OnMouseOut.RemoveAllListeners();
        passive.OnMouseOut = new();
        passive.OnMouseOut.AddListener(action);
        passive.enabled = enabled;
    }

    public static bool HasTask(params TaskTypes[] types) => CustomPlayer.Local.myTasks.Any(x => types.Contains(x.TaskType));
}