using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Items.Food;
using Spooky.Content.NPCs.SpookyBiome.Projectiles;
using Spooky.Core;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class ZomboidArmored : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 45;
            NPC.damage = 15;
            NPC.defense = 5;
            NPC.width = 44;
			NPC.height = 46;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 3;
			AIType = NPCID.GoblinScout;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ZomboidArmored"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.Rain,
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
			if (NPC.localAI[2] == 0)
			{
				//running animation
				NPC.frameCounter++;
				if (NPC.frameCounter > 6 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 6)
				{
					NPC.frame.Y = 0 * frameHeight;
				}

				//jumping frame
				if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0)
				{
					NPC.frame.Y = 6 * frameHeight;
				}
			}
			else
			{
				NPC.frame.Y = 7 * frameHeight;
			}
        }

        public override bool CheckDead()
        {
            if (NPC.localAI[2] == 1)
            {
                NPC.immortal = true;
                NPC.dontTakeDamage = true;
                NPC.life = 1;

                NPC.localAI[2] = 2;

                NPC.netUpdate = true;

                return false;
            }

            return true;
        }
        
        public override void AI()
		{
			Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;

			int Damage = Main.masterMode ? 300 / 3 : Main.expertMode ? 200 / 2 : 100;

			if (NPC.localAI[2] == 0)
			{
				if (player.Distance(NPC.Center) <= 550f)
				{
					NPC.localAI[0]++;

					if (NPC.localAI[0] >= 360)
					{
						if (Main.rand.NextBool(10))
						{
							int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 100, default, 1.5f);
							Main.dust[dust].noGravity = true;
						}
					}

					if (NPC.localAI[0] >= 420)
					{
						NPC.velocity.X *= 0.75f;
					}

					if (NPC.localAI[0] == 480)
					{
						SpookyPlayer.ScreenShakeAmount = 10;

						Vector2 LightingPostion = new Vector2(NPC.Center.X, NPC.Center.Y - 1000);

						Vector2 ShootSpeed = NPC.Center - LightingPostion;
						ShootSpeed.Normalize();
						ShootSpeed *= 100f;

						Projectile.NewProjectile(null, LightingPostion, ShootSpeed, ModContent.ProjectileType<ZomboidArmoredLightning>(), Damage, 0f, player.whoAmI, ShootSpeed.ToRotation(), 100);
						Projectile.NewProjectile(null, LightingPostion - new Vector2(25, 0), ShootSpeed, ModContent.ProjectileType<ZomboidArmoredLightning>(), Damage, 0f, player.whoAmI, ShootSpeed.ToRotation(), 100);
						Projectile.NewProjectile(null, LightingPostion + new Vector2(25, 0), ShootSpeed, ModContent.ProjectileType<ZomboidArmoredLightning>(), Damage, 0f, player.whoAmI, ShootSpeed.ToRotation(), 100);

						Main.NewLightning();

						NPC.localAI[0] = 0;
					}
				}
			}
			else
            {
				NPC.aiStyle = -1;
				NPC.velocity.X *= 0.95f;

                NPC.localAI[1]++;

                if (NPC.localAI[1] > 120)
				{
                    NPC.immortal = false;
                    NPC.dontTakeDamage = false;
                    NPC.netUpdate = true;
                    player.ApplyDamageToNPC(NPC, NPC.lifeMax, 0, 0, false);
				}
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrankenMarshmallow>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0)
			{
				if (NPC.localAI[2] == 0)
				{
					for (int numGores = 1; numGores <= 6; numGores++)
					{
						if (Main.netMode != NetmodeID.Server)
						{
							Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidWindGore" + numGores).Type);
						}
					}
				}
				else
				{
					for (int numDusts = 0; numDusts < 20; numDusts++)
					{
						int CrumbleDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Asphalt, 0f, 0f, 100, default, 1.5f);
						Main.dust[CrumbleDust].velocity *= 1.2f;
					}
				}
            }
        }
    }
}