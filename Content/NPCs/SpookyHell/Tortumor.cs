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
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.SpookyHell.Projectiles;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class Tortumor : ModNPC
    {
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        public static readonly SoundStyle ScreechSound = new("Spooky/Content/Sounds/TumorScreech1", SoundType.Sound) { PitchVariance = 0.6f };

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 85;
            NPC.damage = 45;
            NPC.defense = 5;
            NPC.width = 58;
            NPC.height = 62;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath22;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Tortumor"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/TortumorGlow").Value;

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            //regular anims
            if (NPC.ai[0] <= 420)
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
            if (NPC.ai[0] == 420)
            {
                NPC.frame.Y = 4 * frameHeight;
            }
            if (NPC.ai[0] > 420)
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

            if (!NPC.HasBuff(BuffID.Confused))
            {
			    NPC.ai[0]++;  
            }
            else
            {
                NPC.ai[0] = 0;
            }

            //dust spawning for when big tumor summons this enemy
            if (NPC.ai[0] == -1)
            {
                for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, Main.rand.NextFloat(1f, 2f));
                    Main.dust[DustGore].velocity *= 3f;
                    Main.dust[DustGore].noGravity = true;

                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[DustGore].scale = 0.5f;
                        Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
            }

            if (NPC.ai[0] <= 420)
            {
                int MaxSpeed = 25;

                if (NPC.HasBuff(BuffID.Confused))
                {
                    MaxSpeed = -25;
                }

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

            if (NPC.ai[0] >= 420)
            {
                if (NPC.ai[0] == 420)
                {
                    SoundEngine.PlaySound(ScreechSound, NPC.Center);
                }

                NPC.velocity *= 0.95f;

                if (NPC.ai[0] == 495)
                {
                    for (int numDusts = 0; numDusts < 20; numDusts++)
                    {
                        int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, Main.rand.NextFloat(1f, 2f));
                        Main.dust[DustGore].velocity *= 3f;
                        Main.dust[DustGore].noGravity = true;

                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[DustGore].scale = 0.5f;
                            Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                        }
                    }

                    Vector2 vector = Vector2.UnitY.RotatedByRandom(1.57079637050629f) * new Vector2(5f, 3f);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X + Main.rand.Next(-50, 50), NPC.Center.Y + Main.rand.Next(-50, 50), 
                        vector.X, vector.Y, ModContent.ProjectileType<TumorOrb1>(), NPC.damage / 2, 0f, Main.myPlayer, 0f, (float)NPC.whoAmI);

                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X + Main.rand.Next(-50, 50), NPC.Center.Y + Main.rand.Next(-50, 50), 
                        vector.X, vector.Y, ModContent.ProjectileType<TumorOrb2>(), NPC.damage / 2, 0f, Main.myPlayer, 0f, (float)NPC.whoAmI);

                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X + Main.rand.Next(-50, 50), NPC.Center.Y + Main.rand.Next(-50, 50), 
                        vector.X, vector.Y, ModContent.ProjectileType<TumorOrb3>(), NPC.damage / 2, 0f, Main.myPlayer, 0f, (float)NPC.whoAmI);
                    }
                }

                if (NPC.ai[0] >= 510)
                {
                    NPC.ai[0] = 0;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TortumorStaff>(), 35));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MonsterBloodVial>(), 100));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EyeChocolate>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit) 
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
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TortumorGore" + numGores).Type);
                }

                for (int numDusts = 0; numDusts < 25; numDusts++)
                {
                    int newDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                    Main.dust[newDust].velocity.X *= Main.rand.Next(-8, 8);
                    Main.dust[newDust].velocity.Y *= Main.rand.Next(-8, 8);
                    Main.dust[newDust].noGravity = true;

                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[newDust].scale = 0.5f;
                        Main.dust[newDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
            }
        }
    }
}