using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
    public class RazorLeaf : ModProjectile
    {
        private List<Vector2> cache;
        private Trail trail;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Razor Leaf");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 18;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = ShaderLoader.GlowyTrail;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Spooky/ShaderAssets/EnergyTrail").Value); //trails texture image
            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.05f); //this affects something?
            effect.Parameters["repeats"].SetValue(1); //this is how many times the trail is drawn

            trail?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            return true;
        }

        const int TrailLength = 10;

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < TrailLength; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            cache.Add(Projectile.Center);

            while (cache.Count > TrailLength)
            {
                cache.RemoveAt(0);
            }
        }

		private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, TrailLength, new TriangularTip(4), factor => 10 * factor, factor =>
            {
                //use (* 1 - factor.X) at the end to make it fade at the beginning, or use (* factor.X) at the end to make it fade at the end
                return Color.Lerp(Color.SaddleBrown, Color.Red, factor.X) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;
            
            Lighting.AddLight(Projectile.Center, 0.85f, 0f, 0f);

            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }

            float WaveIntensity = 7.5f;
            float Wave = 10f;

            Projectile.ai[0]++;
            if (Projectile.ai[1] == 0)
            {
                if (Projectile.ai[0] > Wave * 0.5f)
                {
                    Projectile.ai[0] = 0;
                    Projectile.ai[1] = 1;
                }
                else
                {
                    Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(-WaveIntensity));
                    Projectile.velocity = perturbedSpeed;
                }
            }
            else
            {
                if (Projectile.ai[0] <= Wave)
                {
                    Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(WaveIntensity));
                    Projectile.velocity = perturbedSpeed;
                }
                else
                {
                    Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(-WaveIntensity));
                    Projectile.velocity = perturbedSpeed;
                }
                if (Projectile.ai[0] >= Wave * 2)
                {
                    Projectile.ai[0] = 0;
                }
            }
            
            //Projectile.velocity *= 1.00f;
        }
    }
}