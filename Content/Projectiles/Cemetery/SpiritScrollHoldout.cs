using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Cemetery
{
	public class SpiritScrollHoldout : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Spooky Scroll");
		}

		public override void SetDefaults()
		{
            Projectile.width = 26;
            Projectile.height = 28;
            Projectile.DamageType = DamageClass.Summon;
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

		public override bool PreAI()
		{
            Player player = Main.player[Projectile.owner];

            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 ProjDirection = Main.MouseWorld - Projectile.position;
                ProjDirection.Normalize();
                Projectile.ai[0] = ProjDirection.X;
				Projectile.ai[1] = ProjDirection.Y;
            }

            Vector2 direction = new Vector2(Projectile.ai[0], Projectile.ai[1]);

            Projectile.direction = Projectile.spriteDirection = direction.X > 0 ? 1 : -1;

			if (player.channel && player.statMana > 0)
            {
                Projectile.timeLeft = 2;

                player.itemRotation = Projectile.rotation;

                Projectile.position = player.position + new Vector2(direction.X > 0 ? 12 : -18, 2);
                player.bodyFrame.Y = player.bodyFrame.Height * 3;

                Projectile.localAI[0]++;

                if (Projectile.localAI[0] == 60)
                {
                    player.statMana -= 5;

                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy with { Volume = SoundID.DD2_EtherianPortalSpawnEnemy.Volume * 2.5f }, Projectile.Center);

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y - 10, 0, Main.rand.Next(-5, -3), 
                    ModContent.ProjectileType<ScrollPumpkin>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

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

			return true;
		}
	}
}