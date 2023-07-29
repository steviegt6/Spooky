using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Catacomb
{
	public class FlameIdolProj : ModProjectile
	{
		public override void SetDefaults()
		{
            Projectile.width = 50;
            Projectile.height = 70;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
		}

        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Catacomb/FlameIdolProjGlow").Value;
            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            for (int numEffect = 0; numEffect < 4; numEffect++)
            {
                float shakeX = Main.rand.Next(-2, 2);
			    float shakeY = Main.rand.Next(-2, 2);

                Vector2 vector = new Vector2(Projectile.Center.X - 1 + shakeX, Projectile.Center.Y + shakeY) + (numEffect / 4 * 6f + Projectile.rotation).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, vector, rectangle, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
			return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

		public override void AI()
		{
            Player player = Main.player[Projectile.owner];

            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 ProjDirection = Main.MouseWorld - player.position;
                ProjDirection.Normalize();
                Projectile.ai[0] = ProjDirection.X;
				Projectile.ai[1] = ProjDirection.Y;
            }

            Vector2 direction = new Vector2(Projectile.ai[0], Projectile.ai[1]);

            Projectile.direction = Projectile.spriteDirection = direction.X > 0 ? 1 : -1;

			if (player.channel && player.statMana > 0)
            {
                player.AddBuff(BuffID.OnFire, 2);

                Projectile.timeLeft = 2;

                player.itemRotation = Projectile.rotation;

                Projectile.position = player.position + new Vector2(direction.X > 0 ? -4 : -25, -30);
                player.bodyFrame.Y = player.bodyFrame.Height * 3;

                Projectile.localAI[0]++;

                if (Projectile.localAI[0] >= 60)
                {
                    player.statMana -= 10;

                    SoundEngine.PlaySound(SoundID.Item42, Projectile.Center);

                    for (int numProjectiles = 0; numProjectiles < 5; numProjectiles++)
                    {
                        int[] Types = new int[] { ProjectileID.GreekFire1, ProjectileID.GreekFire2, ProjectileID.GreekFire3 };

                        Vector2 Speed = new Vector2(8f, 0f).RotatedByRandom(2 * Math.PI);
                        Vector2 realSpeed = Speed.RotatedBy(2 * Math.PI / 2 * (numProjectiles + Main.rand.NextDouble() - 0.5));
                        Vector2 Position = new Vector2(Projectile.Center.X + Main.rand.Next(-20, 20), Projectile.Center.Y + Main.rand.Next(-20, 20));

                        int GreekFire = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Position, realSpeed, 
                        Main.rand.Next(Types), Projectile.damage, 0f, Main.myPlayer, 0, 0);
                        Main.projectile[GreekFire].DamageType = DamageClass.Magic;
                        Main.projectile[GreekFire].friendly = true;
                        Main.projectile[GreekFire].hostile = false;
                    }

                    Projectile.localAI[0] = 0;
                }

                if (direction.X > 0) 
                {
					player.direction = 1;
				}
				else 
                {
					player.direction = -1;
				}
            }
            else
            {
				Projectile.active = false;
            }

			player.heldProj = Projectile.whoAmI;
			player.itemTime = 1;
			player.itemAnimation = 1;
		}
	}
}