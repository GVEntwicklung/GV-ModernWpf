using System.Linq;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ModernWpf.Controls;

namespace ModernWpf.Automation.Peers
{
    public class ToggleButtonGroupAutomationPeer : FrameworkElementAutomationPeer, ISelectionProvider
    {
        public ToggleButtonGroupAutomationPeer(ToggleButtonGroup owner) : base(owner)
        {
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Selection)
                return this;

            return base.GetPattern(patternInterface);
        }

        protected override string GetClassNameCore()
        {
            return nameof(ToggleButtonGroup);
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.List;
        }

        protected override string GetLocalizedControlTypeCore()
        {
            return "toggle button group";
        }

        // ISelectionProvider
        public bool CanSelectMultiple => false;
        public bool IsSelectionRequired => true;

        public IRawElementProviderSimple[] GetSelection()
        {
            var owner = GetImpl();
            if (owner.SelectedValue == null)
                return System.Array.Empty<IRawElementProviderSimple>();

            var panel = owner.Template?.FindName("PART_ItemsPanel", owner) as Panel;
            if (panel == null)
                return System.Array.Empty<IRawElementProviderSimple>();

            foreach (ToggleButton button in panel.Children.OfType<ToggleButton>())
            {
                if (Equals(button.Tag, owner.SelectedValue))
                {
                    var peer = FromElement(button) ?? CreatePeerForElement(button);
                    if (peer != null)
                        return new[] { ProviderFromPeer(peer) };
                }
            }

            return System.Array.Empty<IRawElementProviderSimple>();
        }

        private ToggleButtonGroup GetImpl()
        {
            return (ToggleButtonGroup)Owner;
        }
    }
}
