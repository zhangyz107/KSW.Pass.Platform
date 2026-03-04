using KSW.ATE01.Application.Models.Patterns;
using KSW.ATE01.Project.Base.Enums.Patterns;
using KSW.ATE01.Start.ViewModels.Patterns;
using KSW.Localization;
using KSW.Ui;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KSW.ATE01.Start.Views.Patterns
{
    /// <summary>
    /// PatternEditorView.xaml 的交互逻辑
    /// </summary>
    public partial class PatternEditorView : IView
    {
        private readonly ILanguageManager _language;
        private List<DataGridColumn> insertColumns = new List<DataGridColumn>();
        private string _dynamicColumnHeader;

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

        public PatternEditorView(IContainerProvider containerProvider)
        {
            InitializeComponent();

            _language = containerProvider.IsRegistered<ILanguageManager>() == true ? containerProvider.Resolve<ILanguageManager>() : null;
            if (_language != null)
            {
                _dynamicColumnHeader = _language["TimingName"];
            }

            if (DataContext is PatternEditorViewModel viewModel)
            {
                viewModel.PatternUpdated += (s, e) =>
                {
                    RefreshDataGrid(e);
                };
            }
        }

        private void RefreshDataGrid(IEnumerable<PatternVectorModel> vectors)
        {
            if (vectors.IsEmpty())
                return;

            var vectorRow = vectors.FirstOrDefault();
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
                    SelectedValueBinding = new Binding($"Pins[{index}].VectorValue") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, IsAsync = true },
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
