using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace lr5
{
    /// <summary>
    /// Логика взаимодействия для order_win.xaml
    /// </summary>
    public partial class order_win : Window
    {
        private const string ConnectionString =
            "Host=localhost;Database=confectionery;Port=5432;Username=postgres;Password=admin";
        
        public order_win()
        {
            InitializeComponent();
            LoadOrderData();
        }

        private void b_menu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
        private void LoadOrderData()
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM \"order\"";

                    using (var adapter = new NpgsqlDataAdapter(sql, connection))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dg.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void b_del_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dg.SelectedItem;

            if (selectedRow != null)
            {
                try
                {
                    using (var connection = new NpgsqlConnection(ConnectionString))
                    {
                        connection.Open();
                        int orderId = Convert.ToInt32(selectedRow["id"]);
                        string deleteSql = $"DELETE FROM \"order\" WHERE id = {orderId}";

                        using (var command = new NpgsqlCommand(deleteSql, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        LoadOrderData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении записи: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления.");
            }
        }

        private void b_save_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dg.SelectedItem;

            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString))
                {
                    connection.Open();

                    if (selectedRow != null)
                    {
                        int orderId = Convert.ToInt32(selectedRow["id"]);
                        string updateSql = "UPDATE \"order\" SET \"date of registration\" = @newDateRegistration, \"date of issue\" = @newDateIssue, customer = @newCustomerId WHERE id = @orderId";

                        using (var command = new NpgsqlCommand(updateSql, connection))
                        {
                            command.Parameters.AddWithValue("@newDateRegistration", selectedRow["date of registration"]);
                            command.Parameters.AddWithValue("@newDateIssue", selectedRow["date of issue"]);
                            command.Parameters.AddWithValue("@newCustomerId", selectedRow["customer"]);
                            command.Parameters.AddWithValue("@orderId", orderId);

                            command.ExecuteNonQuery();
                        }
                        LoadOrderData();
                    }
                    else
                    {
                        MessageBox.Show("Выберите запись для редактирования.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}");
            }

        }

        private void b_add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString))
                {
                    connection.Open();

                    string insertSql = "INSERT INTO \"order\" (\"date of registration\", \"date of issue\", customer) VALUES (@newDateRegistration, @newDateIssue, @newCustomerId) RETURNING id";

                    using (var command = new NpgsqlCommand(insertSql, connection))
                    {
                        command.Parameters.AddWithValue("@newDateRegistration", DateTime.Now); 
                        command.Parameters.AddWithValue("@newDateIssue", DateTime.Now.AddDays(3)); 
                        command.Parameters.AddWithValue("@newCustomerId", DBNull.Value); 

                        int newId = Convert.ToInt32(command.ExecuteScalar());

                        LoadOrderData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении новой записи: {ex.Message}");
            }

        }
    }
}
