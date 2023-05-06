using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.Debuff
{
	public class FleshWhipDefense2 : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}

		private bool initializeStats;
        private int storedDefense;

		public override void Update(NPC npc, ref int buffIndex)
        {
            if (!initializeStats)
            {
                storedDefense = npc.defense;
				npc.defense = (int)(npc.defense * 0.5f);

                initializeStats = true;
			}

			if (npc.buffTime[buffIndex] < 5)
            {
                npc.defense = storedDefense;
            }
		}
    }
}
