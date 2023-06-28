using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.Sentient
{
	public class SentientBladeTongueProj : ModProjectile
	{
		public override void SetDefaults()
		{
            Projectile.width = 38;
            Projectile.height = 56;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
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
                Vector2 ProjDirection = Main.MouseWorld - player.Center;
                ProjDirection.Normalize();
                Projectile.ai[0] = ProjDirection.X;
				Projectile.ai[1] = ProjDirection.Y;
            }

            Vector2 direction = new Vector2(Projectile.ai[0], Projectile.ai[1]);

            Projectile.direction = Projectile.spriteDirection = direction.X > 0 ? 1 : -1;

            if (Projectile.direction >= 0)
            {
                Projectile.rotation = direction.ToRotation() - 1.57f * (float)Projectile.direction;
            }
            else
            {
                Projectile.rotation = direction.ToRotation() + 1.57f * (float)Projectile.direction;
            }

			if (Main.mouseRight)
            {
                Projectile.timeLeft = 2;

                player.itemRotation = Projectile.rotation;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

                Projectile.position = player.position + new Vector2(-8, -9);

                Projectile.ai[2]++;

                //shoot ichor clouds
                if (Projectile.ai[2] >= 20)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.Center);

                    for (int numProjectiles = 0; numProjectiles < 2; numProjectiles++)
                    {
                        Vector2 ShootSpeed = Main.MouseWorld - Projectile.Center;
                        ShootSpeed.Normalize();
                        ShootSpeed.X *= Main.rand.Next(10, 25);
                        ShootSpeed.Y *= Main.rand.Next(10, 25);

                        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 45f;

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X + muzzleOffset.X, Projectile.Center.Y + muzzleOffset.Y, 
                        ShootSpeed.X, ShootSpeed.Y, ModContent.ProjectileType<IchorCloud>(), Projectile.damage / 2, 0, Projectile.owner);
                    }

                    Projectile.ai[2] = 0;
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