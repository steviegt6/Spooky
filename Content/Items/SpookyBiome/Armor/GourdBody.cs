using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[LegacyName("SpookyBody")]
	[AutoloadEquip(EquipType.Body)]
	public class GourdBody : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 5;
			Item.width = 28;
			Item.height = 22;
			Item.rare = ItemRarityID.Blue;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Melee) += 0.1f;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RottenChunk>(), 25)
			.AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 35)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}