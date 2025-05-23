using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Minibiomes.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class SharkBoneLegs : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 4;
			Item.width = 22;
			Item.height = 18;
			Item.rare = ItemRarityID.Blue;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetAttackSpeed(DamageClass.Melee) += 0.08f;
			player.fishingSkill += 4;
		}

		/*
		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 25)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
		*/
	}
}