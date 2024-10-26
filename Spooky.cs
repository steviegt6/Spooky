using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

using Spooky.Core;
using Spooky.Content.Backgrounds;
using Spooky.Content.Backgrounds.Cemetery;
using Spooky.Content.Backgrounds.SpookyHell;
using Spooky.Content.NPCs.Boss.Daffodil;
using Spooky.Content.NPCs.Boss.Moco;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.NPCs.Boss.SpookySpirit;
using Spooky.Content.NPCs.Cemetery;
using Spooky.Content.NPCs.EggEvent;
using Spooky.Content.NPCs.PandoraBox;
using Spooky.Content.NPCs.NoseCult;

namespace Spooky
{
	public class Spooky : Mod
	{
        internal static Spooky Instance;
        
        internal Mod subworldLibrary = null;
        internal Mod remnants = null;

        public static int MistGhostSpawnX;
        public static int MistGhostSpawnY;

        public static int SpookySpiritSpawnX;
        public static int SpookySpiritSpawnY;

        public static int MocoSpawnX;
        public static int MocoSpawnY;

        public static int DaffodilSpawnX;
        public static int DaffodilSpawnY;
        public static int DaffodilParent;

        public static int PandoraBoxX;
        public static int PandoraBoxY;

        public static int OrroboroSpawnX;
        public static int OrroboroSpawnY;

        public static int GiantWebX;
        public static int GiantWebY;

        public static Effect vignetteEffect;
        public static Vignette vignetteShader;

        public static ModKeybind AccessoryHotkey { get; private set; }
        public static ModKeybind ArmorBonusHotkey { get; private set; }

        internal static Spooky mod;

        public Spooky()
		{
			mod = this;
		}

