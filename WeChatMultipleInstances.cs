using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

#nullable enable

namespace WeChatMultipleInstances
{
    public class WeChatMultipleInstancesForm : Form
    {
        private NumericUpDown? numInstances;
        private Button? btnLaunch;
        private Label? lblStatus;
        private Label? lblWeChatPath;
        private TextBox? txtWeChatPath;
        private Button? btnBrowse;
        private Label? lblTitle;
        private Label? lblDescription;

        public WeChatMultipleInstancesForm()
        {
            InitializeComponents();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeComponents()
        {
            // 设置窗体属性
            this.Text = "微信多开工具";
            this.Size = new Size(500, 400);
            this.BackColor = Color.White;

            // 标题
            lblTitle = new Label
            {
                Text = "微信多开工具",
                Font = new Font("Microsoft YaHei", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(460, 40),
                Location = new Point(20, 20)
            };

            // 描述
            lblDescription = new Label
            {
                Text = "此工具可以帮助您同时打开多个微信客户端，实现多账号同时登录。",
                Font = new Font("Microsoft YaHei", 10),
                Size = new Size(460, 40),
                Location = new Point(20, 60)
            };

            // 微信路径标签
            lblWeChatPath = new Label
            {
                Text = "微信安装路径：",
                Font = new Font("Microsoft YaHei", 10),
                Size = new Size(100, 25),
                Location = new Point(20, 110)
            };

            // 微信路径文本框
            txtWeChatPath = new TextBox
            {
                Size = new Size(280, 25),
                Location = new Point(120, 110),
                Font = new Font("Microsoft YaHei", 9)
            };

            // 自动查找微信安装路径
            string? defaultPath = FindWeChatPath();
            if (!string.IsNullOrEmpty(defaultPath) && txtWeChatPath != null)
            {
                txtWeChatPath.Text = defaultPath;
            }
            else if (txtWeChatPath != null)
            {
                txtWeChatPath.Text = @"C:\Program Files (x86)\Tencent\WeChat\WeChat.exe";
            }

            // 浏览按钮
            btnBrowse = new Button
            {
                Text = "浏览...",
                Size = new Size(60, 25),
                Location = new Point(410, 110),
                Font = new Font("Microsoft YaHei", 9)
            };
            btnBrowse.Click += BtnBrowse_Click;

            // 实例数量标签
            Label lblInstances = new Label
            {
                Text = "打开实例数量：",
                Font = new Font("Microsoft YaHei", 10),
                Size = new Size(100, 25),
                Location = new Point(20, 150)
            };

            // 实例数量选择器
            numInstances = new NumericUpDown
            {
                Minimum = 2,
                Maximum = 10,
                Value = 2,
                Size = new Size(60, 25),
                Location = new Point(120, 150),
                Font = new Font("Microsoft YaHei", 9)
            };

            // 启动按钮
            btnLaunch = new Button
            {
                Text = "启动微信",
                Size = new Size(120, 40),
                Location = new Point(190, 200),
                Font = new Font("Microsoft YaHei", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(26, 173, 25),
                ForeColor = Color.White
            };
            btnLaunch.Click += BtnLaunch_Click;

            // 状态标签
            lblStatus = new Label
            {
                Text = "准备就绪",
                Font = new Font("Microsoft YaHei", 9),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(460, 25),
                Location = new Point(20, 250)
            };

            // 添加控件到窗体
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblDescription);
            this.Controls.Add(lblWeChatPath);
            this.Controls.Add(txtWeChatPath);
            this.Controls.Add(btnBrowse);
            this.Controls.Add(lblInstances);
            this.Controls.Add(numInstances);
            this.Controls.Add(btnLaunch);
            this.Controls.Add(lblStatus);

            // 添加说明文本
            Label lblNote = new Label
            {
                Text = "注意：本工具不会修改微信客户端文件，仅通过特定方式启动多个实例。\n如果微信更新后无法使用，请更新本工具到最新版本。",
                Font = new Font("Microsoft YaHei", 8),
                ForeColor = Color.Gray,
                Size = new Size(460, 40),
                Location = new Point(20, 300)
            };
            this.Controls.Add(lblNote);
        }

        private void BtnBrowse_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "可执行文件 (*.exe)|*.exe";
                openFileDialog.Title = "选择微信可执行文件";
                openFileDialog.InitialDirectory = @"C:\Program Files (x86)\Tencent\WeChat";

                if (openFileDialog.ShowDialog() == DialogResult.OK && txtWeChatPath != null)
                {
                    txtWeChatPath.Text = openFileDialog.FileName;
                }
            }
        }

        private void BtnLaunch_Click(object? sender, EventArgs e)
        {
            try
            {
                if (txtWeChatPath == null)
                {
                    MessageBox.Show("界面初始化错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string wechatPath = txtWeChatPath.Text.Trim();

                // 检查微信路径是否存在
                if (!File.Exists(wechatPath))
                {
                    MessageBox.Show("微信可执行文件不存在，请检查路径是否正确！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 获取实例数量
                if (numInstances == null)
                {
                    MessageBox.Show("界面初始化错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int instances = (int)numInstances.Value;

                // 创建临时批处理文件
                string batFilePath = Path.Combine(Path.GetTempPath(), "WeChatMultiple.bat");
                using (StreamWriter sw = new StreamWriter(batFilePath))
                {
                    sw.WriteLine("@echo off");
                    
                    // 添加多个微信启动命令
                    for (int i = 0; i < instances; i++)
                    {
                        sw.WriteLine($"start \"WeChat Instance {i+1}\" \"{wechatPath}\"");
                    }
                    
                    sw.WriteLine("exit");
                }

                // 执行批处理文件
                Process.Start(batFilePath);

                if (lblStatus != null)
                {
                    lblStatus.Text = $"已成功启动 {instances} 个微信实例";
                    lblStatus.ForeColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                if (lblStatus != null)
                {
                    lblStatus.Text = "启动失败：" + ex.Message;
                    lblStatus.ForeColor = Color.Red;
                }
                MessageBox.Show("启动微信时出错：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string? FindWeChatPath()
        {
            // 常见的微信安装路径
            string[] commonPaths = new string[]
            {
                @"C:\Program Files (x86)\Tencent\WeChat\WeChat.exe",
                @"C:\Program Files\Tencent\WeChat\WeChat.exe",
                @"D:\Program Files (x86)\Tencent\WeChat\WeChat.exe",
                @"D:\Program Files\Tencent\WeChat\WeChat.exe"
            };

            foreach (string path in commonPaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            return null;
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WeChatMultipleInstancesForm());
        }
    }
}