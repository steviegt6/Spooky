using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Tiles.SpookyBiome
{
    public class SpookySeedsGreen : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Green Spooky Seeds");
			Tooltip.SetDefault("Places grass on spooky dirt");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
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
			Item.useStyle = ItemUseStyleID.Swing;
			Item.maxStack = 999;
		}

        public override bool? UseItem(Player player)
		{
			Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);

			if ((tile.HasTile && tile.TileType == ModContent.TileType<SpookyDirt>() || tile.HasTile && tile.TileType == ModContent.TileType<SpookyDirt2>()) &&
			player.IsInTileInteractionRange(Player.tileTargetX, Player.tileTargetY))
			{
				Main.tile[Player.tileTargetX, Player.tileTargetY].TileType = (ushort)ModContent.TileType<SpookyGrassGreen>();

				return true;
			}

			return false;
		}
    }
}