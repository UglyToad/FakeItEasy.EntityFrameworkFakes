namespace UglyToad.FakeItEasy.EntityFrameworkFakes.Tests.Domain
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Photon
    {
        [Key]
        public Guid WavePacketIdentifier { get; protected set; }

        public long Energy { get; private set; }

        public Photon(Guid wavePacketIdentifier, long energy)
        {
            WavePacketIdentifier = wavePacketIdentifier;
            Energy = energy;
        }
    }
}