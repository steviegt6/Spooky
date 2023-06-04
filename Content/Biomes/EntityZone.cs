using Terraria;
using Terraria.ModLoader;

using Spooky.Content.NPCs;

namespace Spooky.Content.Biomes
{
    public class EntityZone : ModBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/TheEntity");

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Spooky:EntityEffect", isActive, player.Center);
        }

        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = NPC.AnyNPCs(ModContent.NPCType<TheEntity>());

            return BiomeCondition;
        }
    }
}