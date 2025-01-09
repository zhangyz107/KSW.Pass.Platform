/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：PatternCompilerBLL.cs
// 功能描述：向量编译业务逻辑服务
//
// 作者：zhangyingzhong
// 日期：2024/12/30 16:03
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.Application;
using KSW.ATE01.Pattern.Application.BLLs.Abstractions.Patterns;
using KSW.ATE01.Pattern.Application.Extensions;
using KSW.ATE01.Pattern.Application.Models.Projects;
using KSW.ATE01.Pattern.Domain.Projects.Core.Enums;
using KSW.ATE01.Pattern.Domain.Projects.Patterns;
using System.Text;
using System.Text.RegularExpressions;

namespace KSW.ATE01.Pattern.Application.BLLs.Implements.Patterns
{
    /// <summary>
    /// 向量编译业务逻辑服务
    /// </summary>
    public class PatternCompilerBLL : ServiceBase, IPatternCompilerBLL
    {
        private bool _compileError;

        private Regex _digitalInstrumentRegex = new Regex("^\\s*digital_ins\\s*=\\s*(.+?)\\s*;", RegexOptions.IgnoreCase);

        private Regex _timeSetRegex = new Regex("^\\s*import\\s*tset\\s*(.+?)\\s*;", RegexOptions.IgnoreCase);

        private Regex _instrumentsRegex = new Regex("^\\s*instruments\\s*=\\s*", RegexOptions.IgnoreCase);

        private Regex _aTPPinsRegex = new Regex("\\s*([-a-zA-Z0-9_]*)\\s*([-a-zA-Z0-9_]*)\\s*\\(\\s*\\$tset\\s*,(.+)\\)", RegexOptions.IgnoreCase);

        private Regex _vectorRegex = new Regex("\\s*((([^{:>]+):){0,})\\s*([^{:>]*)>\\s*([-a-zA-Z0-9_]+)\\s+(.+?)\\s*;(.*)", RegexOptions.IgnoreCase);

        private Regex _commandLoopRegex = new Regex("\\s*loop([abc])\\s+([0-9]+)\\s+(>.+;).?", RegexOptions.IgnoreCase);

        private Regex _commandEndLoopRegex = new Regex("\\s*end_loop([abc])\\s+([0-9a-zA-Z_]+\\s*>.+;).?", RegexOptions.IgnoreCase);

        private Regex _commandRepeatRegex = new Regex("\\s*repeat\\s+([0-9]+)\\s+(>.+;).?", RegexOptions.IgnoreCase);

        private Regex _srmuCodeAnalysisRegex = new Regex("(if\\s*\\(\\s*([!\\w]+)\\s*\\)\\s*)*([!\\w]+)*(\\s+|\\()*([!\\w\\s]+)*", RegexOptions.IgnoreCase);

        //private Dictionary<long, ModelLabel> dicVectorLineNumberLabelName = new Dictionary<long, ModelLabel>();

        private MemoryStream _memoryStreamHead;

        private FileStream _memoryStreamVector;

        private FileStream _dicVectorLineNumberComment;

        private string _pathPattern = string.Empty;

        private string _pathTestPlan = string.Empty;

        private string _sheetName = string.Empty;

        private string _pathOutputBin = string.Empty;

        private bool _saveComment;

        private bool _recordingSwitchStart;

        private bool _recordingSwitchTrig;

        private bool _ignoreCurrentVectorRowTrig;

        private bool _ignoreCurrentVectorRowStart;

        private int _dataBlockIndexStart = -1;

        private int _dataBlockIndexTrig = -1;

        private string _strDigitalInstrument = string.Empty;

        private string _strOPCodeMode = string.Empty;

        private string _memoryName = string.Empty;

        private string _vectorName = string.Empty;

        private string _tempNestLoopOutermostLoopName = string.Empty;

        private int _nestLoopIndex = -1;

        private List<string> _atpPinsPinGroups = new List<string>();

        private List<string> _listTotalMaskCC = new List<string>();

        private List<string> _atpPinsPinGroupsWithDigitalMode = new List<string>();

        private List<string> _dataBlockMarkerParameter = new List<string>();

        private List<Instrument> _patternInstrument = new List<Instrument>();

        private Dictionary<string, int> _pseudoInstruDic = new Dictionary<string, int>();

        private Dictionary<string, int> _srmModifilerDic = new Dictionary<string, int>();

        private Dictionary<string, int> _dicPretreatmentLabelNameVectorLineNumber = new Dictionary<string, int>();

        private Dictionary<int, string> _patternTimeSetDic = new Dictionary<int, string>();

        private Dictionary<string, byte> _pinValueNameExchangeDic = new Dictionary<string, byte>();

        private Dictionary<string, List<string>> _testPlanPins = new Dictionary<string, List<string>>();

        private Dictionary<string, Dictionary<string, int>> _dicDataGenerator = new Dictionary<string, Dictionary<string, int>>
        {
            {
                "dreg",
                new Dictionary<string, int>
                {
                    { "reg0", 0 },
                    { "reg1", 1 },
                    { "reg2", 2 },
                    { "reg3", 3 },
                    { "reg4", 4 },
                    { "reg5", 5 },
                    { "reg6", 6 },
                    { "reg7", 7 },
                    { "reg8", 8 },
                    { "reg9", 9 },
                    { "reg10", 10 },
                    { "reg11", 11 },
                    { "reg12", 12 },
                    { "reg13", 13 },
                    { "reg14", 14 },
                    { "reg15", 15 }
                }
            },
            {
                "dg_out",
                new Dictionary<string, int>
                {
                    { "dg0", 0 },
                    { "dg1", 1 },
                    { "jam_reg", 2 },
                    { "map_data", 3 }
                }
            },
            {
                "dgram1_select",
                new Dictionary<string, int>()
            },
            {
                "dgram0_select",
                new Dictionary<string, int>()
            },
            {
                "x_data_generator_address_sel",
                new Dictionary<string, int>
                {
                    { "xa", 0 },
                    { "xb", 1 },
                    { "xc", 2 },
                    { "xd", 3 },
                    { "!xa", 4 },
                    { "!xb", 5 },
                    { "!xc", 6 },
                    { "!xd", 7 }
                }
            },
            {
                "y_data_generator_address_sel",
                new Dictionary<string, int>
                {
                    { "xa", 0 },
                    { "xb", 1 },
                    { "xc", 2 },
                    { "xd", 3 },
                    { "!xa", 4 },
                    { "!xb", 5 },
                    { "!xc", 6 },
                    { "!xd", 7 }
                }
            },
            {
                "z_data_generator_address_sel",
                new Dictionary<string, int>
                {
                    { "xa", 0 },
                    { "xb", 1 },
                    { "xc", 2 },
                    { "xd", 3 },
                    { "!xa", 4 },
                    { "!xb", 5 },
                    { "!xc", 6 },
                    { "!xd", 7 }
                }
            }
        };

        private Dictionary<string, Dictionary<string, int>> _dicMTE = new Dictionary<string, Dictionary<string, int>>();

