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
    /// Логика взаимодействия для positions_win.xaml
    /// </summary>
    public partial class positions_win : Window
    {
        private const string ConnectionString =
            "Host=localhost;Database=confectionery;Port=5432;Username=postgres;Password=admin";

        public positions_win()
        {
            InitializeComponent();
            LoadPositionsData();
        }

        private void b_menu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void LoadPositionsData()
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM positions";

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
                        int positionsId = Convert.ToInt32(selectedRow["id"]);
                        string updateSql = "UPDATE positions SET \"order\" = @newOrderId, product = @newProductId, confectioner = @newConfectionerId, sum = @newSum WHERE id = @positionsId";

                        using (var command = new NpgsqlCommand(updateSql, connection))
                        {
                            command.Parameters.AddWithValue("@newOrderId", selectedRow["order"]);
                            command.Parameters.AddWithValue("@newProductId", selectedRow["product"]);
                            command.Parameters.AddWithValue("@newConfectionerId", selectedRow["confectioner"]);
                            command.Parameters.AddWithValue("@newSum", selectedRow["sum"]);
                            command.Parameters.AddWithValue("@positionsId", positionsId);

                            command.ExecuteNonQuery();
                        }
                        LoadPositionsData();
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
                        int positionId = Convert.ToInt32(selectedRow["id"]);
                        string deleteSql = $"DELETE FROM positions WHERE id = {positionId}";

                        using (var command = new NpgsqlCommand(deleteSql, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        LoadPositionsData();
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

        private void b_add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString))
                {
                    connection.Open();

                    string insertSql = "INSERT INTO positions (\"order\", product, confectioner, sum) VALUES (@newOrderId, @newProductId, @newConfectionerId, @newSum) RETURNING id";

                    using (var command = new NpgsqlCommand(insertSql, connection))
                    {
                        command.Parameters.AddWithValue("@newOrderId", DBNull.Value);
                        command.Parameters.AddWithValue("@newProductId", DBNull.Value);
                        command.Parameters.AddWithValue("@newConfectionerId", DBNull.Value);
                        command.Parameters.AddWithValue("@newSum", 1); 

                        int newId = Convert.ToInt32(command.ExecuteScalar());

                        LoadPositionsData();
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
