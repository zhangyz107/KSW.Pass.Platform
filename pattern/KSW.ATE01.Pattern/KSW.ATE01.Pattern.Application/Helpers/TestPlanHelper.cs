using KSW.ATE01.Pattern.Application.BLLs.Abstractions.Patterns;
using KSW.ATE01.Pattern.Application.BLLs.Implements.Patterns;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KSW.ATE01.Pattern.Application.Helpers.TestPlanHelper;

namespace KSW.ATE01.Pattern.Application.Helpers
{
    public class TestPlanHelper
    {
        public struct Struct_ChildSite
        {
            public string SiteName;

            public string SiteValue;
        }

        public struct Struct_Channel
        {
            public string Pin_Name;

            public string Type;

            public Struct_ChildSite[] SiteInfo;
        }

        public struct Struct_ParentChannel
        {
            public string ChannelName;

            public int SiteCount;

            public string SiteNameJoin;

            public Struct_Channel[] struct_channel;

            public Struct_Channel[] struct_rawChannel;
        }

        private static int _retCode = 10000000;

        private static ITestPlanDataSource _testPlanDataSource = null;

        public static Dictionary<string, Struct_ParentChannel> ChannelDic = new Dictionary<string, Struct_ParentChannel>();

        public static List<string> ReadChannelExcelToDataTable(string filePath, string channelName)
        {
            ChannelDic = new Dictionary<string, Struct_ParentChannel>();
            DataTable dataTable = new DataTable();
            List<string> list = new List<string>();
            new List<string>();
            try
            {
                list = ConnectionExcel(filePath);
                if (list.Count >= 2)
                {
                    list.Clear();
                    _retCode = 10000028;
                    list.Add(_retCode.ToString());
                    list.Add("Read Channel table, error open file");
                    dataTable.Dispose();
                    return list;
                }
                list.Clear();
            }
            catch (Exception ex)
            {
                list.Clear();
                _retCode = 11000028;
                list.Add(_retCode.ToString());
                list.Add(ex.Message);
                return list;
            }
            return ReadChannelExcelToDataTable2(channelName);
        }

