using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.SpookyBiome;

namespace Spooky.Content.NPCs
{
    public class TheEntity : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Entity");
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.ShouldBeCountedAsBoss[NPC.type] = true;
            NPCID.Sets.TrailCacheLength[NPC.type] = 7;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(15f, 75f),
                PortraitPositionXOverride = 8f,
                PortraitPositionYOverride = 45f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 250;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 130;
            NPC.height = 140;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath22;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new FlavorTextBestiaryInfoElement("[Information Redacted]"),
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement()
            });
        }

        public override void FindFrame(int frameHeight)
        {
            Player player = Main.player[NPC.target];

            if (player.Distance(NPC.Center) <= NPC.localAI[0] && NPC.ai[0] >= 1)
            {
                NPC.frame.Y = frameHeight * 1;
            }
            else
            {
                NPC.frame.Y = frameHeight * 0;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/TheEntityGlow").Value;
            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), NPC.frame,
            Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (NPC.ai[0] == 2)
            {
                Vector2 drawOrigin = new(tex.Width * 0.5f, (NPC.height * 0.5f));

                for (int oldPos = 0; oldPos < NPC.oldPos.Length; oldPos++)
                {
                    Vector2 drawPos = NPC.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY + 4);
                    Color color = NPC.GetAlpha(Color.White) * (float)(((float)(NPC.oldPos.Length - oldPos) / (float)NPC.oldPos.Length) / 2);
                    spriteBatch.Draw(tex, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
                }
            }
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];

            player.AddBuff(ModContent.BuffType<EntityDebuff>(), 2);

            NPC.localAI[2]++;
            //make npcs displayed name a random jumble of characters constantly
            if (NPC.localAI[2] % 5 == 0)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-=_+";
                string nameString = new(Enumerable.Repeat(chars, 12).Select(s => s[Main.rand.Next(s.Length)]).ToArray());
                NPC.GivenName = nameString;
            }

            switch ((int)NPC.ai[0])
            {
                //teleport around the player, repeat 5 times
                case 0:
                {
                    NPC.localAI[0]++;

                    NPC.damage = 0;

                    //loop 5 times
                    if (NPC.localAI[1] < 5)
                    {
                        //teleport after a certain time or if the player goes too far
                        if (NPC.localAI[0] >= 450)
                        {
                            Teleport(player, 0);
                            NPC.localAI[0] = 0;
                            NPC.localAI[1]++;
                        }

                        if (NPC.Distance(player.Center) >= 2200f)
                        {
                            NPC.Center = player.Center;
                        }
                    }
                    else
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[0]++;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //countdown (you better start running)
                case 1:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 60)
                    {
                        CombatText.NewText(NPC.getRect(), Color.White, "5", true);
                    }
                    if (NPC.localAI[0] == 120)
                    {
                        CombatText.NewText(NPC.getRect(), Color.White, "4", true);
                    }
                    if (NPC.localAI[0] == 180)
                    {
                        CombatText.NewText(NPC.getRect(), Color.White, "3", true);
                    }
                    if (NPC.localAI[0] == 240)
                    {
                        CombatText.NewText(NPC.getRect(), Color.White, "2", true);
                    }
                    if (NPC.localAI[0] == 300)
                    {
                        CombatText.NewText(NPC.getRect(), Color.White, "1", true);
                    }
                    if (NPC.localAI[0] == 360)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;
                    }

                    break;
                }

                //KILL KILL KILL KILL KILL KILL
                case 2:
                {
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;

                    Vector2 ChargeDirection = player.Center - NPC.Center;
                    ChargeDirection.Normalize();

                    ChargeDirection.X *= 65;
                    ChargeDirection.Y *= 65;
                    NPC.velocity.X = ChargeDirection.X;
                    NPC.velocity.Y = ChargeDirection.Y;

                    if (NPC.Hitbox.Intersects(player.Hitbox))
                    {
                        player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " was [REDACTED]."), 9999999, 0, false);
                        player.ApplyDamageToNPC(NPC, NPC.lifeMax * 2, 0, 0, false);
                        NPC.immortal = false;
                        NPC.dontTakeDamage = false;
                        NPC.netUpdate = true;
                    }

                    break;
                }
            }
        }

        /*
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (NPC.ai[0] == 2)
            {
                NPC.ai[1] = 1;
                NPC.netUpdate = true;
            }
        }
        */

        private void Teleport(Player player, int attemptNum)
        {
            int playerTileX = (int)player.position.X / 16;
            int playerTileY = (int)player.position.Y / 16;
            int npcTileX = (int)NPC.position.X / 16;
            int npcTileY = (int)NPC.position.Y / 16;
            int maxTileDist = 20;
            bool foundNewLoc = false;
            int targetX = Main.rand.Next(playerTileX - maxTileDist, playerTileX + maxTileDist);

            for (int targetY = Main.rand.Next(playerTileY - maxTileDist, playerTileY + maxTileDist); targetY < playerTileY + maxTileDist; ++targetY)
            {
                if ((targetY < playerTileY - 4 || targetY > playerTileY + 4 ||
                (targetX < playerTileX - 4 || targetX > playerTileX + 4)) &&
                (targetY < npcTileY - 1 || targetY > npcTileY + 1 ||
                (targetX < npcTileX - 1 || targetX > npcTileX + 1)) &&
                Main.tile[targetX, targetY].HasUnactuatedTile)
                {
                    bool flag2 = true;
                    if (Main.tile[targetX, targetY - 1].LiquidType == LiquidID.Lava)
                    {
                        flag2 = false;
                    }

                    if (flag2 && Main.tileSolid[(int)Main.tile[targetX, targetY].TileType] && !Collision.SolidTiles(targetX - 1, targetX + 1, targetY - 4, targetY - 1))
                    {
                        NPC.ai[2] = (float)targetX;
                        NPC.ai[3] = (float)targetY;
                        foundNewLoc = true;
                        break;
                    }
                }
            }

            SoundEngine.PlaySound(SoundID.Item8, NPC.position);

            if (NPC.ai[2] != 0 && NPC.ai[3] != 0 && foundNewLoc)
            {
                NPC.position.X = (float)((double)NPC.ai[2] * 16.0 - (double)(NPC.width / 2) + 8.0);
                NPC.position.Y = NPC.ai[3] * 16f - (float)NPC.height;
                NPC.netUpdate = true;

                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Main.dust[Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemDiamond)];
                    dust.noGravity = true;
                    dust.scale = 1f;
                    dust.velocity *= 0.1f;
                }
            }
            else if (attemptNum < 10)
            {
                Teleport(player, attemptNum + 1);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadowClump>(), 1));
        }
    }
}