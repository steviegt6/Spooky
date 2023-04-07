using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Buffs.Pets;
using Spooky.Content.Projectiles.Pets;

namespace Spooky.Content.Items.Pets
{
	public class SpiritLamp : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 30;
			Item.height = 30;
			Item.master = true;
			Item.rare = ItemRarityID.Master;
			Item.value = Item.buyPrice(gold: 5);
			Item.shoot = ModContent.ProjectileType<SpookySpiritPet>();
			Item.buffType = ModContent.BuffType<SpookySpiritPetBuff>();
		}

        public override bool? UseItem(Player player)
        {
			player.AddBuff(Item.buffType, 2);
			return true;
        }
	}
}