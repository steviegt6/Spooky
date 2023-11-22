using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpiderCave
{
    public class TarantulaHawk1 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 600;
            NPC.damage = 45;
            NPC.defense = 15;
            NPC.width = 66;
            NPC.height = 58;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit32;
			NPC.DeathSound = SoundID.NPCDeath38;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TarantulaHawk1"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;

            if (NPC.frameCounter > 3)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0;
            }
        }

        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);
            
            NPC.spriteDirection = NPC.direction;

            switch ((int)NPC.localAI[0])
            {
                //fly to random locations, shoot blood bolts
                case 0:
                {
                    NPC.aiStyle = 5;
			        AIType = NPCID.Moth;

                    NPC.localAI[1]++;

                    if (NPC.localAI[1] > 300)
                    {
                        //do not charge at the player if they are too far or they are not within line of sight
                        if (Vector2.Distance(player.Center, NPC.Center) <= 250f)
                        {
                            NPC.localAI[1] = 0;
                            NPC.localAI[0]++;

                            NPC.netUpdate = true;
                        }
                    }

                    break;
                }

                //charge at the player
                case 1:
                {
                    NPC.aiStyle = -1;
                    NPC.rotation = 0;

                    NPC.localAI[1]++;

                    if (NPC.localAI[1] < 60)
                    {
                        Vector2 GoTo = player.Center;
                        GoTo.X += (NPC.Center.X < player.Center.X) ? -170 : 170;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 8, 12);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    if (NPC.localAI[1] == 60)
                    {
                        NPC.velocity *= 0;
                    }

                    if (NPC.localAI[1] == 65)
                    {
                        SoundEngine.PlaySound(SoundID.Zombie77, NPC.Center);

                        Vector2 ChargeDirection = player.Center - NPC.Center;
                        ChargeDirection.Normalize();
                                
                        ChargeDirection.X = ChargeDirection.X * 30;
                        ChargeDirection.Y = ChargeDirection.Y * 5;
                        NPC.velocity.X = ChargeDirection.X;
                        NPC.velocity.Y = ChargeDirection.Y;
                    }

                    if (NPC.localAI[1] >= 65)
                    {
                        NPC.spriteDirection = NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
                    }

                    if (NPC.localAI[1] >= 90)
                    {
                        NPC.velocity *= 0.65f;
                    }

                    if (NPC.localAI[1] >= 120)
                    {   
                        NPC.localAI[1] = 0;
                        NPC.localAI[0] = 0;

                        NPC.netUpdate = true;
                    }

                    break;
                }
            }
        }
    }

    public class TarantulaHawk2 : TarantulaHawk1
    {   
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TarantulaHawk2"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
        }
    }

    public class TarantulaHawk3 : TarantulaHawk1
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TarantulaHawk3"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
        }
    }
}