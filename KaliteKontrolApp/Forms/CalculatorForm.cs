using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KaliteKontrolApp.Forms
{
    public partial class CalculatorForm : Form
    {
        private TextBox _display;
        private double _result = 0;
        private string _operation = "";
        private bool _isNewNumber = true;

        public CalculatorForm()
        {
            InitializeComponent();
            Text = "Hesap Makinesi";
            Size = new Size(320, 480);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10F);
        }

        private void InitializeComponent()
        {
            _display = new TextBox
            {
                Location = new Point(20, 20),
                Size = new Size(260, 50),
                Font = new Font("Segoe UI", 20F),
                TextAlign = HorizontalAlignment.Right,
                ReadOnly = true,
                Text = "0",
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            Controls.Add(_display);

            string[] buttons = {
                "7", "8", "9", "/",
                "4", "5", "6", "*",
                "1", "2", "3", "-",
                "0", ".", "=", "+",
                "C", "CE", "←", "%"
            };

            int x = 20, y = 80;
            for (int i = 0; i < buttons.Length; i++)
            {
                var btn = new Button
                {
                    Text = buttons[i],
                    Size = new Size(60, 55),
                    Location = new Point(x, y),
                    Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                    BackColor = buttons[i] switch
                    {
                        "=" => Color.FromArgb(0, 120, 212),
                        "/" or "*" or "-" or "+" or "%" => Color.FromArgb(240, 240, 240),
                        "C" or "CE" or "←" => Color.FromArgb(255, 200, 200),
                        _ => Color.White
                    },
                    ForeColor = buttons[i] == "=" ? Color.White : Color.Black,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.BorderColor = Color.LightGray;
                btn.Click += Button_Click;
                Controls.Add(btn);

                x += 65;
                if ((i + 1) % 4 == 0)
                {
                    x = 20;
                    y += 60;
                }
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            string text = btn.Text;

            if (char.IsDigit(text[0]) || text == ".")
            {
                if (_isNewNumber)
                {
                    _display.Text = text == "." ? "0." : text;
                    _isNewNumber = false;
                }
                else
                {
                    if (text == "." && _display.Text.Contains(".")) return;
                    _display.Text += text;
                }
            }
            else if (text == "C")
            {
                _display.Text = "0";
                _result = 0;
                _operation = "";
                _isNewNumber = true;
            }
            else if (text == "CE")
            {
                _display.Text = "0";
                _isNewNumber = true;
            }
            else if (text == "←")
            {
                if (_display.Text.Length > 1)
                    _display.Text = _display.Text[..^1];
                else
                    _display.Text = "0";
            }
            else if (text == "=")
            {
                Calculate();
                _operation = "";
                _isNewNumber = true;
            }
            else
            {
                _operation = text;
                _result = double.Parse(_display.Text);
                _isNewNumber = true;
            }
        }

        private void Calculate()
        {
            double current = double.Parse(_display.Text);
            double result = _operation switch
            {
                "+" => _result + current,
                "-" => _result - current,
                "*" => _result * current,
                "/" => current != 0 ? _result / current : 0,
                "%" => _result * current / 100,
                _ => current
            };
            _display.Text = result.ToString();
            _result = result;
        }
    }
}
