using System;
using System.Runtime.InteropServices;

namespace sistema_acceso;

public static class Scanner
{
    [StructLayout(LayoutKind.Sequential)]
    private struct FTRSCAN_IMAGE_SIZE { public int nWidth, nHeight, nImageSize; }

    [DllImport("ftrScanAPI.dll")] private static extern IntPtr ftrScanOpenDevice();
    [DllImport("ftrScanAPI.dll")] private static extern bool ftrScanGetImageSize(IntPtr h, out FTRSCAN_IMAGE_SIZE s);
    [DllImport("ftrScanAPI.dll")] private static extern bool ftrScanGetFrame(IntPtr h, IntPtr p, IntPtr f);
    [DllImport("ftrScanAPI.dll")] private static extern void ftrScanCloseDevice(IntPtr h);

    public static byte[] GetRawImage()
    {
        IntPtr h = ftrScanOpenDevice();
        if (h == IntPtr.Zero) return null;

        try
        {
            if (ftrScanGetImageSize(h, out var size))
            {
                IntPtr p = Marshal.AllocHGlobal(size.nImageSize);
                try
                {
                    if (ftrScanGetFrame(h, p, IntPtr.Zero))
                    {
                        byte[] data = new byte[size.nImageSize];
                        Marshal.Copy(p, data, 0, size.nImageSize);
                        return data;
                    }
                }
                finally { Marshal.FreeHGlobal(p); }
            }
        }
        finally { ftrScanCloseDevice(h); }
        return null;
    }
}
