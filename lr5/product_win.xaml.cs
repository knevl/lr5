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
    /// Логика взаимодействия для product_win.xaml
    /// </summary>
    public partial class product_win : Window
    {
        private const string ConnectionString =
            "Host=localhost;Database=confectionery;Port=5432;Username=postgres;Password=admin";

        public product_win()
        {
            InitializeComponent();
            LoadProductData();
        }

        private void b_menu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
        private void LoadProductData()
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM product";

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
                        int productId = Convert.ToInt32(selectedRow["id"]);
                        string deleteSql = $"DELETE FROM product WHERE id = {productId}";

                        using (var command = new NpgsqlCommand(deleteSql, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        LoadProductData();
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
                        int productId = Convert.ToInt32(selectedRow["id"]);
                        string updateSql = "UPDATE product SET name = @newName WHERE id = @productId";

                        using (var command = new NpgsqlCommand(updateSql, connection))
                        {
                            command.Parameters.AddWithValue("@newName", selectedRow["name"]);
                            command.Parameters.AddWithValue("@productId", productId);

                            command.ExecuteNonQuery();
                        }
                        LoadProductData();
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

                    string insertSql = "INSERT INTO product (name) VALUES (@newName) RETURNING id";

                    using (var command = new NpgsqlCommand(insertSql, connection))
                    {
                        command.Parameters.AddWithValue("@newName", "Введите наименование. Не забудьте нажать кнопку Сохранить");

                        int newId = Convert.ToInt32(command.ExecuteScalar());

                        LoadProductData();
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
