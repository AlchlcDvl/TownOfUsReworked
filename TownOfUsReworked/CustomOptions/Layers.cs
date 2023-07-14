using static TownOfUsReworked.Languages.Language;
namespace TownOfUsReworked.CustomOptions
{
    public class CustomLayersOption : CustomOption
    {
        public string Layer;
        private int CachedCount;
        private int CachedChance;
        //private List<CustomOption> RoleOptions = new();
        public static readonly List<CustomOption> AllRoleOptions = new();

        public CustomLayersOption(int id, MultiMenu menu, string name, string layer = "") : base(id, menu, name, CustomOptionType.Layers, 0)
        {
            Layer = layer; //This will be used later but we'll see how it goes
            Format = (val, otherVal) => $"{val}% (x{otherVal})";
        }

        public override void OptionCreated()
        {
            base.OptionCreated();
            var role = Setting.Cast<RoleOptionSetting>();
            role.TitleText.text = GetString(Name);
            role.RoleMaxCount = 15;
            role.ChanceText.text = "0%";
            role.CountText.text = "0";
            role.Role = null;
            role.RoleChance = 0;
        }

        public void ShowAdvancedOptions()
        {
            /*ShowRoleDetails();
            var tab = GameSettingMenu.Instance.RolesSettings.AllAdvancedSettingTabs[0];
            GameSettingMenu.Instance.RolesSettings.RoleChancesSettings.SetActive(false);
            GameSettingMenu.Instance.RolesSettings.AdvancedRolesSettings.SetActive(true);
            GameSettingMenu.Instance.RolesSettings.RefreshChildren();
            ControllerManager.Instance.CurrentUiState.BackButton = GameSettingMenu.Instance.AdvancedSettingsBackButton;*/
        }

        /*//I'll get to these later
        public void AddOptions(params CustomOption[] list)
        {
            RoleOptions = list.ToList();
            AllRoleOptions.AddRange(RoleOptions);
        }

        public void CloseAdvancedSettings()
        {
            ShowRoleDetails();
            GameSettingMenu.Instance.RolesSettings.CloseAdvancedSettings();
            ControllerManager.Instance.CurrentUiState.BackButton = GameSettingMenu.Instance.BackButton;
        }*/

        public int GetChance() => (int)Value;

        public int GetCount() => (int)OtherValue;

        public void IncreaseCount()
        {
            if ((GetCount() + 1 > 15 && ConstantVariables.IsCustom) || (GetCount() + 1 > 1 && !ConstantVariables.IsCustom))
                Set(Value, 0);
            else
                Set(Value, GetCount() + 1);

            if (GetChance() == 0 && GetCount() > 0)
                Set(CachedChance == 0 ? 5 : CachedChance, OtherValue);
            else if (GetCount() == 0 && GetChance() > 0)
            {
                CachedChance = GetChance();
                Set(0, 0);
            }

            //ShowRoleDetails();
        }

        public void DecreaseCount()
        {
            if (GetCount() - 1 < 0)
                Set(Value, ConstantVariables.IsCustom ? 15 : 1);
            else
                Set(Value, GetCount() - 1);

            if (GetChance() == 0 && GetCount() > 0)
                Set(CachedChance == 0 ? 5 : CachedChance, OtherValue);
            else if (GetCount() == 0 && GetChance() > 0)
            {
                CachedChance = GetChance();
                Set(0, 0);
            }

            //ShowRoleDetails();
        }

        public void IncreaseChance()
        {
            var chance = GetChance();
            var count = GetCount();
            var increment = Input.GetKeyInt(KeyCode.LeftShift) ? 5 : 10;

            if (chance + increment > 100)
                chance = 0;
            else
                chance += increment;

            if (chance == 0 && count > 0)
            {
                CachedCount = count;
                count = 0;
            }
            else if (count == 0 && chance > 0)
                count = CachedCount == 0 || (!ConstantVariables.IsCustom && CachedCount > 1) ? 1 : CachedCount;

            //ShowRoleDetails();
            Set(chance, count);
        }

        public void DecreaseChance()
        {
            var chance = GetChance();
            var count = GetCount();
            var decrement = Input.GetKeyInt(KeyCode.LeftShift) ? 5 : 10;

            if (chance - decrement < 0)
                chance = 100;
            else
                chance -= decrement;

            if (chance == 0 && count > 0)
            {
                CachedCount = count;
                count = 0;
            }
            else if (count == 0 && chance > 0)
                count = CachedCount == 0 || (!ConstantVariables.IsCustom && CachedCount > 1) ? 1 : CachedCount;

            //ShowRoleDetails();
            Set(chance, count);
        }

        public void ShowRoleDetails()
        {
            //This will be worked on later, when I figure out why the custom options aren't spawning in the Role menu instead
            /*var layer = Info.AllInfo.Find(x => x.Name == Layer || x.Short == Layer);
            
            if (layer == null)
                return;

            GameSettingMenu.Instance.RoleName.text = layer.Name;
            GameSettingMenu.Instance.RoleBlurb.text = layer.Description;
            GameSettingMenu.Instance.RoleIcon.sprite = AssetManager.GetSprite(Layer == layer.Short ? layer.Name : Layer);*/
        }
    }
}