using KSW.ATE01.Project.Base.Enums.Patterns;
using KSW.ATE01.Project.Base.Models.Patterns;
using System.IO;
using System.Text;

namespace KSW.ATE01.Project.Base.Helpers
{
    /// <summary>
    /// 向量读写帮助类
    /// </summary>
    public class PatternReaderWriterHelper
    {
        /// <summary>
        /// 记录向量到文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="singlePinPattern"></param>
        /// <returns></returns>
        public static List<PinPackModel> WritePattern(string path, ModuleType moduleType, Dictionary<string, List<PinPatternModel>> singlePinPattern)
        {
            var result = new List<PinPackModel>();
            if (singlePinPattern == null || !singlePinPattern.Any())
                return result;

            using (var fs = new FileStream(path, FileMode.Create))
            {
                using (var writer = new BinaryWriter(fs))
                {
                    writer.Write((byte)moduleType);

                    foreach (var pinPattern in singlePinPattern)
                    {
                        var timingSet = pinPattern.Value.Where(x => !string.IsNullOrEmpty(x.TimingSet)).Select(x => x.TimingSet).FirstOrDefault();

                        var pack = new PinPackModel()
                        {
                            PinName = pinPattern.Key,
                        };
                        pack.TimingSet = timingSet;
                        DealWithPattern(pack, pinPattern.Value);
                        result.Add(pack);
                        var nameBytes = Encoding.UTF8.GetBytes(pack.PinName);
                        pack.NameLength = nameBytes.Length;
                        byte[] timingSetBytes = null;
                        if (!string.IsNullOrEmpty(timingSet))
                        {
                            timingSetBytes = Encoding.UTF8.GetBytes(timingSet);
                            pack.TimingSetLength = timingSetBytes.Length;
                        }
                        else
                            pack.TimingSetLength = 0;
                        writer.Write(pack.NameLength);
                        writer.Write(nameBytes);
                        writer.Write(pack.TimingSetLength);
                        if (timingSetBytes != null)
                            writer.Write(timingSetBytes);
                        writer.Write(pack.Length);
                        foreach (var data in pack.Data)
                        {
                            writer.Write(data.VectorNumber);
                            writer.Write(data.Instruction);
                            writer.Write(data.Data);
                        }
                    }
                    writer.Flush();
                }
            }

            return result;
        }

