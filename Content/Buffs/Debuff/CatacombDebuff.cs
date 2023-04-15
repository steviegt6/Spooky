using Terraria;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Buffs.Debuff
{
	public class CatacombDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}
    }
}
