// Decompiled with JetBrains decompiler
// Type: Pereverter.LongRead
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
  public class LongRead : Form
  {
    private Form1 PAPA;
    private IContainer components;
    private DataGridView dataGridView1;
    private Button button1;
    private DataGridViewTextBoxColumn Column1;
    private DataGridViewTextBoxColumn Column2;
    private DataGridViewTextBoxColumn Column3;

    public LongRead(Form1 papula)
    {
      this.PAPA = papula;
      this.InitializeComponent();
    }

    private void LongRead_Load(object sender, EventArgs e)
    {
      for (int index = 0; index < this.PAPA.tr.strs.Count; ++index)
      {
        if (this.PAPA.tr.strs[index].Items.Length == 3)
          this.dataGridView1.Rows.Add((object[]) this.PAPA.tr.strs[index].Items);
        else
          this.PAPA.tr.strs.RemoveAt(index);
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.PAPA.longi = new List<Str>((IEnumerable<Str>) this.PAPA.tr.strs);
      this.PAPA.textBox1.AppendText("Файл с протяжённостями открыт: " + this.PAPA.longi[0].Items[0] + "km - " + this.PAPA.longi[this.PAPA.longi.Count - 1].Items[1] + "km\r\n");
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
      this.dataGridView1 = new DataGridView();
      this.button1 = new Button();
      this.Column1 = new DataGridViewTextBoxColumn();
      this.Column2 = new DataGridViewTextBoxColumn();
      this.Column3 = new DataGridViewTextBoxColumn();
      ((ISupportInitialize) this.dataGridView1).BeginInit();
      this.SuspendLayout();
      this.dataGridView1.AllowUserToAddRows = false;
      this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Columns.AddRange((DataGridViewColumn) this.Column1, (DataGridViewColumn) this.Column2, (DataGridViewColumn) this.Column3);
      this.dataGridView1.Location = new Point(0, 0);
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.RowHeadersVisible = false;
      this.dataGridView1.RowTemplate.Height = 18;
      this.dataGridView1.RowTemplate.ReadOnly = true;
      this.dataGridView1.RowTemplate.Resizable = DataGridViewTriState.False;
      this.dataGridView1.Size = new Size(339, 533);
      this.dataGridView1.TabIndex = 0;
      this.button1.BackColor = Color.Gold;
      this.button1.Location = new Point(0, 529);
      this.button1.Name = "button1";
      this.button1.Size = new Size(339, 57);
      this.button1.TabIndex = 1;
      this.button1.Text = "Сохранить";
      this.button1.UseVisualStyleBackColor = false;
      this.button1.Click += new EventHandler(this.button1_Click);
      this.Column1.HeaderText = "Нач. км.";
      this.Column1.Name = "Column1";
      this.Column2.HeaderText = "Кон. км.";
      this.Column2.Name = "Column2";
      this.Column3.HeaderText = "Протяжённость";
      this.Column3.Name = "Column3";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(341, 588);
      this.Controls.Add((Control) this.button1);
      this.Controls.Add((Control) this.dataGridView1);
      this.Name = nameof (LongRead);
      this.Text = nameof (LongRead);
      this.Load += new EventHandler(this.LongRead_Load);
      ((ISupportInitialize) this.dataGridView1).EndInit();
      this.ResumeLayout(false);
    }
  }
}
