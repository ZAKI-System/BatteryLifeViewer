using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BatteryLifeViewer
{
    internal class BatteryInfo
    {
        public static BatteryInformation GetBatteryInformation2(int index)
        {
            IntPtr deviceDataPointer = IntPtr.Zero;
            IntPtr queryInfoPointer = IntPtr.Zero;
            IntPtr batteryInfoPointer = IntPtr.Zero;
            IntPtr batteryWaitStatusPointer = IntPtr.Zero;
            IntPtr batteryStatusPointer = IntPtr.Zero;
            try
            {
                IntPtr deviceHandle = SetupDiGetClassDevs(
                    Win32.GUID_DEVCLASS_BATTERY, Win32.DEVICE_GET_CLASS_FLAGS.DIGCF_PRESENT | Win32.DEVICE_GET_CLASS_FLAGS.DIGCF_DEVICEINTERFACE);

                var deviceInterfaceData = new Win32.SP_DEVICE_INTERFACE_DATA();
                deviceInterfaceData.CbSize = Marshal.SizeOf(deviceInterfaceData);

                // index
                SetupDiEnumDeviceInterfaces(deviceHandle, Win32.GUID_DEVCLASS_BATTERY, index, ref deviceInterfaceData);

                deviceDataPointer = Marshal.AllocHGlobal(Win32.DEVICE_INTERFACE_BUFFER_SIZE);

                var deviceDetailData = new Win32.SP_DEVICE_INTERFACE_DETAIL_DATA();
                deviceDetailData.CbSize = (IntPtr.Size == 8) ? 8 : 4 + Marshal.SystemDefaultCharSize;

                SetupDiGetDeviceInterfaceDetail(deviceHandle, ref deviceInterfaceData, ref deviceDetailData, Win32.DEVICE_INTERFACE_BUFFER_SIZE);

                IntPtr batteryHandle = CreateFile(deviceDetailData.DevicePath, FileAccess.ReadWrite, FileShare.ReadWrite, FileMode.Open, Win32.FILE_ATTRIBUTES.Normal);

                var queryInformation = new Win32.BATTERY_QUERY_INFORMATION();

                DeviceIoControl(batteryHandle, Win32.IOCTL_BATTERY_QUERY_TAG, ref queryInformation.BatteryTag);

                var batteryInformation = new Win32.BATTERY_INFORMATION();
                queryInformation.InformationLevel = Win32.BATTERY_QUERY_INFORMATION_LEVEL.BatteryInformation;

                int queryInfoSize = Marshal.SizeOf(queryInformation);
                int batteryInfoSize = Marshal.SizeOf(batteryInformation);

                queryInfoPointer = Marshal.AllocHGlobal(queryInfoSize);
                Marshal.StructureToPtr(queryInformation, queryInfoPointer, false);

                batteryInfoPointer = Marshal.AllocHGlobal(batteryInfoSize);
                Marshal.StructureToPtr(batteryInformation, batteryInfoPointer, false);

                DeviceIoControl(batteryHandle, Win32.IOCTL_BATTERY_QUERY_INFORMATION, queryInfoPointer, queryInfoSize, batteryInfoPointer, batteryInfoSize);

                Win32.BATTERY_INFORMATION updatedBatteryInformation =
                    (Win32.BATTERY_INFORMATION)Marshal.PtrToStructure(batteryInfoPointer, typeof(Win32.BATTERY_INFORMATION));

                var batteryWaitStatus = new Win32.BATTERY_WAIT_STATUS();
                batteryWaitStatus.BatteryTag = queryInformation.BatteryTag;

                var batteryStatus = new Win32.BATTERY_STATUS();

                int waitStatusSize = Marshal.SizeOf(batteryWaitStatus);
                int batteryStatusSize = Marshal.SizeOf(batteryStatus);

                batteryWaitStatusPointer = Marshal.AllocHGlobal(waitStatusSize);
                Marshal.StructureToPtr(batteryWaitStatus, batteryWaitStatusPointer, false);

                batteryStatusPointer = Marshal.AllocHGlobal(batteryStatusSize);
                Marshal.StructureToPtr(batteryStatus, batteryStatusPointer, false);

                DeviceIoControl(batteryHandle, Win32.IOCTL_BATTERY_QUERY_STATUS, batteryWaitStatusPointer, waitStatusSize, batteryStatusPointer, batteryStatusSize);

                Win32.BATTERY_STATUS updatedStatus = (Win32.BATTERY_STATUS)Marshal.PtrToStructure(batteryStatusPointer, typeof(Win32.BATTERY_STATUS));

                Win32.SetupDiDestroyDeviceInfoList(deviceHandle);

                return new BatteryInformation()
                {
                    DesignedMaxCapacity = updatedBatteryInformation.DesignedCapacity,
                    FullChargeCapacity = updatedBatteryInformation.FullChargedCapacity,
                    CurrentCapacity = updatedStatus.Capacity,
                    Voltage = updatedStatus.Voltage,
                    DischargeRate = updatedStatus.Rate
                };
                //return;
            }
            finally
            {
                Marshal.FreeHGlobal(deviceDataPointer);
                Marshal.FreeHGlobal(queryInfoPointer);
                Marshal.FreeHGlobal(batteryInfoPointer);
                Marshal.FreeHGlobal(batteryStatusPointer);
                Marshal.FreeHGlobal(batteryWaitStatusPointer);
            }
        }
        public static BatteryInformation GetBatteryInformation()
        {
            IntPtr deviceDataPointer = IntPtr.Zero;
            IntPtr queryInfoPointer = IntPtr.Zero;
            IntPtr batteryInfoPointer = IntPtr.Zero;
            IntPtr batteryWaitStatusPointer = IntPtr.Zero;
            IntPtr batteryStatusPointer = IntPtr.Zero;
            try
            {
                IntPtr deviceHandle = SetupDiGetClassDevs(
                Win32.GUID_DEVCLASS_BATTERY, Win32.DEVICE_GET_CLASS_FLAGS.DIGCF_PRESENT | Win32.DEVICE_GET_CLASS_FLAGS.DIGCF_DEVICEINTERFACE);

                Win32.SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new Win32.SP_DEVICE_INTERFACE_DATA();
                deviceInterfaceData.CbSize = Marshal.SizeOf(deviceInterfaceData);

                SetupDiEnumDeviceInterfaces(deviceHandle, Win32.GUID_DEVCLASS_BATTERY, 0, ref deviceInterfaceData);

                deviceDataPointer = Marshal.AllocHGlobal(Win32.DEVICE_INTERFACE_BUFFER_SIZE);
                //Win32.SP_DEVICE_INTERFACE_DETAIL_DATA deviceDetailData =
                //    (Win32.SP_DEVICE_INTERFACE_DETAIL_DATA)Marshal.PtrToStructure(deviceDataPointer, typeof(Win32.SP_DEVICE_INTERFACE_DETAIL_DATA));

                //toggle these two and see if naything changes... ^^^^^^^^^^^^
                Win32.SP_DEVICE_INTERFACE_DETAIL_DATA deviceDetailData = new Win32.SP_DEVICE_INTERFACE_DETAIL_DATA();
                deviceDetailData.CbSize = (IntPtr.Size == 8) ? 8 : 4 + Marshal.SystemDefaultCharSize;

                SetupDiGetDeviceInterfaceDetail(deviceHandle, ref deviceInterfaceData, ref deviceDetailData, Win32.DEVICE_INTERFACE_BUFFER_SIZE);

                IntPtr batteryHandle = CreateFile(deviceDetailData.DevicePath, FileAccess.ReadWrite, FileShare.ReadWrite, FileMode.Open, Win32.FILE_ATTRIBUTES.Normal);

                Win32.BATTERY_QUERY_INFORMATION queryInformation = new Win32.BATTERY_QUERY_INFORMATION();

                DeviceIoControl(batteryHandle, Win32.IOCTL_BATTERY_QUERY_TAG, ref queryInformation.BatteryTag);

                Win32.BATTERY_INFORMATION batteryInformation = new Win32.BATTERY_INFORMATION();
                queryInformation.InformationLevel = Win32.BATTERY_QUERY_INFORMATION_LEVEL.BatteryInformation;

                int queryInfoSize = Marshal.SizeOf(queryInformation);
                int batteryInfoSize = Marshal.SizeOf(batteryInformation);

                queryInfoPointer = Marshal.AllocHGlobal(queryInfoSize);
                Marshal.StructureToPtr(queryInformation, queryInfoPointer, false);

                batteryInfoPointer = Marshal.AllocHGlobal(batteryInfoSize);
                Marshal.StructureToPtr(batteryInformation, batteryInfoPointer, false);

                DeviceIoControl(batteryHandle, Win32.IOCTL_BATTERY_QUERY_INFORMATION, queryInfoPointer, queryInfoSize, batteryInfoPointer, batteryInfoSize);

                Win32.BATTERY_INFORMATION updatedBatteryInformation =
                    (Win32.BATTERY_INFORMATION)Marshal.PtrToStructure(batteryInfoPointer, typeof(Win32.BATTERY_INFORMATION));

                Win32.BATTERY_WAIT_STATUS batteryWaitStatus = new Win32.BATTERY_WAIT_STATUS();
                batteryWaitStatus.BatteryTag = queryInformation.BatteryTag;

                Win32.BATTERY_STATUS batteryStatus = new Win32.BATTERY_STATUS();

                int waitStatusSize = Marshal.SizeOf(batteryWaitStatus);
                int batteryStatusSize = Marshal.SizeOf(batteryStatus);

                batteryWaitStatusPointer = Marshal.AllocHGlobal(waitStatusSize);
                Marshal.StructureToPtr(batteryWaitStatus, batteryWaitStatusPointer, false);

                batteryStatusPointer = Marshal.AllocHGlobal(batteryStatusSize);
                Marshal.StructureToPtr(batteryStatus, batteryStatusPointer, false);

                DeviceIoControl(batteryHandle, Win32.IOCTL_BATTERY_QUERY_STATUS, batteryWaitStatusPointer, waitStatusSize, batteryStatusPointer, batteryStatusSize);

                Win32.BATTERY_STATUS updatedStatus =
                    (Win32.BATTERY_STATUS)Marshal.PtrToStructure(batteryStatusPointer, typeof(Win32.BATTERY_STATUS));

                Win32.SetupDiDestroyDeviceInfoList(deviceHandle);

                return new BatteryInformation()
                {
                    DesignedMaxCapacity = updatedBatteryInformation.DesignedCapacity,
                    FullChargeCapacity = updatedBatteryInformation.FullChargedCapacity,
                    CurrentCapacity = updatedStatus.Capacity,
                    Voltage = updatedStatus.Voltage,
                    DischargeRate = updatedStatus.Rate
                };

            }
            finally
            {
                Marshal.FreeHGlobal(deviceDataPointer);
                Marshal.FreeHGlobal(queryInfoPointer);
                Marshal.FreeHGlobal(batteryInfoPointer);
                Marshal.FreeHGlobal(batteryStatusPointer);
                Marshal.FreeHGlobal(batteryWaitStatusPointer);
            }
        }

        private static bool DeviceIoControl(IntPtr deviceHandle, uint controlCode, ref uint output)
        {
            uint bytesReturned;
            uint junkInput = 0;
            bool retval = Win32.DeviceIoControl(
                deviceHandle, controlCode, ref junkInput, 0, ref output, (uint)Marshal.SizeOf(output), out bytesReturned, IntPtr.Zero);

            if (!retval)
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode != 0)
                    throw Marshal.GetExceptionForHR(errorCode);
                else
                    throw new Exception("DeviceIoControl call failed but Win32 didn't catch an error.");
            }

            return retval;
        }

        private static bool DeviceIoControl(
            IntPtr deviceHandle, uint controlCode, IntPtr input, int inputSize, IntPtr output, int outputSize)
        {
            uint bytesReturned;
            bool retval = Win32.DeviceIoControl(
                deviceHandle, controlCode, input, (uint)inputSize, output, (uint)outputSize, out bytesReturned, IntPtr.Zero);

            if (!retval)
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode != 0)
                    throw Marshal.GetExceptionForHR(errorCode);
                else
                    throw new Exception("DeviceIoControl call failed but Win32 didn't catch an error.");
            }

            return retval;
        }

        private static IntPtr SetupDiGetClassDevs(Guid guid, Win32.DEVICE_GET_CLASS_FLAGS flags)
        {
            IntPtr handle = Win32.SetupDiGetClassDevs(ref guid, null, IntPtr.Zero, flags);

            if (handle == IntPtr.Zero || handle.ToInt32() == -1)
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode != 0)
                    throw Marshal.GetExceptionForHR(errorCode);
                else
                    throw new Exception("SetupDiGetClassDev call returned a bad handle.");
            }
            return handle;
        }

        private static bool SetupDiEnumDeviceInterfaces(
            IntPtr deviceInfoSet, Guid guid, int memberIndex, ref Win32.SP_DEVICE_INTERFACE_DATA deviceInterfaceData)
        {
            bool retval = Win32.SetupDiEnumDeviceInterfaces(
                deviceInfoSet, IntPtr.Zero, ref guid, (uint)memberIndex, ref deviceInterfaceData);

            if (!retval)
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode != 0)
                {
                    if (errorCode == 259)
                        throw new Exception("SetupDeviceInfoEnumerateDeviceInterfaces ran out of batteries to enumerate.");

                    throw Marshal.GetExceptionForHR(errorCode);
                }
                else
                    throw new Exception("SetupDeviceInfoEnumerateDeviceInterfaces call failed but Win32 didn't catch an error.");
            }
            return retval;
        }

        private static bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet)
        {
            bool retval = Win32.SetupDiDestroyDeviceInfoList(deviceInfoSet);

            if (!retval)
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode != 0)
                    throw Marshal.GetExceptionForHR(errorCode);
                else
                    throw new Exception("SetupDiDestroyDeviceInfoList call failed but Win32 didn't catch an error.");
            }
            return retval;
        }

        private static bool SetupDiGetDeviceInterfaceDetail(
            IntPtr deviceInfoSet, ref Win32.SP_DEVICE_INTERFACE_DATA deviceInterfaceData, ref Win32.SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData, int deviceInterfaceDetailSize)
        {
            //int tmpSize = Marshal.SizeOf(deviceInterfaceDetailData);
            uint reqSize;
            bool retval = Win32.SetupDiGetDeviceInterfaceDetail(
                deviceInfoSet, ref deviceInterfaceData, ref deviceInterfaceDetailData, (uint)deviceInterfaceDetailSize, out reqSize, IntPtr.Zero);

            if (!retval)
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode != 0)
                    throw Marshal.GetExceptionForHR(errorCode);
                else
                    throw new Exception("SetupDiGetDeviceInterfaceDetail call failed but Win32 didn't catch an error.");
            }
            return retval;
        }

        private static IntPtr CreateFile(
            string filename, FileAccess access, FileShare shareMode, FileMode creation, Win32.FILE_ATTRIBUTES flags)
        {
            IntPtr handle = Win32.CreateFile(
                filename, access, shareMode, IntPtr.Zero, creation, flags, IntPtr.Zero);

            if (handle == IntPtr.Zero || handle.ToInt32() == -1)
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode != 0)
                    Marshal.ThrowExceptionForHR(errorCode);
                else
                    throw new Exception("SetupDiGetDeviceInterfaceDetail call failed but Win32 didn't catch an error.");
            }
            return handle;
        }
    }
}
