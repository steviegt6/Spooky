using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

using Spooky.Content.UserInterfaces;
using Spooky.Content.Projectiles.Blooms;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.Catacomb;
using Spooky.Content.Projectiles.Catacomb;
using Spooky.Content.Items.Catacomb.Blooms;

namespace Spooky.Core
{
    //separate modplayer for all of the bloom buffs since there is a ton of them and i dont feel like cluttering SpookyPlayer
    public class BloomBuffsPlayer : ModPlayer
    {
        //list of strings for each buff slot
        //each consumable bloom adds its name to a slot in this list of strings, and then each bonus is applied if that string is in the list
        //also used in the bloom UI so that it can draw each respective buff icon on it
        public string[] BloomBuffSlots = new string[4];

        //durations for each buff slot
        public int Duration1 = 0;
        public int Duration2 = 0;
        public int Duration3 = 0;
        public int Duration4 = 0;

        //bools for each edible bloom
        public bool FallGourd = false;
        public bool FallSoulPumpkin = false;
        public bool FallWaterGourd = false;
        public bool FallZucchini = false;
        public bool WinterBlackberry = false;
        public bool WinterBlueberry = false;
        public bool WinterGooseberry = false;
        public bool WinterStrawberry = false;
        public bool SpringHeartFlower = false;
        public bool SpringIris = false;
        public bool SpringOrchid = false;
        public bool SpringRose = false;
        public bool SummerLemon = false;
        public bool SummerOrange = false;
        public bool SummerPineapple = false;
        public bool SummerSunflower = false;
        public bool DandelionHerd = false;
        public bool DandelionMapleSeed = false;
        public bool DandelionTumbleweed = false;
        public bool Dragonfruit = false;

        //misc stuff
        public int DragonFruitSpawnTimer = 0;
        public int DragonfruitStacks = 0;
        public bool UnlockedSlot3 = false;
        public bool UnlockedSlot4 = false;

        //UI default position
        public Vector2 UITopLeft = new Vector2(Main.screenWidth / 2 - 116f, 75f);

        //global bool used for each individual bloom item so that they cannot be eaten if all of your slots are filled
        public bool CanConsumeFruit(string BuffName)
        {
            //if the player eats a bloom they already have active, allow it to be used so it can reset the buff duration
            if (BloomBuffSlots.Contains(BuffName))
            {
                return true;
            }
            else
            {
                //if every single buff slot is filled and both slot 3 and slot 4 are locked, dont allow the player to consume a bloom
                if (BloomBuffSlots[0] != string.Empty && BloomBuffSlots[1] != string.Empty && !UnlockedSlot3 && !UnlockedSlot4)
                {
                    return false;
                }
                //if every single buff slot is filled and the fourth slot is locked, dont allow the player to consume a bloom
                if (BloomBuffSlots[0] != string.Empty && BloomBuffSlots[1] != string.Empty && BloomBuffSlots[2] != string.Empty && !UnlockedSlot4)
                {
                    return false;
                }
                //if every single buff slot is filled, dont allow the player to consume a bloom
                if (BloomBuffSlots[0] != string.Empty && BloomBuffSlots[1] != string.Empty && BloomBuffSlots[2] != string.Empty && BloomBuffSlots[3] != string.Empty)
                {
                    return false;
                }
            }

            return true;
        }

