using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientClingerStaff : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 55;
            Item.mana = 15;
			Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
			Item.autoReuse = true;
            Item.width = 52;
            Item.height = 52;
            Item.useTime = 50;
			Item.useAnimation = 50;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 0;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 50);
            Item.UseSound = SoundID.Item20;
            Item.shoot = ModContent.ProjectileType<CursedExplosion>();
        }

        public override bool CanUseItem(Player player)
        {
            Vector2 mouse = Main.MouseWorld;
            List<Vector2> tiles = new List<Vector2>();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    Tile tile = Framing.GetTileSafely(i + (int)(mouse.X / 16), j + (int)(mouse.Y / 16));
                    Tile tileAbove = Framing.GetTileSafely(i + (int)(mouse.X / 16), j + (int)(mouse.Y / 16) - 1);
                    if (tile.HasTile && Main.tileSolid[tile.TileType] && (!tileAbove.HasTile || !Main.tileSolid[tileAbove.TileType]))
                    {
                        tiles.Add(new Vector2(i + (int)(mouse.X / 16), j + (int)(mouse.Y / 16)));
                    }
                }
            }
            
            if (tiles.Count > 0)
            {
                return true;
            }

            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 mouse = Main.MouseWorld;
            List<Vector2> tiles = new List<Vector2>();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    Tile tile = Framing.GetTileSafely(i + (int)(mouse.X / 16), j + (int)(mouse.Y / 16));
                    Tile tileAbove = Framing.GetTileSafely(i + (int)(mouse.X / 16), j + (int)(mouse.Y / 16) - 1);
                    if (tile.HasTile && Main.tileSolid[tile.TileType] && (!tileAbove.HasTile || !Main.tileSolid[tileAbove.TileType]))
                    {
                        tiles.Add(new Vector2(i + (int)(mouse.X / 16), j + (int)(mouse.Y / 16)));
                    }
                }
            }

            double currentdist = 9999;
            if (tiles.Count > 0)
            {
                for (int k = 0; k < tiles.Count; k++)
                {
                    Vector2 distance = (tiles[k] * 16) - mouse;
                    double truedist = Math.Sqrt((distance.X * distance.X) + (distance.Y * distance.Y));
                    if (truedist < currentdist && (Main.rand.NextBool(5) || currentdist == 9999))
                    {
                        position = tiles[k] * 16;
                        currentdist = truedist;
                    }
                }

                position.Y -= 25;
            }
        }
    }
}