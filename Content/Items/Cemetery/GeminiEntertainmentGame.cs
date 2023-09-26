using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Cemetery
{
    public class GeminiEntertainmentGame : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 42;
            Item.accessory = true;
            Item.rare = ItemRarityID.Gray;  
            //Item.value = Item.buyPrice(gold: 20);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<SpookyPlayer>().GeminiEntertainmentGame = true;
        }
    }
}