using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Buffs;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Dusts;
using Spooky.Content.Items.Creepypasta;
using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Core
{
    public class SpookyPlayer : ModPlayer
    {
        //armors and accessories
        public bool SpookySet = false;
        public bool GoreArmorSet = false;
        public bool TreatBag = false;
        public bool MagicCandle = false;
        public bool SirenHead = false;
        public bool PumpkinCore = false;
        public bool MocoNose = false;
        public bool OrroboroEmbyro = false;

        //minions and pets
        public bool SkullWisp = false;
        public bool TumorMinion = false;
        public bool SpookyWispPet = false;
        public bool RotGourdPet = false;
        public bool MocoPet = false;

        //buffs

        public override void ResetEffects()
        {
            //armors and accessories
            SpookySet = false;
            GoreArmorSet = false;
            TreatBag = false;
            MagicCandle = false;
            SirenHead = false;
            PumpkinCore = false;
            MocoNose = false;
            OrroboroEmbyro = false;

            //minions and pets
            SkullWisp = false;
            TumorMinion = false;
            SpookyWispPet = false;
            RotGourdPet = false;
            MocoPet = false;
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            bool ShouldRevive = true;

            //embryo revive ability
            if (Player.statLife <= 0)
            {
                if (OrroboroEmbyro && !Player.HasBuff(ModContent.BuffType<EmbryoCooldown>()))
                {
                    Player.AddBuff(ModContent.BuffType<EmbryoRevival>(), 300);
                    Player.AddBuff(ModContent.BuffType<EmbryoCooldown>(), 36000);
                    Player.statLife = 1;
                    SoundEngine.PlaySound(SoundID.Item103, Player.position);
                    ShouldRevive = false;
                }
            }

            return ShouldRevive;
        }
        public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter)
        {
            if (MocoNose && Main.rand.Next(2) == 0)
            {
                Vector2 Speed = new Vector2(3f, 0f).RotatedByRandom(2 * Math.PI);

                for (int numProjectiles = 0; numProjectiles < 3; numProjectiles++)
                {
                    Vector2 speed = Speed.RotatedBy(2 * Math.PI / 2 * (numProjectiles + Main.rand.NextDouble() - 0.5));

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, speed,
                        ModContent.ProjectileType<HomingBooger>(), 30 + ((int)damage / 2), 0f, Main.myPlayer, 0, 0);
                    }
                }
            }
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            if (GoreArmorSet && Player.HasBuff(ModContent.BuffType<GoreAuraBuff>()))
            {
                damage = 0;
                Player.AddBuff(ModContent.BuffType<GoreAuraCooldown>(), 2700);
                SoundEngine.PlaySound(SoundID.AbigailSummon, Player.Center);
                Player.statLife = Player.statLife;

                for (int numDust = 0; numDust < 20; numDust++)
                {
                    int dustEffect = Dust.NewDust(Player.Center, Player.width / 2, Player.height / 2, 90, 0f, 0f, 100, default, 2f);
                    Main.dust[dustEffect].velocity *= 3f;
                    Main.dust[dustEffect].noGravity = true;

                    if (Main.rand.Next(2) == 0)
                    {
                        Main.dust[dustEffect].scale = 0.5f;
                        Main.dust[dustEffect].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
            }

            return true;
        }

        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (SpookySet)
            {
                PlayerDrawLayers.Head.Hide();
            }
        }

        public override void PreUpdate()
        {
            if (PumpkinCore)
            {
                Player.AddBuff(ModContent.BuffType<FlyBuff>(), 2);

                if (Player.ownedProjectileCounts[ModContent.ProjectileType<SwarmFly>()] <= 0)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 vector2_2 = Vector2.UnitY.RotatedByRandom(1.57079637050629f) * new Vector2(5f, 3f);
                        Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, vector2_2.X, vector2_2.Y,
                        ModContent.ProjectileType<SwarmFly>(), 15, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
            }

            if (SpookyWorld.GhostEvent && Player.ZoneOverworldHeight)
            {
                SpawnTileFog();
            }
        }

        private void SpawnTileFog()
        {
            for (int i = (int)Math.Floor(Player.position.X / 16) - 120; i < (int)Math.Floor(Player.position.X / 16) + 120; i++)
            {
                for (int j = (int)Math.Floor(Player.position.Y / 16) - 30; j < (int)Math.Floor(Player.position.Y / 16) + 30; j++)
                {
                    if (!Main.tile[i, j - 1].HasTile && !Main.tile[i, j - 2].HasTile && Main.tile[i, j].HasTile)
                    {
                        if (Main.rand.Next(120) == 0)
                        {
                            int Index = Dust.NewDust(new Vector2((i - 2) * 16, (j - 1) * 16), 5, 5, ModContent.DustType<FogDust>());

                            Main.dust[Index].velocity.Y += 0.09f;
                        }
                    }
                }
            }
        }
    }

    public class GoreAura : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.WebbedDebuffBack);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return !drawInfo.drawPlayer.HasBuff<GoreAuraCooldown>() && drawInfo.drawPlayer.GetModPlayer<SpookyPlayer>().GoreArmorSet;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Texture2D tex;

            if (!drawInfo.drawPlayer.armorEffectDrawOutlines && !drawInfo.drawPlayer.armorEffectDrawShadow)
            {
                tex = ModContent.Request<Texture2D>("Spooky/Content/Items/SpookyHell/Armor/GoreAuraEffect2").Value;
            }
            else
            {
                tex = ModContent.Request<Texture2D>("Spooky/Content/Items/SpookyHell/Armor/GoreAuraEffect").Value;
            }

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

            Color color = Color.Lerp(Color.Lerp(Color.Transparent, new Color(220, 20, 60, 100), fade), Color.Transparent, fade);

            Main.EntitySpriteDraw(tex, drawInfo.drawPlayer.MountedCenter - Main.screenPosition, null, color, 0f, tex.Size() / 2, 0.8f + fade / 2f, SpriteEffects.None, 0);
        }
    }
}