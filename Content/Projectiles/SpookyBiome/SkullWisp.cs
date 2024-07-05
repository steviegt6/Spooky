using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.SpookyBiome
{
	public class SkullWisp : ModProjectile
	{
		bool runOnce = true;
		Vector2[] trailLength = new Vector2[6];

		private static Asset<Texture2D> TrailTexture;
        private static Asset<Texture2D> AuraTexture;

        public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 8;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Raven);
            Projectile.width = 20;
			Projectile.height = 28;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
			Projectile.minion = true;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.netImportant = true;
			Projectile.timeLeft = 2;
			Projectile.penetrate = -1;
			Projectile.minionSlots = 1;
			AIType = ProjectileID.Raven;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailCircle");

			Vector2 drawOrigin = new Vector2(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

				Color color = Color.Lerp(Color.Purple, Color.OrangeRed, scale);

				if (trailLength[k] == Vector2.Zero)
				{
					return true;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length() / (4 * scale);

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, color, Projectile.rotation, drawOrigin, scale * 0.5f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			//draw aura
            AuraTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/SpookyBiome/SkullWispAura");

            Vector2 auraDrawOrigin = new(AuraTexture.Width() * 0.5f, Projectile.height * 0.5f);

            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int numEffect = 0; numEffect < 2; numEffect++)
            {
				float shakeX = Main.rand.Next(-1, 2);
			    float shakeY = Main.rand.Next(-1, 2);

				Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Lerp(Color.Magenta, Color.Orange, numEffect));

                Vector2 vector = new Vector2(Projectile.Center.X - 1 + shakeX, Projectile.Center.Y + shakeY) + (numEffect / 4 * 6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY + 2) * numEffect;
                Rectangle rectangle = new(0, AuraTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, AuraTexture.Width(), AuraTexture.Height() / Main.projFrames[Projectile.type]);
				Main.EntitySpriteDraw(AuraTexture.Value, vector, rectangle, color, Projectile.rotation, auraDrawOrigin, Projectile.scale * 1.035f, effects, 0);
            }

			return true;
		}

		public override bool MinionContactDamage()
		{
			return true;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			Lighting.AddLight(Projectile.Center, 0.25f, 0.12f, 0f);

			if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
				}
				runOnce = false;
			}

			Vector2 current = new Vector2(Projectile.Center.X, Projectile.Center.Y + 5);
			for (int i = 0; i < trailLength.Length; i++)
			{
				Vector2 previousPosition = trailLength[i];
				trailLength[i] = current;
				current = previousPosition;
			}

			if (player.dead)
			{
				player.GetModPlayer<SpookyPlayer>().SkullWisp = false;
			}

			if (player.GetModPlayer<SpookyPlayer>().SkullWisp)
			{
				Projectile.timeLeft = 2;
			}

			//prevent projectiles clumping together
			for (int num = 0; num < Main.projectile.Length; num++)
			{
				Projectile other = Main.projectile[num];
				if (num != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
				{
					const float pushAway = 0.08f;
					if (Projectile.position.X < other.position.X)
					{
						Projectile.velocity.X -= pushAway;
					}
					else
					{
						Projectile.velocity.X += pushAway;
					}
					if (Projectile.position.Y < other.position.Y)
					{
						Projectile.velocity.Y -= pushAway;
					}
					else
					{
						Projectile.velocity.Y += pushAway;
					}
				}
			}
		}
	}
}