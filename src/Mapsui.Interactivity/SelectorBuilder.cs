using Mapsui.Interactivity.Interfaces;

namespace Mapsui.Interactivity
{
    internal class SelectorBuilder : ISelectorBuilder
    {
        private readonly Func<IInteractive> _builder;

        public SelectorBuilder(Func<IInteractive> builder)
        {
            _builder = builder;
        }

        public ISelector Build()
        {
            return (ISelector)_builder.Invoke();
        }
    }
}
