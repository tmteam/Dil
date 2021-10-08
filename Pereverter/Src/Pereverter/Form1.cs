// Decompiled with JetBrains decompiler
// Type: Pereverter.Form1
// Assembly: Pereverter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CB127099-1D57-4175-847F-3E155E90579B
// Assembly location: T:\Code\Deda\Pereverter\Pereverter alpha 0.2\Pereverter alpha 0.2\Pereverter.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Pereverter
{
  public class Form1 : Form
  {
    public txtReader tr = new txtReader();
    public List<Str> longi = new List<Str>();
    public KilometersTable Dano = new KilometersTable();
    public KilometersTable Otvet = new KilometersTable();
    private IContainer components;
    private MenuStrip menuStrip1;
    private ToolStripMenuItem данныеToolStripMenuItem;
    private ToolStripMenuItem считатьДанныеToolStripMenuItem;
    private ToolStripMenuItem считатьПротяжённостиToolStripMenuItem;
    public TextBox textBox1;
    private Button button1;
    private DataGridView dataGridView1;
    private CheckBox checkBox1;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.menuStrip1 = new MenuStrip();
      this.данныеToolStripMenuItem = new ToolStripMenuItem();
      this.считатьДанныеToolStripMenuItem = new ToolStripMenuItem();
      this.считатьПротяжённостиToolStripMenuItem = new ToolStripMenuItem();
      this.textBox1 = new TextBox();
      this.button1 = new Button();
      this.dataGridView1 = new DataGridView();
      this.checkBox1 = new CheckBox();
      this.menuStrip1.SuspendLayout();
      ((ISupportInitialize) this.dataGridView1).BeginInit();
      this.SuspendLayout();
      this.menuStrip1.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.данныеToolStripMenuItem
      });
      this.menuStrip1.Location = new Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new Size(876, 24);
      this.menuStrip1.TabIndex = 0;
      this.menuStrip1.Text = "menuStrip1";
      this.данныеToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.считатьДанныеToolStripMenuItem,
        (ToolStripItem) this.считатьПротяжённостиToolStripMenuItem
      });
      this.данныеToolStripMenuItem.Name = "данныеToolStripMenuItem";
      this.данныеToolStripMenuItem.Size = new Size(59, 20);
      this.данныеToolStripMenuItem.Text = "Данные";
      this.считатьДанныеToolStripMenuItem.Name = "считатьДанныеToolStripMenuItem";
      this.считатьДанныеToolStripMenuItem.Size = new Size(210, 22);
      this.считатьДанныеToolStripMenuItem.Text = "Считать данные";
      this.считатьДанныеToolStripMenuItem.Click += new EventHandler(this.считатьДанныеToolStripMenuItem_Click);
      this.считатьПротяжённостиToolStripMenuItem.Name = "считатьПротяжённостиToolStripMenuItem";
      this.считатьПротяжённостиToolStripMenuItem.Size = new Size(210, 22);
      this.считатьПротяжённостиToolStripMenuItem.Text = "Считать протяжённости";
      this.считатьПротяжённостиToolStripMenuItem.Click += new EventHandler(this.считатьПротяжённостиToolStripMenuItem_Click);
      this.textBox1.BackColor = Color.Black;
      this.textBox1.Font = new Font("Courier New", 9.75f, FontStyle.Regular, GraphicsUnit.Point, (byte) 204);
      this.textBox1.ForeColor = Color.Chartreuse;
      this.textBox1.Location = new Point(0, 473);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.ReadOnly = true;
      this.textBox1.Size = new Size(864, 117);
      this.textBox1.TabIndex = 1;
      this.button1.BackColor = Color.Orange;
      this.button1.ForeColor = Color.Black;
      this.button1.Location = new Point(634, 415);
      this.button1.Name = "button1";
      this.button1.Size = new Size(230, 52);
      this.button1.TabIndex = 2;
      this.button1.Text = "Посчитать!!!";
      this.button1.UseVisualStyleBackColor = false;
      this.button1.Click += new EventHandler(this.button1_Click);
      this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Location = new Point(19, 27);
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.Size = new Size(845, 382);
      this.dataGridView1.TabIndex = 3;
      this.checkBox1.AutoSize = true;
      this.checkBox1.Location = new Point(19, 434);
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.Size = new Size(227, 17);
      this.checkBox1.TabIndex = 4;
      this.checkBox1.Text = "Километры и метры в разных столбцах";
      this.checkBox1.UseVisualStyleBackColor = true;
      this.checkBox1.CheckedChanged += new EventHandler(this.checkBox1_CheckedChanged);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(876, 602);
      this.Controls.Add((Control) this.checkBox1);
      this.Controls.Add((Control) this.dataGridView1);
      this.Controls.Add((Control) this.button1);
      this.Controls.Add((Control) this.textBox1);
      this.Controls.Add((Control) this.menuStrip1);
      this.MainMenuStrip = this.menuStrip1;
      this.Name = nameof (Form1);
      this.Text = "Перевёртер А0.1";
      this.Load += new EventHandler(this.Form1_Load);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      ((ISupportInitialize) this.dataGridView1).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public Form1()
    {
      this.InitializeComponent();
    }

    private void считатьДанныеToolStripMenuItem_Click(object sender, EventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      if (openFileDialog.ShowDialog() != DialogResult.OK)
        return;
      this.tr.Read(openFileDialog.FileName);
      int num = (int) new DataTypeSelector(this).ShowDialog();
    }

    private void считатьПротяжённостиToolStripMenuItem_Click(object sender, EventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      if (openFileDialog.ShowDialog() != DialogResult.OK)
        return;
      this.tr.Read(openFileDialog.FileName);
      int num = (int) new LongRead(this).ShowDialog();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
    }

    private void ShowFinalTable()
    {
      this.Otvet.Headers = this.Dano.Headers;
      this.Otvet.EndsKmMExist = this.Dano.EndsKmMExist;
      this.Otvet.KillometersAndMettersInSingleCell = !this.checkBox1.Checked;
      for (int index1 = this.Dano.Rows.Count - 1; index1 >= 0; --index1)
      {
        KilometerRow kilometerRow = new KilometerRow();
        kilometerRow.Data = this.Dano.Rows[index1].Data;
        if (this.Otvet.EndsKmMExist)
        {
          for (int index2 = 0; index2 < this.longi.Count; ++index2)
          {
            if ((int) Convert.ToInt16(this.longi[index2].Items[0]) < this.Dano.Rows[index1].EndKilometer && (int) Convert.ToInt16(this.longi[index2].Items[1]) >= this.Dano.Rows[index1].EndKilometer)
            {
              kilometerRow.BeginKilometer = (int) Convert.ToInt16(this.longi[index2].Items[0]);
              kilometerRow.BeginMeter = (int) Convert.ToInt16(this.longi[index2].Items[2]) - this.Dano.Rows[index1].EndMeter;
            }
          }
          for (int index2 = 0; index2 < this.longi.Count; ++index2)
          {
            if ((int) Convert.ToInt16(this.longi[index2].Items[0]) < this.Dano.Rows[index1].BeginKilometer && (int) Convert.ToInt16(this.longi[index2].Items[1]) >= this.Dano.Rows[index1].BeginKilometer)
            {
              kilometerRow.EndKilometer = (int) Convert.ToInt16(this.longi[index2].Items[0]);
              kilometerRow.EndMeter = (int) Convert.ToInt16(this.longi[index2].Items[2]) - this.Dano.Rows[index1].BeginMeter;
            }
          }
        }
        else
        {
          for (int index2 = 0; index2 < this.longi.Count; ++index2)
          {
            if ((int) Convert.ToInt16(this.longi[index2].Items[0]) < this.Dano.Rows[index1].BeginKilometer && (int) Convert.ToInt16(this.longi[index2].Items[1]) >= this.Dano.Rows[index1].BeginKilometer)
            {
              kilometerRow.BeginKilometer = (int) Convert.ToInt16(this.longi[index2].Items[0]);
              kilometerRow.BeginMeter = (int) Convert.ToInt16(this.longi[index2].Items[2]) - this.Dano.Rows[index1].BeginMeter;
            }
          }
        }
        this.Otvet.Rows.Add(kilometerRow);
      }
      this.ShowTable(this.Otvet);
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.ShowFinalTable();
    }

    private KilometersTable TableParse(List<Str> strs, bool isexistintervals)
    {
      KilometersTable kilometersTable = new KilometersTable();
      int num1 = -1;
      bool flag = false;
      for (int index = 0; index < strs.Count; ++index)
      {
        if (strs[index].Items.Length > 0)
        {
          string[] strArray = strs[index].Items[0].Split(' ');
          List<string> stringList = new List<string>();
          foreach (string str in strArray)
          {
            if (str != "")
              stringList.Add(str);
          }
          int result;
          if (stringList.Count == 1)
          {
            if (int.TryParse(stringList[0], out result) && int.TryParse(strs[index].Items[1], out result))
            {
              num1 = index;
              flag = false;
              break;
            }
          }
          else if (stringList.Count == 2 && int.TryParse(stringList[0], out result) && int.TryParse(stringList[1], out result))
          {
            num1 = index;
            flag = true;
            break;
          }
        }
      }
      kilometersTable.EndsKmMExist = isexistintervals;
      kilometersTable.KillometersAndMettersInSingleCell = this.checkBox1.Checked;
      int num2 = -1;
      if (num1 != -1)
      {
        for (int index = 0; index < num1; ++index)
          kilometersTable.Headers.Add(new StrL(strs[index].Items));
        if (this.checkBox1.Checked && flag)
        {
          for (int index = 0; index < num1; ++index)
          {
            if (isexistintervals)
            {
              if (kilometersTable.Headers[index].Data.Count > 3)
              {
                kilometersTable.Headers[index].Data.Insert(1, "");
                kilometersTable.Headers[index].Data.Insert(3, "");
              }
              else
              {
                kilometersTable.Headers[index].Data.Add(" ");
                kilometersTable.Headers[index].Data.Add(" ");
              }
            }
            else if (kilometersTable.Headers[index].Data.Count > 1)
              kilometersTable.Headers[index].Data.Insert(1, "");
            else
              kilometersTable.Headers[index].Data.Add(" ");
          }
        }
        for (int index1 = num1; index1 < strs.Count; ++index1)
        {
          KilometerRow kilometerRow = new KilometerRow();
          if (kilometersTable.KillometersAndMettersInSingleCell)
          {
            string[] strArray1 = strs[index1].Items[0].Split(' ');
            List<string> stringList1 = new List<string>();
            foreach (string str in strArray1)
            {
              if (str != "")
                stringList1.Add(str);
            }
            if (stringList1.Count >= 2)
            {
              int.TryParse(stringList1[0], out kilometerRow.BeginKilometer);
              if (!int.TryParse(stringList1[1], out kilometerRow.BeginMeter))
                kilometerRow.BeginMeter = 0;
              num2 = 1;
            }
            else if (stringList1.Count == 1)
            {
              int.TryParse(stringList1[0], out kilometerRow.BeginKilometer);
              kilometerRow.BeginMeter = 0;
              num2 = 1;
            }
            if (isexistintervals)
            {
              string[] strArray2 = strs[index1].Items[1].Split(' ');
              List<string> stringList2 = new List<string>();
              foreach (string str in strArray2)
              {
                if (str != "")
                  stringList2.Add(str);
              }
              num2 = 2;
              if (stringList2.Count >= 2)
              {
                int.TryParse(stringList2[0], out kilometerRow.EndKilometer);
                if (!int.TryParse(stringList2[1], out kilometerRow.EndMeter))
                  kilometerRow.EndMeter = 0;
              }
              else if (stringList2.Count == 1)
              {
                int.TryParse(stringList2[0], out kilometerRow.EndKilometer);
                kilometerRow.EndMeter = 0;
              }
            }
          }
          else
          {
            int.TryParse(strs[index1].Items[0], out kilometerRow.BeginKilometer);
            int.TryParse(strs[index1].Items[1], out kilometerRow.BeginMeter);
            num2 = 2;
            if (isexistintervals)
            {
              num2 = 4;
              int.TryParse(strs[index1].Items[2], out kilometerRow.EndKilometer);
              int.TryParse(strs[index1].Items[3], out kilometerRow.EndMeter);
            }
          }
          kilometerRow.Data = new string[kilometersTable.Headers[0].Data.Count - num2];
          for (int index2 = num2; index2 < strs[index1].Items.Length; ++index2)
            kilometerRow.Data[index2 - num2] = strs[index1].Items[index2];
          kilometersTable.Rows.Add(kilometerRow);
        }
      }
      return kilometersTable;
    }

    private void ShowTable(KilometersTable showKilometersTable)
    {
      this.dataGridView1.Columns.Clear();
      if (showKilometersTable.Headers.Count > 0)
      {
        int num = 0;
        foreach (string str in showKilometersTable.Headers[0].Data)
        {
          if (this.checkBox1.Checked && this.Dano.KillometersAndMettersInSingleCell)
          {
            if (this.Dano.EndsKmMExist)
            {
              if (num == 1 || num == 3)
              {
                this.dataGridView1.Columns.Add((DataGridViewColumn) new DataGridViewTextBoxColumn());
                ++num;
              }
            }
            else if (num == 1)
            {
              this.dataGridView1.Columns.Add((DataGridViewColumn) new DataGridViewTextBoxColumn());
              ++num;
            }
          }
          this.dataGridView1.Columns.Add((DataGridViewColumn) new DataGridViewTextBoxColumn());
          ++num;
        }
      }
      for (int index1 = 0; index1 < showKilometersTable.Headers.Count; ++index1)
      {
        if (this.checkBox1.Checked && this.Dano.KillometersAndMettersInSingleCell)
        {
          if (this.Dano.EndsKmMExist)
          {
            string[] strArray = new string[showKilometersTable.Headers[index1].Data.Count + 2];
            int index2 = 0;
            for (int index3 = 0; index3 < strArray.Length; ++index3)
            {
              if (index3 == 1 || index3 == 3)
              {
                strArray[index3] = "";
              }
              else
              {
                strArray[index3] = showKilometersTable.Headers[index1].Data[index2];
                ++index2;
              }
            }
            this.dataGridView1.Rows.Add((object[]) strArray);
          }
          else
          {
            string[] strArray = new string[showKilometersTable.Headers[index1].Data.Count + 1];
            int index2 = 0;
            for (int index3 = 0; index3 < strArray.Length; ++index3)
            {
              if (index3 == 1)
              {
                strArray[index3] = "";
              }
              else
              {
                strArray[index3] = showKilometersTable.Headers[index1].Data[index2];
                ++index2;
              }
            }
            this.dataGridView1.Rows.Add((object[]) strArray);
          }
        }
        else
        {
          string[] strArray = new string[showKilometersTable.Headers[index1].Data.Count];
          for (int index2 = 0; index2 < strArray.Length; ++index2)
            strArray[index2] = showKilometersTable.Headers[index1].Data[index2];
          this.dataGridView1.Rows.Add((object[]) strArray);
        }
        this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].DefaultCellStyle.BackColor = Color.Gray;
      }
      for (int index = 0; index < showKilometersTable.Rows.Count; ++index)
        this.dataGridView1.Rows.Add();
      for (int index1 = 0; index1 < showKilometersTable.Rows.Count; ++index1)
      {
        int num;
        if (showKilometersTable.KillometersAndMettersInSingleCell)
        {
          this.dataGridView1.Rows[index1 + showKilometersTable.Headers.Count].Cells[0].Style.BackColor = Color.Green;
          this.dataGridView1.Rows[index1 + showKilometersTable.Headers.Count].Cells[0].Value = (object) (showKilometersTable.Rows[index1].BeginKilometer.ToString() + " " + showKilometersTable.Rows[index1].BeginMeter.ToString());
          num = 1;
          if (showKilometersTable.EndsKmMExist)
          {
            this.dataGridView1.Rows[index1 + showKilometersTable.Headers.Count].Cells[1].Value = (object) (showKilometersTable.Rows[index1].EndKilometer.ToString() + " " + showKilometersTable.Rows[index1].EndMeter.ToString());
            this.dataGridView1.Rows[index1 + showKilometersTable.Headers.Count].Cells[1].Style.BackColor = Color.Yellow;
            num = 2;
          }
        }
        else
        {
          this.dataGridView1.Rows[index1 + showKilometersTable.Headers.Count].Cells[0].Value = (object) showKilometersTable.Rows[index1].BeginKilometer.ToString();
          this.dataGridView1.Rows[index1 + showKilometersTable.Headers.Count].Cells[1].Value = (object) showKilometersTable.Rows[index1].BeginMeter.ToString();
          this.dataGridView1.Rows[index1 + showKilometersTable.Headers.Count].Cells[0].Style.BackColor = Color.Green;
          this.dataGridView1.Rows[index1 + showKilometersTable.Headers.Count].Cells[1].Style.BackColor = Color.Green;
          num = 2;
          if (showKilometersTable.EndsKmMExist)
          {
            this.dataGridView1.Rows[index1 + showKilometersTable.Headers.Count].Cells[2].Value = (object) showKilometersTable.Rows[index1].EndKilometer.ToString();
            this.dataGridView1.Rows[index1 + showKilometersTable.Headers.Count].Cells[2].Style.BackColor = Color.Yellow;
            this.dataGridView1.Rows[index1 + showKilometersTable.Headers.Count].Cells[3].Value = (object) showKilometersTable.Rows[index1].EndMeter.ToString();
            this.dataGridView1.Rows[index1 + showKilometersTable.Headers.Count].Cells[3].Style.BackColor = Color.Yellow;
            num = 4;
          }
        }
        for (int index2 = num; index2 < showKilometersTable.Rows[index1].Data.Length + num; ++index2)
          this.dataGridView1.Rows[index1 + showKilometersTable.Headers.Count].Cells[index2].Value = (object) showKilometersTable.Rows[index1].Data[index2 - num];
      }
    }

    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
      this.ShowFinalTable();
    }
  }
}
