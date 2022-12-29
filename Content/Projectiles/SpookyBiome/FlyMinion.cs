using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class FlyMinion : ModProjectile
    {   
        int ChargeTimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fly");
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 26;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.aiStyle = 62;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 3)
                {
                    Projectile.frame = 0;
                }
            }

           	Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                var effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Green) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, drawPos, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            }
			
            return true;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction;

            ChargeTimer++;

            if (ChargeTimer == 180)
            {
                Vector2 mouse = new Vector2(Main.mouseX, Main.mouseY) + Main.screenPosition;

                Vector2 ChargeDirection = mouse - Projectile.Center;
                ChargeDirection.Normalize();
                        
                ChargeDirection.X *= 12;
                ChargeDirection.Y *= 12;  
                Projectile.velocity.X = ChargeDirection.X;
                Projectile.velocity.Y = ChargeDirection.Y;
            }

            if (ChargeTimer >= 180)
            {
                Projectile.spriteDirection = Projectile.direction;

                Projectile.tileCollide = true;
                Projectile.aiStyle = 0;
            }
            else
            {
                Projectile.tileCollide = false;
            }

            if (ChargeTimer >= 360)
            {
                Projectile.Kill();
            }
        }
        
        public override void Kill(int timeLeft)
		{
            for (int numDust = 0; numDust < 25; numDust++)
			{                                                                                 
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Asphalt, 0f, -2f, 0, default, 1.5f);
				Main.dust[newDust].noGravity = true;
				Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                
				if (Main.dust[newDust].position != Projectile.Center)
                {
				    Main.dust[newDust].velocity = Projectile.DirectionTo(Main.dust[newDust].position) * 2f;
                }
			}
        }
    }
}