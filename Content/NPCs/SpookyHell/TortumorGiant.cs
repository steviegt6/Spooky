using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Content.Items.Food;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.NPCs.SpookyHell.Projectiles;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class TortumorGiant : ModNPC
    {
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        public static readonly SoundStyle ScreechSound = new("Spooky/Content/Sounds/SpookyHell/TumorScreech2", SoundType.Sound);

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Giant Tortumor");
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 800;
            NPC.damage = 65;
            NPC.defense = 20;
            NPC.width = 90;
            NPC.height = 92;
            NPC.npcSlots = 5f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 50, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath22;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("If regular tortumors weren't gross enough, these mutated blobs of flesh can fly around, making smaller clones of themselves to attack it's prey."),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/TortumorGiantGlow").Value;

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

            if (player.InModBiome(ModContent.GetInstance<Biomes.SpookyHellBiome>()) && !player.InModBiome(ModContent.GetInstance<Biomes.EggEventBiome>()) && 
            !NPC.AnyNPCs(ModContent.NPCType<TortumorGiant>()))
            {
                //spawn more often in hardmode
                if (Main.hardMode)
                {
                    return 5f;
                }
                else
                {
                    return 2f;
                }
            }
            
            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            //regular anims
            if (NPC.ai[0] <= 480)
            {
                NPC.frameCounter += 1;

                if (NPC.frameCounter > 7)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }

            //screaming animation
            if (NPC.ai[0] == 480)
            {
                NPC.frame.Y = 4 * frameHeight;
            }
            if (NPC.ai[0] > 480)
            {
                NPC.frameCounter += 1;

                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
			Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            NPC.rotation = NPC.velocity.X * 0.04f;

            int Damage = Main.expertMode ? 35 : 50;
			
			NPC.ai[0]++;  

            if (NPC.ai[0] <= 480)
            {
                int MaxSpeed = 30;

                //flies to players X position
                if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -MaxSpeed - 8) 
                {
                    MoveSpeedX--;
                }
                else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= MaxSpeed + 8)
                {
                    MoveSpeedX++;
                }

                NPC.velocity.X = MoveSpeedX * 0.1f;
                
                //flies to players Y position
                if (NPC.Center.Y >= player.Center.Y - 60f && MoveSpeedY >= -MaxSpeed)
                {
                    MoveSpeedY--;
                }
                else if (NPC.Center.Y <= player.Center.Y - 60f && MoveSpeedY <= MaxSpeed)
                {
                    MoveSpeedY++;
                }

                NPC.velocity.Y = MoveSpeedY * 0.1f;
            }

            if (NPC.ai[0] >= 480)
            {
                if (NPC.ai[0] == 480)
                {
                    SoundEngine.PlaySound(ScreechSound, NPC.Center);
                }

                NPC.velocity *= 0.95f;

                //only summon tortumors if no other tortumors exist
                if (!NPC.AnyNPCs(ModContent.NPCType<Tortumor>()))
                {
                    if (NPC.ai[0] == 555)
                    {
                        for (int numSpawns = 0; numSpawns < 2; numSpawns++)
                        {
                            int TumorSummon = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + Main.rand.Next(-150, 150), 
                            (int)NPC.Center.Y + Main.rand.Next(-150, 150), ModContent.NPCType<Tortumor>());

                            //set tortumor ai to -2 so the dust effect happens
                            Main.npc[TumorSummon].ai[0] = -2;

                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, TumorSummon);
                            }
                        }

                        //use npc.ai[1] to prevent projectile shooting attack from running after summoning
                        NPC.ai[1] = 1;
                    }
                }
                else
                {
                    if (NPC.ai[0] >= 550 && NPC.ai[1] == 0)
                    {
                        int[] Projectiles = new int[] { ModContent.ProjectileType<TumorOrb1>(), 
                        ModContent.ProjectileType<TumorOrb2>(), ModContent.ProjectileType<TumorOrb3>() };

                        if (Main.rand.Next(3) == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item87, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {   
                                int TumorOrb = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X + Main.rand.Next(-50, 50), NPC.Center.Y + Main.rand.Next(-50, 50), 
                                0, 0, Main.rand.Next(Projectiles), Damage, 0f, Main.myPlayer, 0f, (float)NPC.whoAmI);
                                Main.projectile[TumorOrb].ai[0] = 179;
                            }   
                        }
                    }
                }

                if (NPC.ai[0] >= 580)
                {
                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TortumorStaff>(), 35));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EyeChocolate>(), 100));
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
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TortumorGiantGore" + numGores).Type);
                }

                for (int numDust = 0; numDust < 45; numDust++)
                {
                    int newDust = Dust.NewDust(NPC.Center, NPC.width / 2, NPC.height / 2, DustID.Blood, 0f, 0f, 100, default(Color), 2f);
                    Main.dust[newDust].velocity.X *= Main.rand.Next(-12, 12);
                    Main.dust[newDust].velocity.Y *= Main.rand.Next(-12, 12);
                    Main.dust[newDust].noGravity = true;

                    if (Main.rand.Next(2) == 0)
                    {
                        Main.dust[newDust].scale = 0.5f;
                        Main.dust[newDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
            }
        }
    }
}