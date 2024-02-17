using Npgsql;
using System.Text.RegularExpressions;

namespace Practice1002
{
    public partial class RegistrationForm : Form
    {
        const string CONNECTION_STRING = "Host= localhost:5432; username = postgres;  Password=25481; Database=UserDatabase";
        public RegistrationForm()
        {
            InitializeComponent();
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            if (textBoxSurname.Text == "" || textBoxName.Text == "" || textBoxEmail.Text == "" ||
                textBoxPassword.Text == "" || textBoxRepeatPassword.Text == "")
                MessageBox.Show("���� �� * ����������� ��� ����������!", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (textBoxPhone.Text != "8-   -   -  -" && !textBoxPhone.MaskFull)
                MessageBox.Show("������� ���������� ����� ��������!", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (!IsEmailCorrect(textBoxEmail.Text.Trim()))
                MessageBox.Show("������� ���������� ����� ����������� �����!", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (IsUserExists(textBoxEmail.Text.Trim()))
                MessageBox.Show("������������ � ��������� ����������� ������ ��� ����������!", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (textBoxPassword.Text != textBoxRepeatPassword.Text)
                MessageBox.Show("������ �� ���������!", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (!IsPasswordProper(textBoxPassword.Text))
                MessageBox.Show("������ ������ �������� �� 8 ��������, ������� ����� �������� � ������� ��������, � ����� ������� \"@,!\"", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                AddUser(textBoxSurname.Text, textBoxName.Text, (textBoxPhone.Text != "8-   -   -  -") ? textBoxPhone.Text : null,
                    textBoxEmail.Text, textBoxPassword.Text);
                MessageBox.Show("����������� ������ �������!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
        }

        private bool IsEmailCorrect(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private bool IsPasswordProper(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;
            string lower = "abcdefghijklmnopqrstuvwxyz��������������������������������";
            string upper = "QWERTYUIOPASDFGHJKLZXCVBNM�������������������������������";
            string signs = "@,!";
            if (password.Length != 8)
                return false;
            if (!lower.Any(password.Contains))
                return false;
            if (!upper.Any(password.Contains))
                return false;
            if (!signs.Any(password.Contains))
                return false;
            return true;
        }

        private bool IsUserExists(string email)
        {
            var con = new NpgsqlConnection(CONNECTION_STRING);
            con.Open();
            var cmd = new NpgsqlCommand($"SELECT * FROM users_data WHERE email = '{email}'", con);
            var scalar = cmd.ExecuteScalar();
            if (scalar != null)
            {
                return true;
            }
            return false;
        }

        private void AddUser(string surname, string username, string? phoneNumber, string email, string password)
        {
            var con = new NpgsqlConnection(CONNECTION_STRING);
            con.Open();
            var cmd = new NpgsqlCommand($"INSERT INTO  users_data(surname, user_name, phone_number, email, user_password) " +
                $"VALUES(@surname, @name, @number, " +
                $"@email, @password)", con);
            cmd.Parameters.AddWithValue("surname", surname);
            cmd.Parameters.AddWithValue("name", username);
            cmd.Parameters.AddWithValue("number", (object?)phoneNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("email", email);
            cmd.Parameters.AddWithValue("password", password);
            cmd.ExecuteNonQuery();
        }
    }
}