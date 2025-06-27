namespace EmergencyRoulette
{
    public class SymbolPicker
    {
        private readonly SymbolLibrary _library;
        
        public SymbolPicker(SymbolLibrary library)
        {
            _library = library;
        }

        public SymbolType Pick()
        {
            return SymbolType.Data;
        }
    }
}