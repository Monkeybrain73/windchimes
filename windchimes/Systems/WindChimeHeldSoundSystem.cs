
namespace windchimes
{
    public class WindChimeHeldSoundSystem : ModSystem
    {
        private ICoreClientAPI capi;
        private ILoadedSound heldSound;
        private WindChime activeWindChime;

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide == EnumAppSide.Client;
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            capi = api;
            capi.Event.RegisterGameTickListener(OnClientTick, 100);
        }

        private void OnClientTick(float dt)
        {
            ItemSlot slot = capi.World.Player?.InventoryManager?.ActiveHotbarSlot;
            WindChime windChime = slot?.Itemstack?.Collectible as WindChime;

            if (windChime == null)
            {
                StopHeldSound();
                return;
            }

            if (activeWindChime != windChime)
            {
                StopHeldSound();

                activeWindChime = windChime;
                StartHeldSound(windChime);
            }

            if (heldSound == null || !heldSound.IsPlaying)
            {
                StartHeldSound(windChime);
            }

            UpdateVolume();
        }

        private void StartHeldSound(WindChime windChime)
        {
            AssetLocation soundLocation = GetHeldSoundLocation(windChime);

            if (soundLocation == null)
            {
                return;
            }

            heldSound = capi.World.LoadSound(new SoundParams()
                {
                    Location = soundLocation,
                    ShouldLoop = true,
                    DisposeOnFinish = false,

                    RelativePosition = true,
                    Position = new Vec3f(),

                    Volume = GameMath.Clamp(
                        Configs.CConfig.WindChimeHeldVolume,
                        0f,
                        1f
                    )
                }
            );

            heldSound?.Start();
        }

        private void UpdateVolume()
        {
            if (heldSound == null)
            {
                return;
            }

            heldSound.SetVolume(GameMath.Clamp(Configs.CConfig.WindChimeHeldVolume, 0f, 1f));
        }

        private void StopHeldSound()
        {
            if (heldSound != null)
            {
                heldSound.Stop();
                heldSound.Dispose();
                heldSound = null;
            }

            activeWindChime = null;
        }

        private AssetLocation GetHeldSoundLocation(WindChime windChime)
        {
            string soundPath = windChime.Attributes?["heldIdleSound"].AsString();

            if (string.IsNullOrWhiteSpace(soundPath))
            {
                DebugUtil.Verbose(
                    capi,
                    $"No held wind chime sound defined for {windChime.Code}",
                    DebugUtil.LogSide.Client
                );

                return null;
            }

            return AssetLocation.Create(soundPath, windChime.Code.Domain);
        }

        public override void Dispose()
        {
            StopHeldSound();

            base.Dispose();
        }
    }
}