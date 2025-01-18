namespace INS_napravleniya
{
    partial class DatasetForm
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
            treeViewDataset = new TreeView();
            panelImages = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // treeViewDataset
            // 
            treeViewDataset.Location = new Point(12, 12);
            treeViewDataset.Name = "treeViewDataset";
            treeViewDataset.Size = new Size(121, 621);
            treeViewDataset.TabIndex = 0;
            // 
            // panelImages
            // 
            panelImages.Location = new Point(165, 12);
            panelImages.Name = "panelImages";
            panelImages.Size = new Size(868, 621);
            panelImages.TabIndex = 1;
            // 
            // DatasetForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1086, 783);
            Controls.Add(panelImages);
            Controls.Add(treeViewDataset);
            Name = "DatasetForm";
            Text = "DatasetForm";
            ResumeLayout(false);
        }

        #endregion

        private TreeView treeViewDataset;
        private FlowLayoutPanel panelImages;
    }
}
