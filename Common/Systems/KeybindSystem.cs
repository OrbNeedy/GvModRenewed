using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace GvMod.Common.Systems
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind primaryAbility { get; private set; }
        public static ModKeybind secondaryAbility { get; private set; }
        public static ModKeybind specialAbility { get; private set; }

        public override void Load()
        {
            primaryAbility = KeybindLoader.RegisterKeybind(Mod, "Primary ability", "F");
            secondaryAbility = KeybindLoader.RegisterKeybind(Mod, "Secondary ability", "Q");
            specialAbility = KeybindLoader.RegisterKeybind(Mod, "Special ability", "X");
            // abilityMenu = KeybindLoader.RegisterKeybind(Mod, "Ability menu", "P");
        }

        public override void Unload()
        {
            primaryAbility = null;
            secondaryAbility = null;
            specialAbility = null;
            // abilityMenu = null;
        }
    }
}