        public static List<string> ReadChannelExcelToDataTable2(string channelName)
        {
            int result = 0;
            ChannelDic = new Dictionary<string, Struct_ParentChannel>();
            DataTable inputDataTable = new DataTable();
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            DicSignalNumberChannelNumber = new Dictionary<string, string>();
            try
            {
                try
                {
                    inputDataTable = GetSpecialDataTable(ChannelName);
                    RemoveEmptyColumnsRows.RemoveAllColumnsAfterEmptyColumns(ref inputDataTable, 2);
                    RemoveEmptyColumnsRows.RemoveAllRowsAfterEmptyRow(ref inputDataTable);
                }
                catch (Exception ex)
                {
                    _retCode = 10000100;
                    list.Add(_retCode.ToString());
                    list.Add("Wrong : An exception occurred when reading the Channel sheet " + ChannelName + ", exception message is " + ex.Message);
                    return list;
                }
                inputDataTable.TableName = ChannelName;
                if (!ChannelMapSubDataTable.GetSubChannelMap(inputDataTable, out ChannelMapSubDataTable.SubChannelMap, out var logMessage))
                {
                    return new List<string> { "193004", logMessage };
                }
                int count = inputDataTable.Columns.Count;
                foreach (DataRow row in inputDataTable.Rows)
                {
                    string text = "";
                    string text2 = "";
                    for (int i = 0; i < count; i++)
                    {
                        if (i == count - 1)
                        {
                            text += row[i].ToString();
                            text2 = text2 ?? "";
                        }
                        else
                        {
                            text = text + row[i].ToString() + ",";
                            text2 += ",";
                        }
                    }
                    if (text != text2)
                    {
                        list2.Add(text);
                    }
                }
                if (list2.Count <= 3)
                {
                    _retCode = 10000103;
                    list.Add(_retCode.ToString());
                    list.Add("Channel Map '" + ChannelName + "' is empty or have not pin, please check.");
                    return list;
                }
                if (!(list2[0].Split(',')[0] == "Channel"))
                {
                    _retCode = 10000102;
                    list.Add(_retCode.ToString());
                    list.Add("Channel map '" + ChannelName + "' key type is not 'Channel', please check");
                    return list;
                }
                if (!(list2[1].Split(',')[0] == "SiteCount") || !(list2[1].Split(',')[1] != "") || !(list2[2].Split(',')[0] == "GroupName") || !(list2[2].Split(',')[1] == "PinName") || !(list2[2].Split(',')[2] == "Type"))
                {
                    _retCode = 10000101;
                    list.Add(_retCode.ToString());
                    string text3 = "Check channel map '" + ChannelName + "' key fail.";
                    if (list2[1].Split(',')[0] != "SiteCount")
                    {
                        text3 = text3 + "'SiteCount' key don't match, current is '" + list2[1].Split(',')[0] + "' at Cell(1,0), please check.  ";
                    }
                    if (list2[1].Split(',')[1] != "")
                    {
                        text3 += "Site count is empty at Cell(1,1), please check. ";
                    }
                    if (list2[2].Split(',')[0] != "GroupName")
                    {
                        text3 = text3 + "'GroupName' key don't match, current is '" + list2[2].Split(',')[0] + "' at Cell(2,0), please check. ";
                    }
                    if (list2[2].Split(',')[1] != "PinName")
                    {
                        text3 = text3 + "'PinName's key don't match, current is '" + list2[2].Split(',')[1] + "' at Cell(2,1), please check. ";
                    }
                    if (list2[2].Split(',')[2] != "Type")
                    {
                        text3 = text3 + "'Type's key don't match, current is '" + list2[2].Split(',')[2] + "' at Cell(2,2), please check. ";
                    }
                    list.Add(text3);
                    return list;
                }
                if (!int.TryParse(list2[1].Split(',')[1], out result))
                {
                    _retCode = 11000100;
                    string item = "Site count " + list2[1].Split(',')[1] + " isn't intergrate.";
                    list.Clear();
                    list.Add(_retCode.ToString());
                    list.Add(item);
                    return list;
                }
                List<string> list3 = list2[2].Split(',').ToList();
                string text4 = "";
                bool flag = false;
                List<string> list4 = new List<string>();
                if (list3.Count >= 3 + result)
                {
                    for (int j = 3; j < list3.Count; j++)
                    {
                        if (!list3[j].Trim().StartsWith("Site "))
                        {
                            flag = true;
                            list4.Add("Wrong : Site Name " + list3[j].Trim() + " don't start with 'Site' key.");
                        }
                        if (list3[j].Trim().Replace("Site ", "") != (j - 3).ToString())
                        {
                            flag = true;
                            list4.Add($"Wrong : The site number of the Site Name {list3[j].Trim()} isn't correct, the site number should be the {j - 3}.");
                        }
                        text4 = text4 + list3[j].Trim() + ",";
                    }
                    struct_parentChannel.SiteNameJoin = text4;
                }
                else
                {
                    flag = true;
                    list4.Add($"Wrong : Site name col {list3.Count - 3} is less than the site count {result}.");
                }
                if (flag)
                {
                    _retCode = 11000100;
                    string item2 = string.Join("\n", list4);
                    list.Clear();
                    list.Add(_retCode.ToString());
                    list.Add(item2);
                    return list;
                }
                if (struct_parentChannel.SiteNameJoin.Split(',').Length - 1 != result)
                {
                    _retCode = 11000100;
                    string item3 = $" Site count {result} and Channel Assign site count don't match, please check. ";
                    list.Clear();
                    list.Add(_retCode.ToString());
                    list.Add(item3);
                    return list;
                }
                string text5 = "";
                string text6 = "";
                List<string> list5 = new List<string>();
                List<string> list6 = new List<string>();
                string text7 = "";
                for (int k = 3; k < list2.Count; k++)
                {
                    if (list2[k].Split(',')[0] == "" && text7 == "")
                    {
                        list6.Add("Pins");
                    }
                    else if (list2[k].Split(',')[0] == "" && text7 != "")
                    {
                        list6.Add(text7);
                        if (text5 != "" || text6 != "")
                        {
                            string[] array = list2[k].Split(',');
                            array[0] = text5;
                            if (array[2] == "")
                            {
                                array[2] = text6;
                            }
                            string text8 = "";
                            for (int l = 0; l < array.Count(); l++)
                            {
                                text8 = text8 + array[l] + ",";
                            }
                            list2[k] = text8;
                        }
                    }
                    else if (list2[k].Split(',')[0] != null)
                    {
                        text5 = list2[k].Split(',')[0];
                        text6 = list2[k].Split(',')[2];
                        text7 = list2[k].Split(',')[0];
                        list6.Add(list2[k].Split(',')[0]);
                    }
                }
                list5 = list6.Distinct().ToList();
                int num = -1;
                List<string> list7 = new List<string>();
                for (int m = 0; m < list3.Count; m++)
                {
                    if (list3[m] == "PinName")
                    {
                        num = m;
                        break;
                    }
                }
                list7.Clear();
                for (int n = 3; n < list2.Count; n++)
                {
                    if (list2[n].Split(',')[0] == "")
                    {
                        string item4 = list2[n].Split(',')[num];
                        list7.Add(item4);
                    }
                }
                int count2 = list7.Count;
                list7 = list7.Distinct().ToList();
                if (count2 != list7.Count)
                {
                    list.Add("10000032");
                    list.Add("The Pin Name in the Channel sheet is not unique");
                    return list;
                }
                int num2 = 0;
                int num3 = list2[2].Split(',').Count() - 4;
                List<string> list8 = new List<string>();
                for (int num4 = 3; num4 < list2.Count; num4++)
                {
                    if (!(list2[num4].Split(',')[0] == ""))
                    {
                        continue;
                    }
                    string text9 = list2[num4].Split(',')[2];
                    for (int num5 = 3; num5 < num3 + 3; num5++)
                    {
                        string text10 = list2[num4].Split(',')[num5].Trim();
                        string text11 = "";
                        if (text10.ToUpper().Contains(":"))
                        {
                            text11 = list2[num4].Split(',')[num5].Trim();
                        }
                        else if (text10.ToUpper().StartsWith("SITE"))
                        {
                            int num6 = int.Parse(list2[num4].Split(',')[num5].Trim().ToUpper().Replace("SITE", "")
                                .Trim());
                            if (num6 >= num3)
                            {
                                list.Add("10100036");
                                list.Add($"SITE number {num6} out of range , site count {num3} at channel map row {num4 + 3}");
                                return list;
                            }
                            text11 = list2[num4].Split(',')[num6 + 3];
                            if (!text11.Contains(":"))
                            {
                                list.Add("10100033");
                                list.Add(string.Format(ErrorMessageFormat.ChannelNameFormatIncorrect, text11, num6, ChannelName, num4 + 3));
                                return list;
                            }
                            num2++;
                        }
                        else
                        {
                            if (!text10.ToUpper().Contains(".SITE"))
                            {
                                list.Add("10100034");
                                list.Add($"Channel ‘{text10}’ format don't support at Channel map row {num4 + 3}");
                                return list;
                            }
                            int num7 = int.Parse(list2[num4].Split(',')[num5].Trim().ToUpper().Split('.')[1].Replace("SITE", "").Trim());
                            string text12 = list2[num4].Split(',')[num5].Trim().Split('.')[0];
                            if (num7 >= num3)
                            {
                                list.Add("10100036");
                                list.Add($"SITE number {num7} out of range , site count {num3} at channel map row {num4 + 3}");
                                return list;
                            }
                            DataRow[] array2 = inputDataTable.Select("F2='" + text12 + "' and F3='" + list2[num4].Split(',')[2].Trim() + "'");
                            if (array2.Length < 1)
                            {
                                if (array2.Length == 0)
                                {
                                    list.Add("10100053");
                                    list.Add("Can't find the shared pin name '" + text12 + "' in the channel map");
                                    return list;
                                }
                                list.Add("10100054");
                                list.Add($"Find the {array2.Length} shared pin name '{text12}' in the channel map");
                                return list;
                            }
                            if (array2[0][0].ToString().Trim() != "")
                            {
                                list.Add("10100052");
                                list.Add("Share pin's group name should be empty, but the group name is '" + array2[0][0].ToString().Trim() + "'");
                                return list;
                            }
                            if (!array2[0][num7 + 3].ToString().Contains(":"))
                            {
                                list.Add("10100050");
                                list.Add(string.Format(ErrorMessageFormat.ChannelNameFormatIncorrect, text11, num7, ChannelName, num4 + 3));
                                return list;
                            }
                            num2++;
                            if (array2.Length > 1)
                            {
                                for (int num8 = 1; num8 < array2.Length; num8++)
                                {
                                    if (array2[num8][0].ToString() == "" && array2[num8][num7 + 3].ToString().Contains(":"))
                                    {
                                        list.Add("10100054");
                                        list.Add("There have the same pin name " + text12 + " in the channel map");
                                        return list;
                                    }
                                }
                            }
                            text11 = array2[0][num7 + 3].ToString();
                        }
                        list8.Add(text9 + "_" + text11);
                    }
                }
                int count3 = list8.Count;
                list8 = list8.Distinct().ToList();
                if (count3 - num2 != list8.Count)
                {
                    list.Add("10000033");
                    list.Add("The Type && site channel in the Channel sheet is not unique");
                    return list;
                }
                string text13 = "";
                string text14 = "";
                for (int num9 = 3; num9 < list2.Count; num9++)
                {
                    if (!(list2[num9].Split(',')[0] != text13) && !(text13 == ""))
                    {
                        if (list2[num9].Split(',')[0] == text13 && !(text14 == list2[num9].Split(',')[2]))
                        {
                            _retCode = 10000055;
                            list.Add(_retCode.ToString());
                            list.Add("Type under the same GroupName in the Channel sheet is different");
                            return list;
                        }
                    }
                    else
                    {
                        text13 = list2[num9].Split(',')[0];
                        text14 = list2[num9].Split(',')[2];
                    }
                }
                List<string> list9 = new List<string>();
                List<string> list10 = new List<string>();
                for (int num10 = 3; num10 < list2.Count; num10++)
                {
                    if (list2[num10].Split(',')[0] == "")
                    {
                        list9.Add(list2[num10].Split(',')[1]);
                    }
                    else
                    {
                        list10.Add(list2[num10].Split(',')[1]);
                    }
                }
                list10 = list10.Distinct().ToList();
                if (list10.Count > 0)
                {
                    for (int num11 = 0; num11 < list10.Count; num11++)
                    {
                        bool flag2 = false;
                        for (int num12 = 0; num12 < list9.Count; num12++)
                        {
                            if (list10[num11] == list9[num12])
                            {
                                flag2 = true;
                            }
                        }
                        if (!flag2)
                        {
                            _retCode = 10000056;
                            list.Add(_retCode.ToString());
                            list.Add("The PinName " + list10[num11] + " in the grouping is not found in the defined pin list");
                            return list;
                        }
                    }
                }
                for (int num13 = 3; num13 < list2.Count; num13++)
                {
                    for (int num14 = 3; num14 < count; num14++)
                    {
                        if (list2[num13].Split(',')[num14] != "" && list2[num13].Split(',')[0] != "")
                        {
                            _retCode = 10000057;
                            list.Add(_retCode.ToString());
                            list.Add("When GroupName is not empty, the channel information must be empty");
                            return list;
                        }
                    }
                }
                P_ChannelSheet_Info = list2;
                int num15 = 0;
                text7 = "";
                for (int num16 = 0; num16 < list5.Count; num16++)
                {
                    num15 = 0;
                    for (int num17 = 0; num17 < list6.Count; num17++)
                    {
                        if (list5[num16] == list6[num17])
                        {
                            num15++;
                        }
                    }
                    struct_parentChannel.ChannelName = ChannelName;
                    struct_parentChannel.SiteCount = result;
                    struct_parentChannel.struct_channel = new Struct_Channel[num15];
                    struct_parentChannel.struct_rawChannel = new Struct_Channel[num15];
                    for (int num18 = 0; num18 < num15; num18++)
                    {
                        struct_parentChannel.struct_channel[num18].SiteInfo = new Struct_ChildSite[count - 3];
                        struct_parentChannel.struct_rawChannel[num18].SiteInfo = new Struct_ChildSite[count - 3];
                    }
                    int num19 = 0;
                    text7 = "";
                    string text15 = "";
                    string empty = string.Empty;
                    try
                    {
                        for (int num20 = 0; num20 < list6.Count; num20++)
                        {
                            if (!(list5[num16] == list6[num20]))
                            {
                                continue;
                            }
                            struct_parentChannel.struct_channel[num19].Pin_Name = list2[num20 + 3].Split(',')[1];
                            struct_parentChannel.struct_channel[num19].Type = list2[num20 + 3].Split(',')[2];
                            empty = struct_parentChannel.struct_channel[num19].Type;
                            struct_parentChannel.struct_rawChannel[num19].Pin_Name = list2[num20 + 3].Split(',')[1];
                            struct_parentChannel.struct_rawChannel[num19].Type = list2[num20 + 3].Split(',')[2];
                            empty = struct_parentChannel.struct_rawChannel[num19].Type;
                            for (int num21 = 3; num21 < count; num21++)
                            {
                                text15 = list2[num20 + 3];
                                string channelName = list2[num20 + 3].Split(',')[num21].ToUpper();
                                if (channelName.ToUpper().StartsWith("SITE"))
                                {
                                    int num22 = int.Parse(channelName.Replace("SITE".ToUpper(), ""));
                                    channelName = list2[num20 + 3].Split(',')[num22 + 3].ToUpper();
                                }
                                else if (channelName.ToUpper().Contains(".SITE"))
                                {
                                    string text16 = list2[num20 + 3].Split(',')[num21];
                                    int num23 = int.Parse(text16.Split('.')[1].Replace("SITE", "").Trim());
                                    string text17 = text16.Split('.')[0];
                                    string text18 = list2[num20 + 3].Split(',')[2];
                                    channelName = inputDataTable.Select("F2='" + text17 + "' and F3='" + text18 + "'")[0][num23 + 3].ToString().ToUpper();
                                }
                                else
                                {
                                    channelName.ToUpper().Contains(":");
                                }
                                string strChannelName = string.Empty;
                                if (!string.IsNullOrEmpty(channelName))
                                {
                                    string text19 = empty.ToUpper();
                                    if (!(text19 == "I/O"))
                                    {
                                        if (text19 == "DPS")
                                        {
                                            if (!SignalNameChannelNameConvertor.DPS_ConvertChannelNameToSignalName(ref channelName, out var signalName, out var listErrorMessage))
                                            {
                                                list.Add("10000079");
                                                list.Add(string.Format("Convert DPS channel name '{0}' to signal name fail in {1} sheet row {2}. Message is {3}", channelName, ChannelName, num20 + 3 + 1, string.Join(",", listErrorMessage)));
                                                return list;
                                            }
                                            if (!SignalNameChannelNameConvertor.ConvertChannelNameToDTEDisplayName(ref channelName, out var diplayName, out var listErrorMessage2))
                                            {
                                                list.Add("10000079");
                                                list.Add(string.Format("Convert DPS channel name '{0}' to signal name fail in {1} sheet row {2}. Message is {3}", channelName, ChannelName, num20 + 3 + 1, string.Join(",", listErrorMessage2)));
                                                return list;
                                            }
                                            channelName = signalName;
                                            strChannelName = diplayName;
                                        }
                                    }
                                    else
                                    {
                                        if (!SignalNameChannelNameConvertor.IO_ConvertChannelNameToSignalName(ref channelName, out var signalName2, out var listErrorMessage3))
                                        {
                                            list.Add("10000079");
                                            list.Add(string.Format("Convert I/O channel name '{0}' to signal name fail in {1} sheet row {2}. Message is {3}", channelName, ChannelName, num20 + 3 + 1, string.Join(",", listErrorMessage3)));
                                            return list;
                                        }
                                        if (!SignalNameChannelNameConvertor.ConvertChannelNameToDTEDisplayName(ref channelName, out var diplayName2, out var listErrorMessage4))
                                        {
                                            list.Add("10000079");
                                            list.Add(string.Format("Convert DPS channel name '{0}' to signal name fail in {1} sheet row {2}. Message is {3}", channelName, ChannelName, num20 + 3 + 1, string.Join(",", listErrorMessage4)));
                                            return list;
                                        }
                                        channelName = signalName2;
                                        strChannelName = diplayName2;
                                    }
                                }
                                struct_parentChannel.struct_channel[num19].SiteInfo[num21 - 3].SiteValue = ChannelUtlity.GetFormatChannelName(channelName, empty);
                                struct_parentChannel.struct_channel[num19].SiteInfo[num21 - 3].SiteName = list2[2].Split(',')[num21].Trim();
                                struct_parentChannel.struct_rawChannel[num19].SiteInfo[num21 - 3].SiteValue = ChannelUtlity.GetFormatChannelName(strChannelName, empty);
                                struct_parentChannel.struct_rawChannel[num19].SiteInfo[num21 - 3].SiteName = list2[2].Split(',')[num21].Trim();
                            }
                            num19++;
                        }
                        ChannelDic.Add(list5[num16], struct_parentChannel);
                    }
                    catch (Exception ex2)
                    {
                        _retCode = 10100102;
                        list.Add(_retCode.ToString());
                        list.Add("遍历 pin 得到 site channel " + text15 + " 信息发生异常， 异常信息，" + ex2.Message);
                        return list;
                    }
                }
            }
            catch (Exception ex3)
            {
                _retCode = 10000104;
                list.Add(_retCode.ToString());
                list.Add("An exception found in the load channel map '" + ChannelName + "', exception message is " + ex3.Message + ", please check the channal map");
                return list;
            }
            finally
            {
                if (list.Count < 2)
                {
                    list.Add("0");
                }
                inputDataTable.Dispose();
            }
            return list;
        }

