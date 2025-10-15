namespace windchimes
{
    public class WindChime : BlockRainAmbient
    {

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
        }

        public override float GetAmbientSoundStrength(IWorldAccessor world, BlockPos pos)
        {
            var capi = world.Api as ICoreClientAPI;
            var player = capi?.World?.Player;
            if (player == null) return 0;

            string chimeType = GetChimeType(world, pos);

            float baseStrength = GetChimeVolumeMultiplier(chimeType) * Configs.CConfig.WindChimeMainVolumeMultiplier;

            var roomReg = world.Api.ModLoader.GetModSystem<RoomRegistry>();
            Room room = roomReg?.GetRoomForPosition(player.Entity.Pos.AsBlockPos);
            bool isValidLargeRoom = room != null && room.ExitCount == 0 && !room.IsSmallRoom;

            if (isValidLargeRoom)
            {
                baseStrength *= Configs.CConfig.WindChimeIndoorVolume;
                baseStrength = Math.Max(baseStrength, Configs.CConfig.WindChimeMinVolume);
                Core.DebugUtil.Verbose(world.Api,
                    $"Player is indoors in a large room, reducing wind chime volume to {baseStrength}");
            }

            double dist = player.Entity.Pos.AsBlockPos.DistanceTo(pos);
            float maxDist = Configs.CConfig.WindChimeMaxDistance;

            if (maxDist > 0)
            {
                float attenuation = 1f - (float)(dist / maxDist);
                attenuation = GameMath.Clamp(attenuation, 0f, 1f);
                attenuation = MathF.Pow(attenuation, Configs.CConfig.WindChimeFalloffExponent);

                baseStrength *= attenuation;

                Core.DebugUtil.Verbose(world.Api,
                    $"WindChime: type={chimeType} , dist={dist:0.0}, attn={attenuation:0.00}, vol={baseStrength:0.00}");
            }

            return baseStrength;
        }

        private string GetChimeType(IWorldAccessor world, BlockPos pos)
        {
            return world.BlockAccessor.GetBlock(pos)?.Variant["model"];
        }

        private float GetChimeVolumeMultiplier(string chimeType)
        {
            return chimeType switch
            {
                "bamboo" => Configs.CConfig.BambooChimeVolume,
                "wood" => Configs.CConfig.WoodChimeVolume,
                "brass" => Configs.CConfig.BrassChimeVolume,
                "copper" => Configs.CConfig.CopperChimeVolume,
                "crystal" => Configs.CConfig.CrystalChimeVolume,
                _ => 0.5f,
            };
        }
    }
}
