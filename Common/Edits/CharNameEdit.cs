using GvMod.Common.Players;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.Localization;

namespace GvMod.Common.Edits
{
    // I might not need to edit the name, so this may be deleted later
    class CharNameEdit
    {
        public static void Load()
        {
            Terraria.GameContent.UI.Elements.IL_UICharacterListItem.DrawSelf += UICharacterListItem_DrawSelf;
        }

        private static void UICharacterListItem_DrawSelf(ILContext il)
        {
            ILCursor c = new(il);

            if (!c.TryGotoNext(MoveType.After, x => x.MatchStloc(4)))
                return;

            c.Emit(OpCodes.Ldloc_S, (byte)4);
            c.Emit(OpCodes.Ldarg_0);
            c.Emit<Terraria.GameContent.UI.Elements.UICharacterListItem>(OpCodes.Ldfld, "_data");
            c.Emit<Terraria.IO.PlayerFileData>(OpCodes.Callvirt, "get_Player");

            c.EmitDelegate((string input, Player player) =>
            {
                var data = player.GetModPlayer<SeptimaPlayer>().septima;
                return (data is not null && data.Type != Players.Sevenths.SeptimaType.None) ? input + 
                Language.GetText($"Mods.GvMod.UI.SeptimaNames.{data.Type.ToString()}") : input;
            });

            c.Emit(OpCodes.Stloc_S, (byte)4);
        }
    }
}
