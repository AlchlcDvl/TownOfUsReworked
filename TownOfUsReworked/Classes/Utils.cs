namespace TownOfUsReworked.Classes;

public static class Utils
{
    public static bool Shapeshifted;

    public static bool HasDied(this PlayerControl player) => player == null || player.Data.IsDead || player.Data.Disconnected;

    public static TextMeshPro NameText(this PlayerControl p) => p.cosmetics.nameText;

    public static TextMeshPro ColorBlindText(this PlayerControl p) => p.cosmetics.colorBlindText;

    public static TextMeshPro NameText(this PoolablePlayer p) => p.cosmetics.nameText;

    public static SpriteRenderer MyRend(this PlayerControl p) => p?.cosmetics?.currentBodySprite?.BodySprite;

    public static SpriteRenderer MyRend(this Vent v) => v?.myRend;

    public static SpriteRenderer MyRend(this Console c) => c?.Image;

    public static SpriteRenderer MyRend(this DeadBody d) => d?.bodyRenderers?.FirstOrDefault();

    public static bool IsImpostor(this GameData.PlayerInfo playerinfo) => playerinfo?.Role?.TeamType == RoleTeamTypes.Impostor;

    public static bool IsImpostor(this PlayerControl playerinfo) => playerinfo?.Data?.Role?.TeamType == RoleTeamTypes.Impostor;

    public static bool IsImpostor(this PlayerVoteArea playerinfo) => PlayerByVoteArea(playerinfo).IsImpostor();

    public static GameData.PlayerOutfit GetDefaultOutfit(this PlayerControl playerControl) => playerControl.Data.DefaultOutfit;

    public static void SetOutfit(this PlayerControl playerControl, CustomPlayerOutfitType customOutfitType, GameData.PlayerOutfit outfit)
    {
        playerControl.Data.SetOutfit((PlayerOutfitType)customOutfitType, outfit);
        playerControl.SetOutfit(customOutfitType);
    }

    public static void SetOutfit(this PlayerControl playerControl, CustomPlayerOutfitType customOutfit)
    {
        if (playerControl == null)
            return;

        var outfitType = (PlayerOutfitType)customOutfit;

        if (!playerControl.Data.Outfits.TryGetValue(outfitType, out var newOutfit))
            return;

        playerControl.CurrentOutfitType = outfitType;
        playerControl.RawSetName(newOutfit.PlayerName);
        playerControl.RawSetColor(newOutfit.ColorId);
        playerControl.RawSetHat(newOutfit.HatId, newOutfit.ColorId);
        playerControl.RawSetVisor(newOutfit.VisorId, newOutfit.ColorId);

        if (!playerControl.Data.IsDead)
            playerControl.RawSetPet(newOutfit.PetId, newOutfit.ColorId);

        if (!playerControl.Data.IsDead)
            playerControl.RawSetSkin(newOutfit.SkinId, newOutfit.ColorId);
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

            HUD.StartCoroutine(Effects.Lerp(1, new Action<float>(p =>
            {
                var color = UColor.Lerp(CustomColors.GetColor(player.GetDefaultOutfit().ColorId, false), CustomColors.GetColor(morphTarget.GetDefaultOutfit().ColorId, false), p);
                PlayerMaterial.SetColors(color, player.MyRend());
                var size = Mathf.Lerp(player.GetSize(), morphTarget.GetSize(), p);
                var speed = Mathf.Lerp(player.GetSpeed(), morphTarget.GetSpeed(), p);

                if (!TransitioningSize.ContainsKey(player.PlayerId))
                    TransitioningSize.TryAdd(player.PlayerId, size);
                else
                    TransitioningSize[player.PlayerId] = size;

                if (!TransitioningSpeed.ContainsKey(player.PlayerId))
                    TransitioningSpeed.TryAdd(player.PlayerId, speed);
                else
                    TransitioningSpeed[player.PlayerId] = speed;

                if (p == 1)
                {
                    TransitioningSize.Remove(player.PlayerId);
                    TransitioningSpeed.Remove(player.PlayerId);
                    player.SetOutfit(CustomPlayerOutfitType.Morph, morphTarget.Data.DefaultOutfit);
                }
            })));

            HUD.StartCoroutine(Effects.Lerp(0.5f, new Action<float>(p =>
            {
                player.SetHatAndVisorAlpha(1 - p);
                var color = player.cosmetics.skin.layer.color;
                player.cosmetics.skin.layer.color = new(color.r, color.g, color.b, 1 - p);
            })));

            yield return Wait(0.5f);

            player.SetOutfit(CustomPlayerOutfitType.Morph, morphTarget.Data.DefaultOutfit);

            HUD.StartCoroutine(Effects.Lerp(0.5f, new Action<float>(p =>
            {
                player.SetHatAndVisorAlpha(p);
                var color = player.cosmetics.skin.layer.color;
                player.cosmetics.skin.layer.color = new(color.r, color.g, color.b, p);
            })));

            yield return Wait(0.5f);
        }

