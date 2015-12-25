using InAudioSystem.Runtime;
using System.Collections.Generic;

namespace InAudioSystem
{
    public class InFolderData : InAudioNodeBaseData, IBankUsage
    {
        public bool ExternalPlacement;
        public bool OverrideParentBank;
        public InAudioBankLink BankLink;
        

        public bool ParentBankOverride
        {
            get { return OverrideParentBank; }
            set { OverrideParentBank = value; }
        }

        public InAudioBankLink BankConnection
        {
            get { return BankLink; }
            set { BankLink = value; }
        }

        public float VolumeMin = 1.0f;

        
        public float runtimeVolume;

        [System.NonSerialized]
        public float hiearchyVolume;

        [System.NonSerialized]
        public List<InPlayer> runtimePlayers = new List<InPlayer>();
    }

}