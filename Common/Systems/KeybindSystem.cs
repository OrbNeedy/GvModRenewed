using Terraria.ModLoader;

namespace GvMod.Common.Systems
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind primaryAbility { get; private set; }
        public static ModKeybind secondaryAbility { get; private set; }
        public static ModKeybind specialAbility { get; private set; }
        public static ModKeybind nextAbility { get; private set; }
        public static ModKeybind previousAbility { get; private set; }
        public static ModKeybind abilityMenu { get; private set; }

        public override void Load()
        {
            primaryAbility = KeybindLoader.RegisterKeybind(Mod, "Primary ability", "F");
            secondaryAbility = KeybindLoader.RegisterKeybind(Mod, "Secondary ability", "Q");
            specialAbility = KeybindLoader.RegisterKeybind(Mod, "Use special ability", "X");
            nextAbility = KeybindLoader.RegisterKeybind(Mod, "Select next ability", "V");
            previousAbility = KeybindLoader.RegisterKeybind(Mod, "Select previous ability", "Z");
            abilityMenu = KeybindLoader.RegisterKeybind(Mod, "Hide UI", "P");
        }

        public override void Unload()
        {
            primaryAbility = null;
            secondaryAbility = null;
            specialAbility = null;
            nextAbility = null;
            previousAbility = null;
            abilityMenu = null;
        }
    }
}
