using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Tiles.Catacomb.Ambient;

namespace Spooky.Content.Items
{
    public class CatacombWeedKiller : ModItem
    {
        public override string Texture => "Spooky/Content/Items/BossSummon/CowBell";

        public override void SetDefaults()
        {                
            Item.width = 20;
            Item.height = 28;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
        }

        public override bool? UseItem(Player player)
        {
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Tile tile = Main.tile[i, j];

                    if (tile.TileType == ModContent.TileType<CatacombVines>() || tile.TileType == ModContent.TileType<CatacombVines2>() ||
                    tile.TileType == ModContent.TileType<CatacombWeeds>() || tile.TileType == ModContent.TileType<SporeMushroom>())
                    {
                        WorldGen.KillTile(i, j);
                    }
                }
            }

            return true;
        }
    }
}