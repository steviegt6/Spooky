using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

using Spooky.Core;
using Spooky.Content.Projectiles.SpookyHell;
using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class EyeHead : ModItem, IHelmetGlowmask
	{
		public string GlowmaskTexture => "Spooky/Content/Items/SpookyHell/Armor/EyeHead_Glow";

		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 38;
			Item.height = 28;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<EyeBody>() && legs.type == ModContent.ItemType<EyeLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.EyeArmor");
			player.GetModPlayer<SpookyPlayer>().EyeArmorSet = true;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Summon) += 0.08f;
			player.GetDamage(DamageClass.SummonMeleeSpeed) += 0.08f;
			player.maxMinions += 1;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
			.AddIngredient(ItemID.DemoniteBar, 8)
			.AddIngredient(ModContent.ItemType<LivingFleshItem>(), 50)
            .AddTile(TileID.Anvils)
            .Register();

			CreateRecipe()
			.AddIngredient(ItemID.CrimtaneBar, 8)
			.AddIngredient(ModContent.ItemType<LivingFleshItem>(), 50)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}