        private static void DealWithPattern(PinPackModel pack, List<PinPatternModel> pinPatterns)
        {
            if (pinPatterns == null || !pinPatterns.Any())
                return;

            var lastInstruction = pinPatterns.FirstOrDefault()?.Instruction ?? CommandType.nop;
            object lastCommandParameter = pinPatterns.FirstOrDefault()?.CommandParameter;
            var startIndex = 0;
            var endIndex = 0;
            var isPack = false;
            var isFinish = true;
            var andTempVector = true;
            var maxVectorCount = 124;
            var loopVectors = new List<PinPatternModel>();
            var tempVectors = new List<PinPatternModel>();

            foreach (var pinPatternModel in pinPatterns)
            {
                if (pinPatternModel.Instruction != lastInstruction || endIndex - startIndex >= maxVectorCount)
                {
                    var count = endIndex - startIndex;

                    if (pinPatternModel.Instruction == CommandType.halt)
                    {
                        isPack = true;
                        if (count < maxVectorCount)
                        {
                            count += 1;
                            andTempVector = false;
                            tempVectors.Add(pinPatternModel);
                        }
                        else
                        {
                            andTempVector = true;
                            isFinish = false;
                        }
                    }
                    else if (pinPatternModel.Instruction == CommandType.loop)
                    {
                        loopVectors.Clear();
                        lastInstruction = pinPatternModel.Instruction;
                        lastCommandParameter = pinPatternModel.CommandParameter;
                        loopVectors.Add(pinPatternModel);
                        andTempVector = false;
                        isPack = true;
                    }
                    else if (pinPatternModel.Instruction == CommandType.endloop)
                    {
                        loopVectors.Add(pinPatternModel);
                        if (loopVectors.Any())
                        {
                            var groups = PackLoopPattern(lastCommandParameter, loopVectors);
                            if (groups.Any())
                            {
                                pack.Data.AddRange(groups);
                                pack.Length += 64 * groups.Count;
                            }
                        }
                        lastInstruction = CommandType.nop;
                        lastCommandParameter = null;
                        startIndex = ++endIndex;
                        isPack = false;
                        isFinish = true;
                    }
                    else
                    {
                        switch (lastInstruction)
                        {
                            case CommandType.loop:
                                loopVectors.Add(pinPatternModel);
                                isPack = false;
                                break;
                            default:
                                andTempVector = true;
                                isPack = true;
                                break;
                        }
                    }

                    if (isPack)
                    {
                        var patterns = tempVectors;//pinPatterns.GetRange(startIndex, count);
                        if (patterns.Any())
                        {
                            var group = PackPattern(lastInstruction, lastCommandParameter, patterns);
                            startIndex = endIndex;
                            endIndex++;
                            pack.Data.Add(group);
                            pack.Length += 64;
                        }
                        isFinish = true;
                        tempVectors.Clear();
                        if (andTempVector)
                            tempVectors.Add(pinPatternModel);
                    }
                }
                else
                {
                    endIndex++;
                    if (lastInstruction == CommandType.loop)
                        loopVectors.Add(pinPatternModel);
                    else
                        tempVectors.Add(pinPatternModel);
                    isFinish = false;
                }

            }

            if (!isFinish)
            {
                var count = pinPatterns.Count - startIndex;
                var patterns = tempVectors;//pinPatterns.GetRange(startIndex, count);
                var group = PackPattern(lastInstruction, lastCommandParameter, patterns);
                pack.Data.Add(group);
                pack.Length += 64;
            }
        }

