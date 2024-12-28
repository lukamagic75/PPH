using System;
using Avalonia;
using PPH.Library.Services;
using PPH.Library.ViewModels;
using PPH.Services;
using Microsoft.Extensions.DependencyInjection;

namespace PPH;

public class ServiceLocator {
    private readonly IServiceProvider _serviceProvider;

    private static ServiceLocator _current;
    
    // 获取ServiceLocator的实例
    public static ServiceLocator Current {
        get
        {
            if (_current is not null) {
                return _current;
            }

            if (Application.Current.TryGetResource(nameof(ServiceLocator),
                    null, out var resource) &&
                resource is ServiceLocator serviceLocator) {
                return _current = serviceLocator;
            }

            throw new Exception("理论上来讲不应该发生这种情况。");
        }
    }
    
    //对外暴露
    public TodayWordViewModel TodayWordViewModel => 
        _serviceProvider.GetRequiredService<TodayWordViewModel>();
    
    public MainWindowViewModel MainWindowViewModel =>
        _serviceProvider.GetRequiredService<MainWindowViewModel>();
    
    
    // TODO Delete this
    public IRootNavigationService RootNavigationService =>
        _serviceProvider.GetRequiredService<IRootNavigationService>();
    
    public MainViewModel MainViewModel =>
        _serviceProvider.GetRequiredService<MainViewModel>();
    
    public TranslateViewModel TranslateViewModel =>
        _serviceProvider.GetRequiredService<TranslateViewModel>();
    
    public DetailViewModel DetailViewModel =>
        _serviceProvider.GetRequiredService<DetailViewModel>();
    
    public InitializationViewModel InitializationViewModel =>
        _serviceProvider.GetRequiredService<InitializationViewModel>();
    
    public QueryWordViewModel QueryWordViewModel =>
        _serviceProvider.GetRequiredService<QueryWordViewModel>();
    
    public QueryWordResultViewModel QueryWordResultViewModel =>
        _serviceProvider.GetRequiredService<QueryWordResultViewModel>();
    
    public FavoriteWordViewModel FavoriteWordViewModel =>
        _serviceProvider.GetRequiredService<FavoriteWordViewModel>();
    
    public QuizViewModel QuizViewModel =>
        _serviceProvider.GetRequiredService<QuizViewModel>();
    public MemoViewModel MemoViewModel => 
        _serviceProvider.GetRequiredService<MemoViewModel>();
    
    public WeatherViewModel WeatherViewModel => 
        _serviceProvider.GetRequiredService<WeatherViewModel>();
    
    public ChatViewModel ChatViewModel => 
        _serviceProvider.GetRequiredService<ChatViewModel>();
    
    public MusicPlayerViewModel MusicPlayerViewModel => 
        _serviceProvider.GetRequiredService<MusicPlayerViewModel>();
    
    public ServiceLocator() {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IPreferenceStorage, FilePreferenceStorage>();
        serviceCollection.AddSingleton<IWordStorage, WordStorage>();
        serviceCollection.AddSingleton<IAlertService, AlertService>();
        serviceCollection.AddSingleton<IRootNavigationService, RootNavigationService>();
        serviceCollection.AddSingleton<IMenuNavigationService, MenuNavigationService>();
        serviceCollection.AddSingleton<IContentNavigationService, ContentNavigationService>();
        serviceCollection.AddSingleton<ITranslateService, TranslateService>();
        serviceCollection.AddSingleton<ITodayImageService, BingImageService>();
        serviceCollection.AddSingleton<ITodayImageStorage, TodayImageStorage>();
        serviceCollection.AddSingleton<IFavoriteWordStorage, FavoriteWordStorage>();
        serviceCollection.AddSingleton<IMemoStorage, MemoStorage>();
        serviceCollection.AddSingleton<IChatService, ChatService>();
        serviceCollection.AddSingleton<IMusicPlayer, MusicPlayer>();
        serviceCollection.AddSingleton<IMusicStorage, MusicStorage>();
        
        serviceCollection.AddSingleton<MainWindowViewModel>();
        serviceCollection.AddSingleton<TodayWordViewModel>();
        serviceCollection.AddSingleton<MainViewModel>();
        serviceCollection.AddSingleton<TranslateViewModel>();
        serviceCollection.AddSingleton<DetailViewModel>();
        serviceCollection.AddSingleton<InitializationViewModel>();
        serviceCollection.AddSingleton<QueryWordViewModel>();
        serviceCollection.AddSingleton<QueryWordResultViewModel>();
        serviceCollection.AddSingleton<FavoriteWordViewModel>();
        serviceCollection.AddSingleton<QuizViewModel>();
        serviceCollection.AddSingleton<ChatViewModel>();
        serviceCollection.AddSingleton<WeatherViewModel>();
        serviceCollection.AddSingleton<IWeatherService, WeatherService>();
        serviceCollection.AddSingleton<MemoViewModel>();
        serviceCollection.AddSingleton<MusicPlayerViewModel>();
        
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }
    
}