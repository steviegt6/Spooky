using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Relic
{
	public class BanditBookRelicItem : ModItem
	{
		public override void SetDefaults() 
        {
			Item.DefaultToPlaceableTile(ModContent.TileType<BanditBookRelic>());
			Item.master = true;
			Item.width = 16;
			Item.height = 16;
			Item.rare = ItemRarityID.Master;
			Item.value = Item.buyPrice(gold: 1);
		}
	}
}