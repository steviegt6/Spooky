using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Tiles.Catacomb;

namespace Spooky.Content.Biomes
{
    public class CatacombBiome : ModBiome
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Creepy Catacombs");
        }

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Catacombs");
       
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/CatacombBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;


        public override void OnInBiome(Player player)
        {
            if (!Flags.downedBigBone)
            {
                player.AddBuff(ModContent.BuffType<CatacombDebuff>(), 2);
            }
        }

        public override bool IsBiomeActive(Player player)
        {
            int PlayerX = (int)player.Center.X / 16;
            int PlayerY = (int)player.Center.Y / 16;

            bool BiomeCondition = Main.tile[PlayerX, PlayerY].WallType == ModContent.WallType<CatacombBrickWall>() || 
            Main.tile[PlayerX, PlayerY].WallType == ModContent.WallType<CatacombBrickWall2>();

            return BiomeCondition;
        }
    }
}