        yield break;
    }

    public static void DefaultOutfit(PlayerControl player) => Coroutines.Start(DefaultOutfitCoro(player));

    public static IEnumerator DefaultOutfitCoro(PlayerControl player)
    {
        if (player.GetCustomOutfitType() == CustomPlayerOutfitType.Invis)
        {
            player.SetOutfit(CustomPlayerOutfitType.Invis, InvisOutfit1(player));
            var a = player.MyRend().color.a;
            HUD.StartCoroutine(Effects.Lerp(1, new Action<float>(p =>
            {
                var rend = player.MyRend();
                rend.color = new(1f, 1f, 1f, Mathf.Clamp(p, a, 1));

                if (player != CustomPlayer.Local)
                {
                    var text = player.NameText();
                    text.color = new(text.color.a, text.color.a, text.color.a, p);
                    var cbtext = player.ColorBlindText();
                    cbtext.color = new(cbtext.color.a, cbtext.color.a, cbtext.color.a, p);
                }

                if (!HudUpdate.IsCamoed)
                {
                    player.SetHatAndVisorAlpha(p);
                    var color2 = player.cosmetics.skin.layer.color;
                    player.cosmetics.skin.layer.color = new(color2.r, color2.g, color2.b, p);
                }

                var color = Color.Lerp(CustomColors.GetColor(37, false), HudUpdate.IsCamoed ? UColor.grey : CustomColors.GetColor(player.GetDefaultOutfit().ColorId, false), p);
                PlayerMaterial.SetColors(color, player.MyRend());
            })));

            yield return Wait(1f);
        }
        else if (player.GetCustomOutfitType() == CustomPlayerOutfitType.Camouflage)
        {
            HUD.StartCoroutine(Effects.Lerp(1, new Action<float>(p =>
            {
                var color = UColor.Lerp(UColor.grey, CustomColors.GetColor(player.GetDefaultOutfit().ColorId, false), p);
                PlayerMaterial.SetColors(color, player.MyRend());
                player.SetHatAndVisorAlpha(p);
                var color2 = player.cosmetics.skin.layer.color;
                player.cosmetics.skin.layer.color = new(color2.r, color2.g, color2.b, p);

                if (CustomGameOptions.CamoHideSize)
                {
                    var size = Mathf.Lerp(1f, player.GetSize(), p);

                    if (!TransitioningSize.ContainsKey(player.PlayerId))
                        TransitioningSize.TryAdd(player.PlayerId, size);
                    else
                        TransitioningSize[player.PlayerId] = size;

                    if (p == 1)
                        TransitioningSize.Remove(player.PlayerId);
                }

                if (CustomGameOptions.CamoHideSpeed)
                {
                    var speed = Mathf.Lerp(1f, player.GetSpeed(), p);

                    if (!TransitioningSpeed.ContainsKey(player.PlayerId))
                        TransitioningSpeed.TryAdd(player.PlayerId, speed);
                    else
                        TransitioningSpeed[player.PlayerId] = speed;

                    if (p == 1)
                        TransitioningSpeed.Remove(player.PlayerId);
                }
            })));

            yield return Wait(1f);
        }
        else if (player.GetCustomOutfitType() == CustomPlayerOutfitType.Morph)
        {
            var morphTarget = PlayerById(CachedMorphs.TryGetValue(player.PlayerId, out var otherId) ? otherId : player.PlayerId);

            HUD.StartCoroutine(Effects.Lerp(1, new Action<float>(p =>
            {
                var color = UColor.Lerp(CustomColors.GetColor(morphTarget.GetDefaultOutfit().ColorId, false), CustomColors.GetColor(player.GetDefaultOutfit().ColorId, false), p);
                PlayerMaterial.SetColors(color, player.MyRend());
                var size = Mathf.Lerp(morphTarget.GetSize(), player.GetSize(), p);
                var speed = Mathf.Lerp(morphTarget.GetSpeed(), player.GetSpeed(), p);

                if (!TransitioningSize.ContainsKey(player.PlayerId))
                    TransitioningSize.TryAdd(player.PlayerId, size);
                else
                    TransitioningSize[player.PlayerId] = size;

                if (!TransitioningSpeed.ContainsKey(player.PlayerId))
                    TransitioningSpeed.TryAdd(player.PlayerId, speed);
                else
                    TransitioningSpeed[player.PlayerId] = speed;

                if (p == 1)
                {
                    TransitioningSize.Remove(player.PlayerId);
                    TransitioningSpeed.Remove(player.PlayerId);
                    player.SetOutfit(CustomPlayerOutfitType.Default);
                }
            })));

            HUD.StartCoroutine(Effects.Lerp(0.5f, new Action<float>(p =>
            {
                player.SetHatAndVisorAlpha(1 - p);
                var color2 = player.cosmetics.skin.layer.color;
                player.cosmetics.skin.layer.color = new(color2.r, color2.g, color2.b, 1 - p);
            })));

            yield return Wait(0.5f);

            player.SetOutfit(CustomPlayerOutfitType.Default);

            HUD.StartCoroutine(Effects.Lerp(0.5f, new Action<float>(p =>
            {
                player.SetHatAndVisorAlpha(p);
                var color2 = player.cosmetics.skin.layer.color;
                player.cosmetics.skin.layer.color = new(color2.r, color2.g, color2.b, p);
            })));

            yield return Wait(0.5f);

            CachedMorphs.Remove(player.PlayerId);
        }

        if (!HudUpdate.IsCamoed && player.GetCustomOutfitType() != CustomPlayerOutfitType.Default)
            player.SetOutfit(CustomPlayerOutfitType.Default);

        Shapeshifted = false;
        yield break;
    }

    public static void Camouflage() => CustomPlayer.AllPlayers.ForEach(CamoSingle);

    public static void CamoSingle(PlayerControl player)
    {
        if ((int)player.GetCustomOutfitType() is not (4 or 5 or 6 or 7) && !player.Data.IsDead && !CustomPlayer.LocalCustom.IsDead && player != CustomPlayer.Local)
        {
            player.SetOutfit(CustomPlayerOutfitType.Camouflage, CamoOutfit(player));
            HUD.StartCoroutine(Effects.Lerp(1, new Action<float>(p =>
            {
                var color = Color.Lerp(CustomColors.GetColor(player.GetDefaultOutfit().ColorId, false), UColor.grey, p);
                PlayerMaterial.SetColors(color, player.MyRend());
                player.SetHatAndVisorAlpha(1 - p);
                var color2 = player.cosmetics.skin.layer.color;
                player.cosmetics.skin.layer.color = new(color2.r, color2.g, color2.b, 1 - p);

                if (CustomGameOptions.CamoHideSize)
                {
                    var size = Mathf.Lerp(player.GetSize(), 1f, p);

                    if (!TransitioningSize.ContainsKey(player.PlayerId))
                        TransitioningSize.TryAdd(player.PlayerId, size);
                    else
                        TransitioningSize[player.PlayerId] = size;

                    if (p == 1)
                        TransitioningSize.Remove(player.PlayerId);
                }

                if (CustomGameOptions.CamoHideSpeed)
                {
                    var speed = Mathf.Lerp(player.GetSpeed(), 1f, p);

                    if (!TransitioningSpeed.ContainsKey(player.PlayerId))
                        TransitioningSpeed.TryAdd(player.PlayerId, speed);
                    else
                        TransitioningSpeed[player.PlayerId] = speed;

                    if (p == 1)
                        TransitioningSpeed.Remove(player.PlayerId);
                }
            })));
        }
    }

    public static void Invis(PlayerControl player, bool condition = false)
    {
        var ca = condition || CustomPlayer.LocalCustom.IsDead || player == CustomPlayer.Local || CustomPlayer.Local.Is(LayerEnum.Torch) ? 0.1f : 0f;

        if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Invis && !player.Data.IsDead)
        {
            player.SetOutfit(CustomPlayerOutfitType.Invis, InvisOutfit1(player));
            HUD.StartCoroutine(Effects.Lerp(1, new Action<float>(p =>
            {
                var rend = player.MyRend();
                rend.color = new(1f, 1f, 1f, Mathf.Clamp(1 - p, ca, 1));
                player.SetHatAndVisorAlpha(1 - p);
                var color2 = player.cosmetics.skin.layer.color;
                player.cosmetics.skin.layer.color = new(color2.r, color2.g, color2.b, 1 - p);
                var color = Color.Lerp(HudUpdate.IsCamoed ? UColor.grey : CustomColors.GetColor(player.GetDefaultOutfit().ColorId, false), CustomColors.GetColor(37, false), p);
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
            })));
        }
    }

    public static IEnumerator Wait(float duration)
    {
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            yield return null;
        }
    }

    public static GameData.PlayerOutfit InvisOutfit1(PlayerControl player) => new()
    {
        ColorId = player.CurrentOutfit.ColorId,
        HatId = player.CurrentOutfit.HatId,
        SkinId = player.CurrentOutfit.SkinId,
        VisorId = player.CurrentOutfit.VisorId,
        NamePlateId = "",
        PlayerName = " ",
        PetId = ""
    };

    public static GameData.PlayerOutfit InvisOutfit2() => new()
    {
        ColorId = 37,
        HatId = "",
        SkinId = "",
        VisorId = "",
        NamePlateId = "",
        PlayerName = " ",
        PetId = ""
    };

    public static GameData.PlayerOutfit BlankOutfit(PlayerControl player) => new()
    {
        ColorId = player.GetDefaultOutfit().ColorId,
        HatId = "",
        SkinId = "",
        VisorId = "",
        NamePlateId = "",
        PlayerName = " ",
        PetId = ""
    };

    public static GameData.PlayerOutfit CamoOutfit(PlayerControl player) => new()
    {
        ColorId = player.CurrentOutfit.ColorId,
        HatId = player.CurrentOutfit.HatId,
        SkinId = player.CurrentOutfit.SkinId,
        VisorId = player.CurrentOutfit.VisorId,
        NamePlateId = "",
        PlayerName = GetRandomisedName(),
        PetId = ""
    };

    public static GameData.PlayerOutfit ColorblindOutfit() => new()
    {
        ColorId = 7,
        HatId = "",
        SkinId = "",
        VisorId = "",
        NamePlateId = "",
        PlayerName = " ",
        PetId = ""
    };

    public static GameData.PlayerOutfit NightVisonOutfit() => new()
    {
        ColorId = 11,
        HatId = "",
        SkinId = "",
        VisorId = "",
        PlayerName = " ",
        NamePlateId = "",
        PetId = ""
    };

    public static void DefaultOutfitAll() => CustomPlayer.AllPlayers.ForEach(DefaultOutfit);

    public static void AddUnique<T>(this ISystem.List<T> self, T item) where T : IDisconnectHandler
    {
        if (!self.Contains(item))
            self.Add(item);
    }

    public static Color GetShadowColor(this PlayerControl player, bool camoCondition = true, bool otherCondition = false)
    {
        if ((HudUpdate.IsCamoed && camoCondition) || otherCondition)
            return new(0.5f, 0.5f, 0.5f, 1f);
        else
            return CustomColors.GetColor(player.GetDefaultOutfit().ColorId, true);
    }

    public static Color GetPlayerColor(this PlayerControl player, bool camoCondition = true, bool otherCondition = false)
    {
        if ((HudUpdate.IsCamoed && camoCondition) || otherCondition)
            return UColor.grey;
        else
            return CustomColors.GetColor(player.GetDefaultOutfit().ColorId, false);
    }

    public static PlayerControl PlayerById(byte id) => CustomPlayer.AllPlayers.Find(x => x.PlayerId == id);

    public static PlayerVoteArea VoteAreaById(byte id) => AllVoteAreas.Find(x => x.TargetPlayerId == id);

    public static DeadBody BodyById(byte id) => AllBodies.Find(x => x.ParentId == id);

    public static DeadBody BodyByPlayer(PlayerControl player) => BodyById(player.PlayerId);

    public static PlayerControl PlayerByBody(DeadBody body) => PlayerById(body.ParentId);

    public static PlayerVoteArea VoteAreaByPlayer(PlayerControl player) => VoteAreaById(player.PlayerId);

    public static Vent VentById(int id) => AllVents.Find(x => x.Id == id);

    public static PlayerControl PlayerByVoteArea(PlayerVoteArea state) => PlayerById(state.TargetPlayerId);

    public static Vector2 GetSize() => Vector2.Scale(AllMapVents[0].GetComponent<BoxCollider2D>().size, AllMapVents[0].transform.localScale) * 0.75f;

    public static double GetDistBetweenPlayers(PlayerControl player, PlayerControl refplayer)
    {
        if (player == null || refplayer == null)
            return double.MaxValue;

        return Vector2.Distance(refplayer.transform.position, player.transform.position);
    }

    public static double GetDistBetweenPlayers(PlayerControl player, Vent refVent)
    {
        if (player == null || refVent == null)
            return double.MaxValue;

        return Vector2.Distance(refVent.transform.position, player.transform.position);
    }

    public static double GetDistBetweenPlayers(PlayerControl player, DeadBody refBody)
    {
        if (player == null || refBody == null)
            return double.MaxValue;

        return Vector2.Distance(refBody.TruePosition, player.transform.position);
    }

    public static void RpcMurderPlayer(PlayerControl self, DeathReasonEnum reason = DeathReasonEnum.Killed, bool lunge = true) => RpcMurderPlayer(self, self, reason, lunge);

    public static void RpcMurderPlayer(PlayerControl killer, PlayerControl target, DeathReasonEnum reason = DeathReasonEnum.Killed, bool lunge = true)
    {
        if (killer == null || target == null)
            return;

        MurderPlayer(killer, target, reason, lunge);
        CallRpc(CustomRPC.Action, ActionsRPC.BypassKill, killer, target, reason, lunge);
    }

    public static void MurderPlayer(PlayerControl killer, PlayerControl target, DeathReasonEnum reason, bool lunge)
    {
        if (killer == null || target == null)
            return;

        var data = target.Data;

        if (data == null)
            return;

        lunge &= !killer.Is(LayerEnum.Ninja) && killer != target;

        if (data.IsDead)
            return;

        if (target.inVent || target.walkingToVent)
            target.GetClosestVent(ignoreWalls: true)?.Buttons.ForEach(x => x.gameObject.SetActive(false));

        if (IsCustomHnS || CustomPlayer.LocalCustom.IsDead)
            UObject.Instantiate(GameManagerCreator.Instance.HideAndSeekManagerPrefab.DeathPopupPrefab, HUD.transform.parent).Show(target, 0);

        if (IsCustomHnS)
            GameData.Instance.RecomputeTaskCounts();

        if (killer == CustomPlayer.Local || target == CustomPlayer.Local)
            Play("Kill");

        if (target == CustomPlayer.Local)
        {
            var tracker = DestroyableSingleton<RoomTracker>.Instance.text;
            var location = tracker.transform.localPosition.y != -3.25f ? tracker.text : "an unknown location";
            BodyLocations.TryAdd(target.PlayerId, location);
            CallRpc(CustomRPC.Misc, MiscRPC.BodyLocation, target, location);
        }

        if (FirstDead == null)
            FirstDead = target;

        target.gameObject.layer = LayerMask.NameToLayer("Ghost");
        target.Visible = false;

        if (CustomPlayer.Local.Is(LayerEnum.Coroner) && !CustomPlayer.LocalCustom.IsDead)
            Flash(Colors.Coroner);

        if (CustomPlayer.LocalCustom.IsDead)
            Flash(Colors.Stalemate);

        if (killer == CustomPlayer.Local && killer.Is(LayerEnum.VampireHunter) && target.Is(SubFaction.Undead))
            Flash(Colors.Undead);

        var targetRole = Role.GetRole(target);

        if (target.Is(LayerEnum.VIP))
        {
            Flash(targetRole.Color);
            Role.LocalRole.AllArrows.TryAdd(target.PlayerId, new(CustomPlayer.Local, Colors.VIP));
            Role.LocalRole.AllArrows[target.PlayerId].Update(Colors.VIP);
        }

        var killerRole = Role.GetRole(killer);

        if (target.AmOwner)
        {
            if (ActiveTask)
                ActiveTask.Close();

            if (Map)
                Map.Close();

            HUD.KillOverlay.ShowKillAnimation(killer.Data, data);
            HUD.ShadowQuad.gameObject.SetActive(false);
            HUD.Chat.SetVisible(true);
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

        if (target.Is(LayerEnum.Framer))
            ((Framer)targetRole).Framed.Clear();

        RecentlyKilled.Add(target);
        KilledPlayers.Add(new(killer.PlayerId, target.PlayerId));
        ReassignPostmortals(target);

        if (target == Role.DriveHolder)
            RoleGen.AssignChaosDrive();

        if (target.Is(LayerEnum.Lovers) && AmongUsClient.Instance.AmHost)
        {
            var lover = target.GetOtherLover();

            if (!lover.Is(LayerEnum.Pestilence) && CustomGameOptions.BothLoversDie && !lover.Data.IsDead)
                RpcMurderPlayer(lover);
        }

        if (target.Is(LayerEnum.Diseased) && killer != target)
            killerRole.Diseased = true;
        else if (target.Is(LayerEnum.Bait) && killer != target)
            BaitReport(killer, target);

        if (killer.Is(LayerEnum.Politician))
            Ability.GetAbility<Politician>(killer).VoteBank++;

        if (target.Is(LayerEnum.Troll) && AmongUsClient.Instance.AmHost)
        {
            var troll = Role.GetRole<Troll>(target);
            troll.Killed = true;
            CallRpc(CustomRPC.WinLose, WinLoseRPC.TrollWin, troll);

            if (!CustomGameOptions.AvoidNeutralKingmakers)
                RpcMurderPlayer(target, killer, DeathReasonEnum.Trolled, false);
        }

        if (Meeting)
            MarkMeetingDead(target, killer);
    }

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

            if (target.Is(LayerEnum.Swapper))
            {
                var swapper = Ability.GetAbility<Swapper>(target);
                swapper.Swap1 = null;
                swapper.Swap2 = null;
                swapper.SwapMenu.HideButtons();
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, swapper, 255, 255);
            }

            if (target.Is(LayerEnum.Dictator))
            {
                var dict = Role.GetRole<Dictator>(target);
                dict.DictMenu.HideButtons();
                dict.ToBeEjected.Clear();
                dict.ToDie = false;
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, dict, DictActionsRPC.SetExiles, false, dict.ToBeEjected.ToArray());
            }

            if (target.Is(LayerEnum.Retributionist))
                Role.GetRole<Retributionist>(target).RetMenu.HideButtons();

            if (target.IsAssassin())
            {
                var assassin = Ability.GetAbility<Assassin>(target);
                assassin.Exit(Meeting);
                assassin.AssassinMenu.HideButtons();
            }

            if (target.Is(LayerEnum.Guesser))
            {
                var guesser = Role.GetRole<Guesser>(CustomPlayer.Local);
                guesser.Exit(Meeting);
                guesser.GuessMenu.HideButtons();
            }

            if (target.Is(LayerEnum.Thief))
            {
                var thief = Role.GetRole<Thief>(CustomPlayer.Local);
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
            if (target == role.BlackmailedPlayer && role.PrevOverlay != null)
            {
                voteArea.Overlay.sprite = role.PrevOverlay;
                voteArea.Overlay.color = role.PrevColor;
            }
        }

        foreach (var role in PlayerLayer.GetLayers<Silencer>())
        {
            if (target == role.SilencedPlayer && role.PrevOverlay != null)
            {
                voteArea.Overlay.sprite = role.PrevOverlay;
                voteArea.Overlay.color = role.PrevColor;
            }
        }

        foreach (var role in PlayerLayer.GetLayers<PromotedGodfather>())
        {
            if (target == role.BlackmailedPlayer && role.PrevOverlay != null && role.IsBM)
            {
                voteArea.Overlay.sprite = role.PrevOverlay;
                voteArea.Overlay.color = role.PrevColor;
            }
        }

        foreach (var role in PlayerLayer.GetLayers<PromotedRebel>())
        {
            if (target == role.SilencedPlayer && role.PrevOverlay != null && role.IsSil)
            {
                voteArea.Overlay.sprite = role.PrevOverlay;
                voteArea.Overlay.color = role.PrevColor;
            }
        }

        if (CustomPlayer.Local.IsAssassin() && !CustomPlayer.LocalCustom.IsDead)
        {
            var assassin = Ability.GetAbility<Assassin>(CustomPlayer.Local);
            assassin.Exit(Meeting);
            assassin.AssassinMenu.HideSingle(target.PlayerId);
        }

        if (CustomPlayer.Local.Is(LayerEnum.Guesser) && !CustomPlayer.LocalCustom.IsDead)
        {
            var guesser = Role.GetRole<Guesser>(CustomPlayer.Local);
            guesser.Exit(Meeting);
            guesser.GuessMenu.HideSingle(target.PlayerId);
        }

        if (CustomPlayer.Local.Is(LayerEnum.Thief) && !CustomPlayer.LocalCustom.IsDead)
        {
            var thief = Role.GetRole<Thief>(CustomPlayer.Local);
            thief.Exit(Meeting);
            thief.GuessMenu.HideSingle(target.PlayerId);
        }

        if (CustomPlayer.Local.Is(LayerEnum.Swapper) && !CustomPlayer.LocalCustom.IsDead)
        {
            var swapper = Ability.GetAbility<Swapper>(CustomPlayer.Local);

            if (swapper.SwapMenu.Actives.Any(x => x.Key == target.PlayerId && x.Value))
            {
                if (swapper.Swap1 == voteArea)
                    swapper.Swap1 = null;
                else if (swapper.Swap2 == voteArea)
                    swapper.Swap2 = null;

                swapper.SwapMenu.Actives[target.PlayerId] = false;
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, swapper, 255, 255);
            }

            swapper.SwapMenu.HideSingle(target.PlayerId);
        }

        if (CustomPlayer.Local.Is(LayerEnum.Dictator) && !CustomPlayer.LocalCustom.IsDead)
        {
            var dictator = Role.GetRole<Dictator>(CustomPlayer.Local);

            if (dictator.DictMenu.Actives.Any(x => x.Key == target.PlayerId && x.Value))
            {
                dictator.ToBeEjected.Clear();

                for (byte i = 0; i < dictator.DictMenu.Actives.Count; i++)
                    dictator.DictMenu.Actives[i] = false;

                dictator.DictMenu.Actives[target.PlayerId] = false;
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, dictator, DictActionsRPC.SetExiles, false, dictator.ToBeEjected.ToArray());
            }

            dictator.DictMenu.HideSingle(target.PlayerId);
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
                    CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, pol, PoliticianActionsRPC.Add, votesRegained);
                }
            }

            AssignPostmortals(target);
            SetPostmortals.AssassinatedPlayers.Add(target);
            Meeting.CheckForEndVoting();
        }

        if (!noReason)
        {
            var role2 = Role.GetRole(target);

            if ((killer != target && doesKill) || !doesKill)
            {
                role2.DeathReason = DeathReasonEnum.Guessed;
                role2.KilledBy = " By " + killer.name;
            }
            else
                role2.DeathReason = DeathReasonEnum.Misfire;
        }
    }

    public static void BaitReport(PlayerControl killer, PlayerControl target) => Coroutines.Start(BaitReportDelay(killer, target));

    public static IEnumerator BaitReportDelay(PlayerControl killer, PlayerControl target)
    {
        if (killer == null || target == null || killer == target)
            yield break;

        yield return Wait(URandom.RandomRange(CustomGameOptions.BaitMinDelay, CustomGameOptions.BaitMaxDelay));

        if (BodyById(target.PlayerId) != null)
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

    public static List<PlayerControl> GetClosestPlayers(Vector2 truePosition, float radius, bool includeDead = false) => CustomPlayer.AllPlayers.Where(x => Vector2.Distance(truePosition,
        x.GetTruePosition()) <= radius && (!x.Data.IsDead || (x.Data.IsDead && includeDead))).ToList();

    public static bool IsTooFar(PlayerControl player, PlayerControl target)
    {
        if (player == null || target == null)
            return true;

        return GetDistBetweenPlayers(player, target) > CustomGameOptions.InteractionDistance;
    }

    public static bool IsTooFar(PlayerControl player, DeadBody target)
    {
        if (player == null || target == null)
            return true;

        return GetDistBetweenPlayers(player, target) > CustomGameOptions.InteractionDistance;
    }

    public static bool IsTooFar(PlayerControl player, Vent target)
    {
        if (player == null || target == null)
            return true;

        return GetDistBetweenPlayers(player, target) > CustomGameOptions.InteractionDistance / 2;
    }

    public static bool NoButton(PlayerControl target, LayerEnum layer) => CustomPlayer.AllPlayers.Count <= 1 || target == null || target.Data == null || !target.CanMove ||
        !target.Is(layer) || !IsRoaming || Meeting || target != CustomPlayer.Local;

    public static void Spread(PlayerControl interacter, PlayerControl target)
    {
        PlayerLayer.GetLayers<Plaguebearer>().ForEach(pb => pb.RpcSpreadInfection(interacter, target));
        PlayerLayer.GetLayers<Arsonist>().ForEach(arso => arso.RpcSpreadDouse(target, interacter));
        PlayerLayer.GetLayers<Cryomaniac>().ForEach(cryo => cryo.RpcSpreadDouse(target, interacter));
    }

    public static void StopDragging(byte id)
    {
        PlayerLayer.GetLayers<Janitor>().Where(x => x.CurrentlyDragging != null && x.CurrentlyDragging.ParentId == id).ForEach(x => x.Drop());
        PlayerLayer.GetLayers<PromotedGodfather>().Where(x => x.CurrentlyDragging != null && x.CurrentlyDragging.ParentId == id).ForEach(x => x.Drop());
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
        yield break;
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
            miner.Vents.Add(SpawnVent(miner.Vents, position, "Miner"));
        else if (role is PromotedGodfather gf)
            gf.Vents.Add(SpawnVent(gf.Vents, position, "Godfather"));
    }

    public static Vent SpawnVent(List<Vent> vents, Vector2 position, string name)
    {
        var ventPrefab = UObject.FindObjectOfType<Vent>();
        var vent = UObject.Instantiate(ventPrefab, ventPrefab.transform.parent);

        vent.Id = GetAvailableId();
        vent.transform.position = position;

        if (vents.Count > 0)
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
        ShipStatus.Instance.AllVents = allVents.ToArray();

        if (IsSubmerged())
        {
            vent.gameObject.layer = 12;
            vent.gameObject.AddSubmergedComponent("ElevatorMover"); //Just in case elevator vent is not blocked

            if (vent.transform.position.y > -7)
                vent.transform.position = new(vent.transform.position.x, vent.transform.position.y, 0.03f);
            else
            {
                vent.transform.position = new(vent.transform.position.x, vent.transform.position.y, 0.0009f);
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

    public static void Flash(Color color, string message, float duration = 0.5f, float size = 100f) => Flash(color, duration, message, size);

    public static void Flash(Color color, float duration = 0.5f, string message = "", float size = 100f) => Coroutines.Start(FlashCoro(color, duration, message, size));

    public static IEnumerator FlashCoro(Color color, float duration, string message, float size)
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

        HUD.Notifier.AddItem($"<color=#FFFFFFFF><size={size}%>{message}</size></color>");

        yield return Wait(duration);

        if (HudManager.InstanceExists && HUD.FullScreen)
            SetFullScreenHUD();

        yield break;
    }

    public static void SetFullScreenHUD()
    {
        if (!(HudManager.InstanceExists && HudManager.Instance.FullScreen))
            return;

        var fullscreen = HUD.FullScreen;
        fullscreen.color = new(0.6f, 0.6f, 0.6f, 0f);
        fullscreen.enabled = true;
        fullscreen.gameObject.active = true;
        var fs = false;

        if (ShipStatus.Instance && !LobbyBehaviour.Instance)
        {
            switch (MapPatches.CurrentMap)
            {
                case 0 or 1 or 3:
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
                    var reactor2 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<HeliSabotageSystem>();
                    fs = reactor2.IsActive;
                    break;

                case 6:
                    if (!SubLoaded)
                        break;

                    fs = CustomPlayer.Local.myTasks.Any(x => x.TaskType == RetrieveOxygenMask);
                    break;

                case 7:
                    if (!LILoaded)
                        break;

                    var reactor3 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                    var oxygen3 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                    var seismic2 = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                    fs = reactor3.IsActive || seismic2.IsActive || oxygen3.IsActive;
                    break;
            }
        }

        if (fs)
            fullscreen.color = new(1f, 0f, 0f, 0.37254903f);
    }

    public static void Warp()
    {
        var coordinates = GenerateWarpCoordinates();
        var writer = AmongUsClient.Instance.StartRpcImmediately(CustomPlayer.Local.NetId, 255, SendOption.Reliable);
        writer.Write((byte)CustomRPC.Action);
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
        if (coordinates.ContainsKey(CustomPlayer.Local.PlayerId))
        {
            Flash(Colors.Warper);

            if (ActiveTask)
                ActiveTask.Close();

            if (Map)
                Map.Close();

            if (CustomPlayer.Local.inVent)
            {
                CustomPlayer.Local.MyPhysics.RpcExitVent(Vent.currentVent.Id);
                CustomPlayer.Local.MyPhysics.ExitAllVents();
            }
        }

        foreach (var (key, value) in coordinates)
        {
            var player = PlayerById(key);
            player.transform.position = value;

            if (TownOfUsReworked.IsTest)
                LogError($"Warping {player.name} to {value}");
        }

        if (AmongUsClient.Instance.AmHost)
        {
            PlayerLayer.GetLayers<Janitor>().Where(x => x.CurrentlyDragging != null).ForEach(x => x.Drop());
            PlayerLayer.GetLayers<PromotedGodfather>().Where(x => x.CurrentlyDragging != null).ForEach(x => x.Drop());
        }
    }

    public static Dictionary<byte, Vector2> GenerateWarpCoordinates()
    {
        var targets = CustomPlayer.AllPlayers.Where(player => !player.HasDied());
        var coordinates = new Dictionary<byte, Vector2>();
        var allLocations = new List<Vector2>();

        foreach (var player in targets)
        {
            if (!player.onLadder && !player.inMovingPlat && !player.inVent)
                allLocations.Add(player.transform.position);
        }

        AllVents.ForEach(x => allLocations.Add(GetVentPosition(x)));
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

        targets.ForEach(x => coordinates.Add(x.PlayerId, allLocations.Random()));
        return coordinates;
    }

    public static Vector2 GetVentPosition(Vent vent) => new(vent.transform.position.x, vent.transform.position.y + 0.3636f);

    public static IEnumerator Fade(bool fadeAway)
    {
        HUD.FullScreen.enabled = true;

        if (fadeAway)
        {
            for (var i = 1f; i >= 0; i -= Time.deltaTime)
            {
                HUD.FullScreen.color = new(0, 0, 0, i);
                yield return null;
            }
        }
        else
        {
            for (var i = 0f; i <= 1; i += Time.deltaTime)
            {
                HUD.FullScreen.color = new(0, 0, 0, i);
                yield return null;
            }
        }

        SetFullScreenHUD();
    }

    public static IEnumerator CoTeleportPlayer(PlayerControl __instance, Vector2 position)
    {
        yield return Fade(false);
        yield return Wait(0.25f);
        __instance.NetTransform.RpcSnapTo(position);
        yield return Wait(0.25f);
        yield return Fade(true);
        yield break;
    }

    public static void Teleport(PlayerControl player, Vector2 position)
    {
        player.MyPhysics.ResetMoveState();
        player.NetTransform.RpcSnapTo(position);

        if (CustomPlayer.Local == player)
        {
            Flash(Colors.Teleporter);

            if (ActiveTask)
                ActiveTask.Close();

            if (Map)
                Map.Close();

            if (IsSubmerged())
            {
                ChangeFloor(player.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }
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

        foreach (var role in PlayerLayer.GetLayers<Politician>())
        {
            foreach (var number in role.ExtraVotes)
            {
                if (dictionary.TryGetValue(number, out var num))
                    dictionary[number] = num + 1;
                else
                    dictionary[number] = 1;
            }
        }

        foreach (var role in PlayerLayer.GetLayers<Mayor>())
        {
            if (role.Revealed)
            {
                if (dictionary.TryGetValue(VoteAreaByPlayer(role.Player).VotedFor, out var num))
                    dictionary[VoteAreaByPlayer(role.Player).VotedFor] = num + CustomGameOptions.MayorVoteCount;
                else
                    dictionary[VoteAreaByPlayer(role.Player).VotedFor] = 1 + CustomGameOptions.MayorVoteCount;
            }
        }

        var knighted = new List<byte>();

        foreach (var role in PlayerLayer.GetLayers<Monarch>())
        {
            foreach (var id in role.Knighted)
            {
                if (!knighted.Contains(id))
                {
                    var area = VoteAreaById(id);

                    if (dictionary.TryGetValue(area.VotedFor, out var num))
                        dictionary[area.VotedFor] = num + CustomGameOptions.KnightVoteCount;
                    else
                        dictionary[area.VotedFor] = 1 + CustomGameOptions.KnightVoteCount;

                    knighted.Add(id);
                }
            }
        }

        foreach (var swapper in PlayerLayer.GetLayers<Swapper>())
        {
            if (swapper.Player.HasDied() || swapper.Swap1 == null || swapper.Swap2 == null)
                continue;

            var swapPlayer1 = PlayerByVoteArea(swapper.Swap1);
            var swapPlayer2 = PlayerByVoteArea(swapper.Swap2);

            if (swapPlayer1 == null || swapPlayer2 == null || swapPlayer1.HasDied() || swapPlayer2.HasDied())
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

                if (PlayerByVoteArea(player).Is(LayerEnum.Tiebreaker))
                {
                    if (dictionary.TryGetValue(player.VotedFor, out var num))
                        dictionary[player.VotedFor] = num + 1;
                    else
                        dictionary[player.VotedFor] = 1;
                }
            }
        }

        _ = dictionary.MaxPair(out tie);
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

    public static void AssignPostmortals(bool revealer, bool ghoul, bool banshee, bool phantom, PlayerControl player = null)
    {
        if (!AmongUsClient.Instance.AmHost)
            return;

        if (player != null)
        {
            if (SetPostmortals.RevealerOn && !SetPostmortals.WillBeRevealers.Contains(player) && player.Is(Faction.Crew) && SetPostmortals.WillBeRevealers.Count(x => x) <
                CustomGameOptions.RevealerCount)
            {
                SetPostmortals.WillBeRevealers.Add(player);
                CallRpc(CustomRPC.Misc, MiscRPC.SetRevealer, player);
            }
            else if (SetPostmortals.PhantomOn && !SetPostmortals.WillBePhantoms.Contains(player) && player.Is(Faction.Neutral) && SetPostmortals.WillBePhantoms.Count(x => x) <
                CustomGameOptions.PhantomCount && !NeutralHasUnfinishedBusiness(player))
            {
                SetPostmortals.WillBePhantoms.Add(player);
                CallRpc(CustomRPC.Misc, MiscRPC.SetPhantom, player);
            }
            else if (SetPostmortals.BansheeOn && !SetPostmortals.WillBeBanshees.Contains(player) && player.Is(Faction.Syndicate) && SetPostmortals.WillBeBanshees.Count(x => x) <
                CustomGameOptions.BansheeCount)
            {
                SetPostmortals.WillBeBanshees.Add(player);
                CallRpc(CustomRPC.Misc, MiscRPC.SetBanshee, player);
            }
            else if (SetPostmortals.GhoulOn && !SetPostmortals.WillBeGhouls.Contains(player) && player.Is(Faction.Intruder) && SetPostmortals.WillBeGhouls.Count(x => x) <
                CustomGameOptions.GhoulCount)
            {
                SetPostmortals.WillBeGhouls.Add(player);
                CallRpc(CustomRPC.Misc, MiscRPC.SetGhoul, player);
            }
        }
        else if (revealer)
        {
            var toChooseFrom = CustomPlayer.AllPlayers.Where(x => x.Is(Faction.Crew) && x.Data.IsDead && !x.Data.Disconnected).ToList();

            if (toChooseFrom.Count == 0)
                CallRpc(CustomRPC.Misc, MiscRPC.SetRevealer, 255);
            else
            {
                var pc = toChooseFrom.Random();
                SetPostmortals.WillBeRevealers.Add(pc);
                CallRpc(CustomRPC.Misc, MiscRPC.SetRevealer, pc);
            }
        }
        else if (phantom)
        {
            var toChooseFrom = CustomPlayer.AllPlayers.Where(x => x.Is(Faction.Neutral) && !NeutralHasUnfinishedBusiness(x) && x.Data.IsDead && !x.Data.Disconnected).ToList();

            if (toChooseFrom.Count == 0)
                CallRpc(CustomRPC.Misc, MiscRPC.SetPhantom, 255);
            else
            {
                var pc = toChooseFrom.Random();
                SetPostmortals.WillBePhantoms.Add(pc);
                CallRpc(CustomRPC.Misc, MiscRPC.SetPhantom, pc);
            }
        }
        else if (banshee)
        {
            var toChooseFrom = CustomPlayer.AllPlayers.Where(x => x.Is(Faction.Syndicate) && x.Data.IsDead && !x.Data.Disconnected).ToList();

            if (toChooseFrom.Count == 0)
                CallRpc(CustomRPC.Misc, MiscRPC.SetBanshee, 255);
            else
            {
                var pc = toChooseFrom.Random();
                SetPostmortals.WillBeBanshees.Add(pc);
                CallRpc(CustomRPC.Misc, MiscRPC.SetBanshee, pc);
            }
        }
        else if (ghoul)
        {
            var toChooseFrom = CustomPlayer.AllPlayers.Where(x => x.Is(Faction.Intruder) && x.Data.IsDead && !x.Data.Disconnected).ToList();

            if (toChooseFrom.Count == 0)
                CallRpc(CustomRPC.Misc, MiscRPC.SetGhoul, 255);
            else
            {
                var pc = toChooseFrom.Random();
                SetPostmortals.WillBeGhouls.Add(pc);
                CallRpc(CustomRPC.Misc, MiscRPC.SetGhoul, pc);
            }
        }
    }

    public static void AssignPostmortals(PlayerControl player) => AssignPostmortals(false, false, false, false, player);

    public static void ReassignPostmortals(PlayerControl player)
    {
        var revealer = SetPostmortals.WillBeRevealers.RemoveAll(x => x == player) > 0 && SetPostmortals.WillBeRevealers.Count < CustomGameOptions.RevealerCount;
        var phantom = SetPostmortals.WillBePhantoms.RemoveAll(x => x == player) > 0 && SetPostmortals.WillBePhantoms.Count < CustomGameOptions.PhantomCount;
        var banshee = SetPostmortals.WillBeBanshees.RemoveAll(x => x == player) > 0 && SetPostmortals.WillBeBanshees.Count < CustomGameOptions.BansheeCount;
        var ghoul = SetPostmortals.WillBeGhouls.RemoveAll(x => x == player) > 0 && SetPostmortals.WillBeGhouls.Count < CustomGameOptions.GhoulCount;

        SetPostmortals.WillBeRevealers.RemoveAll(x => x == null);
        SetPostmortals.WillBePhantoms.RemoveAll(x => x == null);
        SetPostmortals.WillBeBanshees.RemoveAll(x => x == null);
        SetPostmortals.WillBeGhouls.RemoveAll(x => x == null);

        AssignPostmortals(revealer, ghoul, banshee, phantom);
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
            if (player.Data.IsDead || !player.Collider.enabled || player.onLadder || player.inMovingPlat || (player.inVent && !CustomGameOptions.VentTargeting))
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
            if (Role.Cleaned.Any(x => x.PlayerId == body.ParentId))
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

            if (normalPlayerTask != null)
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
            var num = text.IndexOfAny(new[] { ' ', '\t', '\r' }, startIndex);

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

    public static string WrapTexts(List<string> texts, int width = 90, bool overflow = true)
    {
        var result = WrapText(texts[0], width, overflow);
        texts.Skip(1).ForEach(x => result += $"\n{WrapText(x, width, overflow)}");
        return result;
    }

    public static bool IsNullEmptyOrWhiteSpace(string text) => text is null or "" || text.All(x => x is ' ' or '\n');

    public static AudioClip GetIntroSound(RoleTypes roleType) => RoleManager.Instance.AllRoles.ToList().Find(x => x.Role == roleType).IntroSound;

    public static Color FromHex(string hexCode) => ColorUtility.TryParseHtmlString(hexCode, out var color) ? color : default;
}