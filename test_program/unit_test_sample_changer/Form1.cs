using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace unit_test_sample_changer
{
  public partial class Form1 : Form
  {
    device dev = null;
    DevCommand cmd = new DevCommand();
    DeviceState dev_sts = new DeviceState();
    public Form1()
    {
      InitializeComponent();
    }

    void ehDevGetId(object sender, OnGetDeviceIdArgs args)
    {
      this.BeginInvoke((MethodInvoker)delegate
      {
        toolStripStatusLabel1.Text = args.Id;
      });
    }

    void ehDevGetDeviceState(object sender, OnDeviceGetStateArgs args)
    {
      this.BeginInvoke((MethodInvoker)delegate
      {
        dev_sts = args.sts;
        label1.Text = "Cell = " + dev_sts.cell_state.ToString();
        label2.Text = "Lift = " + dev_sts.lift_state.ToString();
        label3.Text = "Disk = " + dev_sts.disk_state.ToString();
      });
    }

    private void button1_Click(object sender, EventArgs e)
    {
      cmd.command = DevCommands.SetLiftDown;
      dev.InsertCommand(cmd);
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (writer != null)
        try
        {
          writer.WriteLine("Total tests\t{0}", test_count.ToString());
          writer.WriteLine("Total erros\t{0}\n", error_count.ToString());
          writer.WriteLine("End test\t{0}", DateTime.Now.ToString());
          writer.Close();
        }
        catch (Exception ea) { }
      dev.Dispose();
    }

    private void Form1_Shown(object sender, EventArgs e)
    {
      dev = new device();
      dev.GetDeviceId += ehDevGetId;
      dev.GetDeviceState += ehDevGetDeviceState;
    }

    private void button2_Click(object sender, EventArgs e)
    {
      cmd.command = DevCommands.SetLiftUp;
      dev.InsertCommand(cmd);
    }

    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
      if (checkBox1.Checked)
      {
        byte[] data = new byte[1] { (byte)numericUpDown1.Value };
        cmd.data = data;
        cmd.command = DevCommands.SetSpinnerOn;
        dev.InsertCommand(cmd);
      }
      else
      {
        cmd.command = DevCommands.SetSpinnerOff;
        dev.InsertCommand(cmd);
      }
    }

    private void numericUpDown1_KeyUp(object sender, KeyEventArgs e)
    {
      if ((checkBox1.Checked)&&(e.KeyData == Keys.Enter))
      {
        byte[] data = new byte[1] { (byte)numericUpDown1.Value };
        cmd.data = data;
        cmd.command = DevCommands.SetSpinnerOn;
        dev.InsertCommand(cmd);
      }
    }

    private void radioButton1_CheckedChanged(object sender, EventArgs e)
    {
      if (radioButton1.Checked) {
        radioButton2.Checked = false;
        cmd.command = DevCommands.SetLiftUp;
        dev.InsertCommand(cmd);
      }
    }

    private void radioButton2_CheckedChanged(object sender, EventArgs e)
    {
      if (radioButton2.Checked)
      {
        radioButton1.Checked = false;
        cmd.command = DevCommands.SetLiftDown;
        dev.InsertCommand(cmd);
      }
    }

    private void radioButton3_CheckedChanged(object sender, EventArgs e)
    {
      cmd.command = DevCommands.SetSampleChanger;
      cmd.data = (byte)1;
      dev.InsertCommand(cmd);
    }

    private void radioButton4_CheckedChanged(object sender, EventArgs e)
    {
      cmd.command = DevCommands.SetSampleChanger;
      cmd.data = (byte)2;
      dev.InsertCommand(cmd);
    }

    private void radioButton5_CheckedChanged(object sender, EventArgs e)
    {
      cmd.command = DevCommands.SetSampleChanger;
      cmd.data = (byte)3;
      dev.InsertCommand(cmd);
    }

    private void radioButton6_CheckedChanged(object sender, EventArgs e)
    {
      cmd.command = DevCommands.SetSampleChanger;
      cmd.data = (byte)4;
      dev.InsertCommand(cmd);
    }

    private void radioButton7_CheckedChanged(object sender, EventArgs e)
    {
      cmd.command = DevCommands.SetSampleChanger;
      cmd.data = (byte)5;
      dev.InsertCommand(cmd);
    }

    private void radioButton8_CheckedChanged(object sender, EventArgs e)
    {
      cmd.command = DevCommands.SetSampleChanger;
      cmd.data = (byte)6;
      dev.InsertCommand(cmd);
    }

    private void radioButton9_CheckedChanged(object sender, EventArgs e)
    {
      cmd.command = DevCommands.SetSampleChanger;
      cmd.data = (byte)7;
      dev.InsertCommand(cmd);
    }

    private void radioButton10_CheckedChanged(object sender, EventArgs e)
    {
      cmd.command = DevCommands.SetSampleChanger;
      cmd.data = (byte)8;
      dev.InsertCommand(cmd);
    }

    private void radioButton11_CheckedChanged(object sender, EventArgs e)
    {
      cmd.command = DevCommands.SetSampleChanger;
      cmd.data = (byte)9;
      dev.InsertCommand(cmd);
    }

    private void radioButton12_CheckedChanged(object sender, EventArgs e)
    {
      cmd.command = DevCommands.SetSampleChanger;
      cmd.data = (byte)10;
      dev.InsertCommand(cmd);
    }

    private void radioButton13_CheckedChanged(object sender, EventArgs e)
    {
      cmd.command = DevCommands.SetSampleChanger;
      cmd.data = (byte)11;
      dev.InsertCommand(cmd);
    }

    private void radioButton14_CheckedChanged(object sender, EventArgs e)
    {
      cmd.command = DevCommands.SetSampleChanger;
      cmd.data = (byte)12;
      dev.InsertCommand(cmd);
    }

    private void radioButton15_CheckedChanged(object sender, EventArgs e)
    {
      cmd.command = DevCommands.SetSampleChanger;
      cmd.data = (byte)13;
      dev.InsertCommand(cmd);
    }

    private void radioButton16_CheckedChanged(object sender, EventArgs e)
    {
      cmd.command = DevCommands.SetSampleChanger;
      cmd.data = (byte)14;
      dev.InsertCommand(cmd);
    }

    private void radioButton17_CheckedChanged(object sender, EventArgs e)
    {
      cmd.command = DevCommands.SetSampleChanger;
      cmd.data = (byte)15;
      dev.InsertCommand(cmd);
    }

    private void radioButton18_CheckedChanged(object sender, EventArgs e)
    {
      cmd.command = DevCommands.SetSampleChanger;
      cmd.data = (byte)16;
      dev.InsertCommand(cmd);
    }

    private void UpdateRandomPosition(byte p) {
      if (p == 1)
        radioButton3.Checked = true;
      if (p == 2)
        radioButton4.Checked = true;
      if (p == 3)
        radioButton5.Checked = true;
      if (p == 4)
        radioButton6.Checked = true;
      if (p == 5)
        radioButton7.Checked = true;
      if (p == 6)
        radioButton8.Checked = true;
      if (p == 7)
        radioButton9.Checked = true;
      if (p == 8)
        radioButton10.Checked = true;
      if (p == 9)
        radioButton11.Checked = true;
      if (p == 10)
        radioButton12.Checked = true;
      if (p == 11)
        radioButton13.Checked = true;
      if (p == 12)
        radioButton14.Checked = true;
      if (p == 13)
        radioButton15.Checked = true;
      if (p == 14)
        radioButton16.Checked = true;
      if (p == 15)
        radioButton17.Checked = true;
      if (p == 16)
        radioButton18.Checked = true;
    }

    int time = 0, constTime = 0;
    int timeout = 0, constTimeout = 0;
    byte state = 0;
    byte position = 0;
    uint test_count = 0;
    uint error_count = 0;
    private void timer1_Tick(object sender, EventArgs e)
    {
      
      Random rnd = new Random();
      if (checkBox2.Checked)
      {
        switch (state)
        {
          case 0:
            timeout = constTimeout;
            state = 1;
            position = (byte)rnd.Next(1, 16);
            cmd.data = position;
            cmd.command = DevCommands.SetSampleChanger;
            dev.InsertCommand(cmd);
            UpdateRandomPosition(position);
            test_count++;
            label6.Text = "Test count = " + test_count.ToString();
            break;
          case 1:
            if ((dev_sts.disk_state == DiskState.DiskOk)||(dev_sts.disk_state == DiskState.DiskReady)){
              time = constTime;
              state = 2;
              if(writer != null)
                writer.WriteLine("\tN:{4}\tpos:{0}\tdisk:{1}\tlift:{2}\tspiner:{3} ", position, dev_sts.disk_state.ToString(), dev_sts.lift_state.ToString(), dev_sts.cell_state.ToString(),test_count.ToString());
            }
            if (timeout == 0)
              state = 255;
            break;
          case 2:
            if (time == 0)
              state = 0;
            if (timeout == 0)
              state = 255;
            break;
          case 255:
            if (writer != null) {
              error_count++;
              label7.Text = "Error count = " + error_count.ToString();
              writer.WriteLine("Errort\t{0}", DateTime.Now.ToString());
              writer.WriteLine("\tN:{4}\tpos:{0}\tdisk:{1}\tlift:{2}\tspiner:{3}, ", position, dev_sts.disk_state.ToString(), dev_sts.lift_state.ToString(), dev_sts.cell_state.ToString(), test_count.ToString());
            }
              state = 0;
            break;
        }
        if (time != 0)
          time--;
        if (timeout != 0)
          timeout--;
      }
    }

    System.IO.StreamWriter writer = null;
    private void checkBox2_CheckedChanged(object sender, EventArgs e)
    {
      if (checkBox2.Checked)
      {
        test_count = 0;
        error_count = 0;
        constTimeout = (byte)numericUpDown2.Value;
        constTime = (byte)numericUpDown3.Value;
        writer = new System.IO.StreamWriter("output_data.txt");
        writer.WriteLine("Start test\t{0}\n",DateTime.Now.ToString());
        groupBox1.Enabled = groupBox2.Enabled = groupBox3.Enabled = numericUpDown2.Enabled = numericUpDown3.Enabled = false;
      }
      else {
        if (writer != null)
        {
          writer.WriteLine("Total tests\t{0}", test_count.ToString());
          writer.WriteLine("Total erros\t{0}", error_count.ToString());
          writer.WriteLine("End test\t{0}", DateTime.Now.ToString());
          writer.Close();
        }
        groupBox1.Enabled = groupBox2.Enabled = groupBox3.Enabled = numericUpDown2.Enabled = numericUpDown3.Enabled = true;
      }
    }
  }
}
