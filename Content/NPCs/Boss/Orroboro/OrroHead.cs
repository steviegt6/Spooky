using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

using Spooky.Content.NPCs.Boss.Orroboro.Projectiles;

namespace Spooky.Content.NPCs.Boss.Orroboro
{
    //[AutoloadBossHead]
    public class OrroHead : ModNPC
    {
        Vector2 SavePlayerPosition;
        public bool Transition = false;
        private bool spawned;

        public static readonly SoundStyle HissSound1 = new("Spooky/Content/Sounds/Orroboro/HissShort", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle HissSound2 = new("Spooky/Content/Sounds/Orroboro/HissLong", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle SpitSound = new("Spooky/Content/Sounds/Orroboro/VenomSpit", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle SplitSound = new("Spooky/Content/Sounds/SpookyHell/OrroboroSplit", SoundType.Sound);
        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/SpookyHell/EnemyHit", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orro");

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);

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
            //bools
            writer.Write(Transition);
            writer.Write(spawned);

            //local ai
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            Transition = reader.ReadBoolean();
            spawned = reader.ReadBoolean();

            //local ai
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = Main.masterMode ? 52000 / 3 : Main.expertMode ? 45000 / 2 : 32000;
            NPC.damage = 60;
            NPC.defense = 35;
            NPC.width = 54;
            NPC.height = 54;
            NPC.npcSlots = 25f;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.behindTiles = true;
            NPC.netAlways = true;
            NPC.HitSound = HitSound;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Orroboro");
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			float Divide = 1.8f;

			if (projectile.penetrate <= -1)
			{
				damage /= (int)Divide;
			}
			else if (projectile.penetrate >= 3)
			{
				damage /= (int)Divide;
			}
		}

        //rotate the bosses map icon to the NPCs direction
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture =  ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = new Rectangle(0, NPC.frame.Y, texture.Width, texture.Height / Main.npcFrameCount[NPC.type]);
            Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
            Main.spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, frame, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            int Damage = Main.masterMode ? 100 / 3 : Main.expertMode ? 80 / 2 : 50;

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

            //despawn if all players are dead or not in the biome
            if (player.dead || !player.InModBiome(ModContent.GetInstance<Biomes.SpookyHellBiome>()))
            {
                NPC.localAI[3]++;
                if (NPC.localAI[3] >= 75)
                {
                    NPC.velocity.Y = 25;
                    NPC.EncourageDespawn(10);
                }
            }
            else
            {
                NPC.localAI[3] = 0;
            }

