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
        
        public static Operation StoreProductKey => new MyOperation(nameof(StoreProductKey));
        
        public static Operation UpdateProductKey => new MyOperation(nameof(UpdateProductKey));
    }
}
