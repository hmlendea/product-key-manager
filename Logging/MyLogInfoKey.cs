using NuciLog.Core;

namespace ProductKeyManager.Logging
{
    public sealed class MyLogInfoKey : LogInfoKey
    {
        MyLogInfoKey(string name)
            : base(name)
        {
            
        }

        public static LogInfoKey StoreName => new MyLogInfoKey(nameof(StoreName));

        public static LogInfoKey ProductName => new MyLogInfoKey(nameof(ProductName));
    }
}
