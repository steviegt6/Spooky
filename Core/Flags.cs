using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System.IO;

namespace Spooky.Core
{
    public sealed partial class Flags : ModSystem
    {
        public static bool downedRotGourd = false;
        public static bool downedMoco = false;
        public static bool downedOrroboro = false;

        public static bool SpookyBackgroundAlt = false; 

        public override void SaveWorldData(TagCompound tag)
        {
            if (downedRotGourd) tag["downedRotGourd"] = true;
            if (downedMoco) tag["downedMoco"] = true;
            if (downedOrroboro) tag["downedOrroboro"] = true;

            if (SpookyBackgroundAlt) tag["SpookyBackgroundAlt"] = true;
        }

        public override void LoadWorldData(TagCompound tag) 
        {
			downedRotGourd = tag.ContainsKey("downedRotGourd");
            downedMoco = tag.ContainsKey("downedMoco");
            downedOrroboro = tag.ContainsKey("downedOrroboro");

            SpookyBackgroundAlt = tag.ContainsKey("SpookyBackgroundAlt");
		}

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = downedRotGourd;
            flags[1] = downedMoco;
            flags[2] = downedOrroboro;
            writer.Write(flags);

            var miscFlags = new BitsByte();
            miscFlags[0] = SpookyBackgroundAlt;
            writer.Write(miscFlags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedRotGourd = flags[0];
            downedMoco = flags[1];
            downedOrroboro = flags[2];

            BitsByte miscFlags = reader.ReadByte();
            SpookyBackgroundAlt = miscFlags[0];
        }
    }
}
