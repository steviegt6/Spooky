using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Dusts;
using Spooky.Content.Tiles.SpiderCave.Ambient;
using Spooky.Content.Tiles.SpiderCave.Mushrooms;

namespace Spooky.Content.Tiles.SpiderCave
{
	public class DampGrass : ModTile
	{
		public override void SetStaticDefaults()
		{
            TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<DampSoil>();
            Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(137, 115, 47));
            RegisterItemDrop(ModContent.ItemType<DampSoilItem>());
            DustType = ModContent.DustType<DampGrassDust>();
			MineResist = 0.65f;
		}

		public override void RandomUpdate(int i, int j)
        {
            Tile Tile = Framing.GetTileSafely(i, j);
			Tile Below = Framing.GetTileSafely(i, j + 1);
            Tile Above = Framing.GetTileSafely(i, j - 1);

			if (!Below.HasTile && Below.LiquidType <= 0 && !Tile.BottomSlope) 
            {
                if (Main.rand.Next(8) == 0) 
                {
                    Below.TileType = (ushort)ModContent.TileType<DampVines>();
                    Below.HasTile = true;
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server) 
                    {
                        NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                    }
                }
            }

			if (!Above.HasTile && Above.LiquidType <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
            {
                //grow small weeds
                if (Main.rand.Next(3) == 0)
                {
                    Above.TileType = (ushort)ModContent.TileType<SpiderCaveWeeds>();
                    Above.HasTile = true;
                    Above.TileFrameY = 0;
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(16) * 18);
                    WorldGen.SquareTileFrame(i, j + 1, true);

                    if (Main.netMode == NetmodeID.Server) 
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                    }
                }

                //mushrooms 
                if (Main.rand.NextBool(18))
                {
                    ushort[] Shrooms = new ushort[] { (ushort)ModContent.TileType<MushroomGreen1>(), (ushort)ModContent.TileType<MushroomGreen2>(),
                    (ushort)ModContent.TileType<MushroomGreen3>(), (ushort)ModContent.TileType<MushroomGreen4>(),
                    (ushort)ModContent.TileType<MushroomOrange1>(), (ushort)ModContent.TileType<MushroomOrange2>(),
                    (ushort)ModContent.TileType<MushroomOrange3>(), (ushort)ModContent.TileType<MushroomOrange4>() };

                    ushort newObject = Main.rand.Next(Shrooms);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }

                //giant mushrooms
                if (Main.rand.NextBool(35))
                {
                    ushort[] Shrooms = new ushort[] { (ushort)ModContent.TileType<GiantShroomGreen1>(), (ushort)ModContent.TileType<GiantShroomGreen2>(),
                    (ushort)ModContent.TileType<GiantShroomOrange1>(), (ushort)ModContent.TileType<GiantShroomOrange2>() };

                    ushort newObject = Main.rand.Next(Shrooms);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }
			}

            //spread grass
            List<Point> adjacents = OpenAdjacents(i, j, ModContent.TileType<DampSoil>());

            if (adjacents.Count > 0)
            {
                Point tilePoint = adjacents[Main.rand.Next(adjacents.Count)];
                if (HasOpening(tilePoint.X, tilePoint.Y))
                {
                    Framing.GetTileSafely(tilePoint.X, tilePoint.Y).TileType = (ushort)ModContent.TileType<DampGrass>();

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
