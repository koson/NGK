using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace NGK.CorrosionMonitoringSystem.Forms.Controls
{
    /// <summary>
    /// Панель для системных кнопок
    /// </summary>
    public class ButtonsPanel
    {
        public struct ButtonNames
        {
            public const string ButtonOne = "ButtonOne";
            public const string ButtonTwo = "ButtonTwo";
            public const string ButtonThree = "ButtonThree";
            public const string ButtonFour = "ButtonFour";
            public const string ButtonFive = "ButtonFive";
            
            public static string[] ToArray()
            {
                return new string[] { ButtonOne, ButtonTwo, 
                    ButtonThree, ButtonFour, ButtonFive };
            }
        }

        #region Fields And Properties
        /// <summary>
        /// Панель на которой необходимо разместить кнопки.
        /// </summary>
        private SplitterPanel _Panel;
        /// <summary>
        /// Кнопка "1"
        /// </summary>
        private Button _ButtonOne;
        /// <summary>
        /// Возвращает кнопку "1" на панели
        /// </summary>
        public Button ButtonOne
        {
            get { return _ButtonOne; }
        }
        /// <summary>
        /// Кнопка "2"
        /// </summary>
        private Button _ButtonTwo;
        /// <summary>
        /// Возвращает кнопку "2" на панели
        /// </summary>
        public Button ButtonTwo
        {
            get { return _ButtonTwo; }
        }
        /// <summary>
        /// Кнопка "3"
        /// </summary>
        private Button _ButtonThree;
        /// <summary>
        /// Возвращает кнопку "3" на панели
        /// </summary>
        public Button ButtonThree
        {
            get { return _ButtonThree; }
        }
        /// <summary>
        /// Кнопка "4"
        /// </summary>
        private Button _ButtonFour;
        /// <summary>
        /// Возвращает кнопку "4" на панели
        /// </summary>
        public Button ButtonFour
        {
            get { return _ButtonFour; }
        }
        /// <summary>
        /// Кнопка "5"
        /// </summary>
        private Button _ButtonFive;
        /// <summary>
        /// Возвращает кнопку "5" на панели
        /// </summary>
        public Button ButtonFive
        {
            get { return _ButtonFive; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        public ButtonsPanel(SplitterPanel panel)
        {
            this._Panel = panel;
            this._Panel.Resize += new EventHandler(EventHandler_Panel_Resize);
            this._Panel.Controls.Clear();
            
            this._ButtonOne = new Button();
            this._ButtonOne.Name = ButtonNames.ButtonOne;
            this._ButtonOne.Text = "Button1";
            this._ButtonOne.Size = 
                new System.Drawing.Size(this._Panel.Width, this._Panel.Height);

            this._ButtonTwo = new Button();
            this._ButtonTwo.Name = ButtonNames.ButtonTwo;
            this._ButtonTwo.Text = "Button2";
            this._ButtonTwo.Size =
                new System.Drawing.Size(this._Panel.Width, this._Panel.Height);

            this._ButtonThree = new Button();
            this._ButtonThree.Name = ButtonNames.ButtonThree;
            this._ButtonThree.Text = "Button3";
            this._ButtonThree.Size =
                new System.Drawing.Size(this._Panel.Width, this._Panel.Height);

            this._ButtonFour = new Button();
            this._ButtonFour.Name = ButtonNames.ButtonFour;
            this._ButtonFour.Text = "Button4";
            this._ButtonFour.Size =
                new System.Drawing.Size(this._Panel.Width, this._Panel.Height);

            this._ButtonFive = new Button();
            this._ButtonFive.Name = ButtonNames.ButtonFive;
            this._ButtonFive.Text = "Button5";
            this._ButtonFive.Size =
                new System.Drawing.Size(this._Panel.Width, this._Panel.Height);
            
            this._Panel.Controls.AddRange(new Control[] { this._ButtonOne, 
                this._ButtonTwo, this._ButtonThree, this._ButtonFour, this._ButtonFive });

            this.ScalingPanel();

            return;
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public Button GetButton(string buttonName)
        {
            Button btn = null;

            foreach (Control control in this._Panel.Controls)
            {
                if (control is Button)
                {
                    if (buttonName.Equals(control.Name))
                    {
                        btn = (Button)control;
                        break;
                    }
                }
            }
            return btn;
        }

        private void EventHandler_Panel_Resize(object sender, EventArgs e)
        {
            this.ScalingPanel();
            return;
        }
        /// <summary>
        /// Метод рассчитывает размеры кнопок в зависимости от размеров панели
        /// </summary>
        /// <returns></returns>
        private void ResizeButtons()
        {
            System.Drawing.Size size;

            size = new System.Drawing.Size(this._Panel.Width, this._Panel.Height / 7);

            this._ButtonOne.Size = size;
            this._ButtonTwo.Size = size;
            this._ButtonThree.Size = size;
            this._ButtonFour.Size = size;
            this._ButtonFive.Size = size;
            
            return;
        }
        private void RelocateButtons()
        {            
            // Координата кнопки "1"
            this._ButtonOne.Location = new System.Drawing.Point(0, this._ButtonOne.Height);
            this._ButtonTwo.Location = new System.Drawing.Point(0, this._ButtonOne.Height * 2);
            this._ButtonThree.Location = new System.Drawing.Point(0, this._ButtonOne.Height * 3);
            this._ButtonFour.Location = new System.Drawing.Point(0, this._ButtonOne.Height * 4);
            this._ButtonFive.Location = new System.Drawing.Point(0, this._ButtonOne.Height * 5);

            return;
        }
        /// <summary>
        /// Маштабирует элементы (кнопки) на панели в зависимости от её текущих размеров
        /// </summary>
        private void ScalingPanel()
        {
            this.ResizeButtons();
            this.RelocateButtons();
            return;
        }
        #endregion
    }// End Of Class
}//End Of NameSpace