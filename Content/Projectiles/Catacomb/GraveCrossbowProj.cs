using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.Catacomb
{
	public class GraveCrossbowProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
            Projectile.width = 66;
            Projectile.height = 88;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
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

		public override bool PreAI()
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

			if (player.channel) 
            {
                Projectile.timeLeft = 2;

                player.itemRotation = Projectile.rotation;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

				Projectile.position = player.position + new Vector2(-23, -25);
				player.velocity.X *= 0.98f;

                Projectile.localAI[0] += 0.25f;

                if (Projectile.localAI[0] == 5 || Projectile.localAI[0] == 10 || Projectile.localAI[0] == 15)
                {
                    Projectile.frame++;
                }

                if (Projectile.frame >= 3)
                {   
                    Projectile.frame = 2;
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
				if (Projectile.owner == Main.myPlayer)
				{
                    SoundEngine.PlaySound(SoundID.Item5, Projectile.Center);

                    Vector2 ShootSpeed = Main.MouseWorld - Projectile.Center;
                    ShootSpeed.Normalize();

                    int damage = Projectile.damage;

                    switch (Projectile.frame)
                    {
                        case 0:
                        {
                            ShootSpeed *= 10;
                            damage = Projectile.damage / 3;
                            break;
                        }
                        case 1:
                        {
                            ShootSpeed *= 15;
                            damage = Projectile.damage / 2;
                            break;
                        }
                        case 2:
                        {
                            ShootSpeed *= 25;
                            damage = Projectile.damage;
                            break;
                        }
                    }

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, ShootSpeed.X, ShootSpeed.Y, 
                    ModContent.ProjectileType<GraveCrossbowArrow>(), damage, Projectile.knockBack, Projectile.owner);
				}

				Projectile.active = false;
			}

			player.heldProj = Projectile.whoAmI;
			player.itemTime = 1;
			player.itemAnimation = 1;

			return true;
		}
	}
}