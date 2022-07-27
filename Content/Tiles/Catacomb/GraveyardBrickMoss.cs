using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Catacomb
{
	public class GraveyardBrickMoss : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			AddMapEntry(new Color(74, 86, 77));
			ItemDrop = ModContent.ItemType<GraveyardBrickMossItem>();
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
		}
	}
}
