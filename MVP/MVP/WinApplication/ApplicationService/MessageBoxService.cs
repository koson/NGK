using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Mvp.WinApplication.ApplicationService
{
    public static class MessageBoxService
    {
        public enum Result
        {
            Yes,
            No
        }

        public static void ShowInformation(string text, string caption)
        {
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ShowError(string text, string caption)
        {
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowWarning(string text, string caption)
        {
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static Result ShowQuestion(string text, string caption)
        {
            DialogResult result = 
                MessageBox.Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            switch (result)
            {
                case DialogResult.Yes: return Result.Yes;
                case DialogResult.No: return Result.No;
                default: throw new Exception();
            }
        }
    }
}
