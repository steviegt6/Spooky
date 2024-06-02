using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.NoseTemple
{
    public class NoseTempleFancyWallGreen : ModWall
    {
        private Asset<Texture2D> MergeTexture;

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(37, 43, 5));
            DustType = DustID.Stone;
        }

        public override bool CanExplode(int i, int j)
		{
			return false;
		}

        public override void KillWall(int i, int j, ref bool fail)
        {
            fail = true;
        }

        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1 ? Vector2.Zero : Vector2.One * 12;

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //wall merges
            MergeTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/NoseTemple/NoseTempleFancyWallMergeGreen");
            
            int Type = ModContent.WallType<NoseTempleWallGreen>();

            Tile Left = Main.tile[i - 1, j];
            Tile Right = Main.tile[i + 1, j];
            Tile Above = Main.tile[i, j - 1];
            Tile Below = Main.tile[i, j + 1];

            //top left
            if (Left.WallType == Type && Above.WallType == Type && !WorldGen.SolidTile(i - 1, j) && !WorldGen.SolidTile(i, j - 1))
            {
                spriteBatch.Draw(MergeTexture.Value, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 0, 0, 16, 16), Lighting.GetColor(i, j));
            }
            //top right
            if (Right.WallType == Type && Above.WallType == Type && !WorldGen.SolidTile(i + 1, j) && !WorldGen.SolidTile(i, j - 1))
            {
                spriteBatch.Draw(MergeTexture.Value, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 1, 0, 16, 16), Lighting.GetColor(i, j));
            }
            //bottom left
            if (Left.WallType == Type && Below.WallType == Type && !WorldGen.SolidTile(i - 1, j) && !WorldGen.SolidTile(i, j + 1))
            {
                spriteBatch.Draw(MergeTexture.Value, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 2, 0, 16, 16), Lighting.GetColor(i, j));
            }
            //bottom right
            if (Right.WallType == Type && Below.WallType == Type && !WorldGen.SolidTile(i + 1, j) && !WorldGen.SolidTile(i, j + 1))
            {
                spriteBatch.Draw(MergeTexture.Value, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 3, 0, 16, 16), Lighting.GetColor(i, j));
            }
        }
    }

    public class NoseTempleFancyWallGreenSafe : NoseTempleFancyWallGreen 
    {
        public override string Texture => "Spooky/Content/Tiles/NoseTemple/NoseTempleFancyWallGreen";

        private Asset<Texture2D> MergeTexture;

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(37, 43, 5));
            DustType = DustID.Stone;
        }

        public override bool CanExplode(int i, int j)
		{
			return true;
		}

        public override void KillWall(int i, int j, ref bool fail)
        {
            fail = false;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //wall merges
            MergeTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/NoseTemple/NoseTempleFancyWallMergeGreen");
            
            int Type = ModContent.WallType<NoseTempleWallGreenSafe>();

            Tile Left = Main.tile[i - 1, j];
            Tile Right = Main.tile[i + 1, j];
            Tile Above = Main.tile[i, j - 1];
            Tile Below = Main.tile[i, j + 1];

            //top left
            if (Left.WallType == Type && Above.WallType == Type && !WorldGen.SolidTile(i - 1, j) && !WorldGen.SolidTile(i, j - 1))
            {
                spriteBatch.Draw(MergeTexture.Value, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 0, 0, 16, 16), Lighting.GetColor(i, j));
            }
            //top right
            if (Right.WallType == Type && Above.WallType == Type && !WorldGen.SolidTile(i + 1, j) && !WorldGen.SolidTile(i, j - 1))
            {
                spriteBatch.Draw(MergeTexture.Value, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 1, 0, 16, 16), Lighting.GetColor(i, j));
            }
            //bottom left
            if (Left.WallType == Type && Below.WallType == Type && !WorldGen.SolidTile(i - 1, j) && !WorldGen.SolidTile(i, j + 1))
            {
                spriteBatch.Draw(MergeTexture.Value, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 2, 0, 16, 16), Lighting.GetColor(i, j));
            }
            //bottom right
            if (Right.WallType == Type && Below.WallType == Type && !WorldGen.SolidTile(i + 1, j) && !WorldGen.SolidTile(i, j + 1))
            {
                spriteBatch.Draw(MergeTexture.Value, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 3, 0, 16, 16), Lighting.GetColor(i, j));
            }
        }
    }
}