using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using MySql.Data.MySqlClient;

namespace HardwareStore.ViewModels
{
    public class ReportsViewModel : INotifyPropertyChanged
    {
        private string connectionString = "server=localhost;uid=root;pwd=;database=pos_hardware_store";

        private DateTime _startDate = DateTime.Today.AddMonths(-1);

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged(nameof(StartDate));
                }
            }
        }

        private DateTime _endDate = DateTime.Today;
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged(nameof(EndDate));
                }
            }
        }

        public ObservableCollection<string> ReportTypes { get; } = new ObservableCollection<string>()
        {
            "Sales Report",
            "Inventory Report",
            "Low Stock Report"
        };

        private string _selectedReportType;
        public string SelectedReportType
        {
            get => _selectedReportType;
            set
            {
                if (_selectedReportType != value)
                {
                    _selectedReportType = value;
                    OnPropertyChanged(nameof(SelectedReportType));
                }
            }
        }

        private DataTable _reportData = new DataTable();
        public DataTable ReportData
        {
            get => _reportData;
            set
            {
                if (_reportData != value)
                {
                    _reportData = value;
                    OnPropertyChanged(nameof(ReportData));
                }
            }
        }

        public ICommand LoadReportCommand { get; }
        public ICommand ExportCsvCommand { get; }
        public ICommand PrintReportCommand { get; }

        public ReportsViewModel()
        {
            SelectedReportType = ReportTypes[0];

            LoadReportCommand = new RelayCommand(LoadReport);
            ExportCsvCommand = new RelayCommand(ExportCsv, CanExportOrPrint);
            PrintReportCommand = new RelayCommand(PrintReport, CanExportOrPrint);
        }

        private void LoadReport()
        {
            var dt = new DataTable();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                if (SelectedReportType == "Sales Report")
                {
                    using (MySqlCommand cmd = new MySqlCommand(@"
                        SELECT Id AS 'Sale ID', CreatedAt AS 'Date', Total AS 'Total Amount'
                        FROM Sales
                        WHERE CreatedAt BETWEEN @start AND @end
                        ORDER BY CreatedAt DESC", conn))
                    {
                        cmd.Parameters.AddWithValue("@start", StartDate);
                        cmd.Parameters.AddWithValue("@end", EndDate);

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
                else if (SelectedReportType == "Inventory Report")
                {
                    using (MySqlCommand cmd = new MySqlCommand(@"
                        SELECT i.Name AS 'Item Name', c.Name AS 'Category', i.Stock
                        FROM Items i
                        JOIN Categories c ON i.CategoryId = c.Id
                        ORDER BY c.Name, i.Name", conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
                else if (SelectedReportType == "Low Stock Report")
                {
                    using (MySqlCommand cmd = new MySqlCommand(@"
                        SELECT Name AS 'Item Name', Stock
                        FROM Items
                        WHERE Stock <= 10
                        ORDER BY Stock ASC", conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }

            ReportData = dt;
        }

        private bool CanExportOrPrint() => ReportData != null && ReportData.Rows.Count > 0;

        private void ExportCsv()
        {
            if (ReportData == null || ReportData.Rows.Count == 0)
            {
                System.Windows.MessageBox.Show("No data to export.", "Export CSV", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = $"{SelectedReportType.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.csv"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(dialog.FileName))
                    {
                        // Write column headers
                        var columnNames = ReportData.Columns.Cast<DataColumn>().Select(col => Quote(col.ColumnName));
                        writer.WriteLine(string.Join(",", columnNames));

                        // Write rows
                        foreach (DataRow row in ReportData.Rows)
                        {
                            var fields = row.ItemArray.Select(field => Quote(field?.ToString() ?? ""));
                            writer.WriteLine(string.Join(",", fields));
                        }
                    }

                    System.Windows.MessageBox.Show("CSV exported successfully!", "Export CSV", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error exporting CSV:\n" + ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }

            string Quote(string input)
            {
                if (input.Contains(",") || input.Contains("\"") || input.Contains("\n"))
                {
                    input = input.Replace("\"", "\"\"");
                    return $"\"{input}\"";
                }
                return input;
            }
        }

        private void PrintReport()
        {
            if (ReportData == null || ReportData.Rows.Count == 0)
            {
                System.Windows.MessageBox.Show("No data to print.", "Print Report", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                return;
            }

            var doc = new System.Windows.Documents.FlowDocument
            {
                PagePadding = new System.Windows.Thickness(50),
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                FontSize = 12
            };

            doc.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run($"{SelectedReportType}"))
            {
                FontSize = 16,
                FontWeight = System.Windows.FontWeights.Bold,
                TextAlignment = System.Windows.TextAlignment.Center,
                Margin = new System.Windows.Thickness(0, 0, 0, 20)
            });

            var table = new System.Windows.Documents.Table();

            for (int i = 0; i < ReportData.Columns.Count; i++)
            {
                table.Columns.Add(new System.Windows.Documents.TableColumn());
            }

            var headerRow = new System.Windows.Documents.TableRow();
            foreach (DataColumn column in ReportData.Columns)
            {
                headerRow.Cells.Add(new System.Windows.Documents.TableCell(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(column.ColumnName)))
                {
                    FontWeight = System.Windows.FontWeights.Bold,
                    Padding = new System.Windows.Thickness(4),
                    BorderBrush = System.Windows.Media.Brushes.Gray,
                    BorderThickness = new System.Windows.Thickness(0.5)
                });
            }

            var rowGroup = new System.Windows.Documents.TableRowGroup();
            rowGroup.Rows.Add(headerRow);

            foreach (DataRow row in ReportData.Rows)
            {
                var tableRow = new System.Windows.Documents.TableRow();
                foreach (var item in row.ItemArray)
                {
                    tableRow.Cells.Add(new System.Windows.Documents.TableCell(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(item?.ToString())))
                    {
                        Padding = new System.Windows.Thickness(4),
                        BorderBrush = System.Windows.Media.Brushes.LightGray,
                        BorderThickness = new System.Windows.Thickness(0.25)
                    });
                }
                rowGroup.Rows.Add(tableRow);
            }

            table.RowGroups.Add(rowGroup);
            doc.Blocks.Add(table);

            var pd = new System.Windows.Controls.PrintDialog();
            if (pd.ShowDialog() == true)
            {
                doc.Name = "ReportDocument";
                pd.PrintDocument(((System.Windows.Documents.IDocumentPaginatorSource)doc).DocumentPaginator, $"{SelectedReportType} Print");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Simple RelayCommand implementation
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public void Execute(object parameter) => _execute();
    }
}
