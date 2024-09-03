using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyHell
{
    public class PeptoStomach : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 60;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 30);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
        }
    }
}