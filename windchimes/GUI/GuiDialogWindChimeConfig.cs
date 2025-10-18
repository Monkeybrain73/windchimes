namespace windchimes
{
    public class GuiDialogWindChimeConfig : GuiDialog
    {
        private readonly Configs.ClientConfig clientConfig;
        private readonly Configs.ServerConfig serverConfig;

        private readonly Dictionary<string, float> sliderScale = new();

        public override string ToggleKeyCombinationCode => "windchimeconfig";

        public GuiDialogWindChimeConfig(ICoreClientAPI capi, Configs.ClientConfig clientConfig, Configs.ServerConfig serverConfig)
            : base(capi)
        {
            this.clientConfig = clientConfig;
            this.serverConfig = serverConfig;

            bool system = capi.ModLoader.IsModEnabled("configlib");

            if (system != true)
            {
                ComposeDialog();
            }

        }

        private void ComposeDialog()
        {
            int insetWidth = 270;
            int insetHeight = 580;
            int rowHeight = 32;

            ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog
                .WithAlignment(EnumDialogArea.CenterMiddle)
                .WithFixedAlignmentOffset(0, 0);

            ElementBounds insetBounds = ElementBounds.Fixed(0, GuiStyle.TitleBarHeight, insetWidth, insetHeight);
            ElementBounds scrollbarBounds = insetBounds.RightCopy().WithFixedWidth(20);

            ElementBounds clipBounds = insetBounds.ForkContainingChild(GuiStyle.HalfPadding, GuiStyle.HalfPadding, GuiStyle.HalfPadding, GuiStyle.HalfPadding);
            ElementBounds containerBounds = insetBounds.ForkContainingChild(GuiStyle.HalfPadding, GuiStyle.HalfPadding, GuiStyle.HalfPadding, GuiStyle.HalfPadding);
            ElementBounds containerRowBounds = ElementBounds.Fixed(2, 0, insetWidth, rowHeight);

            ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding).WithSizing(ElementSizing.FitToChildren).WithChildren(insetBounds, scrollbarBounds);

            SingleComposer = capi.Gui
                .CreateCompo("windchimeconfig", dialogBounds)
                .AddShadedDialogBG(bgBounds)
                .AddDialogTitleBar(Lang.Get("Wind Chimes Settings"), OnClose);

            double y = 40;
            double spacing = 32;

            SingleComposer.BeginChildElements();

            // Volume and distance sliders
            AddSlider(SingleComposer, "windchimes:windchime-main-volume", nameof(clientConfig.WindChimeMainVolumeMultiplier), clientConfig.WindChimeMainVolumeMultiplier, 0f, 4f, 0.01f, ref y);
            AddSlider(SingleComposer, "windchimes:windchime-bamboo-volume", nameof(clientConfig.BambooChimeVolume), clientConfig.BambooChimeVolume, 0f, 2f, 0.01f, ref y);
            AddSlider(SingleComposer, "windchimes:windchime-wood-volume", nameof(clientConfig.WoodChimeVolume), clientConfig.WoodChimeVolume, 0f, 2f, 0.01f, ref y);
            AddSlider(SingleComposer, "windchimes:windchime-brass-volume", nameof(clientConfig.BrassChimeVolume), clientConfig.BrassChimeVolume, 0f, 2f, 0.01f, ref y);
            AddSlider(SingleComposer, "windchimes:windchime-copper-volume", nameof(clientConfig.CopperChimeVolume), clientConfig.CopperChimeVolume, 0f, 2f, 0.01f, ref y);
            AddSlider(SingleComposer, "windchimes:windchime-crystal-volume", nameof(clientConfig.CrystalChimeVolume), clientConfig.CrystalChimeVolume, 0f, 2f, 0.01f, ref y);
            AddSlider(SingleComposer, "windchimes:windchime-min-volume", nameof(clientConfig.WindChimeMinVolume), clientConfig.WindChimeMinVolume, 0f, 0.5f, 0.01f, ref y);
            AddSlider(SingleComposer, "windchimes:windchime-indoor-volume", nameof(clientConfig.WindChimeIndoorVolume), clientConfig.WindChimeIndoorVolume, 0f, 1f, 0.01f, ref y);
            AddSlider(SingleComposer, "windchimes:windchime-max-distance", nameof(clientConfig.WindChimeMaxDistance), clientConfig.WindChimeMaxDistance, 4f, 64f, 0.5f, ref y);
            AddSlider(SingleComposer, "windchimes:windchime-falloff-exponent", nameof(clientConfig.WindChimeFalloffExponent), clientConfig.WindChimeFalloffExponent, 0.5f, 5f, 0.1f, ref y);

            y += spacing * 0.5;

            // Debug toggle
            ElementBounds toggleLabelBounds = ElementBounds.Fixed(20, y, 180, 25);
            ElementBounds toggleSwitchBounds = ElementBounds.Fixed(210, y - 4, 50, 30);

            SingleComposer.AddStaticText(Lang.Get("Debug Logging (server)"), CairoFont.WhiteSmallText(), toggleLabelBounds);
            SingleComposer.AddSwitch(OnDebugToggle, toggleSwitchBounds, "debugtoggle");
            SingleComposer.GetSwitch("debugtoggle").SetValue(serverConfig.EnableDebugLogging);
            y += spacing * 1.5;

            // Save/Cancel buttons
            SingleComposer.AddSmallButton(Lang.Get("Save"), OnSaveClicked, ElementBounds.Fixed(20, y, 120, 30))
                    .AddSmallButton(Lang.Get("Close"), OnCancelClicked, ElementBounds.Fixed(170, y, 120, 30));

            SingleComposer.EndChildElements();

            SingleComposer.Compose();
        }

        public override void OnGuiOpened()
        {
            base.OnGuiOpened();
            capi.Gui.RequestFocus(this);
        }

        private void AddSlider(GuiComposer SingleComposer, string label, string key, float value, float min, float max, float step, ref double y)
        {
            float scale = 1f / step;
            sliderScale[key] = scale;

            int intMin = (int)(min * scale);
            int intMax = (int)(max * scale);
            int intValue = (int)(value * scale);

            label = Lang.Get(label);

            ElementBounds labelBounds = ElementBounds.Fixed(15, y, 250, 26);
            ElementBounds sliderBounds = ElementBounds.Fixed(15, y + 20, 300, 20);
            y += 50;

            SingleComposer.AddStaticText(label, CairoFont.WhiteSmallText(), labelBounds);
            SingleComposer.AddSlider((val) => OnSliderChanged(key, val), sliderBounds, key);
            SingleComposer.GetSlider(key).SetValues(intValue, intMin, intMax, 1);
        }

        private bool OnSliderChanged(string key, float rawValue)
        {
            float scale = sliderScale[key];
            float value = rawValue / scale;

            switch (key)
            {
                case nameof(Configs.ClientConfig.WindChimeMainVolumeMultiplier): clientConfig.WindChimeMainVolumeMultiplier = value; break;
                case nameof(Configs.ClientConfig.BambooChimeVolume): clientConfig.BambooChimeVolume = value; break;
                case nameof(Configs.ClientConfig.WoodChimeVolume): clientConfig.WoodChimeVolume = value; break;
                case nameof(Configs.ClientConfig.BrassChimeVolume): clientConfig.BrassChimeVolume = value; break;
                case nameof(Configs.ClientConfig.CopperChimeVolume): clientConfig.CopperChimeVolume = value; break;
                case nameof(Configs.ClientConfig.CrystalChimeVolume): clientConfig.CrystalChimeVolume = value; break;
                case nameof(Configs.ClientConfig.WindChimeMinVolume): clientConfig.WindChimeMinVolume = value; break;
                case nameof(Configs.ClientConfig.WindChimeIndoorVolume): clientConfig.WindChimeIndoorVolume = value; break;
                case nameof(Configs.ClientConfig.WindChimeMaxDistance): clientConfig.WindChimeMaxDistance = value; break;
                case nameof(Configs.ClientConfig.WindChimeFalloffExponent): clientConfig.WindChimeFalloffExponent = value; break;
                default: break;
            }
            return true;
        }

        private void OnDebugToggle(bool on)
        {
            serverConfig.EnableDebugLogging = on;
        }

        private bool OnSaveClicked()
        {
            capi.StoreModConfig(clientConfig, Const.ConfigNameClient);
            capi.StoreModConfig(serverConfig, Const.ConfigNameServer);
            capi.ShowChatMessage(Lang.Get("WindChime settings saved!"));
            TryClose();
            return true;
        }

        private bool OnCancelClicked()
        {
            TryClose();
            return true;
        }

        private void OnClose() => TryClose();
    }
}
