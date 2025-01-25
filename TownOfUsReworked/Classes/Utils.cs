namespace TownOfUsReworked.Classes;

public static class Utils
{
    public static bool HasDied(this PlayerControl player) => !player || !player.Data || player.Data.IsDead || player.Data.Disconnected;

    public static TextMeshPro NameText(this PlayerControl p) => p.cosmetics.nameText;

    public static TextMeshPro ColorBlindText(this PlayerControl p) => p.cosmetics.colorBlindText;

    public static SpriteRenderer MyRend(this PlayerControl p) => p?.cosmetics?.currentBodySprite?.BodySprite;

    public static SpriteRenderer MyRend(this Vent v) => v?.myRend;

    public static SpriteRenderer MyRend(this Console c) => c?.Image;

    public static SpriteRenderer MyRend(this DeadBody d) => d?.bodyRenderers?.FirstOrDefault();

    public static SpriteRenderer MyRend(this MonoBehaviour m) => m switch
    {
        PlayerControl player => player.MyRend(),
        DeadBody body => body.MyRend(),
        Console console => console.MyRend(),
        Vent vent => vent.MyRend(),
        _ => m?.TryGetComponent<SpriteRenderer>(out var rend) == true ? rend : m?.GetComponentInChildren<SpriteRenderer>()
    };

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

