using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Items.Pets;
using Spooky.Content.NPCs.Boss.BigBone;

namespace Spooky.Content.NPCs.Catacomb
{
    [AutoloadBossHead]
    public class CatacombGuardian : ModNPC  
    {
        Vector2 SavePlayerPosition;

        public int attackPattern = 0;
        public int SaveDirection;
        public float SaveRotation;

        private static Asset<Texture2D> EyeGlowTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Rotation = MathHelper.PiOver2,
                Position = new Vector2(34f, 0f),
                PortraitPositionXOverride = 8f,
                PortraitPositionYOverride = -8f
            };
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 280000;
            NPC.damage = 420000;
            NPC.defense = 0;
            NPC.width = 126;
			NPC.height = 116;
            NPC.npcSlots = 0f;
			NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 11;
			AIType = NPCID.DungeonGuardian;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.CatacombBiome>().Type, ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            int associatedNPCType = ModContent.NPCType<BigBone>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.CatacombGuardian"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        //draw eye after images
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            EyeGlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Catacomb/CatacombGuardianEye");

            Vector2 drawOrigin = new(EyeGlowTexture.Width() * 0.5f, NPC.height * 0.5f);

            for (int oldPos = 0; oldPos < NPC.oldPos.Length; oldPos++)
            {
                var effects = attackPattern == 2 && NPC.localAI[0] >= 360 ? (SaveDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally) : (NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

                Vector2 drawPos = NPC.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY + 4);
                Color color = NPC.GetAlpha(Color.Red) * (float)(((float)(NPC.oldPos.Length - oldPos) / (float)NPC.oldPos.Length) / 2);
                spriteBatch.Draw(EyeGlowTexture.Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
            }
		}
        
        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;
            NPC.defense = 0;

            //EoC rotation
            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            /*
            if (NPC.life <= NPC.lifeMax * 0.85)
            {
                NPC.localAI[0]++;

                if (NPC.localAI[0] >= 300)
                {
                    NPC.velocity *= 0.97f;
                }

                if (NPC.localAI[0] == 310)
                {
                    attackPattern = Main.rand.Next(3);

                    NPC.aiStyle = -1;
                    AIType = 0;
                }

                switch (attackPattern)
                {
                    //shoot burst of wisps
                    case 0:
                    {
                        if (NPC.localAI[0] == 360)
                        {
                            NPC.velocity *= 0.9f;
                        }

                        if (NPC.localAI[0] == 380)
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath6, NPC.Center);

                            for (int numProjectiles = 0; numProjectiles < 25; numProjectiles++)
                            {
                                //shoot speed
                                Vector2 ShootSpeed = player.Center - NPC.Center;
                                ShootSpeed.Normalize();
                                ShootSpeed.X *= 15 + Main.rand.Next(-5, 15);
                                ShootSpeed.Y *= 15 + Main.rand.Next(-5, 15);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-100, 100), NPC.Center.Y + Main.rand.Next(-100, 100), 
                                    ShootSpeed.X, ShootSpeed.Y, ModContent.ProjectileType<BoneWisp>(), NPC.damage / 2, 1, Main.myPlayer, 0, NPC.whoAmI);
                                }
                            }
                        }

                        break;
                    }
                    //shoot homing flowers
                    case 1:
                    {
                        if (NPC.localAI[0] == 340)
                        {
                            Vector2 Recoil = player.Center - NPC.Center;
                            Recoil.Normalize();
                                    
                            Recoil *= -8;
                            NPC.velocity.X = Recoil.X;
                            NPC.velocity.Y = Recoil.Y;
                        }

                        if (NPC.localAI[0] >= 360 && NPC.localAI[0] <= 380 && NPC.localAI[0] % 2 == 0)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, 25f * NPC.DirectionTo(player.Center), 
                            ModContent.ProjectileType<HomingFlower>(), NPC.damage / 2, 0f, Main.myPlayer);
                        }

                        break;
                    }
                    //charge at the player
                    case 2:
                    {
                        if (NPC.localAI[0] == 360)
                        {
                            SaveRotation = NPC.rotation;
                            SaveDirection = NPC.direction;
                            SavePlayerPosition = player.Center;

                            Vector2 Recoil = player.Center - NPC.Center;
                            Recoil.Normalize();
                                    
                            Recoil.X *= -5;
                            Recoil.Y *= -5;  
                            NPC.velocity.X = Recoil.X;
                            NPC.velocity.Y = Recoil.Y;
                        }

                        if (NPC.localAI[0] > 360)
                        {
                            NPC.spriteDirection = SaveDirection;
                            NPC.rotation = SaveRotation;
                        }

                        if (NPC.localAI[0] == 370)
                        {
                            NPC.velocity *= 0.5f;
                        }

                        if (NPC.localAI[0] == 380)
                        {
                            SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                            Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
                            ChargeDirection.Normalize();
                                    
                            ChargeDirection.X *= 75;
                            ChargeDirection.Y *= 75;
                            NPC.velocity.X = ChargeDirection.X;
                            NPC.velocity.Y = ChargeDirection.Y;
                        }

                        if (NPC.localAI[0] >= 420)
                        {
                            NPC.velocity *= 0.75f;
                        }

                        break;
                    }
                }

                if (NPC.localAI[0] >= 440)
                {
                    NPC.aiStyle = 11;
			        AIType = NPCID.DungeonGuardian;

                    NPC.localAI[0] = 0;

                    NPC.netUpdate = true;
                }
            }
            */
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkullKey>()));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BigBoneGore2").Type);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BigBoneGore3").Type);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BigBoneGore4").Type);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BigBoneGore5").Type);
                }
            }
        }
    }
}