        public override void Load()
        {
            Instance = this;
            
            ModLoader.TryGetMod("SubworldLibrary", out subworldLibrary);
            ModLoader.TryGetMod("Remnants", out remnants);

            AccessoryHotkey = KeybindLoader.RegisterKeybind(this, "AccessoryHotkey", "E");
            ArmorBonusHotkey = KeybindLoader.RegisterKeybind(this, "ArmorBonusHotkey", "F");

            if (!Main.dedServ)
            {
                Filters.Scene["Spooky:CemeterySky"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(0f, 135f, 35f).UseOpacity(0.001f), EffectPriority.VeryHigh);
                SkyManager.Instance["Spooky:CemeterySky"] = new CemeterySky();

                Filters.Scene["Spooky:RaveyardSky"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);
                SkyManager.Instance["Spooky:RaveyardSky"] = new RaveyardSky();

                Filters.Scene["Spooky:SpookyForestTint"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(255f, 116f, 23f).UseOpacity(0.001f), EffectPriority.VeryHigh);

                Filters.Scene["Spooky:HallucinationEffect"] = new Filter(new SpookyScreenShader("FilterMoonLordShake").UseIntensity(0.5f), EffectPriority.VeryHigh);
            }

            if (Main.netMode != NetmodeID.Server)
			{
				vignetteEffect = ModContent.Request<Effect>("Spooky/Effects/Vignette", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
				vignetteShader = new Vignette(vignetteEffect, "MainPS");
				Filters.Scene["Spooky:Vignette"] = new Filter(vignetteShader, (EffectPriority)100);
            }

            SpiderCaveBG.Load();
            SpookyHellBG.Load();
        }

        public override void Unload()
        {
            subworldLibrary = null;
            remnants = null;

            AccessoryHotkey = null;
            ArmorBonusHotkey = null;

			mod = null;
		}

        public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			SpookyMessageType messageType = (SpookyMessageType)reader.ReadByte();
			switch (messageType)
			{
                case SpookyMessageType.SpawnMistGhost:
                {
                    int[] Types = new int[] { ModContent.NPCType<MistGhost>(), ModContent.NPCType<MistGhostFaces>(), ModContent.NPCType<MistGhostWiggle>() };
                    NPC.NewNPC(null, MistGhostSpawnX, MistGhostSpawnY, Main.rand.Next(Types));
                    break;
                }
                case SpookyMessageType.SpawnSpookySpirit:
                {
                    NPC.NewNPC(null, SpookySpiritSpawnX, SpookySpiritSpawnY, ModContent.NPCType<SpookySpirit>());
                    break;
                }
                case SpookyMessageType.SpawnMoco:
                {
                    NPC.NewNPC(null, MocoSpawnX, MocoSpawnY, ModContent.NPCType<MocoSpawner>());
					break;
                }
                case SpookyMessageType.SpawnOrroboro:
                {
                    NPC.NewNPC(null, OrroboroSpawnX, OrroboroSpawnY, ModContent.NPCType<OrroHeadP1>(), ai0: -1);
                    break;
                }
                case SpookyMessageType.SpawnDaffodilEye:
                {
                    NPC.NewNPC(null, DaffodilSpawnX, DaffodilSpawnY, ModContent.NPCType<DaffodilEye>(), ai0: (Flags.downedDaffodil && Main.rand.NextBool(20)) ? -4 : -1, ai1: DaffodilParent);
                    break;
                }
                case SpookyMessageType.SpawnBobbert:
                {
                    int NewNPC = NPC.NewNPC(null, PandoraBoxX, PandoraBoxY, ModContent.NPCType<Bobbert>());
                    Main.npc[NewNPC].velocity.X = Main.rand.Next(-10, 11);
                    Main.npc[NewNPC].velocity.Y = Main.rand.Next(-10, -5);
                    break;
                }
                case SpookyMessageType.SpawnStitch:
                {
                    int NewNPC = NPC.NewNPC(null, PandoraBoxX, PandoraBoxY, ModContent.NPCType<Stitch>());
                    Main.npc[NewNPC].velocity.X = Main.rand.Next(-10, 11);
                    Main.npc[NewNPC].velocity.Y = Main.rand.Next(-10, -5);
                    break;
                }
                case SpookyMessageType.SpawnSheldon:
                {
                    int NewNPC = NPC.NewNPC(null, PandoraBoxX, PandoraBoxY, ModContent.NPCType<Sheldon>());
                    Main.npc[NewNPC].velocity.X = Main.rand.Next(-10, 11);
                    Main.npc[NewNPC].velocity.Y = Main.rand.Next(-10, -5);
                    break;
                }
                case SpookyMessageType.SpawnChester:
                {
                    int NewNPC = NPC.NewNPC(null, PandoraBoxX, PandoraBoxY, ModContent.NPCType<Chester>());
                    Main.npc[NewNPC].velocity.Y = -8;
                    break;
                }
                case SpookyMessageType.PandoraBoxDowned:
                {
                    Flags.downedPandoraBox = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.OldHunterHat:
                {
                    Flags.OldHunterHat = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.OldHunterSkull:
                {
                    Flags.OldHunterSkull = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.OldHunterTorso:
                {
                    Flags.OldHunterTorso = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.OldHunterLegs:
                {
                    Flags.OldHunterLegs = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.OldHunterAssembled:
                {
                    Flags.OldHunterAssembled = true;
                    Flags.KillWeb = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.EggIncursionDowned:
                {
                    Flags.downedEggEvent = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.MocoIdolDowned1:
                {
                    Flags.downedMocoIdol1 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.MocoIdolDowned2:
                {
                    Flags.downedMocoIdol2 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.MocoIdolDowned3:
                {
                    Flags.downedMocoIdol3 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.MocoIdolDowned4:
                {
                    Flags.downedMocoIdol4 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.MocoIdolDowned5:
                {
                    Flags.downedMocoIdol5 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.MocoIdolDowned6:
                {
                    Flags.downedMocoIdol6 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.CatacombKey1:
                {
                    Flags.CatacombKey1 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.CatacombKey2:
                {
                    Flags.CatacombKey2 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.CatacombKey3:
                {
                    Flags.CatacombKey3 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.BountyAccepted:
                {
                    Flags.BountyInProgress = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.Bounty1Complete:
                {
                    Flags.LittleEyeBounty1 = true;
                    Flags.BountyInProgress = false;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.Bounty2Complete:
                {
                    Flags.LittleEyeBounty2 = true;
                    Flags.BountyInProgress = false;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.Bounty3Complete:
                {
                    Flags.LittleEyeBounty3 = true;
                    Flags.BountyInProgress = false;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.Bounty4Complete:
                {
                    Flags.LittleEyeBounty4 = true;
                    Flags.BountyInProgress = false;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
				//should never occur I think?
				default:
                {
					Logger.Warn("Spooky Mod: Unknown Message type: " + messageType);
					break;
                }
			}
		}
    }

    enum SpookyMessageType : byte
    {
        SpawnMistGhost,
        SpawnSpookySpirit,
        SpawnMoco,
        SpawnOrroboro,
        SpawnDaffodilEye,
        SpawnBobbert,
        SpawnStitch,
        SpawnSheldon,
        SpawnChester,
        PandoraBoxDowned,
        OldHunterHat,
        OldHunterSkull,
        OldHunterTorso,
        OldHunterLegs,
        OldHunterAssembled,
        EggIncursionDowned,
        MocoIdolDowned1,
        MocoIdolDowned2,
        MocoIdolDowned3,
        MocoIdolDowned4,
        MocoIdolDowned5,
        MocoIdolDowned6,
        CatacombKey1,
        CatacombKey2,
        CatacombKey3,
        BountyAccepted,
        Bounty1Complete,
        Bounty2Complete,
        Bounty3Complete,
        Bounty4Complete,
	}
}