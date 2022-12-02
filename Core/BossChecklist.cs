using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Items.BossBags;
using Spooky.Content.Items.BossBags.Accessory;
using Spooky.Content.Items.BossBags.Pets;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.Items.Catacomb.Boss;
using Spooky.Content.Items.SpookyBiome;
using Spooky.Content.Items.SpookyHell.Boss;
using Spooky.Content.Tiles.MusicBox;
using Spooky.Content.Tiles.Relic;

namespace Spooky.Core
{
	public class BossChecklistCompatibility : ModSystem
	{
        public override void PostSetupContent() 
        {
			BossChecklistSetup();
        }

        private void BossChecklistSetup() 
        {
			//do not load any of this without boss checklist enabled
			if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklistMod)) 
            {
				return;
			}

            if (bossChecklistMod.Version < new Version(1, 3, 1)) 
            {
				return;
			}

			/*
			Boss Checklist weight for all vanilla bosses, mini bosses, and events

			Bosses:

			KingSlime = 1f;
			EyeOfCthulhu = 2f;
			EvilBosses = 3f;
			QueenBee = 4f;
			Skeletron = 5f;
			DeerClops = 6f;
			WallOfFlesh = 7f;
			QueenSlime = 8f;
			TheTwins = 9f;
			TheDestroyer = 10f;
			SkeletronPrime = 11f;
			Plantera = 12f;
			Golem = 13f;
			Betsy = 14f;
			EmpressOfLight = 15f;
			DukeFishron = 16f;
			LunaticCultist = 17f;
			Moonlord = 18f;

			Events and Mini-Bosses:

			TorchGod = 1.5f;
			BloodMoon = 2.5f;
			GoblinArmy = 3.33f;
			OldOnesArmy = 3.66f;
			DarkMage = OldOnesArmy + 0.01f;
			Ogre = SkeletronPrime + 0.01f;
			FrostLegion = 7.33f;
			PirateInvasion = 7.66f;
			PirateShip = PirateInvasion + 0.01f;
			SolarEclipse = 11.5f;
			PumpkinMoon = 12.33f;
			MourningWood = PumpkinMoon + 0.01f;
			Pumpking = PumpkinMoon + 0.02f;
			FrostMoon = 12.66f;
			Everscream = FrostMoon + 0.01f;
			SantaNK1 = FrostMoon + 0.02f;
			IceQueen = FrostMoon + 0.03f;
			MartianMadness = 13.5f;
			MartianSaucer = MartianMadness + 0.01f;
			LunarEvent = LunaticCultist + 0.01f;
			*/

			//Rot Gourd
			string PumpkinName = "Rot Gourd";
			int Pumpkin = ModContent.NPCType<Content.NPCs.Boss.RotGourd.RotGourd>();
			Func<bool> PumpkinDowned = () => Flags.downedRotGourd;
			int PumpkinSummonItem = ModContent.ItemType<RottenSeed>();
			string PumpkinSpawnInfo = $"Use a [i:{PumpkinSummonItem}] in the spooky forest. It is sometimes dropped by breaking the pumpkins that grow in the spooky forest";
			string PumpkinDespawnInfo = "Rot Gourd has stomped all players";

			List<int> PumpkinDrops = new List<int>()
			{
				ModContent.ItemType<SpookyChestKey>(),
				ModContent.ItemType<RottenChunk>(),
				ModContent.ItemType<PumpkinBossBox>()
			};

			var PumpkinPortrait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => 
            {
				Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/RotGourd/RotGourdBC").Value;
				Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
				spriteBatch.Draw(texture, centered, color);
			};

			//register rot gourd
			bossChecklistMod.Call("AddBoss", Mod, PumpkinName, Pumpkin, 2.5f, PumpkinDowned, true, PumpkinDrops, 
			PumpkinSummonItem, PumpkinSpawnInfo, PumpkinDespawnInfo, PumpkinPortrait);

