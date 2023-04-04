using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
	public class HealingFlowerSeed : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Flower Seed");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 18;                   			 
            Projectile.height = 16;         
			Projectile.hostile = true;                                 			  		
            Projectile.tileCollide = true;
			Projectile.ignoreWater = false;            					
            Projectile.timeLeft = 60;
		}

		public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new Vector2(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Color.Lerp(Color.Red, Color.Chocolate, oldPos / (float)Projectile.oldPos.Length) * 0.65f * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new Rectangle(0, (tex.Height / Main.projFrames[Projectile.type]) * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }

		public override void AI()
		{
			Lighting.AddLight(Projectile.Center, 0.3f, 0f, 0f);

            Projectile.rotation += 0.25f * Projectile.direction; 

			Projectile.ai[0]++;
			if (Projectile.ai[0] > 20 && Main.rand.Next(45) == 0)
			{
				Projectile.Kill();
			}
		}

		public override void Kill(int timeLeft)
		{
			for (int numDust = 0; numDust < 20; numDust++)
			{                                                                                  
				int DustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, 0f, -2f, 0, default(Color), 1.5f);
				Main.dust[DustGore].noGravity = true;
				Main.dust[DustGore].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[DustGore].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                
				if (Main.dust[DustGore].position != Projectile.Center)
                {
				    Main.dust[DustGore].velocity = Projectile.DirectionTo(Main.dust[DustGore].position) * 2f;
                }
			}

			if (NPC.CountNPCS(ModContent.NPCType<HealingFlower>()) <= 12)
			{		
				int Flower = NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<HealingFlower>());

				//net update so it doesnt vanish on multiplayer
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: Flower);
				}
			}
		}
	}
}