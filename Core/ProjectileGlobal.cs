using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.Linq;

using Spooky.Content.Biomes;
using Spooky.Content.Projectiles.Sentient;
using Spooky.Content.Tiles.Cemetery;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Core
{
    public class ProjectileGlobal : GlobalProjectile
    {
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            //creepy candle makes magic projectiles inflict on fire
            if (Main.LocalPlayer.GetModPlayer<SpookyPlayer>().MagicCandle && projectile.DamageType == DamageClass.Magic)
            {
                if (Main.rand.NextBool(5))
                {
                    target.AddBuff(BuffID.OnFire, 120);
                }
            }
        }

        public override bool PreAI(Projectile projectile)
		{
            //disable gravestones in the catacombs to prevent graveyards from forming there
            int[] Gravestones = new int[] {ProjectileID.Tombstone, ProjectileID.GraveMarker, ProjectileID.CrossGraveMarker,
            ProjectileID.Headstone, ProjectileID.Gravestone, ProjectileID.Obelisk, ProjectileID.RichGravestone1, ProjectileID.RichGravestone2,
            ProjectileID.RichGravestone3, ProjectileID.RichGravestone4, ProjectileID.RichGravestone5 };

			if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<CatacombBiome>()))
            {
                if (Gravestones.Contains(projectile.type))
                {
                    projectile.active = false;
                    return false;
                }
			}

            //convert spooky forest tiles into purity when sprayed with green solution
            if (projectile.type == ProjectileID.PureSpray)
            {
                ConvertSpookyIntoPurity((int)(projectile.position.X + (projectile.width * 0.5f)) / 16, (int)(projectile.position.Y + (projectile.height * 0.5f)) / 16, 2);
            }

            //dont allow fishing in the blood lake in the valley of eyes, unless you have the goblin shark rod
            if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
            {
                if (projectile.aiStyle == ProjAIStyleID.Bobber && projectile.wet && projectile.type != ModContent.ProjectileType<SentientChumCasterBobber>())
                {
                    projectile.Kill();
                }
            }

			return base.PreAI(projectile);
		}

        public override bool PreKill(Projectile projectile, int timeLeft)
        {
            //make the world globe change the spooky forest backgrounds
            if (projectile.type == ProjectileID.WorldGlobe && (Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpookyBiome>()) || 
            Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpookyBiomeUg>())))
            {
                if (!Flags.SpookyBackgroundAlt)
                {
                    Flags.SpookyBackgroundAlt = true;
                }
                else
                { 
                    Flags.SpookyBackgroundAlt = false;
                }
                
                NetMessage.SendData(MessageID.WorldData);
            }

            return true;
        }

        private static void ConvertSpookyIntoPurity(int i, int j, int size = 4)
        {
            for (int k = i - size; k <= i + size; k++)
            {
                for (int l = j - size; l <= j + size; l++)
                {
                    if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size)))
                    {

                        //replace spooky grasses with regular grass
                        int[] GrassReplace = { ModContent.TileType<SpookyGrass>(), ModContent.TileType<SpookyGrassGreen>(), ModContent.TileType<CemeteryGrass>() };

                        if (GrassReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Grass;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky dirt with dirt 
                        int[] DirtReplace = { ModContent.TileType<SpookyDirt>(), ModContent.TileType<CemeteryDirt>() };

                        if (DirtReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Dirt;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky stone with stone
                        int[] StoneReplace = { ModContent.TileType<SpookyStone>(), ModContent.TileType<CemeteryStone>() };

                        if (StoneReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Stone;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky grass walls with grass walls
                        int[] GrassWallReplace = { ModContent.WallType<SpookyGrassWall>(), ModContent.WallType<CemeteryGrassWall>() };

                        if (GrassWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.GrassUnsafe;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }
                    }
                }
            }
        }
    }
}
