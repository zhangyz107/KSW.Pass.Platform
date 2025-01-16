using KSW.ATE01.Pattern.Application.Events;
using KSW.ATE01.Pattern.Application.Models.Projects;
using KSW.ATE01.Pattern.Domain.Projects.Core.Enums;
using KSW.Localization;
using Prism.Ioc;
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
        private readonly IContainerProvider _containerProvider;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILanguageManager _language;
        private List<DataGridColumn> insertColumns = new List<DataGridColumn>();
        private string _dynamicColumnHeader;
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

        public PatternEditorView(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator)
        {
            InitializeComponent();

            _containerProvider = containerProvider;
            _eventAggregator = eventAggregator;
            _language = containerProvider.IsRegistered<ILanguageManager>() == true ? containerProvider.Resolve<ILanguageManager>() : null;

            if (_language != null)
            {
                _dynamicColumnHeader = _language["TimingName"];
            }
            _eventAggregator.GetEvent<PatternColInfoUpdateEvent>().Subscribe(RefreshDataGrid);
        }

        private void RefreshDataGrid(PatternModel model)
        {
            if (model == null)
                return;

            if (model.PatternVectors.IsEmpty())
                return;

            var vectorRow = model.PatternVectors.FirstOrDefault();
            if (vectorRow?.Pins?.IsEmpty() == true)
                return;

            CleanUpDynamicColumns();

            var lastHeaderName = _dynamicColumnHeader;
            foreach (var pinInfo in vectorRow?.Pins)
            {
                var index = vectorRow?.Pins.IndexOf(pinInfo);
                InsertColumnAfter(lastHeaderName, new DataGridComboBoxColumn()
                {
                    Header = pinInfo.PinName,
                    ItemsSource = VectorValueDic,
                    SelectedValuePath = "Key",
                    DisplayMemberPath = "Value",
                    SelectedValueBinding = new Binding($"Pins[{index}].VectorValue") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged },
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

        private void Export_Initialized(object sender, EventArgs e)
        {
            this.btnExport.ContextMenu = null;
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            this.btnContextMenu.PlacementTarget = this.btnExport;
            this.btnContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            this.btnContextMenu.IsOpen = true;
        }
    }
}
