using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.SpookyHell
{
	public class LeechWhipProj : ModProjectile
	{
		private float ChargeTime 
		{
			get => Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		public override void SetStaticDefaults() 
		{
			//This makes the projectile use whip collision detection and allows flasks to be applied to it.
			ProjectileID.Sets.IsAWhip[Type] = true;
		}

		public override void SetDefaults() 
		{
			//This method quickly sets the whip's properties.
			Projectile.DefaultToWhip();

			//use these to change from the vanilla defaults
			Projectile.WhipSettings.Segments = 40;
			Projectile.WhipSettings.RangeMultiplier = 1.5f;
		}

		//This example uses PreAI to implement a charging mechanic.
		//If you remove this, also remove Item.channel = true from the item's SetDefaults.
		public override bool PreAI() 
		{
			Player owner = Main.player[Projectile.owner];

			// Like other whips, this whip updates twice per frame (Projectile.extraUpdates = 1), so 120 is equal to 1 second.
			if (!owner.channel || ChargeTime >= 120) 
			{
				return true; // Let the vanilla whip AI run.
			}

			if (++ChargeTime % 12 == 0) // 1 segment per 12 ticks of charge.
			{
				Projectile.WhipSettings.Segments++;
			}

			//Increase range up to 2x for full charge.
			Projectile.WhipSettings.RangeMultiplier += 1 / 180f;

			// Reset the animation and item timer while charging.
			owner.itemAnimation = owner.itemAnimationMax;
			owner.itemTime = owner.itemTimeMax;

			return false; // Prevent the vanilla whip AI from running.
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
			Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
			Projectile.damage = (int)(damageDone * 0.8f);

			if (Main.rand.Next(8) == 0)
            {
				Item.NewItem(Projectile.GetSource_DropAsItem(), target.position, target.Size, ModContent.ItemType<LeechWhipGore>());
            }
		}

		public override bool PreDraw(ref Color lightColor) 
        {
			List<Vector2> list = new();
			Projectile.FillWhipControlPoints(Projectile, list);

			Main.instance.LoadProjectile(Type);
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 pos = list[0];

			for (int i = 0; i < list.Count - 1; i++) 
            {
				//14 is the width of the whole whip, 26 is the height for the tips hotbox
				Rectangle frame = new(0, 0, 20, 26);
				Vector2 origin = new(5, 8);
				float scale = 1;

				// These statements determine what part of the spritesheet to draw for the current segment.
				// They can also be changed to suit your sprite.
				if (i == list.Count - 2) 
                {
					frame.Y = 72;
					frame.Height = 18;
				}
				//loop between the two middle segments
				else if (i % 2 == 0) 
				{
					frame.Y = 56;
					frame.Height = 16;
				}
				else if (i % 1 == 0) 
				{
					frame.Y = 40;
					frame.Height = 16;
				}
				else if (i > 0) 
                {
					frame.Y = 24;
					frame.Height = 24;
				}

				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
				Color color = Lighting.GetColor(element.ToTileCoordinates());

				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

				pos += diff;
			}

			return false;
		}
	}
}