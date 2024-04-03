using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyHell
{
	public class NoseTempleFancyBrick : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(72, 47, 56));
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
			//MinPick = int.MaxValue;
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 3 * 288; //288 is the width of each individual sheet
			frameYOffset = j % 2 * 270; //288 is the width of each individual sheet
        }

       	public override bool CanExplode(int i, int j)
        {
			return false;
        }
    }
}
