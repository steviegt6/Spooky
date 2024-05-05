using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Dusts;
using Spooky.Content.NPCs.PandoraBox.Projectiles;

namespace Spooky.Content.NPCs.PandoraBox
{
    public class Bobbert : ModNPC
    {
        private static Asset<Texture2D> ChainTexture;
        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 200;
            NPC.damage = 50;
            NPC.defense = 0;
            NPC.width = 40;
            NPC.height = 38;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit54;
            NPC.DeathSound = SoundID.NPCDeath52;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type, ModContent.GetInstance<Biomes.PandoraBoxBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Bobbert"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.PandoraBoxBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			for (int k = 0; k < Main.maxNPCs; k++)
			{
				if (Main.npc[k].active && Main.npc[k].type == ModContent.NPCType<PandoraBox>() && NPC.Distance(Main.npc[k].Center) <= 750f) 
				{
                    ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/PandoraBox/ChainSmall");
                    
                    Vector2 ParentCenter = Main.npc[k].Center;

					Rectangle? chainSourceRectangle = null;
					float chainHeightAdjustment = 0f;

					Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture.Size() / 2f);
					Vector2 chainDrawPosition = NPC.Center;
					Vector2 vectorToParent = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
					Vector2 unitVectorToParent = vectorToParent.SafeNormalize(Vector2.Zero);
					float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture.Height()) + chainHeightAdjustment;

					if (chainSegmentLength == 0)
					{
						chainSegmentLength = 10;
					}

					float chainRotation = unitVectorToParent.ToRotation() + MathHelper.PiOver2;
					int chainCount = 0;
					float chainLengthRemainingToDraw = vectorToParent.Length() + chainSegmentLength / 2f;
		
					while (chainLengthRemainingToDraw > 0f)
					{
						Color chainDrawColor = Color.Cyan * 0.5f;

						Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

						chainDrawPosition += unitVectorToParent * chainSegmentLength;
						chainCount++;
						chainLengthRemainingToDraw -= chainSegmentLength;
					}
				}
			}

            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6f)) / 2f + 0.5f;

            var effects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawPosition = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4);
			Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.LightBlue);

            for (int repeats = 0; repeats < 4; repeats++)
            {
                Vector2 afterImagePosition = new Vector2(NPC.Center.X, NPC.Center.Y) + NPC.rotation.ToRotationVector2() - screenPos + new Vector2(0, NPC.gfxOffY + 4) - NPC.velocity * repeats;
                Main.spriteBatch.Draw(NPCTexture.Value, afterImagePosition, NPC.frame, color * fade, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.2f, effects, 0f);
            }

            Main.spriteBatch.Draw(NPCTexture.Value, drawPosition, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.2f, effects, 0f);

            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 7)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 3)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override bool CheckActive()
        {
            return !PandoraBoxWorld.PandoraEventActive;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            SoundEngine.PlaySound(SoundID.Item14, NPC.Center);

            Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BobbertExplosion>(), NPC.damage / 4, 0, Main.myPlayer);

            NPC.active = false;
        }

        public override void AI()
		{
            NPC.TargetClosest(true);
			Player player = Main.player[NPC.target];

            NPC.rotation = NPC.velocity.X * 0.04f;

            float goToX = player.Center.X - NPC.Center.X;
            float goToY = player.Center.Y - NPC.Center.Y;
            float speed = 0.08f;

            if (NPC.velocity.X < goToX)
            {
                NPC.velocity.X = NPC.velocity.X + speed;
                if (NPC.velocity.X < 0f && goToX > 0f)
                {
                    NPC.velocity.X = NPC.velocity.X + speed;
                }
            }
            else if (NPC.velocity.X > goToX)
            {
                NPC.velocity.X = NPC.velocity.X - speed;
                if (NPC.velocity.X > 0f && goToX < 0f)
                {
                    NPC.velocity.X = NPC.velocity.X - speed;
                }
            }
            if (NPC.velocity.Y < goToY)
            {
                NPC.velocity.Y = NPC.velocity.Y + speed;
                if (NPC.velocity.Y < 0f && goToY > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y + speed;
                    return;
                }
            }
            else if (NPC.velocity.Y > goToY)
            {
                NPC.velocity.Y = NPC.velocity.Y - speed;
                if (NPC.velocity.Y > 0f && goToY < 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y - speed;
                    return;
                }
            }

            for (int num = 0; num < Main.maxNPCs; num++)
			{
				NPC other = Main.npc[num];
				if (num != NPC.whoAmI && other.type == NPC.type && other.active && Math.Abs(NPC.position.X - other.position.X) + Math.Abs(NPC.position.Y - other.position.Y) < NPC.width)
				{
					const float pushAway = 0.2f;
					if (NPC.position.X < other.position.X)
					{
						NPC.velocity.X -= pushAway;
					}
					else
					{
						NPC.velocity.X += pushAway;
					}
					if (NPC.position.Y < other.position.Y)
					{
						NPC.velocity.Y -= pushAway;
					}
					else
					{
						NPC.velocity.Y += pushAway;
					}
				}
			}
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                SoundEngine.PlaySound(SoundID.Item14, NPC.Center);

                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BobbertExplosion>(), NPC.damage / 2, 0, Main.myPlayer);

                for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.2f);
                    Main.dust[dustGore].color = Color.Cyan;
                    Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-5f, 5f);
                    Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-3f, 3f);
                    Main.dust[dustGore].noGravity = true;
                }
            }
        }
    }
}