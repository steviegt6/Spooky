using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyHell.NoseTemple
{
	public class NoseTempleBrickPurple : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(108, 87, 148));
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
			//MinPick = int.MaxValue;
		}

		public override bool CanExplode(int i, int j)
        {
			return false;
        }
    }

	public class NoseTempleBrickPurpleSafe : ModTile
	{
		public override string Texture => "Spooky/Content/Tiles/SpookyHell/NoseTemple/NoseTempleBrickPurple";

		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(108, 87, 148));
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
		}
    }
}
