using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Tiles.SpookyHell.Chests
{
	public class SpookyHellChestItem : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Eye Chest");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 16;
			Item.height = 16;
			Item.useTime = 7;
			Item.useAnimation = 15;
			Item.useStyle = 1;
			Item.maxStack = 99;
			Item.createTile = ModContent.TileType<SpookyHellChest>();
		}
	}
}