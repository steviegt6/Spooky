using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Dusts;
using Spooky.Content.Tiles.Cemetery.Ambient;

namespace Spooky.Content.Tiles.Cemetery
{
	public class CemeteryGrass : ModTile
	{
		public override void SetStaticDefaults()
		{
            //grass properties
            TileID.Sets.Conversion.Grass[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.JungleSpecial[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<CemeteryDirt>();
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(70, 130, 93));
			ItemDrop = ModContent.ItemType<CemeteryDirtItem>();
            DustType = ModContent.DustType<CemeteryGrassDust>();
		}

        public override void RandomUpdate(int i, int j)
        {
            Tile Tile = Framing.GetTileSafely(i, j);
			Tile Below = Framing.GetTileSafely(i, j + 1);
            Tile Above = Framing.GetTileSafely(i, j - 1);

            if (!Above.HasTile && Above.LiquidType <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
            {
                //grow small weeds
                if (Main.rand.Next(5) == 0)
                {
                    Above.TileType = (ushort)ModContent.TileType<CemeteryWeeds>();
                    Above.HasTile = true;
                    Above.TileFrameY = 0;
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(6) * 18);
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server) 
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                    }
				}

                if (Main.rand.Next(8) == 0) 
                {
                    ushort[] TallWeed = new ushort[] { (ushort)ModContent.TileType<CemeteryWeedsTall1>(), (ushort)ModContent.TileType<CemeteryWeedsTall2>(),
                    (ushort)ModContent.TileType<CemeteryWeedsTall3>(), (ushort)ModContent.TileType<CemeteryWeedsTall4>(),
                    (ushort)ModContent.TileType<CemeteryWeedsTall5>(), (ushort)ModContent.TileType<CemeteryWeedsTall6>() };

                    ushort newObject = Main.rand.Next(TallWeed);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacment(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }
            }

            //spread grass
            List<Point> adjacents = OpenAdjacents(i, j, ModContent.TileType<CemeteryDirt>());

            if (adjacents.Count > 0)
            {
                Point tilePoint = adjacents[Main.rand.Next(adjacents.Count)];
                if (HasOpening(tilePoint.X, tilePoint.Y))
                {
                    Framing.GetTileSafely(tilePoint.X, tilePoint.Y).TileType = (ushort)ModContent.TileType<CemeteryGrass>();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, tilePoint.X, tilePoint.Y, 1, TileChangeType.None);
                    }
                }
            }
        }

        private List<Point> OpenAdjacents(int i, int j, int type)
        {
            var tileList = new List<Point>();

            for (int k = -1; k < 2; ++k)
            {
                for (int l = -1; l < 2; ++l)
                {
                    if (!(l == 0 && k == 0) && Framing.GetTileSafely(i + k, j + l).HasTile && Framing.GetTileSafely(i + k, j + l).TileType == type)
                    {
                        tileList.Add(new Point(i + k, j + l));
                    }
                }
            }

            return tileList;
        }

        private bool HasOpening(int i, int j)
        {
            for (int k = -1; k < 2; k++)
            {
                for (int l = -1; l < 2; l++)
                {
                    if (!Framing.GetTileSafely(i + k, j + l).HasTile)
                    {
                        return true;
                    }
                }
            }
                    
            return false;
        }
	}
}
