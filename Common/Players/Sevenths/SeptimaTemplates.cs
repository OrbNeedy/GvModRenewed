using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Common.Players.Sevenths
{
    public enum SeptimaType
    {
        None,
        AzureStriker, 
        EnergyWool, 
        Rebirth
    }

    public static class SeptimaTemplates
    {
        public static Dictionary<SeptimaType, Septima> Septimas { get; private set; } = new();
        public static Dictionary<SeptimaType, Asset<Texture2D>> SeptimaIcons { get; private set; } = new();
        public static Dictionary<SeptimaType, Dictionary<int, Resistance>> NPCDamageResistances { get; set; } = new();
        public static Dictionary<SeptimaType, Dictionary<int, Resistance>> ProjectileDamageResistances { get; set; } = new();
        public static SeptimaType[] _selectableSeptimas = { SeptimaType.AzureStriker, SeptimaType.Rebirth };

        public static void LoadSeptimas(Mod mod)
        {
            SetSelectableSeptimaList();

            Septimas = new() { 
                [SeptimaType.None] = new Septima(),
                [SeptimaType.AzureStriker] = new AzureStriker(),
                [SeptimaType.EnergyWool] = new EnergyWool(),
                [SeptimaType.Rebirth] = new Rebirth()
            };

            foreach (Septima septima in Septimas.Values)
            {
                septima.LoadSeptima(mod);
                NPCDamageResistances[septima.Type] = septima.GetNPCResistances();
                ProjectileDamageResistances[septima.Type] = septima.GetProjectileResistances();
            }
        }

        public static void PostLoadSeptimas(Mod mod)
        {
            foreach (Septima septima in Septimas.Values)
            {
                septima.PostLoadSeptima(mod);
            }
        }

        public static void LoadIcons()
        {
            foreach (SeptimaType type in Septimas.Keys)
            {
                SeptimaIcons.Add(type, ModContent.Request<Texture2D>($"GvMod/Assets/UI/{type.ToString()}SelectionIcon"));
            }
        }

        public static Septima GetSeptimaTemplate(SeptimaType type)
        {
            try
            {
                return Septimas[type];
            } catch (Exception e)
            {
                ModContent.GetInstance<GvMod>().Logger.Error("Could not get requested septima template.", e);
                return new Septima();
            }
        }

        public static void SetSelectableSeptimaList()
        {
            _selectableSeptimas = new[] { SeptimaType.AzureStriker, SeptimaType.Rebirth };
        }

        public static Septima GetNewSeptima(SeptimaType type)
        {
            SetSelectableSeptimaList();

            if (_selectableSeptimas.Contains(type))
            {
                try
                {
                    switch (type)
                    {
                        case SeptimaType.EnergyWool:
                            return new EnergyWool();
                        case SeptimaType.Rebirth:
                            return new Rebirth();
                        case SeptimaType.AzureStriker:
                        default:
                            return new AzureStriker();
                    }
                } catch (Exception e)
                {
                    ModContent.GetInstance<GvMod>().Logger.Error("Could not get requested septima instance.", e);
                    return new AzureStriker();
                }
            } else
            {
                return new AzureStriker();
            }
        }

        public static Septima GetRandSeptima()
        {
            SetSelectableSeptimaList();

            try
            {
                SeptimaType selectedSeptima = Main._rand.Next(_selectableSeptimas);
                switch (selectedSeptima)
                {
                    case SeptimaType.EnergyWool:
                        return new EnergyWool();
                    case SeptimaType.Rebirth:
                        return new Rebirth();
                    case SeptimaType.AzureStriker:
                    default:
                        return new AzureStriker();
                }
            }
            catch (Exception e)
            {
                ModContent.GetInstance<GvMod>().Logger.Error("Could not get requested septima instance.", e);
                return new AzureStriker();
            }
        }
    }
}
