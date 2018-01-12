using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace lingvo.morphology
{
	/// <summary>
    /// информация для инициализации морфо-модели
	/// </summary>
	public struct MorphoModelConfig
	{
        /// <summary>
        /// type of impl tree-dictionary
        /// </summary>
        public enum TreeDictionaryTypeEnum
        {
            Native,
            Classic,
            FastMemPlenty,
        }

        /// <summary>
        /// type of permanently use dictionary
        /// </summary>
        public enum PermanentlyUseTypeEnum
        {
            PermanentStayInMemory,
            CanBeUnloadFromMemory,
        }

        /// <summary>
        /// type of impl tree-dictionary
        /// </summary>
        public TreeDictionaryTypeEnum TreeDictionaryType
        {
            get;
            set;
        }

        /// <summary>
        /// type of impl tree-dictionary
        /// </summary>
        public PermanentlyUseTypeEnum PermanentlyUseType
        {
            get;
            set;
        }
        public bool IsPermanentStayInMemoryUseType 
        {
            get { return (PermanentlyUseType == PermanentlyUseTypeEnum.PermanentStayInMemory); } 
        }

		/// путь к модели
        public string BaseDirectory
        {
            get;
            set;
        }
		/// список имен словарей
		/// первый файл - морфотипы
        public ICollection< string > MorphoTypesFilenames
        {
            get;
            set;
        }
		/// второй - имена собственные
        public ICollection< string > ProperNamesFilenames
        {
            get;
            set;
        }
		/// остальные - нарицательные        
		public ICollection< string > CommonFilenames
        {
            get;
            set;
        }

        public Action< string, string > ModelLoadingErrorCallback
        {
            get;
            set;
        }
	}
}