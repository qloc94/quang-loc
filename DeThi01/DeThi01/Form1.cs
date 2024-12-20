using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace DeThi01
{
    public partial class Form1 : Form
    {
        // Chuỗi kết nối đến SQL Server
        private string connectionString = "Server=localhost;Database=QuanlySV;Integrated Security=True;";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDanhSachSinhVien();
                LoadDanhSachLop();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}");
            }
        }

        private void LoadDanhSachSinhVien()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Sinhvien", connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvSinhvien.DataSource = null; // Xóa dữ liệu cũ
                    dgvSinhvien.DataSource = dt; // Tải lại dữ liệu mới
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách sinh viên: {ex.Message}");
            }
        }

        private void LoadDanhSachLop()
        {
            try
            {
                ebolop.Items.Clear(); // Xóa các item cũ
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM Lop", connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        ebolop.Items.Add(reader["MaLop"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách lớp: {ex.Message}");
            }
        }

        private void btTim_Click(object sender, EventArgs e)
        {
            try
            {
                string maSV = txtMaSV.Text.Trim();
                string hoTen = txtHoTen.Text.Trim();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Sinhvien WHERE MaSV LIKE @MaSV OR HoTenSV LIKE @HoTen";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@MaSV", $"%{maSV}%");
                    command.Parameters.AddWithValue("@HoTen", $"%{hoTen}%");

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvSinhvien.DataSource = null;
                    dgvSinhvien.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm sinh viên: {ex.Message}");
            }
        }

        private void btThem_Click(object sender, EventArgs e)
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            dtNgaysinh.Value = DateTime.Now;
            ebolop.SelectedIndex = -1;
            txtMaSV.Focus();
        }

        private void btXoa_Click(object sender, EventArgs e)
        {
            if (dgvSinhvien.CurrentRow != null)
            {
                try
                {
                    string maSV = dgvSinhvien.CurrentRow.Cells["MaSV"].Value.ToString();
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("DELETE FROM Sinhvien WHERE MaSV = @MaSV", connection);
                        command.Parameters.AddWithValue("@MaSV", maSV);
                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show("Xóa sinh viên thành công!");
                    LoadDanhSachSinhVien();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa sinh viên: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa.");
            }
        }

        private void btSua_Click(object sender, EventArgs e)
        {
            if (dgvSinhvien.CurrentRow != null)
            {
                txtMaSV.Text = dgvSinhvien.CurrentRow.Cells["MaSV"].Value.ToString();
                txtHoTen.Text = dgvSinhvien.CurrentRow.Cells["HoTenSV"].Value.ToString();
                dtNgaysinh.Value = Convert.ToDateTime(dgvSinhvien.CurrentRow.Cells["NgaySinh"].Value);
                ebolop.SelectedItem = dgvSinhvien.CurrentRow.Cells["MaLop"].Value.ToString();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần sửa.");
            }
        }

        private void btLuu_Click(object sender, EventArgs e)
        {
            string maSV = txtMaSV.Text.Trim();
            string hoTen = txtHoTen.Text.Trim();
            DateTime ngaySinh = dtNgaysinh.Value;
            string maLop = ebolop.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(maSV) || string.IsNullOrEmpty(hoTen) || string.IsNullOrEmpty(maLop))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("IF EXISTS (SELECT 1 FROM Sinhvien WHERE MaSV = @MaSV) " +
                                                         "UPDATE Sinhvien SET HoTenSV = @HoTen, NgaySinh = @NgaySinh, MaLop = @MaLop WHERE MaSV = @MaSV " +
                                                         "ELSE INSERT INTO Sinhvien (MaSV, HoTenSV, NgaySinh, MaLop) VALUES (@MaSV, @HoTen, @NgaySinh, @MaLop)", connection);
                    command.Parameters.AddWithValue("@MaSV", maSV);
                    command.Parameters.AddWithValue("@HoTen", hoTen);
                    command.Parameters.AddWithValue("@NgaySinh", ngaySinh);
                    command.Parameters.AddWithValue("@MaLop", maLop);
                    command.ExecuteNonQuery();
                }
                MessageBox.Show("Lưu thông tin thành công!");
                LoadDanhSachSinhVien();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu thông tin: {ex.Message}");
            }
        }

        private void btKhong_Click(object sender, EventArgs e)
        {
            btThem_Click(sender, e);
        }

        private void btThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
