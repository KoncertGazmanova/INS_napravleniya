namespace INS_napravleniya
{
    partial class mainform
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBoxCanvas = new PictureBox();
            textBoxLabel = new TextBox();
            buttonSave = new Button();
            buttonVisualize = new Button();
            buttonCancel = new Button();
            buttonTrain = new Button();
            buttonRecognize = new Button();
            buttonRefreshDataset = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBoxCanvas).BeginInit();
            SuspendLayout();
            // 
            // pictureBoxCanvas
            // 
            pictureBoxCanvas.Location = new Point(12, 12);
            pictureBoxCanvas.Name = "pictureBoxCanvas";
            pictureBoxCanvas.Size = new Size(300, 300);
            pictureBoxCanvas.TabIndex = 0;
            pictureBoxCanvas.TabStop = false;
            // 
            // textBoxLabel
            // 
            textBoxLabel.Location = new Point(12, 347);
            textBoxLabel.Name = "textBoxLabel";
            textBoxLabel.Size = new Size(300, 23);
            textBoxLabel.TabIndex = 1;
            // 
            // buttonSave
            // 
            buttonSave.Location = new Point(12, 376);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(300, 23);
            buttonSave.TabIndex = 2;
            buttonSave.Text = "Сохранить";
            buttonSave.UseVisualStyleBackColor = true;
            // 
            // buttonVisualize
            // 
            buttonVisualize.Location = new Point(390, 12);
            buttonVisualize.Name = "buttonVisualize";
            buttonVisualize.Size = new Size(241, 23);
            buttonVisualize.TabIndex = 3;
            buttonVisualize.Text = "Просмотр выборки";
            buttonVisualize.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(12, 318);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(300, 23);
            buttonCancel.TabIndex = 4;
            buttonCancel.Text = "Отмена последнего действия";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonTrain
            // 
            buttonTrain.Location = new Point(390, 41);
            buttonTrain.Name = "buttonTrain";
            buttonTrain.Size = new Size(241, 23);
            buttonTrain.TabIndex = 5;
            buttonTrain.Text = "Обучить";
            buttonTrain.UseVisualStyleBackColor = true;
            buttonTrain.Click += ButtonTrain_Click;
            // 
            // buttonRecognize
            // 
            buttonRecognize.Location = new Point(390, 70);
            buttonRecognize.Name = "buttonRecognize";
            buttonRecognize.Size = new Size(241, 23);
            buttonRecognize.TabIndex = 6;
            buttonRecognize.Text = "Распознать";
            buttonRecognize.UseVisualStyleBackColor = true;
            buttonRecognize.Click += ButtonRecognize_Click;
            // 
            // buttonRefreshDataset
            // 
            buttonRefreshDataset.Location = new Point(390, 99);
            buttonRefreshDataset.Name = "buttonRefreshDataset";
            buttonRefreshDataset.Size = new Size(241, 23);
            buttonRefreshDataset.TabIndex = 7;
            buttonRefreshDataset.Text = "Обновить датасет";
            buttonRefreshDataset.UseVisualStyleBackColor = true;
            // 
            // mainform
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1288, 631);
            Controls.Add(buttonRefreshDataset);
            Controls.Add(buttonRecognize);
            Controls.Add(buttonTrain);
            Controls.Add(buttonCancel);
            Controls.Add(buttonVisualize);
            Controls.Add(buttonSave);
            Controls.Add(textBoxLabel);
            Controls.Add(pictureBoxCanvas);
            Name = "mainform";
            Text = "mainform";
            ((System.ComponentModel.ISupportInitialize)pictureBoxCanvas).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBoxCanvas;
        private TextBox textBoxLabel;
        private Button buttonSave;
        private Button buttonVisualize;
        private Button buttonCancel;
        private Button buttonTrain;
        private Button buttonRecognize;
        private Button buttonRefreshDataset;
    }
}
