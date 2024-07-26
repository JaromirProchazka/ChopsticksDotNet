using Microsoft.Extensions.Logging;
using ChopsticksDotNet;

namespace ChopsticksForAndroid
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            ChopsticksApi api = new ChopsticksApi(new DefaultConfigBuilder("acala").GetManager());

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
