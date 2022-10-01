using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.Projectiles.Catacomb;
 
namespace Spooky.Content.Items.Catacomb
{
	public class GraveCrossbow : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Old Hunter's Crossbow");
			Tooltip.SetDefault("Charge the crossbow to shoot a high damage piercing arrow");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 30;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 66;           
			Item.height = 32;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.knockBack = 2;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 8;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.Item17;
			Item.shoot = ModContent.ProjectileType<Blank>();
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 0f;
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<GraveCrossbowProj>(), damage, knockback, player.whoAmI, 0f, 0f);

			return false;
		}
	}
}
