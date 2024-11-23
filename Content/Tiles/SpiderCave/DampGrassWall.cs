using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.SpiderCave
{
    public class DampGrassWall : ModWall 
    {
        private Asset<Texture2D> LeafTexture;

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(65, 56, 24));
            DustType = ModContent.DustType<DampGrassDust>();
            HitSound = SoundID.Grass;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            LeafTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpiderCave/DampGrassWallLeaf");

            Vector2 pos = TileGlobal.TileCustomPosition(i, j);

            if (Main.tile[i, j + 1].WallType > 0 && Main.tile[i - 1, j].WallType > 0 && Main.tile[i, j - 1].WallType > 0 && Main.tile[i + 1, j].WallType > 0)
            {
                if (i > Main.screenPosition.X / 16 && i < Main.screenPosition.X / 16 + Main.screenWidth / 16 && j > Main.screenPosition.Y / 16 && j < Main.screenPosition.Y / 16 + Main.screenHeight / 16)
                {
                    var rand = new Random(i + (j * 100000));

                    float offset = i * j % 6.28f + (float)rand.NextDouble() / 8f;
                    float sin = (float)Math.Sin(Main.GameUpdateCount / 45f + offset);

                    spriteBatch.Draw(LeafTexture.Value, pos + new Vector2(6, 6) + new Vector2(1, 0.5f) * sin * 2.2f,
                    new Rectangle(rand.Next(6) * 18, 0, 16, 16), Lighting.GetColor(i, j), offset + sin * 0.09f, new Vector2(12, 12), 1 + sin / 14f, 0, 0);
                }
            }
        }
    }
}