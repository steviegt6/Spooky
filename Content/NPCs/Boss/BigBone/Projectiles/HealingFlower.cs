using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
	public class HealingFlower : ModNPC
	{
		int HealingAmount = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Healing Flower");

			NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 600;
            NPC.damage = 0;
            NPC.defense = 20;
            NPC.width = 30;
            NPC.height = 30;
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
					Vector2 rootPosition = Main.npc[k].Center;

					Vector2[] bezierPoints = { rootPosition, rootPosition + new Vector2(0, -30), NPC.Center + new Vector2(-30 * NPC.direction, 0).RotatedBy(NPC.rotation), NPC.Center + new Vector2(0, 0).RotatedBy(NPC.rotation) };
					float bezierProgress = 0;
					float bezierIncrement = 8;

					Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/Projectiles/HealingFlowerChain").Value;
					Vector2 textureCenter = new Vector2(8, 8);

					float rotation;

					while (bezierProgress < 1)
					{
						//draw stuff
						Vector2 oldPos = BezierCurveUtil.BezierCurve(bezierPoints, bezierProgress);

						//increment progress
						while ((oldPos - BezierCurveUtil.BezierCurve(bezierPoints, bezierProgress)).Length() < bezierIncrement)
						{
							bezierProgress += 0.1f / BezierCurveUtil.BezierCurveDerivative(bezierPoints, bezierProgress).Length();
						}

						Vector2 newPos = BezierCurveUtil.BezierCurve(bezierPoints, bezierProgress);
						rotation = (newPos - oldPos).ToRotation() + MathHelper.Pi;

						spriteBatch.Draw(texture, (oldPos + newPos) / 2 - Main.screenPosition, texture.Frame(), drawColor, rotation, textureCenter, NPC.scale, SpriteEffects.None, 0f);
					}
				}
			}

            return true;
        }

		public override void AI()
		{
			Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

			for (int k = 0; k < Main.maxNPCs; k++)
			{
				if (Main.npc[k].active && Main.npc[k].type == ModContent.NPCType<BigBone>()) 
				{
					NPC.ai[0]++;
					if (NPC.ai[0] >= 120 && Main.npc[k].life < Main.npc[k].lifeMax && HealingAmount > 0)
					{
						Main.npc[k].life += HealingAmount;
						Main.npc[k].HealEffect(HealingAmount, true);
						NPC.ai[0] = 0;
					}
				}
			}

			NPC.ai[1]++;
			if (NPC.ai[1] >= 360)
			{
				HealingAmount = Main.masterMode ? 70 : Main.expertMode ? 50 : 20;
			}
			if (NPC.ai[1] >= 720)
			{
				HealingAmount = Main.masterMode ? 90 : Main.expertMode ? 70 : 40;
			}
			if (NPC.ai[1] >= 1080)
			{
				HealingAmount = Main.masterMode ? 115 : Main.expertMode ? 95 : 65;
			}
		}
	}
}