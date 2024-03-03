using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
	public class DianiteMeteor : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Type] = 18;
			ProjectileID.Sets.TrailingMode[Type] = 3;
		}

		public override void SetDefaults()
		{
			Projectile.width = 26;
			Projectile.height = 26;
			Projectile.aiStyle = 56;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.ignoreWater = true;

			Projectile.alpha = 255;

			Projectile.SetTrail<DianiteMeteorTrail>();
		}

		public ref float InitialTargetPositionY => ref Projectile.ai[0];


		bool spawned = false;
		bool rotationClockwise = false;

		public override void AI()
		{
			if (!spawned)
			{
				rotationClockwise = Main.rand.NextBool();
				spawned = true;
				Projectile.netUpdate = true;
			}

			Projectile.velocity.Y += 0.2f;

			if (rotationClockwise)
				Projectile.rotation += 0.1f;
			else
				Projectile.rotation -= 0.1f;


            if (Projectile.alpha > 0)
                 Projectile.alpha -= 15;
 
            Vector2 velocity = -Projectile.velocity.RotatedByRandom(MathHelper.Pi / 2f) * 0.1f;
            Dust dust = Dust.NewDustDirect(Projectile.position, (int)(Projectile.width), (int)(Projectile.height), DustID.Flare, velocity.X, velocity.Y, Scale: 1f);
            dust.noGravity = true;

        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			if (InitialTargetPositionY > Projectile.position.Y)
				return false;

			return true;
		}

		private SpriteBatchState state;
		public override bool PreDraw(ref Color lightColor)
		{
			state.SaveState(Main.spriteBatch);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(BlendState.Additive, state);

			Projectile.GetTrail().Draw(TextureAssets.Projectile[Type].Size() / 2f);
            float count = 25f * (float)(1f - Projectile.alpha/255f);
            for (int n = 2; n < count; n++)
            {
                Vector2 trailPosition = Projectile.Center - Projectile.velocity.SafeNormalize(default) * n * 7;
                Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, trailPosition - Main.screenPosition, null, Color.OrangeRed * (1f - (float)n / count), 0f, TextureAssets.Projectile[Type].Value.Size() / 2f, Projectile.scale * (0.5f + 0.5f * (1f - (float)n / count)), SpriteEffects.None, 0f);
            }

            Main.spriteBatch.End();
			Main.spriteBatch.Begin(state);

			return true;
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 25; i++)
			{
				Vector2 velocity = Main.rand.NextVector2Circular(25f, 25f);
				Dust dust = Dust.NewDustDirect(Projectile.position + Projectile.oldVelocity, (int)(Projectile.width), Projectile.height, DustID.Flare, velocity.X, velocity.Y, Scale: 2.4f);
				dust.noGravity = true;
			}

			//var smoke = Particle.CreateParticle<Smoke>(Projectile.Center + Projectile.oldVelocity, Vector2.Zero, scale: 0.6f); 
			//smoke.Velocity *= 0.2f;
			//smoke.Velocity.X += Main.rand.Next(-10, 11) * 0.15f;
			//smoke.Velocity.Y += Main.rand.Next(-10, 11) * 0.15f;

			var explosion = Particle.CreateParticle<TintableExplosion>(p =>
			{
				p.Position = Projectile.Center + Projectile.oldVelocity + Main.rand.NextVector2Circular(10f, 10f);
				p.DrawColor = (new Color(195, 115, 62)).WithOpacity(0.6f);
				p.Scale = 0.9f;
				p.NumberOfInnerReplicas = 6;
				p.ReplicaScalingFactor = 0.3f;
			});

		}
	}
}