        private Dictionary<string, Dictionary<string, int>> _dicMisc = new Dictionary<string, Dictionary<string, int>>
        {
            {
                "device_data_shift",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "shift", 1 }
                }
            },
            {
                "ps",
                new Dictionary<string, int>()
            },
            {
                "xaddr",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "shift", 1 }
                }
            },
            {
                "yaddr",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "shift", 1 }
                }
            },
            {
                "zaddr",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "shift", 1 }
                }
            },
            {
                "data",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "shift", 1 }
                }
            },
            {
                "stvc",
                new Dictionary<string, int>
                {
                    { "nop", 0 },
                    { "stv", 1 }
                }
            },
            {
                "stvm0",
                new Dictionary<string, int>
                {
                    { "nop", 0 },
                    { "stv", 1 }
                }
            },
            {
                "stvm1",
                new Dictionary<string, int>
                {
                    { "nop", 0 },
                    { "stv", 1 }
                }
            },
            {
                "util_cntr_a",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "int", 1 },
                    { "inc", 2 },
                    { "dec", 3 },
                    { "preset", 4 }
                }
            },
            {
                "util_cntr_b",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "int", 1 },
                    { "inc", 2 },
                    { "dec", 3 },
                    { "preset", 4 }
                }
            },
            {
                "util_cntr_c",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "int", 1 },
                    { "inc", 2 },
                    { "dec", 3 },
                    { "preset", 4 }
                }
            },
            {
                "start_refresh",
                new Dictionary<string, int>
                {
                    { "nop", 0 },
                    { "start", 1 }
                }
            },
            {
                "vbcnd_stb",
                new Dictionary<string, int>
                {
                    { "disable", 0 },
                    { "enable", 1 }
                }
            }
        };

        private Dictionary<string, Dictionary<string, int>> _dicXAddressGenerator = new Dictionary<string, Dictionary<string, int>>
        {
            {
                "xpreset_select",
                new Dictionary<string, int>
                {
                    { "sv", 0 },
                    { "reg", 1 }
                }
            },
            {
                "xreg",
                new Dictionary<string, int>
                {
                    { "reg0", 0 },
                    { "reg1", 1 },
                    { "reg2", 2 },
                    { "reg3", 3 },
                    { "reg4", 4 },
                    { "reg5", 5 },
                    { "reg6", 6 },
                    { "reg7", 7 },
                    { "reg8", 8 },
                    { "reg9", 9 },
                    { "reg10", 10 },
                    { "reg11", 11 },
                    { "reg12", 12 },
                    { "reg13", 13 },
                    { "reg14", 14 },
                    { "reg15", 15 }
                }
            },
            {
                "xalu_const0",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "jam_reg", 1 },
                    { "shift_left", 2 },
                    { "shift_right", 3 }
                }
            },
            {
                "xalu_const1",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "jam_reg", 1 },
                    { "shift_left", 2 },
                    { "shift_right", 3 }
                }
            },
            {
                "xenable_load",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "jam_reg", 1 },
                    { "shift_left_and", 2 },
                    { "invert", 3 }
                }
            },
            {
                "xalu_op",
                new Dictionary<string, int>
                {
                    { "add", 0 },
                    { "add_link", 1 },
                    { "sub", 2 },
                    { "sub_link", 3 },
                    { "or", 4 },
                    { "and", 5 },
                    { "xor", 6 },
                    { "nop", 7 },
                    { "shift_left", 8 },
                    { "shift_left_link", 9 },
                    { "shift_right", 10 },
                    { "shift_right_link", 11 },
                    { "nor", 12 },
                    { "nand", 13 },
                    { "xnor", 14 },
                    { "inv", 15 }
                }
            },
            {
                "xaluj_sel",
                new Dictionary<string, int>
                {
                    { "xa", 0 },
                    { "xb", 1 },
                    { "xc", 2 },
                    { "xd", 3 },
                    { "logic_0xfff", 4 },
                    { "logic_0", 5 },
                    { "alu_constant0", 6 },
                    { "alu_constant1", 7 }
                }
            },
            {
                "xaluk_sel",
                new Dictionary<string, int>
                {
                    { "xa", 0 },
                    { "xb", 1 },
                    { "xc", 2 },
                    { "xd", 3 },
                    { "logic_0xfff", 4 },
                    { "logic_0", 5 },
                    { "alu_constant0", 6 },
                    { "alu_constant1", 7 }
                }
            },
            {
                "xdevadr5",
                new Dictionary<string, int>
                {
                    { "xa", 0 },
                    { "xb", 1 },
                    { "xc", 2 },
                    { "xd", 3 },
                    { "!xa", 4 },
                    { "!xb", 5 },
                    { "!xc", 6 },
                    { "!xd", 7 }
                }
            },
            {
                "xdevadr4",
                new Dictionary<string, int>
                {
                    { "xa", 0 },
                    { "xb", 1 },
                    { "xc", 2 },
                    { "xd", 3 },
                    { "!xa", 4 },
                    { "!xb", 5 },
                    { "!xc", 6 },
                    { "!xd", 7 }
                }
            },
            {
                "xdevadr3",
                new Dictionary<string, int>
                {
                    { "xa", 0 },
                    { "xb", 1 },
                    { "xc", 2 },
                    { "xd", 3 },
                    { "!xa", 4 },
                    { "!xb", 5 },
                    { "!xc", 6 },
                    { "!xd", 7 }
                }
            },
            {
                "xdevadr2",
                new Dictionary<string, int>
                {
                    { "xa", 0 },
                    { "xb", 1 },
                    { "xc", 2 },
                    { "xd", 3 },
                    { "!xa", 4 },
                    { "!xb", 5 },
                    { "!xc", 6 },
                    { "!xd", 7 }
                }
            },
            {
                "xdevadr1",
                new Dictionary<string, int>
                {
                    { "xa", 0 },
                    { "xb", 1 },
                    { "xc", 2 },
                    { "xd", 3 },
                    { "!xa", 4 },
                    { "!xb", 5 },
                    { "!xc", 6 },
                    { "!xd", 7 }
                }
            },
            {
                "xdevadr0",
                new Dictionary<string, int>
                {
                    { "xa", 0 },
                    { "xb", 1 },
                    { "xc", 2 },
                    { "xd", 3 },
                    { "!xa", 4 },
                    { "!xb", 5 },
                    { "!xc", 6 },
                    { "!xd", 7 }
                }
            },
            {
                "xa",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "load_alu", 2 },
                    { "jam_reg", 6 },
                    { "load_reg", 7 },
                    { "dec", 8 },
                    { "dec_link_y", 10 },
                    { "dec_link_z", 11 },
                    { "inc", 12 },
                    { "inc_link_y", 14 },
                    { "inc_link_z", 15 }
                }
            },
            {
                "xb",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "load_alu", 2 },
                    { "jam_reg", 6 },
                    { "load_reg", 7 },
                    { "dec", 8 },
                    { "dec_link_y", 10 },
                    { "dec_link_z", 11 },
                    { "inc", 12 },
                    { "inc_link_y", 14 },
                    { "inc_link_z", 15 }
                }
            },
            {
                "xc",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "load_alu", 2 },
                    { "jam_reg", 6 },
                    { "load_reg", 7 },
                    { "dec", 8 },
                    { "dec_link_y", 10 },
                    { "dec_link_z", 11 },
                    { "inc", 12 },
                    { "inc_link_y", 14 },
                    { "inc_link_z", 15 }
                }
            },
            {
                "xd",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "load_alu", 2 },
                    { "jam_reg", 6 },
                    { "load_reg", 7 },
                    { "dec", 8 },
                    { "dec_link_y", 10 },
                    { "dec_link_z", 11 },
                    { "inc", 12 },
                    { "inc_link_y", 14 },
                    { "inc_link_z", 15 }
                }
            }
        };

        private Dictionary<string, Dictionary<string, int>> _dicYAddressGenerator = new Dictionary<string, Dictionary<string, int>>
        {
            {
                "ypreset_select",
                new Dictionary<string, int>
                {
                    { "sv", 0 },
                    { "reg", 1 }
                }
            },
            {
                "yreg",
                new Dictionary<string, int>
                {
                    { "reg0", 0 },
                    { "reg1", 1 },
                    { "reg2", 2 },
                    { "reg3", 3 },
                    { "reg4", 4 },
                    { "reg5", 5 },
                    { "reg6", 6 },
                    { "reg7", 7 },
                    { "reg8", 8 },
                    { "reg9", 9 },
                    { "reg10", 10 },
                    { "reg11", 11 },
                    { "reg12", 12 },
                    { "reg13", 13 },
                    { "reg14", 14 },
                    { "reg15", 15 }
                }
            },
            {
                "yalu_const0",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "jam_reg", 1 },
                    { "shift_left", 2 },
                    { "shift_right", 3 }
                }
            },
            {
                "yalu_const1",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "jam_reg", 1 },
                    { "shift_left", 2 },
                    { "shift_right", 3 }
                }
            },
            {
                "yenable_load",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "jam_reg", 1 },
                    { "shift_left_and", 2 },
                    { "invert", 3 }
                }
            },
            {
                "yalu_op",
                new Dictionary<string, int>
                {
                    { "add", 0 },
                    { "add_link", 1 },
                    { "sub", 2 },
                    { "sub_link", 3 },
                    { "or", 4 },
                    { "and", 5 },
                    { "xor", 6 },
                    { "nop", 7 },
                    { "shift_left", 8 },
                    { "shift_left_link", 9 },
                    { "shift_right", 10 },
                    { "shift_right_link", 11 },
                    { "nor", 12 },
                    { "nand", 13 },
                    { "xnor", 14 },
                    { "inv", 15 }
                }
            },
            {
                "yaluj_sel",
                new Dictionary<string, int>
                {
                    { "ya", 0 },
                    { "yb", 1 },
                    { "yc", 2 },
                    { "yd", 3 },
                    { "logic_0xfff", 4 },
                    { "logic_0", 5 },
                    { "alu_constant0", 6 },
                    { "alu_constant1", 7 }
                }
            },
            {
                "yaluk_sel",
                new Dictionary<string, int>
                {
                    { "ya", 0 },
                    { "yb", 1 },
                    { "yc", 2 },
                    { "yd", 3 },
                    { "logic_0xfff", 4 },
                    { "logic_0", 5 },
                    { "alu_constant0", 6 },
                    { "alu_constant1", 7 }
                }
            },
            {
                "ydevadr5",
                new Dictionary<string, int>
                {
                    { "ya", 0 },
                    { "yb", 1 },
                    { "yc", 2 },
                    { "yd", 3 },
                    { "!ya", 4 },
                    { "!yb", 5 },
                    { "!yc", 6 },
                    { "!yd", 7 }
                }
            },
            {
                "ydevadr4",
                new Dictionary<string, int>
                {
                    { "ya", 0 },
                    { "yb", 1 },
                    { "yc", 2 },
                    { "yd", 3 },
                    { "!ya", 4 },
                    { "!yb", 5 },
                    { "!yc", 6 },
                    { "!yd", 7 }
                }
            },
            {
                "ydevadr3",
                new Dictionary<string, int>
                {
                    { "ya", 0 },
                    { "yb", 1 },
                    { "yc", 2 },
                    { "yd", 3 },
                    { "!ya", 4 },
                    { "!yb", 5 },
                    { "!yc", 6 },
                    { "!yd", 7 }
                }
            },
            {
                "ydevadr2",
                new Dictionary<string, int>
                {
                    { "ya", 0 },
                    { "yb", 1 },
                    { "yc", 2 },
                    { "yd", 3 },
                    { "!ya", 4 },
                    { "!yb", 5 },
                    { "!yc", 6 },
                    { "!yd", 7 }
                }
            },
            {
                "ydevadr1",
                new Dictionary<string, int>
                {
                    { "ya", 0 },
                    { "yb", 1 },
                    { "yc", 2 },
                    { "yd", 3 },
                    { "!ya", 4 },
                    { "!yb", 5 },
                    { "!yc", 6 },
                    { "!yd", 7 }
                }
            },
            {
                "ydevadr0",
                new Dictionary<string, int>
                {
                    { "ya", 0 },
                    { "yb", 1 },
                    { "yc", 2 },
                    { "yd", 3 },
                    { "!ya", 4 },
                    { "!yb", 5 },
                    { "!yc", 6 },
                    { "!yd", 7 }
                }
            },
            {
                "ya",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "load_alu", 2 },
                    { "jam_reg", 6 },
                    { "load_reg", 7 },
                    { "dec", 8 },
                    { "dec_link_y", 10 },
                    { "dec_link_z", 11 },
                    { "inc", 12 },
                    { "inc_link_y", 14 },
                    { "inc_link_z", 15 }
                }
            },
            {
                "yb",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "load_alu", 2 },
                    { "jam_reg", 6 },
                    { "load_reg", 7 },
                    { "dec", 8 },
                    { "dec_link_y", 10 },
                    { "dec_link_z", 11 },
                    { "inc", 12 },
                    { "inc_link_y", 14 },
                    { "inc_link_z", 15 }
                }
            },
            {
                "yc",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "load_alu", 2 },
                    { "jam_reg", 6 },
                    { "load_reg", 7 },
                    { "dec", 8 },
                    { "dec_link_y", 10 },
                    { "dec_link_z", 11 },
                    { "inc", 12 },
                    { "inc_link_y", 14 },
                    { "inc_link_z", 15 }
                }
            },
            {
                "yd",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "load_alu", 2 },
                    { "jam_reg", 6 },
                    { "load_reg", 7 },
                    { "dec", 8 },
                    { "dec_link_y", 10 },
                    { "dec_link_z", 11 },
                    { "inc", 12 },
                    { "inc_link_y", 14 },
                    { "inc_link_z", 15 }
                }
            }
        };

        private Dictionary<string, Dictionary<string, int>> _dicZAddressGenerator = new Dictionary<string, Dictionary<string, int>>
        {
            {
                "zpreset_select",
                new Dictionary<string, int>
                {
                    { "sv", 0 },
                    { "reg", 1 }
                }
            },
            {
                "zreg",
                new Dictionary<string, int>
                {
                    { "reg0", 0 },
                    { "reg1", 1 },
                    { "reg2", 2 },
                    { "reg3", 3 },
                    { "reg4", 4 },
                    { "reg5", 5 },
                    { "reg6", 6 },
                    { "reg7", 7 },
                    { "reg8", 8 },
                    { "reg9", 9 },
                    { "reg10", 10 },
                    { "reg11", 11 },
                    { "reg12", 12 },
                    { "reg13", 13 },
                    { "reg14", 14 },
                    { "reg15", 15 }
                }
            },
            {
                "zalu_const0",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "jam_reg", 1 },
                    { "shift_left", 2 },
                    { "shift_right", 3 }
                }
            },
            {
                "zalu_const1",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "jam_reg", 1 },
                    { "shift_left", 2 },
                    { "shift_right", 3 }
                }
            },
            {
                "zenable_load",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "jam_reg", 1 },
                    { "shift_left_and", 2 },
                    { "invert", 3 }
                }
            },
            {
                "zalu_op",
                new Dictionary<string, int>
                {
                    { "add", 0 },
                    { "add_link", 1 },
                    { "sub", 2 },
                    { "sub_link", 3 },
                    { "or", 4 },
                    { "and", 5 },
                    { "xor", 6 },
                    { "nop", 7 },
                    { "shift_left", 8 },
                    { "shift_left_link", 9 },
                    { "shift_right", 10 },
                    { "shift_right_link", 11 },
                    { "nor", 12 },
                    { "nand", 13 },
                    { "xnor", 14 },
                    { "inv", 15 }
                }
            },
            {
                "zaluj_sel",
                new Dictionary<string, int>
                {
                    { "za", 0 },
                    { "zb", 1 },
                    { "zc", 2 },
                    { "zd", 3 },
                    { "logic_0xfff", 4 },
                    { "logic_0", 5 },
                    { "alu_constant0", 6 },
                    { "alu_constant1", 7 }
                }
            },
            {
                "zaluk_sel",
                new Dictionary<string, int>
                {
                    { "za", 0 },
                    { "zb", 1 },
                    { "zc", 2 },
                    { "zd", 3 },
                    { "logic_0xfff", 4 },
                    { "logic_0", 5 },
                    { "alu_constant0", 6 },
                    { "alu_constant1", 7 }
                }
            },
            {
                "zdevadr5",
                new Dictionary<string, int>
                {
                    { "za", 0 },
                    { "zb", 1 },
                    { "zc", 2 },
                    { "zd", 3 },
                    { "!za", 4 },
                    { "!zb", 5 },
                    { "!zc", 6 },
                    { "!zd", 7 }
                }
            },
            {
                "zdevadr4",
                new Dictionary<string, int>
                {
                    { "za", 0 },
                    { "zb", 1 },
                    { "zc", 2 },
                    { "zd", 3 },
                    { "!za", 4 },
                    { "!zb", 5 },
                    { "!zc", 6 },
                    { "!zd", 7 }
                }
            },
            {
                "zdevadr3",
                new Dictionary<string, int>
                {
                    { "za", 0 },
                    { "zb", 1 },
                    { "zc", 2 },
                    { "zd", 3 },
                    { "!za", 4 },
                    { "!zb", 5 },
                    { "!zc", 6 },
                    { "!zd", 7 }
                }
            },
            {
                "zdevadr2",
                new Dictionary<string, int>
                {
                    { "za", 0 },
                    { "zb", 1 },
                    { "zc", 2 },
                    { "zd", 3 },
                    { "!za", 4 },
                    { "!zb", 5 },
                    { "!zc", 6 },
                    { "!zd", 7 }
                }
            },
            {
                "zdevadr1",
                new Dictionary<string, int>
                {
                    { "za", 0 },
                    { "zb", 1 },
                    { "zc", 2 },
                    { "zd", 3 },
                    { "!za", 4 },
                    { "!zb", 5 },
                    { "!zc", 6 },
                    { "!zd", 7 }
                }
            },
            {
                "zdevadr0",
                new Dictionary<string, int>
                {
                    { "za", 0 },
                    { "zb", 1 },
                    { "zc", 2 },
                    { "zd", 3 },
                    { "!za", 4 },
                    { "!zb", 5 },
                    { "!zc", 6 },
                    { "!zd", 7 }
                }
            },
            {
                "za",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "load_alu", 2 },
                    { "jam_reg", 6 },
                    { "load_reg", 7 },
                    { "dec", 8 },
                    { "dec_link_y", 10 },
                    { "dec_link_z", 11 },
                    { "inc", 12 },
                    { "inc_link_y", 14 },
                    { "inc_link_z", 15 }
                }
            },
            {
                "zb",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "load_alu", 2 },
                    { "jam_reg", 6 },
                    { "load_reg", 7 },
                    { "dec", 8 },
                    { "dec_link_y", 10 },
                    { "dec_link_z", 11 },
                    { "inc", 12 },
                    { "inc_link_y", 14 },
                    { "inc_link_z", 15 }
                }
            },
            {
                "zc",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "load_alu", 2 },
                    { "jam_reg", 6 },
                    { "load_reg", 7 },
                    { "dec", 8 },
                    { "dec_link_y", 10 },
                    { "dec_link_z", 11 },
                    { "inc", 12 },
                    { "inc_link_y", 14 },
                    { "inc_link_z", 15 }
                }
            },
            {
                "zd",
                new Dictionary<string, int>
                {
                    { "hold", 0 },
                    { "load_alu", 2 },
                    { "jam_reg", 6 },
                    { "load_reg", 7 },
                    { "dec", 8 },
                    { "dec_link_y", 10 },
                    { "dec_link_z", 11 },
                    { "inc", 12 },
                    { "inc_link_y", 14 },
                    { "inc_link_z", 15 }
                }
            }
        };

        private Dictionary<string, int> VM_PSEUDO_INSTRU = new Dictionary<string, int>
        {
            { "repeat", 1 },
            { "loop", 2 },
            { "match", 3 },
            { "jump", 4 },
            { "halt", 5 },
            { "reburst", 6 },
            { "endloop", 7 },
            { "trig", 8 },
            { "start", 13 },
            { "cpua", 9 },
            { "cpuaend", 10 },
            { "fstart", 11 },
            { "fstop", 12 }
        };

        private Dictionary<string, int> LVM_PSEUDO_INSTRU = new Dictionary<string, int>
        {
            { "nop", 0 },
            { "halt", 0 },
            { "repeat", 1 },
            { "call", 4 },
            { "trig", 8 }
        };

        private Dictionary<string, int> SRM_PSEUDO_INSTRU = new Dictionary<string, int>
        {
            { "nop", 0 },
            { "clr_flag", 2 },
            { "loopa", 4 },
            { "loopb", 4 },
            { "loopc", 4 },
            { "fstart", 7 },
            { "fstop", 7 },
            { "enable", 8 },
            { "set_cpu", 11 },
            { "poploopa", 13 },
            { "poploopb", 13 },
            { "poploopc", 13 },
            { "jump", 17 },
            { "end_loopa", 23 },
            { "end_loopb", 23 },
            { "end_loopc", 23 },
            { "return_lvm", 26 },
            { "trig", 27 },
            { "repeat", 30 },
            { "repeat_cc", 31 }
        };

        private Dictionary<string, int> SRM_Modifiler_strPseudo = new Dictionary<string, int>
        {
            { "set_loopa", 1 },
            { "set_loopb", 2 },
            { "set_loopc", 4 },
            { "loopa", 1 },
            { "loopb", 2 },
            { "loopc", 4 },
            { "end_loopa", 1 },
            { "end_loopb", 2 },
            { "end_loopc", 4 },
            { "poploopa", 1 },
            { "poploopb", 2 },
            { "poploopc", 4 }
        };

        private Dictionary<string, int> SRM_Modifiler_ConditionFlag = new Dictionary<string, int>
        {
            { "pass", 3 },
            { "flag", 1 },
            { "fail", 2 },
            { "!fail", 3 },
            { "cpua", 4 },
            { "!cpua", 5 },
            { "ext", 6 },
            { "!ext", 7 }
        };

        private Dictionary<string, int> SRM_Modifiler_Enable_And = new Dictionary<string, int> { { "enable", 4 } };

        private Dictionary<string, byte> VM_PIN_VALUENAME_EXCHANGE = new Dictionary<string, byte>
        {
            { "0", 0 },
            { "1", 1 },
            { "L", 4 },
            { "H", 5 },
            { "M", 6 },
            { "X", 7 },
            { "0L", 8 },
            { "0H", 9 },
            { "1L", 10 },
            { "1H", 11 },
            { "D", 12 },
            { "C", 13 },
            { "V", 14 }
        };

        private Dictionary<string, byte> LVM_PIN_VALUENAME_EXCHANGE = new Dictionary<string, byte>
        {
            { "0", 0 },
            { "X", 1 },
            { "-", 2 },
            { "1", 3 },
            { "L", 4 },
            { "M", 5 },
            { "V", 6 },
            { "H", 7 }
        };

        private Dictionary<string, byte> SRM_PIN_VALUENAME_EXCHANGE = new Dictionary<string, byte>
        {
            { "0", 0 },
            { "X", 1 },
            { "-", 2 },
            { "1", 3 },
            { "L", 4 },
            { "M", 5 },
            { "V", 6 },
            { "H", 7 }
        };

        private Dictionary<string, byte> SRM_MTE_PIN_VALUENAME_EXCHANGE = new Dictionary<string, byte>
        {
            { "0", 0 },
            { "X", 1 },
            { "D", 2 },
            { "1", 3 },
            { "L", 4 },
            { "E", 5 },
            { "V", 6 },
            { "H", 7 }
        };

        private Dictionary<string, int> _nestLoopBuffDic = new Dictionary<string, int>();

        private List<string> LVM_MASKCC = new List<string> { "stv", "maskb", "maska", "rsrm" };

        private List<string> SRM_MASKCC = new List<string>
        {
            "accfail", "ccnd", "padd", "sadd", "clr_fail", "ign", "icc", "ifc", "stv", "maskb",
            "maska", "rlvm"
        };

        private Dictionary<long, LabelModel> _dicVectorLineNumberLabelName = new Dictionary<long, LabelModel>();

        private Dictionary<long, LabelModel> _dicInUseVectorLineNumberLabelName = new Dictionary<long, LabelModel>();

        private ModuleType moduleType;

        public PatternCompilerBLL(IContainerProvider containerProvider) : base(containerProvider)
        {

        }

        public void SetCompilerPath(string patternFilePath, string testPlanFilePath, string testPlanSheetName, string outputBinFilePath, bool saveComment = true)
        {
            _pathPattern = patternFilePath;
            _pathTestPlan = testPlanFilePath;
            _sheetName = testPlanSheetName;
            _pathOutputBin = outputBinFilePath;
            _saveComment = saveComment;
        }

        public int CompilePattern(string tempFolder = "")
        {
            _compileError = false;
            if (string.IsNullOrEmpty(tempFolder))
            {
                tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempFolder);
            }
            _dicVectorLineNumberLabelName.Clear();
            _dicInUseVectorLineNumberLabelName.Clear();
            string path = Path.Combine(tempFolder, "CompilerComment.bin");
            //dicVectorLineNumberComment = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            _memoryStreamHead = new MemoryStream();
            string path2 = Path.Combine(tempFolder, "CompilerVector.bin");
            _memoryStreamVector = new FileStream(path2, FileMode.Create, FileAccess.ReadWrite);
            AnalysisPatternTimeSet(_pathPattern, _patternTimeSetDic);
            AnalysisPatternDigitalInstrument(_pathPattern, out _strDigitalInstrument);
            //AnalysisPatternOPCodeMode(_pathPattern, out _strOPCodeMode);
            //AnalysisPatternPins(_pathTestPlan, _sheetName, _testPlanPins);
            AnalysisPatternPins(_pathTestPlan, _atpPinsPinGroups, _atpPinsPinGroupsWithDigitalMode, ref _memoryName, ref _vectorName);
            UpdateParametersByModuleType(moduleType);
            CheckPatternPinsRelationTestPlanSplitPinGroup(_atpPinsPinGroupsWithDigitalMode, _testPlanPins, out var pins);
            AnalysisPatternInstrument(_pathPattern, _atpPinsPinGroups, _testPlanPins, _patternInstrument);
            if (_compileError)
            {
                CompileComplete();
                return 1;
            }
            //if (list_PatternInstrument.Count != 0)
            GetInstrumentsInfoFromDataBlock(_pathPattern, _atpPinsPinGroups, _patternInstrument, out _dataBlockMarkerParameter);
            if (_compileError)
            {
                CompileComplete();
                return 1;
            }
            return 0;
        }

        private void CompileComplete()
        {
            if (!_compileError)
            {
                //if (this.eventPrintCompileInfo != null)
                //{
                //    this.eventPrintCompileInfo($"Compile Successful !");
                //}
                WriteBytesFromMemoryToFlie(_memoryStreamHead, _dicVectorLineNumberLabelName, _dicInUseVectorLineNumberLabelName, _dicVectorLineNumberComment, _memoryStreamVector);
                ReleaseResources();
            }
            else
            {
                //if (this.eventPrintCompileInfo != null)
                //{
                //    this.eventPrintCompileInfo($"Compile Fail !");
                //}
                ReleaseResources();
            }
            //this.eventPrintCompilePercent(100.0);
        }

        private void ReleaseResources()
        {
            _dicVectorLineNumberComment.Dispose();
            _memoryStreamHead.Dispose();
            _memoryStreamVector.Dispose();
            File.Delete(_memoryStreamVector.Name);
            File.Delete(_dicVectorLineNumberComment.Name);
        }

        private void WriteBytesFromMemoryToFlie(MemoryStream memoryStreamHead, Dictionary<long, LabelModel> dicVectorLineNumberLabelName, Dictionary<long, LabelModel> dicInUseVectorLineNumberLabelName, FileStream dicVectorLineNumberComment, FileStream memoryStreamVector)
        {
            FormatLabelToByte(dicVectorLineNumberLabelName, out var arrBytes);
            FormatCommentToByte(dicVectorLineNumberComment, out var arrBytes2);
            FormatLabelToByte(dicInUseVectorLineNumberLabelName, out var arrBytes3);
            RecalculateDataToWrite(memoryStreamHead, arrBytes.Length, arrBytes2.Length, arrBytes3.Length, out var arrEachBlockLength);
            using FileStream fileStream = new FileStream(_pathOutputBin, FileMode.Create, FileAccess.Write);
            byte[] array = new byte[5242880];
            memoryStreamHead.Position = 0L;
            for (int num = memoryStreamHead.Read(array, 0, array.Length); num > 0; num = memoryStreamHead.Read(array, 0, array.Length))
            {
                fileStream.Write(array, 0, num);
            }
            fileStream.Write(arrBytes, 0, arrBytes.Length);
            fileStream.Write(arrBytes2, 0, arrBytes2.Length);
            fileStream.Write(arrBytes3, 0, arrBytes3.Length);
            fileStream.Write(arrEachBlockLength, 0, arrEachBlockLength.Length);
            memoryStreamVector.Position = 0L;
            for (int num2 = memoryStreamVector.Read(array, 0, array.Length); num2 > 0; num2 = memoryStreamVector.Read(array, 0, array.Length))
            {
                fileStream.Write(array, 0, num2);
            }
        }

        private void FormatLabelToByte(Dictionary<long, LabelModel> dicVectorLineNumberLabelName, out byte[] arrBytes)
        {
            arrBytes = new byte[0];
            List<byte> list = new List<byte>();
            foreach (KeyValuePair<long, LabelModel> item in dicVectorLineNumberLabelName)
            {
                byte[] bytes = Encoding.Default.GetBytes(item.Value.LabelName);
                byte[] array = BitConverter.GetBytes(bytes.Length);
                Array.Resize(ref array, 4);
                list.AddRange(array.Reverse());
                byte[] array2 = BitConverter.GetBytes(item.Key);
                Array.Resize(ref array2, 4);
                list.AddRange(array2.Reverse());
                list.AddRange(bytes);
                byte[] array3 = BitConverter.GetBytes((int)item.Value.LabelType);
                Array.Resize(ref array3, 4);
                list.AddRange(array3.Reverse());
            }
            arrBytes = list.ToArray();
        }

        private void FormatCommentToByte(FileStream dicVectorLineNumberComment, out byte[] arrBytes)
        {
            dicVectorLineNumberComment.Position = 0L;
            byte[] array = new byte[dicVectorLineNumberComment.Length];
            dicVectorLineNumberComment.Read(array, 0, array.Length);
            dicVectorLineNumberComment.Flush();
            List<byte> list = new List<byte>();
            list.AddRange(array);
            arrBytes = list.ToArray();
        }

        private void RecalculateDataToWrite(MemoryStream memoryStreamHead, int labelsByteCount, int commentByteCount, int useLabelsByteCount, out byte[] arrEachBlockLength)
        {
            arrEachBlockLength = new byte[512];
            byte[] array = BitConverter.GetBytes((int)memoryStreamHead.Length);
            Array.Resize(ref array, 4);
            array = array.Reverse().ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                arrEachBlockLength[i] = array[i];
            }
            byte[] array2 = BitConverter.GetBytes(labelsByteCount);
            Array.Resize(ref array2, 4);
            array2 = array2.Reverse().ToArray();
            for (int j = 0; j < array2.Length; j++)
            {
                arrEachBlockLength[j + 4] = array2[j];
            }
            byte[] array3 = BitConverter.GetBytes(commentByteCount);
            Array.Resize(ref array3, 4);
            array3 = array3.Reverse().ToArray();
            for (int k = 0; k < array3.Length; k++)
            {
                arrEachBlockLength[k + 8] = array3[k];
            }
            byte[] array4 = BitConverter.GetBytes(useLabelsByteCount);
            Array.Resize(ref array4, 4);
            array4 = array4.Reverse().ToArray();
            for (int l = 0; l < array4.Length; l++)
            {
                arrEachBlockLength[l + 12] = array4[l];
            }
            byte[] array5 = BitConverter.GetBytes((int)memoryStreamHead.Length + labelsByteCount + commentByteCount + useLabelsByteCount + arrEachBlockLength.Length);
            Array.Resize(ref array5, 4);
            array5 = array5.Reverse().ToArray();
            memoryStreamHead.Position = 0L;
            memoryStreamHead.Write(array5, 0, array5.Length);
            memoryStreamHead.Position = memoryStreamHead.Length;
        }

        private void AnalysisTestPlanPins(string pathTestPlan, string sheetName, Dictionary<string, List<string>> _listTestPlanPins)
        {
            _listTestPlanPins.Clear();


        }

        private void AnalysisPatternTimeSet(string pathPattern, Dictionary<int, string> patternTimeSetDic)
        {
            patternTimeSetDic.Clear();
            using StreamReader streamReader = new StreamReader(pathPattern);
            string input;
            do
            {
                if (!streamReader.EndOfStream)
                {
                    input = streamReader.ReadLine().Trim();
                    continue;
                }
                _compileError = true;

                return;
            } while (!_timeSetRegex.IsMatch(input));
            string[] array = _timeSetRegex.Match(input).Groups[1].Value.Split(new string[3] { ",", "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < array.Length; i++)
            {
                if (patternTimeSetDic.Values.Contains(array[i].Trim()))
                {
                    _compileError = true;
                }
                patternTimeSetDic.Add(i, array[i].Trim());
            }
        }

        private void AnalysisPatternDigitalInstrument(string pathPattern, out string strDigitalInstrument)
        {
            strDigitalInstrument = string.Empty;
            using StreamReader streamReader = new StreamReader(pathPattern);
            string input;
            do
            {
                if (!streamReader.EndOfStream)
                {
                    input = streamReader.ReadLine().Trim();
                    if (_digitalInstrumentRegex.IsMatch(input))
                    {
                        strDigitalInstrument = _digitalInstrumentRegex.Match(input).Groups[1].Value;
                        break;
                    }
                    continue;
                }
                break;
            }
            while (!_aTPPinsRegex.IsMatch(input));
        }

        //private void AnalysisPatternOPCodeMode(string pathPattern, out string strOPCodeMode)
        //{
        //    strOPCodeMode = string.Empty;
        //    using StreamReader streamReader = new StreamReader(pathPattern);
        //    string input;
        //    do
        //    {
        //        if (!streamReader.EndOfStream)
        //        {
        //            input = streamReader.ReadLine().Trim();
        //            if (_o.IsMatch(input))
        //            {
        //                strOPCodeMode = regex_OPCodeMode.Match(input).Groups[1].Value;
        //                break;
        //            }
        //            continue;
        //        }
        //        PrintCompileInfo("Information: 'opcode_mode' doesn't exist in atp. Please check keyword 'opcode_mode' in atp file, And it must be ';' At the end.");
        //        break;
        //    }
        //    while (!regex_ATPPins.IsMatch(input));
        //}

        private void AnalysisPatternPins(string pathPattern, List<string> patternPins, List<string> patternPinsWithDigitalMode, ref string memoryName, ref string vectorName)
        {
            patternPins.Clear();
            patternPinsWithDigitalMode.Clear();
            using StreamReader streamReader = new StreamReader(pathPattern);
            string empty = string.Empty;
            string text = string.Empty;
            string empty2 = string.Empty;
            do
            {
                if (!streamReader.EndOfStream)
                {
                    empty = text;
                    do
                    {
                        text = streamReader.ReadLine().Trim().RemoveNode();
                    }
                    while (text.Length == 0);
                    empty2 = empty + " " + text;
                    //if (this.eventPinAnalysis != null)
                    //{
                    //    empty2 = this.eventPinAnalysis(empty2, Path.GetFileNameWithoutExtension(path_Pattern));
                    //}
                    continue;
                }
                _compileError = true;
                //if (this.eventPrintCompileInfo != null)
                //{
                //    this.eventPrintCompileInfo("error PCE1002: Can not find Pins info. Please check keyword '$tset,' in atp file,");
                //    this.eventPrintCompileInfo("               and use english ',' to separate elements.");
                //}
                return;
            }
            while (!_aTPPinsRegex.IsMatch(empty2));
            Match match = _aTPPinsRegex.Match(empty2);
            memoryName = match.Groups[1].Value.Trim();
            vectorName = match.Groups[2].Value.Trim();
            string[] array = match.Groups[3].Value.Split(new string[3] { ",", "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i].Replace("(", ":").Replace(")", "");
                patternPinsWithDigitalMode.Add(array[i]);
                string text2 = array[i].Split(':')[0];
                if (patternPins.Contains(text2))
                {
                    _compileError = true;
                    //if (this.eventPrintCompileInfo != null)
                    //{
                    //    this.eventPrintCompileInfo("error PCE1021: The PinName list contains the same name " + text2 + ".");
                    //}
                }
                patternPins.Add(text2);
            }
            switch (memoryName.ToLower())
            {
                default:
                    _compileError = true;
                    //this.eventPrintCompileInfo?.Invoke("error PCE1029: Unrecognized module type " + memoryName + ".");
                    break;
                case "srm_vector":
                    moduleType = ModuleType.SRM_Vector;
                    break;
                case "lvm_vector":
                    moduleType = ModuleType.LVM_Vector;
                    break;
                case "vm_vector":
                    moduleType = ModuleType.VM_Vector;
                    break;
            }
        }

        private void UpdateParametersByModuleType(ModuleType moduleType)
        {
            switch (this.moduleType)
            {
                case ModuleType.VM_Vector:
                    _pseudoInstruDic = VM_PSEUDO_INSTRU;
                    _pinValueNameExchangeDic = VM_PIN_VALUENAME_EXCHANGE;
                    break;
                case ModuleType.LVM_Vector:
                    _pseudoInstruDic = LVM_PSEUDO_INSTRU;
                    _pinValueNameExchangeDic = LVM_PIN_VALUENAME_EXCHANGE;
                    _listTotalMaskCC = LVM_MASKCC;
                    break;
                case ModuleType.SRM_Vector:
                    _srmModifilerDic = SRM_Modifiler_strPseudo.Concat(SRM_Modifiler_ConditionFlag).Concat(SRM_Modifiler_Enable_And).ToDictionary((KeyValuePair<string, int> x) => x.Key, (KeyValuePair<string, int> y) => y.Value);
                    _pseudoInstruDic = SRM_PSEUDO_INSTRU;
                    _pinValueNameExchangeDic = SRM_PIN_VALUENAME_EXCHANGE;
                    _listTotalMaskCC = SRM_MASKCC;
                    _dicMTE = _dicDataGenerator.Concat(_dicMisc).Concat(_dicXAddressGenerator).Concat(_dicYAddressGenerator)
                        .Concat(_dicZAddressGenerator)
                        .ToDictionary((KeyValuePair<string, Dictionary<string, int>> x) => x.Key, (KeyValuePair<string, Dictionary<string, int>> y) => y.Value);
                    AnalysisPatternTotalLabels(_pathPattern, out _dicPretreatmentLabelNameVectorLineNumber);
                    break;
            }
        }

        private void AnalysisPatternTotalLabels(string pathPattern, out Dictionary<string, int> dicLabelVectorLineNumber)
        {
            dicLabelVectorLineNumber = new Dictionary<string, int>();
            using StreamReader streamReader = new StreamReader(pathPattern);
            long num = 1L;
            long num2 = 0L;
            while (!streamReader.EndOfStream && !_aTPPinsRegex.IsMatch(streamReader.ReadLine().Trim()))
            {
                num++;
            }
            string text = string.Empty;
            string text2 = string.Empty;
            while (!streamReader.EndOfStream)
            {
                streamReader.ReadLine().SplitValidAndComment(out var valid, out var comment);
                text = text + " " + valid;
                text2 = text2 + " " + comment;
                num++;
                if (!_vectorRegex.IsMatch(text))
                {
                    continue;
                }
                new List<byte>();
                string text3 = _vectorRegex.Match(text).Groups[1].Value.Replace(":", "");
                if (text3.Where((char x) => x == ':').Count() <= 1)
                {
                    if (text3.Trim().Length != 0)
                    {
                        if (!dicLabelVectorLineNumber.ContainsKey(text3))
                        {
                            dicLabelVectorLineNumber.Add(text3, (int)num2);
                        }
                        else
                        {
                            _compileError = true;
                            //this.eventPrintCompileInfo?.Invoke($"Error PCE1032: Line {num} have same label {text3}.");
                        }
                    }
                    text = string.Empty;
                    text2 = string.Empty;
                    num2++;
                    if (_compileError)
                    {
                        break;
                    }
                    continue;
                }
                _compileError = true;
                //this.eventPrintCompileInfo?.Invoke($"line {num} - error PCE1035: A single vector line cannot contain more than one label.");
                break;
            }
        }

        private void CheckPatternPinsRelationTestPlanSplitPinGroup(List<string> atpPinsPinGroupsWithDigitalMode, Dictionary<string, List<string>> testPlanPins, out List<string> pins)
        {
            pins = new List<string>();
            foreach (string item2 in atpPinsPinGroupsWithDigitalMode)
            {
                string strPinPinGroupName = string.Empty;
                string strPinPinGroupDigitalMode = string.Empty;
                if (item2.Contains(":"))
                {
                    string[] array = item2.Split(':');
                    strPinPinGroupName = array[0];
                    strPinPinGroupDigitalMode = array[1];
                }
                else
                {
                    strPinPinGroupName = item2;
                    strPinPinGroupDigitalMode = string.Empty;
                }
                if (testPlanPins.ContainsKey("Pins") && testPlanPins["Pins"].Contains(strPinPinGroupName))
                {
                    string item = ((strPinPinGroupDigitalMode == string.Empty) ? strPinPinGroupName : (strPinPinGroupName + ":" + strPinPinGroupDigitalMode));
                    pins.Add(item);
                }
                else if (testPlanPins.Keys.Contains(strPinPinGroupName))
                {
                    List<string> collection = testPlanPins[strPinPinGroupName].Select(delegate (string x)
                    {
                        string text = strPinPinGroupName + "-" + x;
                        return (!(strPinPinGroupDigitalMode == string.Empty)) ? (text + ":" + strPinPinGroupDigitalMode) : text;
                    }).ToList();
                    pins.AddRange(collection);
                }
                else
                {
                    _compileError = true;
                    //if (this.eventPrintCompileInfo != null)
                    //{
                    //    this.eventPrintCompileInfo($"error PCE2001: Can not find pin '{item2}' in TestPlan. Please check TestPlan file.");
                    //}
                }
            }
        }

        private void AnalysisPatternInstrument(string pathPattern, List<string> atpPinsPinGroups, Dictionary<string, List<string>> testPlanPins, List<Instrument> patternInstrument)
        {
            using StreamReader streamReader = new StreamReader(pathPattern);
            string text;
            do
            {
                if (!streamReader.EndOfStream)
                {
                    text = streamReader.ReadLine().RemoveNode().Trim();
                    if (!_instrumentsRegex.IsMatch(text))
                    {
                        continue;
                    }
                    string text2 = text;
                    while (true)
                    {
                        if (!text2.Contains("}"))
                        {
                            if (_aTPPinsRegex.IsMatch(text2))
                            {
                                break;
                            }
                            text2 += streamReader.ReadLine().RemoveNode().Trim();
                            continue;
                        }
                        int num = text2.IndexOf("{");
                        int num2 = text2.IndexOf("}");
                        if (num >= 0 && num2 >= 0 && num <= num2)
                        {
                            string[] array = text2.Substring(num + 1, num2 - num - 1).Split(new string[1] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < array.Length; i++)
                            {
                                Instrument instrumentInstance = GetInstrumentInstance(array[i].Trim(), testPlanPins);
                                if (instrumentInstance != null)
                                {
                                    FillRegularExpressionForPinPinGroup(instrumentInstance, atpPinsPinGroups);
                                    patternInstrument.Add(instrumentInstance);
                                }
                            }
                        }
                        else
                        {
                            _compileError = true;
                            //if (this.eventPrintCompileInfo != null)
                            //{
                            //    this.eventPrintCompileInfo($"error PCE1020: Cannot recognize symbol '{{' and '}}' in the Instruments string '{text2}', cannot format it to Instrument object.");
                            //}
                        }
                        return;
                    }
                    _compileError = true;
                    //if (this.eventPrintCompileInfo != null)
                    //{
                    //    this.eventPrintCompileInfo("error PCE1017: No symbol '}' was found to match instruments module.");
                    //}
                    break;
                }
                //PrintCompileInfo("Information: 'instruments' doesn't exist in atp.");
                break;
            }
            while (!_aTPPinsRegex.IsMatch(text));
        }

        private Instrument GetInstrumentInstance(string strInstrument, Dictionary<string, List<string>> testPlanPins)
        {
            Instrument result = null;
            string[] array = strInstrument.Split(new string[2] { ":", " " }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length != 6)
            {
                _compileError = true;
                //if (this.eventPrintCompileInfo != null)
                //{
                //    this.eventPrintCompileInfo($"error PCE1018: The number of elements in the '{strInstrument}' in Instruments module is incorrec. Please check it.");
                //}
                return result;
            }
            result = new Instrument();
            result.StrInstrument = strInstrument;
            string[] array2 = array[0].Replace("(", "").Replace(")", "").Split(new string[3] { ",", " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < array2.Length; i++)
            {
                if (testPlanPins["Pins"].Contains(array2[i]))
                {
                    result.DicPinItem.Add(array2[i], new List<string> { array2[i] });
                    continue;
                }
                if (testPlanPins.Keys.Contains(array2[i]))
                {
                    result.DicPinItem.Add(array2[i], testPlanPins[array2[i]]);
                    continue;
                }
                _compileError = true;
                //if (this.eventPrintCompileInfo != null)
                //{
                //    this.eventPrintCompileInfo($"error PCE1015: Can not find Instrument pin '{array2[i]}' in TestPlan.");
                //}
            }
            result.DigitalMode = array[1];
            if (!int.TryParse(array[2], out var result2))
            {
                _compileError = true;
                //if (this.eventPrintCompileInfo != null)
                //{
                //    this.eventPrintCompileInfo($"error PCE1023: Unable to identify the Instrument Width value '{array[2]}', it should be the number.");
                //}
            }
            else
            {
                if (result2 < 1 || result2 > 32)
                {
                    _compileError = true;
                    //if (this.eventPrintCompileInfo != null)
                    //{
                    //    this.eventPrintCompileInfo($"error PCE1024: The Instrument Width ranges from 1 to 32. Current instrument width is {result2}.");
                    //}
                }
                if (array[3].ToLower() == "parallel" && result2 != 1)
                {
                    _compileError = true;
                    //if (this.eventPrintCompileInfo != null)
                    //{
                    //    this.eventPrintCompileInfo($"error PCE1025: Instrument width must be 1 when the Instrument mode is Parallel. Current instrument width is {result2}");
                    //}
                }
                result.InstrumentWidth = result2;
            }
            result.InstrumentMode = array[3];
            result.BitOrder = array[4];
            result.Format = array[5];
            return result;
        }

        private void FillRegularExpressionForPinPinGroup(Instrument instrument, List<string> atpPinsPinGroups)
        {
            string text = string.Empty;
            if (instrument.DigitalMode.ToLower() == "digsrc")
            {
                text = "D";
            }
            else if (instrument.DigitalMode.ToLower() == "digcap")
            {
                text = "V";
            }
            List<string> list = new List<string>();
            for (int i = 0; i < atpPinsPinGroups.Count; i++)
            {
                if (instrument.DicPinItem.Keys.Contains(atpPinsPinGroups[i]))
                {
                    int count = instrument.DicPinItem[atpPinsPinGroups[i]].Count;
                    list.Add(text + "{" + count + "}");
                }
                else
                {
                    list.Add("[01LHMXDCV]+");
                }
            }
            string pattern = string.Join("\\s+", list) + "\\s*";
            instrument.RegularExpression = new Regex(pattern, RegexOptions.IgnoreCase);
        }

        private void GetInstrumentsInfoFromDataBlock(string pathPattern, List<string> atpPinsPinGroups, List<Instrument> instruments, out List<string> dataBlockMarkerParameter)
        {
            int num = 1;
            bool flag = true;
            List<string> list = new List<string>();
            dataBlockMarkerParameter = new List<string>();
            GroupInstrumentBySrcAndCap(instruments, out var listSrcInstrument, out var listCapInstrument);
            List<int> normalPinIndex = GetNormalPinIndex(atpPinsPinGroups, listSrcInstrument, listCapInstrument);
            using StreamReader streamReader = new StreamReader(pathPattern);
            while (!streamReader.EndOfStream && !_aTPPinsRegex.IsMatch(streamReader.ReadLine().Trim()))
            {
                num++;
            }
            string text = string.Empty;
            string text2 = string.Empty;
            while (!streamReader.EndOfStream)
            {
                streamReader.ReadLine().SplitValidAndComment(out var valid, out var comment);
                text = text + " " + valid;
                text2 = text2 + " " + comment;
                num++;
                if (!_vectorRegex.IsMatch(text))
                {
                    continue;
                }
                list.Add(text);
                if (_commandLoopRegex.IsMatch(text))
                {
                    Match match = _commandLoopRegex.Match(text);
                    string text3 = "Loop" + match.Groups[1].Value;
                    int.Parse(match.Groups[2].Value);
                    _tempNestLoopOutermostLoopName = ((_tempNestLoopOutermostLoopName.Length == 0) ? text3 : _tempNestLoopOutermostLoopName);
                    flag = false;
                }
                else if (_commandEndLoopRegex.IsMatch(text))
                {
                    string value = _commandEndLoopRegex.Match(text).Groups[1].Value;
                    if (_tempNestLoopOutermostLoopName == "Loop" + value)
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    ProgramOneRowVector(num, list, listSrcInstrument, listCapInstrument, atpPinsPinGroups, dataBlockMarkerParameter, normalPinIndex);
                    list.Clear();
                    _tempNestLoopOutermostLoopName = string.Empty;
                }
                text = string.Empty;
                text2 = string.Empty;
                if (_compileError)
                {
                    break;
                }
            }
        }

        private void GroupInstrumentBySrcAndCap(List<Instrument> instruments, out List<Instrument> listSrcInstrument, out List<Instrument> listCapInstrument)
        {
            listSrcInstrument = new List<Instrument>();
            listCapInstrument = new List<Instrument>();
            listSrcInstrument = instruments.Where((Instrument x) => x.DigitalMode.ToLower() == "digsrc").ToList();
            listCapInstrument = instruments.Where((Instrument x) => x.DigitalMode.ToLower() == "digcap").ToList();
        }

        private List<int> GetNormalPinIndex(List<string> atpPinsPinGroups, List<Instrument> listSrcInstrument, List<Instrument> listCapInstrument)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < listSrcInstrument.Count; i++)
            {
                List<int> collection = listSrcInstrument[i].DicPinItem.Keys.Select((string x) => atpPinsPinGroups.IndexOf(x)).ToList();
                list.AddRange(collection);
            }
            for (int j = 0; j < listSrcInstrument.Count; j++)
            {
                List<int> collection2 = listSrcInstrument[j].DicPinItem.Keys.Select((string x) => atpPinsPinGroups.IndexOf(x)).ToList();
                list.AddRange(collection2);
            }
            List<int> list2 = new List<int>();
            for (int k = 0; k < atpPinsPinGroups.Count; k++)
            {
                if (!list.Contains(k))
                {
                    list2.Add(k);
                }
            }
            return list2;
        }

        private void ProgramOneRowVector(int rowNumber, List<string> listRowVector, List<Instrument> listSrcInstrument, List<Instrument> listCapInstrument, List<string> atpPinsPinGroups, List<string> listDataBlockMarkerParameter, List<int> normalPinIndex)
        {
            _nestLoopBuffDic = new Dictionary<string, int>();
            _nestLoopIndex = -1;
            int num = 0;
            while (true)
            {
                if (num >= listRowVector.Count)
                {
                    return;
                }
                int num2 = 1;
                Match match = _vectorRegex.Match(listRowVector[num]);
                string value = match.Groups[1].Value;
                string strPseudo = string.Empty;
                string strPseudoParameter = string.Empty;
                _ = match.Groups[5].Value;
                string value2 = match.Groups[6].Value;
                string value3 = match.Groups[7].Value;
                List<string> list = (from y in match.Groups[4].Value.Trim().Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                     select y.Trim()).ToList();
                List<string> list2 = (from x in list
                                      where _listTotalMaskCC.Contains(x.Trim().ToLower())
                                      select x into y
                                      select y.Trim()).ToList();
                for (int i = 0; i < list2.Count; i++)
                {
                    if (list.Contains(list2[i]))
                    {
                        list.Remove(list2[i]);
                    }
                }
                if (value.Where((char x) => x == ':').Count() <= 1)
                {
                    if (list.Count <= 1)
                    {
                        string text = ((list.Count == 1) ? list[0] : string.Empty);
                        switch (moduleType)
                        {
                            case ModuleType.SRM_Vector:
                                if (!GetPseudoWithParameterSRM(text, out strPseudo, out strPseudoParameter))
                                {
                                    _compileError = true;
                                    //this.eventPrintCompileInfo?.Invoke($"line {rowNumber} - error PCE1028: The number of pseudo instructions and parameters is greater than 2. Content is {text}");
                                }
                                break;
                            case ModuleType.VM_Vector:
                            case ModuleType.LVM_Vector:
                                if (!GetPseudoWithParameter(text, out strPseudo, out strPseudoParameter))
                                {
                                    _compileError = true;
                                    //this.eventPrintCompileInfo?.Invoke($"line {rowNumber} - error PCE1028: The number of pseudo instructions and parameters is greater than 2. Content is {text}");
                                }
                                break;
                        }
                        if (value3.Replace(" ", "").Replace("\t", "").Length != 0)
                        {
                            _compileError = true;
                            //if (this.eventPrintCompileInfo != null)
                            //{
                            //    this.eventPrintCompileInfo($"line {rowNumber - (listRowVector.Count - 1 - num)} - error PCE1010: Comment should start with '//'.");
                            //}
                        }
                        string[] array = value2.Split(new string[2] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                        if (array.Length != atpPinsPinGroups.Count)
                        {
                            break;
                        }
                        if (array.Contains("D") || array.Contains("V"))
                        {
                            for (int j = 0; j < normalPinIndex.Count; j++)
                            {
                                if (array[normalPinIndex[j]].Contains("D") || array[normalPinIndex[j]].Contains("V"))
                                {
                                    _compileError = true;
                                    //if (this.eventPrintCompileInfo != null)
                                    //{
                                    //    this.eventPrintCompileInfo($"line {rowNumber - (listRowVector.Count - 1 - num)} - error PCE1012: The V/D character appears in undefined Instrument.");
                                    //}
                                }
                            }
                        }
                        if (strPseudo.ToLower() == "start")
                        {
                            _recordingSwitchStart = true;
                            _dataBlockIndexStart++;
                            if (!listDataBlockMarkerParameter.Contains(strPseudoParameter))
                            {
                                listDataBlockMarkerParameter.Add(strPseudoParameter);
                            }
                            _ignoreCurrentVectorRowStart = true;
                            if (value2.ToUpper().Contains("D"))
                            {
                                //this.eventPrintCompileInfo($"line {rowNumber - (listRowVector.Count - 1 - num)} - warn PCW1001: The 'Start' pseudoinstruction should not contain the 'D' on the line. This row will be ignored and will not be counted in the number of rows containing D.");
                            }
                        }
                        else if (strPseudo.ToLower() == "trig")
                        {
                            _recordingSwitchTrig = true;
                            _dataBlockIndexTrig++;
                            _ignoreCurrentVectorRowTrig = true;
                            if (value2.ToUpper().Contains("V"))
                            {
                                //this.eventPrintCompileInfo($"line {rowNumber - (listRowVector.Count - 1 - num)} - warn PCW1002: The 'Trig' pseudoinstruction should not contain the 'V' on the line. This row will be ignored and will not be counted in the number of rows containing V.");
                            }
                        }
                        else if (strPseudo.ToLower() == "repeat")
                        {
                            if (!int.TryParse(strPseudoParameter, out var result))
                            {
                                _compileError = true;
                                //if (this.eventPrintCompileInfo != null)
                                //{
                                //    this.eventPrintCompileInfo($"line {rowNumber - (listRowVector.Count - 1 - num)} - error PCW1003: Can't convert repeat parameter '{strPseudoParameter}' to a number.");
                                //}
                            }
                            num2 = result;
                        }
                        else if (strPseudo.ToLower() == "loopa" || strPseudo.ToLower() == "loopb" || strPseudo.ToLower() == "loopc")
                        {
                            if (!int.TryParse(strPseudoParameter, out var result2))
                            {
                                _compileError = true;
                                //if (this.eventPrintCompileInfo != null)
                                //{
                                //    this.eventPrintCompileInfo($"line {rowNumber - (listRowVector.Count - 1 - num)} - error PCW1004: Can't convert loop parameter '{strPseudoParameter}' to a number.");
                                //}
                            }
                            _nestLoopBuffDic.Add(strPseudo.ToLower(), result2);
                            _nestLoopIndex++;
                        }
                        if (_recordingSwitchStart && !_ignoreCurrentVectorRowStart)
                        {
                            foreach (Instrument item in listSrcInstrument)
                            {
                                if (item.RegularExpression.IsMatch(value2))
                                {
                                    int num3 = 1;
                                    for (int num4 = _nestLoopIndex; num4 >= 0; num4--)
                                    {
                                        num3 *= _nestLoopBuffDic.ElementAt(num4).Value;
                                    }
                                    if (item.StartTrigBlockIndexAndCount.Keys.Contains(_dataBlockIndexStart))
                                    {
                                        item.StartTrigBlockIndexAndCount[_dataBlockIndexStart] += num3 * num2;
                                    }
                                    else
                                    {
                                        item.StartTrigBlockIndexAndCount.Add(_dataBlockIndexStart, num3 * num2);
                                    }
                                    continue;
                                }
                                List<int> pinsIndex = GetPinsIndex(item.DicPinItem, atpPinsPinGroups);
                                for (int k = 0; k < pinsIndex.Count; k++)
                                {
                                    if (array[pinsIndex[k]].Contains("D"))
                                    {
                                        _compileError = true;
                                        //if (this.eventPrintCompileInfo != null)
                                        //{
                                        //    this.eventPrintCompileInfo(string.Format("line {0} - error PCE1013: All pins in current instrument must be '{1}' at the same time.", rowNumber - (listRowVector.Count - 1 - num), "D"));
                                        //}
                                    }
                                }
                            }
                        }
                        _ignoreCurrentVectorRowStart = false;
                        if (_recordingSwitchTrig && !_ignoreCurrentVectorRowTrig)
                        {
                            foreach (Instrument item2 in listCapInstrument)
                            {
                                if (item2.RegularExpression.IsMatch(value2))
                                {
                                    int num5 = 1;
                                    for (int num6 = _nestLoopIndex; num6 >= 0; num6--)
                                    {
                                        num5 *= _nestLoopBuffDic.ElementAt(num6).Value;
                                    }
                                    if (item2.StartTrigBlockIndexAndCount.Keys.Contains(_dataBlockIndexTrig))
                                    {
                                        item2.StartTrigBlockIndexAndCount[_dataBlockIndexTrig] += num5 * num2;
                                    }
                                    else
                                    {
                                        item2.StartTrigBlockIndexAndCount.Add(_dataBlockIndexTrig, num5 * num2);
                                    }
                                    continue;
                                }
                                List<int> pinsIndex2 = GetPinsIndex(item2.DicPinItem, atpPinsPinGroups);
                                for (int l = 0; l < pinsIndex2.Count; l++)
                                {
                                    if (array[pinsIndex2[l]].Contains("V"))
                                    {
                                        _compileError = true;
                                        //if (this.eventPrintCompileInfo != null)
                                        //{
                                        //    this.eventPrintCompileInfo(string.Format("line {0} - error PCE1014: All pins in current instrument must be '{1}' at the same time.", rowNumber - (listRowVector.Count - 1 - num), "V"));
                                        //}
                                    }
                                }
                            }
                        }
                        _ignoreCurrentVectorRowTrig = false;
                        if (strPseudo.ToLower() == "end_loopa" || strPseudo.ToLower() == "end_loopb" || strPseudo.ToLower() == "end_loopc")
                        {
                            _nestLoopIndex--;
                        }
                        num++;
                        continue;
                    }
                    _compileError = true;
                    //this.eventPrintCompileInfo?.Invoke($"line {rowNumber} - error PCE1030: Unrecognized vector content. Cannot contain mutli pseudo.");
                    return;
                }
                _compileError = true;
                //this.eventPrintCompileInfo?.Invoke($"line {rowNumber} - error PCE1035: A single vector line cannot contain more than one label.");
                return;
            }
            _compileError = true;
            //if (this.eventPrintCompileInfo != null)
            //{
            //    this.eventPrintCompileInfo($"line {rowNumber - (listRowVector.Count - 1 - num)} - error PCE1009: Vector pins count is not match defined pins count in atp file.");
            //}
        }

        private List<int> GetPinsIndex(Dictionary<string, List<string>> dicPinItem, List<string> atpPinsPinGroups)
        {
            return dicPinItem.Keys.Select((string x) => atpPinsPinGroups.IndexOf(x)).ToList();
        }

        private bool GetPseudoWithParameterSRM(string uCode, out string strPseudo, out string strPseudoParameter)
        {
            strPseudo = string.Empty;
            strPseudoParameter = string.Empty;
            if (_srmuCodeAnalysisRegex.IsMatch(uCode))
            {
                Match match = _srmuCodeAnalysisRegex.Match(uCode);
                strPseudo = match.Groups[3].Value.Trim();
                strPseudoParameter = match.Groups[5].Value.Trim();
                return true;
            }
            return false;
        }


        private bool GetPseudoWithParameter(string uCode, out string strPseudo, out string strPseudoParameter)
        {
            List<string> list = uCode.Split(new string[2] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            bool flag = false;
            switch (list.Count)
            {
                default:
                    strPseudo = string.Empty;
                    strPseudoParameter = string.Empty;
                    return false;
                case 0:
                    strPseudo = string.Empty;
                    strPseudoParameter = string.Empty;
                    return true;
                case 1:
                    strPseudo = list[0];
                    strPseudoParameter = string.Empty;
                    return true;
                case 2:
                    strPseudo = list[0];
                    strPseudoParameter = list[1];
                    return true;
            }
        }

    }
}
