// Decompiled with JetBrains decompiler
// Type: Pereverter.DataTypeSelector
// Assembly: Pereverter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CB127099-1D57-4175-847F-3E155E90579B
// Assembly location: T:\Code\Deda\Pereverter\Pereverter alpha 0.2\Pereverter alpha 0.2\Pereverter.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Pereverter
{
  public class DataTypeSelector : Form
  {
    private KilometersTable t = new KilometersTable();
    private Form1 PAPA;
    private IContainer components;
    private CheckBox checkBox1;
    private DataGridView dataGridView1;
    private DataGridViewTextBoxColumn KmBegin;
    private DataGridViewTextBoxColumn MB;
    private Button button1;

    public DataTypeSelector(Form1 papula)
    {
      this.PAPA = papula;
      this.InitializeComponent();
    }

    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
      this.t = this.TableParse(this.PAPA.tr.strs, this.checkBox1.Checked);
      this.ShowTable(this.t);
    }

    private void DataTypeSelector_Load(object sender, EventArgs e)
    {
      this.t = this.TableParse(this.PAPA.tr.strs, this.checkBox1.Checked);
      this.ShowTable(this.t);
    }

    private KilometersTable TableParse(List<Str> strs, bool hasIntervals)
    {
      var kilometersTable = new KilometersTable();
      int num1 = -1;
      bool killometersAndMettersInSingleCell = false;
      for (int index = 0; index < strs.Count; ++index)
      {
        if (strs[index].Items.Length > 0)
        {
          var strArray = strs[index].Items[0].Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries );
          
          int result;
          if (strArray.Length == 1)
          {
            if (int.TryParse(strArray[0], out result) && int.TryParse(strs[index].Items[1], out result))
            {
              num1 = index;
              killometersAndMettersInSingleCell = false;
              break;
            }
          }
          else if (strArray.Length == 2 && int.TryParse(strArray[0], out result) && int.TryParse(strArray[1], out result))
          {
            num1 = index;
            killometersAndMettersInSingleCell = true;
            break;
          }
        }
      }
      kilometersTable.EndsKmMExist = hasIntervals;
      kilometersTable.KillometersAndMettersInSingleCell = killometersAndMettersInSingleCell;
      
      int num2 = -1;
      if (num1 != -1)
      {
        for (int index = 0; index < num1; ++index)
          kilometersTable.Headers.Add(new StrL(strs[index].Items));
        for (int index1 = num1; index1 < strs.Count; ++index1)
        {
          KilometerRow kilometerRow = new KilometerRow();
          if (killometersAndMettersInSingleCell)
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
            if (hasIntervals)
            {
              var strArray2 = strs[index1].Items[1].Split(new[]{' '}, StringSplitOptions.RemoveEmptyEntries );
              num2 = 2;
              if (strArray2.Length >= 2)
              {
                int.TryParse(strArray2[0], out kilometerRow.EndKilometer);
                if (!int.TryParse(strArray2[1], out kilometerRow.EndMeter))
                  kilometerRow.EndMeter = 0;
              }
              else if (strArray2.Length == 1)
              {
                int.TryParse(strArray2[0], out kilometerRow.EndKilometer);
                kilometerRow.EndMeter = 0;
              }
            }
          }
          else
          {
            int.TryParse(strs[index1].Items[0], out kilometerRow.BeginKilometer);
            int.TryParse(strs[index1].Items[1], out kilometerRow.BeginMeter);
            num2 = 2;
            if (hasIntervals)
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
      foreach (string str in showKilometersTable.Headers[0].Data)
        this.dataGridView1.Columns.Add((DataGridViewColumn) new DataGridViewTextBoxColumn());
      for (int index = 0; index < showKilometersTable.Headers.Count; ++index)
      {
        this.dataGridView1.Rows.Add((object) showKilometersTable.Headers[index].Data);
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
          this.dataGridView1.Rows[index1 + showKilometersTable.Headers.Count].Cells[0].Value = (object) (showKilometersTable.Rows[index1].BeginKilometer.ToString() + " +" + showKilometersTable.Rows[index1].BeginMeter.ToString());
          num = 1;
          if (showKilometersTable.EndsKmMExist)
          {
            this.dataGridView1.Rows[index1 + showKilometersTable.Headers.Count].Cells[1].Value = (object) (showKilometersTable.Rows[index1].EndKilometer.ToString() + " +" + showKilometersTable.Rows[index1].EndMeter.ToString());
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

    private void button1_Click(object sender, EventArgs e)
    {
      this.PAPA.Dano = this.t;
      this.PAPA.textBox1.AppendText("Файл с данными открыт: [" + (object) this.t.Rows[0].BeginKilometer + " +" + (object) this.t.Rows[0].BeginMeter + "]  -  [" + (object) this.t.Rows[this.t.Rows.Count - 1].BeginKilometer + " +" + (object) this.t.Rows[this.t.Rows.Count - 1].BeginMeter + "]\r\n");
      this.Close();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.checkBox1 = new CheckBox();
      this.dataGridView1 = new DataGridView();
      this.KmBegin = new DataGridViewTextBoxColumn();
      this.MB = new DataGridViewTextBoxColumn();
      this.button1 = new Button();
      ((ISupportInitialize) this.dataGridView1).BeginInit();
      this.SuspendLayout();
      this.checkBox1.AutoSize = true;
      this.checkBox1.Location = new Point(506, 492);
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.Size = new Size(170, 17);
      this.checkBox1.TabIndex = 0;
      this.checkBox1.Text = "Присутствует конец участка";
      this.checkBox1.UseVisualStyleBackColor = true;
      this.checkBox1.CheckedChanged += new EventHandler(this.checkBox1_CheckedChanged);
      this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Columns.AddRange((DataGridViewColumn) this.KmBegin, (DataGridViewColumn) this.MB);
      this.dataGridView1.Location = new Point(12, 12);
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.RowHeadersVisible = false;
      this.dataGridView1.RowTemplate.Height = 18;
      this.dataGridView1.RowTemplate.Resizable = DataGridViewTriState.False;
      this.dataGridView1.Size = new Size(664, 462);
      this.dataGridView1.TabIndex = 1;
      this.KmBegin.HeaderText = "KmB";
      this.KmBegin.Name = "KmBegin";
      this.MB.HeaderText = "MB";
      this.MB.Name = "MB";
      this.button1.BackColor = Color.Gold;
      this.button1.Location = new Point(12, 480);
      this.button1.Name = "button1";
      this.button1.Size = new Size(488, 39);
      this.button1.TabIndex = 2;
      this.button1.Text = "Сохранить";
      this.button1.UseVisualStyleBackColor = false;
      this.button1.Click += new EventHandler(this.button1_Click);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(688, 531);
      this.Controls.Add((Control) this.button1);
      this.Controls.Add((Control) this.dataGridView1);
      this.Controls.Add((Control) this.checkBox1);
      this.Name = nameof (DataTypeSelector);
      this.Text = nameof (DataTypeSelector);
      this.Load += new EventHandler(this.DataTypeSelector_Load);
      ((ISupportInitialize) this.dataGridView1).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
