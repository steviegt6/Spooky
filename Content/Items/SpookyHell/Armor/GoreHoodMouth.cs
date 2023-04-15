using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.GameContent.Creative;

using Spooky.Core;
using Spooky.Content.Buffs;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.SpookyHell.Boss;
using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class GoreHoodMouth : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 7;
			Item.width = 28;
			Item.height = 26;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<GoreBody>() && legs.type == ModContent.ItemType<GoreLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			LocalizedText Description = Language.GetOrRegister("Mods.Spooky.ArmorSetBonus.GoreArmorMeleeRanged");
			player.setBonus = Description.Value;
			player.GetModPlayer<SpookyPlayer>().GoreArmorSet = true;

			if (!player.HasBuff(ModContent.BuffType<GoreAuraCooldown>()))
            {
				player.AddBuff(ModContent.BuffType<GoreAuraBuff>(), 1);

				player.GetDamage(DamageClass.Melee) += 0.8f;
				player.GetDamage(DamageClass.Ranged) += 0.8f;
            }
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawShadow = true;
			player.armorEffectDrawOutlines = true;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Melee) += 0.12f;
			player.GetDamage(DamageClass.Ranged) += 0.12f;
			player.GetCritChance(DamageClass.Melee) += 8;
			player.GetCritChance(DamageClass.Ranged) += 8;
			player.GetAttackSpeed(DamageClass.Melee) += 0.12f;
			player.ammoCost80 = true;
			player.aggro += 75;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<ArteryPiece>(), 12)
			.AddIngredient(ModContent.ItemType<CreepyChunk>(), 20)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
	}
}
