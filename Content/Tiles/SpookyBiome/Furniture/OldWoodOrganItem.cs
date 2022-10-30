using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Tiles.SpookyBiome.Furniture
{
	public class OldWoodOrganItem : ModItem
    {
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Old Wood Organ");
			Tooltip.SetDefault("Right click to play the organ\nBut, it can only play Toccata and Fugue in D minor");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 16;
			Item.height = 16;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = 1;
			Item.maxStack = 99;
			Item.createTile = ModContent.TileType<OldWoodOrgan>();
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 15)
			.AddRecipeGroup(RecipeGroupID.IronBar, 5)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}