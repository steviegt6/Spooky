using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class AnalogHorrorTape : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 56;
            Item.accessory = true;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.buyPrice(platinum: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpookyPlayer>().AnalogHorrorTape = true;
            player.GetModPlayer<SpookyPlayer>().GeminiEntertainmentGame = true;
            player.GetModPlayer<SpookyPlayer>().MandelaCatalogueTV = true;
            player.GetModPlayer<SpookyPlayer>().CarnisFlavorEnhancer = true;
            player.GetModPlayer<SpookyPlayer>().Local58Telescope = true;

            //monument mythos pyramid defense
            if (!player.HasBuff(ModContent.BuffType<MonumentMythosCooldown>()))
            {
                player.GetModPlayer<SpookyPlayer>().MonumentMythosPyramid = true;
                player.statDefense += 40;
            }

            //spawn orbiting moon
            bool NotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<Local58Moon>()] <= 0;
			if (NotSpawned && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(null, player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<Local58Moon>(), 200, 5f, player.whoAmI);
			}
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<GeminiEntertainmentGame>())
            .AddIngredient(ModContent.ItemType<MandelaCatalogueTV>())
            .AddIngredient(ModContent.ItemType<CarnisFlavorEnhancer>())
            .AddIngredient(ModContent.ItemType<BackroomsCorpse>())
            .AddIngredient(ModContent.ItemType<Local58Telescope>())
            .AddIngredient(ModContent.ItemType<MonumentMythosPyramid>())
            .AddIngredient(ItemID.LunarBar, 5)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
        }
    }
}