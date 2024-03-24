using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Catacomb.Safe
{
	public class CatacombBrick2Safe : ModTile
	{
		public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrick2";

		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(87, 52, 37));
			DustType = DustID.t_Lihzahrd;
			HitSound = SoundID.Tink;
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 3 * 288; //288 is the width of each individual sheet
        }
    }
}
