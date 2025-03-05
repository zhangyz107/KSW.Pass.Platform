using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models.Memory;
using System.Text;
using System.Text.Json;
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
                //MessageBox.Show("");
                return;
            }
            var memoryDataBlock = memoryExsitDataBlock.FirstOrDefault();
            data = memoryDataBlock?.BlockValue;
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

        private void WriteToMemory(MemoryHeadInfo headInfo)
        {
            byte[] serializedInstance = this.GetSerializedInstance(headInfo);
            ShareMemoryHelper.WriteShareMemory(_memoryName, 4, serializedInstance, serializedInstance.Length);
            byte[] bytes = BitConverter.GetBytes(serializedInstance.Length);
            ShareMemoryHelper.WriteShareMemory(_memoryName, 0, bytes, bytes.Length);
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
            MemoryDataBlock memoryDataBlock = memoryExistDataBlock.FirstOrDefault();
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
            this.WriteToMemory(this.HeadInfo);
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
            return JsonSerializer.SerializeToUtf8Bytes(objectInstance);
        }

        private T GetDeserialized<T>(byte[] array)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(array);
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
