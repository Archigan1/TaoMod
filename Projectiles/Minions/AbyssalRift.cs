using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using TaoMod.PartialPlayerClasses;

namespace TaoMod.Projectiles.Minions
{
	public class AbyssalRift : ModProjectile
	{
        protected float idleAccel = 0.05f;
		protected float spacingMult = 1f;
		protected float viewDist = 400f;
		protected float chaseDist = 200f;
		protected float chaseAccel = 6f;
		protected float inertia = 40f;
		protected float shootCool = 90f;
		protected float shootSpeed;
		protected int shoot;
		public override void SetStaticDefaults() {
			Main.projFrames[projectile.type] = 3;
			Main.projPet[projectile.type] = true;
			ProjectileID.Sets.Homing[projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true; //This is necessary for right-click targeting
        }
		public override void SetDefaults() {
			projectile.netImportant = true;
			projectile.width = 24;
			projectile.height = 32;
			projectile.friendly = true;
			projectile.minion = true;
			projectile.minionSlots = 2;
			projectile.penetrate = -1;
			projectile.timeLeft = 18000;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			inertia = 20f;
			shoot = mod.ProjectileType("AbyssalShard");
			shootSpeed = 24f;
		}

		public virtual void SelectFrame() {
        	projectile.frameCounter++;
			if (projectile.frameCounter >= 8) {
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % 3;
			}
		}
		public virtual void CheckActive() {
			Player player = Main.player[projectile.owner];
			TaoPlayer modPlayer = player.GetModPlayer<TaoPlayer>();
			if (player.dead) {
				modPlayer.riftMinion = false;
			}
			if (modPlayer.riftMinion) {
				projectile.timeLeft = 2;
			}
		}
        public virtual void Behavior() {
			Player player = Main.player[projectile.owner];
			float spacing = (float)projectile.width * spacingMult;
			for (int k = 0; k < 1000; k++) {
				Projectile otherProj = Main.projectile[k];
				if (k != projectile.whoAmI && otherProj.active && otherProj.owner == projectile.owner && otherProj.type == projectile.type && Math.Abs(projectile.position.X - otherProj.position.X) + Math.Abs(projectile.position.Y - otherProj.position.Y) < spacing) {
					if (projectile.position.X < Main.projectile[k].position.X) {
						projectile.velocity.X -= idleAccel;
					}
					else {
						projectile.velocity.X += idleAccel;
					}
					if (projectile.position.Y < Main.projectile[k].position.Y) {
						projectile.velocity.Y -= idleAccel;
					}
					else {
						projectile.velocity.Y += idleAccel;
					}
				}
			}
			Vector2 targetPos = projectile.position;
			float targetDist = viewDist;
			bool target = false;
			projectile.tileCollide = true;
			if (player.HasMinionAttackTargetNPC) {
				NPC npc = Main.npc[player.MinionAttackTargetNPC];
				if (Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height)) {
					targetDist = Vector2.Distance(projectile.Center, targetPos);
					targetPos = npc.Center;
					target = true;
				}
			}
			else {
				for (int k = 0; k < 200; k++) {
					NPC npc = Main.npc[k];
					if (npc.CanBeChasedBy(this, false)) {
						float distance = Vector2.Distance(npc.Center, projectile.Center);
						if ((distance < targetDist || !target) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height)) {
							targetDist = distance;
							targetPos = npc.Center;
							target = true;
						}
					}
				}
			}

			if (Vector2.Distance(player.Center, projectile.Center) > (target ? 1000f : 500f)) {
				projectile.ai[0] = 1f;
				projectile.netUpdate = true;
			}
			if (projectile.ai[0] == 1f) {
				projectile.tileCollide = false;
			}
			if (target && projectile.ai[0] == 0f) {
				Vector2 direction = targetPos - projectile.Center;
				if (direction.Length() > chaseDist) {
					direction.Normalize();
					projectile.velocity = (projectile.velocity * inertia + direction * chaseAccel) / (inertia + 1);
				}
				else {
					projectile.velocity *= (float)Math.Pow(0.97, 40.0 / inertia);
				}
			}
			else {
				if (!Collision.CanHitLine(projectile.Center, 1, 1, player.Center, 1, 1)) {
					projectile.ai[0] = 1f;
				}
				float speed = 6f;
				if (projectile.ai[0] == 1f) {
					speed = 15f;
				}
				Vector2 center = projectile.Center;
				Vector2 direction = player.Center - center;
				projectile.ai[1] = 3600f;
				projectile.netUpdate = true;
				int num = 1;
				for (int k = 0; k < projectile.whoAmI; k++) {
					if (Main.projectile[k].active && Main.projectile[k].owner == projectile.owner && Main.projectile[k].type == projectile.type) {
						num++;
					}
				}
				direction.X -= (float)((10 + num * 40) * player.direction);
				direction.Y -= 70f;
				float distanceTo = direction.Length();
				if (distanceTo > 200f && speed < 9f) {
					speed = 9f;
				}
				if (distanceTo < 100f && projectile.ai[0] == 1f && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height)) {
					projectile.ai[0] = 0f;
					projectile.netUpdate = true;
				}
				if (distanceTo > 2000f) {
					projectile.Center = player.Center;
				}
				if (distanceTo > 48f) {
					direction.Normalize();
					direction *= speed;
					float temp = inertia / 2f;
					projectile.velocity = (projectile.velocity * temp + direction) / (temp + 1);
				}
				else {
					projectile.direction = Main.player[projectile.owner].direction;
					projectile.velocity *= (float)Math.Pow(0.9, 40.0 / inertia);
				}
			}
			projectile.rotation = projectile.velocity.X * 0.05f;
			SelectFrame();
			if (projectile.velocity.X > 0f) {
				projectile.spriteDirection = projectile.direction = -1;
			}
			else if (projectile.velocity.X < 0f) {
				projectile.spriteDirection = projectile.direction = 1;
			}
			if (projectile.ai[1] > 0f) {
				projectile.ai[1] += 1f;
				if (Main.rand.NextBool(3)) {
					projectile.ai[1] += 1f;
				}
			}
			if (projectile.ai[1] > shootCool) {
				projectile.ai[1] = 0f;
				projectile.netUpdate = true;
			}
			if (projectile.ai[0] == 0f) {
				if (target) {
					if ((targetPos - projectile.Center).X > 0f) {
						projectile.spriteDirection = projectile.direction = -1;
					}
					else if ((targetPos - projectile.Center).X < 0f) {
						projectile.spriteDirection = projectile.direction = 1;
					}
					if (projectile.ai[1] == 0f) {
						projectile.ai[1] = 1f;
						if (Main.myPlayer == projectile.owner) {
							Vector2 shootVel = targetPos - projectile.Center;
							if (shootVel == Vector2.Zero) {
								shootVel = new Vector2(0f, 1f);
							}
							shootVel.Normalize();
							shootVel *= shootSpeed;
							int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, shootVel.X, shootVel.Y, shoot, projectile.damage, projectile.knockBack, Main.myPlayer, 0f, 0f);
							Main.projectile[proj].timeLeft = 300;
							Main.projectile[proj].netUpdate = true;
							projectile.netUpdate = true;
						}
					}
				}
			}
		}
		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough) {
			fallThrough = true;
			return true;
		}
        public override void AI(){
            CheckActive();
            Behavior();
        }
	}
}