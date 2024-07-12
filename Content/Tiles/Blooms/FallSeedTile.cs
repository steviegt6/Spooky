using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.Items.SpookyBiome.Misc;
using Terraria.DataStructures;
using Spooky.Content.Tiles.SpookyBiome;
using System.Collections.Generic;

using Spooky.Content.Items.Blooms;

namespace Spooky.Content.Tiles.Blooms
{
	public class FallSeedTile : ModTile
	{
		public override string Texture => "Spooky/Content/Tiles/Blooms/BloomPlantTestTexture";

		private Asset<Texture2D> PlantTexture;

		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(147, 33, 27));
			RegisterItemDrop(ModContent.ItemType<FallSeed>());
			DustType = 288;
			HitSound = SoundID.Dig;
		}

		public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1 ? Vector2.Zero : Vector2.One * 12;

		public static Vector2 TileCustomPosition(int i, int j, Vector2? off = null)
		{
			return ((new Vector2(i, j) + TileOffset) * 16) - Main.screenPosition - (off ?? new Vector2(0, -2));
		}

		public static void DrawPlant(int i, int j, Texture2D tex, Rectangle? source, Vector2? offset = null, Vector2? origin = null)
		{
			Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(0, -2));

			Main.spriteBatch.Draw(tex, drawPos, source, Lighting.GetColor(i, j), 0, origin ?? source.Value.Size() / 3f, 1f, SpriteEffects.None, 0f);
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			//do not draw the tile texture itself
			return false;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			Vector2 Offset = new Vector2(40, 46);

			//draw the tile only on the bottom center of each tiles y-frame
			if (tile.TileFrameY == 36 || tile.TileFrameY == 90 || tile.TileFrameY == 144 || tile.TileFrameY == 198)
			{
				//also only draw the bloom tile on the middle x-frame
				if (tile.TileFrameX == 18 || tile.TileFrameX == 72 || tile.TileFrameX == 126 || tile.TileFrameX == 180 || tile.TileFrameX == 234)
				{
					PlantTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Blooms/FallBloomPlant");

					int frame = 0;

					//growth stage frames
					if (tile.TileFrameX == 18) frame = 0;
					if (tile.TileFrameX == 72) frame = 1;
					if (tile.TileFrameX == 126) frame = 2;
					if (tile.TileFrameX == 180) frame = 3;

					//fully grown frames
					if (tile.TileFrameX == 234)
					{
						if (tile.TileFrameY == 36) frame = 4;
						if (tile.TileFrameY == 90) frame = 5;
						if (tile.TileFrameY == 144) frame = 6;
						if (tile.TileFrameY == 198) frame = 7;
					}

					DrawPlant(i, j, PlantTexture.Value, new Rectangle(0, 50 * frame, 60, 48), TileOffset.ToWorldCoordinates(), Offset);
				}
			}
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			if (frameX == 216)
			{
				if (frameY == 0) Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i + 1, j) * 16, ModContent.ItemType<FallGourd>());
				if (frameY == 54) Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i + 1, j) * 16, ModContent.ItemType<FallSoulPumpkin>());
				if (frameY == 108) Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i + 1, j) * 16, ModContent.ItemType<FallWaterGourd>());
				if (frameY == 162) Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i + 1, j) * 16, ModContent.ItemType<FallZucchini>());
			}
		}

		public override void RandomUpdate(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			if (tile.TileFrameX < 216)
			{
				int left = i - tile.TileFrameX / 18 % 3;
				int top = j - tile.TileFrameY / 18 % 3;

				for (int x = left; x < left + 3; x++)
				{
					for (int y = top; y < top + 3; y++)
					{
						Tile CheckTile = Framing.GetTileSafely(x, y);
						CheckTile.TileFrameX += 54;
					}
				}

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendTileSquare(-1, left, top, 12);
				}
			}
		}
	}
}