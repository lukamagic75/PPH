using CommunityToolkit.Mvvm.Input;
using PPH.Library.Services;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PPH.Library.ViewModels
{
    public class TranslateViewModel : ViewModelBase
    {
        private readonly ITranslateService _translateService;

        public TranslateViewModel(ITranslateService translateService)
        {
            _translateService = translateService;
            TranslateCommand = new AsyncRelayCommand(TranslateAsync);
        }

        private string _sourceText = string.Empty;
        public string SourceText
        {
            get => _sourceText;
            set => SetProperty(ref _sourceText, value);
        }

        private string _targetText = string.Empty;
        public string TargetText
        {
            get => _targetText;
            set => SetProperty(ref _targetText, value);
        }

        private TargetLanguageType _languageType = TargetLanguageType.ToEnglishType;
        public TargetLanguageType LanguageType
        {
            get => _languageType;
            set => SetProperty(ref _languageType, value);
        }

        public ICommand TranslateCommand { get; }

        public async Task TranslateAsync()
        {
            if (string.IsNullOrWhiteSpace(SourceText))
            {
                TargetText = "请输入文本进行翻译";
                return;
            }

            string text = SourceText.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");

            // 调用翻译服务
            TargetText = await _translateService.Translate(text, "auto", LanguageType.ToLanguage);
        }
    }

    public class TargetLanguageType
    {
        public static readonly TargetLanguageType ToChineseType = new("中文", "zh");
        public static readonly TargetLanguageType ToEnglishType = new("英文", "en");
        public static readonly TargetLanguageType ToFrenchType = new("法语", "fr");
        public static readonly TargetLanguageType ToRussianType = new("俄语", "ru");
        public static readonly TargetLanguageType ToSpanishType = new("西班牙语", "es");
        public static readonly TargetLanguageType ToArabicType = new("阿拉伯语", "ar");
        public static readonly TargetLanguageType ToJapaneseType = new("日语", "ja");

        public static List<TargetLanguageType> TargetLanguageTypes { get; } =
            new List<TargetLanguageType>
            {
                ToChineseType, ToEnglishType, ToFrenchType, ToRussianType,
                ToSpanishType, ToArabicType, ToJapaneseType
            };

        private TargetLanguageType(string name, string toLanguage)
        {
            Name = name;
            ToLanguage = toLanguage;
        }

        public string Name { get; }
        public string ToLanguage { get; }
    }
}
