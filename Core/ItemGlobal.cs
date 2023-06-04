using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using System.Linq;

using Spooky.Content.Buffs;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.Costume;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Core
{
    public class ItemGlobal : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            if (player.HasBuff(ModContent.BuffType<CatacombDebuff>()))
            {
                int[] Torches = { 8, 430, 432, 427, 429, 428, 1245, 431, 974, 3114, 3004, 2274, 433, 523, 1333, 3045, 4383, 4384, 4385, 4386, 4387, 4388, 5293, 5353 };

                //disable tools, block placement, and the rod of discord
                if (item.pick > 0 || item.hammer > 0 || item.axe > 0 || (item.createTile > 0 && !Torches.Contains(item.type)) || item.type == ItemID.RodofDiscord)
                {
                    return false;
                }

                //disable the use of any explosives
                int[] Explosives = { 166, 3196, 3115, 3547, 4908, 4827, 167, 4826, 4825, 4423, 235, 4909, 2896, 4824 };

                if (Explosives.Contains(item.type))
                {
                    return false;
                }

                //disable the use of buckets and sponges
                int[] LiquidItems = { 205, 206, 207, 1128, 3031, 4820, 5302, 5364, 3032, 4872, 5303, 5304 };

                if (LiquidItems.Contains(item.type))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.GetModPlayer<SpookyPlayer>().ShadowflameCandle && item.DamageType == DamageClass.Magic)
            {
                if (Main.rand.NextBool(10))
                {
                    SoundEngine.PlaySound(SoundID.Item103, player.Center);
                    Projectile.NewProjectile(source, position, velocity * 1.35f, ProjectileID.ShadowFlame, (int)knockback, player.whoAmI);
                }
            }

            if (Main.LocalPlayer.GetModPlayer<SpookyPlayer>().MocoNose && Main.LocalPlayer.HasBuff(ModContent.BuffType<BoogerFrenzyBuff>()) &&
            !Main.LocalPlayer.HasBuff(ModContent.BuffType<BoogerFrenzyCooldown>()))
            {
                int newProjectile = Projectile.NewProjectile(source, position, velocity * 1.35f, ModContent.ProjectileType<BlasterBoogerSmall>(), (int)knockback, player.whoAmI);
                Main.projectile[newProjectile].DamageType = item.DamageType;
            }

            return true;
        }

        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
		{
			if (item.type == ItemID.GoodieBag)
			{
				int[] DevMasks = new int[] { ModContent.ItemType<BananalizardHead>(), ModContent.ItemType<DylanDoeHead>(), ModContent.ItemType<KrakenHead>(), 
                ModContent.ItemType<TacoHead>(), ModContent.ItemType<WaasephiHead>(), ModContent.ItemType<HatHead>(), ModContent.ItemType<SeasaltHead>() };

                itemLoot.Add(ItemDropRule.OneFromOptions(30, DevMasks));
			}
		}
	}
}