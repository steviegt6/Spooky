using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.SpookyHell.Furniture
{
	public class EyeLantern : ModTile
    {
		public override void SetStaticDefaults()
		{
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newSubTile.LavaDeath = false;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
            TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(114, 13, 39), name);
            DustType = DustID.Blood;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AdjTiles = new int[] { TileID.HangingLanterns };
        }

        public override void NumDust(int i, int j, bool fail, ref int num) 
        {
            num = fail ? 1 : 3;
        }

        public override void HitWire(int i, int j)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 1;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 2;
            for (int x = left; x < left + 1; x++)
            {
                for (int y = top; y < top + 2; y++)
                {
                    if (Main.tile[x, y].TileFrameX >= 18)
                    {
                        Main.tile[x, y].TileFrameX -= 18;
                    }
                    else
                    {
                        Main.tile[x, y].TileFrameX += 18;
                    }
                }
            }

            if (Wiring.running)
            {
                Wiring.SkipWire(left, top);
                Wiring.SkipWire(left, top + 1);
            }

            NetMessage.SendTileSquare(-1, left, top + 1, 2);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX < 18)
            {
                r = 0.55f;
                g = 0.55f;
                b = 0.55f;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
			Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Furniture/EyeLanternGlow").Value;
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

			spriteBatch.Draw(tex, new Vector2(i * 16, j * 16 - 2) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
        }
    }
}