using System.Windows.Automation.Peers;

namespace ModernWpf.Controls
{
    public class InfoBarAutomationPeer : FrameworkElementAutomationPeer
    {
        public InfoBarAutomationPeer(InfoBar owner) : base(owner) { }

        protected override string GetClassNameCore() => nameof(InfoBar);

        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.StatusBar;
    }
}
