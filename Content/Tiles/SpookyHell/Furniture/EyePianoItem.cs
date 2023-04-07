using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpookyHell.Furniture
{
	public class EyePianoItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 16;
			Item.height = 16;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.maxStack = 9999;
			Item.createTile = ModContent.TileType<EyePiano>();
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<LivingFleshItem>(), 15)
			.AddIngredient(ItemID.Bone, 4)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}