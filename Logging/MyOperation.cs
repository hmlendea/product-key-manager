using NuciLog.Core;

namespace ProductKeyManager.Logging
{
    public sealed class MyOperation : Operation
    {
        MyOperation(string name)
            : base(name)
        {
            
        }
        
        public static Operation GetProductKey => new MyOperation(nameof(GetProductKey));
    }
}
