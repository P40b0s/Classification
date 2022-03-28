using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification.Modules.DocumentTextViewerModule.Models
{
    public sealed class WordPositionStruct
    {
        public int IndexOfWord { get; set; }
        public int GetLenghtOfWord
        {
            get { return Word.Length; }
        }
        public int rootLenght { get; set; }
        public string Word { get; set; }

        public bool FirstCharIsUpperCase
        {
            get
            {
                if (!string.IsNullOrEmpty(Word) && Char.IsUpper(Word[0]))
                    return true;
                else
                    return false;
            }
        }

        public override bool Equals(object obj)
        {
            var item = obj as WordPositionStruct;
            if (item == null)
            {
                return false;
            }
            return (this.IndexOfWord.Equals(item.IndexOfWord) &&
                    this.GetLenghtOfWord.Equals(item.GetLenghtOfWord) &&
                    this.Word.Equals(item.Word));
        }
        public override int GetHashCode()
        {
            return ((this.IndexOfWord + this.GetLenghtOfWord).ToString() + this.Word).GetHashCode();
        }

    }
}
