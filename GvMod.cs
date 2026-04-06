using GvMod.Common.Edits;
using GvMod.Common.Players;
using GvMod.Common.Players.Sevenths;
using GvMod.Content.Items;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace GvMod
{
    public enum MessageType : byte
    {
        ResurrectionSync, 
        SeptimaStateSync, 
        ReincarnationVisualSync, 
        PrevasionVisualSync, 
        RechargeSync
    }


    public class GvMod : Mod
    {
        public override void Load()
        {
            Asset<Effect> morvoltShader = Assets.Request<Effect>("Effects/RainbowEffect");
            Asset<Effect> petrifiedShader = Assets.Request<Effect>("Effects/PetrifiedEffect");
            Asset<Effect> rebirthShader = Assets.Request<Effect>("Effects/RebirthEffect");

            GameShaders.Misc["GvMod:Rainbow"] = new MiscShaderData(morvoltShader, "MorvoltWings");
            GameShaders.Misc["GvMod:Petrification"] = new MiscShaderData(petrifiedShader, "PetrificationColor");
            GameShaders.Misc["GvMod:Rebirth"] = new MiscShaderData(rebirthShader, "Rebirth").
                UseImage1(Assets.Request<Texture2D>("Assets/Effects/RebirthMask"));

            GameShaders.Armor.BindShader(ModContent.ItemType<PowerfulDye>(), 
                new ArmorShaderData(morvoltShader, "MorvoltWings"));

            SeptimaTemplates.LoadSeptimas(this);
            SeptimaTemplates.LoadIcons();

            CharCreationEdit.Load();
        }

        public override void PostSetupContent()
        {
            SeptimaTemplates.PostLoadSeptimas(this);
        }

        public override void Unload()
        {
            CrossModUIEditCompat.Unload();
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            byte type = reader.ReadByte();
            switch ((MessageType)type)
            {
                case MessageType.ResurrectionSync:
                    ResurrectionPlayer.ReceiveResurrectSync(reader, whoAmI);
                    break;
                case MessageType.SeptimaStateSync:
                    SeptimaPlayer.ReceiveStateSync(reader, whoAmI);
                    break;
                case MessageType.ReincarnationVisualSync:
                    ResurrectionPlayer.ReceiveVisualResurrectSync(reader, whoAmI);
                    break;
                case MessageType.PrevasionVisualSync:
                    PlayerPrevasion.ReceivePrevasionSync(reader, whoAmI);
                    break;
                case MessageType.RechargeSync:
                    SeptimaPlayer.ReceiveRechargeSync(reader, whoAmI);
                    break;
                default:
                    Logger.Error($"Received packet with unrecognized id {type}.");
                    break;
            }
        }
    }
}
