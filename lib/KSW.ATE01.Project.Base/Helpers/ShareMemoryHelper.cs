using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace KSW.ATE01.Project.Base.Helpers
{
    public class ShareMemoryHelper
    {
        private static Dictionary<string, MemoryMappedFile> mappedFileDic = new Dictionary<string, MemoryMappedFile>();

        public static void CreateShareMemory(string memoryName, int size)
        {
            MemoryMappedFile memoryMappedFile = MemoryMappedFile.CreateOrOpen(memoryName, (long)size);
            memoryMappedFile.CreateViewAccessor();
            AppendMemoryMappedFileToDictionary(memoryName, memoryMappedFile);
        }

        public static (bool, string) OpenShareMemory(string memoryName)
        {
            string result;
            bool flag = false;
            try
            {
                var mappedFile = MemoryMappedFile.OpenExisting(memoryName);
                AppendMemoryMappedFileToDictionary(memoryName, mappedFile);
                result = "Successful";
                flag = true;
            }
            catch (FileNotFoundException)
            {
                result = "The Memory '" + memoryName + "' doesn't exist.";
            }
            catch (Exception ex)
            {
                result = "Exeception unkown, exception message is " + ex.Message;
            }

            return (flag, result);
        }

        public static bool IsShareMemoryExisting(string shareMemoryName)
        {
            bool result;
            try
            {
                MemoryMappedFile.OpenExisting(shareMemoryName);
                result = true;
            }
            catch (FileNotFoundException)
            {
                result = false;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public unsafe static void ReadShareMemory(string memoryName, int offset, byte[] arr, int num)
        {
            using (MemoryMappedViewAccessor memoryMappedViewAccessor = mappedFileDic[memoryName].CreateViewAccessor())
            {
                byte* value = null;
                memoryMappedViewAccessor.SafeMemoryMappedViewHandle.AcquirePointer(ref value);
                Marshal.Copy(IntPtr.Add(new IntPtr((void*)value), offset), arr, 0, num);
                memoryMappedViewAccessor.SafeMemoryMappedViewHandle.ReleasePointer();
            }
        }

        public static void ReadShareMemory(string memoryName, int offset, out int intValue)
        {
            using (MemoryMappedViewAccessor memoryMappedViewAccessor = mappedFileDic[memoryName].CreateViewAccessor())
            {
                intValue = memoryMappedViewAccessor.ReadInt32((long)offset);
            }
        }

        public static void WriteShareMemory(string memoryName, int offset, byte[] arr, int num)
        {
            using (MemoryMappedViewAccessor memoryMappedViewAccessor = mappedFileDic[memoryName].CreateViewAccessor())
            {
                memoryMappedViewAccessor.WriteArray<byte>((long)offset, arr, 0, num);
            }
        }

        public static void WriteShareMemory(string memoryName, int offset, int intValue)
        {
            using (MemoryMappedViewAccessor memoryMappedViewAccessor = mappedFileDic[memoryName].CreateViewAccessor())
            {
                memoryMappedViewAccessor.Write((long)offset, intValue);
                memoryMappedViewAccessor.ReadInt32((long)offset);
            }
        }

        private static void AppendMemoryMappedFileToDictionary(string memoryName, MemoryMappedFile memoryMappedFile)
        {
            if (mappedFileDic.ContainsKey(memoryName))
            {
                mappedFileDic[memoryName] = memoryMappedFile;
                return;
            }
            mappedFileDic.Add(memoryName, memoryMappedFile);
        }
    }
}
