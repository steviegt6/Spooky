using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Tiles.Catacomb.Safe;
using Spooky.Content.Tiles.Cemetery;

namespace Spooky.Content.Tiles.Catacomb
{
    public class GildedBrickItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 16;
			Item.height = 16;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.maxStack = 9999;
			Item.createTile = ModContent.TileType<GildedBrickSafe>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(25)
            .AddIngredient(ModContent.ItemType<CatacombBrick2Item>(), 25)
            .AddIngredient(ItemID.GoldOre)
            .AddTile(TileID.Furnaces)
            .Register();
        }
    }
}