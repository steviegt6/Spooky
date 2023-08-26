using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.PandoraBox
{
    public class Sheldon : ModNPC
    {
        int repeats = Main.rand.Next(1, 3);

        public bool SpawnedShield = false;

        Vector2 SavePosition;
        Vector2 SavePlayerPosition;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] 
                {
                    BuffID.Confused,
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //ints
            writer.Write(repeats);
            writer.Write(SavePosition.X);
            writer.Write(SavePosition.Y);

            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //ints
            repeats = reader.ReadInt32();
            SavePosition.X = reader.ReadInt32();
            SavePosition.X = reader.ReadInt32();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 250;
            NPC.damage = 50;
            NPC.defense = 12;
            NPC.width = 46;
            NPC.height = 36;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 0, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit54;
            NPC.DeathSound = SoundID.NPCDeath52;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type, ModContent.GetInstance<Biomes.PandoraBoxBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Sheldon"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.PandoraBoxBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			for (int k = 0; k < Main.maxNPCs; k++)
			{
				if (Main.npc[k].active && Main.npc[k].type == ModContent.NPCType<PandoraBox>()) 
				{
					Vector2 ParentCenter = Main.npc[k].Center;

					Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/PandoraBox/ChainSmall");

					Rectangle? chainSourceRectangle = null;
					float chainHeightAdjustment = 0f;

					Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
					Vector2 chainDrawPosition = NPC.Center;
					Vector2 vectorFromProjectileToPlayerArms = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
					Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
					float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;

					if (chainSegmentLength == 0)
					{
						chainSegmentLength = 10;
					}

					float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
					int chainCount = 0;
					float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;
		
					while (chainLengthRemainingToDraw > 0f)
					{
						Color chainDrawColor = Color.Cyan * 0.5f;

						var chainTextureToDraw = chainTexture;

						Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

						chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
						chainCount++;
						chainLengthRemainingToDraw -= chainSegmentLength;
					}
				}
			}

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            float num108 = 4;
            float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6.28318548f)) / 2f + 0.5f;
            float num106 = 0f;

            SpriteEffects spriteEffects3 = (NPC.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 vector33 = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4) - NPC.velocity;
			Color color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.LightBlue);

            for (int num103 = 0; num103 < 4; num103++)
            {
				Color color28 = color29;
                color28 = NPC.GetAlpha(color28);
                color28 *= 1f - num107;
                Vector2 vector29 = new Vector2(NPC.Center.X, NPC.Center.Y) + ((float)num103 / (float)num108 * 6.28318548f + NPC.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - screenPos + new Vector2(0, NPC.gfxOffY + 4) - NPC.velocity * (float)num103;
                Main.spriteBatch.Draw(tex, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.2f, spriteEffects3, 0f);
            }

            Main.spriteBatch.Draw(tex, vector33, NPC.frame, color29, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.2f, spriteEffects3, 0f);

            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;

            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 3)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);
            
            NPC.spriteDirection = NPC.direction;

            if (!SpawnedShield)
            {
                NPC.ai[1] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SheldonShield>(), ai1: NPC.whoAmI);
                
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: (int)NPC.ai[1]);
                }

                SpawnedShield = true;
                NPC.netUpdate = true;
            }

            switch ((int)NPC.ai[0])
            {
                //fly to different locations around the player
                case 0:
                {
                    NPC.localAI[0]++;
                    
                    if (NPC.localAI[1] < repeats)
                    {
                        if (NPC.localAI[0] == 5)
                        {
                            SavePosition = new Vector2(player.Center.X + Main.rand.Next(-250, 250), player.Center.Y - Main.rand.Next(120, 180));
                        }

                        if (NPC.localAI[0] > 5 && NPC.localAI[0] <= 120)
                        {
                            Vector2 GoTo = SavePosition;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 6, Main.rand.Next(7, 12));
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        if (NPC.localAI[0] >= 120)
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

                //charge at the player
                case 1:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] < 60)
                    {
                        Vector2 GoTo = player.Center;
                        GoTo.X += (NPC.Center.X < player.Center.X) ? -250 : 250;
                        GoTo.Y += 0;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 8, 12);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    if (NPC.localAI[0] == 60)
                    {
                        SavePlayerPosition = player.Center;

                        NPC.velocity *= 0;
                    }

                    if (NPC.localAI[0] == 75)
                    {
                        SoundEngine.PlaySound(SoundID.Zombie7, NPC.Center);

                        Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
                        ChargeDirection.Normalize();
                                
                        ChargeDirection.X = ChargeDirection.X * 25;
                        ChargeDirection.Y = ChargeDirection.Y * 0;
                        NPC.velocity.X = ChargeDirection.X;
                        NPC.velocity.Y = ChargeDirection.Y;
                    }

                    if (NPC.localAI[0] >= 75)
                    {
                        NPC.spriteDirection = NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
                    }

                    if (NPC.localAI[0] >= 100)
                    {
                        NPC.velocity *= 0.65f;
                    }

                    if (NPC.localAI[0] >= 135)
                    {   
                        NPC.localAI[0] = 0;
                        NPC.ai[0] = 0;

                        NPC.netUpdate = true;
                    }

                    break;
                }
            }

            for (int num = 0; num < Main.maxNPCs; num++)
			{
				NPC other = Main.npc[num];
				if (num != NPC.whoAmI && other.type == NPC.type && other.active && Math.Abs(NPC.position.X - other.position.X) + Math.Abs(NPC.position.Y - other.position.Y) < NPC.width)
				{
					const float pushAway = 0.2f;
					if (NPC.position.X < other.position.X)
					{
						NPC.velocity.X -= pushAway;
					}
					else
					{
						NPC.velocity.X += pushAway;
					}
					if (NPC.position.Y < other.position.Y)
					{
						NPC.velocity.Y -= pushAway;
					}
					else
					{
						NPC.velocity.Y += pushAway;
					}
				}
			}
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.2f);
                    Main.dust[dustGore].color = Color.Cyan;
                    Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-5f, 5f);
                    Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-3f, 3f);
                    Main.dust[dustGore].noGravity = true;
                }
            }
        }
    }
}