        private static List<PatternVectorGroupModel> PackLoopPattern(object patternParameter, List<PinPatternModel> patterns)
        {
            var result = new List<PatternVectorGroupModel>();
            int loopCount = 0;
            if (patternParameter is int param)
                loopCount = param;
            else
                return result;

            var startIndex = 0;
            var x = (long)patterns.Count * (long)loopCount;
            var patternsVector = patterns.Select(x => x.VectorValue).ToList();

            if (patterns.Count >= 224)
            {
                var n = patterns.Count / 112;
                var reset = patterns.Count % 112;

                if ((n >= 2 && reset > 0) || n > 3)
                {
                    var loopPack = GeneratorLoopPack(patternsVector, loopCount, 112);
                    result.Add(loopPack);
                    startIndex += 112;

                    var normalPackCount = n - 2;
                    if (normalPackCount > 0)
                    {
                        for (int i = 0; i < normalPackCount; i++)
                        {
                            var normalPack = GeneratorNormalPack(patternsVector.Skip(startIndex), 112);
                            result.Add(normalPack);
                            startIndex += 112;
                        }
                    }
                    else if (normalPackCount == 0)
                    {
                        var normalPack = GeneratorNormalPack(patternsVector.Skip(startIndex), 112);
                        result.Add(normalPack);
                        startIndex += 112;
                    }

                    var endPack = GeneratorEndPack(patternsVector, startIndex, reset);
                    result.Add(endPack);
                }
                else
                {
                    var loopPack = GeneratorLoopPack(patternsVector, loopCount, 112);
                    result.Add(loopPack);
                    startIndex += 112;
                    var endPack = GeneratorEndPack(patternsVector, startIndex, reset);
                    result.Add(endPack);
                }
            }
            else if (x >= 372)
            {
                var k = 224 / patterns.Count;
                var n = loopCount / k;
                var loopVectors = new List<VectorValueType>();
                var totoalVectorCount = (long)patternsVector.Count * (long)loopCount;
                for (int i = 0; i < k; i++)
                    loopVectors.AddRange(patternsVector);

                var half = loopVectors.Count / 2;
                var loopPack = GeneratorLoopPack(loopVectors, n, half);
                result.Add(loopPack);
                startIndex += half;
                var endPack = GeneratorEndPack(loopVectors, startIndex, loopVectors.Count - half);
                result.Add(endPack);

                var restLoop = loopCount - n * k;
                var normalNumber = totoalVectorCount - ((long)loopVectors.Count * (long)n);
                var count = normalNumber / 124;
                var rest = (int)(normalNumber % 124);
                var restVectors = Enumerable.Repeat(patternsVector, restLoop).SelectMany(x => x);   //不实际存储的可枚举序列
                startIndex = 0;

                for (var i = 0; i < count; i++)
                {
                    var normalPack = GeneratorNormalPack(restVectors, 124);
                    result.Add(normalPack);
                    startIndex += 124;
                }

                if (rest > 0)
                {
                    var normalPack = GeneratorNormalPack(restVectors.Skip(startIndex), rest);
                    result.Add(normalPack);
                }
            }
            else
            {
                startIndex = 0;
                var isFinish = true;
                var totalVectors = Enumerable.Repeat(patternsVector, loopCount).SelectMany(x => x);
                var totoalVectorCount = (long)patternsVector.Count * (long)loopCount;

                var n = totoalVectorCount / 124;
                var rest = totoalVectorCount % 124;

                for (int i = 0; i <= n; i++)
                {
                    if (i != n)
                    {
                        var normalGroup = GeneratorNormalPack(totalVectors.Skip(startIndex), 124);
                        result.Add(normalGroup);
                        startIndex += 124;
                    }
                    else
                    {
                        var vectors = totalVectors.Skip(startIndex); //totalVectors.GetRange(i * 124, rest);
                        if (vectors.Any())
                        {
                            var end = new PatternVectorGroupModel()
                            {
                                VectorNumber = (byte)vectors.Count(),
                            };
                            startIndex = 0;
                            byte symbol = 0;
                            var index = 0;
                            foreach (var vector in vectors)
                            {
                                if (index % 2 == 0)
                                {
                                    symbol = (byte)vector;
                                    isFinish = false;
                                }
                                else
                                {
                                    symbol |= (byte)((byte)vector << 4);
                                    end.Data[startIndex++] = symbol;
                                    isFinish = true;
                                }
                                index++;
                            }

                            if (!isFinish)
                                end.Data[startIndex] = symbol;

                            result.Add(end);
                        }
                    }
                }
            }

            return result;
        }

        private static PatternVectorGroupModel GeneratorLoopPack(List<VectorValueType> patterns, int loopCount, int vectorNumber)
        {
            var intBytes = BitConverter.GetBytes(loopCount);
            var result = new PatternVectorGroupModel()
            {
                VectorNumber = (byte)vectorNumber,
                Instruction = (byte)CommandType.loop,
            };

            Array.Copy(intBytes, 0, result.Data, 0, intBytes.Length);

            byte vector = 0;
            var index = 0;
            bool isFinish = true;
            for (int i = 0; i < vectorNumber; i++)
            {
                if (i % 2 == 0)
                {
                    vector = (byte)patterns[i];
                    isFinish = false;
                }
                else
                {
                    vector |= (byte)((byte)patterns[i] << 4);
                    result.Data[6 + index++] = vector;
                    isFinish = true;
                }
            }

            if (!isFinish)
                result.Data[6 + index] = vector;

            return result;
        }

        private static PatternVectorGroupModel GeneratorNormalPack(IEnumerable<VectorValueType> patterns, int vectorNumber)
        {
            var result = new PatternVectorGroupModel()
            {
                VectorNumber = (byte)vectorNumber,
            };

            byte vector = 0;
            var index = 0;
            var i = 0;
            bool isFinish = true;

            foreach (var item in patterns)
            {
                if (i % 2 == 0)
                {
                    vector = (byte)item;
                    isFinish = false;
                }
                else
                {
                    vector |= (byte)((byte)item << 4);
                    result.Data[index++] = vector;
                    isFinish = true;
                }
                i++;
                if (i == vectorNumber)
                    break;
            }

            if (!isFinish)
                result.Data[index] = vector;

            return result;
        }

