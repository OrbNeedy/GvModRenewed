using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using System.Linq;
using System.Reflection;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using Mono.Cecil.Cil;
using GvMod.Common.Players;
using GvMod.Common.Players.Sevenths;
using GvMod.Common.UI;

// This entire folder was copied from https://github.com/GabeHasWon/NewBeginnings

namespace GvMod.Common.Edits
{
    class CharCreationEdit
    {
        public static UICharacterCreation _self;
        public static FieldInfo InternalPlayerField;

        internal static void Load()
        {
            InternalPlayerField = typeof(UICharacterCreation).GetField("_player", BindingFlags.Instance | BindingFlags.NonPublic);

            IL_UICharacterCreation.MakeInfoMenu += UICharacterCreation_MakeInfoMenu;

            CharCreationHijackSaveDetour.Load();

            //CharNameEdit.Load();
        }

        /// <summary>IL edit that shoves everything we want into the vanilla UI.</summary>
        private static void UICharacterCreation_MakeInfoMenu(ILContext il)
        {
            ILCursor c = new(il);

            if (!c.TryGotoNext(x => x.MatchLdarg(1)))
                return;

            c.Index += 3;

            c.Emit(OpCodes.Ldloc_0);
            c.EmitDelegate(AddNewButton); //Add player bg button

            if (!c.TryGotoNext(x => x.MatchLdstr("UI.PlayerEmptyName")))
                return;

            if (!c.TryGotoNext(x => x.MatchLdcR4(0.5f)))
                return;

            if (!c.TryGotoNext(x => x.MatchLdloc(0)))
                return;

            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate((UICharacterCreation self) =>
            {
                _self = self;

                var plr = InternalPlayerField.GetValue(_self) as Player;
                if (plr.GetModPlayer<SeptimaPlayer>().septima == null || 
                plr.GetModPlayer<SeptimaPlayer>().septimaType == SeptimaType.None) //Set background data if it's null
                    plr.GetModPlayer<SeptimaPlayer>().SetSeptima(SeptimaTemplates.GetRandSeptima());
            });

            c.Emit(OpCodes.Ldloc_1);
            c.EmitDelegate((UICharacterNameButton characterNameButton) => //Adjust character name button to give space for the player bg button
            {
                characterNameButton.HAlign = 0f;
                characterNameButton.Width = StyleDimension.FromPixelsAndPercent(0f, 0.9f);
            });
        }

        /// <summary>Adds the background icon button to the UI.</summary>
        private static void AddNewButton(UIElement parent)
        {
            UIImageButton backgroundButton = new(ModContent.Request<Texture2D>("GvMod/Assets/UI/SeptimaSelection"))
            {
                HAlign = 0.99f,
                VAlign = 0.0f,
                Width = StyleDimension.FromPixels(40f),
                Height = StyleDimension.FromPixels(40f)
            };

            backgroundButton.SetPadding(0f);
            backgroundButton.OnLeftMouseDown += BackgroundButton_OnMouseDown;

            parent.Append(backgroundButton);
        }

        /// <summary>OnMouseDown event for the background icon button. Sets the background list, changes the description and sets defaults where necessary.</summary>
        /// <param name="evt"></param>
        /// <param name="listeningElement"></param>
        private static void BackgroundButton_OnMouseDown(UIMouseEvent evt, UIElement listeningElement)
        {
            var plr = InternalPlayerField.GetValue(_self) as Player;
            Main.MenuUI.SetState(new SeptimaSelectionUI(plr, (UIMouseEvent evt, UIElement listeningElement) => Main.MenuUI.SetState(_self)));
        }
    }
}
