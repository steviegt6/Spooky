using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.IO;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.Friendly;
using Spooky.Content.Tiles.SpiderCave;
using Spooky.Content.Tiles.SpiderCave.Ambient;
using Spooky.Content.Tiles.SpiderCave.Furniture;
using Spooky.Content.Tiles.SpiderCave.Tree;
using Spooky.Content.Tiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome.Ambient;

namespace Spooky.Content.Generation
{
    public class SpiderCave : ModSystem
    {
        static int initialStartPosX;
        static int startPosX;
        static int startPosY;

        private void PlaceSpiderCave(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.SpiderCave").Value;

            //biome position stuff
            initialStartPosX = GenVars.dungeonX;
            startPosX = GenVars.dungeonX;
            startPosY = Main.maxTilesY >= 1800 ? (Main.maxTilesY - (Main.maxTilesY / 3) - 30) : Main.maxTilesY / 2 + 100;

            //attempt to find a valid position for the biome to place in
            bool foundValidPosition = false;
            int attempts = 0;

            //the biomes initial position is the very center of the snow biome
            //this code basically looks for snow biome blocks, and if it finds any, keep moving the biome over until it is far enough away from the snow biome
            while (!foundValidPosition && attempts++ < 100000)
            {
                while (!CanPlaceBiome(startPosX, startPosY))
                {
                    startPosX += (initialStartPosX > (Main.maxTilesX / 2) ? -50 : 50);
                }
                if (CanPlaceBiome(startPosX, startPosY))
                {
                    foundValidPosition = true;
                }

                if (initialStartPosX < Main.maxTilesX / 2 && startPosX > Main.maxTilesX / 2)
                {
                    startPosX = Main.maxTilesX / 2;
                    foundValidPosition = true;
                }
                if (initialStartPosX > Main.maxTilesX / 2 && startPosX < Main.maxTilesX / 2)
                {
                    startPosX = Main.maxTilesX / 2;
                    foundValidPosition = true;
                }
            }

            int cavePerlinSeed = WorldGen.genRand.Next();

            Point origin = new Point(startPosX, startPosY);
            Vector2 center = origin.ToVector2() * 16f + new Vector2(8f);

            float angle = MathHelper.Pi * 0.15f;
            float otherAngle = MathHelper.PiOver2 - angle;

            int InitialSize = Main.maxTilesY >= 1800 ? 240 : 150;
            int biomeSize = InitialSize + (Main.maxTilesX / 180);
            float actualSize = biomeSize * 16f;
            float constant = actualSize * 2f / (float)Math.Sin(angle);

            float biomeSpacing = actualSize * (float)Math.Sin(otherAngle) / (float)Math.Sin(angle);
            int verticalRadius = (int)(constant / 16f);

            Vector2 biomeOffset = Vector2.UnitY * biomeSpacing;
            Vector2 biomeTop = center - biomeOffset;
            Vector2 biomeBottom = center + biomeOffset;

            //first place a bunch of spider caves as a barrier around the biome
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        float percent = dist / constant;
                        float blurPercent = 0.99f;

                        if (percent > blurPercent && Main.tile[X, Y - 20].WallType != WallID.SpiderUnsafe)
                        {
                            SpookyWorldMethods.PlaceCircle(X, Y, -1, WallID.SpiderUnsafe, WorldGen.genRand.Next(45, 65), false, false);
                        }
                    }
                }
            }

            //dig out all the caverns and place walls
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        float percent = dist / constant;
                        float blurPercent = 0.99f;

                        if (percent < blurPercent)
                        {
                            //clear absolutely everything before generating the caverns
                            Main.tile[X, Y].ClearEverything();

                            //generate perlin noise caves
                            float horizontalOffsetNoise = SpookyWorldMethods.PerlinNoise2D(X / 80f, Y / 80f, 5, unchecked(cavePerlinSeed + 1)) * 0.01f;
                            float cavePerlinValue = SpookyWorldMethods.PerlinNoise2D(X / 1450f, Y / 800f, 5, cavePerlinSeed) + 0.5f + horizontalOffsetNoise;
                            float cavePerlinValue2 = SpookyWorldMethods.PerlinNoise2D(X / 1450f, Y / 800f, 5, unchecked(cavePerlinSeed - 1)) + 0.5f;
                            float caveNoiseMap = (cavePerlinValue + cavePerlinValue2) * 0.5f;
                            float caveCreationThreshold = horizontalOffsetNoise * 3.5f + 0.235f;
                            float caveCreationWallThreshold = horizontalOffsetNoise * 3.5f + 0.335f;

                            //kill or place tiles depending on the noise map
                            if (caveNoiseMap * caveNoiseMap > caveCreationThreshold)
                            {
                                WorldGen.KillTile(X, Y);
                            }
                            else
                            {
                                WorldGen.PlaceTile(X, Y, (ushort)ModContent.TileType<DampSoil>());
                            }

                            //place a layer of grass walls around blocks
                            if (caveNoiseMap * caveNoiseMap <= caveCreationWallThreshold)
                            {
                                WorldGen.PlaceWall(X, Y, ModContent.WallType<DampSoilWall>());
                            }
                        }
                    }
                }
            }

            //clean out small floating chunks of blocks
            CleanOutSmallClumps();

            //place dithering around the edge of the biome
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        float percent = dist / constant;
                        float blurPercent = 0.99f;

                        if (percent >= blurPercent)
                        {
                            if (WorldGen.genRand.NextBool())
                            {
                                if (Main.tile[X, Y].HasTile)
                                {
                                    Main.tile[X, Y].TileType = (ushort)ModContent.TileType<DampSoil>();
                                }

                                if (Main.tile[X, Y].WallType > 0)
                                {
                                    Main.tile[X, Y].WallType = (ushort)ModContent.WallType<DampSoilWall>();
                                }
                            }
                        }
                    }
                }
            }

            //place clumps of stone
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        float percent = dist / constant;
                        float blurPercent = 0.95f;

                        if (percent < blurPercent)
                        {
                            //occasionally place large chunks of stone blocks
                            if (WorldGen.genRand.NextBool(1000) && Main.tile[X, Y].TileType == ModContent.TileType<DampSoil>())
                            {
                                WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(25, 35), WorldGen.genRand.Next(25, 35), ModContent.TileType<SpookyStone>(), false, 0f, 0f, true, true);
                            }
                        }
                    }
                }
            }

            //after the main biome is done, generate some clumps of web and leaves
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        float percent = dist / constant;
                        float blurPercent = 0.98f;

                        if (percent < blurPercent)
                        {
                            //place mounds of web blocks on the floor
                            if (WorldGen.genRand.NextBool(20) && Main.tile[X, Y].TileType == ModContent.TileType<DampSoil>() && !Main.tile[X, Y - 1].HasTile)
                            {
                                SpookyWorldMethods.PlaceMound(X, Y + 3, ModContent.TileType<WebBlock>(), WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(6, 10), false);
                            }

                            //place smaller chunks of web blocks on the ceiling
                            if (WorldGen.genRand.NextBool(45) && Main.tile[X, Y].TileType == ModContent.TileType<DampSoil>() && !Main.tile[X, Y + 1].HasTile)
                            {
                                SpookyWorldMethods.PlaceCircle(X, Y + 2, TileID.LivingMahoganyLeaves, 0, WorldGen.genRand.Next(2, 6), false, false);
                            }
                        }
                    }
                }
            }

            //some small last minute things, mainly clean up before ambient tiles are placed
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        //tile sloping
                        Tile.SmoothSlope(X, Y, true);

                        //remove any extra liquids that may still be in the biome
                        Main.tile[X, Y].LiquidAmount = 0;

                        //clean tiles that are sticking out (basically tiles only attached to one tile on one side)
                        bool OnlyRight = !Main.tile[X, Y - 1].HasTile && !Main.tile[X, Y + 1].HasTile && !Main.tile[X - 1, Y].HasTile;
                        bool OnlyLeft = !Main.tile[X, Y - 1].HasTile && !Main.tile[X, Y + 1].HasTile && !Main.tile[X + 1, Y].HasTile;
                        bool OnlyDown = !Main.tile[X, Y - 1].HasTile && !Main.tile[X - 1, Y].HasTile && !Main.tile[X + 1, Y].HasTile;
                        bool OnlyUp = !Main.tile[X, Y + 1].HasTile && !Main.tile[X - 1, Y].HasTile && !Main.tile[X + 1, Y].HasTile;

                        if (OnlyRight || OnlyLeft || OnlyDown || OnlyUp)
                        {
                            WorldGen.KillTile(X, Y);
                        }

                        //kill random single floating tiles
                        if (!Main.tile[X, Y - 1].HasTile && !Main.tile[X, Y + 1].HasTile && !Main.tile[X - 1, Y].HasTile && !Main.tile[X + 1, Y].HasTile)
                        {
                            WorldGen.KillTile(X, Y);
                        }

                        //kill random single floating walls
                        if (Main.tile[X, Y - 1].WallType <= 0 && Main.tile[X, Y + 1].WallType <= 0 && Main.tile[X - 1, Y].WallType <= 0 && Main.tile[X + 1, Y].WallType <= 0)
                        {
                            WorldGen.KillWall(X, Y);
                        }

						//kill random single floating walls
						if (Main.tile[X, Y].TileType == TileID.LivingMahoganyLeaves)
						{
							Tile tile = Main.tile[X, Y];

							tile.TileColor = PaintID.BrownPaint;
						}

                        //get rid of 1x2 tiles on the ground since it looks weird
                        if (Main.tile[X, Y].HasTile && Main.tile[X - 1, Y].HasTile && !Main.tile[X - 2, Y].HasTile && !Main.tile[X + 1, Y].HasTile)
                        {
                            WorldGen.KillTile(X, Y);
                            WorldGen.KillTile(X - 1, Y);
                        }

                        //get rid of single tiles on the ground since it looks weird
                        if (Main.tile[X, Y].HasTile && !Main.tile[X - 1, Y].HasTile && !Main.tile[X + 1, Y].HasTile)
                        {
                            WorldGen.KillTile(X, Y);
                        }
					}
                }
            }

            //place clumps of vanilla ores
            
            //determine which ores to place based on the opposite of what ores generate
            ushort OppositeTier1Ore = WorldGen.SavedOreTiers.Copper == TileID.Copper ? TileID.Tin : TileID.Copper;
            ushort OppositeTier2Ore = WorldGen.SavedOreTiers.Iron == TileID.Iron ? TileID.Lead : TileID.Iron;
            ushort OppositeTier3Ore = WorldGen.SavedOreTiers.Silver == TileID.Silver ? TileID.Tungsten : TileID.Silver;
            ushort OppositeTier4Ore = WorldGen.SavedOreTiers.Gold == TileID.Gold ? TileID.Platinum : TileID.Gold;

            for (int copper = 0; copper < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 5E-05); copper++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(8, 12), WorldGen.genRand.Next(8, 12), OppositeTier1Ore, false, 0f, 0f, false, true);
                }
            }

            for (int iron = 0; iron < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 4E-05); iron++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(6, 10), WorldGen.genRand.Next(6, 10), OppositeTier2Ore, false, 0f, 0f, false, true);
                }
            }

            for (int silver = 0; silver < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 3E-05); silver++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(4, 8), WorldGen.genRand.Next(4, 8), OppositeTier3Ore, false, 0f, 0f, false, true);
                }
            }

            for (int gold = 0; gold < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 2E-05); gold++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(4, 6), WorldGen.genRand.Next(4, 6), OppositeTier4Ore, false, 0f, 0f, false, true);
                }
            }

            //generate old hunter piece structures
            GenerateOldHunterPile(startPosX - (Main.maxTilesX / 42) + WorldGen.genRand.Next(0, 60), startPosY - (Main.maxTilesY / 18), "OldHunterHat", 12, 8);
            GenerateOldHunterPile(startPosX + (Main.maxTilesX / 42) - WorldGen.genRand.Next(0, 60), startPosY - (Main.maxTilesY / 18), "OldHunterSkull", 12, 8);
            GenerateOldHunterPile(startPosX - (Main.maxTilesX / 42) + WorldGen.genRand.Next(0, 60), startPosY + (Main.maxTilesY / 30), "OldHunterTorso", 12, 8);
            GenerateOldHunterPile(startPosX + (Main.maxTilesX / 42) - WorldGen.genRand.Next(0, 60), startPosY + (Main.maxTilesY / 30), "OldHunterLegs", 12, 8);

            //place giant web in the center of the biome
            Vector2 giantWebOrigin = new Vector2(origin.X - 40, origin.Y - 61);
			StructureHelper.API.Generator.GenerateStructure("Content/Structures/SpiderCave/GiantWebHouse", giantWebOrigin.ToPoint16(), Mod);

            Flags.SpiderGrottoCenter = new Vector2(origin.X * 16, origin.Y * 16);

            Flags.SpiderWebPosition = new Vector2(origin.X * 16, origin.Y * 16);
            int GiantWeb = NPC.NewNPC(null, (int)Flags.SpiderWebPosition.X, (int)Flags.SpiderWebPosition.Y, ModContent.NPCType<GiantWeb>());
            Main.npc[GiantWeb].position.X += 18;
            Main.npc[GiantWeb].position.Y += 1518;
            
            //generate structures
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) + 10; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        float percent = dist / constant;
                        float blurPercent = 0.95f;

                        if (percent < blurPercent)
                        {
                            //ground structures
                            if (Main.tile[X, Y].TileType == ModContent.TileType<DampSoil>() && Main.tile[X - 1, Y].TileType == ModContent.TileType<DampSoil>() &&
                            Main.tile[X - 2, Y].TileType == ModContent.TileType<DampSoil>() && Main.tile[X + 1, Y].TileType == ModContent.TileType<DampSoil>() &&
                            Main.tile[X + 2, Y].TileType == ModContent.TileType<DampSoil>() && !Main.tile[X, Y - 1].HasTile && !Main.tile[X, Y - 2].HasTile)
                            {
                                if (WorldGen.genRand.NextBool(7) && CanPlaceStructure(X, Y))
                                {
                                    switch (WorldGen.genRand.Next(12))
                                    {
                                        case 0:
                                        {
                                            Vector2 structureOrigin = new Vector2(X - 12, Y - 13);
                                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/SpiderCave/HouseSmall.shstruct", structureOrigin.ToPoint16(), Mod);
                                            break;
                                        }
                                        case 1:
                                        {
                                            Vector2 structureOrigin = new Vector2(X - 13, Y - 25);
											StructureHelper.API.Generator.GenerateStructure("Content/Structures/SpiderCave/HouseMedium.shstruct", structureOrigin.ToPoint16(), Mod);
                                            break;
                                        }
                                        case 2:
                                        {
                                            Vector2 structureOrigin = new Vector2(X - 17, Y - 23);
											StructureHelper.API.Generator.GenerateStructure("Content/Structures/SpiderCave/HouseLarge.shstruct", structureOrigin.ToPoint16(), Mod);
                                            break;
                                        }
                                        case 3:
                                        {
                                            Vector2 structureOrigin = new Vector2(X - 21, Y - 28);
											StructureHelper.API.Generator.GenerateStructure("Content/Structures/SpiderCave/HouseGiant.shstruct", structureOrigin.ToPoint16(), Mod);
                                            break;
                                        }
                                        case 4:
                                        {
                                            Vector2 structureOrigin = new Vector2(X - 12, Y - 14);
                                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/SpiderCave/GraveSmall.shstruct", structureOrigin.ToPoint16(), Mod);
                                            break;
                                        }
                                        case 5:
                                        {
                                            Vector2 structureOrigin = new Vector2(X - 15, Y - 16);
                                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/SpiderCave/GraveLarge.shstruct", structureOrigin.ToPoint16(), Mod);
                                            break;
                                        }
                                        case 6:
                                        {
                                            Vector2 structureOrigin = new Vector2(X - 18, Y - 22);
                                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/SpiderCave/RuinsSmall.shstruct", structureOrigin.ToPoint16(), Mod);
                                            break;
                                        }
                                        case 7:
                                        {
                                            Vector2 structureOrigin = new Vector2(X - 15, Y - 25);
                                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/SpiderCave/RuinsMedium.shstruct", structureOrigin.ToPoint16(), Mod);
                                            break;
                                        }
                                        case 8:
                                        {
                                            Vector2 structureOrigin = new Vector2(X - 12, Y - 23);
                                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/SpiderCave/RuinsLarge.shstruct", structureOrigin.ToPoint16(), Mod);
                                            break;
                                        }
                                        case 9:
                                        {
                                            Vector2 structureOrigin = new Vector2(X - 9, Y - 20);
                                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/SpiderCave/TowerSmall.shstruct", structureOrigin.ToPoint16(), Mod);
                                            break;
                                        }
                                        case 10:
                                        {
                                            Vector2 structureOrigin = new Vector2(X - 9, Y - 26);
                                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/SpiderCave/TowerLarge.shstruct", structureOrigin.ToPoint16(), Mod);
                                            break;
                                        }
                                        case 11:
                                        {
                                            Vector2 structureOrigin = new Vector2(X - 10, Y - 15);
                                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/SpiderCave/SmallShrine.shstruct", structureOrigin.ToPoint16(), Mod);
                                            break;
                                        }
                                    }
                                }
                            }

                            //ceiling structures
                            if (Main.tile[X, Y].TileType == ModContent.TileType<DampSoil>() && Main.tile[X - 1, Y].TileType == ModContent.TileType<DampSoil>() &&
                            Main.tile[X - 2, Y].TileType == ModContent.TileType<DampSoil>() && Main.tile[X + 1, Y].TileType == ModContent.TileType<DampSoil>() &&
                            Main.tile[X + 2, Y].TileType == ModContent.TileType<DampSoil>() && !Main.tile[X, Y + 1].HasTile && !Main.tile[X, Y + 2].HasTile)
                            {
                                if (WorldGen.genRand.NextBool(35) && CanPlaceStructure(X, Y))
                                {
                                    switch (WorldGen.genRand.Next(5))
                                    {
                                        case 0:
                                        {
                                            Vector2 structureOrigin = new Vector2(X - 7, Y - 4);
                                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/SpiderCave/HangingTowerSmall.shstruct", structureOrigin.ToPoint16(), Mod);
                                            break;
                                        }
                                        case 1:
                                        {
                                            Vector2 structureOrigin = new Vector2(X - 7, Y - 4);
                                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/SpiderCave/HangingTowerLarge.shstruct", structureOrigin.ToPoint16(), Mod);
                                            break;
                                        }
                                        case 2:
                                        {
                                            Vector2 structureOrigin = new Vector2(X - 18, Y - 8);
                                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/SpiderCave/HangingTowerGiant.shstruct", structureOrigin.ToPoint16(), Mod);
                                            break;
                                        }
                                        case 3:
                                        {
                                            Vector2 structureOrigin = new Vector2(X - 16, Y - 6);
                                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/SpiderCave/HangingLootWeb.shstruct", structureOrigin.ToPoint16(), Mod);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //spread grass after placing structures, but before placing ambient tiles
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        Tile tile = Main.tile[X, Y];
                        Tile tileAbove = Main.tile[X, Y - 1];
                        Tile tileBelow = Main.tile[X, Y + 1];
                        Tile tileLeft = Main.tile[X - 1, Y];
                        Tile tileRight = Main.tile[X + 1, Y];

                        if (tile.WallType == ModContent.WallType<DampSoilWall>() && (!tileAbove.HasTile || !tileBelow.HasTile || !tileLeft.HasTile || !tileRight.HasTile))
                        {
                            tile.WallType = (ushort)ModContent.WallType<DampGrassWall>();
                        }

                        //spread grass onto the dirt blocks throughout the biome
                        WorldGen.SpreadGrass(X, Y, ModContent.TileType<DampSoil>(), ModContent.TileType<DampGrass>(), false);
                    }
                }
            }

            //place ambient tiles
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        Tile tile = Main.tile[X, Y];
                        Tile tileAbove = Main.tile[X, Y - 1];
                        Tile tileBelow = Main.tile[X, Y + 1];

                        //place ceiling webs
                        if (tile.TileType == ModContent.TileType<WebBlock>())
                        {
                            ushort[] CeilingWebs = new ushort[] { (ushort)ModContent.TileType<CeilingWeb1>(), (ushort)ModContent.TileType<CeilingWeb2>() };

                            WorldGen.PlaceObject(X, Y + 1, WorldGen.genRand.Next(CeilingWebs));
                            WorldGen.PlaceObject(X, Y + 2, WorldGen.genRand.Next(CeilingWebs));
                        }

                        //place ambient tiles that can spawn on stone and grass
                        if (tile.TileType == ModContent.TileType<DampGrass>() || tile.TileType == ModContent.TileType<SpookyStone>())
                        {
                            if (WorldGen.genRand.NextBool(4))
                            {
                                WorldGen.PlacePot(X, Y - 1, 28, WorldGen.genRand.Next(19, 22));
                            }

                            //large hanging roots
                            if (WorldGen.genRand.NextBool())
                            {
                                ushort[] HangingRoots = new ushort[] { (ushort)ModContent.TileType<HangingRoots1>(), (ushort)ModContent.TileType<HangingRoots2>(), 
                                (ushort)ModContent.TileType<HangingRoots3>(), (ushort)ModContent.TileType<HangingRoots4>() };

                                WorldGen.PlaceObject(X, Y + 1, WorldGen.genRand.Next(HangingRoots));
                                WorldGen.PlaceObject(X, Y + 2, WorldGen.genRand.Next(HangingRoots));
                            }
                            
                            //light roots on the ground
                            if (WorldGen.genRand.NextBool(20))
                            {
                                ushort[] LightRoots = new ushort[] { (ushort)ModContent.TileType<LightRoot1>(), (ushort)ModContent.TileType<LightRoot2>(), 
                                (ushort)ModContent.TileType<LightRoot3>(), (ushort)ModContent.TileType<LightRoot4>() };

                                WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(LightRoots));
                            }

                            //light root on the ceiling
                            if (WorldGen.genRand.NextBool(15))
                            {
                                ushort[] LightRoots = new ushort[] { (ushort)ModContent.TileType<HangingLightRoot1>(), (ushort)ModContent.TileType<HangingLightRoot2>(), 
                                (ushort)ModContent.TileType<HangingLightRoot3>(), (ushort)ModContent.TileType<HangingLightRoot4>() };

                                WorldGen.PlaceObject(X, Y + 1, WorldGen.genRand.Next(LightRoots));
                            }
                        }

                        //grass only ambient tiles
                        if (tile.TileType == ModContent.TileType<DampGrass>())
                        {
                            if (WorldGen.genRand.NextBool(3) && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
                            {
                                CanGrowGiantRoot(X, Y, ModContent.TileType<GiantRoot>(), 6, 12);
                            }

                            if (WorldGen.genRand.NextBool(3) && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
                            {
                                CanGrowTallMushroom(X, Y, ModContent.TileType<TallMushroom>(), 2, 5);
                            }

                            //mushrooms
                            if (WorldGen.genRand.NextBool(5))
                            {
                                ushort[] Mushrooms = new ushort[] { (ushort)ModContent.TileType<MushroomBlue1>(), (ushort)ModContent.TileType<MushroomBlue2>(), 
                                (ushort)ModContent.TileType<MushroomGreen1>(), (ushort)ModContent.TileType<MushroomGreen2>(), 
                                (ushort)ModContent.TileType<MushroomRed1>(), (ushort)ModContent.TileType<MushroomRed2>(), 
                                (ushort)ModContent.TileType<MushroomYellow1>(), (ushort)ModContent.TileType<MushroomYellow2>() };

                                WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Mushrooms));
                            }

                            //grow weeds
                            if (WorldGen.genRand.NextBool() && !tileAbove.HasTile && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
                            {
                                WorldGen.PlaceTile(X, Y - 1, (ushort)ModContent.TileType<SpiderCaveWeeds>());
                                tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(16) * 18);
                                WorldGen.SquareTileFrame(X, Y + 1, true);
                                if (Main.netMode == NetmodeID.Server)
                                {
                                    NetMessage.SendTileSquare(-1, X, Y - 1, 1, TileChangeType.None);
                                }
                            }
                        }

                        //place rock piles on mossy stone
                        if (tile.TileType == ModContent.TileType<SpookyStone>())
                        {
                            if (WorldGen.genRand.NextBool(3))
                            {
                                WorldGen.PlaceObject(X, Y - 1, (ushort)ModContent.TileType<MossyRock>());           
                            }
                        }

                        //place spider webs on walls
                        if (WorldGen.genRand.NextBool(120) && Main.tile[X, Y].WallType == ModContent.WallType<DampSoilWall>() && !Main.tile[X, Y].HasTile)
                        {
                            ushort[] WallWebs = new ushort[] { (ushort)ModContent.TileType<WallWeb1>(), (ushort)ModContent.TileType<WallWeb2>() };

                            WorldGen.PlaceObject(X, Y, WorldGen.genRand.Next(WallWebs));
                        }
                    }
                }
            }

            //place vines
            for (int X = 20; X <= Main.maxTilesX - 20; X++)
            {
                for (int Y = 20; Y <= Main.maxTilesY - 20; Y++)
                {
                    if (Main.tile[X, Y].TileType == ModContent.TileType<DampGrass>() && !Main.tile[X, Y + 1].HasTile)
                    {
                        if (WorldGen.genRand.NextBool(2))
                        {
                            WorldGen.PlaceTile(X, Y + 1, (ushort)ModContent.TileType<DampVines>());
                        }
                    }
                    if (Main.tile[X, Y].TileType == ModContent.TileType<DampVines>())
                    {
                        int[] ValidTiles = { ModContent.TileType<DampGrass>() };

                        SpookyWorldMethods.PlaceVines(X, Y, ModContent.TileType<DampVines>(), ValidTiles);
                    }
                }
            }
        }

        private void DeleteAnnoyingTraps(GenerationProgress progress, GameConfiguration configuration)
        {
            Point origin = new Point(startPosX, startPosY);
            Vector2 center = origin.ToVector2() * 16f + new Vector2(8f);

            float angle = MathHelper.Pi * 0.15f;
            float otherAngle = MathHelper.PiOver2 - angle;

            int InitialSize = Main.maxTilesY >= 1800 ? 240 : 150;
            int biomeSize = InitialSize + (Main.maxTilesX / 180);
            float actualSize = biomeSize * 16f;
            float constant = actualSize * 2f / (float)Math.Sin(angle);

            float biomeSpacing = actualSize * (float)Math.Sin(otherAngle) / (float)Math.Sin(angle);
            int verticalRadius = (int)(constant / 16f);

            Vector2 biomeOffset = Vector2.UnitY * biomeSpacing;
            Vector2 biomeTop = center - biomeOffset;
            Vector2 biomeBottom = center + biomeOffset;

            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        WorldGen.KillWire(X, Y);
                        Main.tile[X, Y].LiquidAmount = 0;

                        if (Main.tile[X, Y].TileType == TileID.PressurePlates || Main.tile[X, Y].TileType == TileID.Traps || Main.tile[X, Y].TileType == TileID.GeyserTrap)
                        {
                            WorldGen.KillTile(X, Y);
                        }
                    }
                }
            }
        }

        public void GenerateOldHunterPile(int startX, int startY, string StructureFile, int offsetX, int offsetY)
        {
            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 100000)
            {
                while (!WorldGen.SolidTile(startX, startY) || (WorldGen.SolidTile(startX, startY) && WorldGen.SolidTile(startX, startY - 1)))
				{
					startY++;
				}
                if (WorldGen.SolidTile(startX, startY) && !WorldGen.SolidTile(startX, startY - 1))
                {
                    Vector2 origin = new Vector2(startX - offsetX, startY - offsetY);
                    StructureHelper.API.Generator.GenerateStructure("Content/Structures/SpiderCave/" + StructureFile + ".shstruct", origin.ToPoint16(), Mod);
                }

                placed = true;
            }
        }

        //method to clean up small clumps of tiles
        public static void CleanOutSmallClumps()
        {
            List<ushort> blockTileTypes = new()
            {
                (ushort)ModContent.TileType<DampGrass>(),
                (ushort)ModContent.TileType<DampSoil>(),
                (ushort)ModContent.TileType<SpookyStone>(),
                (ushort)ModContent.TileType<WebBlock>(),
            };
            
            void getAttachedPoints(int x, int y, List<Point> points)
            {
                Tile t = Main.tile[x, y];
				Point p = new(x, y);

				if (!WorldGen.InWorld(x, y))
				{
					t = new Tile();
				}
                
                if (!blockTileTypes.Contains(t.TileType) || !t.HasTile || points.Count > 90 || points.Contains(p))
                {
                    return;
                }

                points.Add(p);

                getAttachedPoints(x + 1, y, points);
                getAttachedPoints(x - 1, y, points);
                getAttachedPoints(x, y + 1, points);
                getAttachedPoints(x, y - 1, points);
            }

            Point origin = new Point(startPosX, startPosY);
            Vector2 center = origin.ToVector2() * 16f + new Vector2(8f);

            float angle = MathHelper.Pi * 0.15f;
            float otherAngle = MathHelper.PiOver2 - angle;

            int InitialSize = Main.maxTilesY >= 1800 ? 240 : 150;
            int biomeSize = InitialSize + (Main.maxTilesX / 180);
            float actualSize = biomeSize * 16f;
            float constant = actualSize * 2f / (float)Math.Sin(angle);

            float biomeSpacing = actualSize * (float)Math.Sin(otherAngle) / (float)Math.Sin(angle);
            int verticalRadius = (int)(constant / 16f);

            Vector2 biomeOffset = Vector2.UnitY * biomeSpacing;
            Vector2 biomeTop = center - biomeOffset;
            Vector2 biomeBottom = center + biomeOffset;

            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        List<Point> chunkPoints = new();
                        getAttachedPoints(X, Y, chunkPoints);

                        int cutoffLimit = 90;
                        if (chunkPoints.Count >= 1 && chunkPoints.Count < cutoffLimit)
                        {
                            foreach (Point p in chunkPoints)
                            {
                                WorldUtils.Gen(p, new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
                                {
                                    new Actions.ClearTile(true)
                                }));
                            }
                        }
                    }
                }
            }
        }

        //determine if theres no snow blocks nearby so the biome doesnt place in the snow biome
        public static bool CanPlaceBiome(int X, int Y)
        {
            Point origin = new Point(startPosX, startPosY);
            Vector2 center = origin.ToVector2() * 16f + new Vector2(8f);

            float angle = MathHelper.Pi * 0.15f;
            float otherAngle = MathHelper.PiOver2 - angle;

            //slightly bigger size than default to account for the area of spider cave walls around the biome
            int InitialSize = Main.maxTilesY >= 1800 ? 260 : 170;
            int biomeSize = InitialSize + (Main.maxTilesX / 180);
            float actualSize = biomeSize * 16f;
            float constant = actualSize * 2f / (float)Math.Sin(angle);

            float biomeSpacing = actualSize * (float)Math.Sin(otherAngle) / (float)Math.Sin(angle);
            int verticalRadius = (int)(constant / 16f);

            Vector2 biomeOffset = Vector2.UnitY * biomeSpacing;
            Vector2 biomeTop = center - biomeOffset;
            Vector2 biomeBottom = center + biomeOffset;

            for (int i = origin.X - biomeSize - 2; i <= origin.X + biomeSize + 2; i++)
            {
                for (int j = (int)(origin.Y - verticalRadius * 0.4f) - 3; j <= origin.Y + verticalRadius + 3; j++)
                {
                    if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        int[] InvalidTiles = { TileID.SnowBlock, TileID.IceBlock, TileID.Sandstone, TileID.JungleGrass, TileID.LihzahrdBrick };

                        if (WorldGen.InWorld(i, j) && (InvalidTiles.Contains(Main.tile[i, j].TileType) || Main.tileDungeon[Main.tile[i, j].TileType]))
                        {
                            return false;
                        }
                    }
                }
            }
    
            return true;
        }

        //determine if a structure can be placed at a set position
        public static bool CanPlaceStructure(int X, int Y)
        {
            //check a 70 by 70 square for other structures before placing
            for (int i = X - 35; i < X + 35; i++)
            {
                for (int j = Y - 35; j < Y + 35; j++)
                {
                    //if any mossy stone bricks are found in the square then another structure is too close, so dont allow it to place
                    if (Main.tile[i, j].HasTile && (Main.tile[i, j].TileType == ModContent.TileType<SpookyStoneBricks>() || Main.tile[i, j].TileType == ModContent.TileType<RootWood>()))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        //determine if a giant root can be grown on a set block
        public static bool CanGrowGiantRoot(int X, int Y, int tileType, int minSize, int maxSize)
        {
            //check a 10 by 10 square for other giant roots before placing
            for (int i = X - 5; i < X + 5; i++)
            {
                for (int j = Y - 5; j < Y + 5; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == tileType)
                    {
                        return false;
                    }
                }
            }

            GiantRoot.Grow(X, Y + 1, minSize, maxSize);

            return true;
        }

        //determine if a tall mushroom can grow
        public static bool CanGrowTallMushroom(int X, int Y, int tileType, int minSize, int maxSize)
        {
            //do not allow tall mushrooms to place if another one is too close
            for (int i = X - 10; i < X + 10; i++)
            {
                for (int j = Y - 10; j < Y + 10; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == tileType)
                    {
                        return false;
                    }
                }
            }

            //make sure the area is large enough for it to place in both horizontally and vertically
            for (int i = X - 2; i < X + 2; i++)
            {
                for (int j = Y - 12; j < Y - 2; j++)
                {
                    //only check for solid blocks, ambient objects dont matter
                    if (Main.tile[i, j].HasTile && Main.tileSolid[Main.tile[i, j].TileType])
                    {
                        return false;
                    }
                }
            }

            TallMushroom.Grow(X, Y - 1, minSize, maxSize);

            return true;
        }

        //method to make sure things only generate in the biome's circle
        public static bool CheckInsideOval(Point tile, Vector2 focus1, Vector2 focus2, float distanceConstant, Vector2 center, out float distance)
        {
            Vector2 point = tile.ToWorldCoordinates();
            float distY = center.Y - point.Y;

            //squish the circle vertically to create an oval shape
            point.Y -= distY * 2.5f;

            float distance1 = Vector2.Distance(point, focus1);
            float distance2 = Vector2.Distance(point, focus2);
            distance = distance1 + distance2;
            
            return distance <= distanceConstant;
        }

        //worldgenning tasks
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {  
            int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Hellforge"));
            if (GenIndex1 == -1) 
            {
                return;
            }

            tasks.Insert(GenIndex1 + 1, new PassLegacy("Spider Grotto", PlaceSpiderCave));

            int GenIndex2 = tasks.FindIndex(genpass => genpass.Name.Equals("Water Plants"));
            if (GenIndex2 == -1)
            {
                return;
            }

            tasks.Insert(GenIndex2 + 1, new PassLegacy("Spider Grotto Trap Removal", DeleteAnnoyingTraps));
        }

        public override void PostWorldGen()
		{
            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++) 
            {
				Chest chest = Main.chest[chestIndex];

				if (chest == null) 
                {
					continue;
				}

				if (WorldGen.InWorld(chest.x, chest.y))
				{
					Tile chestTile = Main.tile[chest.x, chest.y];

					int[] MainItems = new int[] { ItemID.BandofRegeneration, ItemID.AnkletoftheWind, ItemID.HermesBoots, ItemID.CloudinaBottle, ItemID.Aglet, ItemID.LuckyHorseshoe };

					if (chestTile.TileFrameX == 5 * 36 && MainItems.Contains(chest.item[0].type))
					{
						//potions
						int[] Potions1 = new int[] { ItemID.BattlePotion, ItemID.CratePotion, ItemID.EndurancePotion };

						//more potions
						int[] Potions2 = new int[] { ItemID.LuckPotion, ItemID.ShinePotion, ItemID.LifeforcePotion };

						//recorvery potions
						int[] RecoveryPotions = new int[] { ItemID.HealingPotion, ItemID.ManaPotion };

						//bars
						int[] Bars = new int[] { ItemID.GoldBar, ItemID.PlatinumBar };

						//bars
						chest.item[1].SetDefaults(WorldGen.genRand.Next(Bars));
						chest.item[1].stack = WorldGen.genRand.Next(5, 16);
                        //torches
                        chest.item[2].SetDefaults(ModContent.ItemType<SpiderBiomeTorchItem>());
						chest.item[2].stack = WorldGen.genRand.Next(3, 8);
						//potions
						chest.item[3].SetDefaults(WorldGen.genRand.Next(Potions1));
						chest.item[3].stack = WorldGen.genRand.Next(1, 3);
						//even more potions
						chest.item[4].SetDefaults(WorldGen.genRand.Next(Potions2));
						chest.item[4].stack = WorldGen.genRand.Next(1, 3);
						//recovery potions
						chest.item[5].SetDefaults(WorldGen.genRand.Next(RecoveryPotions));
						chest.item[5].stack = WorldGen.genRand.Next(3, 7);
						//goodie bags
						chest.item[6].SetDefaults(ItemID.GoodieBag);
						chest.item[6].stack = WorldGen.genRand.Next(1, 3);
						//gold coins
						chest.item[7].SetDefaults(ItemID.GoldCoin);
						chest.item[7].stack = WorldGen.genRand.Next(1, 3);
					}
				}
            }
        }
    }
}