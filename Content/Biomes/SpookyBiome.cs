using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Backgrounds.SpookyBiome;
using Spooky.Content.Gores.Misc;
using Spooky.Content.Tiles.Water;

namespace Spooky.Content.Biomes
{
    public class SpookyBiome : ModBiome
    {
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => Flags.SpookyBackgroundAlt ? ModContent.GetInstance<SpookyForestBGAlt>() : ModContent.GetInstance<SpookyForestBG>();

        //for whatever reason spooky mod underground backgrounds just break underground backgrounds
        //this will be disabled until it gets fixed or something
        //public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => Flags.SpookyBackgroundAlt ? ModContent.GetInstance<SpookyUndergroundBackgroundStyleAlt>() : ModContent.GetInstance<SpookyUndergroundBackgroundStyle>();

        public override int Music
        {
            get
            {
                int music = Main.curMusic;

                if (Main.raining)
                {
                    music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyBiomeRain");
                }
                else
                {
                    if (Main.dayTime)
                    {
                        music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyBiomeDay");
                    }
                    else
                    { 
                        if (!Main.bloodMoon)
                        {
                            music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyBiomeNight");
                        }
                        else
                        {
                            music = MusicID.Eerie;
                        }
                    }
                }
                
                return music;
            }
        }

        public override ModWaterStyle WaterStyle => ModContent.GetInstance<SpookyWaterStyle>();
       
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        
        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Spooky:HalloweenSky", isActive, player.Center);
        }

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/SpookyBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override void OnInBiome(Player player)
        {
            //spawn falling leaves while in the spooky forest
            int[] Leaves = new int[] { ModContent.GoreType<LeafGreen>(), ModContent.GoreType<LeafOrange>(), ModContent.GoreType<LeafRed>() };

            if (Main.rand.NextBool(40) && player.ZoneOverworldHeight)
            { 
                float Scale = Main.rand.NextFloat(1f, 1.2f);
                int SpawnX = (int)Main.screenPosition.X - 100;
                int SpawnY = (int)Main.screenPosition.Y + Main.rand.Next(-100, Main.screenHeight);
                int LeafGore = Gore.NewGore(null, new Vector2(SpawnX, SpawnY), Vector2.Zero, Leaves[Main.rand.Next(3)], Scale);
                Main.gore[LeafGore].rotation = 0f;
                Main.gore[LeafGore].velocity.X = Main.rand.NextFloat(0.5f, 3.5f);
                Main.gore[LeafGore].velocity.Y = Main.rand.NextFloat(0.5f, 1.2f);
            }

            if (Main.rand.NextBool(40) && player.ZoneOverworldHeight)
            {
                float Scale = Main.rand.NextFloat(1f, 1.2f);
                int SpawnX = (int)Main.screenPosition.X + Main.screenWidth + 100;
                int SpawnY = (int)Main.screenPosition.Y + Main.rand.Next(-100, Main.screenHeight);
                int LeafGore = Gore.NewGore(null, new Vector2(SpawnX, SpawnY), Vector2.Zero, Leaves[Main.rand.Next(3)], Scale);
                Main.gore[LeafGore].rotation = 0f;
                Main.gore[LeafGore].velocity.X = Main.rand.NextFloat(-0.5f, -3.5f);
                Main.gore[LeafGore].velocity.Y = Main.rand.NextFloat(0.5f, 1.2f);
            }
        }

        //conditions to be in the biome
        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = ModContent.GetInstance<TileCount>().spookyTiles >= 500;
            bool SurfaceCondition = player.ZoneSkyHeight || player.ZoneOverworldHeight;

            return BiomeCondition && SurfaceCondition;
        }
    }
}