            Coroutines.Start(PerformTimedAction(1, p =>
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
    }

    public static void DefaultOutfit(PlayerControl player) => Coroutines.Start(DefaultOutfitCoro(player));

    public static IEnumerator DefaultOutfitCoro(PlayerControl player)
    {
        if (player.GetCustomOutfitType() == CustomPlayerOutfitType.Invis)
        {
            var rend = player.MyRend();
            var a = player.cosmetics.GetPhantomRoleAlpha();
            yield return PerformTimedAction(1, p =>
            {
                player.cosmetics.SetPhantomRoleAlpha(Mathf.Lerp(a, 1, p));

                if (!player.AmOwner)
                {
                    var text = player.NameText();
                    text.color = new(text.color.a, text.color.a, text.color.a, p);
                    var cbtext = player.ColorBlindText();
                    cbtext.color = new(cbtext.color.a, cbtext.color.a, cbtext.color.a, p);
                }
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

                if (BetterSabotages.CamoHideSize)
                    TransitioningSize[player.PlayerId] = Mathf.Lerp(1f, player.GetSize(), p);

                if (BetterSabotages.CamoHideSpeed)
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

            Coroutines.Start(PerformTimedAction(1, p =>
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
    }

    public static void Camouflage() => AllPlayers().ForEach(CamoSingle);

    public static void CamoSingle(PlayerControl player)
    {
        if (!player.HasDied() && (int)player.GetCustomOutfitType() is not (4 or 5 or 6 or 7) && !CustomPlayer.Local.HasDied() && !player.AmOwner)
        {
            player.SetOutfit(CustomPlayerOutfitType.Camouflage, CamoOutfit(player));
            Coroutines.Start(PerformTimedAction(1, p =>
            {
                var color = UColor.Lerp(player.Data.DefaultOutfit.ColorId.GetColor(false), UColor.grey, p);
                PlayerMaterial.SetColors(color, player.MyRend());
                player.SetHatAndVisorAlpha(1 - p);
                var color2 = player.cosmetics.skin.layer.color;
                player.cosmetics.skin.layer.color = new(color2.r, color2.g, color2.b, 1 - p);

                if (BetterSabotages.CamoHideSize)
                    TransitioningSize[player.PlayerId] = Mathf.Lerp(player.GetSpeed(), 1f, p);

                if (BetterSabotages.CamoHideSpeed)
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
        var ca = condition || CustomPlayer.LocalCustom.Dead || player.AmOwner || CustomPlayer.Local.Is<Torch>() ? 0.1f : 0f;

        if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Invis && !player.Data.IsDead)
        {
            player.SetOutfit(CustomPlayerOutfitType.Invis, CurrentOutfit(player));
            Coroutines.Start(PerformTimedAction(1, p =>
            {
                player.cosmetics.SetPhantomRoleAlpha(Mathf.Lerp(1, ca, p));

                if (!player.AmOwner)
                {
                    var text = player.NameText();
                    text.color = new(text.color.r, text.color.g, text.color.b, 1 - p);
                    var cbtext = player.ColorBlindText();
                    cbtext.color = new(cbtext.color.r, cbtext.color.g, cbtext.color.b, 1 - p);
                }
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
    }

    public static NetworkedPlayerInfo.PlayerOutfit CurrentOutfit(PlayerControl player) => player.CurrentOutfit;

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
        PlayerName = ClientOptions.OptimisationMode ? "" : GetRandomisedName(),
        PetId = "pet_EmptyPet"
    };

    public static NetworkedPlayerInfo.PlayerOutfit ColorblindOutfit() => new()
    {
        ColorId = 39,
        HatId = "hat_NoHat",
        SkinId = "skin_None",
        VisorId = "visor_EmptyVisor",
        NamePlateId = "nameplate_NoPlate",
        PlayerName = " ",
        PetId = "pet_EmptyPet"
    };

    public static NetworkedPlayerInfo.PlayerOutfit NightVisonOutfit() => new()
    {
        ColorId = 6,
        HatId = "hat_NoHat",
        SkinId = "skin_None",
        VisorId = "visor_EmptyVisor",
        PlayerName = " ",
        NamePlateId = "nameplate_NoPlate",
        PetId = "pet_EmptyPet"
    };

    public static void DefaultOutfitAll() => AllPlayers().ForEach(DefaultOutfit);

    public static PlayerControl PlayerById(byte? id) => GameData.Instance?.GetPlayerById(id ?? 255)?.Object;

    public static PlayerVoteArea VoteAreaById(byte? id) => AllVoteAreas().Find(x => x.TargetPlayerId == id);

    public static DeadBody BodyById(byte? id) => AllBodies().Find(x => x.ParentId == id);

    public static DeadBody BodyByPlayer(PlayerControl player) => BodyById(player?.PlayerId);

    public static PlayerControl PlayerByBody(DeadBody body) => PlayerById(body?.ParentId);

    public static PlayerVoteArea VoteAreaByPlayer(PlayerControl player) => VoteAreaById(player?.PlayerId);

    public static PlayerControl PlayerByVoteArea(PlayerVoteArea state) => PlayerById(state?.TargetPlayerId);

    public static Vent VentById(int id) => AllVents().Find(x => x.Id == id);

    public static Vector2 GetSize()
    {
        var first = AllVents().First();
        return Vector2.Scale(first.GetComponent<BoxCollider2D>().size, first.transform.localScale) * 0.75f;
    }

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
        MurderPlayer(killer, target, reason, lunge);
        CallRpc(CustomRPC.Action, ActionsRPC.BypassKill, killer, target, reason, lunge);
    }

    public static void MurderPlayer(PlayerControl self, DeathReasonEnum reason = DeathReasonEnum.Killed, bool lunge = true) => MurderPlayer(self, self, reason, lunge);

    public static void MurderPlayer(PlayerControl killer, PlayerControl target, DeathReasonEnum reason = DeathReasonEnum.Killed, bool lunge = true)
    {
        if (!killer || !target || !target.Data || target.Data.IsDead || !killer.Data)
            return;

        AchievementManager.Instance.OnMurder(killer.AmOwner, target.AmOwner, CachedMorphs.ContainsKey(killer.PlayerId), CachedMorphs.TryGetValue(killer.PlayerId, out var id) ? id : 255,
            target.PlayerId);
        lunge &= !killer.Is<Ninja>() && killer != target;

        if (IsCustomHnS() || CustomPlayer.LocalCustom.Dead)
            UObject.Instantiate(GameManagerCreator.Instance.HideAndSeekManagerPrefab.DeathPopupPrefab, HUD().transform.parent).Show(target, 0);

        GameData.Instance.RecomputeTaskCounts();

        if (killer.AmOwner || target.AmOwner)
            Play("Kill");

        Coroutines.Start(CoPerformKill(killer.KillAnimations.Random(), killer, target, reason, lunge));
    }

    public static void MarkMeetingDead(PlayerControl target) => MarkMeetingDead(target, target);

    public static void MarkMeetingDead(PlayerControl target, PlayerControl killer)
    {
        Play("Kill");

        if (target.AmOwner)
        {
            HUD().KillOverlay.ShowKillAnimation(killer.Data, target.Data);
            Meeting().SetForegroundForDead();
            Meeting().ClearVote();

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
                dict.ToBeEjected = null;
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, dict, DictActionsRPC.SelectToEject, 255);
            }
            else if (target.TryGetLayer<Retributionist>(out var ret))
                ret.RetMenu.HideButtons();
            else if (target.TryGetILayer<IGuesser>(out var assassin))
            {
                assassin.GuessingMenu.Close();
                assassin.GuessMenu.HideButtons();
            }
        }

        target.CustomDie(DeathReasonEnum.Guessed, killer);
        var voteArea = VoteAreaByPlayer(target);
        voteArea.AmDead = true;
        voteArea.Overlay.gameObject.SetActive(true);
        voteArea.Overlay.color = UColor.white;
        voteArea.XMark.gameObject.SetActive(true);
        voteArea.XMark.transform.localScale = Vector3.one;

        if (TalkingPatches.CachedOverlay)
        {
            foreach (var role in PlayerLayer.GetILayers<IIntimidator>())
            {
                if (target == role.Target)
                {
                    voteArea.Overlay.sprite = TalkingPatches.CachedOverlay;
                    voteArea.Overlay.color = TalkingPatches.CachedColor ?? UColor.white;
                }
            }
        }

        if (!CustomPlayer.LocalCustom.Dead)
        {
            if (CustomPlayer.Local.TryGetILayer<IGuesser>(out var assassin))
            {
                assassin.GuessingMenu.Close();
                assassin.GuessMenu.HideSingle(target.PlayerId);
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
                    dict.ToBeEjected = null;
                    dict.DictMenu.Actives.Keys.ForEach(x => dict.DictMenu.Actives[x] = false);
                    CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, dict, DictActionsRPC.SelectToEject, dict.ToBeEjected);
                }

                dict.DictMenu.HideSingle(target.PlayerId);
            }
        }

        foreach (var area in AllVoteAreas())
        {
            if (area.VotedFor == target.PlayerId || area.TargetPlayerId == target.PlayerId)
                area.UnsetVote();
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

            Meeting().CheckForEndVoting();
        }
    }

    public static void BaitReport(PlayerControl killer, PlayerControl target) => Coroutines.Start(BaitReportDelay(killer, target));

    public static IEnumerator BaitReportDelay(PlayerControl killer, PlayerControl target)
    {
        if (!killer || !target || killer == target || !AmongUsClient.Instance.AmHost)
            yield break;

        yield return Wait(URandom.RandomRange(Bait.BaitMinDelay.Value, Bait.BaitMaxDelay.Value));

        if (BodyById(target.PlayerId))
            killer.ReportDeadBody(target.Data);
    }

    public static void EndGame()
    {
        if (AmongUsClient.Instance.AmHost)
            GameManager.Instance.RpcEndGame((GameOverReason)9, false);
    }

    public static IEnumerable<PlayerControl> GetClosestPlayers(Vector2 truePosition, float radius, bool includeDead = false) => AllPlayers().Where(x => Vector2.Distance(truePosition,
        x.GetTruePosition()) <= radius && (!x.Data.IsDead || includeDead));

    public static IEnumerable<PlayerControl> GetClosestPlayers(PlayerControl player, float radius, Func<PlayerControl, bool> filter = null, bool includeDead = false)
    {
        var result = AllPlayers().Where(x => Vector2.Distance(player.GetTruePosition(), x.GetTruePosition()) <= radius && (!x.Data.IsDead || includeDead) && x != player);

        if (filter != null)
            result = result.Where(filter);

        return result;
    }

    public static void StopDragging(byte id) => PlayerLayer.GetILayers<IDragger>().Where(x => x.CurrentlyDragging?.ParentId == id).ForEach(x => x.Drop());

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

    public static bool IsInRange(this int num, float min, float max, bool minInclusive = false, bool maxInclusive = false) => ((float)num).IsInRange(min, max, minInclusive, maxInclusive);

    public static string GetRandomisedName()
    {
        var length = URandom.RandomRangeInt(1, 11);
        var name = "";

        while (name.Length < length)
            name += Everything[URandom.RandomRangeInt(0, Everything.Length)];

        return name;
    }

    public static void FadeBody(DeadBody body)
    {
        if (!body)
            return;

        Coroutines.Start(PerformTimedAction(1f, p =>
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
                Cleaned.Add(body.ParentId);
            }
        }));
    }

    public static void RpcSpawnVent(Role role)
    {
        if (role is not IDigger digger)
            return;

        var position = (Vector2)role.Player.transform.position;
        CallRpc(CustomRPC.Action, ActionsRPC.Mine, role, position);
        AddVent(digger, position);
    }

    public static void AddVent(IDigger digger, Vector2 position) => digger.Vents.Add(SpawnVent(digger.Vents, position, digger.Player.transform.position.z, digger.Name));

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
        vent.myAnim?.Stop();

        var allVents = AllMapVents().ToList();
        allVents.Add(vent);
        Ship().AllVents = allVents.ToArray();

        if (IsSubmerged())
        {
            vent.gameObject.layer = 12;
            vent.gameObject.AddSubmergedComponent("ElevatorMover"); // Just in case elevator vent is not blocked

            if (vent.transform.position.y > -7)
                vent.transform.SetWorldZ(0.03f);
            else
            {
                vent.transform.SetWorldZ(0.0009f);
                vent.transform.SetLocalZ(-0.003f);
            }
        }

        return vent;
    }

    public static int GetAvailableId()
    {
        var id = 0;

        while (true)
        {
            if (AllMapVents().All(v => v.Id != id))
                return id;

            id++;
        }
    }

    public static void Flash(Color32 color, float duration = 0.5f) => Coroutines.Start(FlashCoro(color, duration));

    public static void Flash(UColor color, float duration = 0.5f) => Coroutines.Start(FlashCoro(color, duration));

    private static IEnumerator FlashCoro(UColor color, float duration)
    {
        if (IntroCutscene.Instance || ShowRolePatch.Starting || !HudManager.InstanceExists)
            yield break;

        color.a = 0.3f;
        var fullscreen = HUD().FullScreen;
        fullscreen.enabled = true;
        fullscreen.gameObject.active = true;
        fullscreen.color = color;
        yield return Wait(duration);
        SetFullScreenHUD();
    }

    public static void SetFullScreenHUD()
    {
        if (!HudManager.InstanceExists || !Ship() || Lobby())
            return;

        var fullscreen = HUD().FullScreen;
        fullscreen.color = new(0.6f, 0.6f, 0.6f, 0f);
        fullscreen.enabled = true;
        fullscreen.gameObject.active = true;
        var ship = Ship();
        var fs = MapPatches.CurrentMap switch
        {
            0 or 1 or 3 => ship.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>().IsActive || ship.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>().IsActive,
            2 => ship.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>().IsActive,
            4 => ship.Systems[SystemTypes.HeliSabotage].Cast<HeliSabotageSystem>().IsActive,
            5 => ship.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>().IsActive,
            6 => SubLoaded && HasTask(RetrieveOxygenMask),
            7 => LILoaded && ((ship.Systems.TryGetValue(SystemTypes.Reactor, out var system1) && system1.Cast<ReactorSystemType>().IsActive) ||
                (ship.Systems.TryGetValue(SystemTypes.Laboratory, out system1) && system1.Cast<ReactorSystemType>().IsActive) || (ship.Systems.TryGetValue(SystemTypes.Laboratory, out
                    system1) && system1.Cast<LifeSuppSystemType>().IsActive)),
            _ => false
        };

        if (fs)
            fullscreen.color = new(1f, 0f, 0f, 0.37254903f);
    }

    public static Dictionary<byte, Vector2> GenerateWarpCoordinates()
    {
        var targets = AllPlayers().Where(player => !player.HasDied() && !UninteractiblePlayers.ContainsKey(player.PlayerId) && !player.inVent);
        var coordinates = new Dictionary<byte, Vector2>();
        var allLocations = new List<Vector2>();
        allLocations.AddRanges(targets.Select(x => new Vector2(x.GetTruePosition().x, x.GetTruePosition().y + 0.3636f)), AllVents().Select(x => (Vector2)x.transform.position),
            AllBodies().Select(x => x.TruePosition));
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
        AllBodies().ForEach(x => coordinates.Add(x.ParentId, allLocations.Random()));
        return coordinates;
    }

    public static IEnumerator Fade(bool fadeAway)
    {
        if (!HudManager.InstanceExists)
            yield break;

        var hud = HUD();
        hud.FullScreen.enabled = true;

        if (fadeAway)
        {
            for (var i = 1f; i >= 0; i -= Time.deltaTime)
            {
                hud.FullScreen.color = new(0, 0, 0, i);
                yield return EndFrame();
            }
        }
        else
        {
            for (var i = 0f; i <= 1; i += Time.deltaTime)
            {
                hud.FullScreen.color = new(0, 0, 0, i);
                yield return EndFrame();
            }
        }

        SetFullScreenHUD();
    }

    public static IEnumerator CoTeleportPlayer(PlayerControl __instance, Vector2 position)
    {
        yield return Fade(false);
        yield return Wait(0.25f);
        __instance.RpcCustomSnapTo(position);
        yield return Wait(0.25f);
        yield return Fade(true);
    }

    public static PlayerControl GetClosestPlayer(this PlayerControl refPlayer, IEnumerable<PlayerControl> allPlayers = null, float maxDistance = float.NaN, bool ignoreWalls = false,
        Func<PlayerControl, bool> predicate = null, bool includeDead = false)
    {
        if (predicate != null)
            return GetClosestPlayer(refPlayer.GetTruePosition(), allPlayers, maxDistance, ignoreWalls, x => x != refPlayer && predicate(x), includeDead);
        else
            return GetClosestPlayer(refPlayer.GetTruePosition(), allPlayers, maxDistance, ignoreWalls, x => x != refPlayer, includeDead);
    }

    public static PlayerControl GetClosestPlayer(Vector2 position, IEnumerable<PlayerControl> allPlayers = null, float maxDistance = float.NaN, bool ignoreWalls = false, Func<PlayerControl,
        bool> predicate = null, bool includeDead = false)
    {
        var closestDistance = float.MaxValue;
        PlayerControl closestPlayer = null;
        allPlayers ??= AllPlayers();

        if (predicate != null)
            allPlayers = allPlayers.Where(predicate);

        if (float.IsNaN(maxDistance))
            maxDistance = GameSettings.InteractionDistance;

        foreach (var player in allPlayers)
        {
            if ((player.Data.IsDead && !includeDead) || !player.Collider.enabled || player.onLadder || player.inMovingPlat || (player.inVent && !GameModifiers.VentTargeting) ||
                player.walkingToVent || player.isKilling)
            {
                continue;
            }

            var truePos = player.GetTruePosition();
            var distance = Vector2.Distance(position, truePos);
            var vector = truePos - position;

            if (distance > closestDistance || distance > maxDistance)
                continue;

            if (PhysicsHelpers.AnyNonTriggersBetween(position, vector.normalized, distance, Constants.ShipAndObjectsMask) && !ignoreWalls)
                continue;

            closestPlayer = player;
            closestDistance = distance;
        }

        return closestPlayer;
    }

    public static Vent GetClosestVent(this PlayerControl refPlayer, IEnumerable<Vent> allVents = null, float maxDistance = float.NaN, bool ignoreWalls = false, Func<Vent, bool> predicate =
        null) => GetClosestVent(refPlayer.GetTruePosition(), allVents, maxDistance, ignoreWalls, predicate);

    public static Vent GetClosestVent(Vector2 position, IEnumerable<Vent> allVents = null, float maxDistance = float.NaN, bool ignoreWalls = false, Func<Vent, bool> predicate = null)
    {
        var closestDistance = float.MaxValue;
        Vent closestVent = null;
        allVents ??= AllMapVents();

        if (float.IsNaN(maxDistance))
            maxDistance = AllMapVents().First().UsableDistance;

        if (predicate != null)
            allVents = allVents.Where(predicate);

        foreach (var vent in allVents)
        {
            var distance = Vector2.Distance(position, vent.transform.position);
            var vector = (Vector2)vent.transform.position - position;

            if (distance > maxDistance || distance > closestDistance)
                continue;

            if (PhysicsHelpers.AnyNonTriggersBetween(position, vector.normalized, distance, Constants.ShipAndObjectsMask) && !ignoreWalls)
                continue;

            closestVent = vent;
            closestDistance = distance;
        }

        return closestVent;
    }

    public static DeadBody GetClosestBody(this PlayerControl refPlayer, IEnumerable<DeadBody> allBodies = null, float maxDistance = float.NaN, bool ignoreWalls = false, Func<DeadBody, bool>
        predicate = null) => GetClosestBody(refPlayer.GetTruePosition(), allBodies, maxDistance, ignoreWalls, predicate);

    public static DeadBody GetClosestBody(Vector2 position, IEnumerable<DeadBody> allBodies = null, float maxDistance = float.NaN, bool ignoreWalls = false, Func<DeadBody, bool> predicate =
        null)
    {
        var closestDistance = float.MaxValue;
        DeadBody closestBody = null;
        allBodies ??= AllBodies();

        if (float.IsNaN(maxDistance))
            maxDistance = GameSettings.InteractionDistance;

        if (predicate != null)
            allBodies = allBodies.Where(predicate);

        foreach (var body in allBodies)
        {
            if (Cleaned.Any(x => x == body.ParentId))
                continue;

            var distance = Vector2.Distance(position, body.TruePosition);
            var vector = body.TruePosition - position;

            if (distance > maxDistance || distance > closestDistance)
                continue;

            if (PhysicsHelpers.AnyNonTriggersBetween(position, vector.normalized, distance, Constants.ShipAndObjectsMask) && !ignoreWalls)
                continue;

            closestBody = body;
            closestDistance = distance;
        }

        return closestBody;
    }

    public static Console GetClosestConsole(this PlayerControl refPlayer, IEnumerable<Console> allConsoles = null, float maxDistance = float.NaN, bool ignoreWalls = false, Func<Console, bool>
        predicate = null) => GetClosestConsole(refPlayer.GetTruePosition(), allConsoles, maxDistance, ignoreWalls, predicate);

    public static Console GetClosestConsole(Vector2 position, IEnumerable<Console> allConsoles = null, float maxDistance = float.NaN, bool ignoreWalls = false, Func<Console, bool> predicate =
        null)
    {
        var closestDistance = float.MaxValue;
        Console closestConsole = null;
        allConsoles ??= AllConsoles();

        if (predicate != null)
            allConsoles = allConsoles.Where(predicate);

        foreach (var console in allConsoles)
        {
            var distance = Vector2.Distance(position, console.transform.position);
            var vector = (Vector2)console.transform.position - position;
            var tempMaxDistance = maxDistance;

            if (float.IsNaN(tempMaxDistance))
                tempMaxDistance = console.UsableDistance;

            if (distance > tempMaxDistance || distance > closestDistance)
                continue;

            if (PhysicsHelpers.AnyNonTriggersBetween(position, vector.normalized, distance, Constants.ShipAndObjectsMask) && !ignoreWalls)
                continue;

            closestConsole = console;
            closestDistance = distance;
        }

        return closestConsole;
    }

    public static MonoBehaviour GetClosestMono(this PlayerControl player, IEnumerable<MonoBehaviour> allMonos, float trueMaxDistance = float.NaN, bool ignoreWalls = false, Func<MonoBehaviour,
        bool> predicate = null) => GetClosestMono(player.GetTruePosition(), allMonos, trueMaxDistance, ignoreWalls, predicate);

    public static MonoBehaviour GetClosestMono(Vector2 position, IEnumerable<MonoBehaviour> allMonos, float trueMaxDistance = float.NaN, bool ignoreWalls = false, Func<MonoBehaviour, bool>
        predicate = null)
    {
        var closestDistance = float.MaxValue;
        MonoBehaviour closestMono = null;

        if (predicate != null)
            allMonos = allMonos.Where(predicate);

        foreach (var mono in allMonos)
        {
            if (!mono)
                continue;

            var distance = Vector2.Distance(position, mono.transform.position);
            var vector = (Vector2)mono.transform.position - position;
            var maxDistance = trueMaxDistance;

            if (float.IsNaN(maxDistance))
            {
                maxDistance = mono switch
                {
                    PlayerControl or DeadBody => GameSettings.InteractionDistance,
                    Console console => console.UsableDistance,
                    Vent vent => vent.UsableDistance,
                    _ => 1f,
                };
            }

            if (distance > maxDistance || distance > closestDistance)
                continue;

            if (PhysicsHelpers.AnyNonTriggersBetween(position, vector.normalized, distance, Constants.ShipAndObjectsMask) && !ignoreWalls)
                continue;

            closestMono = mono;
            closestDistance = distance;
        }

        return closestMono;
    }

    public static void RemoveTasks(PlayerControl player)
    {
        foreach (var task in player.myTasks)
        {
            if (task.TryCast<NormalPlayerTask>(out var normalPlayerTask))
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

        return $"{result}";

        void AddWord(string word)
        {
            var word1 = "";

            if (!overflow && word.Length > width)
            {
                for (var startIndex = 0; startIndex < word.Length; startIndex += word1.Length)
                {
                    word1 = word.Substring(startIndex, Mathf.Min(width, word.Length - startIndex));
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
        Role.BreakShield(target, Medic.ShieldBreaks);
        CallRpc(CustomRPC.Misc, MiscRPC.BreakShield, target);
    }

    public static T AddComponent<T>(this Component component) where T : Component => component?.gameObject?.AddComponent<T>();

    public static T EnsureComponent<T>(this Component component) where T : Component => component?.gameObject?.EnsureComponent<T>();

    public static T EnsureComponent<T>(this GameObject gameObject) where T : Component => gameObject?.GetComponent<T>() ?? gameObject?.AddComponent<T>();

    public static void CallMeeting(PlayerControl player)
    {
        if (player.AmOwner)
            player.RemainingEmergencies++;

        if (AmongUsClient.Instance.AmHost)
        {
            MeetingRoomManager.Instance.reporter = player;
            MeetingRoomManager.Instance.target = null;
            AmongUsClient.Instance.DisconnectHandlers.AddUnique(MeetingRoomManager.Instance.Cast<IDisconnectHandler>());
            HUD().OpenMeetingRoom(player);
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

    public static string SanitisePath(this string path)
    {
        path = path.Replace(".png", "");
        path = path.Replace(".wav", "");
        path = path.Replace(".txt", "");
        path = path.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[^1];
        path = path.Split('\\', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[^1];
        path = path.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[^1];
        return path;
    }

    public static IEnumerator EndFrame()
    {
        yield return new WaitForEndOfFrame();
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

        if (player.AmOwner)
        {
            if (ActiveTask())
                ActiveTask().Close();

            if (MapBehaviourPatches.MapActive)
                Map().Close();

            if (IsSubmerged())
            {
                ChangeFloor(CustomPlayer.Local.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }

            if (player.GetRole() is IDragger dragger)
                dragger.Drop();
        }

        player.moveable = true;
        player.Collider.enabled = !player.Data.IsDead;
        player.MyPhysics.ResetMoveState();
        player.MyPhysics.ResetAnimState();
    }

    public static NetworkedPlayerInfo.PlayerOutfit GetCurrentOutfit(this PlayerControl player) => player.Data.Outfits.TryGetValue(player.CurrentOutfitType, out var outfit) ? outfit :
        player.Data.DefaultOutfit;

    public static IEnumerator PerformTimedAction(float duration, Action<float> action)
    {
        for (var t = 0f; t < duration; t += Time.deltaTime)
        {
            action(t / duration);
            yield return EndFrame();
        }

        action(1f);
    }

    public static void OverrideOnClickListeners(this PassiveButton passive, Action action, bool enabled = true)
    {
        if (!passive)
            return;

        passive.OnClick?.RemoveAllListeners();
        passive.OnClick = new();
        passive.OnClick.AddListener(action);
        passive.enabled = enabled;
    }

    public static void OverrideOnMouseOverListeners(this PassiveButton passive, Action action, bool enabled = true)
    {
        if (!passive)
            return;

        passive.OnMouseOver?.RemoveAllListeners();
        passive.OnMouseOver = new();
        passive.OnMouseOver.AddListener(action);
        passive.enabled = enabled;
    }

    public static void OverrideOnMouseOutListeners(this PassiveButton passive, Action action, bool enabled = true)
    {
        if (!passive)
            return;

        passive.OnMouseOut?.RemoveAllListeners();
        passive.OnMouseOut = new();
        passive.OnMouseOut.AddListener(action);
        passive.enabled = enabled;
    }

    public static bool HasTask(params TaskTypes[] types) => CustomPlayer.Local.myTasks.Any(x => types.Contains(x.TaskType));

    public static void WipeListeners(this PassiveButton passive)
    {
        passive.OnClick?.RemoveAllListeners();
        passive.OnMouseOut?.RemoveAllListeners();
        passive.OnMouseOver?.RemoveAllListeners();
        passive.OnClick = new();
        passive.OnMouseOut = new();
        passive.OnMouseOver = new();
    }

    public static T GetValue<T>(this PropertyInfo prop, object obj) => (T)prop.GetValue(obj);

    public static T GetValue<T>(this FieldInfo field, object obj) => (T)field.GetValue(obj);

    // public static bool IsAny<T>(this T value, params T[] values) => values.Any(x => Equals(x, value));

    public static Transform FindRecursive(this Transform self, string exactName) => self.FindRecursive(child => child.name == exactName);

    public static Transform FindRecursive(this Transform self, Func<Transform, bool> selector)
    {
        for (var i = 0; i < self.childCount; i++)
        {
            var child = self.GetChild(i);

            if (selector(child))
                return child;

            var finding = child.FindRecursive(selector);

            if (finding)
                return finding;
        }

        return null;
    }

    public static IEnumerable<T> GetAllComponents<T>(this Transform self) where T : Component
    {
        var result = new List<T>();
        var comp = self.GetComponent<T>();

        if (comp)
            result.Add(comp);

        for (var i = 0; i < self.childCount; i++)
            result.AddRange(self.GetChild(i).GetAllComponents<T>());

        return result;
    }

    public static IEnumerable<T> GetValuesFromTo<T>(T start, T end, Func<T, bool> predicate = null, bool startInclusive = true, bool endInclusive = true) where T : struct, Enum
    {
        var values = Enum.GetValues<T>();
        var startIndex = values.IndexOf(start);
        var endIndex = values.IndexOf(end);
        var result = values.Where(x => values.IndexOf(x).IsInRange(startIndex, endIndex, startInclusive, endInclusive));

        if (predicate != null)
            result = result.Where(predicate);

        return result;
    }

    public static IEnumerable<TResult> GetValuesFromToAndMorph<TValue, TResult>(TValue start, TValue end, Func<TValue, TResult> select, Func<TValue, bool> predicate = null, bool startInclusive
        = true, bool endInclusive = true) where TValue : struct, Enum => GetValuesFromTo(start, end, predicate, startInclusive, endInclusive).Select(select);

    public static void CustomDie(this PlayerControl player, DeathReasonEnum customReason, PlayerControl killer = null, DeathReason reason = DeathReason.Kill)
    {
        if (player.Data.IsDead && !player.IsPostmortal())
            return;

        killer ??= player;
        player.logger.Debug($"Player {player.PlayerId} dying to {killer.PlayerId} for reason {reason} and custom reason {customReason}");

        if (killer != player)
        {
            var prevKiller = MostRecentKiller;
            MostRecentKiller = killer.name;

            if (!KillCounts.TryGetValue(killer.PlayerId, out var count))
                count = 0;

            KillCounts[killer.PlayerId] = count + 1;

            if (killer.AmOwner)
            {
                CustomStatsManager.IncrementStat(CustomStatsManager.StatsKilled);
                CustomAchievementManager.UnlockAchievement("TasteForDeath");

                if (IsNullEmptyOrWhiteSpace(prevKiller))
                    CustomAchievementManager.UnlockAchievement("FirstBlood");
            }
        }

        GameData.LastDeathReason = reason;

        if (player.inMovingPlat && Ship().TryCast<FungleShipStatus>(out var fungle))
            fungle.Zipline.CancelZiplineUseForPlayer(player);

        FirstDead ??= player.name;
        player.cosmetics.AnimatePetMourning();
        player.FixMixedUpOutfit();
        player.Data.IsDead = true;
        player.clickKillCollider.enabled = false;
        player.gameObject.layer = LayerMask.NameToLayer("Ghost");
        player.cosmetics.SetNameMask(false);
        player.cosmetics.PettingHand.StopPetting();
        DefaultOutfit(player);

        player.Data.Role.OnDeath(reason);
        GameManager.Instance.OnPlayerDeath(player, false);

        if (player.AmOwner)
        {
            CustomStatsManager.IncrementStat(StringNames.StatsTimesMurdered);
            CustomAchievementManager.UnlockAchievement("Fatality");
            Chat().SetVisible(true);
            var hud = HUD();
            hud.ShadowQuad.gameObject.SetActive(false);
            player.AdjustLighting();
            AllPlayers().ForEach(x => x.cosmetics.ToggleNameVisible(GameManager.Instance.LogicOptions.GetShowCrewmateNames()));
            player.RpcSetScanner(false);
            hud.KillOverlay.ShowKillAnimation(killer.Data, player.Data);
            player.NameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
            var tracker = hud.roomTracker.text;
            var location = tracker.transform.localPosition.y != -3.25f ? tracker.text : "an unknown location";
            BodyLocations[player.PlayerId] = location;
            CallRpc(CustomRPC.Misc, MiscRPC.BodyLocation, player, location);

            if (Vent.currentVent)
                Vent.currentVent.Buttons?.ForEach(x => x.gameObject.SetActive(false));

            if (ActiveTask())
                ActiveTask().Close();

            if (MapBehaviourPatches.MapActive)
                Map().Close();

            if (!TutorialManager.InstanceExists)
            {
                StatsManager.Instance.LastGameStarted = Il2CppSystem.DateTime.MinValue;
                StatsManager.Instance.BanPoints--;
            }

            if (FirstDead == player.name)
            {
                CustomAchievementManager.UnlockAchievement("ParticipationTrophy");

                if (player.Is<Troll>())
                    CustomAchievementManager.UnlockAchievement("Martyrdom");
            }
        }

        if (player.walkingToVent)
        {
            player.inVent = false;
            Vent.currentVent = null;
            player.moveable = true;
            player.MyPhysics.StopAllCoroutines();
        }

        RecentlyKilled.Add(player.PlayerId);
        KilledPlayers.RemoveAll(x => x.PlayerId == player.PlayerId);
        KilledPlayers.Add(new(killer.PlayerId, player.PlayerId));
        SetPostmortals.BeginPostmortals(player, false);
        Pestilence.Infected.Remove(player.PlayerId);

        player.GetLayers().ForEach(x => x.OnDeath(reason, customReason, killer));
        killer.GetLayers().ForEach(x => x.OnKill(player));

        if (AmongUsClient.Instance.AmHost)
            CheckEndGame.CheckEnd();
    }

    public static IEnumerator CoPerformKill(KillAnimation __instance, PlayerControl source, PlayerControl target, DeathReasonEnum reason, bool lunge)
    {
        var cam = HUD().PlayerCam;
        var isParticipant = source.AmOwner || target.AmOwner;
        KillAnimation.SetMovement(source, false);
        KillAnimation.SetMovement(target, false);

        if (isParticipant)
        {
            CustomPlayer.Local.isKilling = true;
            source.isKilling = true;
        }

        var deadBody = UObject.Instantiate(GameManager.Instance.DeadBodyPrefab);
        deadBody.enabled = false;
        deadBody.ParentId = target.PlayerId;
        deadBody.AddComponent<DeadBodyHandler>();
        target.SetPlayerMaterialColors(deadBody.bloodSplatter);
        var vector = target.transform.position + __instance.BodyOffset;
        vector.z = vector.y / 1000f;
        deadBody.transform.position = vector;

        if (isParticipant)
        {
            cam.Locked = true;
            ConsoleJoystick.SetMode_Task();
            CustomPlayer.Local.MyPhysics.inputHandler.enabled = true;
        }

        target.CustomDie(reason, source);

        if (lunge)
        {
            yield return source.MyPhysics.Animations.CoPlayCustomAnimation(__instance.BlurAnim);
            source.NetTransform.SnapTo(target.transform.position);
        }

        source.MyPhysics.Animations.PlayIdleAnimation();
        KillAnimation.SetMovement(source, true);
        KillAnimation.SetMovement(target, true);
        deadBody.enabled = true;

        if (isParticipant)
        {
            cam.Locked = false;
            CustomPlayer.Local.isKilling = false;
            source.isKilling = false;
        }
    }

    public static string PadCenter(this string text, int length, char padChar = ' ')
    {
        if (text.Length >= length)
            return text;

        var left = (length - text.Length) / 2;
        var right = length - text.Length - left;
        return new string(padChar, left) + text + new string(padChar, right);
    }

    public static bool IsAny<T>(this T item, params T[] items) where T : UObject => items.Any(x => x == item);

    public static bool TryCast<T>(this Il2CppObjectBase obj, out T result) where T : Il2CppObjectBase => (result = obj.TryCast<T>()) != null;

    public static void AnimatePortal(PlayerControl player, float duration)
    {
        if (!player.HasDied())
        {
            player.moveable = false;
            player.NetTransform.Halt();
            player.MyPhysics.ResetMoveState();
            player.MyPhysics.ResetAnimState();
            player.MyPhysics.StopAllCoroutines();
        }

        var go = new GameObject($"PortalAnim{player.name}") { layer = LayerMask.NameToLayer("Players") };
        var anim = go.AddComponent<SpriteRenderer>();
        anim.sprite = PortalAnimation[0];
        anim.material = HatManager.Instance.PlayerMaterial;
        anim.transform.position = new(player.GetTruePosition().x, player.GetTruePosition().y + 0.35f, player.transform.position.z - 0.01f);
        anim.flipX = player.MyRend().flipX;
        anim.transform.localScale *= 0.89f * player.GetModifiedSize();

        if (IsSubmerged())
            go.AddSubmergedComponent("ElevatorMover");

        Coroutines.Start(PerformTimedAction(duration, p =>
        {
            if (Meeting())
                anim.sprite = PortalAnimation[0];
            else
            {
                anim.sprite = PortalAnimation[Mathf.Clamp((int)(p * PortalAnimation.Count), 0, PortalAnimation.Count - 1)];
                player.SetPlayerMaterialColors(anim);
            }

            if (p == 1)
            {
                anim.sprite = PortalAnimation[0];
                go.Destroy();
            }
        }));
    }

    // public static object TryCast(this Il2CppObjectBase self, Type type) => TryCastMethod.MakeGenericMethod(type).Invoke(self, null);

    // public static bool TryCast(this Il2CppObjectBase obj, Type type, out object result) => (result = obj.TryCast(type)) != null;
}