using Terraria.ModLoader;

namespace Spooky.Content.Backgrounds.SpookyBiome
{
	public class SpookyForestBGAlt : ModSurfaceBackgroundStyle
	{
		public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
		{
			return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Backgrounds/SpookyBiome/SpookyForestBGAltClose");
		}

		public override int ChooseMiddleTexture()
		{
			return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Backgrounds/SpookyBiome/SpookyForestBGAltMiddle");
		}

		public override int ChooseFarTexture()
		{
			return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Backgrounds/SpookyBiome/SpookyForestBGAltFar");
		}

        public override void ModifyFarFades(float[] fades, float transitionSpeed)
		{
			for (int i = 0; i < fades.Length; i++)
			{
				if (i == Slot)
				{
					fades[i] += transitionSpeed;
					if (fades[i] > 1f)
					{
						fades[i] = 1f;
					}
				}
				else
				{
					fades[i] -= transitionSpeed;
					if (fades[i] < 0f)
					{
						fades[i] = 0f;
					}
				}
			}
		}
    }
}