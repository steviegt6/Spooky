using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Catacomb
{
    public class PandoraAmalgam : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 60;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;  
            Item.value = Item.buyPrice(gold: 50);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<PandoraChalice>())
            .AddIngredient(ModContent.ItemType<PandoraCross>())
            .AddIngredient(ModContent.ItemType<PandoraCuffs>())
            .AddIngredient(ModContent.ItemType<PandoraRosary>())
            .AddIngredient(ItemID.Ectoplasm, 25)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
        }
    }
}