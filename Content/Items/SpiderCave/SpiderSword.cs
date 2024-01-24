using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave
{
    public class SpiderSword : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 18;
			Item.DamageType = DamageClass.MeleeNoSpeed;
			Item.autoReuse = false;
            Item.noMelee = true;
			Item.noUseGraphic = true;
            Item.width = 62;
            Item.height = 62;
            Item.useTime = 18;
			Item.useAnimation = 18;
			Item.useStyle = ItemUseStyleID.Rapier;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 2);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<SpiderSwordProj>();
			Item.shootSpeed = 2.5f;
        }
    }
}