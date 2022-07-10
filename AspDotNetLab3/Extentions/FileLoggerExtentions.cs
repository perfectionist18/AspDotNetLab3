using AspDotNetLab3.Logger;
using Microsoft.Extensions.Logging;

namespace AspDotNetLab3.Extentions
{
    public static class FileLoggerExtentions
    {
        public static ILoggerFactory AddFile(this ILoggerFactory factory, string filePath)
        {
            factory.AddProvider(new FileLoggerProvider(filePath));
            return factory;
        }
    }
}
