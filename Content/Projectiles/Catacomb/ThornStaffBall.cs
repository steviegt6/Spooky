using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class ThornStaffBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snot Ball");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
		
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 30;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 2000;
        }
        
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Color color = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0).MultiplyRGBA(Color.Yellow);

            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int numEffect = 0; numEffect < 2; numEffect++)
            {
                Color newColor = color;
                newColor = Projectile.GetAlpha(newColor);
                newColor *= 1f;
                Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (numEffect / 2 * 6.28318548f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(-1, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, vector, rectangle, newColor, Projectile.rotation, drawOrigin, Projectile.scale * 1.25f, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Main.rand.Next(3) == 0 && !target.HasBuff(ModContent.BuffType<EnsnaredCooldown>()))
            {
                target.AddBuff(ModContent.BuffType<EnsnaredDebuff>(), 120);
                target.AddBuff(ModContent.BuffType<EnsnaredCooldown>(), 720);
                target.AddBuff(BuffID.Poisoned, 120);
            }
        }

        public override void AI()
        {
			Projectile.rotation += 0.25f * (float)Projectile.direction;

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 20f)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;
            }
        }

		public override void Kill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Grass, Projectile.Center);

            for (int numDust = 0; numDust < 20; numDust++)
			{                                                                                  
				int DustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass, 0f, -2f, 0, default(Color), 1.5f);
				Main.dust[DustGore].noGravity = true;
				Main.dust[DustGore].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[DustGore].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                
				if (Main.dust[DustGore].position != Projectile.Center)
                {
				    Main.dust[DustGore].velocity = Projectile.DirectionTo(Main.dust[DustGore].position) * 2f;
                }
			}
		}
    }
}
     
          






