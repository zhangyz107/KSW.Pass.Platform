using KSW.ATE01.Project.Base.Enums.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Patterns
{
    public struct LabelModel
    {
        private string _labelName;

        private long _indexInVectors;

        private LabelCommandType _labelType;

        private string _labelFullContent;

        public string LabelName;
        //{
        //    get => _labelName;
        //    set => _labelName = value;
        //}

        public long IndexInVectors;
        //{
        //    get => _indexInVectors;
        //    set => _indexInVectors = value;
        //}

        public LabelCommandType LabelType;
        //{
        //    get => _labelType;
        //    set => _labelType = value;
        //}

        public string LabelFullContent;
        //{
        //    get => _labelFullContent;
        //    set => _labelFullContent = value;
        //}
    }
}
