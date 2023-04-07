using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class RotGourdMask : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 28;
			Item.vanity = true;
			Item.rare = ItemRarityID.Blue;
		}
	}
}