        //when the player consumes a bloom, add that blooms name to a buff list slot and set its duration in that specific slot
        public void AddBuffToList(string BuffName, int Duration)
        {
            //if the player consumes a bloom they already have, then add that blooms base duration to the existing bloom buffs duration
            //also cap out the maximum duration for every single bloom buff at 72000 (which is 20 minutes in real time)
            if (BloomBuffSlots.Contains(BuffName))
            {
                if (BloomBuffSlots[0] == BuffName)
                {
                    Duration1 += Duration;

                    if (Duration1 > 72000)
                    {
                        Duration1 = 72000;
                    }
                }
                else if (BloomBuffSlots[1] == BuffName)
                {
                    Duration2 += Duration;

                    if (Duration2 > 72000)
                    {
                        Duration2 = 72000;
                    }
                }
                else if (BloomBuffSlots[2] == BuffName && UnlockedSlot3)
                {
                    Duration3 += Duration;

                    if (Duration3 > 72000)
                    {
                        Duration3 = 72000;
                    }
                }
                else if (BloomBuffSlots[3] == BuffName && UnlockedSlot4)
                {
                    Duration4 += Duration;

                    if (Duration4 > 72000)
                    {
                        Duration4 = 72000;
                    }
                }

                return;
            }

            //add the buff to the list by checking each slot to see if its open, and if it is add that buff to that slot
            //only attempt to check beyond the second slot when the player has each unlockable slot unlocked
            if (BloomBuffSlots[0] == string.Empty)
            {
                BloomBuffSlots[0] = BuffName;
                Duration1 = Duration;
            }
            else if (BloomBuffSlots[1] == string.Empty)
            {
                BloomBuffSlots[1] = BuffName;
                Duration2 = Duration;
            }
            else if (BloomBuffSlots[2] == string.Empty && UnlockedSlot3)
            {
                BloomBuffSlots[2] = BuffName;
                Duration3 = Duration;
            }
            else if (BloomBuffSlots[3] == string.Empty && UnlockedSlot4)
            {
                BloomBuffSlots[3] = BuffName;
                Duration4 = Duration;
            }
        }

        //manually set the bools for each bonus if the list of buffs contains that buff name
        public void GivePlayerBloomBonus()
        {
            FallGourd = BloomBuffSlots.Contains("FallGourd");
            FallSoulPumpkin = BloomBuffSlots.Contains("FallSoulPumpkin");
            FallWaterGourd = BloomBuffSlots.Contains("FallWaterGourd");
            FallZucchini = BloomBuffSlots.Contains("FallZucchini");
            WinterBlackberry = BloomBuffSlots.Contains("WinterBlackberry");
            WinterBlueberry = BloomBuffSlots.Contains("WinterBlueberry");
            WinterGooseberry = BloomBuffSlots.Contains("WinterGooseberry");
            WinterStrawberry = BloomBuffSlots.Contains("WinterStrawberry");
            SpringHeartFlower = BloomBuffSlots.Contains("SpringHeartFlower");
            SpringIris = BloomBuffSlots.Contains("SpringIris");
            SpringOrchid = BloomBuffSlots.Contains("SpringOrchid");
            SpringRose = BloomBuffSlots.Contains("SpringRose");
            SummerLemon = BloomBuffSlots.Contains("SummerLemon");
            SummerOrange = BloomBuffSlots.Contains("SummerOrange");
			SummerPineapple = BloomBuffSlots.Contains("SummerPineapple");
			SummerSunflower = BloomBuffSlots.Contains("SummerSunflower");
            DandelionHerd = BloomBuffSlots.Contains("DandelionHerd");
            DandelionMapleSeed = BloomBuffSlots.Contains("DandelionMapleSeed");
            DandelionTumbleweed = BloomBuffSlots.Contains("DandelionTumbleweed");
            Dragonfruit = BloomBuffSlots.Contains("Dragonfruit");
        }

        //handler for the buffs duration decreasing over time and setting each buff slot back to blank if the duration of that buff slot runs out
        public void HandleBloomBuffDuration()
        {
            if (Duration1 > 0)
            {
                Duration1--;
            }
            else
            {
                BloomBuffSlots[0] = string.Empty;
            }

            if (Duration2 > 0)
            {
                Duration2--;
            }
            else
            {
                BloomBuffSlots[1] = string.Empty;
            }

            //automatically set the string to empty if the player doesnt have the additional 3rd slot unlocked
            if (Duration3 > 0 && UnlockedSlot3)
            {
                Duration3--;
            }
            else
            {
                BloomBuffSlots[2] = string.Empty;
            }

            //automatically set the string to empty if the player doesnt have the additional 4th slot unlocked
            if (Duration4 > 0 && UnlockedSlot4)
            {
                Duration4--;
            }
            else
            {
                BloomBuffSlots[3] = string.Empty;
            }
        }

