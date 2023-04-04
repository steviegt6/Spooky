using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Items.BossBags.Accessory
{
    [LegacyName("PumpkinCore")]
    public class FlyCharm : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Charm of the Flies");
            /* Tooltip.SetDefault("Summons a swarm of flies around you over time, up to ten total"
            + "\nEach active fly will give you one extra point of defense"
            + "\nWhen you get hit, the flies will die and respawn after thirty seconds"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.expert = true;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 10);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            if (!player.HasBuff(ModContent.BuffType<FlyCooldown>()))
            {
                player.GetModPlayer<SpookyPlayer>().FlyAmulet = true;
            }
        }
    }
}