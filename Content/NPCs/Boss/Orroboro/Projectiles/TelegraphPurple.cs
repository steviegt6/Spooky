using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.Orroboro.Projectiles
{
    public class TelegraphPurple : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Telegraph");
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 58;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 35;  
        }
       
        public override Color? GetAlpha(Color lightColor)
		{
			Color[] ColorList = new Color[]
            {
                new Color(140, 99, 201),
                new Color(88, 49, 129)
            };

            float fade = Main.GameUpdateCount % 20 / 20f;
			int index = (int)(Main.GameUpdateCount / 20 % 2);
			return Color.Lerp(ColorList[index], ColorList[(index + 1) % 2], fade);
		}

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] <= 30)
            {
                Projectile.alpha -= 10;

                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                }
            }
            
            Projectile.ai[1]++;
            if (Projectile.ai[1] < 10)
            {
                Projectile.scale += 0.1f;
            }
            if (Projectile.ai[1] >= 10)
            {
                Projectile.scale -= 0.1f;
            }

            if (Projectile.ai[1] > 20)
            {
                Projectile.ai[1] = 0;
                Projectile.scale = 1f;
            }
        }
    }
}