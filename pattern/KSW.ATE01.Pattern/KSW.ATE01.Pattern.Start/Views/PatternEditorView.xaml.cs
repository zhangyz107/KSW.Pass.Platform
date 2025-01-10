using KSW.ATE01.Pattern.Application.Events;
using KSW.ATE01.Pattern.Application.Models.Projects;
using KSW.ATE01.Pattern.Domain.Projects.Core.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace KSW.ATE01.Pattern.Start.Views
{
    /// <summary>
    /// PatternEditorView.xaml 的交互逻辑
    /// </summary>
    public partial class PatternEditorView : UserControl
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private List<DataGridColumn> insertColumns = new List<DataGridColumn>();
        private const string _dynamicColumnHeader = "Timing Name";
        #endregion

        #region Properties
        public Dictionary<VectorValueType, string> VectorValueDic => new Dictionary<VectorValueType, string>()
        {
            { VectorValueType.Zero, VectorValueType.Zero.Description() },
            { VectorValueType.One, VectorValueType.One.Description() },
            { VectorValueType.L, VectorValueType.L.Description() },
            { VectorValueType.H, VectorValueType.H.Description() },
            { VectorValueType.M, VectorValueType.M.Description() },
            { VectorValueType.X, VectorValueType.X.Description() },
            { VectorValueType.V, VectorValueType.V.Description() },
            { VectorValueType.None, VectorValueType.None.Description() }
        };

        #endregion

        public PatternEditorView(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<PatternColInfoUpdateEvent>().Subscribe(RefreshDataGrid);
        }

        private void RefreshDataGrid(PatternInfoModel model)
        {
            if (model == null)
                return;

            if (model.PinInfos.IsEmpty())
                return;

            CleanUpDynamicColumns();

            var lastHeaderName = _dynamicColumnHeader;
            foreach (var pinInfo in model.PinInfos)
            {
                var index = model.PinInfos.IndexOf(pinInfo);
                InsertColumnAfter(lastHeaderName, new DataGridComboBoxColumn()
                {
                    Header = pinInfo.PinName,
                    ItemsSource = VectorValueDic,
                    SelectedValuePath = "Key",
                    DisplayMemberPath = "Value",
                    SelectedValueBinding = new Binding($"PinInfos[{index}].VectorValue") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged },
                    HeaderStyle = FindResource("VerticalColHeader") as Style,
                    ElementStyle = FindResource("DiscolorationCombobox") as Style,
                    EditingElementStyle = FindResource("DiscolorationEditCombobox") as Style,
                });

                lastHeaderName = pinInfo.PinName;
            }
        }

        public void InsertColumnAfter(string headerName, DataGridColumn newColumn)
        {
            int index = dataGrid.Columns.IndexOf(dataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == headerName));

            if (index >= 0)
            {
                dataGrid.Columns.Insert(index + 1, newColumn);
            }
            else
            {
                dataGrid.Columns.Add(newColumn);  // 如果没有找到，直接添加到最后
            }

            insertColumns.Add(newColumn);
        }

        public void CleanUpDynamicColumns()
        {
            if (insertColumns.IsEmpty())
                return;
            foreach (var column in insertColumns)
            {
                if (dataGrid.Columns.Contains(column))
                    dataGrid.Columns.Remove(column);
            }

            insertColumns.Clear();

        }
    }
}
