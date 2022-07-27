using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.BossBags;
using Spooky.Content.Items.BossBags.Pets;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Boss;
using Spooky.Content.NPCs.Boss.Orroboro.Projectiles;
using Spooky.Content.Tiles.Relic;

namespace Spooky.Content.NPCs.Boss.Orroboro.Phase2
{
    [AutoloadBossHead]
    public class BoroHead : ModNPC
    {
        Vector2 SavePoint;
        private bool spawned;

        public bool DeathState = false;

        public static readonly SoundStyle GrowlSound = new("Spooky/Content/Sounds/OrroboroGrowl2", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Boro");
            Main.npcFrameCount[NPC.type] = 5;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/Boss/Orroboro/BoroBestiary",
                Position = new Vector2(40f, 24f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 20f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] 
                {
                    BuffID.Confused
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = Main.masterMode ? 18000 / 3 : Main.expertMode ? 14500 / 2 : 10000;
            NPC.damage = 55;
            NPC.defense = 30;
            NPC.width = 62;
            NPC.height = 62;
            NPC.npcSlots = 25f;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.behindTiles = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath5;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Orroboro");
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Content.Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), //Plain black background
				new FlavorTextBestiaryInfoElement("A blind and aggressive creature that will work together with Orro to defend its territory. It is said that the living hell is made from the chunks of flesh left forgotten by this strange duo.")
			});
		}

        //rotate the bosses map icon to the NPCs direction
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;
            if (NPC.frameCounter > 4)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
            {
                NPC.frame.Y = 0;
            }
        }

        public override bool CheckDead()
        {
            //if orro hasnt died, put this npc in its death state
            if (Main.npc[(int)NPC.ai[1]].ai[3] <= 0)
            {
                NPC.ai[3] = 1;

                NPC.dontTakeDamage = true;
                NPC.life = 1;

                return false;
            }
            if (Main.npc[(int)NPC.ai[1]].ai[3] == 1)
            {
                NPC.ai[3] = 2;
                Main.npc[(int)NPC.ai[1]].ai[3] = 3;

                NPC.dontTakeDamage = true;
                NPC.life = 1;

                return false;
            }

            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //if boro is in its death state
            if (NPC.ai[3] > 0)
            {
                float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6.28318548f)) / 2f + 0.5f;

                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

                Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.Red);

                for (int numEffect = 0; numEffect < 4; numEffect++)
                {
                    Color newColor = color;
                    newColor = NPC.GetAlpha(newColor);
                    newColor *= 1f - fade;
                    Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y) + (numEffect / 4 * 6.28318548f + NPC.rotation + 0f).ToRotationVector2() * (4f * fade + 2f) - Main.screenPosition + new Vector2(0, NPC.gfxOffY) - NPC.velocity * numEffect;
                    Main.EntitySpriteDraw(tex, vector, NPC.frame, newColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.5f, SpriteEffects.None, 0);
                }
            }

            return true;
        }

        public override void AI()
        {
            NPC Orro = Main.npc[(int)NPC.ai[1]];

            //death animation
            if (NPC.ai[3] >= 2)
            {
				NPC.velocity *= 0.95f;

                NPC.ai[0] = -1;

                NPC.ai[2]++;
                if (NPC.ai[2] < 240)
                {
                    NPC.Center = new Vector2(NPC.Center.X, NPC.Center.Y);
                    NPC.Center += Main.rand.NextVector2Square(-5, 5);
                }
                
				if (NPC.ai[2] >= 240)
				{
					NPC.life = 0;
					Main.NewText("Boro has been defeated!", 171, 64, 255);

                    if (Main.netMode != NetmodeID.Server)
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity / 5, ModContent.Find<ModGore>("Spooky/BoroHeadGore1").Type);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity / 5, ModContent.Find<ModGore>("Spooky/BoroHeadGore2").Type);
                    }

                    if (NPC.ai[3] == 2)
                    {
					    NPC.checkDead();
                    }
				}

				return;
			}
            
            if (NPC.CountNPCS(ModContent.NPCType<BoroHead>()) > 1) //|| (NPC.ai[3] > 0 && !NPC.AnyNPCs(ModContent.NPCType<OrroHead>())))
            {
                NPC.active = false;
            }

            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            int Damage = Main.masterMode ? 100 / 3 : Main.expertMode ? 80 / 2 : 50;

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

            //despawn if all players are dead or not in the biome
            if (Main.player[NPC.target].dead || !player.InModBiome(ModContent.GetInstance<Biomes.SpookyHellBiome>()))
            {
                NPC.localAI[3]++;
                if (NPC.localAI[3] >= 120)
                {
                    NPC.velocity.Y = 25;
                }

                if (NPC.localAI[3] >= 180)
                {
                    NPC.active = false;
                }
            }
            else
            {
                NPC.localAI[3] = 0;
            }

            //Make the worm itself
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!spawned)
                {
                    NPC.realLife = NPC.whoAmI;
                    int latestNPC = NPC.whoAmI;

                    for (int i = 0; i < 2; ++i)
                    {
                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), ModContent.NPCType<BoroBody>(), NPC.whoAmI, 0, latestNPC);                   
                        Main.npc[latestNPC].realLife = NPC.whoAmI;
                        Main.npc[latestNPC].ai[2] = NPC.whoAmI;
                        Main.npc[latestNPC].netUpdate = true;
                    }

                    latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), ModContent.NPCType<BoroTail>(), NPC.whoAmI, 0, latestNPC);
                    Main.npc[latestNPC].realLife = NPC.whoAmI;
                    Main.npc[latestNPC].ai[2] = NPC.whoAmI;
                    Main.npc[latestNPC].netUpdate = true;

                    NPC.netUpdate = true;
                    spawned = true;
                }
            }

            if (!player.dead && player.InModBiome(ModContent.GetInstance<Biomes.SpookyHellBiome>()) && NPC.localAI[3] < 120)
            {
                //attacks
                switch ((int)NPC.ai[0])
                {
                    //charge from the sides while orro chases
                    case 0:
                    {
                        NPC.localAI[0]++;

                        if (NPC.localAI[1] < 3)
                        {
                            if (NPC.localAI[0] < 60)
                            {
                                Vector2 GoTo = player.Center;
                                GoTo.X += (NPC.Center.X < player.Center.X) ? -1200 : 1200;
                                GoTo.Y -= 0;

                                float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 30);
                                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                            }

                            if (NPC.localAI[0] == 60)
                            {
                                NPC.velocity *= 0;

                                NPC.position.X = (NPC.Center.X < player.Center.X) ? player.Center.X - 1200 : player.Center.X + 1200;
                                NPC.position.Y = player.Center.Y;

                                Projectile.NewProjectile(NPC.GetSource_FromThis(), (NPC.Center.X < player.Center.X) ? player.Center.X - 400 : player.Center.X + 400, 
                                player.Center.Y, 0, 0, ModContent.ProjectileType<TelegraphRed>(), 0, 0f, 0);
                            }

                            if (NPC.localAI[0] == 75)
                            {
                                SoundEngine.PlaySound(GrowlSound, NPC.Center);

                                Vector2 ChargeDirection = player.Center - NPC.Center;
                                ChargeDirection.Normalize();
                                        
                                ChargeDirection.X *= 40;
                                ChargeDirection.Y *= 0;  
                                NPC.velocity.X = ChargeDirection.X;
                                NPC.velocity.Y = ChargeDirection.Y;
                            }

                            if (NPC.localAI[0] > 100)
                            {
                                if (NPC.localAI[1] == 2)
                                {
                                    NPC.velocity *= 0.95f; 
                                }
                                else
                                {
                                    NPC.velocity *= 0.99f;
                                }
                            }

                            if (NPC.localAI[0] >= 145)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1]++;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.velocity *= 0.5f;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }
                        
                        break;
                    }

                    //charge at the player 
                    case 1:
                    {
                        NPC.localAI[0]++;
                        
                        NPC.velocity *= 0.975f;

                        if (NPC.localAI[1] < 3)
                        {
                            if (NPC.localAI[0] == 45 || NPC.localAI[0] == 95 || NPC.localAI[0] == 145)
                            {
                                SoundEngine.PlaySound(GrowlSound, NPC.Center);

                                Vector2 ChargeDirection = player.Center - NPC.Center;
                                ChargeDirection.Normalize();
                                        
                                ChargeDirection.X *= 25 + Main.rand.Next(-5, 5);
                                ChargeDirection.Y *= 25 + Main.rand.Next(-5, 5);  
                                NPC.velocity.X = ChargeDirection.X;
                                NPC.velocity.Y = ChargeDirection.Y;
                            }

                            if (NPC.localAI[0] > 145)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1]++;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            if (NPC.localAI[0] >= 60)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1] = 0;
                                NPC.ai[0]++;
                                NPC.netUpdate = true;
                            }
                        }

                        break;
                    }

                    //go to players side, charge and then curve and spit projectiles in sync with orro
                    case 2:
                    {
                        NPC.localAI[0]++;
                        
                        if (NPC.localAI[0] < 75)
                        {
                            //this is slightly offset so its even with the other worm in game
                            Vector2 GoTo = player.Center;
                            GoTo.X += 1200;
                            GoTo.Y += 0;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 18, 42);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }
                        
                        //set exact position right before so the curling up is always even
                        if (NPC.localAI[0] == 75)
                        {
                            NPC.velocity *= 0;

                            //this is slightly offset so its even with the other worm in game
                            NPC.position.X = player.Center.X + 1200;
                            NPC.position.Y = player.Center.Y + 0;

                            Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center.X + 550, player.Center.Y, 0, 0,
                            ModContent.ProjectileType<TelegraphRed>(), 0, 0f, 0);
                        }

                        if (NPC.localAI[0] == 90)
                        {
                            SoundEngine.PlaySound(GrowlSound, NPC.Center);

                            NPC.velocity.X = -42;
                            NPC.velocity.Y *= 0;
                        }

                        if (NPC.localAI[0] >= 125 && NPC.localAI[0] <= 170)
                        {
                            double angle = NPC.DirectionTo(player.Center).ToRotation() - NPC.velocity.ToRotation();
                            while (angle > Math.PI)
                            {
                                angle -= 2.0 * Math.PI;
                            }
                            while (angle < -Math.PI)
                            {
                                angle += 2.0 * Math.PI;
                            }

                            NPC.localAI[1] = Math.Sign(angle);
                            NPC.velocity = Vector2.Normalize(NPC.velocity) * 32;

                            NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(4f) * NPC.localAI[1]);

                            if (NPC.localAI[0] == 130 || NPC.localAI[0] == 140 || NPC.localAI[0] == 150 || NPC.localAI[0] == 160 || NPC.localAI[0] == 170)
                            {
                                Vector2 ShootSpeed = player.Center - NPC.Center;
                                ShootSpeed.Normalize();
                                ShootSpeed.X *= 3f;
                                ShootSpeed.Y *= 3f;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X, ShootSpeed.Y, 
                                    ModContent.ProjectileType<EyeSpit>(), Damage, 1, Main.myPlayer, 0, 0);  
                                }
                            }
                        }   

                        if (NPC.localAI[0] == 170)
                        {
                            NPC.velocity *= 0.25f;
                        }

                        if (NPC.localAI[0] > 270)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //charge and shoot acidic flames while orro drops acid
                    case 3:
                    {
                        NPC.localAI[0]++;
                        NPC.velocity *= 0.97f;

                        if (NPC.localAI[1] < 4)
                        {
                            if (NPC.localAI[0] == 1 || NPC.localAI[0] == 80 || NPC.localAI[0] == 160)
                            {
                                Vector2 CenterPoint = player.Center;

                                Projectile.NewProjectile(NPC.GetSource_FromThis(), CenterPoint.X, CenterPoint.Y, 0, 0, 
                                ModContent.ProjectileType<AcidTarget>(), 0, 0f, 0);
                                
                                //use SavePoint to save where the telegraph was
                                SavePoint = CenterPoint;
                            }

                            //charge towards where the telegraphs saved point is
                            if (NPC.localAI[0] == 20 || NPC.localAI[0] == 100 || NPC.localAI[0] == 180)
                            {
                                SoundEngine.PlaySound(GrowlSound, NPC.Center);
                                SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, NPC.Center);

                                Vector2 ChargeDirection = SavePoint - NPC.Center;
                                ChargeDirection.Normalize();
                                        
                                ChargeDirection.X *= 28;
                                ChargeDirection.Y *= 28;  
                                NPC.velocity.X = ChargeDirection.X;
                                NPC.velocity.Y = ChargeDirection.Y;
                            }
                            
                            if ((NPC.localAI[0] > 20 && NPC.localAI[0] < 40) || (NPC.localAI[0] > 100 && NPC.localAI[0] < 120) || (NPC.localAI[0] > 180 && NPC.localAI[0] < 200))
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X + (NPC.velocity.X / 3), NPC.Center.Y + (NPC.velocity.Y / 3), 
                                NPC.velocity.X * 0.5f + Main.rand.NextFloat(-0.2f, 0.2f) * 1, NPC.velocity.Y * 0.5f + Main.rand.NextFloat(-0.2f, 0.2f) * 1, 
                                ModContent.ProjectileType<AcidBreath>(), Damage, 0f, 0);
                            }
                                
                            if (NPC.localAI[0] >= 240)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1]++;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.velocity *= 0.25f;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0; 
                            NPC.ai[0]++; 
                            NPC.netUpdate = true;
                        }
                        
                        break;
                    }

                    //use tentacle pillars while orro spits acid and chases
                    case 4:
                    {
                        NPC.localAI[0]++;

                        if (NPC.localAI[1] < 3)
                        {
                            Vector2 GoTo = player.Center;
                            GoTo.X += 0;
                            GoTo.Y += 600;

                            //go from side to side
                            if (NPC.localAI[0] < 120)
                            {
                                GoTo.X += -1000;
                            }
                            if (NPC.localAI[0] > 120)
                            {
                                GoTo.X += 1000;
                            }
                            
                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 17, 25);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

                            if (NPC.localAI[0] % 60 == 20)
                            {
                                for (int j = 0; j <= 0; j++)
                                {
                                    for (int i = 0; i <= 0; i += 1) 
                                    {
                                        Vector2 center = new(NPC.Center.X, player.Center.Y + player.height / 4);
                                        center.X += j * Main.rand.Next(150, 220) * i; //distance between each spike
                                        int numtries = 0;
                                        int x = (int)(center.X / 16);
                                        int y = (int)(center.Y / 16);
                                        while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && 
                                        Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y)) 
                                        {
                                            y++;
                                            center.Y = y * 16;
                                        }
                                        while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 10) 
                                        {
                                            numtries++;
                                            y--;
                                            center.Y = y * 16;
                                        }

                                        if (numtries >= 10)
                                        {
                                            break;
                                        }

                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(NPC.GetSource_FromThis(), center.X, center.Y + 20, 0, 0, 
                                            ModContent.ProjectileType<ThornTelegraph>(), Damage, 1, Main.myPlayer, 0, 0);
                                        }
                                    }
                                }
                            }

                            if (NPC.localAI[0] >= 240)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1]++;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0; 
                            NPC.ai[0]++; 
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //fly to corner, close in, charge down in the middle
                    case 5:
                    {
                        NPC.localAI[0]++;

                        if (NPC.localAI[0] > 60 && NPC.localAI[0] < 180)
                        {
                            //this is slightly offset so its even with the other worm in game
                            Vector2 GoTo = player.Center;
                            GoTo.X += 1550;
                            GoTo.Y -= 750;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 18, 42);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        if (NPC.localAI[0] == 180)
                        {
                            NPC.velocity *= 0;

                            //this is slightly offset so its even with the other worm in game
                            NPC.position.X = player.Center.X + 1550;
                            NPC.position.Y = player.Center.Y - 750;

                            for (int i = 230; i <= 1000; i += 175)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center.X + i, player.Center.Y - 175, 0, 0,
                                ModContent.ProjectileType<TelegraphPurple>(), 0, 0f, 0);
                            }
                        }

                        if (NPC.localAI[0] == 200)
                        {
                            SoundEngine.PlaySound(GrowlSound, NPC.Center);

                            NPC.velocity.X = -20;
                            NPC.velocity.Y *= 0;
                        }

                        if (NPC.localAI[0] > 200 && NPC.localAI[0] < 270)
                        {
                            if (Main.rand.Next(2) == 0)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, Main.rand.Next(-1, 1), 8, 
                                    ModContent.ProjectileType<EyeSpit>(), Damage, 0f, Main.myPlayer, 0, 0);
                                }
                            }
                        }

                        if (NPC.localAI[0] == 280)
                        {
                            NPC.velocity *= 0;
                        }

                        if (NPC.localAI[0] == 300)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y + 450, 0, 0, 
                            ModContent.ProjectileType<TelegraphRed>(), 0, 0f, Main.myPlayer, 0, 0);
                        }

                        if (NPC.localAI[0] == 350)
                        {
                            SoundEngine.PlaySound(GrowlSound, NPC.Center);

                            NPC.velocity.X *= 0;
                            NPC.velocity.Y = 35;
                        }

                        if (NPC.localAI[0] >= 420)
                        {
                            NPC.velocity *= 0.5f;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //circle and chase other worm
                    case 6:
                    {
                        NPC.localAI[0]++;

                        if (NPC.localAI[0] < 119)
                        {
                            Vector2 GoTo = player.Center;
                            GoTo.X += 1000;
                            GoTo.Y -= 750;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 18, 42);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }
                        
                        //set exact position right before so it is even
                        if (NPC.localAI[0] == 119)
                        {
                            NPC.velocity *= 0;

                            NPC.position.X = player.Center.X + 1000;
                            NPC.position.Y = player.Center.Y - 750;
                        }

                        if (NPC.localAI[0] == 120)
                        {
                            SoundEngine.PlaySound(GrowlSound, NPC.position);

                            Vector2 ChargeDirection = player.Center - NPC.Center;
                            ChargeDirection.Normalize();
                                    
                            ChargeDirection.X *= 40;
                            ChargeDirection.Y *= 0;  
                            NPC.velocity.X = ChargeDirection.X;
                            NPC.velocity.Y = ChargeDirection.Y;
                        }

                        if (NPC.localAI[0] >= 151 && NPC.localAI[0] <= 500)
                        {
                            if (NPC.localAI[0] >= 250 && NPC.localAI[0] <= 380)
                            {
                                if (NPC.localAI[0] % 20 == 5)
                                {
                                    Vector2 ShootSpeed = player.Center - NPC.Center;
                                    ShootSpeed.Normalize();
                                    ShootSpeed.X *= 2f;
                                    ShootSpeed.Y *= 2f;

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X, ShootSpeed.Y, 
                                        ModContent.ProjectileType<EyeSpit>(), Damage, 1, Main.myPlayer, 0, 0);  
                                    }
                                }

                                NPC.localAI[2] += 0.025f;
                            }

                            double angle = NPC.velocity.ToRotation();

                            //always subtract angle so it goes in a set circle
                            angle -= 3.5 * Math.PI;

                            NPC.localAI[1] = Math.Sign(angle);
                            NPC.velocity = Vector2.Normalize(NPC.velocity) * 50;

                            NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(3.5f + NPC.localAI[2]) * NPC.localAI[1]);
                        }

                        if (NPC.localAI[0] >= 500)
                        {
                            NPC.velocity *= 0.25f;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.localAI[2] = 0;
                            NPC.ai[0] = 0;
                            NPC.netUpdate = true;
                        }

                        break;
                    }
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());
            
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BossBagOrroboro>()));

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<OrroboroEye>(), 4));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<OrroboroRelicItem>()));

            int[] MainItem = new int[] { ModContent.ItemType<EyeFlail>(), ModContent.ItemType<Scycler>(), 
            ModContent.ItemType<EyeRocketLauncher>(), ModContent.ItemType<MouthFlamethrower>(), 
            ModContent.ItemType<LeechStaff>(), ModContent.ItemType<LeechWhip>() };

            notExpertRule.OnSuccess(ItemDropRule.Common(Main.rand.Next(MainItem)));

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<OrroboroChunk>(), 1, 12, 25));

            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref Flags.downedOrroboro, -1);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override bool CheckActive()
        {
            return false;
        }
        
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.2f;
            return null;
        }
    }
}