            //Create the worm itself
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!spawned)
                {
                    NPC.realLife = NPC.whoAmI;
                    int LatestNPC = NPC.whoAmI;

                    //spawn the 3 orro segments
                    LatestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<OrroBody1>(), NPC.whoAmI, 0, LatestNPC);                   
                    Main.npc[LatestNPC].realLife = NPC.whoAmI;
                    Main.npc[LatestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[LatestNPC].netUpdate = true;
                    LatestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<OrroBody2>(), NPC.whoAmI, 0, LatestNPC);                   
                    Main.npc[LatestNPC].realLife = NPC.whoAmI;
                    Main.npc[LatestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[LatestNPC].netUpdate = true;
                    LatestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<OrroBody1>(), NPC.whoAmI, 0, LatestNPC);                   
                    Main.npc[LatestNPC].realLife = NPC.whoAmI;
                    Main.npc[LatestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[LatestNPC].netUpdate = true;

                    //spawn connector segment
                    LatestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<BoroBodyConnect>(), NPC.whoAmI, 0, LatestNPC);                   
                    Main.npc[LatestNPC].realLife = NPC.whoAmI;
                    Main.npc[LatestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[LatestNPC].netUpdate = true;

                    //spawn the 3 boro segments
                    LatestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<BoroBody1>(), NPC.whoAmI, 0, LatestNPC);                   
                    Main.npc[LatestNPC].realLife = NPC.whoAmI;
                    Main.npc[LatestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[LatestNPC].netUpdate = true;
                    LatestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<BoroBody2>(), NPC.whoAmI, 0, LatestNPC);                   
                    Main.npc[LatestNPC].realLife = NPC.whoAmI;
                    Main.npc[LatestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[LatestNPC].netUpdate = true;
                    LatestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<BoroBody1>(), NPC.whoAmI, 0, LatestNPC);                   
                    Main.npc[LatestNPC].realLife = NPC.whoAmI;
                    Main.npc[LatestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[LatestNPC].netUpdate = true;
                    //add extra body here so boro is the same length as orro visually
                    LatestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<BoroBody2>(), NPC.whoAmI, 0, LatestNPC);                   
                    Main.npc[LatestNPC].realLife = NPC.whoAmI;
                    Main.npc[LatestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[LatestNPC].netUpdate = true;

                    //spawn boro tail
                    LatestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<BoroTail>(), NPC.whoAmI, 0, LatestNPC);                   
                    Main.npc[LatestNPC].realLife = NPC.whoAmI;
                    Main.npc[LatestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[LatestNPC].netUpdate = true;

                    NPC.netUpdate = true;
                    spawned = true;
                }
            }

            //splitting transition
            if (NPC.life <= NPC.lifeMax / 2)
			{
                Transition = true;
                NPC.immortal = true;
                NPC.dontTakeDamage = true;
                NPC.netUpdate = true;
                NPC.velocity *= 0.97f;

                NPC.ai[2]++;
                
                //hiss
                if (NPC.ai[2] == 2)
                {
                    SoundEngine.PlaySound(HissSound2, NPC.Center);
                }
                
                //shake
                if (NPC.ai[2] < 180)
                {
                    NPC.Center = new Vector2(NPC.Center.X, NPC.Center.Y);
                    NPC.Center += Main.rand.NextVector2Square(-2, 2);
                }

                //spawn both worms (boro is spawned by orro because yeah)
                if (NPC.ai[2] >= 180)
                {
                    SoundEngine.PlaySound(SplitSound, NPC.Center);

                    int Orro = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<OrroHeadP2>());

                    //net update so the worms dont vanish on multiplayer
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: Orro);
                    }

                    NPC.netUpdate = true;
                    NPC.active = false;
                }
            }

            if (!Transition && !player.dead && NPC.localAI[3] < 75)
            {
                //attacks
                switch ((int)NPC.ai[0])
                {
                    //basic movement
                    case 0:
                    {
                        NPC.localAI[0]++;

                        //literally just basic worm movement
                        Movement(player, 20f, 0.38f, false);
                            
                        if (NPC.localAI[0] > 350)
                        {
                            NPC.localAI[0] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //chase the player directly, charge twice
                    case 1:
                    {
                        NPC.localAI[0]++;

                        //use chase movement
                        if (NPC.localAI[0] < 170 || (NPC.localAI[0] >= 240 && NPC.localAI[0] < 370) || NPC.localAI[0] >= 450)
                        {
                            Movement(player, 10f, 0.2f, true);
                        }

                        //slow down before charging
                        if (NPC.localAI[0] == 170 || NPC.localAI[0] == 370)
                        {
                            NPC.velocity *= 0.95f;
                        }

                        //charge at the saved location
                        if (NPC.localAI[0] == 200 || NPC.localAI[0] == 400)
                        {
                            SoundEngine.PlaySound(HissSound1, NPC.Center);

                            Vector2 ChargeDirection = player.Center - NPC.Center;
                            ChargeDirection.Normalize();
                                    
                            ChargeDirection.X *= 28;
                            ChargeDirection.Y *= 28;
                            NPC.velocity.X = ChargeDirection.X;
                            NPC.velocity.Y = ChargeDirection.Y;
                        }

                        //slow down after charging
                        if (NPC.localAI[0] == 240 || NPC.localAI[0] == 450)
                        {
                            NPC.velocity *= 0.2f;
                        }

                        if (NPC.localAI[0] > 475)
                        {
                            NPC.localAI[0] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //go below player, dash up, then curve back down
                    case 2:
                    {
                        NPC.localAI[0]++;

                        //go below the player
                        if (NPC.localAI[0] < 60)
                        {
                            Vector2 GoTo = player.Center;
                            GoTo.X += 0;
                            GoTo.Y += 850;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 18, 42);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        //teleport below the player, then create telegraph
                        if (NPC.localAI[0] == 60)
                        {
                            NPC.velocity *= 0;

                            NPC.position.X = player.Center.X - 20;
                            NPC.position.Y = player.Center.Y + 850;

                            Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center.X, player.Center.Y + 225, 0, 0,
                            ModContent.ProjectileType<TelegraphRed>(), 0, 0f, 0);
                        }

                        //charge up
                        if (NPC.localAI[0] == 75)
                        {
                            SoundEngine.PlaySound(HissSound2, NPC.Center);

                            NPC.velocity.X *= 0;
                            NPC.velocity.Y = -42;
                        }

                        //turn around after vertically passing the player
                        if (NPC.localAI[0] >= 75 && NPC.localAI[0] <= 135)
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

                            if (Math.Abs(angle) > Math.PI / 2)
                            {
                                NPC.localAI[1] = Math.Sign(angle);
                                NPC.velocity = Vector2.Normalize(NPC.velocity) * 30;
                            }

                            NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(4.5f) * NPC.localAI[1]);
                        }

                        if (NPC.localAI[0] > 135)
                        {
                            NPC.velocity *= 0.5f;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //charge at the player and shoot spreads of spit, 3 times
                    case 3:
                    {
                        NPC.localAI[0]++;

                        if (NPC.localAI[1] < 3)
                        {
                            //chase the player
                            if (NPC.localAI[0] < 70)
                            {
                                Movement(player, 13f, 0.2f, true);
                            }

                            //slow down
                            if (NPC.localAI[0] == 70)
                            {
                                NPC.velocity *= 0.97f;
                            }

                            //charge at the player
                            if (NPC.localAI[0] == 100)
                            {
                                SoundEngine.PlaySound(SpitSound, NPC.Center);

                                Vector2 ChargeDirection = player.Center - NPC.Center;
                                ChargeDirection.Normalize();
                                        
                                ChargeDirection.X *= 30;
                                ChargeDirection.Y *= 30;
                                NPC.velocity.X = ChargeDirection.X;
                                NPC.velocity.Y = ChargeDirection.Y;
                            }

                            //shoot spread of venom spit
                            if (NPC.localAI[0] == 110)
                            {
                                for (int numProjectiles = -3; numProjectiles <= 3; numProjectiles++)
                                {
                                    Vector2 velocity = NPC.velocity * 0.25f;

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity.RotatedBy(MathHelper.ToRadians(12) * numProjectiles),
                                        ModContent.ProjectileType<EyeSpit>(), Damage, 0f, Main.myPlayer);
                                    }
                                }
                            }

                            if (NPC.localAI[0] >= 130)
                            {
                                NPC.velocity *= 0.5f;
                                NPC.localAI[0] = 0;
                                NPC.localAI[1]++;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            if (NPC.localAI[0] >= 30)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1] = 0;
                                NPC.ai[0]++;
                                NPC.netUpdate = true;
                            }
                        }

                        break;
                    }

                    //go to the top corner of the player, dash horizontally, and then circle the player
                    case 4:
                    {
                        NPC.localAI[0]++;

                        //go to the top corner of the player
                        if (NPC.localAI[0] <= 100)
                        {
                            Vector2 GoTo = player.Center;
                            GoTo.X += (NPC.Center.X < player.Center.X) ? -1300 : 1300;
                            GoTo.Y -= 400;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 35);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        //teleport to the exact position before using attack
                        if (NPC.localAI[0] == 100)
                        {
                            NPC.velocity *= 0.02f;

                            NPC.position.X = (NPC.Center.X < player.Center.X) ? player.Center.X - 1300 : player.Center.X + 1300;
                            NPC.position.Y = player.Center.Y - 400;
                        }

                        //charge horizontally toward the player, but not vertically
                        if (NPC.localAI[0] == 110)
                        {
                            SoundEngine.PlaySound(HissSound1, NPC.position);

                            Vector2 ChargeDirection = player.Center - NPC.Center;
                            ChargeDirection.Normalize();
                                    
                            ChargeDirection.X *= 45;
                            ChargeDirection.Y *= 0;  
                            NPC.velocity.X = ChargeDirection.X;
                            NPC.velocity.Y = ChargeDirection.Y;
                        }

                        //spin around after horizontally passing the player
                        if (NPC.localAI[0] >= 120 && NPC.localAI[0] <= 250)
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

                            if (Math.Abs(angle) > Math.PI / 2)
                            {
                                NPC.localAI[1] = Math.Sign(angle);
                                NPC.velocity = Vector2.Normalize(NPC.velocity) * 35;
                            }

                            NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(5f) * NPC.localAI[1]);
                        }

                        //shoot venom spit at the player while spinning
                        if (NPC.localAI[0] >= 140 && NPC.localAI[0] <= 250)
                        {
                            if (NPC.localAI[0] % 20 == 5)
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

                        if (NPC.localAI[0] > 250)
                        {
                            NPC.velocity *= 0.5f;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //go below player, charge up, and shoot spreads of venom spit upward
                    case 5:
                    {
                        NPC.localAI[0]++;
                            
                        if (NPC.localAI[0] < 80)
                        {
                            Vector2 GoTo = player.Center;
                            GoTo.X += 0;
                            GoTo.Y += 750;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 35);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        if (NPC.localAI[0] == 80)
                        {
                            NPC.velocity *= 0.02f;

                            NPC.position.X = player.Center.X - 20;
                            NPC.position.Y = player.Center.Y + 750;

                            Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center.X, player.Center.Y + 225, 0, 0,
                            ModContent.ProjectileType<TelegraphRed>(), 0, 0f, 0);
                        }

                        if (NPC.localAI[0] == 100)
                        {
                            SoundEngine.PlaySound(HissSound1, NPC.Center);

                            NPC.velocity.X *= 0;
                            NPC.velocity.Y = -32;
                        }

                        if (NPC.localAI[0] == 125 || NPC.localAI[0] == 135)
                        {
                            SoundEngine.PlaySound(SpitSound, NPC.Center);

                            int MaxProjectiles = Main.rand.Next(5, 8);

                            for (int numProjectiles = -MaxProjectiles; numProjectiles <= MaxProjectiles; numProjectiles++)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 18f * NPC.DirectionTo(new Vector2(NPC.Center.X, NPC.Center.Y - 100)).RotatedBy(MathHelper.ToRadians(8) * numProjectiles),
                                    ModContent.ProjectileType<EyeSpit2>(), Damage, 0f, Main.myPlayer);
                                }
                            }
                        }

                        if (NPC.localAI[0] > 135)
                        {
                            NPC.velocity *= 0.97f;
                        }

                        if (NPC.localAI[0] > 300)
                        {
                            NPC.velocity *= 0.5f;
                            NPC.localAI[0] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //summon thorn pillars 2 times, then charge up at the player
                    case 6:
                    {
                        NPC.localAI[0]++;

                        if (NPC.localAI[1] < 3)
                        {
                            Vector2 GoTo = player.Center;
                            GoTo.X += (NPC.Center.X < player.Center.X) ? -1000 : 1000;
                            GoTo.Y += 750;

                            //go from side to side
                            if (NPC.localAI[0] < 120)
                            {
                                GoTo.X += 1000;
                            }
                            if (NPC.localAI[0] > 120)
                            {
                                GoTo.X += -1000;
                            }
                            
                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 25);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

                            if (NPC.localAI[0] % 20 == 5 && NPC.localAI[1] > 0)
                            {
                                for (int j = 0; j <= 0; j++) //0 was 1, 1 was 10
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

                                    for (int i = -1; i <= 1; i += 2) 
                                    {
                                        Vector2 center = new(player.Center.X, player.Center.Y + player.height / 4);
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

                                        if (Main.rand.Next(15) == 0)
                                        {
                                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                            {
                                                Projectile.NewProjectile(NPC.GetSource_FromThis(), center.X, center.Y + 20, 0, 0, 
                                                ModContent.ProjectileType<ThornTelegraph>(), Damage, 1, Main.myPlayer, 0, 0);
                                            }
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

                    //charge up after thorns
                    case 7:
                    {
                        NPC.localAI[0]++;

                        //go directly below the player
                        if (NPC.localAI[0] < 60)
                        {
                            Vector2 GoTo = player.Center;
                            GoTo.X += 0;
                            GoTo.Y += 750;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 18, 42);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        //teleport below the player, then create telegraph
                        if (NPC.localAI[0] == 60)
                        {
                            NPC.velocity *= 0;
                            
                            NPC.position.X = player.Center.X - 20;
                            NPC.position.Y = player.Center.Y + 750;

                            Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center.X, player.Center.Y + 225, 0, 0,
                            ModContent.ProjectileType<TelegraphRed>(), 0, 0f, 0);
                        }

                        //charge up
                        if (NPC.localAI[0] == 75)
                        {
                            SoundEngine.PlaySound(HissSound1, NPC.position);

                            NPC.velocity.X *= 0;
                            NPC.velocity.Y = -35;
                        }

                        //reset attack pattern back to the beginning
                        if (NPC.localAI[0] >= 130)
                        {
                            NPC.velocity *= 0.5f;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0; 
                            NPC.ai[0] = 0; 
                            NPC.netUpdate = true;
                        }

                        break;
                    }
                }
            }
        }

        //goofy as hell modified vanilla worm movement code
        private void Movement(Player player, float maxSpeed, float accel, bool ChaseMovement = false)
        {
            bool collision = false;
            bool FastMovement = false;

            if (Vector2.Distance(NPC.Center, player.Center) >= 500)
            {
                FastMovement = true;
            }
            if (Vector2.Distance(NPC.Center, player.Center) <= 500)           
            {
                FastMovement = false;
            }

            float speed = maxSpeed;
            float acceleration = accel;

            if (!collision)
            {
                Rectangle rectangle1 = new((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);

                int maxDistance = 350;

                if (!ChaseMovement)
                {
                    maxDistance = 350;
                }
                if (ChaseMovement)
                {
                    maxDistance = 12;
                }

                bool playerCollision = true;

                for (int index = 0; index < 255; ++index)
                {
                    if (Main.player[index].active)
                    {
                        Rectangle rectangle2 = new((int)Main.player[index].position.X - maxDistance, 
                        (int)Main.player[index].position.Y - maxDistance, maxDistance * 2, maxDistance * 2);
                        if (rectangle1.Intersects(rectangle2))
                        {
                            playerCollision = false;
                        
                            break;
                        }
                    }
                }

                if (playerCollision)
                {
                    collision = true;
                }
            }

            Vector2 NPCCenter = new(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
            float targetXPos = player.position.X + (player.width / 2);
            float targetYPos = player.position.Y + (player.height / 2);

            float targetRoundedPosX = (int)(targetXPos / 16.0) * 16;
            float targetRoundedPosY = (int)(targetYPos / 16.0) * 16;
            NPCCenter.X = (int)(NPCCenter.X / 16.0) * 16;
            NPCCenter.Y = (int)(NPCCenter.Y / 16.0) * 16;
            float dirX = targetRoundedPosX - NPCCenter.X;
            float dirY = targetRoundedPosY - NPCCenter.Y;
            float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
            
            if (!collision)
            {
                NPC.TargetClosest(true);

                if (NPC.velocity.Y > speed)
                {
                    NPC.velocity.Y = speed;
                }
                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.4)
                {
                    if (NPC.velocity.X < 0.0)
                    {
                        NPC.velocity.X -= acceleration * 1.1f;
                    }
                    else
                    {
                        NPC.velocity.X += acceleration * 1.1f;
                    }
                }

                else if (NPC.velocity.Y == speed)
                {
                    if (NPC.velocity.X < dirX)
                    {
                        NPC.velocity.X += acceleration;
                    }
                    else if (NPC.velocity.X > dirX)
                    {
                        NPC.velocity.X -= acceleration;
                    }
                }
                else if (NPC.velocity.Y > 4.0)
                {
                    if (NPC.velocity.X < 0.0)
                    {
                        NPC.velocity.X += acceleration * 1f;
                    }
                    else
                    {
                        NPC.velocity.X -= acceleration * 1f; 
                    }
                }
            }

            if (!collision)
            {
                NPC.TargetClosest(true);
                NPC.velocity.Y = NPC.velocity.Y + 0.11f;

                if (NPC.velocity.Y > speed)
                { 
                    NPC.velocity.Y = speed;
                }

                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 1.0)
                {
                    if (NPC.velocity.X < 0.0)
                    {
                        NPC.velocity.X -= acceleration * 1.1f;
                    }
                    else
                    {
                        NPC.velocity.X += acceleration * 1.4f;
                    }
                }
                
                if (NPC.velocity.Y > 5.0) 
                {
                    if (NPC.velocity.X < 0.0)
                    {
                        NPC.velocity.X += acceleration * 0.9f;
                    }
                    else
                    {
                        NPC.velocity.X -= acceleration * 0.9f;
                    }
                }
            }
            else if (collision || FastMovement)
            {
                float absDirX = Math.Abs(dirX);
                float absDirY = Math.Abs(dirY);
                float newSpeed = speed / length;
                dirX *= newSpeed;
                dirY *= newSpeed;

                if (NPC.velocity.X > 0.0 && dirX > 0.0 || NPC.velocity.X < 0.0 && dirX < 0.0 || (NPC.velocity.Y > 0.0 && dirY > 0.0 || NPC.velocity.Y < 0.0 && dirY < 0.0))
                {
                    if (NPC.velocity.X < dirX)
                    {
                        NPC.velocity.X += acceleration;
                    }
                    else if (NPC.velocity.X > dirX)
                    {
                        NPC.velocity.X -= acceleration;
                    }
                    if (NPC.velocity.Y < dirY)
                    {
                        NPC.velocity.Y += acceleration;
                    }
                    else if (NPC.velocity.Y > dirY)
                    {
                        NPC.velocity.Y -= acceleration;
                    }

                    if (Math.Abs(dirY) < speed * 0.2 && (NPC.velocity.X > 0.0 && dirX < 0.0 || NPC.velocity.X < 0.0 && dirX > 0.0))
                    {
                        if (NPC.velocity.Y > 0.0)
                        {
                            NPC.velocity.Y += acceleration * 2f;
                        }
                        else
                        {
                            NPC.velocity.Y -= acceleration * 2f;
                        }
                    }
                    if (Math.Abs(dirX) < speed * 0.2 && (NPC.velocity.Y > 0.0 && dirY < 0.0 || NPC.velocity.Y < 0.0 && dirY > 0.0))
                    {
                        if (NPC.velocity.X > 0.0)
                        {
                            NPC.velocity.X += acceleration * 2f;
                        }
                        else
                        {
                            NPC.velocity.X -= acceleration * 2f;
                        }
                    }
                }
                else if (absDirX > absDirY)
                {
                    if (NPC.velocity.X < dirX)
                    {
                        NPC.velocity.X += acceleration * 1.1f;
                    }
                    else if (NPC.velocity.X > dirX)
                    {
                        NPC.velocity.X -= acceleration * 1.1f;
                    }
                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5)
                    {
                        if (NPC.velocity.Y > 0.0)
                        {
                            NPC.velocity.Y += acceleration;
                        }
                        else
                        {
                            NPC.velocity.Y -= acceleration;
                        }
                    }
                }
                else
                {
                    if (NPC.velocity.Y < dirY)
                    {
                        NPC.velocity.Y += acceleration * 1.1f;
                    }
                    else if (NPC.velocity.Y > dirY)
                    {
                        NPC.velocity.Y -= acceleration * 1.1f;
                    }

                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5)
                    {
                        if (NPC.velocity.X > 0.0)
                        {
                            NPC.velocity.X += acceleration;
                        }
                        else
                        {
                            NPC.velocity.X -= acceleration;
                        }
                    }
                }
            }

            //Some netupdate stuff
            if (collision)
            {
                if (NPC.localAI[2] != 1)
                {
                    NPC.netUpdate = true;
                }
                NPC.localAI[2] = 1f;
            }
            else
            {
                if (NPC.localAI[2] != 0.0)
                {
                    NPC.netUpdate = true;
                }
                NPC.localAI[2] = 0.0f;
            }
            if ((NPC.velocity.X > 0.0 && NPC.oldVelocity.X < 0.0 || NPC.velocity.X < 0.0 && NPC.oldVelocity.X > 0.0 || 
            (NPC.velocity.Y > 0.0 && NPC.oldVelocity.Y < 0.0 || NPC.velocity.Y < 0.0 && NPC.oldVelocity.Y > 0.0)) && !NPC.justHit)
            {
                NPC.netUpdate = true;
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
        
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.2f;
            return null;
        }
    }
}