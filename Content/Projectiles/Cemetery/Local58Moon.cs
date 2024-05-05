﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Cemetery
{
    public class Local58Moon : ModProjectile
    {
        int ParryDelay = 0;
        float Distance = 100f;

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 54;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.localNPCHitCooldown = 100;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
        }

        public override bool? CanDamage()
		{
            return !Main.dayTime;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead)
            {
                player.GetModPlayer<SpookyPlayer>().Local58Telescope = false;
            }

            if (player.GetModPlayer<SpookyPlayer>().Local58Telescope)
            {
                Projectile.timeLeft = 2;
            }

            if (!Main.dayTime && Distance < 250f)
            {
                Distance += 5f;
            }
            if (Main.dayTime && Distance > 100f)
            {
                Distance -= 5f;
            }

            Projectile.Center = player.Center + Projectile.ai[0].ToRotationVector2() * Distance;
            Projectile.rotation = Projectile.ai[0] + MathHelper.PiOver2 + MathHelper.PiOver4;
            Projectile.ai[0] -= MathHelper.ToRadians(Main.dayTime ? 1.5f : 3.5f);

            if (ParryDelay > 0)
            {
                ParryDelay--;
            }

            if (Main.dayTime && ParryDelay <= 0)
            {
                int damageToActivateParry = Main.masterMode ? 120 : Main.expertMode ? 90 : 50;

                for (int i = 0; i <= Main.maxProjectiles; i++)
                {
                    if (Projectile.Hitbox.Intersects(Main.projectile[i].Hitbox) && Main.projectile[i].hostile && Main.projectile[i].damage <= damageToActivateParry)
                    {
                        ParryDelay = 30;
                        SoundEngine.PlaySound(SoundID.Item150, Projectile.Center);
                        Main.projectile[i].velocity = -Main.projectile[i].velocity;
                    }
                }
            }
        }
	}
}