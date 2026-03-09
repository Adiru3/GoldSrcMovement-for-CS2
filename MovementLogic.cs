using System;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace GoldSrcMovement;

public class MovementLogic
{
    private const float SV_AIRACCELERATE = 100f;
    private const float TICK_RATE = 1.0f / 64.0f;

    public static void ApplyBlock1And2(CCSPlayerController player, GoldSrcMovementConfig config)
    {
        if (player == null || !player.IsValid) return;
        var pawn = player.PlayerPawn.Value;
        if (pawn == null) return;

        var movementServices = pawn.MovementServices;
        if (movementServices == null) return;
        var csms = new CCSPlayer_MovementServices(movementServices.Handle);

        // Block 2: Landing Lag removal
        // Set VelocityModifier to 1.0f every tick so player never loses speed from stamina
        if (config.RemoveLandingLag)
        {
            pawn.VelocityModifier = 1.0f;
        }

        var buttons = player.Buttons;
        var velocity = pawn.AbsVelocity;
        
        uint flags = (uint)pawn.Flags;
        bool isOnGround = (flags & 1) != 0; // FL_ONGROUND is 1<<0

        // Auto Bhop via pure physics injection
        if (config.AutoBhopEnabled && isOnGround && (buttons & PlayerButtons.Jump) != 0)
        {
            // Standard CS2 max jump impulse without stamina penalty
            velocity.Z = 301.993377f;
            pawn.AbsVelocity.Z = velocity.Z;
            Utilities.SetStateChanged(pawn, "CBaseEntity", "m_vecVelocity");
            
            // Force player off ground for this tick if possible
            isOnGround = false; 
        }

        // Block 1: Quake Air Acceleration & Pre-strafe
        if (config.QuakeAirAcceleration)
        {
            Vector wishdir = new Vector(0, 0, 0);
            float forwardmove = 0f;
            float sidemove = 0f;

            if ((buttons & PlayerButtons.Forward) != 0) forwardmove += 1f;
            if ((buttons & PlayerButtons.Back) != 0) forwardmove -= 1f;
            if ((buttons & PlayerButtons.Moveright) != 0) sidemove += 1f;
            if ((buttons & PlayerButtons.Moveleft) != 0) sidemove -= 1f;

            if (forwardmove != 0 || sidemove != 0)
            {
                var eyeAngles = pawn.EyeAngles;
                float yaw = eyeAngles.Y * (float)(Math.PI / 180.0);

                float cp = (float)Math.Cos(yaw);
                float sp = (float)Math.Sin(yaw);

                float rightX = sp;
                float rightY = -cp;

                float forwardX = cp;
                float forwardY = sp;

                wishdir.X = forwardX * forwardmove + rightX * sidemove;
                wishdir.Y = forwardY * forwardmove + rightY * sidemove;
                
                float length = (float)Math.Sqrt(wishdir.X * wishdir.X + wishdir.Y * wishdir.Y);
                if (length > 0)
                {
                    wishdir.X /= length;
                    wishdir.Y /= length;
                }

                if (!isOnGround)
                {
                    // Quake Air Accelerate
                    float wishspeed = 30f; 
                    float currentspeed = velocity.X * wishdir.X + velocity.Y * wishdir.Y;
                    float addspeed = wishspeed - currentspeed;

                    if (addspeed > 0)
                    {
                        float accelspeed = SV_AIRACCELERATE * wishspeed * TICK_RATE;
                        if (accelspeed > addspeed) accelspeed = addspeed;

                        velocity.X += accelspeed * wishdir.X;
                        velocity.Y += accelspeed * wishdir.Y;
                        
                        pawn.AbsVelocity.X = velocity.X;
                        pawn.AbsVelocity.Y = velocity.Y;
                        Utilities.SetStateChanged(pawn, "CBaseEntity", "m_vecVelocity");
                    }
                }
                else if (config.RemoveSpeedCap)
                {
                    // Pre-strafe projection 
                    // To let players reach > 250 speed on ground like old CS 1.6
                    float maxSpeed = 250f;
                    float currentspeed = velocity.X * wishdir.X + velocity.Y * wishdir.Y;
                    float addspeed = maxSpeed - currentspeed;
                    
                    if (addspeed > 0)
                    {
                        float accelspeed = 10f * maxSpeed * TICK_RATE; // Using sv_accelerate 10
                        if (accelspeed > addspeed) accelspeed = addspeed;
                        
                        velocity.X += accelspeed * wishdir.X;
                        velocity.Y += accelspeed * wishdir.Y;
                        
                        // We do not cap if magnitude exceeds 250! The engine does capping at end of tick, 
                        // but setting m_vecVelocity manually after engine runs avoids the cap.
                        pawn.AbsVelocity.X = velocity.X;
                        pawn.AbsVelocity.Y = velocity.Y;
                        Utilities.SetStateChanged(pawn, "CBaseEntity", "m_vecVelocity");
                    }
                }
            }
        }
        
        // Block 4: Ducking & Cooldown
        if (config.InstantDuck)
        {
            // Fully remove ducking cooldowns
            csms.DuckSpeed = 8.0f;
            
            // Force instant hitbox shift (no lerp)
            if (csms.DesiresDuck)
            {
                csms.DuckAmount = 1.0f;
            }
            else
            {
                csms.DuckAmount = 0.0f;
            }
        }
    }
}
