using System;
using Terraria.GameContent.UI.States;
using Terraria;

namespace GvMod.Common.Edits
{
    class CharCreationHijackSaveDetour
    {
        private static bool FirstSave = false;

        public static void Load()
        {
            On_UICharacterCreation.FinishCreatingCharacter += UICharacterCreation_FinishCreatingCharacter;
            Terraria.IO.On_PlayerFileData.CreateAndSave += PlayerFileData_CreateAndSave;

            CrossModUIEditCompat.AddCharCreationDetour();
        }

        internal static void CrossmodFinishHookCharacter(Action<object> orig, object self)
        {
            FirstSave = true;
            orig(self);
            FirstSave = false;
        }

        private static void UICharacterCreation_FinishCreatingCharacter(On_UICharacterCreation.orig_FinishCreatingCharacter orig, UICharacterCreation self)
        {
            FirstSave = true;
            orig(self);
            FirstSave = false;
        }

        private static Terraria.IO.PlayerFileData PlayerFileData_CreateAndSave(Terraria.IO.On_PlayerFileData.orig_CreateAndSave orig, Player player)
        {
            /*if (FirstSave)
            {
                Septima data = player.GetModPlayer<SeptimaPlayer>().septima;

                if (data.Type == SeptimaType.None || data == null)
                    data = Custom.GetCustomBackground(player, true);

                data.ApplyToPlayer(player);
                data.Delegates.ModifyPlayerCreation?.Invoke(player);
            }*/

            return orig(player);
        }
    }
}
