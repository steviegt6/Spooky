using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class BigBoneHammerHit : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.alpha = 255;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 3;
        }

        public override void AI()
        {
            for (int numDusts = 0; numDusts < 30; numDusts++)
			{
                int dustGore = Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 1.5f);
                Main.dust[dustGore].color = Color.Yellow;
                Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-3f, 3f);
                Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-3f, 3f);
                Main.dust[dustGore].scale = 0.25f; 
                Main.dust[dustGore].noGravity = true;
            }
        }
    }
}