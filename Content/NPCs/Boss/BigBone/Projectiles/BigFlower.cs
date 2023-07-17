using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
	public class BigFlower : ModNPC
	{
		public static readonly SoundStyle MagicCastSound = new("Spooky/Content/Sounds/BigBone/BigBoneMagic2", SoundType.Sound) { PitchVariance = 0.6f };

		public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 5200;
            NPC.damage = 0;
            NPC.defense = 25;
            NPC.width = 62;
            NPC.height = 60;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			for (int k = 0; k < Main.maxNPCs; k++)
			{
				if (Main.npc[k].active && Main.npc[k].type == ModContent.NPCType<BigBone>()) 
				{
					Vector2 ParentCenter = Main.npc[k].Center;

					Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/Projectiles/BigFlowerChain");

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
						Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

						var chainTextureToDraw = chainTexture;

						Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

						chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
						chainCount++;
						chainLengthRemainingToDraw -= chainSegmentLength;
					}
				}
			}

			float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

			Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/Projectiles/BigFlowerGlow").Value;

			Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.DarkOrange);

			for (int numEffect = 0; numEffect < 5; numEffect++)
			{
				Color newColor = color;
				newColor = NPC.GetAlpha(newColor);
				newColor *= 1f - fade;
				Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4);
				Main.EntitySpriteDraw(tex, vector, NPC.frame, newColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.05f + fade / 3, SpriteEffects.None, 0);
			}

            return true;
        }

		public override void AI()
		{
			Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

			int Damage = Main.masterMode ? 90 / 3 : Main.expertMode ? 70 / 2 : 50;

			Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

			for (int k = 0; k < Main.maxNPCs; k++)
			{
				if (Main.npc[k].active && Main.npc[k].type == ModContent.NPCType<BigBone>()) 
				{
					NPC.ai[0] = Main.npc[k].localAI[2];
				}
			}

			if (NPC.ai[0] == 350)
			{
				SoundEngine.PlaySound(MagicCastSound, NPC.Center);
			}

			if (NPC.ai[0] >= 350 && NPC.ai[0] <= 370)
			{
				int MaxDusts = Main.rand.Next(10, 20);
				for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
				{
					Vector2 dustPos = (Vector2.One * new Vector2((float)NPC.width / 3f, (float)NPC.height / 3f) * 1.5f).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + NPC.Center;
					Vector2 velocity = dustPos - NPC.Center;
					int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, DustID.OrangeTorch, velocity.X * 2f, velocity.Y * 2f, 100, default, 1.4f);
					Main.dust[dustEffect].noGravity = true;
					Main.dust[dustEffect].noLight = false;
					Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * Main.rand.NextFloat(-5f, -2f);
					Main.dust[dustEffect].fadeIn = 1.3f;
				}
			}

			if (NPC.ai[0] == 370)
			{
				SoundEngine.PlaySound(SoundID.Item42, NPC.Center);

				Vector2 ShootSpeed = player.Center - NPC.Center;
				ShootSpeed.Normalize();
						
				ShootSpeed.X *= 15;
				ShootSpeed.Y *= 15;

				Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 70f;
				Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

				if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
				{
					position += muzzleOffset;
				}

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.NewProjectile(NPC.GetSource_FromAI(), position.X, position.Y, ShootSpeed.X, 
					ShootSpeed.Y, ModContent.ProjectileType<MassiveFlameBallBolt>(), Damage, 1, Main.myPlayer, 0, 0);
				}
			}
		}

		public override bool CheckDead() 
		{
            if (Main.netMode != NetmodeID.Server) 
            {
                for (int numGores = 1; numGores <= 6; numGores++)
                {
					if (Main.rand.NextBool(2))
					{
                    	Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BigFlowerGore1").Type);
					}
					else
					{
						Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BigFlowerGore2").Type);
					}
                }
            }

			for (int numDust = 0; numDust < 25; numDust++)
            {
                int DustGore = Dust.NewDust(new Vector2(NPC.Center.X, NPC.Center.Y), NPC.width / 2, NPC.height / 2, DustID.Grass, 0f, 0f, 100, default, 2f);

                Main.dust[DustGore].velocity *= 3f;
                Main.dust[DustGore].noGravity = true;

                if (Main.rand.NextBool(2))
                {
                    Main.dust[DustGore].scale = 0.5f;
                    Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }

            return true;
		}

		//Loot and stuff
        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
			var parameters = new DropOneByOne.Parameters() 
			{
				ChanceNumerator = 1,
				ChanceDenominator = 1,
				MinimumStackPerChunkBase = 1,
				MaximumStackPerChunkBase = 1,
				MinimumItemDropsCount = 2,
				MaximumItemDropsCount = 3,
			};

			npcLoot.Add(new DropOneByOne(ItemID.Heart, parameters));
        }
	}
}