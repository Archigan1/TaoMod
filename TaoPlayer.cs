using Terraria;
using Terraria.ID;
using TaoMod.Items.Accessories;
using TaoMod.Buffs;
using Terraria.ModLoader;
using System.Threading.Tasks;
using TaoMod.UI;
using System.Collections.Generic;
using System.Linq;
using TaoMod.Items.Consumables;
using System;

namespace TaoMod
{
	public class TaoPlayer : ModPlayer
	{
		public static TaoPlayer ModPlayer(Player player) {
			return player.GetModPlayer<TaoPlayer>();
		}
		public bool abyssalDebuff;
		public bool VoidGazersMirror;
		public bool riftMinion;
		public bool HasSoulbinder;
		public override void ResetEffects() {
			ResetVariables();
			abyssalDebuff = false;
			riftMinion = false;
			HasSoulbinder = false;
		}

		private void AddStartItem(ref IList<Item> items, int itemType, int stack = 1)
		{
			Item item = new Item();
			item.SetDefaults(itemType);
			item.stack = stack;
			items.Add(item);
		}
		public override void SetupStartInventory(IList<Item> items, bool mediumcoreDeath)
		{
			Item item = new Item();
			item.SetDefaults(ModContent.ItemType<CubesOfManyWonders>());
			item.stack = 1;
			items.Add(item);
		}

		private int ItemType<T>()
		{
			throw new NotImplementedException();
		}

		public override void UpdateDead() {
			ResetVariables();
		}

	    private void ResetVariables() {
		}
		public virtual void UpdateLifeRegen() {
			if (abyssalDebuff) {
				// These lines zero out any positive lifeRegen. This is expected for all bad life regeneration effects.
				if (player.lifeRegen > 0) {
					player.lifeRegen = 0;
				}
				player.lifeRegenTime = 0;
				// lifeRegen is measured in 1/2 life per second. Therefore, this effect causes 8 life lost per second.
				player.lifeRegen -= 20;
			}
		}
		public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
		{
			if (Main.LocalPlayer.GetModPlayer<TaoPlayer>().HasSoulbinder == true)
			{
				//Adds OnFire for 5 seconds
				int amountToHeal = Main.rand.Next(1, 10); // Heal half of damage dealt.
				if (amountToHeal + player.statLife > player.statLifeMax2)
					amountToHeal = player.statLifeMax - player.statLife; // If healing is larger than health currently missing.
				player.statLife += amountToHeal;
				player.HealEffect(amountToHeal, true);
			}
			base.OnHitNPC(item, target, damage, knockback, crit);
		}
		public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			if (Main.LocalPlayer.GetModPlayer<TaoPlayer>().HasSoulbinder == true)
			{
				int amountToHeal = Main.rand.Next(1, 10); // Heal half of damage dealt.
				if (amountToHeal + player.statLife > player.statLifeMax2)
					amountToHeal = player.statLifeMax - player.statLife; // If healing is larger than health currently missing.
				player.statLife += amountToHeal;
				player.HealEffect(amountToHeal, true);
			}
			base.OnHitNPCWithProj(proj, target, damage, knockback, crit);
		}
	}
}