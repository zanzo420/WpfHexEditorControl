using System;
using System.Collections.Generic;
using System.Text;
using WpfHexEditor.Sample.MVVM.Contracts.Common;

namespace WpfHexEditor.Sample.MVVM.Contracts.App
{
    //语言提供者;
    public class LanguageProvider {
        public LanguageProvider(string languageName, string lanType) {
            this.LanguageName = languageName;
            Type = lanType;
        }
        /// <summary>
        /// 语言名称(比如简体中文);
        /// </summary>
        public string LanguageName { get; }
        /// <summary>
        /// 类型;比如(zh_CN,en_US)
        /// </summary>
        public string Type { get; }
    }

    /// <summary>
    /// 语言服务契约;
    /// </summary>
    public interface ILanguageService {
        //找寻资源字符串;
        string FindResourceString(string keyName);

        /// <summary>
        /// 当前语言类型;
        /// </summary>
        LanguageProvider CurrentProvider { get; set; }

        /// <summary>
        /// 所有语言;
        /// </summary>
        IEnumerable<LanguageProvider> AllProviders { get; }

        /// <summary>
        /// 初始化;
        /// </summary>
        void Initialize();
    }

    /// <summary>
    /// 由于测试项目中无Application.Current对象,故单独抽象出一个接口提供被操作的语言资源字典;
    ///  被操作的语言相关资源字典对象;
    /// </summary>
    public interface ILanguageDict {
        string this[string keyName] { get; }
        /// <summary>
        /// 清除所有合并后字典;
        /// </summary>
        void ClearMergedDictionaries();
        /// <summary>
        /// 从指定的绝对路径读取资源字典,并合并;
        /// </summary>
        /// <param name="path"></param>
        void AddMergedDictionaryFromPath(string path);
    }
    public class LanguageService : GenericServiceStaticInstance<ILanguageService> {
        public static string FindResourceString(string keyName) => Current?.FindResourceString(keyName);
    }
}
