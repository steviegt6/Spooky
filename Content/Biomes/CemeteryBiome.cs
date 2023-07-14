using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Backgrounds.Cemetery;
using Spooky.Content.Tiles.Water;

namespace Spooky.Content.Biomes
{
    public class CemeteryBiome : ModBiome
    {
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<CemeteryBG>();

        public override int Music
        {
            get
            {
                int music = Main.curMusic;

                if (!Main.bloodMoon)
                {
                    if (!Main.IsItStorming)
                    {
                        music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Cemetery");
                    }
                    else
                    {
                        music = MusicID.Monsoon;
                    }
                }
                else
                {
                    music = MusicID.Eerie;
                }

                return music;
            }
        }
        
        public override ModWaterStyle WaterStyle => ModContent.GetInstance<SpookyWaterStyle>();
       
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Spooky:Cemetery", isActive, player.Center);
        }

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/CemeteryBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override void OnInBiome(Player player)
        {
            //graveyard visuals
            player.ZoneGraveyard = true;

            Main.GraveyardVisualIntensity = 0.25f;

            if (Main.rand.NextBool(800))
            {
                Main.NewLightning();
            }
        }

        //conditions to be in the biome
        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = ModContent.GetInstance<TileCount>().cemeteryTiles >= 500; //was 500
            bool SurfaceCondition = player.ZoneSkyHeight || player.ZoneOverworldHeight;

            return BiomeCondition && SurfaceCondition;
        }
    }

    public class RaveyardBiome : ModBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyBiomeRain");
        
        public override ModWaterStyle WaterStyle => ModContent.GetInstance<LeanWaterStyle>();

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Spooky:Raveyard", isActive, player.Center);
        }

        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = player.InModBiome(ModContent.GetInstance<CemeteryBiome>()) && ModContent.GetInstance<TileCount>().raveyardTiles >= 8;

            bool SurfaceCondition = player.ZoneSkyHeight || player.ZoneOverworldHeight;

            return BiomeCondition && SurfaceCondition;
        }
    }
}