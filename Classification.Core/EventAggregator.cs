using Classification.Core.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification.Core
{
    /// <summary>
    /// Добавление ключевого слова в выбранный итем
    /// </summary>
    public class AddingKeyEvent : PubSubEvent<Guid> { };
    /// <summary>
    /// Добавление рубрики вы выбраный итем
    /// </summary>
    public class AddingRubricEvent : PubSubEvent<Guid> { };
    /// <summary>
    /// Загрузка всех документов закончена
    /// </summary>
    public class AllActsIsLoadedEvent : PubSubEvent { };
    /// <summary>
    /// ВЫбор документа в списке
    /// </summary>
    public class ActSelectionChangedEvent : PubSubEvent<ClassificationModel> { };
    /// <summary>
    /// Получение текста документа из базы ИПС 3.0
    /// </summary>
    public class GetTextFromIPS30ServerEvent : PubSubEvent<string> { };
    /// <summary>
    /// Событие окончания загрузки ключевых слов (для постоения списка живового поиска)
    /// </summary>
    public class KeywordsIsLoaded : PubSubEvent<IEnumerable<TextInlineSelection>> { };
    public class RubricsIsLoaded : PubSubEvent<IEnumerable<TextInlineSelection>> { };
    public class AddSynonymToBaseEvent : PubSubEvent<KeyValuePair<string, Guid>> { };

}
