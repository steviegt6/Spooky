using Terraria;
using Terraria.ModLoader;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.Biomes;
using Spooky.Content.NPCs.Catacomb.Layer1;
using Spooky.Content.NPCs.Catacomb.Layer2;
using Spooky.Content.NPCs.SpookyBiome;
using Spooky.Content.NPCs.SpookyHell;
using Spooky.Content.Tiles.Catacomb;
using Spooky.Content.Tiles.Cemetery;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Core
{
    public class SpookyBiomeSpawns : GlobalNPC
    {
		//separate globalNPC for all of spooky mod's biome spawn pools so I can keep them more organized
		//since vanilla enemies spawning is annoying, I just made every single biome clear the spawn pool and then manually add all of the enemies
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
		{
			//spooky forest surface spawns
			if (spawnInfo.Player.InModBiome(ModContent.GetInstance<SpookyBiome>()))
			{
				pool.Clear();

				//day time spawns
				if (Main.dayTime)
				{
					//critters
					pool.Add(ModContent.NPCType<FlySmall>(), 2);
					pool.Add(ModContent.NPCType<FlyBig>(), 2);

					//dont spawn enemies in a town, but also allow enemy spawns in a town with the shadow candle
					if (!spawnInfo.PlayerInTown || (spawnInfo.PlayerInTown && spawnInfo.Player.ZoneShadowCandle))
					{
						pool.Add(ModContent.NPCType<PuttySlime1>(), 6);
						pool.Add(ModContent.NPCType<PuttySlime2>(), 6);
						pool.Add(ModContent.NPCType<PuttySlime3>(), 6);
						pool.Add(ModContent.NPCType<HoppingCandyBasket>(), 2);

                        if (Main.hardMode)
                        {
                            pool.Add(ModContent.NPCType<PuttyPumpkin>(), 2);
                        }
                    }
				}
				//night time spawns
				else
				{
					//critters
					pool.Add(ModContent.NPCType<TinyGhost1>(), 2);
					pool.Add(ModContent.NPCType<TinyGhost2>(), 2);
					pool.Add(ModContent.NPCType<TinyGhostBoof>(), 1.5f);
					pool.Add(ModContent.NPCType<TinyGhostRare>(), 0.5f);
					pool.Add(ModContent.NPCType<SpookyDance>(), 0.5f);

                    //dont spawn enemies in a town, but also allow enemy spawns in a town with the shadow candle
					if (!spawnInfo.PlayerInTown || (spawnInfo.PlayerInTown && spawnInfo.Player.ZoneShadowCandle))
					{
                        pool.Add(ModContent.NPCType<FluffBatSmall1>(), 3);
						pool.Add(ModContent.NPCType<FluffBatSmall2>(), 3);
						pool.Add(ModContent.NPCType<ZomboidThorn>(), 5);
						
						//spawn windchime zomboids during windy days
						if (Main.WindyEnoughForKiteDrops)
						{
							pool.Add(ModContent.NPCType<ZomboidWind>(), 3);
						}

						//do not spawn zomboid warlocks if one already exists
						if (!NPC.AnyNPCs(ModContent.NPCType<ZomboidWarlock>()))
						{
							pool.Add(ModContent.NPCType<ZomboidWarlock>(), 1);
						}
					}
				}
			}

			//spooky forest underground spawns
			if (spawnInfo.Player.InModBiome(ModContent.GetInstance<SpookyBiomeUg>()))
			{
				pool.Clear();

				//critters
				pool.Add(ModContent.NPCType<LittleSpider>(), 3);
				pool.Add(ModContent.NPCType<TinyMushroom>(), 2);

                if (spawnInfo.SpawnTileType == ModContent.TileType<MushroomMoss>())
                {
                    pool.Add(ModContent.NPCType<ShroomHopper>(), 4);
                }

                //dont spawn enemies in a town, but also allow enemy spawns in a town with the shadow candle
				if (!spawnInfo.PlayerInTown || (spawnInfo.PlayerInTown && spawnInfo.Player.ZoneShadowCandle))
				{
                    pool.Add(ModContent.NPCType<FluffBatBig1>(), 4);
                    pool.Add(ModContent.NPCType<FluffBatBig2>(), 4);
                    pool.Add(ModContent.NPCType<ZomboidFungus>(), 5);

                    //do not spawn zomboid warlocks if one already exists
					if (!NPC.AnyNPCs(ModContent.NPCType<ZomboidWarlock>()))
					{
						pool.Add(ModContent.NPCType<ZomboidWarlock>(), 1);
					}

                    //mushroom moss mini-biome spawns
                    if (spawnInfo.SpawnTileType == ModContent.TileType<MushroomMoss>())
                    {
                        pool.Add(ModContent.NPCType<Bungus>(), 3);
                        pool.Add(ModContent.NPCType<Chungus>(), 2);
                    }
                }
			}

			//cemetery spawns (will be done later as the enemies are added)
			if (spawnInfo.Player.InModBiome(ModContent.GetInstance<CemeteryBiome>()))
			{
				pool.Clear();

                //dont spawn enemies in a town, but also allow enemy spawns in a town with the shadow candle
				if (!spawnInfo.PlayerInTown || (spawnInfo.PlayerInTown && spawnInfo.Player.ZoneShadowCandle))
				{
                }
			}

			//catacomb first layer spawns
			if (spawnInfo.Player.InModBiome(ModContent.GetInstance<CatacombBiome>()))
			{
                pool.Clear();

				int[] CatacombLayer1Tiles = { ModContent.TileType<CatacombBrick1>(), 
				ModContent.TileType<CatacombFlooring>(), ModContent.TileType<CemeteryGrass>() };

				//do not allow catacomb enemies to spawn on non catacomb tiles
				if (CatacombLayer1Tiles.Contains(Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType))
				{
					pool.Add(ModContent.NPCType<Skeletoid1>(), 5);
					pool.Add(ModContent.NPCType<Skeletoid2>(), 5);
					pool.Add(ModContent.NPCType<Skeletoid3>(), 5);
					pool.Add(ModContent.NPCType<Skeletoid4>(), 5);
					pool.Add(ModContent.NPCType<SkeletoidBig>(), 2);
					pool.Add(ModContent.NPCType<RollingSkull1>(), 5);
					pool.Add(ModContent.NPCType<RollingSkull2>(), 5);
					pool.Add(ModContent.NPCType<RollingSkull3>(), 5);
					pool.Add(ModContent.NPCType<RollingSkull4>(), 1);
					pool.Add(ModContent.NPCType<GiantPutty>(), 5);
					pool.Add(ModContent.NPCType<BoneStackerBase>(), 4);
					pool.Add(ModContent.NPCType<ZomboidNecromancer>(), 3);
					pool.Add(ModContent.NPCType<ZomboidPyromancer>(), 3);
				}
			}

			//catacomb second layer spawns
			if (spawnInfo.Player.InModBiome(ModContent.GetInstance<CatacombBiome2>()))
			{ 
                pool.Clear();

                int[] CatacombLayer2Tiles = { ModContent.TileType<CatacombBrick2>(), 
				ModContent.TileType<GildedBrick>(), ModContent.TileType<CemeteryGrass>() };

				//do not allow catacomb enemies to spawn on non catacomb tiles
				if (CatacombLayer2Tiles.Contains(Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType))
				{
					pool.Add(ModContent.NPCType<FloatyFlower>(), 5);
					pool.Add(ModContent.NPCType<HoppingFlower>(), 5);
					pool.Add(ModContent.NPCType<RollFlower>(), 4);
					pool.Add(ModContent.NPCType<PlantTrap>(), 3);

					//do not spawn flower sentries if one already exists
					if (!NPC.AnyNPCs(ModContent.NPCType<FlowerSentry>()))
					{
						pool.Add(ModContent.NPCType<FlowerSentry>(), 1);
					}
				}
			}

			//eye valley spawns
            if (spawnInfo.Player.InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
			{ 
                pool.Clear();

				pool.Add(ModContent.NPCType<EyeBat>(), 5);
				pool.Add(ModContent.NPCType<MrHandy>(), 5);
				pool.Add(ModContent.NPCType<ManHole>(), 4);
				pool.Add(ModContent.NPCType<ManHoleBig>(), 4);
				pool.Add(ModContent.NPCType<Tortumor>(), 3);

				//do not spawn giant tortumors if one already exists
				if (!NPC.AnyNPCs(ModContent.NPCType<TortumorGiant>()))
                {
					//spawn more often in hardmode
                    if (Main.hardMode)
                    {
                        pool.Add(ModContent.NPCType<TortumorGiant>(), 1);
                    }
                    else
                    {
                        pool.Add(ModContent.NPCType<TortumorGiant>(), 0.5f);
                    }
                }
            }
        }
    }
}