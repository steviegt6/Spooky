using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Cemetery.Misc
{
    public class CemeteryKey : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Yellow;
        }
    }
}