			//Moco
			string MocoName = "Moco";
			int Moco = ModContent.NPCType<Content.NPCs.Boss.Moco.Moco>();
			Func<bool> MocoDowned = () => Flags.downedMoco;
			int MocoSummonItem = ModContent.ItemType<CottonSwab>();
			string MocoSpawnInfo = $"Use the [i:{MocoSummonItem}] at the nose shrine in the valley of eyes";
			string MocoDespawnInfo = "Moco has sneezed on all players";

			List<int> MocoDrops = new List<int>()
			{
				ModContent.ItemType<BoogerBlaster>(),
				ModContent.ItemType<BoogerFlail>(),
				ModContent.ItemType<MocoBox>()
			};

			var MocoPortrait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => 
            {
				Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Moco/MocoBC").Value;
				Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
				spriteBatch.Draw(texture, centered, color);
			};

			//register moco
			bossChecklistMod.Call("AddBoss", Mod, MocoName, Moco, 3.5f, MocoDowned, true, MocoDrops, 
			MocoSummonItem, MocoSpawnInfo, MocoDespawnInfo, MocoPortrait);

			//Orroboro
			string OrroboroName = "Orro & Boro";
			int Orroboro = ModContent.NPCType<Content.NPCs.Boss.Orroboro.OrroboroHead>();
			Func<bool> OrroboroDowned = () => Flags.downedOrroboro;
			int OrroboroSummonItem = ModContent.ItemType<Concoction>();
			string OrroboroSpawnInfo = $"Use the [i:{OrroboroSummonItem}] at the egg in the valley of eyes";
			string OrroboroDespawnInfo = "Orro-Boro has eaten all players";

			List<int> OrroboroDrops = new List<int>()
			{
				ModContent.ItemType<OrroboroEye>(),
				ModContent.ItemType<OrroboroRelicItem>(),
				ModContent.ItemType<BossBagOrroboro>(),
				ModContent.ItemType<OrroboroChunk>(),
				ModContent.ItemType<Scycler>(),
				ModContent.ItemType<EyeFlail>(),
				ModContent.ItemType<EyeRocketLauncher>(),
				ModContent.ItemType<MouthFlamethrower>(),
				ModContent.ItemType<LeechStaff>(),
				ModContent.ItemType<LeechWhip>(),
				ModContent.ItemType<SpookyHellBossBox>()
			};

			var OrroboroPortrait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => 
            {
				Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Orroboro/OrroboroBC").Value;
				Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
				spriteBatch.Draw(texture, centered, color);
			};

			//register orroboro
			bossChecklistMod.Call("AddBoss", Mod, OrroboroName, Orroboro, 9.5f, OrroboroDowned, true, OrroboroDrops, 
			OrroboroSummonItem, OrroboroSpawnInfo, OrroboroDespawnInfo, OrroboroPortrait);

			//Big Bone
			string BigBoneName = "Big Bone";
			int BigBone = ModContent.NPCType<Content.NPCs.Boss.BigBone.BigBone>();
			Func<bool> BigBoneDowned = () => Flags.downedBigBone;
			int BigBoneSummonItem = ModContent.ItemType<Fertalizer>();
			string BigBoneSpawnInfo = $"Use [i:{BigBoneSummonItem}] while standing at the giant flower pot in the catacombs arena";
			string BigBoneDespawnInfo = "Big Bone has overgrown all players";

			List<int> BigBoneDrops = new List<int>()
			{
				ModContent.ItemType<BigBoneHammer>(),
				ModContent.ItemType<BigBoneBow>(),
				ModContent.ItemType<BigBoneStaff>(),
				ModContent.ItemType<BigBoneScepter>(),
				ModContent.ItemType<BigBoneBox>()
			};

			var BigBonePortrait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => 
            {
				Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneBC").Value;
				Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
				spriteBatch.Draw(texture, centered, color);
			};

			//register big bone
			bossChecklistMod.Call("AddBoss", Mod, BigBoneName, BigBone, 14.5f, BigBoneDowned, true, BigBoneDrops, 
			BigBoneSummonItem, BigBoneSpawnInfo, BigBoneDespawnInfo, BigBonePortrait);
        }
    }       
}