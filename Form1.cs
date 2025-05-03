using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace FPSAide
{
    public partial class Form1 : Form
    {
        // �������μ��ĳ���
        private const int Alt = 0x0001;     // ALT��
        private const int Ctrl = 0x0002;  // CTRL��
        private const int Shift = 0x0004;    // SHIFT��
        private const int Win = 0x0008;      // Windows��

        /// <summary>
        /// ע���ȼ�
        /// </summary>
        /// <param name="hWnd">�����ȼ���Ϣ�Ĵ��ھ��</param>
        /// <param name="id">�ȼ���ʶ����������ȼ���ΨһID��Ӧ�ó����еı�ʶֵ��0x0000��0xbfff֮�䣬DLL�е���0xc000��0xffff֮��   ����˵���ǿ�����д������ͻ���ɡ�</param>
        /// <param name="modifiers">���Ӱ������޸ģ�����˵�㰴ס��CTRL���������ֵ����ѡ��MOD_ALT,MOD_CONTROL,MOD_SHIFT,MOD_WIN,MOD_KEYUP�����������ϼ�����0</param>
        /// <param name="vk">������������ �����磬F7����118��        //�ο��������������кܶ࣬����ֱ��keys.A~Z</param>
        /// <returns></returns>
        [DllImport("user32.dll")] static extern bool RegisterHotKey(IntPtr hWnd, int id, int modifiers, Keys vk);     //ע���ȼ�
        /// <summary>
        /// ע���ȼ� 
        /// </summary>
        /// <param name="hWnd">Ҫע���ĸ����ھ�����ȼ�</param>
        /// <param name="id">��ʼע���ΨһID</param>
        /// <returns></returns>
        [DllImport("user32.dll")] static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // ׼�Ǹ��ǲ�ʵ��
        private CrosshairOverlay overlay;

        // ����׼����ʾ״̬�ı�־
        private bool isVisible = false;
        private const int WM_HOTKEY = 0x0312;
        // �ȼ�ID��ȷ��Ψһ��
        public static AllConfig Config = new AllConfig();

        public Form1()
        {
            InitializeComponent();

            //��ȡ����
            Config = TryRead<AllConfig>();
            InitializeOverlay();
        }

        public class AllConfig
        {
            public CrosshairConfig CrosshairConfig { get; set; } = new CrosshairConfig();
            public List<Hotkey> Hotkeys { get; set; } = new();
        }

        public class Hotkey
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public Keys Keys { get; set; }
            public int Modifiers { get; set; }
        }

        public class CrosshairConfig
        {
            public int width { get; set; } = 3;
            public int length { get; set; } = 15;
            public int color { get; set; } = 0;
            /// <summary>
            /// Xƫ��ֵ
            /// </summary>
            public int OffsetValueX { get; set; } = 0;
            /// <summary>
            /// Yƫ��ֵ
            /// </summary>
            public int OffsetValueY { get; set; } = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //ע���ȼ�
            RegisterHotKeyAll(Config.Hotkeys);

            //��������ɫ��ӵ�������
            foreach (KnownColor color in Enum.GetValues(typeof(KnownColor)))
                comboBox_color.Items.Add(color);
            //��ȡ����
            //���ʿ��
            textBox_width.Text = Config.CrosshairConfig.width.ToString();
            //׼�ǳ���
            textBox_length.Text = Config.CrosshairConfig.length.ToString();
            //��ɫ
            comboBox_color.SelectedIndex = Config.CrosshairConfig.color;
        }

        // ����ر�ʱ��������
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Config.Hotkeys.ForEach(d => UnregisterHotKey(this.Handle, d.ID));
            if (overlay != null && !overlay.IsDisposed)
            {
                overlay.Close();
            }
            // ��������
            TrySave(Config);
        }

        // ��ʼ��׼�Ǹ��ǲ�
        private void InitializeOverlay()
        {
            overlay = new CrosshairOverlay();
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                switch (Config.Hotkeys)
                {
                    case var _ when Config.Hotkeys.Any(d => d.ID == id && d.Name == "Display"):
                        // ������ʾ/�����ȼ�
                        if (!isVisible)
                        {
                            overlay.Show();
                            isVisible = true;
                        }
                        else
                        {
                            overlay.Hide();
                            isVisible = false;
                        }
                        break;
                    case var _ when Config.Hotkeys.Any(d => d.ID == id && d.Name == "Up"):
                        {
                            // ���������ȼ�
                            overlay.Location = new Point(overlay.Location.X, overlay.Location.Y - 5);
                            Config.CrosshairConfig.OffsetValueY -= 5;
                            break;
                        }
                    case var _ when Config.Hotkeys.Any(d => d.ID == id && d.Name == "Down"):
                        {
                            // ���������ȼ�
                            overlay.Location = new Point(overlay.Location.X, overlay.Location.Y + 5);
                            Config.CrosshairConfig.OffsetValueY += 5;
                            break;
                        }
                    case var _ when Config.Hotkeys.Any(d => d.ID == id && d.Name == "Left"):
                        {
                            // ���������ȼ�
                            overlay.Location = new Point(overlay.Location.X - 5, overlay.Location.Y);
                            Config.CrosshairConfig.OffsetValueX -= 5;
                            break;
                        }
                    case var _ when Config.Hotkeys.Any(d => d.ID == id && d.Name == "Right"):
                        {
                            // ���������ȼ�
                            overlay.Location = new Point(overlay.Location.X + 5, overlay.Location.Y);
                            Config.CrosshairConfig.OffsetValueX += 5;
                            break;
                        }
                }
            }
            base.WndProc(ref m);
        }
        // �л�׼����ʾ/���صİ�ť����¼�����
        private void button1_Click(object sender, EventArgs e)
        {
            if (!isVisible)
            {
                overlay.Show();
                isVisible = true;
            }
            else
            {
                overlay.Hide();
                isVisible = false;
            }
        }


        private void textBox_width_TextChanged(object sender, EventArgs e)
        {
            Config.CrosshairConfig.width = int.Parse(textBox_width.Text);
            Config.CrosshairConfig.width = int.Parse(textBox_width.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Config.CrosshairConfig.length = int.Parse(textBox_length.Text);
            Config.CrosshairConfig.length = int.Parse(textBox_length.Text);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ת����ֵ��color
            Config.CrosshairConfig.color = comboBox_color.SelectedIndex;
        }

        //private void button1_Click_1(object sender, EventArgs e)
        //{
        //    // �ȼ��������
        //    if (!isVisible)
        //    {
        //        overlay.Show();
        //        button1.Text = "����";
        //        isVisible = true;
        //    }
        //    else
        //    {
        //        overlay.Hide();
        //        button1.Text = "��ʾ";
        //        isVisible = false;
        //    }
        //}


        public void RegisterHotKeyAll(List<Hotkey> hotkeys)
        {
            var hotkey1 = new Hotkey
            {
                ID = new Random().Next(1, 90000),
                Name = "Display",
                Keys = Keys.F9,
                Modifiers = Shift
            };
            TryAddItems(hotkeys, hotkey1);
            var hotkey2 = new Hotkey
            {
                ID = new Random().Next(1, 90000),
                Name = "Up",
                Keys = Keys.Up,
                Modifiers = Ctrl
            };
            TryAddItems(hotkeys, hotkey2);
            var hotkey3 = new Hotkey
            {
                ID = new Random().Next(1, 90000),
                Name = "Down",
                Keys = Keys.Down,
                Modifiers = Ctrl
            };
            TryAddItems(hotkeys, hotkey3);
            var hotkey4 = new Hotkey
            {
                ID = new Random().Next(1, 90000),
                Name = "Left",
                Keys = Keys.Left,
                Modifiers = Ctrl
            };
            TryAddItems(hotkeys, hotkey4);
            var hotkey5 = new Hotkey
            {
                ID = new Random().Next(1, 90000),
                Name = "Right",
                Keys = Keys.Right,
                Modifiers = Ctrl
            };
            TryAddItems(hotkeys, hotkey5);
            hotkeys.ForEach(d =>
            {
                var success = RegisterHotKey(this.Handle, d.ID, d.Modifiers, d.Keys);
                if (!success)
                    MessageBox.Show($"{d.Name}�ȼ�ע��ʧ�ܣ������ѱ���������ռ�á�");
            });
        }

        public T TryRead<T>(string path = "config.json") where T : new()
        {
            try
            {
                var f = new FileInfo(path);
                if (!f.Exists)
                {
                    using var fs = f.Create();
                    return new();
                }
                else
                {
                    using var fs = f.OpenRead();
                    using var sr = new StreamReader(fs);
                    var json = sr.ReadToEnd();
                    var obj = JsonSerializer.Deserialize<T>(json);
                    return obj;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void TrySave<T>(T obj, string path = "config.json") where T : new()
        {
            try
            {
                var f = new FileInfo(path);
                if (!f.Exists)
                {
                    //�����ļ�,��д��
                    using var fs = f.Create();
                    using var sw = new StreamWriter(fs);
                    var json = JsonSerializer.Serialize(obj);
                    sw.Write(json);
                }
                else
                {
                    using var fs = f.Create();
                    using var sw = new StreamWriter(fs);
                    var json = JsonSerializer.Serialize(obj);
                    sw.Write(json);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void TryAddItems(List<Hotkey> values, Hotkey item)
        {
            if (values.Any(i => i.Name == item.Name))
                return;
            values.Add(item);
        }

        private void ��������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ����׼��λ��
            overlay.Location = new Point(
                (Screen.PrimaryScreen.Bounds.Width / 2 - overlay.Width / 2) + 45,
                Screen.PrimaryScreen.Bounds.Height / 2 - overlay.Height / 2
            );
            Config.CrosshairConfig.OffsetValueX = 0;
            Config.CrosshairConfig.OffsetValueY = 0;
        }
    }

    /// <summary>
    /// ��ʾ׼�ǵ�͸��������
    /// </summary>
    public class CrosshairOverlay : Form
    {
        // Windows API ��������
        // ʹ����͸������������¼���͸������Ĵ���
        private const int WS_EX_TRANSPARENT = 0x20;
        // ���÷ֲ㴰��
        private const int WS_EX_LAYERED = 0x80000;
        // �����������͸
        private const uint WS_EX_CLICKTHROUGH = 0x00000020;

        // ���� Windows API ����
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        // ��ȡ/���ô�����չ��ʽ������
        private const int GWL_EXSTYLE = -20;

        public CrosshairOverlay()
        {
            // ���ô����������
            this.FormBorderStyle = FormBorderStyle.None;        // �ޱ߿�
            this.BackColor = Color.Black;                       // ����ɫ��Ϊ��ɫ
            this.TransparencyKey = Color.Black;                 // ͸��ɫ��Ϊ��ɫ��ʹ����͸����
            this.TopMost = true;                               // ����ʼ���ö�
            this.ShowInTaskbar = false;                        // ������������ʾ
            this.Size = new Size(50, 50);                      // ���ô����С

            // �����嶨λ����Ļ����
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(
                (Screen.PrimaryScreen.Bounds.Width / 2 - this.Width / 2) + 45+Form1.Config.CrosshairConfig.OffsetValueX,
                Screen.PrimaryScreen.Bounds.Height / 2 - this.Height / 2 + Form1.Config.CrosshairConfig.OffsetValueY
            );

            // �������ʱ������չ��ʽ
            this.Load += (sender, e) =>
            {
                // ��ȡ��ǰ������ʽ
                int exStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
                // ����µĴ�����ʽ���ֲ�|͸��|�����͸
                exStyle |= (int)(WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_CLICKTHROUGH);
                // �����µĴ�����ʽ
                SetWindowLong(this.Handle, GWL_EXSTYLE, exStyle);
            };
        }

        // ��д����Ļ��Ʒ���
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // ��������
            using var pen = new Pen(Color.FromKnownColor(Enum.GetValues(KnownColor.ActiveBorder.GetType()).Cast<KnownColor>().ToList()[Form1.Config.CrosshairConfig.color]), width: Form1.Config.CrosshairConfig.width);

            // ����׼�ǵ����ĵ�
            int centerX = this.Width / 2;
            int centerY = this.Height / 2;
            // ׼��ʮ���ߵĳ���
            int length = Form1.Config.CrosshairConfig.length;

            // ����ʮ��׼��
            // ��ˮƽ��
            e.Graphics.DrawLine(pen, centerX - length, centerY, centerX + length, centerY);
            // ����ֱ��
            e.Graphics.DrawLine(pen, centerX, centerY - length, centerX, centerY + length);

            // ��׼�����Ļ���һ��СԲ��
            int dotSize = 2;
            e.Graphics.DrawEllipse(pen, centerX - dotSize, centerY - dotSize, dotSize * 2, dotSize * 2);
        }
    }
}