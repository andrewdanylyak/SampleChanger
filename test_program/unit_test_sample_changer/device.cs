using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace unit_test_sample_changer
{
  public class OnGetDeviceIdArgs : EventArgs
  {
    public string Id { get; set; }
  }

  public class OnDeviceGetStateArgs : EventArgs
  { 
    public DeviceState sts { get; set; }
  }

  public enum CellState
  {
    CellOk,
    CellHandMoove,
    CellRotate,
    CellPark,
    CellError
  }

  public enum LiftState
  {
    Lift_OK,
    Lift_DOWN,
    Lift_UP,
    Lift_ERROR
  }

  public enum DiskState
  {
    DiskOk,
    DiskReady,
    DiskAbsend,
    DiskMooving,
    DiskError
  }

  public struct DeviceState
  {
    public CellState cell_state;
    public LiftState lift_state;
    public DiskState disk_state;
  }

  public enum DevCommands
  {
    SetSpinnerOff,
    SetSpinnerOn,
    SetLiftUp,
    SetLiftDown,
    SetSampleChanger,
  }

  public class DevCommand
  {
    public object data { get; set; }
    public DevCommands command { get; set; }
  }
  public class device:IDisposable
  {
    private bool disposedValue;
    CProtocol protocol = null;
    Thread thread = null;
    AutoResetEvent exit_event = new AutoResetEvent(false);
    Queue<DevCommand> devQueue = new Queue<DevCommand>();
    public device()
    {
      protocol = new CProtocol();
      thread = new Thread(thread_device);
      thread.Start();
    }
    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          
          exit_event.Set();
          thread.Join();
          protocol.CloseConnection();
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
      }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~device()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    public event EventHandler<OnGetDeviceIdArgs> GetDeviceId;
    protected void OnGetId(string id)
    {
      OnGetDeviceIdArgs args = new OnGetDeviceIdArgs();
      EventHandler<OnGetDeviceIdArgs> handler = GetDeviceId;
      if (handler != null)
      {
        args.Id = id;
        handler(this, args);
      }
    }

    public event EventHandler<OnDeviceGetStateArgs> GetDeviceState;

    protected void OnGetDeviceState(DeviceState sts)
    {
      OnDeviceGetStateArgs args = new OnDeviceGetStateArgs();
      EventHandler<OnDeviceGetStateArgs> handler = GetDeviceState;
      if (handler != null)
      {
        args.sts = sts;
        handler(this, args);
      }
    }

    public void InsertCommand(DevCommand cmd)
    {
      devQueue.Enqueue(cmd);
    }

    public void InsertCommand(DevCommands cmd)
    {
      DevCommand mCmd = new DevCommand();
      mCmd.command = cmd;
      devQueue.Enqueue(mCmd);
    }

    public void InsertCommand(DevCommands cmd, object data)
    {
      DevCommand mCmd = new DevCommand();
      mCmd.command = cmd;
      mCmd.data = data;
      devQueue.Enqueue(mCmd);
    }

    void thread_device() {
      bool connected = false;
      byte data;
      DevCommand cmd;
      DeviceState sts = new DeviceState();
      int time1 = Environment.TickCount;
      while (true)
      {
        if (!connected)
        {
          if (!protocol.IsConnected())
          {
            connected = protocol.SearchDeviceElvax();
            if (connected)
              OnGetId(protocol.DeviceId);
          }
        }
        else {
          if ((Environment.TickCount - time1) > 100)
          {
            time1 = Environment.TickCount;
            if (protocol.GetDiskState(ref sts))
              OnGetDeviceState(sts);
          }
          if (devQueue.Count != 0)
          {
            cmd = devQueue.Dequeue();
            switch (cmd.command) 
            {
              case DevCommands.SetSpinnerOff:
                protocol.SetSpinner(false, 0);
                break;
              case DevCommands.SetSpinnerOn:
                protocol.SetSpinner(true, ((byte[])cmd.data)[0]);
                break;
              case DevCommands.SetLiftDown:
                protocol.SetLift(true);
                break;
              case DevCommands.SetLiftUp:
                protocol.SetLift(false);
                break;
              case DevCommands.SetSampleChanger:
                data = (byte)cmd.data;
                protocol.SetDiskPosition(data);
                break;
            }
          }
        }
        if (exit_event.WaitOne(1))
          break;
      }
    }
  }
}
