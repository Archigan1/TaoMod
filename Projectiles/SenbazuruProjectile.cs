using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace TaoMod.Projectiles
{
	public class SenbazuruProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			projectile.thrown = true;
			projectile.width = 52;
			projectile.height = 42;
			projectile.aiStyle = 0;
			projectile.ignoreWater = false;
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.timeLeft = 10000;
			projectile.penetrate = -1;
			projectile.scale = 0.75f;
		}
		private bool BouncedOnce;
		private int StopFuckingMovingBitch = 0;
		private bool StartedTimeLeft;
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (BouncedOnce == false)
			{
				Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
				if (projectile.velocity.X != oldVelocity.X)
				{
					projectile.velocity.X = -oldVelocity.X;
				}
				if (projectile.velocity.Y != oldVelocity.Y)
				{
					projectile.velocity.Y = -oldVelocity.Y;
				}
				BouncedOnce = true;
			}
			if(BouncedOnce == true)
			{
				StopFuckingMovingBitch++;
			}
			if (BouncedOnce == true && StopFuckingMovingBitch > 4)
			{
				projectile.velocity = Vector2.Zero;
			}
			if(projectile.velocity == Vector2.Zero && StartedTimeLeft == false)
			{
				projectile.timeLeft = 600;
				StartedTimeLeft = true;
			}
			return false;
		}
		public override void AI()
		{
			projectile.rotation = projectile.velocity.ToRotation();
		}
	}
}