using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Quest.Projectiles
{
    public class SpiderWebFire : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/TrailSquare";

		int target;

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[12];

		private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.aiStyle = -1;
			Projectile.alpha = 255;
        }

		public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

				Color color = Color.Lerp(Color.White, Color.OrangeRed, scale);

				if (trailLength[k] == Vector2.Zero)
				{
					return true;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length();

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					//gives the projectile after images a shaking effect
					float x = Main.rand.Next(-2, 3) * scale;
					float y = Main.rand.Next(-2, 3) * scale;

					Main.spriteBatch.Draw(ProjTexture.Value, drawPos + new Vector2(x, y), null, color, Projectile.rotation, drawOrigin, scale * 1.2f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

		public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.4f, 0.3f, 0f);

			if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
				}
				runOnce = false;
			}

			Vector2 current = Projectile.Center;
			for (int i = 0; i < trailLength.Length; i++)
			{
				Vector2 previousPosition = trailLength[i];
				trailLength[i] = current;
				current = previousPosition;
			}

			Projectile.ai[0]++;

            if (Projectile.ai[0] >= 30)
            {
                Projectile.ai[1]++;

                if (Projectile.ai[1] < 80)
                {
                    Projectile.velocity *= 0.98f;
                }

                if (Projectile.ai[1] > 80 && Projectile.ai[1] < 120)
                {
                    if (Projectile.ai[0] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        target = -1;
                        float distance = 2000f;
                        for (int k = 0; k < 255; k++)
                        {
                            if (Main.player[k].active && !Main.player[k].dead)
                            {
                                Vector2 center = Main.player[k].Center;
                                float currentDistance = Vector2.Distance(center, Projectile.Center);
                                if (currentDistance < distance || target == -1)
                                {
                                    distance = currentDistance;
                                    target = k;
                                }
                            }
                        }
                        if (target != -1)
                        {
                            Projectile.ai[0] = 1;
                            Projectile.netUpdate = true;
                        }
                    }
                    else if (target >= 0 && target < Main.maxPlayers)
                    {
                        Player targetPlayer = Main.player[target];
                        if (!targetPlayer.active || targetPlayer.dead)
                        {
                            target = -1;
                            Projectile.ai[0] = 0;
                            Projectile.netUpdate = true;
                        }
                        else
                        {
                            float currentRot = Projectile.velocity.ToRotation();
                            Vector2 direction = targetPlayer.Center - Projectile.Center;
                            float targetAngle = direction.ToRotation();
                            if (direction == Vector2.Zero)
                            {
                                targetAngle = currentRot;
                            }

                            float desiredRot = currentRot.AngleLerp(targetAngle, 0.1f);
                            Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(desiredRot);
                        }
                    }

                    Projectile.velocity *= 1.055f;
                }
            }

			if (Projectile.timeLeft <= 85)
            {
                Projectile.ai[2]++;
                if (Projectile.ai[2] < 3)
                {
                    Projectile.scale -= 0.5f;
                }
                if (Projectile.ai[2] >= 3)
                {
                    Projectile.scale += 0.5f;
                }
                
                if (Projectile.ai[2] > 6)
                {
                    Projectile.ai[2] = 0;
                    Projectile.scale = 1f;
                }
            }
        }

        public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, Projectile.Center);

        	float maxAmount = 30;
			int currentAmount = 0;
			while (currentAmount <= maxAmount)
			{
				Vector2 velocity = new Vector2(5f, 5f);
				Vector2 Bounds = new Vector2(3f, 3f);
				float intensity = 5f;

				Vector2 vector12 = Vector2.UnitX * 0f;
				vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
				vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
				int num104 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Torch, 0f, 0f, 100, default, 2f);
				Main.dust[num104].noGravity = true;
				Main.dust[num104].position = Projectile.Center + vector12;
				Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
				currentAmount++;
			}
		}
    }
}