        private static PatternVectorGroupModel GeneratorEndPack(List<VectorValueType> patterns, int startIndex, int reset)
        {
            var result = new PatternVectorGroupModel()
            {
                VectorNumber = (byte)reset,
                Instruction = (byte)CommandType.endloop,
            };

            byte vector = 0;
            var index = 0;
            bool isFinish = true;
            for (int i = 0; i < reset; i++)
            {
                if (i % 2 == 0)
                {
                    vector = (byte)patterns[startIndex + i];
                    isFinish = false;
                }
                else
                {
                    vector |= (byte)((byte)patterns[startIndex + i] << 4);
                    result.Data[6 + index++] = vector;
                    isFinish = true;
                }
            }

            if (!isFinish)
                result.Data[6 + index] = vector;

            return result;
        }

        private static PatternVectorGroupModel PackPattern(CommandType instruction, object patternParamter, List<PinPatternModel> patterns)
        {
            var result = new PatternVectorGroupModel()
            {
                VectorNumber = (byte)patterns.Count,
                Instruction = (byte)instruction,
            };
            int startIndex = 0;
            if (instruction != CommandType.nop)
            {
                startIndex = 6;
                if (patternParamter is int param)
                {
                    var parameterArray = new byte[6];
                    var intParam = instruction == CommandType.loop ? param - 1 : param; //循环情况下
                    var parameterBytes = BitConverter.GetBytes(intParam).ToArray();    // 不反序
                    Array.Copy(parameterBytes, 0, result.Data, 0, parameterBytes.Length);
                }
            }

            byte vector = 0;
            var vectorList = new List<byte>();
            bool isFinish = true;
            var index = 0;
            foreach (var pinPatternModel in patterns)
            {
                if (index % 2 == 0)
                {
                    vector = (byte)pinPatternModel.VectorValue;
                    isFinish = false;
                }
                else
                {
                    vector |= (byte)((byte)pinPatternModel.VectorValue << 4);
                    vectorList.Add(vector);
                    isFinish = true;
                }
                index++;
            }

            if (!isFinish && patterns.Any())
                vectorList.Add(vector);

            if (vectorList.Count <= result.Data.Count())
                Array.Copy(vectorList.ToArray(), 0, result.Data, startIndex, vectorList.Count);

            return result;
        }

        /// <summary>
        /// 读取Pattern
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static BinPatternModel ReadPattern(string path)
        {
            var result = new BinPatternModel();
            result.PinPacks = new List<PinPackModel>();

            if (File.Exists(path))
            {
                result.FileName = Path.GetFileName(path);
                result.FilePath = path;
                result.VectorName = Path.GetFileNameWithoutExtension(path);

                using (var fs = new FileStream(path, FileMode.Open))
                {
                    using (var br = new BinaryReader(fs))
                    {
                        result.ModuleType = (ModuleType)br.ReadByte();
                        while (br.BaseStream.Position < br.BaseStream.Length)
                        {
                            var pack = new PinPackModel();
                            var nameLength = br.ReadInt32();
                            var nameBytes = br.ReadBytes(nameLength);
                            pack.PinName = Encoding.UTF8.GetString(nameBytes);
                            var timingSetLength = br.ReadInt32();
                            if (timingSetLength > 0)
                            {
                                var timingSetBytes = br.ReadBytes(timingSetLength);
                                pack.TimingSet = Encoding.UTF8.GetString(timingSetBytes);
                            }
                            var length = br.ReadInt64();
                            pack.Data = new List<PatternVectorGroupModel>();
                            while (length > 0)
                            {
                                var group = new PatternVectorGroupModel();
                                group.VectorNumber = br.ReadByte();
                                group.Instruction = br.ReadByte();
                                group.Data = br.ReadBytes(62);
                                pack.Data.Add(group);
                                length -= 64;
                            }
                            result.PinPacks.Add(pack);
                        }
                    }
                }
            }
            return result;
        }


    }
}
