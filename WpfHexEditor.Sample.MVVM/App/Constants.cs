using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHexEditor.Sample.MVVM.App
{
    static class Constants
    {
        //语言资源字典名称;
        public const string LanguageDict = nameof(LanguageDict);

        //语言配置文件名;
        public const string LanguageConfigName = "LanguageConfig.xml";

        //语言文件夹;
        public const string LanguageDirect = "Languages";

        //当前语言;
        public const string CurrentLanguage = "CurrentLanguage";


        //语言提供器元素;
        public const string LanguageProviders = nameof(LanguageProviders);
        //语言元素名;
        public const string Provider = nameof(Provider);
        //语言名称;
        public const string ProviderName = "Name";
        //语言类型;
        public const string ProviderType = "Type";
    }
}
