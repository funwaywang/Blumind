using System;
using System.Drawing;
using System.Windows.Forms;
using Blumind.Controls;

namespace Blumind
{
    class TestForm : Blumind.Controls.Aero.GlassForm//Form
    {
        private TaskBar taskBar1;
        private Label label1;
        private Button button1;
        private GroupBox groupBox1;
        private ToolStrip toolStrip1;
        private ToolStripButton 新建NToolStripButton;
        private ToolStripButton 打开OToolStripButton;
        private ToolStripButton 保存SToolStripButton;
        private ToolStripButton 打印PToolStripButton;
        private ToolStripSeparator toolStripSeparator;
        private ToolStripButton 剪切UToolStripButton;
        private ToolStripButton 复制CToolStripButton;
        private ToolStripButton 粘贴PToolStripButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton 帮助LToolStripButton;
        private ToolStripButton 新建NToolStripButton1;
        private ToolStripButton 打开OToolStripButton1;
        private ToolStripButton 保存SToolStripButton1;
        private ToolStripButton 打印PToolStripButton1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton 剪切UToolStripButton1;
        private ToolStripButton 复制CToolStripButton1;
        private ToolStripButton 粘贴PToolStripButton1;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton 帮助LToolStripButton1;
        private ToolStripButton 新建NToolStripButton2;
        private ToolStripButton 打开OToolStripButton2;
        private ToolStripButton 保存SToolStripButton2;
        private ToolStripButton 打印PToolStripButton2;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton 剪切UToolStripButton2;
        private ToolStripButton 复制CToolStripButton2;
        private ToolStripButton 粘贴PToolStripButton2;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripButton 新建NToolStripButton3;
        private ToolStripButton 打开OToolStripButton3;
        private ToolStripButton 保存SToolStripButton3;
        private ToolStripButton 打印PToolStripButton3;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripButton 剪切UToolStripButton3;
        private ToolStripButton 复制CToolStripButton3;
        private ToolStripButton 粘贴PToolStripButton3;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripButton 帮助LToolStripButton3;
        private ToolStripButton 帮助LToolStripButton2;
        //MyTabControl tabCtrl;

        public TestForm()
        {
            InitializeComponent();
            //tabCtrl = new MyTabControl();
            //tabCtrl.Dock = DockStyle.Fill;
            //tabCtrl.Alignment = TabAlignment.Right;
            //Controls.Add(tabCtrl);

            //for (int i = 0; i < 4; i++)
            //{
            //    Control ctrl = new Control(string.Format("Tab {0}", i));
            //    tabCtrl.AddPage(ctrl);
            //}
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ResetAeroGlass();
            //ExcludeControlFromAeroGlass(button1);
            button1.BackColor = Color.Transparent;
        }

        protected override void OnAeroGlassCompositionChanged()
        {
            base.OnAeroGlassCompositionChanged();

            ResetAeroGlass();
        }

