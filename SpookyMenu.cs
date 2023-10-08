﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky
{
    internal class SpookyMenu : ModMenu
    {
        private bool HasClicked;

        private float Intensity;

        private Vector2 logoCenter = Vector2.Zero;

        public override string DisplayName => "Spooky Mod";

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyMenu");

        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>("Spooky/SpookyMenuLogo");

        public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blank");

        public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blank");
        
        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => null;

        public static readonly SoundStyle LogoClickSound1 = new("Spooky/Content/Sounds/MenuLogoClick1", SoundType.Sound);
        public static readonly SoundStyle LogoClickSound2 = new("Spooky/Content/Sounds/MenuLogoClick2", SoundType.Sound);

        public override void OnDeselected()
        {
            //un-hide the sun when this menu is switched
            Main.sunModY = 0;
        }

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            drawColor = Color.White;

            //hides the sun offscreen so you cant click it
            Main.sunModY = -300;

            //set daytime to true 
            Main.time = 27000;
            Main.dayTime = true;

            logoDrawCenter -= new Vector2(0, 0);
            logoScale = 0.8f;

            Texture2D texture = ModContent.Request<Texture2D>("Spooky/SpookyMenu").Value;

            Vector2 drawOffset = Vector2.Zero;
            float xScale = (float)Main.screenWidth / texture.Width;
            float yScale = (float)Main.screenHeight / texture.Height;
            float scale = xScale;

            if (xScale != yScale)
            {
                if (yScale > xScale)
                {
                    scale = yScale;
                    drawOffset.X -= (texture.Width * scale - Main.screenWidth - 10) * 0.5f;
                }
                else
                {
                    drawOffset.Y -= (texture.Height * scale - Main.screenHeight) * 0.5f;
                }
            }

            spriteBatch.Draw(texture, drawOffset, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            //outlines for each boss
            Texture2D outlineTex = ModContent.Request<Texture2D>("Spooky/SpookyMenuOutlines").Value;
            spriteBatch.Draw(outlineTex, drawOffset, null, Color.White * Intensity, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            //draw the actual menu logo
            logoCenter = logoDrawCenter;
            Main.EntitySpriteDraw(Logo.Value, logoDrawCenter, new Rectangle(0, 0, Utils.Width(Logo), Utils.Height(Logo)), Color.White, 0, Utils.Size(Logo) / 2f, new Vector2(1f + Intensity, 1f - Intensity), SpriteEffects.None, 0);

            return false;
        }

        public override void Update(bool isOnTitleScreen)
        {
            Intensity *= 0.95f;
            if (Main.mouseLeft && !HasClicked && Intensity <= 0.1f && Math.Abs(Main.MouseScreen.X - logoCenter.X) < 300f && Math.Abs(Main.MouseScreen.Y - logoCenter.Y) < 70f)
            {
                Intensity = 1f;

                if (Math.Abs(Intensity) < 0.1f)
                {
                    Intensity = Math.Sign(Intensity) * 0.1f;
                }

                SoundEngine.PlaySound(LogoClickSound1, null);
                SoundEngine.PlaySound(LogoClickSound2, null);
            }

            HasClicked = Main.mouseLeft;
        }
    }
}
