
[assembly: ModInfo(Const.AppId,
                    Authors = new string[] { Const.AppAuthor },
                    Description = "Add decorative whind chimes",
                    Version = Const.AppVersion)]

namespace windchimes
{
    public sealed class Core : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            if (api.Side == EnumAppSide.Server)
            {
                Configs.TryLoadServerConfig(api as ICoreServerAPI);
            }
            if (api.Side == EnumAppSide.Client)
            {
                Configs.TryLoadClientConfig(api as ICoreClientAPI);
            }

            api.RegisterBlockClass("WindChime", typeof(WindChime));

            api.Logger.Event($"[{Const.AppName}] mod started");

        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            base.StartServerSide(api);
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
