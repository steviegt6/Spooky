using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class WizardGangsterHead : ModItem, IExtendedHelmet, IHelmetGlowmask
	{
		public string ExtensionTexture => "Spooky/Content/Items/SpookyBiome/Armor/WizardGangsterHead_Hat";
        public Vector2 ExtensionSpriteOffset(PlayerDrawSet drawInfo) => new Vector2(0, -8f);

		public string GlowmaskTexture => "Spooky/Content/Items/SpookyBiome/Armor/WizardGangsterHead_Glow";

		public override void SetDefaults() 
		{
			Item.defense = 2;
			Item.width = 30;
			Item.height = 26;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<WizardGangsterBody>() && legs.type == ModContent.ItemType<WizardGangsterLegs>();
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawOutlines = true;
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.WizardGangsterArmor");

            if (player.HasItem(ItemID.PlatinumCoin))
            {
                player.GetDamage(DamageClass.Magic) += 0.2f;
			}
			else
			{
				float bonusPerGold = 0.02f;
				int numGoldCoins = player.CountItem(ItemID.GoldCoin);

				if (numGoldCoins < 10)
				{
					player.GetDamage(DamageClass.Magic) += bonusPerGold * numGoldCoins;
				}
				else
				{
					player.GetDamage(DamageClass.Magic) += 0.2f;
				}
			}
        }

		public override void UpdateEquip(Player player) 
		{
			player.GetCritChance(DamageClass.Magic) += 5;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.GoldBar, 8)
			.AddIngredient(ItemID.Silk, 8)
			.AddIngredient(ModContent.ItemType<SpookyGlowshroom>(), 20)
            .AddTile(TileID.Anvils)
            .Register();

			CreateRecipe()
            .AddIngredient(ItemID.PlatinumBar, 8)
			.AddIngredient(ItemID.Silk, 8)
			.AddIngredient(ModContent.ItemType<SpookyGlowshroom>(), 20)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}