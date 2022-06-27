using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Content.Tiles.SpookyHell.Tree
{
	public class EyeTree : ModTree
	{
		//copied from example mod, idk what this does yet
		public override TreePaintingSettings TreeShaderSettings => new TreePaintingSettings 
		{
			UseSpecialGroups = true,
			SpecialGroupMinimalHueValue = 11f / 72f,
			SpecialGroupMaximumHueValue = 0.25f,
			SpecialGroupMinimumSaturationValue = 0.88f,
			SpecialGroupMaximumSaturationValue = 1f
		};

		//the tiles this tree can grow on
		public override void SetStaticDefaults() 
		{
			GrowsOnTileId = new int[2] { ModContent.TileType<SpookyMush>(), ModContent.TileType<EyeBlock>() };
		}

		//the sapling that grows into this tree
		public override int SaplingGrowthType(ref int style)
		{
			style = 0;
			return ModContent.TileType<EyeSapling>();
		}

		//drop wood
		public override int DropWood()
		{
			return ModContent.ItemType<EyeBlockItem>();
		}

		//get the tree trunk texture
		public override Asset<Texture2D> GetTexture() 
		{
			return ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTree");
		}

		//branch Textures
		public override Asset<Texture2D> GetBranchTextures() 
		{
			return ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeBranches");
		}

		//top Textures
		public override Asset<Texture2D> GetTopTextures() 
		{
			return ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeTops");
		}

		//special settings for the tree tops
		public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
        {
			topTextureFrameWidth = 248;
			topTextureFrameHeight = 108;
			floorY = 2;
		}

        public override int TreeLeaf()
        {
			return -1;
		}

		public override bool Shake(int x, int y, ref bool createLeaves)
		{
			return false;
		}
	}
}