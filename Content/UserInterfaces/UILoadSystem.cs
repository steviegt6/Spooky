﻿using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using System.Collections.Generic;

namespace Spooky.Content.UserInterfaces
{
	public class UILoadSystem : ModSystem
	{
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            //Snotty schnoz meter
            int mouseIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Resource Bars");
            if (mouseIndex != -1)
            {
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Moco Nose UI", () =>
                {
                    MocoNoseBar.Draw(Main.spriteBatch, Main.LocalPlayer);
                    return true;
                }, 
				InterfaceScaleType.None));
            }

            //little eye bounty UI
            int mouseTextIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("Little Eye Bounty UI", () =>
                {
                    LittleEyeQuestUI.Draw(Main.spriteBatch);
                    return true;
                },
                InterfaceScaleType.None));
            }
        }
	}
}