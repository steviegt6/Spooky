using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyBiome
{
	public class SewingThread : ModItem
	{
		public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 62;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;  
            Item.value = Item.buyPrice(gold: 10);
        }
	}
}