        private TabBar AddTabBar(DockStyle dockStyle, TabAlignment tabAlignment)
        {
            TabBar tabBar = new TabBar();
            tabBar.Alignment = tabAlignment;
            tabBar.Width = 30;
            tabBar.Dock = dockStyle;
            tabBar.ShowDropDownButton = true;
            //tabBar.BackColor = SystemColors.Control;// Color.Silver;
            //tabBar.SelectedItemBackColor = Color.White;
            //tabBar.ItemBackColor = SystemColors.Control;
            //tabBar.ItemForeColor = SystemColors.ControlText;
            Controls.Add(tabBar);

            for (int i = 0; i < 5; i++)
            {
                tabBar.Items.Add(new TabItem(string.Format("Tab {0}", i)));
            }

            return tabBar;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestForm));
            this.taskBar1 = new Blumind.Controls.TaskBar();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.新建NToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.打开OToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.保存SToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.打印PToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.剪切UToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.复制CToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.粘贴PToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.帮助LToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.新建NToolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.打开OToolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.保存SToolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.打印PToolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.剪切UToolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.复制CToolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.粘贴PToolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.帮助LToolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.新建NToolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.打开OToolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.保存SToolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.打印PToolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.剪切UToolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.复制CToolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.粘贴PToolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.帮助LToolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.新建NToolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.打开OToolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.保存SToolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.打印PToolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.剪切UToolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.复制CToolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.粘贴PToolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.帮助LToolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // taskBar1
            // 
            this.taskBar1.AeroBackground = false;
            this.taskBar1.BackColor = System.Drawing.Color.Black;
            this.taskBar1.BaseLineSize = 3;
            this.taskBar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.taskBar1.Location = new System.Drawing.Point(0, 0);
            this.taskBar1.Name = "taskBar1";
            this.taskBar1.Size = new System.Drawing.Size(514, 28);
            this.taskBar1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Black;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label1.Location = new System.Drawing.Point(169, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Hello";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(31, 217);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Black;
            this.groupBox1.Location = new System.Drawing.Point(73, 142);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(131, 50);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新建NToolStripButton,
            this.打开OToolStripButton,
            this.保存SToolStripButton,
            this.打印PToolStripButton,
            this.toolStripSeparator,
            this.剪切UToolStripButton,
            this.复制CToolStripButton,
            this.粘贴PToolStripButton,
            this.toolStripSeparator1,
            this.帮助LToolStripButton,
            this.新建NToolStripButton1,
            this.打开OToolStripButton1,
            this.保存SToolStripButton1,
            this.打印PToolStripButton1,
            this.toolStripSeparator2,
            this.剪切UToolStripButton1,
            this.复制CToolStripButton1,
            this.粘贴PToolStripButton1,
            this.toolStripSeparator3,
            this.帮助LToolStripButton1,
            this.新建NToolStripButton2,
            this.打开OToolStripButton2,
            this.保存SToolStripButton2,
            this.打印PToolStripButton2,
            this.toolStripSeparator4,
            this.剪切UToolStripButton2,
            this.复制CToolStripButton2,
            this.粘贴PToolStripButton2,
            this.toolStripSeparator5,
            this.帮助LToolStripButton2,
            this.新建NToolStripButton3,
            this.打开OToolStripButton3,
            this.保存SToolStripButton3,
            this.打印PToolStripButton3,
            this.toolStripSeparator6,
            this.剪切UToolStripButton3,
            this.复制CToolStripButton3,
            this.粘贴PToolStripButton3,
            this.toolStripSeparator7,
            this.帮助LToolStripButton3});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 28);
            this.toolStrip1.MaximumSize = new System.Drawing.Size(400, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(393, 65);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // 新建NToolStripButton
            // 
            this.新建NToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.新建NToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("新建NToolStripButton.Image")));
            this.新建NToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.新建NToolStripButton.Name = "新建NToolStripButton";
            this.新建NToolStripButton.Size = new System.Drawing.Size(23, 20);
            this.新建NToolStripButton.Text = "新建(&N)";
            // 
            // 打开OToolStripButton
            // 
            this.打开OToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.打开OToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("打开OToolStripButton.Image")));
            this.打开OToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.打开OToolStripButton.Name = "打开OToolStripButton";
            this.打开OToolStripButton.Size = new System.Drawing.Size(23, 20);
            this.打开OToolStripButton.Text = "打开(&O)";
            // 
            // 保存SToolStripButton
            // 
            this.保存SToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.保存SToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("保存SToolStripButton.Image")));
            this.保存SToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.保存SToolStripButton.Name = "保存SToolStripButton";
            this.保存SToolStripButton.Size = new System.Drawing.Size(23, 20);
            this.保存SToolStripButton.Text = "保存(&S)";
            // 
            // 打印PToolStripButton
            // 
            this.打印PToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.打印PToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("打印PToolStripButton.Image")));
            this.打印PToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.打印PToolStripButton.Name = "打印PToolStripButton";
            this.打印PToolStripButton.Size = new System.Drawing.Size(23, 20);
            this.打印PToolStripButton.Text = "打印(&P)";
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 23);
            // 
            // 剪切UToolStripButton
            // 
            this.剪切UToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.剪切UToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("剪切UToolStripButton.Image")));
            this.剪切UToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.剪切UToolStripButton.Name = "剪切UToolStripButton";
            this.剪切UToolStripButton.Size = new System.Drawing.Size(23, 20);
            this.剪切UToolStripButton.Text = "剪切(&U)";
            // 
            // 复制CToolStripButton
            // 
            this.复制CToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.复制CToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("复制CToolStripButton.Image")));
            this.复制CToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.复制CToolStripButton.Name = "复制CToolStripButton";
            this.复制CToolStripButton.Size = new System.Drawing.Size(23, 20);
            this.复制CToolStripButton.Text = "复制(&C)";
            // 
            // 粘贴PToolStripButton
            // 
            this.粘贴PToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.粘贴PToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("粘贴PToolStripButton.Image")));
            this.粘贴PToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.粘贴PToolStripButton.Name = "粘贴PToolStripButton";
            this.粘贴PToolStripButton.Size = new System.Drawing.Size(23, 20);
            this.粘贴PToolStripButton.Text = "粘贴(&P)";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // 帮助LToolStripButton
            // 
            this.帮助LToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.帮助LToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("帮助LToolStripButton.Image")));
            this.帮助LToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.帮助LToolStripButton.Name = "帮助LToolStripButton";
            this.帮助LToolStripButton.Size = new System.Drawing.Size(23, 20);
            this.帮助LToolStripButton.Text = "帮助(&L)";
            // 
            // 新建NToolStripButton1
            // 
            this.新建NToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.新建NToolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("新建NToolStripButton1.Image")));
            this.新建NToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.新建NToolStripButton1.Name = "新建NToolStripButton1";
            this.新建NToolStripButton1.Size = new System.Drawing.Size(23, 20);
            this.新建NToolStripButton1.Text = "新建(&N)";
            // 
            // 打开OToolStripButton1
            // 
            this.打开OToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.打开OToolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("打开OToolStripButton1.Image")));
            this.打开OToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.打开OToolStripButton1.Name = "打开OToolStripButton1";
            this.打开OToolStripButton1.Size = new System.Drawing.Size(23, 20);
            this.打开OToolStripButton1.Text = "打开(&O)";
            // 
            // 保存SToolStripButton1
            // 
            this.保存SToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.保存SToolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("保存SToolStripButton1.Image")));
            this.保存SToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.保存SToolStripButton1.Name = "保存SToolStripButton1";
            this.保存SToolStripButton1.Size = new System.Drawing.Size(23, 20);
            this.保存SToolStripButton1.Text = "保存(&S)";
            // 
            // 打印PToolStripButton1
            // 
            this.打印PToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.打印PToolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("打印PToolStripButton1.Image")));
            this.打印PToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.打印PToolStripButton1.Name = "打印PToolStripButton1";
            this.打印PToolStripButton1.Size = new System.Drawing.Size(23, 20);
            this.打印PToolStripButton1.Text = "打印(&P)";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 23);
            // 
            // 剪切UToolStripButton1
            // 
            this.剪切UToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.剪切UToolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("剪切UToolStripButton1.Image")));
            this.剪切UToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.剪切UToolStripButton1.Name = "剪切UToolStripButton1";
            this.剪切UToolStripButton1.Size = new System.Drawing.Size(23, 20);
            this.剪切UToolStripButton1.Text = "剪切(&U)";
            // 
            // 复制CToolStripButton1
            // 
            this.复制CToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.复制CToolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("复制CToolStripButton1.Image")));
            this.复制CToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.复制CToolStripButton1.Name = "复制CToolStripButton1";
            this.复制CToolStripButton1.Size = new System.Drawing.Size(23, 20);
            this.复制CToolStripButton1.Text = "复制(&C)";
            // 
            // 粘贴PToolStripButton1
            // 
            this.粘贴PToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.粘贴PToolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("粘贴PToolStripButton1.Image")));
            this.粘贴PToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.粘贴PToolStripButton1.Name = "粘贴PToolStripButton1";
            this.粘贴PToolStripButton1.Size = new System.Drawing.Size(23, 20);
            this.粘贴PToolStripButton1.Text = "粘贴(&P)";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 23);
            // 
            // 帮助LToolStripButton1
            // 
            this.帮助LToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.帮助LToolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("帮助LToolStripButton1.Image")));
            this.帮助LToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.帮助LToolStripButton1.Name = "帮助LToolStripButton1";
            this.帮助LToolStripButton1.Size = new System.Drawing.Size(23, 20);
            this.帮助LToolStripButton1.Text = "帮助(&L)";
            // 
            // 新建NToolStripButton2
            // 
            this.新建NToolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.新建NToolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("新建NToolStripButton2.Image")));
            this.新建NToolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.新建NToolStripButton2.Name = "新建NToolStripButton2";
            this.新建NToolStripButton2.Size = new System.Drawing.Size(23, 20);
            this.新建NToolStripButton2.Text = "新建(&N)";
            // 
            // 打开OToolStripButton2
            // 
            this.打开OToolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.打开OToolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("打开OToolStripButton2.Image")));
            this.打开OToolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.打开OToolStripButton2.Name = "打开OToolStripButton2";
            this.打开OToolStripButton2.Size = new System.Drawing.Size(23, 20);
            this.打开OToolStripButton2.Text = "打开(&O)";
            // 
            // 保存SToolStripButton2
            // 
            this.保存SToolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.保存SToolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("保存SToolStripButton2.Image")));
            this.保存SToolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.保存SToolStripButton2.Name = "保存SToolStripButton2";
            this.保存SToolStripButton2.Size = new System.Drawing.Size(23, 20);
            this.保存SToolStripButton2.Text = "保存(&S)";
            // 
            // 打印PToolStripButton2
            // 
            this.打印PToolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.打印PToolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("打印PToolStripButton2.Image")));
            this.打印PToolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.打印PToolStripButton2.Name = "打印PToolStripButton2";
            this.打印PToolStripButton2.Size = new System.Drawing.Size(23, 20);
            this.打印PToolStripButton2.Text = "打印(&P)";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 23);
            // 
            // 剪切UToolStripButton2
            // 
            this.剪切UToolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.剪切UToolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("剪切UToolStripButton2.Image")));
            this.剪切UToolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.剪切UToolStripButton2.Name = "剪切UToolStripButton2";
            this.剪切UToolStripButton2.Size = new System.Drawing.Size(23, 20);
            this.剪切UToolStripButton2.Text = "剪切(&U)";
            // 
            // 复制CToolStripButton2
            // 
            this.复制CToolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.复制CToolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("复制CToolStripButton2.Image")));
            this.复制CToolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.复制CToolStripButton2.Name = "复制CToolStripButton2";
            this.复制CToolStripButton2.Size = new System.Drawing.Size(23, 20);
            this.复制CToolStripButton2.Text = "复制(&C)";
            // 
            // 粘贴PToolStripButton2
            // 
            this.粘贴PToolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.粘贴PToolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("粘贴PToolStripButton2.Image")));
            this.粘贴PToolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.粘贴PToolStripButton2.Name = "粘贴PToolStripButton2";
            this.粘贴PToolStripButton2.Size = new System.Drawing.Size(23, 20);
            this.粘贴PToolStripButton2.Text = "粘贴(&P)";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 23);
            // 
            // 帮助LToolStripButton2
            // 
            this.帮助LToolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.帮助LToolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("帮助LToolStripButton2.Image")));
            this.帮助LToolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.帮助LToolStripButton2.Name = "帮助LToolStripButton2";
            this.帮助LToolStripButton2.Size = new System.Drawing.Size(23, 20);
            this.帮助LToolStripButton2.Text = "帮助(&L)";
            // 
            // 新建NToolStripButton3
            // 
            this.新建NToolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.新建NToolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("新建NToolStripButton3.Image")));
            this.新建NToolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.新建NToolStripButton3.Name = "新建NToolStripButton3";
            this.新建NToolStripButton3.Size = new System.Drawing.Size(23, 20);
            this.新建NToolStripButton3.Text = "新建(&N)";
            // 
            // 打开OToolStripButton3
            // 
            this.打开OToolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.打开OToolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("打开OToolStripButton3.Image")));
            this.打开OToolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.打开OToolStripButton3.Name = "打开OToolStripButton3";
            this.打开OToolStripButton3.Size = new System.Drawing.Size(23, 20);
            this.打开OToolStripButton3.Text = "打开(&O)";
            // 
            // 保存SToolStripButton3
            // 
            this.保存SToolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.保存SToolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("保存SToolStripButton3.Image")));
            this.保存SToolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.保存SToolStripButton3.Name = "保存SToolStripButton3";
            this.保存SToolStripButton3.Size = new System.Drawing.Size(23, 20);
            this.保存SToolStripButton3.Text = "保存(&S)";
            // 
            // 打印PToolStripButton3
            // 
            this.打印PToolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.打印PToolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("打印PToolStripButton3.Image")));
            this.打印PToolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.打印PToolStripButton3.Name = "打印PToolStripButton3";
            this.打印PToolStripButton3.Size = new System.Drawing.Size(23, 20);
            this.打印PToolStripButton3.Text = "打印(&P)";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 23);
            // 
            // 剪切UToolStripButton3
            // 
            this.剪切UToolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.剪切UToolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("剪切UToolStripButton3.Image")));
            this.剪切UToolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.剪切UToolStripButton3.Name = "剪切UToolStripButton3";
            this.剪切UToolStripButton3.Size = new System.Drawing.Size(23, 20);
            this.剪切UToolStripButton3.Text = "剪切(&U)";
            // 
            // 复制CToolStripButton3
            // 
            this.复制CToolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.复制CToolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("复制CToolStripButton3.Image")));
            this.复制CToolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.复制CToolStripButton3.Name = "复制CToolStripButton3";
            this.复制CToolStripButton3.Size = new System.Drawing.Size(23, 20);
            this.复制CToolStripButton3.Text = "复制(&C)";
            // 
            // 粘贴PToolStripButton3
            // 
            this.粘贴PToolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.粘贴PToolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("粘贴PToolStripButton3.Image")));
            this.粘贴PToolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.粘贴PToolStripButton3.Name = "粘贴PToolStripButton3";
            this.粘贴PToolStripButton3.Size = new System.Drawing.Size(23, 20);
            this.粘贴PToolStripButton3.Text = "粘贴(&P)";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 23);
            // 
            // 帮助LToolStripButton3
            // 
            this.帮助LToolStripButton3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.帮助LToolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.帮助LToolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("帮助LToolStripButton3.Image")));
            this.帮助LToolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.帮助LToolStripButton3.Name = "帮助LToolStripButton3";
            this.帮助LToolStripButton3.Size = new System.Drawing.Size(23, 20);
            this.帮助LToolStripButton3.Text = "帮助(&L)";
            // 
            // TestForm
            // 
            this.ClientSize = new System.Drawing.Size(514, 343);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.taskBar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "TestForm";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.DrawString("Hello,World", Font, Brushes.Blue, 50, 50);
            e.Graphics.FillRectangle(Brushes.Transparent, new Rectangle(10, 10, 60, 60));
        }
    }
}
