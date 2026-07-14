using NuciLog.Core;

namespace ProductKeyManager.Logging
{
    public sealed class MyOperation : Operation
    {
        MyOperation(string name) : base(name) { }

        public static Operation AddProductKey => new MyOperation(nameof(AddProductKey));

        public static Operation GetProductKey => new MyOperation(nameof(GetProductKey));

        public static Operation UpdateProductKey => new MyOperation(nameof(UpdateProductKey));
    }
}
