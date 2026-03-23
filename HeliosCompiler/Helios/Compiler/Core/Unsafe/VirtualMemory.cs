using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Helios.Compiler.Core.Unsafe
{
    public enum MemoryOperation
    {
        Reserve, Commit, Decommit, Free
    }

    public unsafe struct VirtualSegment(void* memory, nuint size)
    {
        private nuint _committed = 0;

        public void* Get()
        {
            return memory;
        }

        public nuint GetCommitted()
        {
            return _committed;
        }

        public nuint GetSize()
        {
            return size;
        }

        public bool Commit(nuint offset)
        {
            if (_committed + offset > size) return false;
            _committed += offset;
            return true;
        }

        public bool Decommit(nuint offset)
        {
            if ((Int64)_committed - (Int64)offset < 0) return false;
            _committed -= offset;
            return true;
        }

        public static bool operator !=(VirtualSegment a, VirtualSegment b)
        {
            return !(a == b);
        }

        public static bool operator ==(VirtualSegment a, VirtualSegment b)
        {
            return a._committed == b._committed && a.GetSize() == b.GetSize() && a.Get() == b.Get();
        }
    }

    internal static unsafe class VirtualMemory
    {
        // Linux
        private const int ProtNone = 0x0;

        private const int ProtRead = 0x1;
        private const int ProtWrite = 0x2;
        private const int MapPrivate = 0x02;
        private const int MapAnonymous = 0x20;

        //Windows
        private const uint MemReserve = 0x00002000;

        private const uint MemCommit = 0x00001000;
        private const uint MemDecommit = 0x00004000;
        private const uint MemRelease = 0x00008000;

        private const uint PageNoAccess = 0x01;
        private const uint PageReadWrite = 0x04;

        public static readonly VirtualSegment InvalidSegment = new(null, 0);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern void* VirtualAlloc(
            void* lpAddress,
            nuint dwSize,
            uint fAllocationType,
            uint fProtect
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualFree(
            void* lpAddress,
            nuint dwSize,
            uint dwFreeType);

        [DllImport("libc", SetLastError = true)]
        private static extern void* mmap(
            void* addr,
            nuint length,
            int prot,
            int flags,
            int fd,
            nint offset);

        [DllImport("libc", SetLastError = true)]
        private static extern int munmap(
            void* addr,
            nuint length);

        [DllImport("libc", SetLastError = true)]
        private static extern int mprotect(
            void* addr,
            nuint length,
            int prot);

        public static VirtualSegment Allocate(VirtualSegment segment, MemoryOperation operation, nuint size)
        {
            void* memory = null;
            switch (operation)
            {
                case MemoryOperation.Reserve:
                    if (segment != InvalidSegment) throw new InvalidDataException("Reserve needs an invalid segment");
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        memory = VirtualAlloc(null, size, MemReserve, PageNoAccess);
                        if (memory == null) return InvalidSegment;
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        memory = mmap(null, size, ProtNone, MapAnonymous | MapPrivate, -1, 0);
                        if (memory == (void*)-1) return InvalidSegment;
                        segment.Commit(size);
                    }
                    else
                    {
                        throw new UnreachableException("Invalid OS");
                    }

                    return new VirtualSegment(memory, size);

                case MemoryOperation.Commit:
                    if (segment == InvalidSegment) throw new InvalidDataException("Commit needs a valid segment");
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        memory = VirtualAlloc(segment.Get(), size, MemCommit, PageReadWrite);
                        if (memory == null) return InvalidSegment;
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        var res = mprotect(segment.Get(), size, ProtRead | ProtWrite);
                        if (res != 0) return InvalidSegment;
                    }
                    else
                    {
                        throw new UnreachableException("Invalid OS");
                    }
                    return segment;

                case MemoryOperation.Decommit:
                case MemoryOperation.Free:
                default:
                    throw new UnreachableException("Decommit and Free are not valid for Allocate — use Deallocate");
            }
        }

        public static bool Deallocate(VirtualSegment segment, MemoryOperation operation, nuint offset)
        {
            switch (operation)
            {
                case MemoryOperation.Decommit:
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        if (!VirtualFree((byte*)(segment.Get()) + segment.GetCommitted() - offset, offset, MemDecommit))
                            return false;
                        segment.Decommit(offset);
                        return true;
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        var val = mprotect((byte*)(segment.Get()) + segment.GetCommitted(), offset, ProtNone);
                        if (val != 0) return false;
                    }
                    else
                    {
                        throw new UnreachableException("Invalid OS");
                    }

                    return false;
                case MemoryOperation.Free:
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        if (VirtualFree(segment.Get(), 0, MemRelease)) return true;
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        var val = munmap(segment.Get(), segment.GetSize());
                        if (val != 0) return false;
                    }
                    else
                    {
                        throw new UnreachableException("Invalid OS");
                    }

                    return false;
                case MemoryOperation.Reserve:
                case MemoryOperation.Commit:
                default:
                    throw new UnreachableException("Commit and Reserve are not valid for Deallocate — use Allocate");
            }
        }
    }
}