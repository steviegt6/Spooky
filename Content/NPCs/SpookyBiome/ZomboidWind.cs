using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;

using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class ZomboidWind : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Zomboid");
            Main.npcFrameCount[NPC.type] = 6;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 55;
            NPC.damage = 15;
            NPC.defense = 5;
            NPC.width = 46;
			NPC.height = 46;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.75f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 3;
			AIType = NPCID.GoblinScout;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("A zomboid that has been impaled by a large metal windchime, yet somehow still walks as if it's fine."),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (!spawnInfo.Invasion && Main.invasionType == 0 && !Main.pumpkinMoon && !Main.snowMoon && !Main.eclipse &&
            !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust))
            {
                if (player.InModBiome(ModContent.GetInstance<Biomes.SpookyBiome>()) && !Main.dayTime && Main.WindyEnoughForKiteDrops)
                {
                    return 8f;
                }
            }

            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {   
            NPC.frameCounter += 1;
            //running animation
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            //jumping frame
            if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0)
            {
                NPC.frame.Y = 5 * frameHeight;
            }
        }
        
        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrankenMarshmallow>(), 100));
        }

        public override void HitEffect(int hitDirection, double damage) 
        {
            //dont run on multiplayer
			if (Main.netMode == NetmodeID.Server) 
            {
				return;
			}

			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidWindGore" + numGores).Type);
                }
            }
        }
    }
}