        //save and load the unlocked slots so they are permanent
        public override void SaveData(TagCompound tag)
        {
            tag["UITopLeft"] = UITopLeft;

            if (UnlockedSlot3) tag["UnlockedSlot3"] = true;
            if (UnlockedSlot4) tag["UnlockedSlot4"] = true;
        }
        public override void LoadData(TagCompound tag)
        {
            UITopLeft = tag.Get<Vector2>("UITopLeft");

            UnlockedSlot3 = tag.ContainsKey("UnlockedSlot3");
            UnlockedSlot4 = tag.ContainsKey("UnlockedSlot4");
        }

        public override void PreUpdate()
        {
			GivePlayerBloomBonus();
            HandleBloomBuffDuration();

			//open the bloom buff UI if you have any bloom buff at all, if not then close it
			//instead of just appearing, make the UI fade in for a cool effect if the player eats a bloom
			if (BloomBuffSlots[0] == string.Empty && BloomBuffSlots[1] == string.Empty && BloomBuffSlots[2] == string.Empty && BloomBuffSlots[3] == string.Empty)
			{
				if (BloomBuffUI.Transparency > 0f)
				{
					BloomBuffUI.Transparency -= 0.05f;
				}
			}
			else
			{
				//draw the bloom UI fully when the players inventory is not open
				if (!Main.playerInventory)
				{
					if (BloomBuffUI.Transparency < 1f)
					{
						BloomBuffUI.Transparency += 0.05f;
					}
				}
				//fade out a little if the players inventory is open
				else
				{
					if (BloomBuffUI.Transparency > 0.5f)
					{
						BloomBuffUI.Transparency -= 0.05f;
					}
					if (BloomBuffUI.Transparency < 0.5f)
					{
						BloomBuffUI.Transparency += 0.05f;
					}
				}
			}

			//spawn orbiting dragon fruits around the player and spawn more with each stack the player has
			if (Dragonfruit && Player.ownedProjectileCounts[ModContent.ProjectileType<DragonfruitOrbiter>()] < DragonfruitStacks)
			{
				DragonFruitSpawnTimer++;

				if (DragonFruitSpawnTimer >= 120)
				{
                    SoundEngine.PlaySound(SoundID.NPCDeath42 with { Pitch = 0.75f, Volume = 0.1f }, Player.Center);

                    int numOrbiters = Player.ownedProjectileCounts[ModContent.ProjectileType<DragonfruitOrbiter>()];

					int DistanceFromPlayer = 20 * (numOrbiters + 1);

					Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, 0, 0, ModContent.ProjectileType<DragonfruitOrbiter>(), 50, 0f, Main.myPlayer, Main.rand.Next(0, 2), Main.rand.Next(0, 360), DistanceFromPlayer);

					DragonFruitSpawnTimer = 0;
				}
			}
			else
			{
				DragonFruitSpawnTimer = 0;
			}
			//automatically remove all dragonfruit stacks and reset the timer if the player doesnt have the buff active
			if (!Dragonfruit)
			{
				DragonfruitStacks = 0;
				DragonFruitSpawnTimer = 0;
			}
        }

        public override void PostUpdate()
        {
            //fall gourd increases damage by 12% if you are falling
			if (FallGourd && Player.velocity.Y > 0f)
			{
				Player.GetDamage(DamageClass.Generic) += 0.12f;
			}
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            //spring rose gives the player thorns and inflicts bleeding sometimes
            if (SpringRose)
            {
				Player.ApplyDamageToNPC(npc, 50, 2, 0, false);

				if (Main.rand.NextBool(3))
				{
					//TODO: make a custom bleeding debuff for the rose to inflict
				}
            }
        }
	}
}