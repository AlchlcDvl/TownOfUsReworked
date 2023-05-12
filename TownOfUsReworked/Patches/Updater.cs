using Twitch;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class ModUpdaterButton
    {
        public static void Prefix(MainMenuManager __instance)
        {
            //Check if there's a ToU-Rew update
            ModUpdater.LaunchUpdater();

            if (ModUpdater.hasREWUpdate)
            {
                //If there's an update, create and show the update button
                var template = GameObject.Find("ExitGameButton");

                if (template != null)
                {
                    var touButton = UObject.Instantiate(template, null);
                    touButton.transform.localPosition = new(touButton.transform.localPosition.x, touButton.transform.localPosition.y + 0.6f, touButton.transform.localPosition.z);
                    var passiveTOUButton = touButton.GetComponent<PassiveButton>();
                    SpriteRenderer touButtonSprite = touButton.GetComponent<SpriteRenderer>();
                    passiveTOUButton.OnClick.RemoveAllListeners();
                    touButtonSprite.sprite = AssetManager.GetSprite("UpdateToUButton");

                    //Add onClick event to run the update on button click
                    passiveTOUButton.OnClick.AddListener((Action)(() =>
                    {
                        ModUpdater.ExecuteUpdate("TOU");
                        touButton.SetActive(false);
                    }));

                    //Set button text
                    var text = touButton.transform.GetChild(0).GetComponent<TMP_Text>();
                    __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(_ => text.SetText(""))));
                    //Set popup stuff
                    var man = TwitchManager.Instance;
                    ModUpdater.InfoPopup = UObject.Instantiate(man.TwitchPopup);
                    ModUpdater.InfoPopup.TextAreaTMP.fontSize *= 0.7f;
                    ModUpdater.InfoPopup.TextAreaTMP.enableAutoSizing = false;
                }
            }

            if (ModUpdater.hasSubmergedUpdate)
            {
                //If there's an update, create and show the update button
                var template = GameObject.Find("ExitGameButton");

                if (template != null)
                {
                    var submergedButton = UObject.Instantiate(template, null);
                    submergedButton.transform.localPosition = new(submergedButton.transform.localPosition.x, submergedButton.transform.localPosition.y + 1.2f,
                        submergedButton.transform.localPosition.z);
                    var passiveSubmergedButton = submergedButton.GetComponent<PassiveButton>();
                    SpriteRenderer submergedButtonSprite = submergedButton.GetComponent<SpriteRenderer>();
                    passiveSubmergedButton.OnClick.RemoveAllListeners();
                    submergedButtonSprite.sprite = AssetManager.GetSprite("UpdateSubmergedButton");

                    //Add onClick event to run the update on button click
                    passiveSubmergedButton.OnClick.AddListener((Action)(() =>
                    {
                        ModUpdater.ExecuteUpdate("SUB");
                        submergedButton.SetActive(false);
                    }));

                    //Set button text
                    var text = submergedButton.transform.GetChild(0).GetComponent<TMP_Text>();
                    __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(_ => text.SetText(""))));
                    //Set popup stuff
                    var man = TwitchManager.Instance;
                    ModUpdater.InfoPopup = UObject.Instantiate(man.TwitchPopup);
                    ModUpdater.InfoPopup.TextAreaTMP.fontSize *= 0.7f;
                    ModUpdater.InfoPopup.TextAreaTMP.enableAutoSizing = false;
                }
            }
        }
    }
}