using System.Security.Cryptography;
using System.Text;

namespace ConnString_Convertor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == null && textBox1.Text == "")
                {
                    MessageBox.Show("Please write connection string!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                string enc_ = textBox1.Text;
                textBox2.Text = EncryptString(enc_);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox2.Text == null && textBox2.Text == "")
                {
                    MessageBox.Show("Please write connection string!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                string dec_ = textBox2.Text;
                textBox1.Text = DecryptString(dec_);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }




        public static string EncryptString(string plainText)
        {
            string key = "6c2a47cb557a43a690b479ac2382fa10";

            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string cipherText)
        {
            string key = "6c2a47cb557a43a690b479ac2382fa10";

            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }


    }
}