using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace WindowsFormsApplication1
{
    /// <summary>
    /// MINIDUMP_EXCEPTION_INFORMATION — структура, которая будет хранить информацию об исключении, 
    /// благодаря которому программа завершила свою работу.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MINIDUMP_EXCEPTION_INFORMATION
    {
        public uint ThreadId;
        public IntPtr ExceptionPointers;
        public int ClientPointers;
    }
    
    /// <summary>
    /// MINIDUMP_TYPE содержит все типы минидампов, которые мы можем создавать. 
    /// Каждый тип ассоциирован с определенной константой. Полный список констант можно узнать на
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms680519(v=vs.85).aspx
    /// </summary>
    public enum MINIDUMP_TYPE: int
    {
        /// <summary>
        /// Include just the information necessary to 
        /// capture stack traces for all existing threads in a process.
        /// </summary>
        MiniDumpNormal = 0x00000000,
        /// <summary>
        /// Include the data sections from all loaded modules. 
        /// This results in the inclusion of global variables, 
        /// which can make the minidump file significantly larger. 
        /// For per-module control, use the ModuleWriteDataSeg enumeration 
        /// value from MODULE_WRITE_FLAGS.
        /// </summary>
        MiniDumpWithDataSegs = 0x00000001,
        /// <summary>
        /// Include all accessible memory in the process. 
        /// The raw memory data is included at the end, so that the initial structures 
        /// can be mapped directly without the raw memory information. 
        /// This option can result in a very large file.
        /// </summary>
        MiniDumpWithFullMemory = 0x00000002,
        /// <summary>
        /// Include high-level information about the operating system 
        /// handles that are active when the minidump is made.
        /// </summary>
        MiniDumpWithHandleData = 0x00000004,
        /// <summary>
        /// Stack and backing store memory written to the minidump 
        /// file should be filtered to remove all but the pointer 
        /// values necessary to reconstruct a stack trace.
        /// </summary>
        MiniDumpFilterMemory = 0x00000008,
        /// <summary>
        /// Stack and backing store memory should be scanned for pointer 
        /// references to modules in the module list. If a module is 
        /// referenced by stack or backing store memory, the ModuleWriteFlags 
        /// member of the MINIDUMP_CALLBACK_OUTPUT structure is set to ModuleReferencedByMemory.
        /// </summary>
        MiniDumpScanMemory = 0x00000010,
        /// <summary>
        /// Include information from the list of modules that were recently unloaded, 
        /// if this information is maintained by the operating system. 
        /// Windows Server 2003 and Windows XP:  The operating system does 
        /// not maintain information for unloaded modules until Windows Server 2003 
        /// with SP1 and Windows XP with SP2.
        /// DbgHelp 5.1:  This value is not supported.
        /// </summary>
        MiniDumpWithUnloadedModules = 0x00000020,
        /// <summary>
        /// Include pages with data referenced by locals or other stack memory. 
        /// This option can increase the size of the minidump file significantly. 
        /// DbgHelp 5.1:  This value is not supported.
        /// </summary>
        MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
        /// <summary>
        /// Filter module paths for information such as user names or important directories. 
        /// This option may prevent the system from locating the image file and should be used only in special situations. 
        /// DbgHelp 5.1:  This value is not supported.
        /// </summary>
        MiniDumpFilterModulePaths = 0x00000080,
        /// <summary>
        /// Include complete per-process and per-thread information from the operating system. 
        /// DbgHelp 5.1:  This value is not supported.
        /// </summary>
        MiniDumpWithProcessThreadData = 0x00000100,
        /// <summary>
        /// Scan the virtual address space for PAGE_READWRITE memory to be included. 
        /// DbgHelp 5.1:  This value is not supported.
        /// </summary>
        MiniDumpWithPrivateReadWriteMemory = 0x00000200,
        /// <summary>
        /// Reduce the data that is dumped by eliminating memory regions that are 
        /// not essential to meet criteria specified for the dump. 
        /// This can avoid dumping memory that may contain data that is 
        /// private to the user. However, it is not a guarantee that no private information will be present. 
        /// DbgHelp 6.1 and earlier:  This value is not supported.
        /// </summary>
        MiniDumpWithoutOptionalData = 0x00000400,
        /// <summary>
        /// Include memory region information. For more information, see MINIDUMP_MEMORY_INFO_LIST. 
        /// DbgHelp 6.1 and earlier:  This value is not supported.
        /// </summary>
        MiniDumpWithFullMemoryInfo = 0x00000800,
        /// <summary>
        /// Include thread state information. For more information, see MINIDUMP_THREAD_INFO_LIST. 
        /// DbgHelp 6.1 and earlier:  This value is not supported.
        /// </summary>
        MiniDumpWithThreadInfo = 0x00001000,
        /// <summary>
        /// Include all code and code-related sections from loaded modules to capture 
        /// executable content. For per-module control, use the ModuleWriteCodeSegs 
        /// enumeration value from MODULE_WRITE_FLAGS. 
        /// DbgHelp 6.1 and earlier:  This value is not supported.
        /// </summary>
        MiniDumpWithCodeSegs = 0x00002000,
        /// <summary>
        /// Turns off secondary auxiliary-supported memory gathering.
        /// </summary>
        MiniDumpWithoutAuxiliaryState = 0x00004000,
        /// <summary>
        /// Requests that auxiliary data providers include their state in the dump image; 
        /// the state data that is included is provider dependent. This option can result in a large dump image.
        /// </summary>
        MiniDumpWithFullAuxiliaryState = 0x00008000,
        /// <summary>
        /// Scans the virtual address space for PAGE_WRITECOPY memory to be included. 
        /// Prior to DbgHelp 6.1:  This value is not supported.
        /// </summary>
        MiniDumpWithPrivateWriteCopyMemory = 0x00010000,
        /// <summary>
        /// If you specify MiniDumpWithFullMemory, the MiniDumpWriteDump function will 
        /// fail if the function cannot read the memory regions; however, 
        /// if you include MiniDumpIgnoreInaccessibleMemory, the MiniDumpWriteDump function will 
        /// ignore the memory read failures and continue to generate the dump. 
        /// Note that the inaccessible memory regions are not included in the dump.
        /// Prior to DbgHelp 6.1:  This value is not supported.
        /// </summary>
        MiniDumpIgnoreInaccessibleMemory = 0x00020000,
        /// <summary>
        /// Adds security token related data. This will make the "!token" extension work when processing a user-mode dump. 
        /// Prior to DbgHelp 6.1:  This value is not supported.
        /// </summary>
        MiniDumpWithTokenInformation = 0x00040000,
        /// <summary>
        /// Adds module header related data. 
        /// Prior to DbgHelp 6.1:  This value is not supported.
        /// </summary>
        MiniDumpWithModuleHeaders = 0x00080000,
        /// <summary>
        /// Adds filter triage related data. 
        /// Prior to DbgHelp 6.1:  This value is not supported.
        /// </summary>
        MiniDumpFilterTriage = 0x00100000,
        /// <summary>
        /// Indicates which flags are valid.
        /// </summary>
        MiniDumpValidTypeFlags = 0x001fffff,
    }
    /// <summary>
    /// Пример взят из http://habrahabr.ru/post/153181/
    /// </summary>
    public class DumpMaker
    {
        /// <summary>
        /// Возврещает ID текущего процесса.
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        /// <summary>
        /// Библиотечный метод, непосредственно выполняющий создание дампа и его запись. Он вызывается из метода
        /// </summary>
        /// <param name="hProcess">дескриптор процесса, для которого генерируется информация</param>
        /// <param name="ProcessId">ID процесса, для которого генерируется информация</param>
        /// <param name="hFile">дескриптор файла</param>
        /// <param name="DumpType">тип дампа (будем использовать MiniDumpNormal)</param>
        /// <param name="ExceptionParam">информация об исключении</param>
        /// <param name="UserStreamParam">информация, определяемая пользователем. 
        /// Мы не будем включать ее в дамп и передадим в метод IntPtr.Zero</param>
        /// <param name="CallbackParam">информация об обратном вызове. Так же не будем ее использовать.</param>
        /// <returns></returns>
        [DllImport("Dbghelp.dll")]
        static extern bool MiniDumpWriteDump(
            IntPtr hProcess, 
            uint ProcessId, 
            IntPtr hFile, 
            int DumpType, 
            ref MINIDUMP_EXCEPTION_INFORMATION ExceptionParam, 
            IntPtr UserStreamParam, 
            IntPtr CallbackParam);



        
        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Unhandled exception!");

            CreateMiniDump();
        }

        /// <summary>
        /// Дамп записывается в файл с уникальным названием (FileName), который будет храниться в той же директории, что и exe файл. 
        /// Перед тем как вызвать MiniDumpWriteDump, инициализируем структуру типа MINIDUMP_EXCEPTION_INFORMATION.
        /// </summary>
        private static void CreateMiniDump()
        {
            using (System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess())
            {
                string FileName = string.Format(@"CRASH_DUMP_{0}_{1}.dmp", DateTime.Today.ToShortDateString(), DateTime.Now.Ticks);

                MINIDUMP_EXCEPTION_INFORMATION Mdinfo = new MINIDUMP_EXCEPTION_INFORMATION();

                Mdinfo.ThreadId = GetCurrentThreadId();
                Mdinfo.ExceptionPointers = Marshal.GetExceptionPointers();
                Mdinfo.ClientPointers = 1;

                using (FileStream fs = new FileStream(FileName, FileMode.Create))
                {

                    {
                        MiniDumpWriteDump(
                            process.Handle, 
                            (uint)process.Id, 
                            fs.SafeFileHandle.DangerousGetHandle(), 
                            (Int32)MINIDUMP_TYPE.MiniDumpNormal,
                            ref Mdinfo,
                            IntPtr.Zero,
                            IntPtr.Zero);
                    }
                }
            }
        }
    }
}
