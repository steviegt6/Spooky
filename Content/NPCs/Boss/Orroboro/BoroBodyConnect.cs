using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.NPCs.Boss.Orroboro
{
    public class BoroBodyConnect : ModNPC
    {
        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/EggEvent/EnemyHit", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Ichor] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.ShadowFlame] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 15000;
            NPC.damage = 55;
            NPC.defense = 30;
            NPC.width = 65;
            NPC.height = 65;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.noGravity = true;
            NPC.HitSound = HitSound;
            NPC.aiStyle = -1;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 4)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
            {
                NPC.frame.Y = 0;
            }
        }

        public override bool PreAI()
        {   
            //go invulnerable and shake during phase 2 transition
            if (NPC.AnyNPCs(ModContent.NPCType<OrroHeadP1>()))
            {
                if (Main.npc[(int)NPC.ai[1]].ai[2] > 0)
                {
                    NPC.immortal = true;
                    NPC.dontTakeDamage = true;
                    NPC.netUpdate = true;
                    NPC.velocity *= 0f;

                    NPC.ai[2]++;

                    NPC.Center = new Vector2(NPC.Center.X, NPC.Center.Y);
                    NPC.Center += Main.rand.NextVector2Square(-2, 2);
                }
            }

            //kill segment if the head doesnt exist
			if (!Main.npc[(int)NPC.ai[1]].active)
            {
                NPC.active = false;
            }
			
			if (NPC.ai[1] < (double)Main.npc.Length)
            {
                Vector2 npcCenter = new(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                float dirX = Main.npc[(int)NPC.ai[1]].position.X + (float)(Main.npc[(int)NPC.ai[1]].width / 2) - npcCenter.X;
                float dirY = Main.npc[(int)NPC.ai[1]].position.Y + (float)(Main.npc[(int)NPC.ai[1]].height / 2) - npcCenter.Y;
                NPC.rotation = (float)Math.Atan2(dirY, dirX) + 1.57f;
                float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
                float dist = (length - (float)NPC.width) / length;
                float posX = dirX * dist;
                float posY = dirY * dist;
 
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + posX;
                NPC.position.Y = NPC.position.Y + posY;
            }

			return false;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }
    }
}
