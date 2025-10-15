using Vintagestory.ServerMods.NoObf;

namespace windchimes
{
    public class ModSystemWindChimeClient : ModSystem
    {
        private ICoreClientAPI capi;
        private GuiDialogWindChimeConfig dialog;

        public Configs.ClientConfig Settings { get; set; } = new();

        public override void StartClientSide(ICoreClientAPI api)
        {
            capi = api;

            bool isLoaded = api.ModLoader.IsModEnabled("configlib");
            Core.DebugUtil.Verbose(api, "[WindChimes] Checking for ConfigLib mod... {0}", isLoaded);
            if (isLoaded == true)
            {
                Configs.TryLoadClientConfig(api);
                SubscribeToConfigChange(api);
                Core.DebugUtil.Verbose(api, "[WindChimes] ConfigLib mod found! Wind Chime default configuration GUI will be disabled.");
                return;
            }
            else
            {
                Core.DebugUtil.Verbose(api, "[WindChimes] ConfigLib mod not found! Wind Chime default configuration GUI will be enabled.");

                Configs.TryLoadClientConfig(api);
                api.Input.RegisterHotKey("windchimeconfig", "Open Wind Chime Config", GlKeys.O, HotkeyType.GUIOrOtherControls, false, true, false);
                api.Input.SetHotKeyHandler("windchimeconfig", OnHotkeyPressed);

                api.ChatCommands
                    .Create("windchimeconfig")
                    .WithDescription("Open Wind Chime configuration menu")
                    .HandleWith(_ => { OpenDialog(); return TextCommandResult.Success(); });

                base.StartClientSide(api);
            }

        }

        private bool OnHotkeyPressed(KeyCombination key)
        {
            OpenDialog();
            Core.DebugUtil.Verbose(capi, "Wind Chime config hotkey pressed");
            return true;
        }


        private void OpenDialog()
        {
            if (dialog == null || !dialog.IsOpened())
            {
                dialog = new GuiDialogWindChimeConfig(capi, Configs.CConfig, Configs.SConfig);
                dialog.TryOpen();
                dialog.Focus();
            }
        }

        private void SubscribeToConfigChange(ICoreAPI api)
        {
            ConfigLibModSystem system = api.ModLoader.GetModSystem<ConfigLibModSystem>();

            system.SettingChanged += (domain, config, setting) =>
            {
                if (domain != "windchimes") return;

                setting.AssignSettingValue(Settings);
            };

            system.ConfigsLoaded += () =>
            {
                system.GetConfig("windchimes")?.AssignSettingsValues(Settings);
            };
        }



        public override void Dispose()
        {
            if (dialog != null && dialog.IsOpened())
            {
                dialog.TryClose();
                dialog = null;
            }
            base.Dispose();
        }
    }
}
