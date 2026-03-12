
/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：PatternBLL.cs
// 功能描述：向量业务逻辑层
//
// 作者：zhangyingzhong
// 日期：2026/02/04 15:58
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.Application;
using KSW.ATE01.Application.BLLs.Abstractions.Patterns;
using KSW.ATE01.Application.Models.Patterns;
using KSW.ATE01.Project.Base.Enums.Patterns;
using KSW.ATE01.Project.Base.Helpers;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommandType = KSW.ATE01.Project.Base.Enums.Patterns.CommandType;

namespace KSW.ATE01.Application.BLLs.Implements.Patterns
{
    /// <summary>
    /// 向量业务逻辑层
    /// </summary>
    public class PatternBLL : ServiceBase, IPatternBLL
    {
        public PatternBLL(IContainerProvider containerProvider) : base(containerProvider)
        {

        }

        public async Task<List<PatternModel>> GetPatternsByFilesAsync(string path, bool isDir = false)
        {
            var result = new List<PatternModel>();
            try
            {
                if (isDir)
                {
                    if (Directory.Exists(path))
                    {
                        var files = Directory.GetFiles(path, "*.atp");

                        if (files.IsEmpty())
                            return result;

                        await Task.Factory.StartNew(() =>
                        {
                            foreach (var file in files)
                            {
                                var fileName = Path.GetFileNameWithoutExtension(file);
                                var source = PatternHelper.AnalysisPattern(file);
                                var pattern = ConverterPattern(source);
                                pattern.FileName = fileName;
                                pattern.FilePath = file;
                                result.Add(pattern);
                            }
                        });
                    }
                }
                else
                {
                    await Task.Factory.StartNew(() =>
                    {
                        if (File.Exists(path))
                        {
                            var fileName = Path.GetFileNameWithoutExtension(path);
                            var source = PatternHelper.AnalysisPattern(path);
                            var pattern = ConverterPattern(source);
                            pattern.FileName = fileName;
                            pattern.FilePath = path;
                            result.Add(pattern);
                        }
                    });
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private PatternModel ConverterPattern(Project.Base.Models.Patterns.PatternModel source)
        {
            var result = new PatternModel();
            if (source == null)
                return result;

            result.VectorName = source.VectorName;
            result.TimingSets = source.TimingSets;
            result.InstrumentName = source.InstrumentName;
            result.ModuleType = source.ModuleType;
            if (!source.PatternVectors.IsEmpty())
            {
                foreach (var vector in source.PatternVectors)
                {
                    var tempVector = new PatternVectorModel();
                    tempVector.Label = ConverterLabel(vector.Label);
                    tempVector.Command = ConverterCommand(vector.Command);
                    tempVector.InstrumentName = vector.InstrumentName;
                    tempVector.TimingSet = vector.TimingSet;
                    ConvertPins(vector.Pins, tempVector.Pins);
                    tempVector.Comment = vector.Comment;

                    result.PatternVectors.Add(tempVector);
                }
            }
            return result;
        }

        private LabelModel ConverterLabel(Project.Base.Models.Patterns.LabelModel label)
        {
            var labelModel = new LabelModel();
            labelModel.LabelName = label.LabelName;
            labelModel.IndexInVectors = label.IndexInVectors;
            labelModel.LabelType = label.LabelType;
            labelModel.LabelFullContent = label.LabelFullContent;

            return labelModel;
        }

        private CommandModel ConverterCommand(Project.Base.Models.Patterns.CommandModel command)
        {
            var commandModel = new CommandModel();
            commandModel.Type = command.Type;
            commandModel.CommandParameter = command.CommandParameter;
            commandModel.CommandFullContent = command.CommandParameter == null ? command.CommandFullContent : $"{command.CommandFullContent} {command.CommandParameter}";

            return commandModel;
        }

        private void ConvertPins(List<Project.Base.Models.Patterns.PinModel> src, List<PinModel> tar)
        {
            tar.Clear();

            foreach (var pin in src)
            {
                var pinModel = new PinModel();
                pinModel.PinName = pin.PinName;
                pinModel.VectorValue = pin.VectorValue;

                tar.Add(pinModel);
            }
        }

        public async Task<bool> SavePattern(PatternModel pattern, string savePath)
        {
            var result = false;
            if (pattern == null)
                return result;

            try
            {
                switch (pattern.ModuleType)
                {
                    case ModuleType.VM_Vector:
                    case ModuleType.LVM_Vector:
                        await ExportLvmPattern(pattern, savePath);
                        break;
                    case ModuleType.SRM_Vector:
                        //ExportSrmPattern(pattern, savePath);
                        break;
                    default:
                        break;
                }
                result = true;
            }
            catch (Exception)
            {
                throw;
            }
            return result;

        }

        private async Task ExportLvmPattern(PatternModel patternModel, string exportFilePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(exportFilePath);
            var pattern = patternModel.PatternVectors.FirstOrDefault();
            using (var fs = new FileStream(exportFilePath, FileMode.Create, FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs))
                {
                    //time set行
                    var timeSet = $"import tset {string.Join(",", patternModel.TimingSets)};";
                    sw.WriteLine(timeSet);

                    if (!patternModel.InstrumentName.IsEmpty())
                    {
                        //digital
                        var digital = $"digital_ins={patternModel.InstrumentName};";
                        sw.WriteLine(digital);
                    }

                    //if (!patternModel.InstrumentInfo.StrInstrument.IsEmpty())
                    //{
                    //    //instrument
                    //    sw.WriteLine("instruments = {");
                    //    sw.WriteLine($"{patternModel.InstrumentInfo.StrInstrument};");
                    //    sw.WriteLine("}");
                    //}

                    //moduleType
                    sw.WriteLine(patternModel.ModuleType.Description());

                    //pattern header
                    var patternHeader = GetPatternHeader(pattern);
                    sw.WriteLine($"{fileName}\t($tset,\t {patternHeader})");

                    //pattern vector
                    sw.WriteLine("{");
                    var colHeaderLength = $"{fileName}\t\t\t".Length;
                    foreach (var vector in patternModel.PatternVectors)
                    {
                        var timingSetAndVector = GetTimingSetAndVector(vector, colHeaderLength);
                        sw.WriteLine(timingSetAndVector);
                    }
                    sw.WriteLine("}");

                    sw.Flush();
                }
            }
        }

        private string GetPatternHeader(PatternVectorModel pattern)
        {
            var patternHeader = string.Empty;
            if (pattern == null || pattern.Pins.IsEmpty())
                return patternHeader;

            int index = 0;
            foreach (var pin in pattern.Pins)
            {
                patternHeader += pin.PinName;

                if (pattern.Pins.IndexOf(pin) != pattern.Pins.Count - 1)
                    patternHeader += ",";

                if (index == 7)
                {
                    patternHeader += "\t";
                    index = 0;
                }
                else
                    index++;
            }

            return patternHeader;
        }

        private string GetTimingSetAndVector(PatternVectorModel vector, int colHeaderLength)
        {
            var result = string.Empty;
            if (!vector.Label.LabelFullContent.IsEmpty())
                result += vector.Label.LabelFullContent;

            if (!vector.Command.CommandFullContent.IsEmpty())
            {
                if (result.IsEmpty())
                    result += vector.Command.CommandFullContent;
                else
                    result += $",{vector.Command.CommandFullContent}";
            }

            // 固定字符串宽度
            result = result.PadRight(colHeaderLength);

            // TimingSet填写
            result += $"> {vector.TimingSet}\t";

            int index = 0;
            foreach (var pin in vector.Pins)
            {
                result += $"{pin.VectorValue.Description()} ";
                if (vector.Pins.IndexOf(pin) == vector.Pins.Count - 1)
                    result += ";";

                if (index == 7)
                {
                    result += "\t";
                    index = 0;
                }
                else
                    index++;
            }

            return result;
        }

        public async Task<bool> CompileAsync(PatternModel pattern, string filePath)
        {
            var result = false;

            if (pattern == null)
                return result;

            try
            {
                var singlePinPatternDic = GetSinglePinPatternList(pattern);
                PatternReaderWriterHelper.WritePattern(filePath, pattern.ModuleType, singlePinPatternDic);
                result = true;
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }

        private Dictionary<string, List<Project.Base.Models.Patterns.PinPatternModel>> GetSinglePinPatternList(PatternModel pattern)
        {
            var result = new Dictionary<string, List<Project.Base.Models.Patterns.PinPatternModel>>();
            if (pattern.PatternVectors == null || !pattern.PatternVectors.Any())
                return result;

            var lastInstruction = CommandType.nop;
            object lastCommandParameter = null;

            foreach (var vectorModel in pattern.PatternVectors)
            {
                if (vectorModel.Command != null)
                {
                    switch (vectorModel.Command.Type)
                    {
                        case CommandType.loop:
                            lastInstruction = vectorModel.Command.Type;
                            lastCommandParameter = vectorModel.Command.CommandParameter;
                            break;
                        case CommandType.endloop:
                            lastInstruction = CommandType.nop;
                            lastCommandParameter = null;
                            break;
                        default:
                            break;
                    }
                }

                foreach (var pinModel in vectorModel.Pins)
                {
                    List<Project.Base.Models.Patterns.PinPatternModel> pinPatternModelList;
                    if (!result.ContainsKey(pinModel.PinName))
                    {
                        pinPatternModelList = new List<Project.Base.Models.Patterns.PinPatternModel>();
                        result.Add(pinModel.PinName, pinPatternModelList);
                    }
                    else
                        pinPatternModelList = result[pinModel.PinName];

                    pinPatternModelList.Add(new Project.Base.Models.Patterns.PinPatternModel
                    {
                        PinName = pinModel.PinName,
                        Instruction = vectorModel.Command.Type == CommandType.nop ? lastInstruction : vectorModel.Command.Type,
                        CommandParameter = vectorModel.Command.Type == CommandType.nop ? lastCommandParameter : vectorModel.Command.CommandParameter,
                        TimingSet = vectorModel.TimingSet,
                        VectorValue = pinModel.VectorValue,
                    });
                }
            }

            return result;
        }
    }
}
