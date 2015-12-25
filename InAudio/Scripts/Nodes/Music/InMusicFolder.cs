namespace InAudioSystem
{
    public class InMusicFolder : InMusicNode, IBankUsage
    {
        public bool _overrideParentBank;

        public bool _externalPlacement;
        public InAudioBankLink _bankLink;


        public bool ParentBankOverride
        {
            get { return _overrideParentBank; }
            set { _overrideParentBank = value; }
        }

        public InAudioBankLink BankConnection
        {
            get { return _bankLink; }
            set { _bankLink = value; }
        }
    }
}