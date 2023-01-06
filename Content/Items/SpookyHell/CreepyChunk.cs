using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell
{
    public class CreepyChunk : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Monster Meat");
            Tooltip.SetDefault("'Looks almost edible!'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 50;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 22;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 1);
            Item.maxStack = 999;
        }
    }
}