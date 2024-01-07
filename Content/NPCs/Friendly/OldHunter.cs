using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.Localization;
using System.Collections.Generic;

using Spooky.Content.Biomes;

namespace Spooky.Content.NPCs.Friendly
{
	[AutoloadHead]
	public class OldHunter : ModNPC
	{
		public const string ShopName = "Shop";

        public override void SetStaticDefaults() 
        {
			Main.npcFrameCount[Type] = 25;
			NPCID.Sets.ExtraFramesCount[Type] = 9;
			NPCID.Sets.AttackFrameCount[Type] = 4;
			NPCID.Sets.DangerDetectRange[Type] = 700;
			NPCID.Sets.AttackType[Type] = 0;
			NPCID.Sets.AttackTime[Type] = 90;
			NPCID.Sets.AttackAverageChance[Type] = 30;
			NPCID.Sets.HatOffsetY[Type] = 4;

			NPC.Happiness
			.SetBiomeAffection<SpiderCaveBiome>(AffectionLevel.Love)
			.SetBiomeAffection<CemeteryBiome>(AffectionLevel.Like)
			.SetBiomeAffection<OceanBiome>(AffectionLevel.Dislike)
			.SetBiomeAffection<HallowBiome>(AffectionLevel.Hate)
			.SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Love)
			.SetNPCAffection(NPCID.Clothier, AffectionLevel.Like)
			.SetNPCAffection(NPCID.Princess, AffectionLevel.Dislike)
			.SetNPCAffection(NPCID.Nurse, AffectionLevel.Hate);

			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() 
            {
				Velocity = 1f,
				Direction = 1
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults() 
		{
			NPC.lifeMax = 250;
            NPC.damage = 10;
			NPC.defense = 15;
            NPC.width = 18;
			NPC.height = 40;
            NPC.knockBackResist = 0.5f;
            NPC.townNPC = true;
			NPC.friendly = true;
			NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 7;
			AnimationType = NPCID.Merchant;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.OldHunter"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool CanTownNPCSpawn(int numTownNPCs) 
        {
            //for now he will always move in, will be changed when the mechanic to get him is made
			return true;
		}

        public override List<string> SetNPCNameList() 
        {
			return new List<string>() 
            {
				"Gerald",
				"Bone",
				"Hunt",
				"Spooky Skeleton"
			};
		}

		public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("LegacyInterface.28");
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shop) 
		{
			if (firstButton) 
			{
				shop = ShopName; // Name of the shop tab we want to open.
			}
		}

		public override string GetChat()
		{
			//default dialogue options
			List<string> Dialogue = new List<string>
			{
				Language.GetTextValue("Mods.Spooky.Dialogue.OldHunter.Default1"),
			};

			return Main.rand.Next(Dialogue);
		}

		//teleports this npc whenever the king statue is activated like vanilla town npcs
		public override bool CanGoToStatue(bool toKingStatue) 
		{
			return true;
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback) 
		{
			damage = 20;
			knockback = 4f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown) 
		{
			cooldown = 30;
			randExtraCooldown = 30;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay) 
		{
			projType = ProjectileID.BoneGloveProj;
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset) 
		{
			multiplier = 12f;
			randomOffset = 2f;
			// SparklingBall is not affected by gravity, so gravityCorrection is left alone.
		}
    }
}