        private static List<string> ConnectionExcel(string filePath)
        {
            List<string> list = new List<string>();
            try
            {
                if (!Path.HasExtension(filePath))
                {
                    if (!Directory.Exists(filePath))
                    {
                        throw new DirectoryNotFoundException("Can't find TestPlan directory '" + filePath + "'.");
                    }
                    _testPlanDataSource = new CsvDataSource(filePath);
                }
                else
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    _retCode = 10000001;
                    if (!fileInfo.Exists)
                    {
                        list.Add(_retCode.ToString());
                        list.Add("The file path does not exist");
                        return list;
                    }
                    switch (fileInfo.Extension.ToLower())
                    {
                        case ".xls":
                        case ".xlsx":
                        case ".xlsm":
                            _testPlanDataSource = new ExcelDataSource(filePath);
                            break;
                        case ".dll":
                            _testPlanDataSource = new DllDataSource(filePath);
                            break;
                    }
                }
                _testPlanDataSource.ParseToDataTable();
                _retCode = 0;
                list.Add(_retCode.ToString());
                return list;
            }
            catch (Exception ex)
            {
                _retCode = 10000002;
                list.Add(_retCode.ToString());
                list.Add("Open file failed, fail message is " + ex.Message);
                //LogHelper.WriteError("Open test plan exception, exception message is " + ex.Message);
                return list;
            }
        }
    }
}
