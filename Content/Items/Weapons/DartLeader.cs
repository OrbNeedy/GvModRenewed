﻿using Terraria.Audio;
using GvMod.Content.Items.Ammo;
using GvMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using System.Collections.Generic;

namespace GvMod.Content.Items.Weapons
{
    public enum DartLeaderUpgrades
    {
        None,
        Naga,
        Technos,
        Orochi,
        Mizuchi,
        Vasuki, 
        Dullahan, 
        Ouroboros
    }
    public class DartLeader : ModItem
    {
        // Max: 3
        public int capacity = 1;
        private List<DartLeaderUpgrades> upgrades = new() { 
            DartLeaderUpgrades.None, 
            DartLeaderUpgrades.None, 
            DartLeaderUpgrades.None };
        private int orochiTimer = 0;

        public override void SetDefaults()
        {
            Item.Size = new Vector2(32, 24);
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 1;
            Item.knockBack = 0;
            Item.rare = ItemRarityID.LightRed;

            Item.shoot = ModContent.ProjectileType<HairDartProjectile>();
            Item.useAmmo = ModContent.ItemType<HairDart>();
            Item.shootSpeed = 10;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.UseSound = new SoundStyle("GvMod/Assets/Sfx/GunShot_G") with
            {
                PitchVariance = 0.1f
            };
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["Capacity"] = capacity;
            for (int i = 0; i < upgrades.Count; i++)
            {
                tag[$"Upgrade{i}"] = (int)upgrades[i];
            }
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey($"Capacity"))
            {
                capacity = tag.GetInt("Capacity");
            }
            for (int i = 0; i < upgrades.Count; i++)
            {
                if (tag.ContainsKey($"Upgrade{i}"))
                {
                    upgrades[i] = (DartLeaderUpgrades)tag.GetInt($"Upgrade{i}");
                }
            }
        }

        public override void UpdateInventory(Player player)
        {
            if (orochiTimer > 0)
            {
                orochiTimer--;
            }
            base.UpdateInventory(player);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (orochiTimer > 0)
            {
                orochiTimer--;
            }
            base.Update(ref gravity, ref maxFallSpeed);
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            for (int i = 0; i < capacity; i++)
            {
                if (upgrades[i] == DartLeaderUpgrades.Dullahan)
                {
                    damage += 39;
                    break;
                }
            }
            base.ModifyWeaponDamage(player, ref damage);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (!upgrades.Contains(DartLeaderUpgrades.Dullahan))
            {
                damage = 1;
            }
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Main.NewText("Capacity: " + capacity);

            //Main.NewText("Control hold: " + player.holdDownCardinalTimer[0]);
            if (upgrades.Contains(DartLeaderUpgrades.Orochi) && orochiTimer <= 0)
            {
                int verticalDirection = 1;
                if (velocity.Y < 0) verticalDirection = -1;

                Projectile.NewProjectile(source, player.Center, new Vector2(-player.direction, verticalDirection), 
                    ModContent.ProjectileType<OrochiDrone>(), damage, knockback, player.whoAmI,
                    (int)upgrades[0], (int)upgrades[1], (int)upgrades[2]);
                orochiTimer = 210;
            }

            bool technos = false;
            //Main.NewText("Upgrades: ");
            for (int i = 0; i < capacity; i++)
            {
                //Main.NewText(upgrades[i].ToString());
                if (upgrades[i] == DartLeaderUpgrades.Technos) technos = true;
            }

            if (technos)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedBy(-MathHelper.PiOver4), type,
                    damage, knockback, player.whoAmI, (int)upgrades[0], (int)upgrades[1], (int)upgrades[2]);
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI,
                    (int)upgrades[0], (int)upgrades[1], (int)upgrades[2]);
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.PiOver4), type,
                    damage, knockback, player.whoAmI, (int)upgrades[0], (int)upgrades[1], (int)upgrades[2]);
            } else
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback,
                    player.whoAmI, (int)upgrades[0], (int)upgrades[1], (int)upgrades[2]);
            }
            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (upgrades.Contains(DartLeaderUpgrades.Ouroboros))
            {
                return Main.rand.NextBool(3);
            }
            return base.CanConsumeAmmo(ammo, player);
        }

        public bool IncreaseCapacity()
        {
            //Main.NewText("Trying to increase capacity.");
            if (capacity >= 3)
            {
                //Main.NewText("Capacity can't increase.");
                return false;
            }

            capacity++;
            //Main.NewText("Capacity increased.");
            return true;
        }

        public void AddUpgrade(DartLeaderUpgrades upgrade)
        {
            //Main.NewText("Adding upgrade");
            // Clear
            if (upgrade == DartLeaderUpgrades.None)
            {
                upgrades = new() { DartLeaderUpgrades.None, DartLeaderUpgrades.None, DartLeaderUpgrades.None };
                //Main.NewText("Upgrades cleared");
                return;
            }

            // Don't repeat upgrades
            if (upgrades.Contains(upgrade)) return;

            // Only replace DartLeaderUpgrades.None
            for (int i = 0; i < capacity; i++)
            {
                if (upgrades[i] == DartLeaderUpgrades.None)
                {
                    upgrades[i] = upgrade;
                    //Main.NewText("Added " + upgrade.ToString());
                    return;
                }
            }

            // Default to the last available slot
            //Main.NewText("Added " + upgrade.ToString() + " replacing the last slot");
            upgrades[capacity - 1] = upgrade;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 10)
                .AddIngredient(ItemID.Revolver)
                .AddIngredient(ItemID.IllegalGunParts)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
