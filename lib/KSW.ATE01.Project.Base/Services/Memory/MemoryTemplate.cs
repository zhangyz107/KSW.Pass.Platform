using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models.Memory;
using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KSW.ATE01.Project.Base.Services.Memory
{
    /// <summary>
    /// 内存模板
    /// </summary>
    internal class MemoryTemplate
    {
        private MemoryHeadInfo _headInfo;
        private string _memoryName;
        private const int memory_data_block_size = 8 + 8 + 256 + 4;

        public MemoryHeadInfo HeadInfo
        {
            get
            {
                return _headInfo;
            }
            set
            {
                if (value == null)
                {
                    _headInfo = new MemoryHeadInfo();
                    _headInfo.MemoryName = _memoryName;
                    _headInfo.CurrentDataAddress = 32768L;
                    return;
                }
                _headInfo = value;
            }
        }


        public string MemoryName
        {
            get => _memoryName;
            set => _memoryName = value;
        }

        public bool CreateMemory(string memoryName, int memorySize)
        {
            ShareMemoryHelper.CreateShareMemory(memoryName, memorySize);
            return ShareMemoryHelper.OpenShareMemory(memoryName).Item1;
        }

        public bool OpenMemory(string memoryName)
        {
            _memoryName = memoryName;
            if (!ShareMemoryHelper.OpenShareMemory(memoryName).Item1)
            {
                return false;
            }
            HeadInfo = GetMemoryHeadFromMemory();
            return true;
        }

        public void GetDataFromMemory(string dataBlockName, out byte[] data)
        {
            HeadInfo = GetMemoryHeadFromMemory();
            data = new byte[0];
            var memoryExsitDataBlock = GetMemoryExistDataBlock(dataBlockName);
            if (memoryExsitDataBlock.Count == 0)
            {
                MessageBox.Show("");
                return;
            }
            var memoryDataBlock = memoryExsitDataBlock.FirstOrDefault();
            var array = new byte[memoryDataBlock.BlockLength];
            ShareMemoryHelper.ReadShareMemory(_memoryName, (int)memoryDataBlock.StartAddress, array, array.Length);
            data = new byte[memoryDataBlock.VaildValueLength];
            Array.Copy(array, memory_data_block_size, data, 0, data.Length);
        }

        private List<MemoryDataBlock> GetMemoryExistDataBlock(string dataBlockName)
        {
            return (from x in _headInfo.Blocks
                    where x.BlockName == dataBlockName
                    select x).ToList<MemoryDataBlock>();
        }

        public void AppendOrUpdateDataToMemory(string dataBlockName, byte[] data)
        {
            HeadInfo = GetMemoryHeadFromMemory();
            if (IsMemoryExistDataBlock(dataBlockName))
            {
                UpdateDataInDataBlock(dataBlockName, data);
                return;
            }
            AppendDataToMemory(dataBlockName, data);
        }

        public void AppendOrUpdateDataToMemory(string dataBlockName, byte[] dataValue, int spaceSizeByte = -1)
        {
            HeadInfo = GetMemoryHeadFromMemory();
            if (IsMemoryExistDataBlock(dataBlockName))
            {
                this.UpdateDataInDataBlock(dataBlockName, dataValue);
                return;
            }
            AppendDataToMemory(dataBlockName, dataValue, spaceSizeByte);
        }

        private void EmptyMemoryOfDataBlock(MemoryDataBlock current)
        {
            byte[] array = new byte[current.BlockLength];
            ShareMemoryHelper.WriteShareMemory(_memoryName, (int)current.StartAddress, array, array.Length);
        }

        private bool IsMemoryExistDataBlock(string dataBlockName)
        {
            return GetMemoryExistDataBlock(dataBlockName).Count != 0;
        }

        private MemoryHeadInfo GetMemoryHeadFromMemory()
        {
            byte[] array = new byte[4];
            ShareMemoryHelper.ReadShareMemory(_memoryName, 0, array, array.Length);
            int length = BitConverter.ToInt32(array, 0);
            if (length == 0)
                return null;
            byte[] array2 = new byte[length];
            ShareMemoryHelper.ReadShareMemory(_memoryName, 4, array2, array2.Length);
            return GetDeserialized<MemoryHeadInfo>(array2);
        }

        private void WriteToMemory(MemoryDataBlock memoryDataBlock)
        {
            byte[] array = this.FormatDataBlockToBytes(memoryDataBlock);
            ShareMemoryHelper.WriteShareMemory(_memoryName, (int)memoryDataBlock.StartAddress, array, array.Length);
        }

        private void WriteToMemory(MemoryHeadInfo headInfo)
        {
            byte[] serializedInstance = this.GetSerializedInstance(headInfo);
            ShareMemoryHelper.WriteShareMemory(_memoryName, 4, serializedInstance, serializedInstance.Length);
            byte[] bytes = BitConverter.GetBytes(serializedInstance.Length);
            ShareMemoryHelper.WriteShareMemory(_memoryName, 0, bytes, bytes.Length);
        }

        private byte[] FormatDataBlockToBytes(MemoryDataBlock memoryDataBlock)
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(memoryDataBlock.BlockLength));
            list.AddRange(BitConverter.GetBytes(memoryDataBlock.StartAddress));
            byte[] bytes = Encoding.Default.GetBytes(memoryDataBlock.BlockName);
            Array.Resize<byte>(ref bytes, 256);
            list.AddRange(bytes);
            list.AddRange(BitConverter.GetBytes(memoryDataBlock.VaildValueLength));
            list.AddRange(memoryDataBlock.BlockValue);
            return list.ToArray();
        }

        public void AppendDataToMemory(string dataBlockName, byte[] dataValue, int spaceSizeByte = -1)
        {
            spaceSizeByte = ((spaceSizeByte == -1) ? dataValue.Length : spaceSizeByte);
            HeadInfo = GetMemoryHeadFromMemory();
            if (IsMemoryExistDataBlock(dataBlockName))
            {
                MessageBox.Show("Block name '" + dataBlockName + "' already exists in memory when append data to memory.", "Warn", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            MemoryDataBlock memoryDataBlock = CreateMemoryDataBlockInstance(dataBlockName, dataValue, spaceSizeByte);
            this.HeadInfo.Blocks.Add(memoryDataBlock);
            this.WriteToMemory(memoryDataBlock);
            this.WriteToMemory(this.HeadInfo);
        }

        public void AppendDataToMemory(string dataBlockName, byte[] dataValue)
        {
            HeadInfo = GetMemoryHeadFromMemory();
            if (IsMemoryExistDataBlock(dataBlockName))
            {
                MessageBox.Show("Block name '" + dataBlockName + "' already exists in memory when append data to memory.", "Warn", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            MemoryDataBlock memoryDataBlock = CreateMemoryDataBlockInstance(dataBlockName, dataValue);
            HeadInfo.Blocks.Add(memoryDataBlock);
            WriteToMemory(memoryDataBlock);
            WriteToMemory(HeadInfo);
        }

        public void UpdateDataInDataBlock(string dataBlockName, byte[] dataValue)
        {
            this.HeadInfo = this.GetMemoryHeadFromMemory();
            List<MemoryDataBlock> memoryExistDataBlock = this.GetMemoryExistDataBlock(dataBlockName);
            if (memoryExistDataBlock.Count == 0)
            {
                MessageBox.Show("Block name '" + dataBlockName + "' does not exists in memory when update data of datablock.", "Warn", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            MemoryDataBlock memoryDataBlock = memoryExistDataBlock[0];
            if (memoryDataBlock.VaildValueLength < dataValue.Length)
            {
                this.EmptyMemoryOfDataBlock(memoryDataBlock);
                _headInfo.Blocks.Remove(memoryDataBlock);
                this.WriteToMemory(_headInfo);
                this.AppendDataToMemory(dataBlockName, dataValue);
                return;
            }
            memoryDataBlock.BlockValue = dataValue;
            memoryDataBlock.VaildValueLength = dataValue.Length;
            memoryDataBlock.BlockLength = (long)(dataValue.Length + memory_data_block_size);
            this.WriteToMemory(memoryDataBlock);
            this.WriteToMemory(this.HeadInfo);
        }

        public void EmptyDataInDataBlock(string dataBlockName)
        {
            this.HeadInfo = this.GetMemoryHeadFromMemory();
            List<MemoryDataBlock> memoryExistDataBlock = this.GetMemoryExistDataBlock(dataBlockName);
            if (memoryExistDataBlock.Count == 0)
            {
                MessageBox.Show("Block name '" + dataBlockName + "' does not exists in memory when delete data from memory.", "Warn", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            MemoryDataBlock memoryDataBlock = memoryExistDataBlock[0];
            byte[] blockValue = new byte[memoryDataBlock.VaildValueLength];
            memoryDataBlock.BlockValue = blockValue;
            this.WriteToMemory(memoryDataBlock);
        }

        public void EmptyAllMemory()
        {
            this.HeadInfo = this.GetMemoryHeadFromMemory();
            for (int i = 0; i < _headInfo.Blocks.Count; i++)
            {
                MemoryDataBlock current = this.HeadInfo.Blocks[i];
                this.EmptyMemoryOfDataBlock(current);
            }
            this.HeadInfo.CurrentDataAddress = 32768L;
            this.HeadInfo.Blocks.Clear();
            this.WriteToMemory(this.HeadInfo);
        }

        public byte[] GetSerializedInstance(object objectInstance)
        {
            return MessagePackSerializer.Serialize(objectInstance);
        }

        private T GetDeserialized<T>(byte[] array)
        {
            try
            {
                return MessagePackSerializer.Deserialize<T>(array);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when deserialized bytes to object. \r\nMessage : " + ex.Message + " \r\nStackTrace : " + ex.StackTrace);
            }
            return default(T);
        }

        private MemoryDataBlock CreateMemoryDataBlockInstance(string dataBlockName, byte[] dataValue)
        {
            MemoryDataBlock memoryDataBlock = new MemoryDataBlock
            {
                BlockName = dataBlockName,
                StartAddress = _headInfo.CurrentDataAddress,
                BlockValue = dataValue,
                VaildValueLength = dataValue.Length,
                BlockLength = (long)(dataValue.Length + memory_data_block_size)
            };
            _headInfo.CurrentDataAddress += memoryDataBlock.BlockLength;
            return memoryDataBlock;
        }

        private MemoryDataBlock CreateMemoryDataBlockInstance(string dataBlockName, byte[] dataValue, int spaceSizeByte)
        {
            MemoryDataBlock memoryDataBlock = new MemoryDataBlock
            {
                BlockName = dataBlockName,
                StartAddress = _headInfo.CurrentDataAddress,
                BlockValue = dataValue,
                BlockValueLength = dataValue.Length,
                BlockHoldValueSize = spaceSizeByte,
                BlockLength = (long)(spaceSizeByte + memory_data_block_size)
            };
            _headInfo.CurrentDataAddress += memoryDataBlock.BlockLength;
            return memoryDataBlock;
        }
    }
}
