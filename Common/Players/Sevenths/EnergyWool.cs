

namespace GvMod.Common.Players.Sevenths
{
    class EnergyWool : Septima
    {
        public override SeptimaType Type { get; protected set; } = SeptimaType.EnergyWool;
        public override float EPUseBase { get; protected set; } = 0.75f;
        public override float EPRecoveryBaseRate { get; protected set; } = 0.004761904762f;
        public override int EPCooldownBaseTimer { get; protected set; } = 90;
        public override float OverheatRecoveryBaseRate { get; protected set; } = 0.002380952381f;
        public override float SPRecoveryBaseRate { get; protected set; } = 0.000185f;
        public override int PrevasionEPCooldownBaseTimer { get; protected set; } = 90;
        public override int MaxEPModifier { get; set; }
    }
}
