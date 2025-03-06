using KSW.ATE01.Project.Base.Helpers;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text.Json;
using System.Windows;

namespace KSW.ATE01.Project.Base.Services.Memory
{
    public class ShareMemoryObjectBase<T>
    {
        private int objectLength;
        private int objectLengthLength = 4;
        private int objectOffsetLength = 4;

        /// <summary>
        /// 共享存储名
        /// </summary>
        public virtual string ShareMemoryName { get; }

        /// <summary>
        /// 共享存储大小
        /// </summary>
        public virtual int ShareMemorySize { get; }

        public void Create()
        {
            ShareMemoryHelper.CreateShareMemory(ShareMemoryName, ShareMemorySize);
        }

        public void Reset()
        {
            if (objectLength == 0)
                return;

            ShareMemoryHelper.WriteShareMemory(ShareMemoryName, 0, new byte[4], 4);
        }

        public bool IsExisting()
        {
            bool result = false;

            try
            {
                MemoryMappedFile.OpenExisting(ShareMemoryName);
                result = true;
            }
            catch (FileNotFoundException)
            {
                result = false;
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        public (bool, string) Open()
        {
            return ShareMemoryHelper.OpenShareMemory(ShareMemoryName);
        }

        public void WriteObject(T obj)
        {
            var serializedInstance = GetSerializedInstance(obj);
            int intValue = serializedInstance.Length;
            if (serializedInstance.Length > ShareMemorySize - objectLengthLength)
            {

            }
            ShareMemoryHelper.WriteShareMemory(ShareMemoryName, 0, intValue);
            ShareMemoryHelper.WriteShareMemory(ShareMemoryName, objectOffsetLength, serializedInstance, intValue);
            this.objectLength = intValue;
        }

        public T ReadObject()
        {
            int num;
            ShareMemoryHelper.ReadShareMemory(ShareMemoryName, 0, out num);
            if (num == 0)
            {
                return default(T);
            }
            if (num > ShareMemorySize - objectLengthLength)
            {

            }
            byte[] array = new byte[num];
            ShareMemoryHelper.ReadShareMemory(ShareMemoryName, objectOffsetLength, array, array.Length);
            return GetDeserialized<T>(array);
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
    }
}
