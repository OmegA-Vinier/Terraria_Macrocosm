using Macrocosm.Content.Buffs.Debuffs;
using Macrocosm.Content.Items.Materials;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Unfriendly.Enemies.Moon {
    public class BloodMoonJuggernaut : MoonEnemy {
        public enum ActionState {
            Walk,
            Roar,
            Punch,
            Sprint,
            Kick
        };

        // ai[0] and ai[3] are used by fighter ai
        public ref float AI_Timer => ref NPC.ai[1];
        public ref float AI_State => ref NPC.ai[2];
        public ref float AI_Direction => ref NPC.localAI[0];

        public override void SetStaticDefaults() {
            base.SetStaticDefaults();

            Main.npcFrameCount[Type] = 32;
        }

        public override void SetDefaults() {
            base.SetDefaults();

            NPC.width = 52;
            NPC.height = 70;
            NPC.damage = 150;
            NPC.defense = 80;
            NPC.lifeMax = 10000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.knockBackResist = 0.0f;
            NPC.aiStyle = NPCAIStyleID.Fighter;
            AIType = NPCID.Krampus;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,

                new FlavorTextBestiaryInfoElement(
                    "Big mofo")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) {
            return spawnInfo.Player.GetModPlayer<MacrocosmPlayer>().ZoneMoon && Main.bloodMoon ? .1f : 0f;
        }

        public override void AI() {
            base.AI(); // base fighter AI 
        }

        public override void PostAI() {
            const float punchCooldown = 180f; // min ticks between attacks 
            const float dashSpeed = 12f; // initial dash speed and cap 
            const float dashDeceleration = 2f; // deceleration factor of the dash

            if (AI_Timer <= 0) {
                AI_Timer = 0;
            }

            // actively targets closest player
            // TODO: check behaviour in MP 
            NPC.TargetClosest();
            Player targetPlayer = Main.player[NPC.target];

            // when not jumping, target is close enough but not too close, on the same horizontal line, no blocks between target and him, and there's no active cooldown
            bool canPunch = NPC.HasValidTarget &&
                       Main.netMode != NetmodeID.MultiplayerClient &&
                       AI_Timer == 0 &&
                       Math.Abs(NPC.velocity.Y) == 0 &&
                       Vector2.Distance(NPC.Center, targetPlayer.Center) <= 16 * 16f &&
                       Vector2.Distance(NPC.Center, targetPlayer.Center) >= 3 * 16f &&
                       Math.Abs(NPC.Center.Y - Main.player[NPC.target].Center.Y) < 2f * 16f &&
                       Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetPlayer.position, targetPlayer.width, targetPlayer.height);

            switch (AI_State) {
                case (float)ActionState.Walk:

                    NPC.damage = 150;

                    if (NPC.velocity.Y < 0f)
                        NPC.velocity.Y += 0.1f; // fall down faster 


                    if (NPC.life <= NPC.lifeMax / 2 && Main.netMode != NetmodeID.MultiplayerClient) {
                        AI_State = (float)ActionState.Sprint;
                        NPC.netUpdate = true;
                    }

                    if (canPunch) {
                        NPC.frameCounter = 0;
                        AI_State = (float)ActionState.Punch;
                        AI_Timer = punchCooldown;
                        NPC.netUpdate = true;
                    }

                    // this is done to avoid base fighter AI boredom mechanic 
                    if (NPC.ai[3] > 0) {
                        NPC.velocity.Y -= 0.5f;
                        NPC.ai[3] = float.MaxValue;
                    }

                    AI_Timer--;
                    break;


                case (float)ActionState.Punch:

                    NPC.damage = 350;

                    targetPlayer.noKnockback = false;

                    targetPlayer.AddBuff(ModContent.BuffType<Fear>(), 120, false);

                    if (NPC.life <= NPC.lifeMax / 2 && Main.netMode != NetmodeID.MultiplayerClient) {
                        AI_State = (float)ActionState.Sprint;
                        NPC.netUpdate = true;
                    }

                    if (AI_Timer == punchCooldown - 1f) {
                        AI_Direction = NPC.direction;
                    }

                    AI_Timer--;

                    NPC.velocity.Y = 2f;

                    if (AI_Timer >= punchCooldown - 60f) // 1 second charge 
                    {
                        NPC.velocity.X = 0;
                    }
                    else {
                        if (AI_Timer == 119) {
                            NPC.velocity.X = dashSpeed * AI_Direction;
                        }

                        if (NPC.direction != AI_Direction && Vector2.Distance(NPC.Center, targetPlayer.Center) > 2 * 16f) {
                            AI_Timer -= 0.5f;

                            NPC.velocity.X -= 0.05f * AI_Direction;
                            if (Math.Sign(NPC.velocity.X) != AI_Direction) {
                                NPC.velocity.X = 0;
                                if (AI_Timer <= 0 && Main.netMode != NetmodeID.MultiplayerClient) {
                                    AI_State = (float)ActionState.Walk;
                                    NPC.netUpdate = true;
                                }
                            }
                        }
                        else {
                            NPC.velocity.X -= AI_Timer * dashDeceleration * -AI_Direction;
                        }

                        if (AI_Timer < punchCooldown - 100f) {
                            AI_State = (float)ActionState.Walk;
                            NPC.netUpdate = true;
                        }
                    }

                    if (Math.Abs(NPC.velocity.X) > dashSpeed) {
                        NPC.velocity.X = dashSpeed * AI_Direction;
                    }

                    if (Vector2.Distance(NPC.Center, targetPlayer.Center) > 22 * 16f && Main.netMode != NetmodeID.MultiplayerClient) {
                        AI_State = (float)ActionState.Walk;
                        NPC.netUpdate = true;
                    }

                    break;

                case (float)ActionState.Sprint:

                    NPC.defense = 120;
                    NPC.damage = 300;

                    if (Math.Abs(NPC.velocity.Y) > 0.5f) // if falling 
                    {
                        NPC.velocity.Y += 0.1f;              // accelerate 	
                        NPC.velocity.X = NPC.direction * 6f; // cap horiz velocity?? 
                    }
                    else {
                        NPC.velocity.X += NPC.direction * 0.4f; // accelerate
                        if (Math.Abs(NPC.velocity.X) >= 8f) {
                            NPC.velocity.X = NPC.direction * 8f; // up to 8f
                        }
                    }

                    break;
            }
        }

        // Frame 0		  : idle
        // Frames 1 - 8   : walking   
        // Frame 9		  : jump w/ arms
        // Frames 10 - 12 : punch chargeup
        // Frames 13 - 17 : punch dash
        // Frames 18 - 20 : roar
        // Frames 21 - 28 : sprint 
        // Frame  29	  : jump w/o arms
        // Frames 30 - 32 : kick
        public override void FindFrame(int frameHeight) {

            bool threeTilesAboveGround = !Main.tile[(int)(NPC.Center.X / 16), (int)((NPC.Center.Y + NPC.height / 2) / 16)].HasTile &&
                                         !Main.tile[(int)(NPC.Center.X / 16), (int)((NPC.Center.Y + NPC.height / 2) / 16) + 1].HasTile &&
                                         !Main.tile[(int)(NPC.Center.X / 16), (int)((NPC.Center.Y + NPC.height / 2) / 16) + 2].HasTile;

            NPC.spriteDirection = NPC.direction;

            if (AI_State == (float)ActionState.Punch)
                NPC.spriteDirection = (int)AI_Direction;

            if (NPC.velocity == Vector2.Zero && AI_State != (float)ActionState.Punch) {
                NPC.frame.Y = 0;      // idle frame 
                NPC.frameCounter = 0;

            }
            else {
                switch (AI_State) {
                    case (float)ActionState.Walk:

                        NPC.frameCounter++;

                        if (threeTilesAboveGround || NPC.velocity.Y > 1f) {
                            NPC.frame.Y = 9 * frameHeight; // frame while above ground 
                        }
                        else {
                            NPC.frame.Y = (int)(NPC.frameCounter / 10 + 1) * frameHeight; // 8 walking frames @ 10 ticks per frame 
                            if (NPC.frameCounter >= 79) {
                                NPC.frameCounter = 0;
                            }
                        }
                        break;

                    case (float)ActionState.Punch:

                        if (NPC.direction != AI_Direction) {
                            NPC.frame.Y = 16 * frameHeight; // after passing through target, static punch frame 
                        }
                        else if (Math.Abs(NPC.velocity.X) < 1f) {
                            if (AI_Timer >= 180f - 15f) {
                                NPC.frame.Y = 10 * frameHeight; // initially static charge frame 
                            }
                            else if (AI_Timer <= 180f - 45f) {
                                NPC.frame.Y = 12 * frameHeight; // another static charge frame after loop  
                            }
                            else {
                                NPC.frameCounter++;

                                NPC.frame.Y = (int)(NPC.frameCounter / 5 + 10) * frameHeight; // 2 punch chargeup loop frames @ 5 ticks per frame 
                                if (NPC.frameCounter >= 9) {
                                    NPC.frameCounter = 0;
                                }
                            }
                        }
                        else {
                            if (Math.Abs(NPC.velocity.X) > 4f) {
                                NPC.frame.Y = 13 * frameHeight; // while dashing use the fist motion blur effect frame 
                            }
                            else {
                                NPC.frame.Y = (int)(NPC.frameCounter / 5 + 14) * frameHeight; // 3 punch dash frames @ 5 ticks per frame 
                                if (NPC.frameCounter >= 9) {
                                    NPC.frameCounter = 0;
                                }
                            }
                        }
                        break;

                    case (float)ActionState.Sprint:
                        if (NPC.velocity.Y < 1f) {
                            NPC.frameCounter++;
                        }

                        if (threeTilesAboveGround || NPC.velocity.Y > 1.5f) {
                            NPC.frame.Y = 28 * frameHeight; // frame while above ground  (armless)
                        }
                        else {
                            NPC.frame.Y = (int)(NPC.frameCounter / 5 + 21) * frameHeight; // 8 sprint frames @ 5 ticks per frame 
                            if (NPC.frameCounter >= 39) {
                                NPC.frameCounter = 0;
                            }
                        }
                        break;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot loot) {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<CosmicDust>()));             // Always drop 1 cosmic dust
            loot.Add(ItemDropRule.Common(ModContent.ItemType<ArtemiteOre>(), 16, 1, 6));  // 1/16 chance to drop 1-6 Artemite Ore
            loot.Add(ItemDropRule.Common(ModContent.ItemType<ChandriumOre>(), 16, 1, 6)); // 1/16 chance to drop 1-6 Chandrium Ore
            loot.Add(ItemDropRule.Common(ModContent.ItemType<SeleniteOre>(), 16, 1, 6));  // 1/16 chance to drop 1-6 Selenite Ore
            loot.Add(ItemDropRule.Common(ModContent.ItemType<DianiteOre>(), 16, 1, 6));   // 1/16 chance to drop 1-6 DianiteOre Ore
        }

        public override void HitEffect(int hitDirection, double damage) {

            if (NPC.life > 0) {
                for (int i = 0; i < 10; i++) {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
                    Dust dust = Main.dust[dustIndex];
                    dust.velocity.X *= dust.velocity.X * 1.25f * hitDirection + Main.rand.Next(0, 100) * 0.015f;
                    dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                    dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                }
            }

            if (Main.netMode == NetmodeID.Server) {
                return; // don't run on the server
            }

            var entitySource = NPC.GetSource_Death();

            if (NPC.life <= NPC.lifeMax / 2 && AI_State != (float)ActionState.Sprint) {
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("JuggernautGoreArm1").Type);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("JuggernautGoreArm2").Type);

                for (int i = 0; i < 50; i++) {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
                    Dust dust = Main.dust[dustIndex];
                    dust.velocity.X *= dust.velocity.X * 1.25f * hitDirection + Main.rand.Next(0, 100) * 0.015f;
                    dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                    dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                }
            }

            if (NPC.life <= 0) {

                for (int i = 0; i < 50; i++) {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
                    Dust dust = Main.dust[dustIndex];
                    dust.velocity.X *= dust.velocity.X * 1.25f * hitDirection + Main.rand.Next(0, 100) * 0.015f;
                    dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                    dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                }
            }
        }
    }
}
