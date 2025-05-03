using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace FPSAide
{
    public partial class Form1 : Form
    {
        // 定义修饰键的常量
        private const int Alt = 0x0001;     // ALT键
        private const int Ctrl = 0x0002;  // CTRL键
        private const int Shift = 0x0004;    // SHIFT键
        private const int Win = 0x0008;      // Windows键

        /// <summary>
        /// 注册热键
        /// </summary>
        /// <param name="hWnd">接收热键消息的窗口句柄</param>
        /// <param name="id">热键标识，代表这个热键的唯一ID，应用程序中的标识值在0x0000和0xbfff之间，DLL中的在0xc000和0xffff之间   简单来说就是可以乱写，不冲突即可。</param>
        /// <param name="modifiers">附加按键的修改，比如说你按住了CTRL键，这里的值可以选择MOD_ALT,MOD_CONTROL,MOD_SHIFT,MOD_WIN,MOD_KEYUP，如果不是组合键则填0</param>
        /// <param name="vk">按键的虚拟码 （比如，F7的是118）        //参考虚拟键码表，网上有很多，或者直接keys.A~Z</param>
        /// <returns></returns>
        [DllImport("user32.dll")] static extern bool RegisterHotKey(IntPtr hWnd, int id, int modifiers, Keys vk);     //注册热键
        /// <summary>
        /// 注销热键 
        /// </summary>
        /// <param name="hWnd">要注销哪个窗口句柄的热键</param>
        /// <param name="id">开始注册的唯一ID</param>
        /// <returns></returns>
        [DllImport("user32.dll")] static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // 准星覆盖层实例
        private CrosshairOverlay overlay;

        // 控制准星显示状态的标志
        private bool isVisible = false;
        private const int WM_HOTKEY = 0x0312;
        // 热键ID（确保唯一）
        public static AllConfig Config = new AllConfig();

        public Form1()
        {
            InitializeComponent();

            //读取配置
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
            /// X偏移值
            /// </summary>
            public int OffsetValueX { get; set; } = 0;
            /// <summary>
            /// Y偏移值
            /// </summary>
            public int OffsetValueY { get; set; } = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //注册热键
            RegisterHotKeyAll(Config.Hotkeys);

            //把所有颜色添加到下拉框
            foreach (KnownColor color in Enum.GetValues(typeof(KnownColor)))
                comboBox_color.Items.Add(color);
            //读取配置
            //画笔宽度
            textBox_width.Text = Config.CrosshairConfig.width.ToString();
            //准星长度
            textBox_length.Text = Config.CrosshairConfig.length.ToString();
            //颜色
            comboBox_color.SelectedIndex = Config.CrosshairConfig.color;
        }

        // 窗体关闭时的清理工作
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Config.Hotkeys.ForEach(d => UnregisterHotKey(this.Handle, d.ID));
            if (overlay != null && !overlay.IsDisposed)
            {
                overlay.Close();
            }
            // 保存配置
            TrySave(Config);
        }

        // 初始化准星覆盖层
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
                        // 处理显示/隐藏热键
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
                            // 处理上移热键
                            overlay.Location = new Point(overlay.Location.X, overlay.Location.Y - 5);
                            Config.CrosshairConfig.OffsetValueY -= 5;
                            break;
                        }
                    case var _ when Config.Hotkeys.Any(d => d.ID == id && d.Name == "Down"):
                        {
                            // 处理下移热键
                            overlay.Location = new Point(overlay.Location.X, overlay.Location.Y + 5);
                            Config.CrosshairConfig.OffsetValueY += 5;
                            break;
                        }
                    case var _ when Config.Hotkeys.Any(d => d.ID == id && d.Name == "Left"):
                        {
                            // 处理左移热键
                            overlay.Location = new Point(overlay.Location.X - 5, overlay.Location.Y);
                            Config.CrosshairConfig.OffsetValueX -= 5;
                            break;
                        }
                    case var _ when Config.Hotkeys.Any(d => d.ID == id && d.Name == "Right"):
                        {
                            // 处理右移热键
                            overlay.Location = new Point(overlay.Location.X + 5, overlay.Location.Y);
                            Config.CrosshairConfig.OffsetValueX += 5;
                            break;
                        }
                }
            }
            base.WndProc(ref m);
        }
        // 切换准星显示/隐藏的按钮点击事件处理
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
            //转换后赋值给color
            Config.CrosshairConfig.color = comboBox_color.SelectedIndex;
        }

        //private void button1_Click_1(object sender, EventArgs e)
        //{
        //    // 热键处理代码
        //    if (!isVisible)
        //    {
        //        overlay.Show();
        //        button1.Text = "隐藏";
        //        isVisible = true;
        //    }
        //    else
        //    {
        //        overlay.Hide();
        //        button1.Text = "显示";
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
                    MessageBox.Show($"{d.Name}热键注册失败，可能已被其他程序占用。");
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
                    //创建文件,并写入
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

        private void 重置坐标ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 重置准星位置
            overlay.Location = new Point(
                (Screen.PrimaryScreen.Bounds.Width / 2 - overlay.Width / 2) + 45,
                Screen.PrimaryScreen.Bounds.Height / 2 - overlay.Height / 2
            );
            Config.CrosshairConfig.OffsetValueX = 0;
            Config.CrosshairConfig.OffsetValueY = 0;
        }
    }

    /// <summary>
    /// 显示准星的透明窗口类
    /// </summary>
    public class CrosshairOverlay : Form
    {
        // Windows API 常量定义
        // 使窗体透明，允许鼠标事件穿透到下面的窗口
        private const int WS_EX_TRANSPARENT = 0x20;
        // 启用分层窗口
        private const int WS_EX_LAYERED = 0x80000;
        // 允许鼠标点击穿透
        private const uint WS_EX_CLICKTHROUGH = 0x00000020;

        // 导入 Windows API 函数
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        // 获取/设置窗口扩展样式的索引
        private const int GWL_EXSTYLE = -20;

        public CrosshairOverlay()
        {
            // 设置窗体基本属性
            this.FormBorderStyle = FormBorderStyle.None;        // 无边框
            this.BackColor = Color.Black;                       // 背景色设为黑色
            this.TransparencyKey = Color.Black;                 // 透明色设为黑色（使背景透明）
            this.TopMost = true;                               // 窗体始终置顶
            this.ShowInTaskbar = false;                        // 不在任务栏显示
            this.Size = new Size(50, 50);                      // 设置窗体大小

            // 将窗体定位到屏幕中心
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(
                (Screen.PrimaryScreen.Bounds.Width / 2 - this.Width / 2) + 45+Form1.Config.CrosshairConfig.OffsetValueX,
                Screen.PrimaryScreen.Bounds.Height / 2 - this.Height / 2 + Form1.Config.CrosshairConfig.OffsetValueY
            );

            // 窗体加载时设置扩展样式
            this.Load += (sender, e) =>
            {
                // 获取当前窗口样式
                int exStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
                // 添加新的窗口样式：分层|透明|点击穿透
                exStyle |= (int)(WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_CLICKTHROUGH);
                // 设置新的窗口样式
                SetWindowLong(this.Handle, GWL_EXSTYLE, exStyle);
            };
        }

        // 重写窗体的绘制方法
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // 创建画笔
            using var pen = new Pen(Color.FromKnownColor(Enum.GetValues(KnownColor.ActiveBorder.GetType()).Cast<KnownColor>().ToList()[Form1.Config.CrosshairConfig.color]), width: Form1.Config.CrosshairConfig.width);

            // 计算准星的中心点
            int centerX = this.Width / 2;
            int centerY = this.Height / 2;
            // 准星十字线的长度
            int length = Form1.Config.CrosshairConfig.length;

            // 绘制十字准星
            // 画水平线
            e.Graphics.DrawLine(pen, centerX - length, centerY, centerX + length, centerY);
            // 画垂直线
            e.Graphics.DrawLine(pen, centerX, centerY - length, centerX, centerY + length);

            // 在准星中心绘制一个小圆点
            int dotSize = 2;
            e.Graphics.DrawEllipse(pen, centerX - dotSize, centerY - dotSize, dotSize * 2, dotSize * 2);
        }
    }
}