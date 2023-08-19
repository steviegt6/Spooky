using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.SpookyHell
{
	// Example Advanced Flail is a complete adaption of Ball O' Hurt projectile. The code has been rewritten a bit to make it easier to follow. Compare this code against the decompiled Terraria code for an example of adapting vanilla code. A few comments and extra code snippets show features from other vanilla flails as well.
	// Example Advanced Flail shows a plethora of advanced AI and collision topics.
	// See ExampleFlail for a simpler but less customizable flail projectile example.
	public class EyeFlailProj : ModProjectile
	{
		private const string ChainTexturePath = "Spooky/Content/Projectiles/SpookyHell/EyeFlailChain"; // The folder path to the flail chain sprite
		
		private enum AIState
		{
			Spinning,
			LaunchingForward,
			Retracting,
			UnusedState,
			ForcedRetracting,
			Ricochet,
			Dropping
		}

		// These properties wrap the usual ai and localAI arrays for cleaner and easier to understand code.
		private AIState CurrentAIState 
        {
			get => (AIState)Projectile.ai[0];
			set => Projectile.ai[0] = (float)value;
		}
		public ref float StateTimer => ref Projectile.ai[1];
		public ref float CollisionCounter => ref Projectile.localAI[0];
		public ref float SpinningStateTimer => ref Projectile.localAI[1];

		public override void SetStaticDefaults() 
        {
			// These lines facilitate the trail drawing
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults() 
        {
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true; 
			Projectile.netImportant = true; 
			Projectile.usesLocalNPCImmunity = true;
			Projectile.penetrate = -1;	
			Projectile.localNPCHitCooldown = 10;
		}

		// PreDraw is used to draw a chain and trail before the projectile is drawn normally.
		public override bool PreDraw(ref Color lightColor)
		{
			Vector2 playerArmPosition = Main.GetPlayerArmPosition(Projectile);

			// This fixes a vanilla GetPlayerArmPosition bug causing the chain to draw incorrectly when stepping up slopes. The flail itself still draws incorrectly due to another similar bug. This should be removed once the vanilla bug is fixed.
			playerArmPosition.Y -= Main.player[Projectile.owner].gfxOffY;

			Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>(ChainTexturePath);
			
			Rectangle? chainSourceRectangle = null;
			// Drippler Crippler customizes sourceRectangle to cycle through sprite frames: sourceRectangle = asset.Frame(1, 6);
			float chainHeightAdjustment = 0f; // Use this to adjust the chain overlap. 

			Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
			Vector2 chainDrawPosition = Projectile.Center;
			Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
			Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
			float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;

			if (chainSegmentLength == 0)
			{
				chainSegmentLength = 10; // When the chain texture is being loaded, the height is 0 which would cause infinite loops.
			}

			float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
			int chainCount = 0;
			float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

			// This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
			while (chainLengthRemainingToDraw > 0f)
			{
				// This code gets the lighting at the current tile coordinates
				Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

				// Flaming Mace and Drippler Crippler use code here to draw custom sprite frames with custom lighting.
				// Cycling through frames: sourceRectangle = asset.Frame(1, 6, 0, chainCount % 6);
				// This example shows how Flaming Mace works. It checks chainCount and changes chainTexture and draw color at different values

				var chainTextureToDraw = chainTexture;

				// Here, we draw the chain texture at the coordinates
				Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

				// chainDrawPosition is advanced along the vector back to the player by the chainSegmentLength
				chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
				chainCount++;
				chainLengthRemainingToDraw -= chainSegmentLength;
			}

			// Add a motion trail when moving forward, like most flails do (don't add trail if already hit a tile)
			if (CurrentAIState == AIState.LaunchingForward)
			{
				Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
				Vector2 drawOrigin = new(projectileTexture.Width * 0.5f, Projectile.height * 0.5f);
				SpriteEffects spriteEffects = SpriteEffects.None;

				if (Projectile.spriteDirection == -1)
				{
					spriteEffects = SpriteEffects.FlipHorizontally;
				}
				for (int oldPos = 0; oldPos < Projectile.oldPos.Length && oldPos < StateTimer; oldPos++)
				{
					Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
					Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
					Main.spriteBatch.Draw(projectileTexture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale - oldPos / (float)Projectile.oldPos.Length / 3, spriteEffects, 0f);
				}
			}

			return true;
		}

		// This AI code was adapted from vanilla code: Terraria.Projectile.AI_015_Flails() 
		public override void AI() 
        {
			Player player = Main.player[Projectile.owner];
			// Kill the projectile if the player dies or gets crowd controlled
			if (!player.active || player.dead || player.noItems || player.CCed || Vector2.Distance(Projectile.Center, player.Center) > 900f) 
            {
				Projectile.Kill();
				return;
			}
			if (Main.myPlayer == Projectile.owner && Main.mapFullscreen) 
            {
				Projectile.Kill();
				return;
			}

			Vector2 mountedCenter = player.MountedCenter;
			bool shouldOwnerHitCheck = false;
			int launchTimeLimit = 15;
			float launchSpeed = 25f; 
			float maxLaunchLength = 800f;
			float retractAcceleration = 3f; 
			float maxRetractSpeed = 20f;
			float forcedRetractAcceleration = 20f; 
			float maxForcedRetractSpeed = 15f; 
			float unusedRetractAcceleration = 1f;
			float unusedMaxRetractSpeed = 14f;
			int unusedChainLength = 60;
			int defaultHitCooldown = 10; 
			int spinHitCooldown = 20; 
			int movingHitCooldown = 20; 
			int ricochetTimeLimit = launchTimeLimit + 5;

			// Scaling these speeds and accelerations by the players meleeSpeed make the weapon more responsive if the player boosts their meleeSpeed
			float meleeSpeed = player.GetAttackSpeed(DamageClass.Melee);
			float meleeSpeedMultiplier = 1f / meleeSpeed;
			launchSpeed *= meleeSpeedMultiplier;
			unusedRetractAcceleration *= meleeSpeedMultiplier;
			unusedMaxRetractSpeed *= meleeSpeedMultiplier;
			retractAcceleration *= meleeSpeedMultiplier;
			maxRetractSpeed *= meleeSpeedMultiplier;
			forcedRetractAcceleration *= meleeSpeedMultiplier;
			maxForcedRetractSpeed *= meleeSpeedMultiplier;
			float launchRange = launchSpeed * launchTimeLimit;
			float maxDroppedRange = launchRange + 160f;
			Projectile.localNPCHitCooldown = defaultHitCooldown;

			switch (CurrentAIState) 
            {
				case AIState.Spinning: 
                {
                    shouldOwnerHitCheck = true;
                    if (Projectile.owner == Main.myPlayer) 
                    {
                        Vector2 unitVectorTowardsMouse = mountedCenter.DirectionTo(new Vector2(Main.MouseWorld.X + Main.rand.Next(-75, 75), Main.MouseWorld.Y + Main.rand.Next(-75, 75))).SafeNormalize(Vector2.UnitX * player.direction);
                        player.ChangeDir((unitVectorTowardsMouse.X > 0f) ? 1 : (-1));

                        if (!player.channel) // If the player releases then change to moving forward mode
                        {
                            CurrentAIState = AIState.LaunchingForward;
                            StateTimer = 0f;
                            Projectile.velocity = unitVectorTowardsMouse * launchSpeed + player.velocity;
                            Projectile.Center = mountedCenter;
                            Projectile.netUpdate = true;
                            Projectile.ResetImmunity();
                            Projectile.localNPCHitCooldown = movingHitCooldown;
                            break;
                        }
                    }

                    SpinningStateTimer += 1f;
                    // This line creates a unit vector that is constantly rotated around the player. 10f controls how fast the projectile visually spins around the player
                    Vector2 offsetFromPlayer = new Vector2(player.direction).RotatedBy((float)Math.PI * 10f * (SpinningStateTimer / 60f) * player.direction);

                    offsetFromPlayer.Y *= 1.2f;
                    if (offsetFromPlayer.Y * player.gravDir > 0f) 
                    {
                        offsetFromPlayer.Y *= 1f;
                    }

					offsetFromPlayer.X *= 1.2f;
                    if (offsetFromPlayer.X * player.gravDir > 0f) 
                    {
                        offsetFromPlayer.X *= 1f;
                    }

                    Projectile.Center = mountedCenter + offsetFromPlayer * 30f;
                    Projectile.velocity = Vector2.Zero;
                    Projectile.localNPCHitCooldown = spinHitCooldown; // set the hit speed to the spinning hit speed

                    break;
                }
				case AIState.LaunchingForward: 
                {
                    bool shouldSwitchToRetracting = StateTimer++ >= launchTimeLimit;
                    shouldSwitchToRetracting |= Projectile.Distance(mountedCenter) >= maxLaunchLength;
                    
                    if (player.controlUseItem) // If the player clicks, transition to the Dropping state
                    {
                        CurrentAIState = AIState.Dropping;
                        StateTimer = 0f;
                        Projectile.netUpdate = true;
                        Projectile.velocity *= 0.2f;
                        // This is where Drippler Crippler spawns its projectile
                        /*
                        if (Main.myPlayer == Projectile.owner)
                            Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Projectile.velocity, 928, Projectile.damage, Projectile.knockBack, Main.myPlayer);
                        */
                        break;
                    }

                    if (shouldSwitchToRetracting)
                    {
                        CurrentAIState = AIState.Retracting;
                        StateTimer = 0f;
                        Projectile.netUpdate = true;
                        Projectile.velocity *= 0.3f;
                        // This is also where Drippler Crippler spawns its projectile, see above code.
                    }
                    player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
                    Projectile.localNPCHitCooldown = movingHitCooldown;
                    break;
                }
				case AIState.Retracting: 
                {
                    Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);

                    if (Projectile.Distance(mountedCenter) <= maxRetractSpeed) 
                    {
                        Projectile.Kill(); // Kill the projectile once it is close enough to the player
                        return;
                    }
                    if (player.controlUseItem) // If the player clicks, transition to the Dropping state
                    {
                        CurrentAIState = AIState.Dropping;
                        StateTimer = 0f;
                        Projectile.netUpdate = true;
                        Projectile.velocity *= 0.2f;
                    }
                    else 
                    {
                        Projectile.velocity *= 0.98f;
                        Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxRetractSpeed, retractAcceleration);
                        player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
                    }
                    break;
                }
				case AIState.UnusedState: // Projectile.ai[0] == 3; This case is actually unused, but maybe a Terraria update will add it back in, or maybe it is useless, so I left it here.
                {
                    if (!player.controlUseItem) 
                    {
                        CurrentAIState = AIState.ForcedRetracting; // Move to super retracting mode if the player taps
                        StateTimer = 0f;
                        Projectile.netUpdate = true;
                        break;
                    }

                    float currentChainLength = Projectile.Distance(mountedCenter);
                    Projectile.tileCollide = StateTimer == 1f;
                    bool flag3 = currentChainLength <= launchRange;

                    if (flag3 != Projectile.tileCollide) 
                    {
                        Projectile.tileCollide = flag3;
                        StateTimer = Projectile.tileCollide ? 1 : 0;
                        Projectile.netUpdate = true;
                    }

                    if (currentChainLength > unusedChainLength) 
                    {

                        if (currentChainLength >= launchRange) 
                        {
                            Projectile.velocity *= 0.5f;
                            Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * unusedMaxRetractSpeed, unusedMaxRetractSpeed);
                        }

                        Projectile.velocity *= 0.98f;
                        Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * unusedMaxRetractSpeed, unusedRetractAcceleration);
                    }
                    else 
                    {
                        if (Projectile.velocity.Length() < 6f) 
                        {
                            Projectile.velocity.X *= 0.96f;
                            Projectile.velocity.Y += 0.2f;
                        }
                        if (player.velocity.X == 0f) {
                            Projectile.velocity.X *= 0.96f;
                        }
                    }

                    player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));

                    break;
                }
				case AIState.ForcedRetracting:
                {
                    Projectile.tileCollide = false;
                    Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);

                    if (Projectile.Distance(mountedCenter) <= maxForcedRetractSpeed) 
                    {
                        Projectile.Kill(); // Kill the projectile once it is close enough to the player
                        return;
                    }

                    Projectile.velocity *= 0.98f;
                    Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxForcedRetractSpeed, forcedRetractAcceleration);
                    Vector2 target = Projectile.Center + Projectile.velocity;
                    Vector2 value = mountedCenter.DirectionFrom(target).SafeNormalize(Vector2.Zero);

                    if (Vector2.Dot(unitVectorTowardsPlayer, value) < 0f) 
                    {
                        Projectile.Kill(); // Kill projectile if it will pass the player
                        return;
                    }

                    player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));

                    break;
                }
				case AIState.Ricochet:
                {
					if (StateTimer++ >= ricochetTimeLimit) 
                    {
						CurrentAIState = AIState.Dropping;
						StateTimer = 0f;
						Projectile.netUpdate = true;
					}
					else 
                    {
						Projectile.localNPCHitCooldown = movingHitCooldown;
						Projectile.velocity.Y += 0.6f;
						Projectile.velocity.X *= 0.95f;
						player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
					}
					break;
                }
				case AIState.Dropping:
                {
					if (!player.controlUseItem || Projectile.Distance(mountedCenter) > maxDroppedRange) 
                    {
						CurrentAIState = AIState.ForcedRetracting;
						StateTimer = 0f;
						Projectile.netUpdate = true;
					}
					else 
                    {
						Projectile.velocity.Y += 0.8f;
						Projectile.velocity.X *= 0.95f;
						player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
					}

					break;
                }
			}

			// This is where Flower Pow launches projectiles. Decompile Terraria to view that code.
			Projectile.direction = (Projectile.velocity.X > 0f) ? 1 : -1;
			Projectile.spriteDirection = Projectile.direction;
			Projectile.ownerHitCheck = shouldOwnerHitCheck; // This prevents attempting to damage enemies without line of sight to the player. The custom Colliding code for spinning makes this necessary.

			// This rotation code is unique to this flail, since the sprite isn't rotationally symmetric and has tip.
			bool freeRotation = CurrentAIState == AIState.Ricochet || CurrentAIState == AIState.Dropping;
			if (freeRotation) 
            {
				if (Projectile.velocity.Length() > 1f)
                {
					Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.X * 0.1f; // skid
                }
				else
                {
					Projectile.rotation += Projectile.velocity.X * 0.1f; // roll
                }
			}
			else 
            {
				Vector2 vectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
				Projectile.rotation = vectorTowardsPlayer.ToRotation() + MathHelper.PiOver2;
			}

			// If you have a ball shaped flail, you can use this simplified rotation code instead
			/*
			if (Projectile.velocity.Length() > 1f)
				Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.X * 0.1f; // skid
			else
				Projectile.rotation += Projectile.velocity.X * 0.1f; // roll
			*/

			Projectile.timeLeft = 2; // Makes sure the flail doesn't die (good when the flail is resting on the ground)
			player.heldProj = Projectile.whoAmI;
			player.SetDummyItemTime(2); //Add a delay so the player can't button mash the flail
			player.itemRotation = Projectile.DirectionFrom(mountedCenter).ToRotation();

			if (Projectile.Center.X < mountedCenter.X) 
            {
				player.itemRotation += (float)Math.PI;
			}

			player.itemRotation = MathHelper.WrapAngle(player.itemRotation);
		}

		public override bool OnTileCollide(Vector2 oldVelocity) 
        {
			int impactIntensity = 0;
			Vector2 velocity = Projectile.velocity;
			float bounceFactor = 0.2f;

			if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Ricochet)
            {
				bounceFactor = 0.4f;
            }

			if (CurrentAIState == AIState.Dropping)
            {
				bounceFactor = 0f;
            }

			if (oldVelocity.X != Projectile.velocity.X) 
            {
				if (Math.Abs(oldVelocity.X) > 4f)
                {
					impactIntensity = 1;
                }

				Projectile.velocity.X = (0f - oldVelocity.X) * bounceFactor;
				CollisionCounter += 1f;
			}

			if (oldVelocity.Y != Projectile.velocity.Y) 
            {
				if (Math.Abs(oldVelocity.Y) > 4f)
                {
					impactIntensity = 1;
                }

				Projectile.velocity.Y = (0f - oldVelocity.Y) * bounceFactor;
				CollisionCounter += 1f;
			}

			// Here the tiles spawn dust indicating they've been hit
			if (impactIntensity > 0) 
            {
				Projectile.netUpdate = true;
				for (int i = 0; i < impactIntensity; i++) 
                {
					Collision.HitTiles(Projectile.position, velocity, Projectile.width, Projectile.height);
				}

				SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
			}

			// Force retraction if stuck on tiles while retracting
			if (CurrentAIState != AIState.UnusedState && CurrentAIState != AIState.Spinning && CurrentAIState != AIState.Ricochet && CurrentAIState != AIState.Dropping && CollisionCounter >= 10f) 
            {
				CurrentAIState = AIState.ForcedRetracting;
				Projectile.netUpdate = true;
			}

			return false;
		}

		public override bool? CanDamage() 
        {
			// Flails in spin mode won't damage enemies within the first 12 ticks. Visually this delays the first hit until the player swings the flail around for a full spin before damaging anything.
			if (CurrentAIState == AIState.Spinning && SpinningStateTimer <= 12f)
            {
				return false;
            }

			return base.CanDamage();
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) 
        {
			// Flails do special collision logic that serves to hit anything within an ellipse centered on the player when the flail is spinning around the player. For example, the projectile rotating around the player won't actually hit a bee if it is directly on the player usually, but this code ensures that the bee is hit. This code makes hitting enemies while spinning more consistant and not reliant of the actual position of the flail projectile.
			if (CurrentAIState == AIState.Spinning) 
            {
				Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
				Vector2 shortestVectorFromPlayerToTarget = targetHitbox.ClosestPointInRect(mountedCenter) - mountedCenter;
				shortestVectorFromPlayerToTarget.Y /= 0.8f; // Makes the hit area an ellipse. Vertical hit distance is smaller due to this math.
				float hitRadius = 55f; // The length of the semi-major radius of the ellipse (the long end)
				return shortestVectorFromPlayerToTarget.Length() <= hitRadius;
			}
			// Regular collision logic happens otherwise.
			return base.Colliding(projHitbox, targetHitbox);
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) 
        {
			modifiers.HitDirectionOverride = (Main.player[Projectile.owner].Center.X < target.Center.X) ? 1 : (-1);

			if (CurrentAIState == AIState.Spinning)
            {
                modifiers.Knockback *= 0.25f;
            }

			if (CurrentAIState == AIState.Dropping)
            {
                modifiers.Knockback *= 0.5f;
            }
		}
	}
}