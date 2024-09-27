using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Tiles.NoseTemple
{
    public class NoseTempleBrickRedItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<NoseTempleBrickRedSafe>());
            Item.width = 16;
			Item.height = 16;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.GrayBrick)
            .AddTile(ModContent.TileType<Cauldron>())
            .Register();

            CreateRecipe()
            .AddIngredient(ModContent.ItemType<NoseTempleWallRedItem>(), 4)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}